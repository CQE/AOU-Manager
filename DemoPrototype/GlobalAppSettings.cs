using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataHandler;
using Windows.Storage;

namespace DemoPrototype
{
    public static class GlobalAppSettings
    {
        static public bool IsCelsius
        {
            get
            {
                return ApplicationData.Current.LocalSettings.Values.ContainsKey("IsCelsius") ?
                       (bool)ApplicationData.Current.LocalSettings.Values["IsCelsius"] : true;
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["IsCelsius"] = (bool)value;
            }
        }

        public static int GetAOUCmdValue(AOUTypes.CommandType cmdType)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("AOUCmdValue-" + cmdType))
            {
                return (int)ApplicationData.Current.LocalSettings.Values["AOUCmdValue-" + cmdType];
            }
            else
            {
                return 0; // ToDo default value all commands
            }
        }

        public static void SetAOUCmdValue(AOUTypes.CommandType cmdType, int value)
        {
            ApplicationData.Current.LocalSettings.Values["AOUCmdValue-" + cmdType] = value;
        }


        static public AOURouter.RunType DataRunType
        { // Serial, File, Random
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("DataRunType"))
                {
                    return (AOURouter.RunType)ApplicationData.Current.LocalSettings.Values["DataRunType"];
                }
                else
                {
                    return AOURouter.RunType.Random;
                }
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["DataRunType"] = (int)value;
            }
        }

        static public AOUSettings.FileSetting FileSettings
        { // Serial, File, Random
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("FileSettings"))
                {
                    return (AOUSettings.FileSetting)ApplicationData.Current.LocalSettings.Values["FileSettings"];
                }
                else
                {
                    return new AOUSettings.FileSetting("","");
                }
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["FileSettings"] = value;
            }
        }

        static public AOUSettings.SerialSetting SerialSettings
        { // Serial, File, Random
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("SerialSettings"))
                {
                    return (AOUSettings.SerialSetting)ApplicationData.Current.LocalSettings.Values["SerialSettings"];
                }
                else
                {
                    return new AOUSettings.SerialSetting("COM3", 9600); 
                }
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["SerialSettings"] = value;
            }
        }

        static public AOUSettings.RandomSetting RandomSettings
        { // Serial, File, Random
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("RandomSettings"))
                {
                    return (AOUSettings.RandomSetting)ApplicationData.Current.LocalSettings.Values["RandomSettings"];
                }
                else
                {
                    return new AOUSettings.RandomSetting(30, 1000);
                }
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["RandomSettings"] = value;
            }
        }
        
    }

    /// <summary>
    /// Global variables for current session.
    /// </summary>
    public static class GlobalVars
    {
        public static GlobalThresHolds globThresholds;

        // Important to call this in MainPage constructor. Program crasch If not 
        public static void Init()
        {
            /* Where - this values
                220
                60
            */
            globThresholds = new GlobalThresHolds();
            globThresholds.ThresholdHot2Cold = 180;
            globThresholds.ThresholdCold2Hot = 50;
            globThresholds.ThresholdHotTankLowLimit = 200;
            globThresholds.ThresholdColdTankUpperLimit = 40;

            globThresholds.ThresholdHotBuffTankAlarmLimit = 230;
            globThresholds.ThresholdMidBuffTankAlarmLimit = 100;
            globThresholds.ThresholdColdTankBuffAlarmLimit = 70;
        }

        public static void Save()
        {
            //  To do in future, Maybe for uniqe parameters as mould
        }

        public static void Load()
        {
            //  To do in future, Maybe for uniqe parameters
        }
    }

    public class GlobalThresHolds
    {
        private int _thresholdHot2Cold;
        private int _ThresholdCold2Hot;
        private int _ThresholdHotTankLowLimit;
        private int _ThresholdColdTankUpperLimit;
        private int _ThresholdHotTankAlarm;
        private int _ThresholdColdTankAlarm;
        private int _ThresholdMidTankAlarm;

        public int ThresholdHot2Cold
        {
            get { return _thresholdHot2Cold; }
            set { _thresholdHot2Cold = value; }
        }

        public int ThresholdCold2Hot
        {
            get { return _ThresholdCold2Hot; }
            set { _ThresholdCold2Hot = value; }
        }

        public int ThresholdHotTankLowLimit
        {
            get { return _ThresholdHotTankLowLimit; }
            set { _ThresholdHotTankLowLimit = value; }
        }

        public int ThresholdColdTankUpperLimit
        {
            get { return _ThresholdColdTankUpperLimit; }
            set { _ThresholdColdTankUpperLimit = value; }
        }

        /* Buffer tank */
        public int ThresholdColdTankBuffAlarmLimit
        {
            get { return _ThresholdColdTankAlarm; }
            set { _ThresholdColdTankAlarm = value; }
        }

        public int ThresholdHotBuffTankAlarmLimit
        {
            get { return _ThresholdHotTankAlarm; }
            set { _ThresholdHotTankAlarm = value; }
        }

        public int ThresholdMidBuffTankAlarmLimit
        {
            get { return _ThresholdMidTankAlarm; }
            set { _ThresholdMidTankAlarm = value; }
        }

    }

}
