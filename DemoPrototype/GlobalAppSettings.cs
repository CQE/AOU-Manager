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
    //temporary created global variables Urban please change this asap
    public class GlobalVar
    {
        static int _thresholdHot2Cold;
        public static int ThresholdHot2Cold
        {
            get { return _thresholdHot2Cold; }
            set { _thresholdHot2Cold = value; }
        }
       
    }
    
}
