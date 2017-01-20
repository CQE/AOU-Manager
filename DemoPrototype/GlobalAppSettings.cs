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

        static public heatTransferFluidsList TransferFluidsList
        {
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("HeatTransferFluidsList"))
                {
                    var xml = (string)ApplicationData.Current.LocalSettings.Values["HeatTransferFluidsList"];
                    var list = new HeatTransferFluidsList(xml);
                    return list.GetHeatTransferFluidsList();
                }
                else
                {
                    var list = new HeatTransferFluidsList(); // Use texatherm32.xml as default
                    return list.GetHeatTransferFluidsList();
                }
            }
            set
            {
                var list = new HeatTransferFluidsList(value);
                ApplicationData.Current.LocalSettings.Values["HeatTransferFluidsList"] = list.GetXMLStringFromFluidList();
            }
        }

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
        public static GlobalSafetyAlarms globSafetyAlarms;
        public static GlobalInitBools globInitBools;
        public static GlobalRemoteSettings globRemoteSettings;
        public static GlobalLogSettings globLogSettings;
        public static GlobalMisc globMisc;


        public static AOUCommands aouCommands;
        // Todo: List sent cmd to change value or list new values received from <ret> or both
        public static List<CommandReceived> retReceived;



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
            //return; //testing purposes
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

        public static void SetCommandValue(AOUDataTypes.CommandType cmd, string value="0")
        {
            int ival;
            double dval;
            if (int.TryParse(value, out ival))
            {
                //success
            }
            else
            {
                //check for null
                if (value == null)
                    return;
                //check if "0x000"-format
                //strip 0x
                string hexString = value.Substring(2); //  "0004";
                ival = int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
            }
            //int ival = int.TryParse(value);
            // Add ret cmd and ival to list of changed values. Perhaps cmd sent too
            // retReceived.Add(new CommandReceived(cmd, ival)); MOVED to after switch /MW
            //special handling of times    
            switch (cmd)
            {
                case AOUDataTypes.CommandType.forceValves:
                    {
                        //divide into mask ant state
                        // -- VALVES -- <Valves>MMSS</Valves> MASK (e.g. “3F”), STATE Bits: 0/Hot valve, 1/Cold valve, 2/Return valve, 4/Coolant valve
                        byte mask = (Byte)(ival & 0xff);
                        byte state = (Byte)(ival >> 8);

                        //gl

                        //if (IsStateSet(mask, VALVE_HOT)) currentHotValve = GetValveState(state, VALVE_HOT);
                        //if (IsStateSet(mask, VALVE_COLD)) currentColdValve = GetValveState(state, VALVE_COLD);
                        //if (IsStateSet(mask, VALVE_RET)) currentReturnValve = GetValveState(state, VALVE_RET);
                        //if (IsStateSet(mask, VALVE_COOL)) currentCoolantValve = GetValveState(state, VALVE_COOL);



                        break;
                    }
                //two new commands HOW    HANDLE?

                case AOUDataTypes.CommandType.HOTMO2REDELAYTIM  :  dval = (double)(ival) / 10;   ival = ival / 10;  break;
                case AOUDataTypes.CommandType.COLDMO2REDELAYTIM: dval = (double)(ival) / 10; ival = ival / 10; break;


                //for delay times, we cannot divide delay into calibrate and tune when returned TODO how hanlde?

                case AOUDataTypes.CommandType.coldDelayTime:
                    {
                        //must check if we have values
                        dval = (double)ival / 10;
                        ival = ival / 10;
                        if (globDelayTimes.ColdCalibrate < -1000 && globDelayTimes.ColdTune < -1000)
                        {
                            //first time
                            globDelayTimes.ColdCalibrate = dval;
                            globDelayTimes.ColdTune = 0;
                        }
                        if (globDelayTimes.ColdCalibrate > -1000 && globDelayTimes.ColdTune > -1000)
                        {
                            //both have values, just ignore
                            //should perhaps show message here if sum does not match
                        }
                        else
                        {
                            if (globDelayTimes.ColdCalibrate > -1000)
                            {
                                //set tune
                                globDelayTimes.ColdTune = dval - globDelayTimes.ColdCalibrate;
                            }
                            else
                            {
                                //set calibrate
                                globDelayTimes.ColdCalibrate = dval - globDelayTimes.ColdTune;
                            }

                        }
                        break;
                    }


                case AOUDataTypes.CommandType.hotDelayTime:
                { 
                    //must check if we have values
                    dval = (double)ival / 10;
                    ival = ival / 10;
                    if (globDelayTimes.HotCalibrate < -1000 && globDelayTimes.HotTune < -1000)
                    {
                        //first time
                        globDelayTimes.HotCalibrate = dval;
                        globDelayTimes.HotTune = 0;
                    }
                    if (globDelayTimes.HotCalibrate > -1000 && globDelayTimes.HotTune > -1000)
                    {
                        //both have values, just ignore
                        //should perhaps show message here if sum does not match
                    }
                    else
                    {
                        if (globDelayTimes.HotCalibrate > -1000)
                        {
                            //set tune
                            globDelayTimes.HotTune = dval - globDelayTimes.HotCalibrate;
                        }
                        else
                        {
                            //set calibrate
                            globDelayTimes.HotCalibrate = dval - globDelayTimes.HotTune;
                        }

                    }
                    break;
                }

               



                case AOUDataTypes.CommandType.runModeAOU:
                    {
                        GlobalAppSettings.RunningMode = ival/4;
                        //this only works for 0,1,2

                    } break;

                case AOUDataTypes.CommandType.coolingTime:  globFeedTimes.CoolingActive = ival/10; ival = ival / 10; break; // globDelayTimes.ColdTune = ival; break;
                case AOUDataTypes.CommandType.heatingTime:  globFeedTimes.HeatingActive = ival / 10; ival = ival / 10;  break;//  globDelayTimes.HotTune = ival; break;
                    
                case AOUDataTypes.CommandType.toolCoolingFeedPause: globFeedTimes.CoolingPause = ival/10; ival = ival / 10; break;
                case AOUDataTypes.CommandType.toolHeatingFeedPause: globFeedTimes.HeatingPause = ival/10; ival = ival / 10; break;

                case AOUDataTypes.CommandType.offsetHotFeed2RetValveTime:
                    {
                        //must check if we have values
                        dval = (double)ival / 10;
                        ival = ival / 10;
                        if (globDelayTimes.EACalibrate < -1000 && globDelayTimes.EATune < -1000)
                        {
                            //first time
                            globDelayTimes.EACalibrate = dval;
                            globDelayTimes.EATune = 0;
                        }
                        if (globDelayTimes.EACalibrate > -1000 && globDelayTimes.EATune > -1000)
                        {
                            //both have values, just ignore
                            //should perhaps show message here if sum does not match
                        }
                        else
                        {
                            if (globDelayTimes.EACalibrate > -1000)
                            {
                                //set tune
                                globDelayTimes.EATune = dval - globDelayTimes.EACalibrate;
                            }
                            else
                            {
                                //set calibrate
                                globDelayTimes.EACalibrate = dval - globDelayTimes.EATune;
                            }
                     
                        }
                        break;
                    }


                case AOUDataTypes.CommandType.offsetRetValveHotPeriod:
                    {
                        //must check if we have values
                        dval = (double)ival / 10;
                        ival = ival / 10;
                     
                        if (globDelayTimes.VACalibrate < -1000 && globDelayTimes.VATune < -1000)
                        {
                            //first time
                            globDelayTimes.VACalibrate = dval;
                            globDelayTimes.VATune = 0;
                        }
                        if (globDelayTimes.VACalibrate > -1000 && globDelayTimes.VATune > -1000)
                        {
                            //both have values, just ignore
                            //should perhaps show message here if sum does not match
                        }
                        else
                        {
                            if (globDelayTimes.VACalibrate > -1000)
                            {
                                //set tune
                                globDelayTimes.VATune = dval - globDelayTimes.VACalibrate;
                            }
                            else
                            {
                                //set calibrate
                                globDelayTimes.VACalibrate = dval - globDelayTimes.VATune;
                            }

                        }
                        break;
                    }
                    
               case AOUDataTypes.CommandType.hotFeed2MouldDelayTime: 
                    {
                        //must check if we have values
                        dval = (double)ival / 10;
                        ival = ival / 10;
                    
                        if (globDelayTimes.F2MCalibrateUsed < -1000 && globDelayTimes.F2MTuneUsed < -1000)
                        {
                            //first time
                            globDelayTimes.F2MCalibrateUsed = dval;
                            globDelayTimes.F2MTuneUsed = 0;
                        }
                        if (globDelayTimes.F2MCalibrateUsed > -1000 && globDelayTimes.F2MTuneUsed > -1000)
                        {
                            //both have values, just ignore
                            //should perhaps show message here if sum does not match
                        }
                        else
                        {
                            if (globDelayTimes.F2MCalibrateUsed > -1000)
                            {
                                //set tune
                                globDelayTimes.F2MTuneUsed = dval - globDelayTimes.F2MCalibrateUsed;
                            }
                            else
                            {
                                //set calibrate
                                globDelayTimes.F2MCalibrateUsed = dval - globDelayTimes.F2MTuneUsed;
                            }

                        }
                        break;
                    }


                case AOUDataTypes.CommandType.coldFeed2MouldDelayTime:
                    {
                        //must check if we have values
                        ival = ival / 10;
                        //if (globDelayTimes.F < -1000 && globDelayTimes.F2MTuneUsed < -1000)
                        //{
                        //    //first time
                        //    globDelayTimes.F2MCalibrateUsed = ival;
                        //    globDelayTimes.F2MTuneUsed = 0;
                        //}
                        //if (globDelayTimes.F2MCalibrateUsed > -1000 && globDelayTimes.F2MTuneUsed > -1000)
                        //{
                        //    //both have values, just ignore
                        //    //should perhaps show message here if sum does not match
                        //}
                        //else
                        //{
                        //    if (globDelayTimes.F2MCalibrateUsed > -1000)
                        //    {
                        //        //set tune
                        //        globDelayTimes.F2MTuneUsed = ival - globDelayTimes.F2MCalibrateUsed;
                        //    }
                        //    else
                        //    {
                        //        //set calibrate
                        //        globDelayTimes.F2MCalibrateUsed = ival - globDelayTimes.F2MTuneUsed;
                        //    }

                        //}
                        break;
                    }
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
            globSafetyAlarms = new GlobalSafetyAlarms();
            globInitBools = new GlobalInitBools();
            //set running mode to idle
            GlobalAppSettings.RunningMode = 0;

            globValveChartValues.HotValveLow = 18; // ToDo: Trim
            globValveChartValues.HotValveHi = 24;
            globValveChartValues.HotValveValue = 0; //off

            globValveChartValues.ColdValveLow = 26;
            globValveChartValues.ColdValveHi = 32;
            globValveChartValues.ColdValveValue = 0;

            globValveChartValues.ReturnValveLow = 34;
            globValveChartValues.ReturnValveHi = 38;
            globValveChartValues.ReturnValveValue = 0;

            globValveChartValues.CoolantValveLow = 40;
            globValveChartValues.CoolantValveHi = 46;
            globValveChartValues.CoolantValveValue = 0;

            globSafetyAlarms.SafetyEmergencyHi = 1;
            globSafetyAlarms.SafetyStopHi = 1;
            globSafetyAlarms.SafetyResetHi = 1;
            globSafetyAlarms.SafetyFluidLevelHi = 1;
            globSafetyAlarms.SafetyOverHeatedHi = 1;


            /*
              * Init Log and test settings.
              * Possible to change settings depending of being in debug or release mode

 #if DEBUG
             globLogSettings.LogLevel = 1;
 #else
             globLogSettings.LogLevel = 0;
 #endif
              */

            globLogSettings = new GlobalLogSettings();
            globLogSettings.LogLevel = 0;
            globLogSettings.LogToFile = true;
            globLogSettings.LogToWeb = false;


            /* Old test
            globTestSettings.ChartParameterMonitoringEnabled = true;
            globTestSettings.EnergyBalanceChartEnabled = true;
            globTestSettings.MyTuneChartEnabled = true;
            globTestSettings.StateChartEnabled = true;
            globTestSettings.VolumeBalanceChartEnabled = true;
            globTestSettings.TimeLoggingEnabled = true;
            globTestSettings.DataTimeSpanTicks = 0;
            globTestSettings = new GlobalTestSettings();

            globTestSettings.ChartParameterMonitoringEnabled = true;
            globTestSettings.EnergyBalanceChartEnabled = true;
            globTestSettings.MyTuneChartEnabled = true;
            globTestSettings.StateChartEnabled = true;
            globTestSettings.VolumeBalanceChartEnabled = true;
            globTestSettings.TimeLoggingEnabled = true;

            globTestSettings.DataTimeSpanTicks = 0;
            */

            globRemoteSettings = new GlobalRemoteSettings();


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
            private double _hotTune;
            private double _hotCalibrate;  //sum = hotDelayTime
            private double _coldTune;
            private double _coldCalibrate;  // sum = coldDelayTime
            private double _F2MHotCalibrate;
            private double _F2MHotTune;         //sum = ColdF2MDelayTime   
            private double _F2MColdCalibrate;
            private double _F2MColdTune;         //sum = ColdF2MDelayTime   
            private double _F2MCalibrateUsed;
            private double _F2MTuneUsed;
            private double _hotMoInOut;
            private double _coldMoInOut;
            private double _EACalibrate;
            private double _EATune;
            private double _VACalibrate;
            private double _VATune;
            private double _hotStep;
            private double _coldStep;
            private bool _feedShareAutoMode;
            private bool _mouldShareAutoMode;
            private bool _EAAutoMode;
            private bool _VAAutoMode;

            private double n = 0.45;

            public GlobalDelayTimes()
            {
                //these are only local so we can initialise to 0 we will never receive these from AOU. Maybe save locally and read from file later TODO
                _hotTune = int.MinValue;
                _hotCalibrate = int.MinValue;
                _hotMoInOut = int.MinValue;
                _coldMoInOut = int.MinValue;
                _coldTune =  int.MinValue;
                _coldCalibrate = int.MinValue;
                _F2MHotTune = int.MinValue;
                _F2MHotCalibrate = int.MinValue;
                _F2MColdTune = int.MinValue;
                _F2MColdCalibrate = int.MinValue;
                _F2MTuneUsed = int.MinValue;
                _F2MCalibrateUsed = int.MinValue;
                _EATune = int.MinValue;
                _EACalibrate = int.MinValue;
                _VATune = int.MinValue;
                _VACalibrate = int.MinValue;
                _feedShareAutoMode = true;
                _mouldShareAutoMode = true;
                _EAAutoMode = true;
                _VAAutoMode = true;

                _hotStep = 0; //only local
                _coldStep = 0; //only local
            }

            public bool IsAllValuesReceived()
            {
                return  //TODO not ready MW
                _hotTune != int.MinValue &&
                _hotCalibrate != int.MinValue &&
                _coldTune != int.MinValue &&
                _coldCalibrate != int.MinValue;
            }

            public bool FeedShareAutoMode
            {
                get { return _feedShareAutoMode; }
                set { _feedShareAutoMode = value; }
            }

            public bool MouldShareAutoMode
            {
                get { return _mouldShareAutoMode; }
                set { _mouldShareAutoMode = value; }
            }
            public bool EAAutoMode
            {
                get { return _EAAutoMode; }
                set { _EAAutoMode = value; }
            }
            public bool VAAutoMode
            {
                get { return _VAAutoMode; }
                set { _VAAutoMode = value; }
            }

            public double FeedShareVal
            {
                get
                {
                    if (_hotMoInOut == int.MinValue || _hotCalibrate == int.MinValue || _F2MHotCalibrate == int.MinValue)
                        return 0;
                    else
                        return (_F2MHotCalibrate/(_hotCalibrate- _F2MHotCalibrate - _hotMoInOut));
                }
            }

            public double MouldShareVal
            {
                get
                {
                    if (_hotMoInOut == int.MinValue || _hotCalibrate == int.MinValue )
                        return 0;
                    else
                        return ( _hotMoInOut/ _hotCalibrate );
                }
            }


            public double F2MHotCalibrateAuto (double share)
            {
                    if (_hotCalibrate == int.MinValue || _hotMoInOut == int.MinValue)
                        return int.MinValue;
                    else
                        return (_hotCalibrate - _hotMoInOut) * share/(1+share);
               
            }


            public double F2MColdCalibrateAuto(double share)
            {
                if (_coldCalibrate == int.MinValue || _coldMoInOut == int.MinValue)
                    return int.MinValue;
                else
                    return (_coldCalibrate - _coldMoInOut) * share / (1 + share);

            }

            public double HotCalibrateMin
            { //Total >= feed + mould
                get
                {
                    if (_hotMoInOut == int.MinValue || _F2MHotCalibrate == int.MinValue)
                        return 0;
                    else
                        return Math.Abs(_F2MHotCalibrate+_hotMoInOut);
                }
            }

            public double F2MHotCalibrateMax(bool mouldIsAuto)
            {
                
                    if (_hotCalibrate == int.MinValue || _hotMoInOut == int.MinValue)
                        return 90;
                    else
                    if (mouldIsAuto)
                        return Math.Abs(_hotCalibrate);
                    else
                        return Math.Abs(_hotCalibrate - _hotMoInOut);
             
            }


            public double HotMoInOutMax(bool feedIsAuto)
            {

                if (_hotCalibrate == int.MinValue || _F2MHotCalibrate == int.MinValue)
                    return 90;
                else
                {
                    if (feedIsAuto)
                        return Math.Abs(_hotCalibrate);
                    else
                        return Math.Abs(_hotCalibrate - _F2MHotCalibrate);
                }
                               
            }



            public string HotRetHoseStr
                //logic: all three parts needs to be defined
            {
                get
                {
                    if (_hotMoInOut == int.MinValue || _hotCalibrate == int.MinValue || _F2MHotCalibrate == int.MinValue )
                        return "-";
                    else
                        return (_hotCalibrate - _F2MHotCalibrate - _hotMoInOut).ToString("0.##");
                }
            }

            public double HotRetHoseVal
            //logic: all three parts needs to be defined
            {
                get
                {
                    if (_hotMoInOut == int.MinValue || _hotCalibrate == int.MinValue || _F2MHotCalibrate == int.MinValue)
                        return int.MinValue; //or min?
                    else
                        return (_hotCalibrate - _F2MHotCalibrate - _hotMoInOut);
                }
            }



            public string ColdRetHoseStr
            {
                get
                {
                    if (_coldMoInOut == int.MinValue || _coldCalibrate == int.MinValue || _F2MColdCalibrate == int.MinValue)
                        return "-";
                    else
                        return (_coldCalibrate - _F2MColdCalibrate - _coldMoInOut).ToString("0.##");
                }
            }

            public string ColdDelayTimeSumStr
            {
                get
                {
                    if (_coldTune == int.MinValue || _coldCalibrate == int.MinValue)
                        return "-";
                    else
                        return ( _coldCalibrate + _coldTune).ToString("0.##");
                }
            }

            public string HotDelayTimeSumStr
                //old version. Keep.
            {
                get
                {
                    if (_hotTune == int.MinValue || _hotCalibrate == int.MinValue)
                        return "-";
                    else
                        return (_hotCalibrate + _hotTune).ToString("0.##");
                }
            }

            public string TotalHotDelayTimeSumStr
            //new logic
            {
                get
                {
                    if (_hotCalibrate == int.MinValue)
                        return "-";
                    else
                        return _hotCalibrate.ToString("0.#");
                }
            }



            public string F2MColdTimeSumStr
            {
                get
                {
                    if (_F2MColdTune == int.MinValue || _F2MColdCalibrate == int.MinValue)
                        return "-";
                    else
                        return (_F2MColdCalibrate + _F2MColdTune).ToString();
                }
            }

            public string F2MHotTimeSumStr
            {
                get
                {
                    if (_F2MHotTune == int.MinValue || _F2MHotCalibrate == int.MinValue)
                        return "-";
                    else
                        return (_F2MHotCalibrate + _F2MHotTune).ToString();
                }
            }

            public string F2MUsedSumStr
            {
                get
                {
                    if (_F2MTuneUsed == int.MinValue || _F2MCalibrateUsed == int.MinValue)
                        return "-";
                    else
                        return (_F2MCalibrateUsed + _F2MTuneUsed).ToString();
                }
            }

            public string EASumStr
            {
                get
                {
                    if (_EACalibrate == int.MinValue || _EATune == int.MinValue)
                        return "-";
                    else
                        return (_EACalibrate + _EATune).ToString();
                }
            }
            public string VASumStr
            {
                get
                {
                    if (_VACalibrate == int.MinValue || _VATune == int.MinValue)
                        return "-";
                    else
                        return (_VACalibrate + _VATune).ToString();
                }
            }

            public string F2MTotalComputedSumStr
            {
                get
                {
                    if (_hotCalibrate == int.MinValue || _hotTune == int.MinValue)
                        return "-";
                    else
                    {
                        double dval = (_hotCalibrate + _hotTune) * n;
                        return dval.ToString("0.##");
                    }
                        
                }
            }


            public double HotTune
            {
                get { return _hotTune;}
                set { _hotTune = value; }
            }
            public double HotCalibrate
            {
                get { return _hotCalibrate; }
                set { _hotCalibrate = value; }
            }
            public double ColdTune
            {
                get { return _coldTune; }
                set { _coldTune = value; }
            }
            public double ColdCalibrate
            {
                get { return _coldCalibrate; }
                set { _coldCalibrate = value; }
            }



            public double HotMoInOut
            {
                get { return _hotMoInOut; }
                set { _hotMoInOut = value; }
            }

            public double ColdMoInOut
            {
                get { return _coldMoInOut; }
                set { _coldMoInOut = value; }
            }


            public double F2MColdTune
            {
                get { return _F2MColdTune; }
                set { _F2MColdTune = value; }
            }
            public double F2MColdCalibrate
            {
                get { return _F2MColdCalibrate; }
                set { _F2MColdCalibrate = value; }
            }

            public double F2MHotTune
            {
                get { return _F2MHotTune; }
                set { _F2MHotTune = value; }
            }
            public double F2MHotCalibrate
            {
                get { return _F2MHotCalibrate; }
                set { _F2MHotCalibrate = value; }
            }

           

            public double F2MTuneUsed
            {
                get { return _F2MTuneUsed; }
                set { _F2MTuneUsed = value; }
            }
            public double F2MCalibrateUsed
            {
                get { return _F2MCalibrateUsed; }
                set { _F2MCalibrateUsed = value; }
            }
            public double EATune
            {
                get { return _EATune; }
                set { _EATune = value; }
            }
            public double EACalibrate
            {
                get { return _EACalibrate; }
                set { _EACalibrate = value; }
            }
            public double VATune
            {
                get { return _VATune; }
                set { _VATune = value; }
            }
            public double VACalibrate
            {
                get { return _VACalibrate; }
                set { _VACalibrate = value; }
            }

            public double HotStep
            {
                get { return _hotStep; }
                set { _hotStep = value; }
            }

            public double ColdStep
            {
                get { return _coldStep; }
                set { _coldStep = value; }
            }

            public string HotMoInOutStr
            {
                get
                {
                    if (_hotMoInOut == int.MinValue)
                        return "-";
                    else
                        return _hotMoInOut.ToString("0.##");
                }
            }

            public string ColdMoInOutStr
            {
                get
                {
                    if (_coldMoInOut == int.MinValue)
                        return "-";
                    else
                        return _coldMoInOut.ToString("0.##");
                }
            }


            public string HotF2MStr
            {
                get
                {
                    if (_F2MHotCalibrate == int.MinValue)
                        return "-";
                    else
                        return _F2MHotCalibrate.ToString("0.##");
                }
            }

            public string ColdF2MStr
            {
                get
                {
                    if (_F2MColdCalibrate == int.MinValue)
                        return "-";
                    else
                        return _F2MColdCalibrate.ToString("0.##");
                }
            }

            public string HotTuneStr
            {
                get
                {
                    if (_hotTune == int.MinValue)
                        return "-";
                    else
                        return _hotTune.ToString("0.##");
                }
            }

            public string HotCalibrateStr
            {
                get
                {
                    if (_hotCalibrate == int.MinValue)
                        return "-";
                    else
                        return _hotCalibrate.ToString("0.##");
                }
            }


            public string EATotalStr
            {
                get
                {
                    if (_hotCalibrate == int.MinValue)
                        return "-";
                    else
                        if (_EATune == int.MinValue)
                    { return _hotCalibrate.ToString("0.##"); }
                    else
                    {
                        return (_hotCalibrate + _EATune).ToString("0.##");
                    }
                      
                }
            }



            public string ColdTuneStr
            {
                get
                {
                    if (_coldTune == int.MinValue)
                        return "-";
                    else
                        return _coldTune.ToString("0.##");
                }
            }

            public string ColdCalibrateStr
            {
                get
                {
                    if (_coldCalibrate == int.MinValue)
                        return "-";
                    else
                        return _coldCalibrate.ToString("0.##");
                }
            }

            public string F2MCalibrateUsedStr
            {
                get
                {
                    if (_F2MCalibrateUsed == int.MinValue)
                        return "-";
                    else
                        return _F2MCalibrateUsed.ToString("0.##");
                }
            }

            public string F2MCalibrateComputedStr
            {
                get
                {
                    if (_hotCalibrate == int.MinValue)
                        return "-";
                    else
                        return (_hotCalibrate*n).ToString("0.##");
                }
            }

            public string F2MTuneUsedStr
            {
                get
                {
                    if (_F2MTuneUsed == int.MinValue)
                        return "-";
                    else
                        return _F2MTuneUsed.ToString();
                }
            }

            public string F2MTuneComputedStr
            {
                get
                {
                    if (_hotTune == int.MinValue)
                        return "-";
                    else
                        return (_hotTune*n).ToString("0.##");
                }
            }

            public string EACalibrateStr
            {
                get
                {
                    if (_EACalibrate == int.MinValue)
                        return "-";
                    else
                        return _EACalibrate.ToString();
                }
            }
            public string EATuneStr
            {
                get
                {
                    if (_EATune == int.MinValue)
                        return "-";
                    else
                        return _EATune.ToString();
                }
            }

            public string VACalibrateStr
            {
                get
                {
                    if (_VACalibrate == int.MinValue)
                        return "-";
                    else
                        return _VACalibrate.ToString();
                }
            }
            public string VATuneStr
            {
                get
                {
                    if (_VATune == int.MinValue)
                        return "-";
                    else
                        return _VATune.ToString();
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
            private int _hotValveValue;

            private int _coldValveLow;
            private int _coldValveHi;
            private int _coldValveValue;

            private int _returnValveLow;
            private int _returnValveHi;
            private int _returnValveValue;

            private int _coolantValveLow;
            private int _coolantValveHi;
            private int _coolantValveValue;


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

            public int HotValveValue
            {
                get { return _hotValveValue; }
                set { _hotValveValue = value; }
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
            public int ColdValveValue
            {
                get { return _coldValveValue; }
                set { _coldValveValue = value; }
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
            public int ReturnValveValue
            {
                get { return _returnValveValue; }
                set { _returnValveValue = value; }
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
            public int CoolantValveValue
            {
                get { return _coolantValveValue; }
                set { _coolantValveValue = value; }
            }

        }

        public class GlobalSafetyAlarms        {
            private int _safetyStopLow;
            private int _safetyStopHi;

            private int _safetyResetLow;
            private int _safetyResetHi;

            private int _safetyEmergencyLow;
            private int _safetyEmergencyHi;

            private int _safetyFluidLevelLow;
            private int _safetyFluidLevelHi;

            private int _safetyOverHeatedLow;
            private int _safetyOverHeatedHi;


            public int SafetyStopLow
            {
                get { return _safetyStopLow; }
                set { _safetyStopLow = value; }
            }

            public int SafetyStopHi
            {
                get { return _safetyStopHi; }
                set { _safetyStopHi = value; }
            }

            public int SafetyResetLow
            {
                get { return _safetyResetLow; }
                set { _safetyResetLow = value; }
            }

            public int SafetyResetHi
            {
                get { return _safetyResetHi; }
                set { _safetyResetHi = value; }
            }

            public int SafetyEmergencyLow
            {
                get { return _safetyEmergencyLow; }
                set { _safetyEmergencyLow = value; }
            }

            public int SafetyEmergencyHi
            {
                get { return _safetyEmergencyHi; }
                set { _safetyEmergencyHi = value; }
            }

            public int SafetyFluidLevelLow
            {
                get { return _safetyFluidLevelLow; }
                set { _safetyFluidLevelLow = value; }
            }

            public int SafetyFluidLevelHi
            {
                get { return _safetyFluidLevelHi; }
                set { _safetyFluidLevelHi = value; }
            }
            public int SafetyOverHeatedLow
            {
                get { return _safetyOverHeatedLow; }
                set { _safetyOverHeatedLow = value; }
            }

            public int SafetyOverHeatedHi
            {
                get { return _safetyOverHeatedHi; }
                set { _safetyOverHeatedHi = value; }
            }


        }


        public class GlobalMisc
        {
            private double _delayTimeConst;

            public double DelayTimeConst
            {
                get { return _delayTimeConst; }
                set { _delayTimeConst = value; }
            }
        }



        public class GlobalRemoteSettings
        {
            private bool _on;
            private string _uri;
            private string _password;

            public string URI
            {
                get { return _uri; }
                set { _uri = value; }
            }

            public string password
            {
                get { return _password; }
                set { _password = value; }
            }

            public bool On
            {
                get { return _on; }
                set { _on = value; }
            }


        }

        public class GlobalLogSettings
        {
            private bool _LogToFile;
            private bool _LogToWeb;
            private int _LogLevel;

            public bool LogToFile
            {
                get { return _LogToFile; }
                set { _LogToFile = value; }
            }

            public bool LogToWeb
            {
                get { return _LogToWeb; }
                set { _LogToWeb = value; }
            }

            public int LogLevel
            {
                get { return _LogLevel; }
                set { _LogLevel = value; }
            }

            /* Old test
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
            */
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
