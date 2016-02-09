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

        static public string DataRunFile
        { // Serial, File, Random
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("DataRunFile"))
                {
                    return (string)ApplicationData.Current.LocalSettings.Values["DataRunFile"];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["DataRunFile"] = value;
            }
        }

        static public string DataSerialSettings
        { // Serial, File, Random
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("DataSerialSettings"))
                {
                    return (string)ApplicationData.Current.LocalSettings.Values["DataSerialSettings"];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["DataSerialSettings"] = value;
            }
        }

        static public string DataRandomSettings
        { // Serial, File, Random
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("DataRandomSettings"))
                {
                    return (string)ApplicationData.Current.LocalSettings.Values["DataRandomSettings"];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["DataRandomSettings"] = value;
            }
        }
    }
}
