using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPrototype
{
    public static class AOUDataTypes
    {
       public const long msInWeek = 60000 * 10080; // 1 week = 7*24*60 = 10 080 min

        public const UInt16 UInt16_NaN = UInt16.MaxValue;

        public static void TimeMsToAOUModelTime(long time_ms, out UInt16 time_minutes_of_week, out UInt16 time_ms_of_minute)
        {
            time_ms_of_minute = (UInt16)(time_ms % 60000); // 1 min = 60 000 ms
            time_minutes_of_week = (UInt16)((time_ms % msInWeek) / 60000); 
        }

        public static long AOUModelTimeToTimeMs(UInt16 time_minutes_of_week, UInt16 time_ms_of_minute)
        {
            return time_minutes_of_week * 60000 + time_ms_of_minute;
        }


        public static void TimeDecSecToAOUModelTime(long time_ms, out UInt16 time_hours, out UInt16 time_dec_sek)
        {
            time_hours = 0; //  (UInt16)(time_ms % 60000); // 1 hour = 60 min x 6 (s/10) = 36000
            time_dec_sek = (UInt16)time_ms;
        }

        public static long AOUModelTimeDecSecToTimeMs(UInt16 time_hours, UInt16 time_dec_sek)
        {
            return time_hours*60*60*1000 + time_dec_sek*100;
        }

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

        public enum FeedType { Unknown, Cold, Hot };

        public enum StateType
        {
            NOTHING = 0, SQ_INITIAL, IDLE, SQ_WAIT_HOT_AT_MOULD_ENTRY, SQ_WAIT_COLD_AT_MOULD_ENTRY, SQ_WAIT_FOR_INJECTION_BEGIN,
            SQ_WAIT_FOR_INJECTION_END, SQ_WAIT_FOR_COOLING_END, SQ_WAIT_FOR_OPEN_BEGIN,
            SQ_WAIT_FOR_EJECT_BEGIN, SQ_WAIT_FOR_EJECT_END, SQ_WAIT_FOR_OPEN_END
        };

        public enum IMMSettings
        {
            Nothing, SetIMMError, SetIMMBlockInject, SetIMMBlockOpen, IMMStop,
            CycleAuto, IMMInjecting, IMMEjecting, IMMToolClosed
        };

        public enum AOUDataType
        {
            AOUTempData, AOUColdFeedData, AOUHotFeedData, AOUColdLevelData,
            AOUHotLevelData, AOUValvesData, AOUSeqData, AOUIMMData
        }

        public const int NumStates = 8;

        public static StateType StringToStateType(string state_str)
        {
            switch (state_str)
            {
                case "IDLE": return StateType.IDLE;
                case "SQ_INITIAL": return StateType.SQ_INITIAL;
                case "SQ_WAIT_HOT_AT_MOULD_ENTRY": return StateType.SQ_WAIT_HOT_AT_MOULD_ENTRY;
                case "SQ_WAIT_COLD_AT_MOULD_ENTRY": return StateType.SQ_WAIT_COLD_AT_MOULD_ENTRY;
                case "SQ_WAIT_FOR_INJECTION_BEGIN": return StateType.SQ_WAIT_FOR_INJECTION_BEGIN;
                case "SQ_WAIT_FOR_INJECTION_END": return StateType.SQ_WAIT_FOR_INJECTION_END;
                case "SQ_WAIT_FOR_COOLING_END": return StateType.SQ_WAIT_FOR_COOLING_END;
                case "SQ_WAIT_FOR_OPEN_BEGIN": return StateType.SQ_WAIT_FOR_OPEN_BEGIN;
                case "SQ_WAIT_FOR_EJECT_BEGIN": return StateType.SQ_WAIT_FOR_EJECT_BEGIN;
                case "SQ_WAIT_FOR_EJECT_END": return StateType.SQ_WAIT_FOR_EJECT_END;
                case "SQ_WAIT_FOR_OPEN_END": return StateType.SQ_WAIT_FOR_OPEN_END;
                default: return StateType.NOTHING;
            }
        }

    }
}
