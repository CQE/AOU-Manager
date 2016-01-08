using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHandler
{
    public class AOURouter
    {
        private TextFile dataFile;
        Power lastPower;

        Queue<AOULogMessage> msgQueue;

        public AOURouter()
        {
            dataFile = new TextFile();
            lastPower = null;
            msgQueue = new Queue<AOULogMessage>();
        }


        public void Update()
        {
            Power pwr = ValueGenerator.GetRandomPower();
            var dt = DateTime.Today;
            string dateStr = dt.ToString("yyMMdd");
            string ts = pwr.ElapsedTime.ToString()+',';

            dataFile.AddToFile("AOU\\TBufferCold\\", "TBufferCold-" + dateStr + ".txt", ts + pwr.TBufferCold);
            dataFile.AddToFile("AOU\\TBufferMid\\", "TBufferMid-" + dateStr + ".txt", ts + pwr.TBufferMid);
            dataFile.AddToFile("AOU\\TBufferHot\\", "TBufferHot-" + dateStr + ".txt", ts + pwr.TBufferHot);
            dataFile.AddToFile("AOU\\TCoolTank\\", "TCoolTank-" + dateStr + ".txt", ts + pwr.TCoolTank);
            dataFile.AddToFile("AOU\\THotTank\\", "THotTank-" + dateStr + ".txt", ts + pwr.THotTank);
            dataFile.AddToFile("AOU\\State\\", "State-" + dateStr + ".txt", ts + pwr.State);
            dataFile.AddToFile("AOU\\TReturnActual\\", "TReturnActual-" + dateStr + ".txt", ts + pwr.TReturnActual);
            dataFile.AddToFile("AOU\\TReturnForecasted\\", "TReturnForecasted-" + dateStr + ".txt", ts + pwr.TReturnForecasted);
            dataFile.AddToFile("AOU\\TReturnValve\\", "TReturnValve-" + dateStr + ".txt", ts + pwr.TReturnValve);

            lastPower = pwr; // ToDo
        }

        public Power GetLastPower()
        {
            return lastPower;
        }
    }
}
