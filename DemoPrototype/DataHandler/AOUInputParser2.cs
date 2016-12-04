using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace DemoPrototype
{

    public class AOUInputParser2
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
        public const string tagTempSubTagHeat = "Heat";
        public const string tagTempSubTagBearHot = "BearHot";
        public const string tagTempSubTagCoolant = "Coolant";

        public const string tagTempSubTagReturnForecasted = "RetFor";

        public const string tagPower = "Pow";
        public const string tagValves = "Valves";
        public const string tagSafety = "Safety";
        public const string tagEnergy = "Energy";
        public const string tagUI = "UI";
        public const string tagIMM = "IMM";
        public const string tagMode = "Mode";
        public const string tagSeqState = "Seq";

        public const string tagRetValue = "ret";

        public const string tagLog = "log";
        public const string tagLogSubTagMsg = "Msg";

        #endregion


        public static string GetNextTag(string text, out long time_ms, out string tagContent, out List<string> logs, out string nextLines)
        {
            // Initiate out parameters
            time_ms = 0;
            tagContent = "";
            logs = new List<string>(); // Init log list for log tags and unknown lines

            string tag = ""; // init return value

            // Get next whole line in input text
            string firstLineInText = AOUTagParser.FindNextTextLine(text, out nextLines);

            int tagEndPos; // For next string index after tag pair

            AOUTagParser.ParseLongTime(firstLineInText, out time_ms); // Parse time and convert to ms

            // Get next tag
            if (AOUTagParser.GetTagAndContent(firstLineInText, out tag, out tagContent, out tagEndPos))
            {
                int tagStart = firstLineInText.IndexOf(tag); // Get start position for next tag. Normally index 1
                if (tagStart > 1)
                {
                    logs.Add("Before:"+firstLineInText); // log unknown text before tag
                }
                else if (tagEndPos < firstLineInText.Length)
                {
                    logs.Add("After:" + firstLineInText.Substring(tagEndPos)); // log unknown text after tag pair
                }
            }
            else
            {
                logs.Add(firstLineInText); // No tag. Add to logs
            }

            return tag; // Return next tag found
        }

        public static bool ParseState(string tagText, out AOUStateData stateData)
        {
            long temp; UInt16 tmpval;
            /* 
             <state><Time>19</Time><temp><Heat>34</Heat><Hot>31</Hot><Ret>27</Ret><BuHot>30</BuHot><BuMid>29</BuMid><BuCold>27</BuCold><Cool>32</Cool><Cold>30</Cold><BearHot>0</BearHot>
             <ch9>0</ch9><ch10>0</ch10><ch11>0</ch11><ch12>0</ch12><ch13>0</ch13><ch14>0</ch14><ch15>0</ch15><avg>28</avg></temp></stateData> // Arduino data

             ASCII format from AOU. // MASK_STATE = 2 hex digits MASK (e.g. “3F”), and 2 hex digits STATE (e.g. “12”). Optional tags except Time

             <state><Time>104898416</Time>  // Number of 1/10 second ticks since RESET (32bits unsigned). Not Optional
             <temp>  <Heat>120</Heat><Hot>122</Hot><Ret>68</Ret><BuHot>56</BuHot><BuMid>56</BuMid><BuCold>56</BuCold><Cool>40</Cool><Cold>56</Cold><BearHot>40</BearHot> </temp> // // 16bits signed
                <Pow>127</Pow>           // 8bits unsigned
                <Valves>MMSS</Valves>    // MASK_STATE, Bits: 0/Hot valve, 1/Cold valve, 2/Return valve
                <Energy>MMSS</Energy>    // MASK_STATE, 

                <UI>MMSS</UI>            // MASK_STATE, BUTTON_ONOFF = 0x0001 (Soft on/Off); BUTTON_EMERGENCYOFF = 0x0002 (Hard Off); BUTTON_MANUALOPHEAT = 0x0004 (Forced Heating);
                                         // BUTTON_MANUALOPCOOL = 0x0008 (Forced Cooling); BUTTON_CYCLE = 0x0010 (Forced Cycling); BUTTON_RUN = 0x0020 (Run with IMM)

                <IMM>MMSS</IMM>          // MASK_STATE, IMM_OutIMMError = 0x01; IMM_OutIMMBlockInject = 0x02; IMM_OutIMMBlockOpen = 0x04; IMM_InIMMStop = 0x08
                                         // IMM_InCycleAuto = 0x10; IMM_InIMMInjecting = 0x20; IMM_InIMMEjecting = 0x40; IMM_InIMMToolClosed = 0x80

                <Mode>MMSS</Mode>        // MASK_STATE, <Mode>1</Mode>(int); HT_STATE_INVALID = -999; HT_STATE_COLD = -1; HT_STATE_UNKNOWN = 0; HT_STATE_HOT = 1
                <Seq>117</Seq>           // 
             </state>

             <state><Time>4711</Time>
                <Valves>0101</Valves>      // Example Hot feed valve “on” (i.e. feeds hot tempering fluid)
             </state>
             <state><Time>4721</Time>  // One second (or 10 x 1/10 second) later
                <Valves>0100</Valves>       // Hot feed valve “off” (i.e. stopped feeding hot tempering fluid)
             </state>


 */

            stateData.time_sek_x_10_of_hour = 0;
            stateData.time_hours = 0;

            stateData.coldTankTemp = AOUDataTypes.UInt16_NaN;
            stateData.hotTankTemp = AOUDataTypes.UInt16_NaN;
            stateData.retTemp = UInt16.MaxValue;
            stateData.RetForTemp = Int16.MaxValue;

            stateData.coolerTemp = AOUDataTypes.UInt16_NaN;
            stateData.heaterTemp = AOUDataTypes.UInt16_NaN;

            stateData.bufColdTemp = AOUDataTypes.UInt16_NaN;
            stateData.bufMidTemp = AOUDataTypes.UInt16_NaN;
            stateData.bufHotTemp = AOUDataTypes.UInt16_NaN;

            stateData.seqState = AOUDataTypes.UInt16_NaN;

            stateData.BearHot = AOUDataTypes.UInt16_NaN;
            stateData.Power = AOUDataTypes.UInt16_NaN;
            stateData.Energy = AOUDataTypes.UInt16_NaN;

            stateData.IMM = AOUDataTypes.UInt16_NaN;
            stateData.Valves = AOUDataTypes.UInt16_NaN;
            stateData.Safety = AOUDataTypes.UInt16_NaN;
            stateData.Mode = Int16.MaxValue;
            stateData.UIButtons = AOUDataTypes.UInt16_NaN;

            AOUTagParser.ParseWordTime_sek_x_10(tagText, out stateData.time_hours, out stateData.time_sek_x_10_of_hour);

            AOUTagParser.ParseWord(tagTempSubTagHot, tagText, out stateData.hotTankTemp);
            AOUTagParser.ParseWord(tagTempSubTagCold, tagText, out stateData.coldTankTemp);
            AOUTagParser.ParseWord(tagTempSubTagRet, tagText, out stateData.retTemp);
            AOUTagParser.ParseWord(tagTempBuCold, tagText, out stateData.bufColdTemp);
            AOUTagParser.ParseWord(tagTempBuMid, tagText, out stateData.bufMidTemp);
            AOUTagParser.ParseWord(tagTempBuHot, tagText, out stateData.bufHotTemp);

            AOUTagParser.ParseWord(tagTempSubTagCool, tagText, out stateData.coolerTemp);
            AOUTagParser.ParseWord(tagTempSubTagCoolant, tagText, out stateData.coolantTemp);
            AOUTagParser.ParseWord(tagTempSubTagHeat, tagText, out stateData.heaterTemp);

            AOUTagParser.ParseWord(tagTempSubTagReturnForecasted, tagText, out stateData.RetForTemp);


            AOUTagParser.ParseWord(tagTempSubTagBearHot, tagText, out stateData.BearHot);


            AOUTagParser.ParseWord(tagSeqState, tagText, out stateData.seqState);

            if (AOUTagParser.ParseWord(tagPower, tagText, out tmpval))
            {
                stateData.Power = tmpval;
            }


            if (AOUTagParser.ParseMMSS(tagValves, tagText, out tmpval))
            {
                stateData.Valves = tmpval;
            }

            if (AOUTagParser.ParseMMSS(tagSafety, tagText, out tmpval))
            {
                stateData.Safety = tmpval;
            }

            if (AOUTagParser.ParseMMSS(tagEnergy, tagText, out tmpval))
            {
                stateData.Energy = tmpval;
            }

            if (AOUTagParser.ParseMMSS(tagUI, tagText, out tmpval))
            {
                stateData.UIButtons = tmpval;
            }

            if (AOUTagParser.ParseMMSS(tagIMM, tagText, out tmpval))
            {
                stateData.IMM = tmpval;
            }

            if (AOUTagParser.ParseMMSS(tagMode, tagText, out tmpval))
            {
                stateData.Mode = (Int16)tmpval;
            }
            else if (AOUTagParser.ParseLong(tagMode, tagText, out temp))
            {
                stateData.Mode = (Int16)temp;
            }
            return true;
        }

        public static bool ParseLog(string tagText, out string logMsg, out int prio, out int pid)
        {
            logMsg = "-";
            prio = 2;
            pid = 0;
            // Example: "<log><Time>94962045</Time><Msg>Setup AOU version 1.1 ready (Plastics Unbound Ltd, Cyprus)</Msg></log>";
            return AOUTagParser.ParseString(tagLogSubTagMsg, tagText, out logMsg);
        }

    }
}
