using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace DemoPrototype
{

    public class AOUInputParser
    {
        #region Tag Constants
        public const string tagSubTagTime = "Time"; //


        public const string tagState = "state";
        public const string tagTemp = "temp";
        public const string tagTempBuHot = "BuHot";
        public const string tagTempBuMid = "BuMid";
        public const string tagTempBuCold = "BuCold";
        public const string tagTempSubTagHot = "Hot";
        public const string tagTempSubTagCold = "Cold";
        public const string tagTempSubTagRet = "Ret";
        public const string tagTempSubTagCool = "Cool";
        public const string tagTempSubTagBearHot = "BearHot";

        public const string tagPower = "Pow";
        public const string tagValves = "Valves";
        public const string tagEnergy = "Energy";
        public const string tagUI = "UI";
        public const string tagIMM = "IMM";
        public const string tagMode = "Mode";
        public const string tagSequenceNumber = "Seq";
        #endregion


        #region OldTags
        /* old tags */
        public const string tagTemperature = "temperature";
        public const string tagTempSubTagSpare1 = "Spare1";

        public const string tagFeeds = "feeds";
        public const string tagFeedsHot = "Hot";
        public const string tagFeedsCold = "Cold";
        public const string tagFeedsPrev = "Prev";
        public const string tagFeedsNew = "New";

        public const string tagSequence = "seq";
        public const string tagSeqSubTagState = "State";
        public const string tagSeqSubTagCycle = "Cycle";
        public const string tagSeqSubTagDesc = "Descr";
        public const string tagSeqSubTagLeave = "Leave";

        public const string tagValvesOld = "valves";
        public const string tagValvesSubTagRet = "Ret";
        public const string tagValvesSubTagRetPrev = "Prev";
        public const string tagValvesSubTagRetNew = "New";

        public const string tagIMMOld = "imm";
        public const string tagIMMSubTagSetIMMError = "SetIMMError";
        public const string tagIMMSubTagIMMBlockInject = "SetIMMBlockInject";
        public const string tagIMMSubTagIMMBlockOpen = "SetIMMBlockOpen";
        public const string tagIMMSubTagIMMStop = "IMMStop";
        public const string tagIMMSubTagCycleAuto = "CycleAuto";
        public const string tagIMMSubTagIMMInjecting = "IMMInjecting";
        public const string tagIMMSubTagIMMEjecting = "IMMEjecting";
        public const string tagIMMSubTagIMMToolClosed = "IMMToolClosed";

        public const string tagLevels = "levels";
        public const string tagLevelsSubTagCold = "Cold";
        public const string tagLevelsSubTagHot = "Hot";
        public const string tagLevelsSubTagPrev = "Prev";
        public const string tagLevelsSubTagNew = "New";

        public const string tagLog = "log";
        public const string tagLogSubTagMsg = "Msg";
        #endregion

        public static bool ValidPowerTag(string tag)
        {
            if (tag == tagTemperature || tag == tagFeeds || tag == tagSequence ||
                tag == tagLevels || tag == tagValves || tag == tagValves ||

                tag == tagState // Last tag to have all Data
                )
            {
                return true;
            }
            return false;
        }

        #region Common
        public static string GetNextTag(string text, out string tagContent, out List<string> logs, out int numHandled)
        {
            Regex rTag = new Regex("<[a-zA-Z]+>");
            logs = new List<string>();
            tagContent = "";
            numHandled = 0;

            int lastTextPos = 0;
            bool eot = false;
            string tag = "";
            int tlen = text.Length;
            string textLine = "";
            do
            {
                tag = "";
                textLine = "";
                if (text.IndexOf("\r\n") > 0)
                {
                    int endPos = text.IndexOf("\r\n", lastTextPos + 1);
                    if (endPos >= 0)
                    { 
                        textLine = text.Substring(lastTextPos, endPos - lastTextPos).Trim();
                        lastTextPos = endPos + 1;
                    }
                    else
                    {
                        lastTextPos = lastTextPos + 2;
                    }
                }
                /* If only LF*/
                else if (text.IndexOf("\n") > 0)
                {
                    if ((lastTextPos+1) < tlen)
                    { 
                        int endPos = text.IndexOf("\n", lastTextPos + 1);
                        if (endPos >= 0)
                        {
                            textLine = text.Substring(lastTextPos, endPos - lastTextPos).Trim();
                            lastTextPos = endPos + 1;
                        }
                        else
                        {
                            lastTextPos = lastTextPos + 1;
                        }
                    }
                    else
                    {
                        int err = lastTextPos;
                    }
                }

                if (!eot && textLine.Length > 0)
                { 
                    Match m = rTag.Match(textLine, 0);
                    if (m.Success)
                    {
                        tag = m.Groups[0].Value.Substring(1, m.Groups[0].Value.Length - 2);
                        int tagEndPos = 0;
                        FindTagAndExtractText(tag, textLine, out tagContent, out tagEndPos);
                        break;
                    }
                    else
                    {
                        logs.Add(textLine);
                        textLine = "";
                    }
                }
                else
                {
                    bool dbg = true;
                }
            } while (tag == "" && textLine.Length > 0);
            numHandled = lastTextPos;
            return tag;
        }

        public static bool FindTag(string tag, string textLine)
        {
            string startTag = "<" + tag + ">";
            string endTag = "</" + tag + ">";
            int pos1 = textLine.IndexOf(startTag);
            int pos2 = textLine.IndexOf(endTag);

            return (pos1 != -1 && pos2 != -1);
        }

        public static bool FindTagAndExtractText(string tag, string textLine, out string tagText, out int endPos)
        {
            string startTag = "<" + tag + ">";
            string endTag = "</" + tag + ">";
            int pos1 = textLine.IndexOf(startTag);
            int pos2 = textLine.IndexOf(endTag);
            if (pos1 != -1 && pos2 != -1)
            {
                pos1 += startTag.Length;
                tagText = textLine.Substring(pos1, pos2 - pos1);
                endPos = pos2 + endTag.Length;
                tagText = tagText.Trim();
                return true;
            }
            else
            {
                tagText = "";
                endPos = 0;
                return false;
            }

        }

        public static bool ParseString(string tagText, string textline, out string text)
        {
            int endpos = 0;
            return FindTagAndExtractText(tagText, textline, out text, out endpos);
        }

        public static bool ParseWord(string tag, string textline, out UInt16 value)
        {
            double dbl = double.NaN;
            if (Parsedouble(tag, textline, out dbl))
            {
                value = (UInt16)Math.Round(dbl);
                return true;
            }
            value = UInt16.MaxValue;
            return false;
        }

        public static bool Parsedouble(string tag, string textline, out double value)
        {
            int endpos = 0;
            string tagValue = "";

            if (FindTagAndExtractText(tag, textline, out tagValue, out endpos))
            {
                tagValue.Replace(',', '.');
                return double.TryParse(tagValue, out value);
            }
            value = 0;
            return false;
        }

        public static bool ParseLong(string tagText, string textline, out long value)
        {
            int endpos = 0;
            if (FindTagAndExtractText(tagText, textline, out tagText, out endpos) &&
                long.TryParse(tagText, out value))
            {
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public static Byte LowByte(UInt16 word)
        {
            return (Byte)(word & 0xff);
        }

        public static Byte HighByte(UInt16 word)
        {
            return (Byte)(word >> 8);
        }

        #endregion


        #region ParseXML
        public static bool ParseTime_ms(string textline, out TimeSpan time)
        {

            long time_ms = 0;
            if (ParseLong(tagSubTagTime, textline, out time_ms))
            {
                time = TimeSpan.FromMilliseconds(time_ms);
                return true;
            }
            else
            {
                time = TimeSpan.Zero;
                return false;
            }
        }

        public static bool ParseWordTime(string textline, out UInt16 time_min_of_week, out UInt16 time_ms_of_min) 
        {
            long time_ms = 0;
            if (ParseLong(tagSubTagTime, textline, out time_ms))
            {
                if (time_ms > 283200)
                {
                    long t2 = time_ms;
                }
                AOUDataTypes.TimeMsToAOUModelTime(time_ms, out time_min_of_week, out time_ms_of_min);
                return true;
            }
            else
            {
                time_min_of_week = 0;
                time_ms_of_min = 0;
                return false;
            }
        }

        public static bool ParseWordTime10(string textline, out UInt16 time_hours, out UInt16 time_dec_sek)
        {
            long time_ms = 0;
            if (ParseLong(tagSubTagTime, textline, out time_ms))
            {
                AOUDataTypes.TimeDecSecToAOUModelTime(time_ms, out time_hours, out time_dec_sek);
                return true;
            }
            else
            {
                time_dec_sek = 0;
                time_hours = 0;
                return false;
            }
        }

        public static bool ParseLongTime(string textline, out long time_ms) // Not to be misunderstood
        {
            return ParseLong(tagSubTagTime, textline, out time_ms);
        }

        public static List<string> ParseBetweenTagsMessages(string tagText)
        {
            List<string> logs = new List<string>();

            long time_ms = 0;
            ParseLongTime(tagText, out time_ms); // <Time> value before message

            var r = new Regex("<\\/([a-zA-Z]+)>([^<]+)<"); // match "</tag>message<"
            var matches = r.Matches(tagText, 0);
            if (matches.Count > 0)
            {
                foreach (var match in matches)
                {
                    string s = match.ToString();
                    int n = s.IndexOf('>');
                    string tagBefore = s.Substring(2, n - 2);
                    string between = s.Substring(n + 1, s.Length - n - 2).Trim();
                    if (between.Length > 2)
                    {
                        logs.Add(between); // Todo time
                    }
                }
            }
            return logs;
        }


        // New 
        public static bool ParseState(string tagText, out AOUStateData stateData)
        {
            /* <state><Time>19</Time><temp><Heat>34</Heat><Hot>31</Hot><Ret>27</Ret>
            <BuHot>30</BuHot><BuMid>29</BuMid><BuCold>27</BuCold>
            <Cool>32</Cool><Cold>30</Cold><BearHot>0</BearHot>

            <ch9>0</ch9><ch10>0</ch10><ch11>0</ch11><ch12>0</ch12><ch13>0</ch13><ch14>0</ch14><ch15>0</ch15><avg>28</avg></temp>

            1. ASCII format from AOU
            Tagged telegram format. All sub tag pairs except “Time” are optional.
 
            <state><Time>104898416</Time>  // Number of 1/10 second ticks since RESET (32bits unsigned)
               <temp>  // 16bits signed
            <Heat>120</Heat><Hot>122</Hot><Ret>68</Ret><BuHot>56</BuHot><BuMid>56</BuMid><BuCold>56</BuCold><Cool>40</Cool><Cold>56</Cold><BearHot>40</BearHot>
               </temp>
               <Pow>127</Pow> // 8bits unsigned
               <Valves>MMSS</Valves>     // 2 hex digits MASK (e.g. “3F”), and 2 hex digits STATE (e.g. “12”). Bits: 0/Hot valve, 1/Cold valve, 2/Return valve
               <Energy>MMSS</Energy>    // 2 hex digits MASK (e.g. “3F”), and 2 hex digits STATE (e.g. “12”).
               <UI>MMSS</UI>                   // 2 hex digits MASK (e.g. “3F”), and 2 hex digits STATE (e.g. “12”).
               <IMM>MMSS</IMM>          // 2 hex digits MASK (e.g. “3F”), and 2 hex digits STATE (e.g. “12”).
               <Mode>MMSS</Mode>      // 2 hex digits MASK (e.g. “3F”), and 2 hex digits STATE (e.g. “12”).
               <Seq>117</Seq>
            </state>
 
            Example using this spec:
            <state><Time>4711</Time>
               <Valves>0101</Valves>      // Hot feed valve “on” (i.e. feeds hot tempering fluid)
            </state>
 
            <state><Time>4721</Time>  // One second (or 10 x 1/10 second) later
               <Valves>0100</Valves>       // Hot feed valve “off” (i.e. stopped feeding hot tempering fluid)
            </state>

f. Tool tempering mode
// <state><Time>104898416</Time>
//…
//   <Mode>1</Mode> (int), for modes see below
//
#define HT_STATE_INVALID -999
#define HT_STATE_COLD -1
#define HT_STATE_UNKNOWN 0
#define HT_STATE_HOT 1
 
g. Knapparnas körläge
// <state><Time>104898416</Time>
//…
//   <UI>MMSS</UI> (hex) MM=8bit mask, SS=8bits , for buttons see below
//
#define BUTTON_ONOFF                  0x0001  // Soft on/Off
#define BUTTON_EMERGENCYOFF  0x0002  // Hard Off
 
#define BUTTON_MANUALOPHEAT  0x0004  // Forced Heating
#define BUTTON_MANUALOPCOOL  0x0008  // Forced Cooling
 
#define BUTTON_CYCLE                      0x0010  // Forced Cycling
#define BUTTON_RUN                         0x0020  // Run with IMM
 
 
h. Signals AOU/IMM interaction
// <state><Time>104898416</Time>
//…
//   <IMM>MMSS</IMM> (hex)  MM=8bit mask, SS=8bits for signals (see below)
//
#define IMM_OutIMMError       0x01
#define IMM_OutIMMBlockInject 0x02
#define IMM_OutIMMBlockOpen   0x04
#define IMM_InIMMStop         0x08
#define IMM_InCycleAuto       0x10
#define IMM_InIMMInjecting    0x20
#define IMM_InIMMEjecting     0x40
#define IMM_InIMMToolClosed   0x80
            */
            stateData.time_min_of_week = 0;
            stateData.time_ms_of_min = 0;
            stateData.coldTankTemp = AOUDataTypes.UInt16_NaN;
            stateData.hotTankTemp = AOUDataTypes.UInt16_NaN;
            stateData.retTemp = AOUDataTypes.UInt16_NaN;
            stateData.coolerTemp = AOUDataTypes.UInt16_NaN;

            stateData.bufCold = AOUDataTypes.UInt16_NaN;
            stateData.bufMid = AOUDataTypes.UInt16_NaN;
            stateData.bufHot = AOUDataTypes.UInt16_NaN;

            stateData.Energy = 0;
            stateData.IMM = 0;// ToDo: Rembeber old value
            stateData.Mode = 0;// ToDo: Rembeber old value
            stateData.Power = 0;
            stateData.SeqNr = 0;
            stateData.UI = 0;
            stateData.Valves = 0; // ToDo: Rembeber old value

            ParseWordTime10(tagText, out stateData.time_min_of_week, out stateData.time_ms_of_min);
            ParseWord(tagTempSubTagHot, tagText, out stateData.hotTankTemp);
            ParseWord(tagTempSubTagCold, tagText, out stateData.coldTankTemp);
            ParseWord(tagTempSubTagRet, tagText, out stateData.retTemp);
            ParseWord(tagTempSubTagCool, tagText, out stateData.coolerTemp);
            ParseWord(tagTempBuCold, tagText, out stateData.bufCold);
            ParseWord(tagTempBuMid, tagText, out stateData.bufMid);
            ParseWord(tagTempBuHot, tagText, out stateData.bufHot);
            // ParseWord(tagTempSubTagBearHot, tagText, out todoValue);
            ParseWord(tagPower, tagText, out stateData.Power);
            ParseWord(tagValves, tagText, out stateData.Valves);
            ParseWord(tagEnergy, tagText, out stateData.Energy);
            ParseWord(tagUI, tagText, out stateData.UI);
            ParseWord(tagIMM, tagText, out stateData.IMM);
            ParseWord(tagMode, tagText, out stateData.Mode);
            ParseWord(tagSequenceNumber, tagText, out stateData.SeqNr);


            return true;
        }

        public static bool ParseLog(string tagText, out long time_ms, out string logMsg)
        {
            time_ms = 0;
            logMsg = "-";
            // textLine = "<log><Time>94962045</Time><Msg>Setup AOU version 1.1 ready (Plastics Unbound Ltd, Cyprus)</Msg></log>";
            return ParseLongTime(tagText, out time_ms) && ParseString(tagLogSubTagMsg, tagText, out logMsg);
        }
        #endregion

        #region ParseXMLOld
        public static bool ParseTemperature(string tagText, out AOUTemperatureData tempData)
        {
            // textLine = "<temperature><Time>104898416</Time><Hot>122</Hot><Cold>56</Cold><Ret>68</Ret><Cool>40</Cool></temperature>";

            tempData.time_min_of_week = 0;
            tempData.time_ms_of_min = 0;
            tempData.coldTankTemp = AOUDataTypes.UInt16_NaN;
            tempData.hotTankTemp = AOUDataTypes.UInt16_NaN;
            tempData.retTemp = AOUDataTypes.UInt16_NaN;
            tempData.coolerTemp = AOUDataTypes.UInt16_NaN;

            return ParseWordTime(tagText, out tempData.time_min_of_week, out tempData.time_ms_of_min) &&
                    ParseWord(tagTempSubTagHot, tagText, out tempData.hotTankTemp) &&
                    ParseWord(tagTempSubTagCold, tagText, out tempData.coldTankTemp) &&
                    ParseWord(tagTempSubTagRet, tagText, out tempData.retTemp) &&
                    ParseWord(tagTempSubTagCool, tagText, out tempData.coolerTemp);
        }

        public static bool ParseValves(string tagText, out AOUValvesData valvesData)
        {
            // textLine = "<valves><Time>104903816</Time><Ret><Prev>93</Prev><New>80</New></Ret></valves>";

            valvesData.time_min_of_week = 0;
            valvesData.time_ms_of_min = 0;
            valvesData.prevValveReturnTemp = AOUDataTypes.UInt16_NaN;
            valvesData.newValveReturnTemp = AOUDataTypes.UInt16_NaN;

            // ToDo Check tagValvesSubTagRet
            return ParseWordTime(tagText, out valvesData.time_min_of_week, out valvesData.time_ms_of_min) &&
                   ParseWord(tagValvesSubTagRetPrev, tagText, out valvesData.prevValveReturnTemp) &&
                   ParseWord(tagValvesSubTagRetNew, tagText, out valvesData.newValveReturnTemp);
        }

        public static bool ParseHotFeed(string tagText, out AOUHotFeedData feedData)
        {
            // textLine = "<feeds><Time>104894473</Time><Hot><Prev>60</Prev></New>63,75</New></Hot></feeds>";
            // textLine = "<feeds><Time>104878268</Time><Cold><Prev>65</Prev></New>60</New></Cold></feeds>";
            feedData.time_min_of_week = 0;
            feedData.time_ms_of_min = 0;
            feedData.prevFeedTemp = AOUDataTypes.UInt16_NaN;
            feedData.newFeedTemp = AOUDataTypes.UInt16_NaN;

            return (ParseWordTime(tagText, out feedData.time_min_of_week, out feedData.time_ms_of_min) &&
                ParseWord(tagFeedsPrev, tagText, out feedData.prevFeedTemp) &&
                ParseWord(tagFeedsNew, tagText, out feedData.newFeedTemp));
        }

        public static bool ParseColdFeed(string tagText, out AOUColdFeedData feedData)
        {
            // textLine = "<feeds><Time>104894473</Time><Hot><Prev>60</Prev></New>63,75</New></Hot></feeds>";
            // textLine = "<feeds><Time>104878268</Time><Cold><Prev>65</Prev></New>60</New></Cold></feeds>";
            feedData.time_min_of_week = 0;
            feedData.time_ms_of_min = 0;
            feedData.prevFeedTemp = AOUDataTypes.UInt16_NaN;
            feedData.newFeedTemp = AOUDataTypes.UInt16_NaN;

            return (ParseWordTime(tagText, out feedData.time_min_of_week, out feedData.time_ms_of_min) &&
                ParseWord(tagFeedsPrev, tagText, out feedData.prevFeedTemp) &&
                ParseWord(tagFeedsNew, tagText, out feedData.newFeedTemp));
        }

        public static bool ParseIMM(string tagText, out AOUIMMData immData)
        {
            string tempText = "";
            UInt16 tempValue = 0;

            immData.time_min_of_week = 0;
            immData.time_ms_of_min = 0;
            immData.imm_setting_val = AOUDataTypes.UInt16_NaN;
            immData.imm_setting_type = (UInt16)AOUDataTypes.IMMSettings.Nothing;

            int endpos = 0;
            if (FindTagAndExtractText(tagIMM, tagText, out tempText, out endpos) &&
               ParseWordTime(tempText, out immData.time_min_of_week, out immData.time_ms_of_min))
            {
                if (ParseWord(tagIMMSubTagSetIMMError, tempText, out tempValue))
                {
                    immData.imm_setting_type = (UInt16)AOUDataTypes.IMMSettings.OutIMMError;
                    immData.imm_setting_val = tempValue;
                }
                else if (ParseWord(tagIMMSubTagCycleAuto, tempText, out tempValue))
                {
                    immData.imm_setting_type = (UInt16)AOUDataTypes.IMMSettings.InCycleAuto;
                    immData.imm_setting_val = tempValue;
                }
                else if (ParseWord(tagIMMSubTagIMMEjecting, tempText, out tempValue))
                {
                    immData.imm_setting_type = (UInt16)AOUDataTypes.IMMSettings.InIMMEjecting;
                    immData.imm_setting_val = tempValue;
                }
                else if (ParseWord(tagIMMSubTagIMMInjecting, tempText, out tempValue))
                {
                    immData.imm_setting_type = (UInt16)AOUDataTypes.IMMSettings.InIMMInjecting;
                    immData.imm_setting_val = tempValue;
                }
                else if (ParseWord(tagIMMSubTagIMMToolClosed, tempText, out tempValue))
                {
                    immData.imm_setting_type = (UInt16)AOUDataTypes.IMMSettings.InIMMToolClosed;
                    immData.imm_setting_val = tempValue;
                }
                else if (ParseWord(tagIMMSubTagIMMStop, tempText, out tempValue))
                {
                    immData.imm_setting_type = (UInt16)AOUDataTypes.IMMSettings.InIMMStop;
                    immData.imm_setting_val = tempValue;
                }
                else if (ParseWord(tagIMMSubTagIMMBlockInject, tempText, out tempValue))
                {
                    immData.imm_setting_type = (UInt16)AOUDataTypes.IMMSettings.OutIMMBlockInject;
                    immData.imm_setting_val = tempValue;
                }
                else if (ParseWord(tagIMMSubTagIMMBlockOpen, tempText, out tempValue))
                {
                    immData.imm_setting_type = (UInt16)AOUDataTypes.IMMSettings.OutIMMBlockOpen;
                    immData.imm_setting_val = tempValue;
                }
            }

            return immData.imm_setting_type != (UInt16)AOUDataTypes.IMMSettings.Nothing;
        }

        public static bool ParseHotLevel(string tagText, out AOUHotLevelData levelData)
        {
            // textLine = "<feeds><Time>104894473</Time><Hot><Prev>60</Prev></New>63,75</New></Hot></feeds>";
            // textLine = "<feeds><Time>104878268</Time><Cold><Prev>65</Prev></New>60</New></Cold></feeds>";
            levelData.time_min_of_week = 0;
            levelData.time_ms_of_min = 0;
            levelData.prevLevel = AOUDataTypes.UInt16_NaN;
            levelData.newLevel = AOUDataTypes.UInt16_NaN;


            return ParseWordTime(tagText, out levelData.time_min_of_week, out levelData.time_ms_of_min) &&
                    ParseWord(tagLevelsSubTagPrev, tagText, out levelData.prevLevel) &&
                    ParseWord(tagLevelsSubTagNew, tagText, out levelData.newLevel);
        }

        public static bool ParseColdLevel(string tagText, out AOUColdLevelData levelData)
        {
            // textLine = "<feeds><Time>104894473</Time><Hot><Prev>60</Prev></New>63,75</New></Hot></feeds>";
            // textLine = "<feeds><Time>104878268</Time><Cold><Prev>65</Prev></New>60</New></Cold></feeds>";
            levelData.time_min_of_week = 0;
            levelData.time_ms_of_min = 0;
            levelData.prevLevel = AOUDataTypes.UInt16_NaN;
            levelData.newLevel = AOUDataTypes.UInt16_NaN;

            return ParseWordTime(tagText, out levelData.time_min_of_week, out levelData.time_ms_of_min) &&
                    ParseWord(tagLevelsSubTagPrev, tagText, out levelData.prevLevel) &&
                    ParseWord(tagLevelsSubTagNew, tagText, out levelData.newLevel);
        }

        public static bool ParseSeqState(string tagContent, out UInt16 state)
        {
            string stateStr;
            state = 0;
            if (ParseString(tagSeqSubTagState, tagContent, out stateStr))
            {
                state = (UInt16)AOUDataTypes.StringToStateType(stateStr);
                return true;
            }
            return false;
        }

        public static bool ParseSequence(string tagText, out AOUSeqData seqData)
        {
            seqData.time_min_of_week = 0;
            seqData.time_ms_of_min = 0;
            seqData.state = (UInt16)AOUDataTypes.StateType.NOTHING;
            seqData.cycle = 0;

            return ParseWordTime(tagText, out seqData.time_min_of_week, out seqData.time_ms_of_min) &&
                   ParseSeqState(tagText, out seqData.state) &&
                   ParseWord(tagSeqSubTagCycle, tagText, out seqData.cycle);
        }
        #endregion

        #region CreateXML
        public static string CreateTempXmlString(long time_ms, AOUTemperatureData tempData)
        {
            return String.Format("<{0}><{1}>{6}</{1}><{2}>{7}</{2}><{3}>{8}</{3}><{4}>{9}</{4}><{5}>{10}</{5}></{0}>",
                                    tagTemperature,
                                    tagSubTagTime, tagTempSubTagHot, tagTempSubTagCold, tagTempSubTagRet, tagTempSubTagCool, // 1 - 5
                                    time_ms,
                                    tempData.hotTankTemp, tempData.coldTankTemp, tempData.retTemp, tempData.coolerTemp); // 6 - 10
        }

        public static string CreateSeqXmlString(long time_ms, AOUSeqData tempData)
        {
            return String.Format("<{0}><{1}>{6}</{1}><{2}>{7}</{2}><{3}>{8}</{3}><{4}>{9}</{4}><{5}>{10}</{5}></{0}>",
                                    tagSequence, tagSubTagTime,
                                    tagSeqSubTagState, tagSeqSubTagCycle, tagSeqSubTagDesc, tagSeqSubTagLeave, // 2 - 5
                                    time_ms, tempData.state, tempData.cycle, "Desc...", "Leave..."); // 6 - 10
        }

        /*
        public static string CreateIMMXmlString(long time_ms, AOUIMMData tempData)
        {
            string IMMSettingsTag = null;

            switch ((AOUDataTypes.IMMSettings)tempData.imm_setting_type)
            {
                case AOUDataTypes.IMMSettings.CycleAuto: IMMSettingsTag = tagIMMSubTagCycleAuto; break;
                case AOUDataTypes.IMMSettings.IMMEjecting: IMMSettingsTag = tagIMMSubTagIMMEjecting; break;
                case AOUDataTypes.IMMSettings.IMMInjecting: IMMSettingsTag = tagIMMSubTagIMMInjecting; break;
                case AOUDataTypes.IMMSettings.IMMStop: IMMSettingsTag = tagIMMSubTagIMMStop; break;
                case AOUDataTypes.IMMSettings.IMMToolClosed: IMMSettingsTag = tagIMMSubTagIMMToolClosed; break;
                case AOUDataTypes.IMMSettings.SetIMMBlockInject: IMMSettingsTag = tagIMMSubTagIMMBlockInject; break;
                case AOUDataTypes.IMMSettings.SetIMMBlockOpen: IMMSettingsTag = tagIMMSubTagIMMBlockOpen; break;
                case AOUDataTypes.IMMSettings.SetIMMError: IMMSettingsTag = tagIMMSubTagSetIMMError;  break;
                default: IMMSettingsTag = tagIMMSubTagSetIMMError; break;
            }

            return String.Format("<{0}><{1}>{3}</{1}><{2}>{4}</{2}></{0}>",
                                    tagIMM, tagSubTagTime, IMMSettingsTag, // 0 - 2
                                    time_ms, tempData.imm_setting_val); // 3 - 4
        }
        */
        public static string CreateHotFeedXmlString(long time_ms, AOUHotFeedData data)
        {
            return String.Format("<{0}><{1}>{5}</{1}><{2}> <{3}>{6}</{3}><{4}>{7}</{4}> </{2}></{0}>",
                                    tagFeeds, tagSubTagTime, tagFeedsHot, // 0 - 2
                                    tagFeedsPrev, tagFeedsNew, // 3, 4
                                    time_ms, data.prevFeedTemp, data.newFeedTemp); // 5, 6, 7
        }

        public static string CreateColdFeedXmlString(long time_ms, AOUColdFeedData data)
        {
            return String.Format("<{0}><{1}>{5}</{1}><{2}> <{3}>{6}</{3}><{4}>{7}</{4}> </{2}></{0}>",
                                    tagFeeds, tagSubTagTime, tagFeedsCold, // 0 - 2
                                    tagFeedsPrev, tagFeedsNew, // 3, 4
                                    time_ms, data.prevFeedTemp, data.newFeedTemp); // 5, 6, 7
        }

        public static string CreateHotLevelXmlString(long time_ms, AOUHotLevelData data)
        {
            return String.Format("<{0}><{1}>{5}</{1}><{2}> <{3}>{6}</{3}><{4}>{7}</{4}> </{2}></{0}>",
                                    tagLevels, tagSubTagTime, tagLevelsSubTagHot, // 0 - 2
                                    tagLevelsSubTagPrev, tagLevelsSubTagNew, // 3, 4
                                    time_ms, data.prevLevel, data.newLevel); // 5, 6, 7
        }

        public static string CreateColdLevelXmlString(long time_ms, AOUColdLevelData data)
        {
            return String.Format("<{0}><{1}>{5}</{1}><{2}> <{3}>{6}</{3}><{4}>{7}</{4}> </{2}></{0}>",
                                    tagFeeds, tagSubTagTime, tagLevelsSubTagCold, // 0 - 2
                                    tagFeedsPrev, tagFeedsNew, // 3, 4
                                    time_ms, data.prevLevel, data.newLevel); // 5, 6, 7
        }

        public static string CreateValvesXmlString(long time_ms, AOUValvesData data)
        {
            return String.Format("<{0}><{1}>{5}</{1}><{2}>{7}</{2}><{3}>{6}</{3}><{4}>{7}</{4}></{2}></{0}>",
                                    tagValves, tagSubTagTime, tagValvesSubTagRet, // 0 - 2
                                    tagValvesSubTagRetPrev, tagValvesSubTagRetNew, // 3 - 4
                                    time_ms, data.prevValveReturnTemp, data.newValveReturnTemp); // 5 - 7
        }

        public static string CreateLogXmlString(long time_ms, AOULogMessage data)
        {
            return String.Format("<{0}><{1}>{3}</{1}><{2}>{4}</{2}></{0}>",
                                    tagLog, tagSubTagTime, tagLogSubTagMsg, // 0 - 2
                                    time_ms, data.message); // 3 - 4
        }

        #endregion

    }
}
