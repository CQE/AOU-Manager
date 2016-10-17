using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace DemoPrototype
{
    class AOURemoteData : AOUData
    {
        private AOUSettings.RemoteSetting setting;
        private string textToSend = "";
        private string receivedText = "";
        private string errLog = "";

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


        public async void SendHTTPRequest(string httpUri)
        {

            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();

            var headers = httpClient.DefaultRequestHeaders; // Add a user-agent header to the GET request. 

            string header = "ie";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            header = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            Windows.Web.Http.HttpRequestMessage httpRequest = new Windows.Web.Http.HttpRequestMessage(Windows.Web.Http.HttpMethod.Post, new Uri(httpUri));
            // httpRequest.

            //Send the GET request asynchronously and retrieve the response as a string.
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();

            try
            {
                //Send the GET request
                httpResponse = await httpClient.GetAsync(new Uri(httpUri));
                httpResponse.EnsureSuccessStatusCode();
                receivedText += await httpResponse.Content.ReadAsStringAsync() + "\r\n";
            }
            catch (Exception ex)
            {
                receivedText += "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message + "\r\n";
            }

        }

        // Web Socket
        // wss: makes an encrypted connection.
        const string serverURI = "blabla:9000/echo"; // Only test

        private MessageWebSocket msgWebSocket;

        private async Task WebSendMessage(string message)
        {
            DataWriter messageWriter = new DataWriter(msgWebSocket.OutputStream);
            messageWriter.WriteString(message);
            await messageWriter.StoreAsync();
        }

        public async void InitWebSocket()
        {

            Uri uri = new Uri(serverURI);
            msgWebSocket = new MessageWebSocket();
            msgWebSocket.Control.MessageType = SocketMessageType.Utf8;

            // Callbacks for MessageReceived and Closed messages
            msgWebSocket.MessageReceived += WebMessageReceived;
            msgWebSocket.Closed += WebSockClosed;

            try
            {
                await msgWebSocket.ConnectAsync(uri);
                await WebSendMessage("hello");

            }
            catch (Exception ex)
            {
                errLog += ex.Message + "\r\n";
            }
        }

        private void WebMessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            DataReader messageReader = args.GetDataReader();
            messageReader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
            receivedText += messageReader.ReadString(messageReader.UnconsumedBufferLength) + "\r\n";
        }

        private void WebSockClosed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
            // ToDo
        }
    }
 }
