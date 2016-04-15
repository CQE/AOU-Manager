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

        /********************
        ***  Running Mode
        */
        public static string[] RunningModeStrings = new string[] {
                                                                "Idle", // 0, AOUTypes.CommandType.RunningModeIdle
                                                                "Heating", // 1, RunningModeHeating
                                                                "Cooling", // 2, RunningModeCooling
                                                                "Fixed Cycling", // 3, RunningModefixedCycling
                                                                "Auto with IMM" // 4, RunningModeAutoWidthIMM
                                                                };

        public static int RunningMode
        {
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("RunningMode"))
                {
                    return (int)ApplicationData.Current.LocalSettings.Values["RunningMode"];
                }
                else
                {
                    return (int)AOUTypes.AOURunningMode.Idle;
                }
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["RunningMode"] = value;
            }
        }

        /********************/

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
        { 
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
        { 
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("SerialSettings-comport"))
                {
                    string comport = (string)ApplicationData.Current.LocalSettings.Values["SerialSettings-comport"];
                    uint baudrate = (uint)ApplicationData.Current.LocalSettings.Values["SerialSettings-baudrate"];
                    return new AOUSettings.SerialSetting(comport, baudrate);
                }
                else
                {
                    return new AOUSettings.SerialSetting("COM3", 115200); 
                }
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["SerialSettings-comport"] = value.ComPort;
                ApplicationData.Current.LocalSettings.Values["SerialSettings-baudrate"] = value.BaudRate;
            }
        }

        static public AOUSettings.RandomSetting RandomSettings
        { 
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("RandomSettings"))
                {
                    uint msBetween = (uint)ApplicationData.Current.LocalSettings.Values["RandomSettings-msBetween"];
                    uint numValues = (uint)ApplicationData.Current.LocalSettings.Values["RandomSettings-numValues"];
                    return new AOUSettings.RandomSetting(numValues, msBetween);
                }
                else
                {
                    return new AOUSettings.RandomSetting(30, 1000);
                }
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["RandomSettings-msBetween"] = value.MsBetween;
                ApplicationData.Current.LocalSettings.Values["RandomSettings-numValues"] = value.NumValues;
            }
        }

    }

    /// <summary>
    /// Global variables for current session.
    /// </summary>
    public static class GlobalVars
    {
    public static GlobalThresHolds globThresholds;
    public static GlobalDelayTimes globDelayTimes;
    // Important to call this in MainPage constructor. Program crasch If not 
    public static void Init()
    {
    /* Where - this values
        220
        60
    */
        globThresholds = new GlobalThresHolds();
            globThresholds.ThresholdHot2Cold = 100;
            globThresholds.ThresholdCold2Hot = 110;
            globThresholds.ThresholdHotTankLowLimit = 120;
            globThresholds.ThresholdColdTankUpperLimit = 80;

            globThresholds.ThresholdHotBuffTankAlarmLimit = 110;
            globThresholds.ThresholdMidBuffTankAlarmLimit = 100;
            globThresholds.ThresholdColdTankBuffAlarmLimit = 90;

            globDelayTimes = new GlobalDelayTimes();
            globDelayTimes.HotCalibrate = 8;
            globDelayTimes.ColdCalibrate = 7;
            globDelayTimes.HotTune = 3;
            globDelayTimes.ColdTune = 2;
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

    public class GlobalDelayTimes
    {
        private int _hotTune;
        private int _hotCalibrate;
        private int _coldTune;
        private int _coldCalibrate;

        public int HotTune
        {
            get { return _hotTune;}
            set { _hotTune = value; }
        }
        public int HotCalibrate
        {
            get { return _hotCalibrate; }
            set { _hotCalibrate = value; }
        }
        public int ColdTune
        {
            get { return _coldTune; }
            set { _coldTune = value; }
        }
        public int ColdCalibrate
        {
            get { return _coldCalibrate; }
            set { _coldCalibrate = value; }
        }
    }

}
