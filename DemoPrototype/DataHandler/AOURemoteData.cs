using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPrototype
{
    class AOURemoteData : AOUData
    {
        private AOUSettings.RemoteSetting setting;
        private string textToSend = "";
        private string receivedText = "";


        public AOURemoteData(AOUSettings.RemoteSetting remoteSetting, AOUSettings.DebugMode dbgMode = AOUSettings.DebugMode.noDebug) : base(dbgMode)
        {
            setting = remoteSetting;
        }

        public override bool SendData(string data)
        {
            textToSend += data;
            // Send();
            return true;
        }

        public override void UpdateData()
        {
            base.GetTextDataList();
        }

        protected override string GetTextData()
        {
            string text = receivedText;
            receivedText = "";
            return text;
        }


    }
}
