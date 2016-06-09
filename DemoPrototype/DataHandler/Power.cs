using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPrototype
{

    // Todo: Compare between struct and class for performance
    public struct Power
   // public class Power
    {
        public Power(long etime)
        {
            ElapsedTime = etime;
            State = AOUDataTypes.StateType.NOTHING;

            THotTank = double.NaN;
            TColdTank = double.NaN;

            TReturnValve = double.NaN;
            TReturnActual = double.NaN;
            TReturnForecasted = double.NaN;

            TBufferHot = double.NaN;
            TBufferMid = double.NaN;
            TBufferCold = double.NaN;

            THeatExchangerCoolantOut = double.NaN;
            THeaterOilOut = double.NaN;

            ValveFeedHot = double.NaN;
            ValveFeedCold = double.NaN;
            ValveReturn = double.NaN;
            ValveCoolant = double.NaN;

            PowerHeating = double.NaN;
        }

        // Time in ms
        public long ElapsedTime { get; set; }

        public AOUDataTypes.StateType State { get; set; }

        public double THotTank { get; set; }
        public double TColdTank { get; set; }
        public double TReturnValve { get; set; }

        public double TReturnForecasted { get; set; }
        public double TReturnActual { get; set; }

        public double TBufferHot { get; set; }
        public double TBufferMid { get; set; }
        public double TBufferCold { get; set; }

        public double THeatExchangerCoolantOut { get; set; }
        public double THeaterOilOut { get; set; }

        public double ValveFeedHot { get; set; } // on/off
        public double ValveFeedCold { get; set; } // on/off
        public double ValveReturn { get; set; } // cold/hot
        public double ValveCoolant { get; set; } // % ?

        public double PowerHeating { get; set; } // 

        public override string ToString()
        {
            return "Power:" + AOUHelper.msToTimeSpanStr(ElapsedTime);
        }

        /* Used for testing
        public override string ToString()
        {
            try { 
                string fmt = "{0}: hot:{2:0.}, cold:{3:0.}, ret:{4:0.} (Tbuf h:{7:0.}, m:{8:0.}, c:{9:0.})";
                fmt += " Theco:{10:0.}, heat:{11:0.}, pow:{12:0.}  cool:{16} [Valves hot:{13}, cold:{14}, ret:{15}] [act:{5:0.}, for:{6:0.}] {1}";
                string vHot = ValveFeedHot == 50 ? "off" : "on"; How replace 50?
                string vCold = ValveFeedCold == 50 ? "off" : "on";
                string vRet = ValveReturn == 50 ? "cold" : "hot";
                string vCool = ValveCoolant == 50 ? "off" : "on";

                return String.Format(fmt, AOUHelper.msToTimeSpanStr(ElapsedTime), State.ToString(), THotTank, TColdTank, 
                    TReturnValve, TReturnActual, 0, TBufferHot, TBufferMid, TBufferCold, 
                    THeatExchangerCoolantOut, THeaterOilOut, PowerHeating,  vHot, vCold, vRet, vCool);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        */
    }
}

/*
 -- Run view Parameter monitoring --

     THotTank, TColdTank
     TReturnActual, TReturnForecasted (high/low)
     TBufferHot, TBufferMid, TBufferCold

     New:
         TheatExchangerCoolantOut, TheaterOilOut

         ValveFeedHot (on/off) / preferably curve vertically draggable in graph
         ValveFeedCold (on/off) / preferably curve vertically draggable in graph
         ValveReturn (on/off) / preferably curve vertically draggable in graph
         ValveCoolant (on/off curve & average duty cycle % as number) / preferably curve vertically draggable in graph
         PowerHeating (on/off curve & average duty cycle % as number) / preferably curve vertically draggable in graph

 Calibrate view:
     Feed and Return timings(s):
         THotTank (see how much of this TReturnActual has reached)
         TColdTank (see how much of this TReturnActual has reached)
         TReturnActual (see time diff with TReturnForecasted)
         TReturnForecasted (see time diff with TReturnActual)

     Energy balance (C):
         THotTank
         TColdTank
         TReturnActual

     Volume balance (C):
         TBufferHot
         TBufferMid
         TBufferCold
 */
