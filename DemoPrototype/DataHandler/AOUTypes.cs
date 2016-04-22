using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPrototype
{
    public static class AOUDataTypes
    {
        public enum AOURunningMode { Idle = 0, Heating, Cooling, FixedCycling, AutoWithIMM }

        public enum CommandType
        {
            CmdTypeToDo /* To use when not know */,    RunningMode,

            tempHotTankFeedSet, tempColdTankFeedSet, coolingTime, heatingTime,
            toolHeatingFeedPause, toolCoolingFeedPause,  hotDelayTime, coldDelayTime,

            THotTankAlarmLowThreshold, TColdTankAlarmHighThreshold,
            TReturnThresholdCold2Hot, TReturnThresholdHot2Cold,
            TBufferHotLowerLimit, TBufferMidRefThreshold, TBufferColdUpperLimit
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

        /* Old ms format
        public const long msInWeek = 60000 * 10080; // 1 week = 7*24*60 = 10 080 min

        public static void TimeMsToAOUModelTime(long time_ms, out UInt16 time_minutes_of_week, out UInt16 time_ms_of_minute)
        {
            time_ms_of_minute = (UInt16)(time_ms % 60000); // 1 min = 60 000 ms
            time_minutes_of_week = (UInt16)((time_ms % msInWeek) / 60000);
            
        }

        public static long AOUModelTimeToTimeMs(UInt16 time_minutes_of_week, UInt16 time_ms_of_minute)
        {
            return time_minutes_of_week * 60000 + time_ms_of_minute;
        }
        */

    }
}
