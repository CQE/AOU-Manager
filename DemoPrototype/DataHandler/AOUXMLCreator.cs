using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPrototype
{

    public class AOUXMLCreator
    {
        public static string CreateTag(string tag, string content)
        {
            return String.Format("<{0}>{1}</{0}>", tag, content);
        }

        public static string CreateTag(string tag, uint content)
        {
            return String.Format("<{0}>{1}</{0}>", tag, content);
        }

        public static string CreateTag(string tag, int content)
        {
            return String.Format("<{0}>{1}</{0}>", tag, content);
        }

        private static uint Time2WordToMs(UInt16 time_hours, UInt16 time_sek_x_10_of_hour)
        {
            return (uint)time_hours * 36000 + time_sek_x_10_of_hour;
        }

        public static string CreateTimeXmlString(uint time)
        {
            return CreateTag(AOUInputParser.tagSubTagTime, time.ToString());
        }

        public static string CreateTimeXmlString(UInt16 time_hours, UInt16 time_sek_x_10_of_hour)
        {

            return CreateTimeXmlString(Time2WordToMs(time_hours, time_sek_x_10_of_hour));
        }

        public static string CreateStateXmlString(uint time, string content)
        {
            return CreateTag(AOUInputParser.tagState, CreateTimeXmlString(time) + content);
        }

        public static string CreateValvesXmlString(uint time, uint hotValve, uint coldValve, uint retValve)
        {
            string valvesStr = "7F";
            int valves = 0;
            if (hotValve != 0) valves += 1;
            if (coldValve != 0) valves += 2;
            if (retValve != 0) valves += 4;
            valvesStr += String.Format("{0:X02}", valves);
            return CreateStateXmlString(time, CreateTag(AOUInputParser.tagValves, valvesStr));
        }

        public static string CreateUIXmlString(uint time, string ui)
        {
            // MASK_STATE, BUTTON_ONOFF = 0x0001 (Soft on/Off); BUTTON_EMERGENCYOFF = 0x0002 (Hard Off); BUTTON_MANUALOPHEAT = 0x0004 (Forced Heating);
            // BUTTON_MANUALOPCOOL = 0x0008 (Forced Cooling); BUTTON_CYCLE = 0x0010 (Forced Cycling); BUTTON_RUN = 0x0020 (Run with IMM)
            return CreateStateXmlString(time, CreateTag(AOUInputParser.tagUI, ui));
        }

        public static string CreateIMMXmlString(uint time, string imm)
        {
            return CreateStateXmlString(time, CreateTag(AOUInputParser.tagIMM, imm));
        }

        public static string CreateModeXmlString(uint time, int mode)
        {
            return CreateStateXmlString(time, CreateTag(AOUInputParser.tagMode, mode.ToString()));
        }

        public static string CreateCmdRetXmlString(uint time, string command, string value = "")
        {
            // <ret><Time>time_ds</Time><"ParameterName">value</"ParameterName"></ret>
            string content = CreateTimeXmlString(time) + CreateTag(command, value);
            return CreateTag(AOUInputParser.tagRetValue, content);
        }


        public static string CreatePowXmlString(uint time, uint pow)
        {
            return CreateStateXmlString(time, CreateTag(AOUInputParser.tagPower, pow));
        }

        public static string CreateSeqXmlString(uint time, uint seq)
        {
            return CreateStateXmlString(time, CreateTag(AOUInputParser.tagSeqState, seq));
        }

        public static string CreateLogXmlString(uint time, string msg)
        {
            string timetag = CreateTimeXmlString(time);
            string content = timetag + CreateTag(AOUInputParser.tagLogSubTagMsg, msg);
            return CreateTag(AOUInputParser.tagLog, content);
        }

        public static string CreateWholeTempStateXmlString(AOUStateData data)
        {
            uint time_ms = Time2WordToMs(data.time_hours, data.time_sek_x_10_of_hour);

            string tempContent = CreateTag(AOUInputParser.tagTempSubTagHeat, data.heaterTemp) + CreateTag(AOUInputParser.tagTempSubTagHot, data.hotTankTemp) + CreateTag(AOUInputParser.tagTempSubTagRet, data.retTemp);
            tempContent += CreateTag(AOUInputParser.tagTempBuHot, data.bufHotTemp) + CreateTag(AOUInputParser.tagTempBuMid, data.bufMidTemp) + CreateTag(AOUInputParser.tagTempBuCold, data.bufColdTemp);
            tempContent += CreateTag(AOUInputParser.tagTempSubTagCool, data.coolerTemp) + CreateTag(AOUInputParser.tagTempSubTagCold, data.coldTankTemp) + CreateTag(AOUInputParser.tagTempSubTagBearHot, data.BearHot);
            tempContent += CreateTag(AOUInputParser.tagTempSubTagReturnForecasted, data.RetForTemp);

            return CreateStateXmlString(time_ms, CreateTag(AOUInputParser.tagTemp, tempContent));
        }
    }
}
