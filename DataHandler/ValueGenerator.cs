using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHandler
{
    static public class ValueGenerator
    {
        static Random rnd = new Random();

        static public Power GetRandomPower()
        {
            long ts = (long) ((ulong)DateTime.Now.Ticks / (ulong)10000); // 100 ns to s

            long valTHotTank = rnd.Next(204, 235);
            long valTCoolTank = rnd.Next(31, 45);
            long valTReturnValve = rnd.Next(40, 87);
            long valTReturnActual = rnd.Next(40, 46);
            long valTReturnForecasted = rnd.Next(40, 77);
            int valState = rnd.Next(2, 7);
            long valTBufferHot = rnd.Next(180, 200);
            long valTBufferMid = rnd.Next(75, 110);
            long valTBufferCold = rnd.Next(20, 40);

            var power = new Power()
            {
                ElapsedTime = ts,
                THotTank = valTHotTank,
                TCoolTank = valTCoolTank,
                TReturnValve = valTReturnValve,
                TReturnActual = valTReturnActual,
                TReturnForecasted = valTReturnForecasted,
                TBufferHot = valTBufferHot,
                TBufferMid = valTBufferMid,
                TBufferCold = valTBufferCold,
                State = valState
            };

            return power;
        }

    }
}
