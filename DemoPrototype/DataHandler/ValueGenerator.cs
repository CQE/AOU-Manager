﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPrototype
{
    static public class ValueGenerator
    {
        static Random rnd = new Random();
        static long ts_start = -1;

        #region common
        static public uint RandomFromUIntArray(uint[] uintArr)
        {
            uint index = (uint)rnd.Next(0, uintArr.Length);
            return uintArr[index];
        }

        static public bool GetRandomOk(int max)
        {
            long value = rnd.Next(0, max);
            return (value == 0);
        }

        static public char GetRandomAsciiChar(char lowestChar, char highestChar)
        {
            return (char)(byte)rnd.Next((int)lowestChar, (int)highestChar);
        }

        static public string GetRandomString(int length)
        {
            string str = "";
            for (int i = 0; i < length; i++)
            {

                str += ValueGenerator.GetRandomAsciiChar('A', 'Z');
            }
            return str;
        }

        static public int GetRandomInt(int min, int max)
        {
            return rnd.Next(min, max);
        }

        static public double GetRandomDouble(double min, double max, double numDec)
        {
            // ToDo numDec
            return min + (max - min) * rnd.NextDouble();
        }

        static public UInt16 RealToUInt16(double value)
        {
            //return (UInt16)(Math.Round(value, 2)*100);
            return (UInt16)(Math.Round(value));
        }

        static public Int16 RealToInt16(double value)
        {
            return (Int16)(Math.Round(value));
        }

        static public double WordX100ToDouble(UInt16 value)
        {
            // return value / 100;
            return value;
        }

        #endregion



        static public int GetRandomCommandValue(string cmd)
        {
            return GetRandomCommandValue(GlobalVars.aouCommands.Command(cmd));
        }

        static public int GetRandomCommandValue(AOUDataTypes.CommandType cmd)
        {
            switch (cmd)
            {
                case AOUDataTypes.CommandType.coldDelayTime: return GetRandomInt(6, 10);
                case AOUDataTypes.CommandType.hotDelayTime: return GetRandomInt(5, 8);
                case AOUDataTypes.CommandType.coolingTime: return GetRandomInt(0, 5);
                case AOUDataTypes.CommandType.heatingTime: return GetRandomInt(0, 4);

                case AOUDataTypes.CommandType.toolCoolingFeedPause: return GetRandomInt(0, 4);
                case AOUDataTypes.CommandType.toolHeatingFeedPause: return GetRandomInt(0, 4);
                case AOUDataTypes.CommandType.tempColdTankFeedSet: return GetRandomInt(20, 40);
                case AOUDataTypes.CommandType.tempHotTankFeedSet: return GetRandomInt(100, 240);

                case AOUDataTypes.CommandType.TBufferColdUpperLimit: return GetRandomInt(100, 120);
                case AOUDataTypes.CommandType.TBufferHotLowerLimit: return GetRandomInt(80, 105);
                case AOUDataTypes.CommandType.TBufferMidRefThreshold: return GetRandomInt(70, 90);

                case AOUDataTypes.CommandType.TReturnThresholdHot2Cold: return GetRandomInt(80, 100);
                case AOUDataTypes.CommandType.TReturnThresholdCold2Hot: return GetRandomInt(100, 120);
                case AOUDataTypes.CommandType.THotTankAlarmLowThreshold: return GetRandomInt(80, 200);
                case AOUDataTypes.CommandType.TColdTankAlarmHighThreshold: return GetRandomInt(20, 60);
                default: return 0;

            }
        }

        static public int GetStaticCommandValue(string cmd)
        {
            return GetStaticCommandValue(GlobalVars.aouCommands.Command(cmd));
        }

        static public int GetStaticCommandValue(AOUDataTypes.CommandType cmd)
        {
            switch (cmd)
            {
                /*
                globDelayTimes.HotCalibrate = 8;
                globDelayTimes.ColdCalibrate = 7;
                globDelayTimes.HotTune = 3;
                globDelayTimes.ColdTune = 2;
                */
                case AOUDataTypes.CommandType.coldDelayTime: return 8;
                case AOUDataTypes.CommandType.hotDelayTime: return 7;
                case AOUDataTypes.CommandType.coolingTime: return 3;
                case AOUDataTypes.CommandType.heatingTime: return 2;

                /*
                globFeedTimes.HeatingActive = 0;// 20;
                globFeedTimes.HeatingPause = 0;// 22;
                globFeedTimes.CoolingActive = 0; //21;
                globFeedTimes.CoolingPause = 0;// 23;
                */
                case AOUDataTypes.CommandType.toolCoolingFeedPause: return 20;
                case AOUDataTypes.CommandType.toolHeatingFeedPause: return 22;
                case AOUDataTypes.CommandType.tempColdTankFeedSet: return 29;
                case AOUDataTypes.CommandType.tempHotTankFeedSet: return 188;

                // globThresholds.ThresholdHotBuffTankAlarmLimit = 110;
                // globThresholds.ThresholdMidBuffTankAlarmLimit = 100;
                // globThresholds.ThresholdColdTankBuffAlarmLimit = 90;
                case AOUDataTypes.CommandType.TBufferColdUpperLimit: return 40;
                case AOUDataTypes.CommandType.TBufferHotLowerLimit: return 120;
                case AOUDataTypes.CommandType.TBufferMidRefThreshold: return 90;

                // globThresholds.ThresholdHot2Cold = 100;
                // globThresholds.ThresholdCold2Hot = 110;
                // globThresholds.ThresholdHotTankLowLimit = 120;
                // globThresholds.ThresholdColdTankUpperLimit = 80;
                case AOUDataTypes.CommandType.TReturnThresholdHot2Cold: return 66;
                case AOUDataTypes.CommandType.TReturnThresholdCold2Hot: return 88;
                case AOUDataTypes.CommandType.THotTankAlarmLowThreshold: return 111;
                case AOUDataTypes.CommandType.TColdTankAlarmHighThreshold: return 55;
                default: return 0;

            }
        }

        static public AOUStateData GetRandomStateData(uint time_ms, bool randomRetForTemp)
        {
            AOUStateData stateData;

            AOUDataTypes.Time_ms_to_AOUModelTimeSecX10(time_ms, out stateData.time_hours, out stateData.time_sek_x_10_of_hour);

            stateData.UIButtons = 0;  stateData.Mode = 0; stateData.IMM = 0;  stateData.Valves = 0; stateData.Safety = 0;
            stateData.seqState = 0;   stateData.Power = 0; stateData.Energy = 0; stateData.FrontAndMode = 0;

            /* Only temperature values will be set */
            stateData.bufHotTemp = RealToUInt16(ValueGenerator.GetTBufferHotValue());
            stateData.bufMidTemp = RealToUInt16(ValueGenerator.GetTBufferMidValue());
            stateData.bufColdTemp = RealToUInt16(ValueGenerator.GetTColdTankValue());

            stateData.coldTankTemp = RealToUInt16(ValueGenerator.GetTColdTankValue());
            stateData.hotTankTemp = RealToUInt16(ValueGenerator.GetTHotTankValue());
            stateData.retTemp = RealToUInt16(ValueGenerator.GetTReturnValveValue());

            if (randomRetForTemp)
            {
                stateData.RetForTemp = RealToInt16(ValueGenerator.GetTReturnForecastedValue());
            }
            else
            {
                stateData.RetForTemp = Int16.MaxValue;
            }

            stateData.coolerTemp = RealToUInt16(ValueGenerator.GetValveCoolantValue());
            stateData.coolantTemp = RealToUInt16(ValueGenerator.GetValveCoolantValue());
            stateData.heaterTemp = RealToUInt16(ValueGenerator.GetTheaterOilOutValue());

            stateData.BearHot = RealToUInt16(ValueGenerator.GetTHotTankValue());

            return stateData;
        }

        #region Get Values
        static public double GetTHotTankValue()
        {
            return ValueGenerator.GetRandomDouble(204, 235, 2);
        }

        static public double GetTColdTankValue()
        {
            return ValueGenerator.GetRandomDouble(31, 45, 2);
        }

        static public double GetTReturnValveValue()
        {
            return ValueGenerator.GetRandomDouble(40, 87, 2);
        }

        static public double GetTReturnActualValue()
        {
            return ValueGenerator.GetRandomDouble(40, 46, 2);
        }

        static public double GetTReturnForecastedValue()
        {
            return ValueGenerator.GetRandomDouble(-10, 10, 2);
        }

        static public double GetTBufferHotValue()
        {
            return ValueGenerator.GetRandomDouble(180, 200, 2);
        }

        static public double GetTBufferMidValue()
        {
            return ValueGenerator.GetRandomDouble(75, 110, 2);
        }

        static public double GetTBufferColdValue()
        {
            return ValueGenerator.GetRandomDouble(20, 40, 2);
        }

        static public double GetTheaterOilOutValue()
        {
            return ValueGenerator.GetRandomDouble(250, 290, 2);
        }

        static public double GetValveCoolantValue()
        {
            return rnd.Next(50, 90); // 0-100%, 50-90  
        }
 
        static public double GetPowerHeatingValue()
        {
            return rnd.Next(5, 12); // 0-14kW or % ?
        }

        #endregion

        static public long GetElapsedTime(uint timeBetween)
        {
            if (ts_start == -1)
            {
                ts_start = 0;  // First time
            }

            TimeSpan ts = TimeSpan.FromMilliseconds(ts_start);
            long ret = ts_start;
            ts_start += timeBetween;
            return ret;
        }
    }
}
