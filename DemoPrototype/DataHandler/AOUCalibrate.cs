using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPrototype
{
    class AOUCalibrate
    {
        public AOUCalibrate(bool init = true)
        {
            ElapsedTime = 0;
            TReturnActual = double.NaN;
            TReturnForecasted = double.NaN;
        }

        public long ElapsedTime { get; set; }
        public double TReturnActual { get; set; }
        public double TReturnForecasted { get; set; }
    }
}

