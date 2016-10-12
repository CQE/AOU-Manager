using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media;

namespace DemoPrototype
{
    public static class GlobalAppSettings
    {

        public static bool valueFeedHaveStarted = false;


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

        public static int GetAOUCmdValue(AOUDataTypes.CommandType cmdType)
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

        public static void SetAOUCmdValue(AOUDataTypes.CommandType cmdType, int value)
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
                    return (int)AOUDataTypes.AOURunningMode.Idle;
                }
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["RunningMode"] = value;
            }
        }

        public static int ToolTempMode
        {
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("ToolTempMode"))
                {
                    return (int)ApplicationData.Current.LocalSettings.Values["ToolTempMode"];
                }
                else
                {
                    return (int)AOUDataTypes.HT_StateType.HT_STATE_NOT_SET;
                }
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["ToolTempMode"] = value;
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

        static public string FileSettingsPath
        { 
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("FileSettings-path"))
                {
                    return (string)ApplicationData.Current.LocalSettings.Values["FileSettings-path"];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["FileSettings-path"] = value;
            }
        }

        static public AOUSettings.SerialSetting SerialSettings
        { 
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("SerialSettings-comport"))
                {
                    string comport = (string)ApplicationData.Current.LocalSettings.Values["SerialSettings-comport"];
                    var baudrate = ApplicationData.Current.LocalSettings.Values["SerialSettings-baudrate"];

                    uint b = 0;
                    if (baudrate is int)
                        b = (uint)(int)baudrate;
                    else
                        b = (uint)baudrate;

                    return new AOUSettings.SerialSetting(comport, b);
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
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("RandomSettings-msBetween"))
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

    public static class Data
    {
        public static DataUpdater Updater;

        public static void Init()
        {
            Updater = new DataUpdater();
        }

    }

    public struct CommandReceived
    {
        public AOUDataTypes.CommandType command;

        // public string valueSent;
        // public DateTime sent;

        public int valueReceived;
        public DateTime received;

        public CommandReceived(AOUDataTypes.CommandType cmd, int val)
        {
            command = cmd;
            valueReceived = val;
            received = DateTime.Now;
        }
    }


    /// <summary>
    /// Global variables for current session.
    /// </summary>
    public static class GlobalVars
    {
        public static GlobalThresHolds globThresholds;
        public static GlobalDelayTimes globDelayTimes;
        public static GlobalFeedTimes globFeedTimes;
        public static GlobalTankSetTemps globTankSetTemps;
        public static GlobalValveChartValues globValveChartValues;
        public static GlobalInitBools globInitBools;

        public static AOUCommands aouCommands;
        // Todo: List sent cmd to change value or list new values received from <ret> or both
        public static List<CommandReceived> retReceived;

        public static GlobalTestSettings globTestSettings; // Only for testing performance

        // Important to call this in MainPage constructor. Program crasch If not 

        public static void GetCommandValue(AOUDataTypes.CommandType cmd)
        {
            // Todo if needed
        }

        public static bool IsCommandValueChanged(AOUDataTypes.CommandType cmd, out int value)
        {
            value = 0; // Init
            int index = retReceived.FindIndex(cr => cr.command == cmd);
            if (index >= 0)
            {
                value = retReceived[index].valueReceived;
                retReceived.RemoveAt(index); // Delete from list when catched
                return true;
            }
            return false;
        }

        public static void SetStaticValues()
        {
            globFeedTimes.CoolingActive = ValueGenerator.GetStaticCommandValue(AOUDataTypes.CommandType.coolingTime);
            globFeedTimes.HeatingActive = ValueGenerator.GetStaticCommandValue(AOUDataTypes.CommandType.heatingTime);

            globFeedTimes.CoolingPause = ValueGenerator.GetStaticCommandValue(AOUDataTypes.CommandType.toolCoolingFeedPause);
            globFeedTimes.HeatingPause = ValueGenerator.GetStaticCommandValue(AOUDataTypes.CommandType.toolHeatingFeedPause);

            globTankSetTemps.ColdTankSetTemp = ValueGenerator.GetStaticCommandValue(AOUDataTypes.CommandType.tempColdTankFeedSet);
            globTankSetTemps.HotTankSetTemp = ValueGenerator.GetStaticCommandValue(AOUDataTypes.CommandType.tempHotTankFeedSet);

            globThresholds.ThresholdCold2Hot = ValueGenerator.GetStaticCommandValue(AOUDataTypes.CommandType.TReturnThresholdCold2Hot); 
            globThresholds.ThresholdHot2Cold = ValueGenerator.GetStaticCommandValue(AOUDataTypes.CommandType.TReturnThresholdHot2Cold);

            globThresholds.ThresholdColdTankUpperLimit = ValueGenerator.GetStaticCommandValue(AOUDataTypes.CommandType.TBufferColdUpperLimit);
            globThresholds.ThresholdMidBuffTankAlarmLimit = ValueGenerator.GetStaticCommandValue(AOUDataTypes.CommandType.TBufferMidRefThreshold);
            globThresholds.ThresholdHotTankLowLimit = ValueGenerator.GetStaticCommandValue(AOUDataTypes.CommandType.TBufferHotLowerLimit);

            globThresholds.ThresholdColdTankBuffAlarmLimit = ValueGenerator.GetStaticCommandValue(AOUDataTypes.CommandType.TColdTankAlarmHighThreshold); 
            globThresholds.ThresholdHotBuffTankAlarmLimit = ValueGenerator.GetStaticCommandValue(AOUDataTypes.CommandType.THotTankAlarmLowThreshold); 
        }

        public static void SetCommandValue(AOUDataTypes.CommandType cmd, string value)
        {
            int ival = int.Parse(value);
            // Add ret cmd and ival to list of changed values. Perhaps cmd sent too
           // retReceived.Add(new CommandReceived(cmd, ival)); MOVED to after switch /MW
            //special handling of times    
            switch (cmd)
            {
                //for delay times, we cannot divide delay into calibrate and tune when returned TODO how hanlde?
                //case AOUDataTypes.CommandType.coldDelayTime: globDelayTimes.ColdCalibrate = ival; break;
                //case AOUDataTypes.CommandType.hotDelayTime: globDelayTimes.HotCalibrate = ival; break;

               // case AOUDataTypes.CommandType.co: globDelayTimes.ColdTune = ival; break;
                //case AOUDataTypes.CommandType.heatingTime: globDelayTimes.HotTune = ival; break;

                case AOUDataTypes.CommandType.coolingTime:  globFeedTimes.CoolingActive = ival/10; ival = ival / 10; break; // globDelayTimes.ColdTune = ival; break;
                case AOUDataTypes.CommandType.heatingTime:  globFeedTimes.HeatingActive = ival / 10; ival = ival / 10;  break;//  globDelayTimes.HotTune = ival; break;
                    
                case AOUDataTypes.CommandType.toolCoolingFeedPause: globFeedTimes.CoolingPause = ival/10; ival = ival / 10; break;
                case AOUDataTypes.CommandType.toolHeatingFeedPause: globFeedTimes.HeatingPause = ival/10; ival = ival / 10; break;

                case AOUDataTypes.CommandType.tempColdTankFeedSet: globTankSetTemps.ColdTankSetTemp = ival; break;
                case AOUDataTypes.CommandType.tempHotTankFeedSet: globTankSetTemps.HotTankSetTemp = ival; break;

                case AOUDataTypes.CommandType.TBufferColdUpperLimit: globThresholds.ThresholdColdTankUpperLimit = ival; break;
                case AOUDataTypes.CommandType.TBufferHotLowerLimit: globThresholds.ThresholdHotTankLowLimit = ival; break;

                case AOUDataTypes.CommandType.THotTankAlarmLowThreshold: globThresholds.ThresholdHotBuffTankAlarmLimit = ival; break;
                case AOUDataTypes.CommandType.TBufferMidRefThreshold: globThresholds.ThresholdMidBuffTankAlarmLimit = ival; break;
                case AOUDataTypes.CommandType.TColdTankAlarmHighThreshold: globThresholds.ThresholdColdTankBuffAlarmLimit = ival; break;

                case AOUDataTypes.CommandType.TReturnThresholdCold2Hot: globThresholds.ThresholdCold2Hot = ival; break;
                case AOUDataTypes.CommandType.TReturnThresholdHot2Cold: globThresholds.ThresholdHot2Cold = ival;  break;
           }
            retReceived.Add(new CommandReceived(cmd, ival));
        }

        public static void Init()
        {
            aouCommands = new AOUCommands();
            retReceived = new List<CommandReceived>();

            globThresholds = new GlobalThresHolds();
            globDelayTimes = new GlobalDelayTimes();
            globFeedTimes = new GlobalFeedTimes();
            globTankSetTemps = new GlobalTankSetTemps(); 
            globValveChartValues = new GlobalValveChartValues();
            globInitBools = new GlobalInitBools();

            globValveChartValues.HotValveLow = 20; // ToDo: Trim
            globValveChartValues.HotValveHi = 24;

            globValveChartValues.ColdValveLow = 26;
            globValveChartValues.ColdValveHi = 32;

            globValveChartValues.ReturnValveLow = 34;
            globValveChartValues.ReturnValveHi = 38;

            globValveChartValues.CoolantValveLow = 40;
            globValveChartValues.CoolantValveHi = 44;


            globTestSettings = new GlobalTestSettings();
            globTestSettings.ChartParameterMonitoringEnabled = true;
            globTestSettings.EnergyBalanceChartEnabled = true;
            globTestSettings.MyTuneChartEnabled = true;
            globTestSettings.StateChartEnabled = true;
            globTestSettings.VolumeBalanceChartEnabled = true;
            globTestSettings.TimeLoggingEnabled = true;

            globTestSettings.DataTimeSpanTicks = 0;
        }

        public static void Save()
        {
            //  To do in future, Maybe for uniqe parameters as mould
        }

        public static void Load()
        {
            //  To do in future, Maybe for uniqe parameters
        }

        public class GlobalInitBools
        {
            private bool _hasAskedAOU;
            public GlobalInitBools()
            {
                _hasAskedAOU = false;
            }
            public bool HasAskedAOU
            {
                get { return _hasAskedAOU; }
                set { _hasAskedAOU = value; }
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

            public GlobalThresHolds()
            {
                _thresholdHot2Cold = int.MinValue;
                _ThresholdCold2Hot = int.MinValue;
                _ThresholdHotTankLowLimit = int.MinValue;
                _ThresholdColdTankUpperLimit = int.MinValue;
                _ThresholdHotTankAlarm = int.MinValue;
                _ThresholdColdTankAlarm = int.MinValue;
                _ThresholdMidTankAlarm = int.MinValue;
            }

            public bool IsAllValuesReceived()
            {
                return
                _thresholdHot2Cold != int.MinValue &&
                _ThresholdCold2Hot != int.MinValue &&
                _ThresholdHotTankLowLimit != int.MinValue &&
                _ThresholdColdTankUpperLimit != int.MinValue &&
                _ThresholdHotTankAlarm != int.MinValue &&
                _ThresholdColdTankAlarm != int.MinValue &&
                _ThresholdMidTankAlarm != int.MinValue;
            }

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

            public string ThresholdHot2ColdStr
            {
                get
                {
                    if (_thresholdHot2Cold == int.MinValue)
                        return "-";
                    else
                        return _thresholdHot2Cold.ToString();
                }
            }

            public string ThresholdCold2HotStr
            {
                get
                {
                    if (_ThresholdCold2Hot == int.MinValue)
                        return "-";
                    else
                        return _ThresholdCold2Hot.ToString();
                }
            }

        }


        public class GlobalTankSetTemps
        {
            private int _hotTankSetTemp;
            private int _coldTankSetTemp;

            public GlobalTankSetTemps()
            {
                _hotTankSetTemp = int.MinValue;
                _coldTankSetTemp = int.MinValue;
            }
            public int HotTankSetTemp
            {
                get { return _hotTankSetTemp; }
                set { _hotTankSetTemp = value; }
            }
            public int ColdTankSetTemp
            {
                get { return _coldTankSetTemp; }
                set { _coldTankSetTemp = value; }
            }

        }

        public class GlobalDelayTimes
        {
            private int _hotTune;
            private int _hotCalibrate;
            private int _coldTune;
            private int _coldCalibrate;

            public GlobalDelayTimes()
            {
                //these are only local so we can initialise to 0 we will never receive these from AOU. Maybe save locally and read from file later TODO
                _hotTune = 0;// int.MinValue;
                _hotCalibrate = 0;// int.MinValue;
                _coldTune = 0;// int.MinValue;
                _coldCalibrate = 0;// int.MinValue;
            }

            public bool IsAllValuesReceived()
            {
                return
                _hotTune != int.MinValue &&
                _hotCalibrate != int.MinValue &&
                _coldTune != int.MinValue &&
                _coldCalibrate != int.MinValue;
            }

            public string ColdDelayTimeSumStr
            {
                get
                {
                    if (_coldTune == int.MinValue && _coldCalibrate == int.MinValue)
                        return "-";
                    else
                        return ( _coldCalibrate + _coldTune).ToString();
                }
            }

            public string HotDelayTimeSumStr
            {
                get
                {
                    if (_hotTune == int.MinValue && _hotCalibrate == int.MinValue)
                        return "-";
                    else
                        return (_hotCalibrate + _hotTune).ToString();
                }
            }

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

            public string HotTuneStr
            {
                get
                {
                    if (_hotTune == int.MinValue)
                        return "-";
                    else
                        return _hotTune.ToString();
                }
            }

            public string HotCalibrateStr
            {
                get
                {
                    if (_hotTune == int.MinValue)
                        return "-";
                    else
                        return _hotTune.ToString();
                }
            }

            public string ColdTuneStr
            {
                get
                {
                    if (_coldTune == int.MinValue)
                        return "-";
                    else
                        return _hotTune.ToString();
                }
            }

            public string ColdCalibrateStr
            {
                get
                {
                    if (_coldCalibrate == int.MinValue)
                        return "-";
                    else
                        return _coldCalibrate.ToString();
                }
            }


        }

        public class GlobalFeedTimes
        {
            private int _heatingActive;
            private int _heatingPause;
            private int _coolingActive;
            private int _coolingPause;

            public GlobalFeedTimes()
            {
                _heatingActive = int.MinValue;
                _heatingPause = int.MinValue;
                _coolingActive = int.MinValue;
                _coolingPause = int.MinValue;
            }

            public bool IsAllValuesReceived()
            {
                return
                _heatingActive != int.MinValue &&
                _heatingPause != int.MinValue &&
                _coolingActive != int.MinValue &&
                _coolingPause != int.MinValue;
            }

            public int HeatingActive
            {
                get { return _heatingActive; }
                set { _heatingActive = value; }
            }
            public int HeatingPause
            {
                get { return _heatingPause; }
                set { _heatingPause = value; }
            }
            public int CoolingActive
            {
                get { return _coolingActive; }
                set { _coolingActive = value; }
            }
            public int CoolingPause
            {
                get { return _coolingPause; }
                set { _coolingPause = value; }
            }

            public string HeatingActiveStr
            {
                get
                {
                    if (_heatingActive == int.MinValue)
                        return "-";
                    else
                        return (_heatingActive/10).ToString(); //converts deciseconds to seconds
                }
            }
            public string HeatingPauseStr
            {
                get
                {
                    if (_heatingPause == int.MinValue)
                        return "-";
                    else
                        return _heatingPause.ToString();
                }
            }
            public string CoolingActiveStr
            {
                get
                {
                    if (_coolingActive == int.MinValue)
                        return "-";
                    else
                        return _coolingActive.ToString();
                }
            }
            public string CoolingPauseStr
            {
                get
                {
                    if (_coolingPause == int.MinValue)
                        return "-";
                    else
                        return _coolingPause.ToString();
                }
            }
        }

        public class GlobalValveChartValues
        { 
            private int _hotValveLow;
            private int _hotValveHi;

            private int _coldValveLow;
            private int _coldValveHi;

            private int _returnValveLow;
            private int _returnValveHi;

            private int _coolantValveLow;
            private int _coolantValveHi;


            public int HotValveLow
            {
                get { return _hotValveLow; }
                set { _hotValveLow = value; }
            }
            public int HotValveHi
            {
                get { return _hotValveHi; }
                set { _hotValveHi = value; }
            }

            public int ColdValveLow
            {
                get { return _coldValveLow; }
                set { _coldValveLow = value; }
            }
            public int ColdValveHi
            {
                get { return _coldValveHi; }
                set { _coldValveHi = value; }
            }

            public int ReturnValveLow
            {
                get { return _returnValveLow; }
                set { _returnValveLow = value; }
            }
            public int ReturnValveHi
            {
                get { return _returnValveHi; }
                set { _returnValveHi = value; }
            }

            public int CoolantValveLow
            {
                get { return _coolantValveLow; }
                set { _coolantValveLow = value; }
            }
            public int CoolantValveHi
            {
                get { return _coolantValveHi; }
                set { _coolantValveHi = value; }
            }

        }



        

        public class GlobalTestSettings
        {
            private bool _ChartParameterMonitoringEnabled;
            private bool _MyTuneChartEnabled;
            private bool _StateChartEnabled;
            private bool _VolumeBalanceChartEnabled;
            private bool _EnergyBalanceChartEnabled;
            private bool _TimeLoggingEnabled;

            private long _dataTimeSpanTicks;

            public bool ChartParameterMonitoringEnabled
            {
                get { return _ChartParameterMonitoringEnabled; }
                set { _ChartParameterMonitoringEnabled = value; }
            }

            public bool MyTuneChartEnabled
            {
                get { return _MyTuneChartEnabled; }
                set { _MyTuneChartEnabled = value; }
            }

            public bool StateChartEnabled
            {
                get { return _StateChartEnabled; }
                set { _StateChartEnabled = value; }
            }

            public bool VolumeBalanceChartEnabled
            {
                get { return _VolumeBalanceChartEnabled; }
                set { _VolumeBalanceChartEnabled = value; }
            }

            public bool EnergyBalanceChartEnabled
            {
                get { return _EnergyBalanceChartEnabled; }
                set { _EnergyBalanceChartEnabled = value; }
            }

            public bool TimeLoggingEnabled
            {
                get { return _TimeLoggingEnabled; }
                set { _TimeLoggingEnabled = value; }
            }

            public long DataTimeSpanTicks
            {
                get { return _dataTimeSpanTicks; }
                set { _dataTimeSpanTicks = value; }
            }

        }
    }

    public static class AppColors
    {
        public static SolidColorBrush red {
            get { return new SolidColorBrush(Windows.UI.Colors.DarkRed); }
        }

        public static SolidColorBrush blue { get { return new SolidColorBrush(Windows.UI.Colors.DarkSlateBlue); } }

        public static SolidColorBrush green { get { return new SolidColorBrush(Windows.UI.Colors.DarkGreen); } }

        public static SolidColorBrush gray { get { return new SolidColorBrush(Windows.UI.Colors.DarkSlateGray); } }
    }

}
