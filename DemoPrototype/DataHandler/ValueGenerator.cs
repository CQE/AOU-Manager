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

        static public AOUStateData GetRandomStateData(uint time_ms)
        {
            AOUStateData stateData;

            AOUDataTypes.Time_ms_to_AOUModelTimeSecX10(time_ms, out stateData.time_hours, out stateData.time_sek_x_10_of_hour);

            stateData.UIButtons = 0;  stateData.Mode = 0; stateData.IMM = 0;  stateData.Valves = 0;
            stateData.seqState = 0;   stateData.Power = 0; stateData.Energy = 0;

            /* Only temperature values will be set */
            stateData.bufHotTemp = RealToWordX100(ValueGenerator.GetTBufferHotValue());
            stateData.bufMidTemp = RealToWordX100(ValueGenerator.GetTBufferMidValue());
            stateData.bufColdTemp = RealToWordX100(ValueGenerator.GetTColdTankValue());

            stateData.coldTankTemp = RealToWordX100(ValueGenerator.GetTColdTankValue());
            stateData.hotTankTemp = RealToWordX100(ValueGenerator.GetTHotTankValue());
            stateData.retTemp = RealToWordX100(ValueGenerator.GetTReturnValveValue());
            stateData.RetForTemp = RealToWordX100(ValueGenerator.GetTReturnValveValue());

            stateData.coolerTemp = RealToWordX100(ValueGenerator.GetValveCoolantValue());
            stateData.heaterTemp = RealToWordX100(ValueGenerator.GetTheaterOilOutValue());

            stateData.BearHot = RealToWordX100(ValueGenerator.GetTHotTankValue());

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
