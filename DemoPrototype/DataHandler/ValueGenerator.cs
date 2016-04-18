using System;
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
        private static double[] onOffArr = new double[] { 50, 70 };

        static public long RandomFromIntArray(long[] intArr, int period)
        {
            long index = rnd.Next(0, intArr.Length);
            return intArr[index];
        }

        static public double RandomFromDoubleArray(double[] dblArr, int period)
        {
            long index = rnd.Next(0, dblArr.Length);
            return dblArr[index];
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


        static public double GetRandomDouble(double min, double max, double numDec)
        {
            // ToDo numDec
            return min + (max - min) * rnd.NextDouble();
        }

        static public UInt16 RealToWordX100(double value)
        {
            //return (UInt16)(Math.Round(value, 2)*100);
            return (UInt16)(Math.Round(value));
        }

        static public double WordX100ToDouble(UInt16 value)
        {
            // return value / 100;
            return value;
        }

        #endregion

        static public AOUTemperatureData GetRandomTempData(long time)
        {
            AOUTemperatureData tempData; // = new AOUModels.AOUTemperatureData();

            AOUDataTypes.TimeMsToAOUModelTime(time, out tempData.time_min_of_week, out tempData.time_ms_of_min);

            tempData.coldTankTemp = RealToWordX100(ValueGenerator.GetTColdTankValue());
            tempData.hotTankTemp = RealToWordX100(ValueGenerator.GetTHotTankValue());
            tempData.retTemp = RealToWordX100(ValueGenerator.GetTReturnValveValue());
            tempData.coolerTemp = RealToWordX100(ValueGenerator.GetValveCoolantValue());

            return tempData;
        }

        static public AOUHotFeedData GetRandomHotFeedData(long time)
        {
            AOUHotFeedData feedData; // = new AOUModels.AOUTemperatureData();
            AOUDataTypes.TimeMsToAOUModelTime(time, out feedData.time_min_of_week, out feedData.time_ms_of_min);
            feedData.newFeedTemp = RealToWordX100(ValueGenerator.GetValveFeedHotValue());
            feedData.prevFeedTemp = RealToWordX100(ValueGenerator.GetValveFeedHotValue());

            return feedData;
        }

        static public AOUColdFeedData GetRandomColdFeedData(long time)
        {
            AOUColdFeedData feedData; // = new AOUModels.AOUTemperatureData();
            AOUDataTypes.TimeMsToAOUModelTime(time, out feedData.time_min_of_week, out feedData.time_ms_of_min);
            feedData.newFeedTemp = RealToWordX100(ValueGenerator.GetValveFeedColdValue());
            feedData.prevFeedTemp = RealToWordX100(ValueGenerator.GetValveFeedColdValue());

            return feedData;
        }

        static public AOUHotLevelData GetRandomHotLevelData(long time)
        {
            AOUHotLevelData levelData; // = new AOUModels.AOUTemperatureData();
            AOUDataTypes.TimeMsToAOUModelTime(time, out levelData.time_min_of_week, out levelData.time_ms_of_min);
            levelData.newLevel = RealToWordX100(ValueGenerator.GetValveFeedHotValue());
            levelData.prevLevel = RealToWordX100(ValueGenerator.GetValveFeedHotValue());

            return levelData;
        }

        static public AOUColdLevelData GetRandomColdLevelData(long time)
        {
            AOUColdLevelData levelData; // = new AOUModels.AOUTemperatureData();
            AOUDataTypes.TimeMsToAOUModelTime(time, out levelData.time_min_of_week, out levelData.time_ms_of_min);
            levelData.newLevel = RealToWordX100(ValueGenerator.GetValveFeedColdValue());
            levelData.prevLevel = RealToWordX100(ValueGenerator.GetValveFeedColdValue());

            return levelData;
        }

        static public AOUValvesData GetRandomValvesData(long time)
        {
            AOUValvesData valvesData; // = new AOUModels.AOUTemperatureData();
            AOUDataTypes.TimeMsToAOUModelTime(time, out valvesData.time_min_of_week, out valvesData.time_ms_of_min);

            valvesData.newValveReturnTemp = RealToWordX100(ValueGenerator.GetValveFeedHotValue());
            valvesData.prevValveReturnTemp = RealToWordX100(ValueGenerator.GetValveFeedHotValue());

            return valvesData;
        }

        static public AOUSeqData GetRandomSeqData(long time, AOUDataTypes.StateType state)
        {
            AOUSeqData seqData;
            AOUDataTypes.TimeMsToAOUModelTime(time, out seqData.time_min_of_week, out seqData.time_ms_of_min);

            seqData.state = (UInt16)state;
            seqData.cycle = 0;

            return seqData;
        }

        static public AOUIMMData GetRandomIMMData(long time, AOUDataTypes.IMMSettings setting)
        {
            AOUIMMData immData;

            AOUDataTypes.TimeMsToAOUModelTime(time, out immData.time_min_of_week, out immData.time_ms_of_min);

            immData.imm_setting_type = (UInt16)setting;
            immData.imm_setting_val = 0;

            return immData;
        }

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
            return ValueGenerator.GetRandomDouble(40, 77, 2);
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

        static public double GetValveFeedHotValue()
        {
            return RandomFromDoubleArray(onOffArr, 0);  // Off=50, On=70  
        }

        static public double GetValveFeedColdValue()
        {
            return RandomFromDoubleArray(onOffArr, 0); 
        }

        static public double GetValveReturnValue()
        {
            return RandomFromDoubleArray(onOffArr, 0); // Cold=50, Hot=70  
        }

        static public double GetValveCoolantValue()
        {
            return rnd.Next(50, 90); // 0-100%, 50-90  
        }

        static public double GetPowerHeatingValue()
        {
            return rnd.Next(5, 12); // 0-14kW ?
        }

        /***********************************/


        static public long GetElapsedTime(double minResolution)
        {
            if (ts_start == -1)
                ts_start = 0; // DateTime.Now.Ticks; // First time

            // TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - ts_start); // Diff ticks

            TimeSpan ts = TimeSpan.FromMilliseconds(ts_start);
            long ret = ts_start;
            ts_start += 1000;
            return ret;
            // return (long)(Math.Round(ts.TotalMilliseconds/minResolution) * minResolution);
        }

        static public Power GetRandomPower(double minResolution)
        {
            AOUDataTypes.StateType valState = (AOUDataTypes.StateType)rnd.Next(1, AOUDataTypes.NumStates + 1);

            var power = new Power()
            {
                ElapsedTime = GetElapsedTime(minResolution),
                State = valState, 
                THotTank = GetTHotTankValue(),
                TColdTank = GetTColdTankValue(),
                TReturnValve = GetTReturnValveValue(),
                TReturnActual = GetTReturnActualValue(),
                TReturnForecasted = GetTReturnForecastedValue(),
                TBufferHot = GetTBufferHotValue(),
                TBufferMid = GetTBufferMidValue(),
                TBufferCold = GetTBufferColdValue(),
                THeaterOilOut = GetTheaterOilOutValue(),

                ValveFeedHot = GetValveFeedHotValue(),
                ValveFeedCold = GetValveFeedColdValue(),
                ValveReturn = GetValveReturnValue(),
                ValveCoolant = GetValveCoolantValue(),
                PowerHeating = GetPowerHeatingValue(),
            };

            return power;
        }

        static public AOULogMessage GetRandomLogMsg(double minResolution)
        {
            long ts = GetElapsedTime(minResolution);
            uint prio = (uint)rnd.Next(1, 3);
            uint pid = (uint)rnd.Next(1038, 1965);
            string logtext = "log-" + GetRandomString(10);
            return new AOULogMessage(ts, logtext, prio, pid);
        }
    }
}
