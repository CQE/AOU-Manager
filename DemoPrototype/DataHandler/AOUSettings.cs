using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPrototype
{
    public class AOUSettings
    {

        public enum DebugMode
        {
            noDebug, rawData // Possible to define more debug modes like log to file....
        }

        public struct RemoteSetting
        {
            public string Remote;
            public string Port;
            public string User;
            public string Password;

            public RemoteSetting(string remote, string port, string user, string password)
            {
                Remote = remote;
                Port = port;
                User = user;
                Password = password;
            }
        }

        public struct FileSetting
        {
            public string SourceType;
            public string FilePath;

            public FileSetting(string sourceType, string filePath)
            {
                SourceType = sourceType;
                FilePath = filePath;
            }
        }

        public struct SerialSetting
        {
            public string ComPort;
            public uint BaudRate;

            public SerialSetting(string comPort, uint baudRate)
            {
                ComPort = comPort;
                BaudRate = baudRate;
            }
        }

        public struct RandomSetting
        {
            public uint NumValues;
            public uint MsBetween;

            public RandomSetting(uint numValues, uint msBetween)
            {
                NumValues = numValues;
                MsBetween = msBetween;
            }
        }

    }
}
