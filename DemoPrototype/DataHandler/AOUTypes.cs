using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPrototype
{
    public class AOUCommands : List<KeyValuePair<AOUDataTypes.CommandType, string>>
    {
        /*  Arduino command string list
           char* cmdList[] = {
          "idleMode", "heatingMode", "coolingMode", "fixedCyclingMode", "autoWidthIMMMode", "aouMode",
          "hotDelayTime", "coldDelayTime",
          "THotTankAlarmLowThreshold", "TColdTankAlarmHighThreshold", "TReturnThresholdCold2Hot", "TReturnThresholdHot2Cold",
          "TBufferHotLowerLimit", "TBufferMidRefThreshold", "TBufferColdUpperLimit",
          "tempHotTankFeedSet", "tempColdTankFeedSet",
          "heatingTime",  "coolingTime",  "toolHeatingFeedPause",  "toolCoolingFeedPause",
        }; 
        */
        public AOUCommands()
        {
            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.runModeIdle, "idleMode"));
            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.runModeHeating, "heatingMode"));
            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.runModeCooling, "coolingMode"));
            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.runModeFixedCycling, "fixedCyclingMode"));
            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.runModeAutoWithIMM, "autoWidthIMMMode"));
            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.runModeAOU, "aouMode"));

            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.hotDelayTime, "hotDelayTime"));
            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.coldDelayTime, "coldDelayTime"));

            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.THotTankAlarmLowThreshold, "THotTankAlarmLowThreshold"));
            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.TColdTankAlarmHighThreshold, "TColdTankAlarmHighThreshold"));
            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.TReturnThresholdCold2Hot, "TReturnThresholdCold2Hot"));
            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.TReturnThresholdHot2Cold, "TReturnThresholdHot2Cold"));

            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.TBufferHotLowerLimit, "TBufferHotLowerLimit"));
            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.TBufferMidRefThreshold, "TBufferMidRefThreshold"));
            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.TBufferColdUpperLimit, "TBufferColdUpperLimit"));

            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.tempHotTankFeedSet, "tempHotTankFeedSet"));
            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.tempColdTankFeedSet, "tempColdTankFeedSet"));

            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.heatingTime, "heatingTime"));
            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.coolingTime, "coolingTime"));
            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.toolHeatingFeedPause, "toolHeatingFeedPause"));
            this.Add(new KeyValuePair<AOUDataTypes.CommandType, string>(AOUDataTypes.CommandType.toolCoolingFeedPause, "toolCoolingFeedPause"));
        }

        public string StringValue(AOUDataTypes.CommandType cmd)
        {
            var kvp = this.Find(kv => kv.Key == cmd);
            return kvp.Value;
        }

        public AOUDataTypes.CommandType Command(string cmdStr)
        {
            var kvp = this.Find(kv => kv.Value == cmdStr);
            return kvp.Key;
        }
    }

    public static class AOUDataTypes
    {
        public enum AOURunningMode { Idle = 0, Heating, Cooling, FixedCycling, AutoWithIMM }

        /*  Arduino command Constants
            #define CMD_IDLEMODE 0
            #define CMD_HEATINGMODE 1
            #define CMD_COOLINGMODE 2
            #define CMD_FIXEDCYCLINGMODE 3
            #define CMD_AUTOWIDTHIMMMODE 4
            #define CMD_AOUMODE 5

            #define CMD_HOTDELAYTIME 6
            #define CMD_COLDDELAYTIME 7

            #define CMD_THOTTANKALARMLOWTHRESHOLD 8
            #define CMD_TCOLDTANKALARMHIGHTHRESHOLD 9
            #define CMD_TRETURNTHRESHOLDCOLD2HOT 10
            #define CMD_TRETURNTHRESHOLDHOT2COLD 11
            #define CMD_TBUFFERHOTLOWERLIMIT 12
            #define CMD_TBUFFERMIDREFTHRESHOLD 13
            #define CMD_TBUFFERCOLDUPPERLIMIT 14

            #define CMD_TEMPHOTTANKFEEDSET 15
            #define CMD_TEMPCOLDTANKFEEDSET 16
            #define CMD_HEATINGTIME 17
            #define CMD_COOLINGTIME 18
            #define CMD_TOOLHEATINGFEEDPAUSE 19
            #define CMD_TOOLCOOLINGFEEDPAUSE 20
        */
        public enum CommandType
        {
            RunningMode = -1,
            runModeIdle = 0, runModeHeating, runModeCooling, runModeFixedCycling, runModeAutoWithIMM, runModeAOU,
            hotDelayTime, coldDelayTime,
            THotTankAlarmLowThreshold, TColdTankAlarmHighThreshold,
            TReturnThresholdCold2Hot, TReturnThresholdHot2Cold,
            TBufferHotLowerLimit, TBufferMidRefThreshold, TBufferColdUpperLimit,
            tempHotTankFeedSet, tempColdTankFeedSet,
            heatingTime, coolingTime,
            toolHeatingFeedPause, toolCoolingFeedPause,  

        }
//----------------------------------------------------------------------------------------------------------------------------
        public enum HT_StateType {
            HT_STATE_NOT_SET = -99, HT_STATE_INVALID = -999, HT_STATE_COLD = -1, HT_STATE_UNKNOWN = 0,  HT_STATE_HOT = 1
        }

        // IMM_OutIMMError: 0x01; IMM_OutIMMBlockInject: 0x02; IMM_OutIMMBlockOpen: 0x04; IMM_InIMMStop: 0x08
        // IMM_InCycleAuto: 0x10; IMM_InIMMInjecting: 0x20; IMM_InIMMEjecting: 0x40; IMM_InIMMToolClosed: 0x80
        public enum IMMSettings
        {
            Nothing, OutIMMError, OutIMMBlockInject, OutIMMBlockOpen, InIMMStop,
            InCycleAuto, InIMMInjecting, InIMMEjecting, InIMMToolClosed
        };

        public enum ButtonState
        {
            off = 0, on = 1
        }

        public enum StateType
        {
            NOTHING = 0, SQ_INITIAL, IDLE, SQ_WAIT_HOT_AT_MOULD_ENTRY, SQ_WAIT_COLD_AT_MOULD_ENTRY,
            SQ_WAIT_FOR_INJECTION_BEGIN, SQ_WAIT_FOR_INJECTION_END, SQ_WAIT_FOR_COOLING_END,
            SQ_WAIT_FOR_OPEN_BEGIN, SQ_WAIT_FOR_EJECT_BEGIN, SQ_WAIT_FOR_EJECT_END, SQ_WAIT_FOR_OPEN_END
        };

        // MASK_STATE, BUTTON_ONOFF = 0x0001 (Soft on/Off); BUTTON_EMERGENCYOFF = 0x0002 (Hard Off); BUTTON_MANUALOPHEAT = 0x0004 (Forced Heating);
        // BUTTON_MANUALOPCOOL = 0x0008 (Forced Cooling); BUTTON_CYCLE = 0x0010 (Forced Cycling); BUTTON_RUN = 0x0020 (Run with IMM)
        public struct UI_Buttons
        {
            public ButtonState OnOffButton;
            public ButtonState ButtonEmergencyOff;
            public ButtonState ButtonForcedHeating;
            public ButtonState ButtonForcedCooling;
            public ButtonState ButtonForcedCycling;
            public ButtonState ButtonRunWithIMM;
        }

        //----------------------------------------------------------------------------------------------------------------------------
        // The same as double.IsNaN for UInt16
        public const UInt16 UInt16_NaN = UInt16.MaxValue;

        public static bool IsUInt16NaN(UInt16 value)
        {
            return value == UInt16_NaN;
        }

        public static bool IsInt16NaN(Int16 value)
        {
            return value == Int16.MaxValue;
        }

        public static void Time_ms_to_AOUModelTimeSecX10(long time_ms, out UInt16 time_hours, out UInt16 time_sek_x_10)
        {
            // 1 hour = 60 sek * 10 * 60 min = 36000 (sek x 10) = 3600000 ms
            UInt32 tot_sek_x_10 = (UInt32)(time_ms/100);
            time_hours = (UInt16)(tot_sek_x_10 / 36000);
            time_sek_x_10 = (UInt16)(tot_sek_x_10 % 36000);
        }

        public static long AOUModelTimeSecX10_to_TimeMs(UInt16 time_hours, UInt16 time_sek_x_10)
        {
            return (long)(time_hours * 36000 + time_sek_x_10) * 100;
        }
    }
}
