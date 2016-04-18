using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using System.Threading;
using System.Threading.Tasks;

namespace DemoPrototype
{
    public class AOUSerialData: AOUData
    {
        private SerialDevice serialPort = null;

        private CancellationTokenSource ReadCancellationTokenSource;

        private DataWriter dataWriteObject = null;
        private DataReader dataReaderObject = null;

        private string textToSend = "";
        private string receivedText = "";

        private List<string> deviceList;

        private AOUSettings.SerialSetting setting;

        public AOUSerialData(AOUSettings.SerialSetting serialSetting, AOUSettings.DebugMode dbgMode = AOUSettings.DebugMode.noDebug) : base(dbgMode)
        {
            deviceList = new List<string>();

            setting = serialSetting;
        }

        ~AOUSerialData()
        {
            if (serialPort != null)
            {
                serialPort.Dispose();
                serialPort = null;
            }
        }

        public override bool SendData(string data)
        {
            textToSend += data;
            Send();
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


        public override void Connect()
        {
            base.Connect();
            if (setting.ComPort.Substring(0,3) == "RPI")
            { 
                InitRPiPort(setting.ComPort, setting.BaudRate);
            }
            else
            { 
                InitSerialPort(setting.ComPort, setting.BaudRate);
            }
        }

        public override void Disconnect()
        {
            base.Disconnect();
            CancelReadTask();
        }

        private void ConfigureSerialPort(uint baudrate)
        {
            serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
            serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
            serialPort.BaudRate = baudrate;
            serialPort.Parity = SerialParity.None;
            serialPort.StopBits = SerialStopBitCount.One;
            serialPort.DataBits = 8;
            serialPort.Handshake = SerialHandshake.None;

            AddDataLogText("ProdId:" + serialPort.UsbProductId + ", " + "vendorId:" + serialPort.UsbVendorId);
            AddDataLogText("Baudrate:" + serialPort.BaudRate + ", Parity:" + serialPort.Parity + ", Databits:" + serialPort.DataBits + ", Stopbits:" + serialPort.StopBits + ", No Handshake");
        }


        public async void GetAllDevices()
        {
            deviceList.Clear();

            try
            {
                var devices = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector());
                if (devices.Count == 0)
                {
                    AddDataLogText("No SerialDevices found");
                }
                else
                {
                    AddDataLogText("Found " + devices.Count + " Serial devices");
                    for (int i = 0; i < devices.Count; i++)
                    {
                        var port = await SerialDevice.FromIdAsync(devices[i].Id);
                        if (port != null)
                        { 
                            deviceList.Add(port.PortName);
                            AddDataLogText((i+1) + ": " + devices[i].Name + " - " + port.PortName + ", Kind:" + devices[i].Kind.ToString() + ", Enabled:" + devices[i].IsEnabled);
                            port.Dispose();
                        }
                        else
                        {
                            AddDataLogText((i + 1) + ": " + devices[i].Name + " - No valid serial port, Kind:" + devices[i].Kind.ToString() + ", Enabled:" + devices[i].IsEnabled);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddDataLogText("SerialDevice exception: " + ex.Message);
            }

        }

        private async void InitSerialPort(string portName, uint baudRate)
        {
            Connected = false;
            try
            {
                var devices = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector(portName), null);
                if (devices.Count > 0)
                {

                    serialPort = await SerialDevice.FromIdAsync(devices[0].Id);
                    if (serialPort != null)
                    {
                        ConfigureSerialPort(baudRate);
                        ReadCancellationTokenSource = new CancellationTokenSource();
                        Listen();
                        Connected = true;
                        AddDataLogText("Listening to " + devices[0].Name + " - " + serialPort.PortName);
                    }
                    else
                    {
                        AddDataLogText("Can not connect to Serial Device " + portName);
                    }

                    if (devices.Count > 1) AddDataLogText("More devices Found: " + devices.Count);
                }
                else 
                {
                    AddDataLogText("Device not found: " + portName);
                }
            }
            catch (Exception ex)
            {
                AddDataLogText("Connection exception: " + ex.Message);
            }
            if (!Connected)
            {
                GetAllDevices();
            }
        }

        private async void InitRPiPort(string deviceName, uint baudRate)
        {
            // portName: RPI1, RPI2 ...
            Connected = false;
            try
            {
                var devices = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector()); // Get all devices

                if (devices.Count > 0)
                {
                    string numStr = deviceName.Substring(deviceName.Length - 1);
                    int num = 1;
                    int.TryParse(numStr, out num);
                    if (devices.Count < num)
                    {
                        num = 1;
                    }
                    num--;
                    AddDataLogText(deviceName + " -> " + num);
                    serialPort = await SerialDevice.FromIdAsync(devices[num].Id);
                    if (serialPort != null)
                    {
                        AddDataLogText("Found serial device: " + devices[num].Name + " - " + devices[num].Id);
                        ConfigureSerialPort(baudRate);
                        ReadCancellationTokenSource = new CancellationTokenSource();
                        Listen();
                        Connected = true;
                        AddDataLogText("Listening to " + devices[num].Name);
                    }
                    else
                    {
                        AddDataLogText("Can not connect to Serial Device " + devices[0].Name);
                    }
                }
                else
                {
                    AddDataLogText("No RPI serial devices found");
                }
            }
            catch (Exception ex)
            {
                AddDataLogText("Connection exception: " + ex.Message);
            }
            if (!Connected)
            { 
                GetAllDevices();
            }
        }

        /**************************************************/
        // Sending Data
        /**************************************************/

        private async Task WriteAsync()
        {
            Task<UInt32> storeAsyncTask;
            if (textToSend.Length > 0)
            {
                dataWriteObject.WriteString(textToSend);
                storeAsyncTask = dataWriteObject.StoreAsync().AsTask();
                UInt32 bytesWritten = await storeAsyncTask;
                textToSend = ""; 
            }
        }

        private async void Send()
        {
            try
            {
                if (Connected)
                {
                    dataWriteObject = new DataWriter(serialPort.OutputStream);
                    await WriteAsync();
                }
                else
                {
                    AddDataLogText("Sending data, Not connected to serial port");
                }
            }
            catch (Exception ex)
            {
                AddDataLogText("Error sending data: " + ex.Message);
            }
            finally // Cleanup
            {
                if (dataWriteObject != null)
                {
                    dataWriteObject.DetachStream();
                    dataWriteObject = null;
                }
            }
        }

        /**************************************************/
        // Reading Data
        /**************************************************/
        private void CancelReadTask()
        {
            if (ReadCancellationTokenSource != null)
            {
                if (!ReadCancellationTokenSource.IsCancellationRequested)
                {
                    ReadCancellationTokenSource.Cancel();
                    AddDataLogText("Stop reading from serialDevice");

                    // Must handle this anyway
                }
            }
        }

        private async Task ReadAsync(CancellationToken cancellationToken)
        {
            Task<UInt32> loadAsyncTask;

            uint ReadBufferLength = 1024;

            // Throw Cancel exception if Canceled
            cancellationToken.ThrowIfCancellationRequested();

            dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;
            loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask(cancellationToken);

            // Launch the task and wait
            // AddDataLogText("await loadAsyncTask");
            UInt32 bytesRead = await loadAsyncTask;
            if (debugMode != AOUSettings.DebugMode.noDebug)
            { 
                AddDataLogText("bytesRead:" + bytesRead);
            }
            if (bytesRead > 0)
            {
                receivedText += dataReaderObject.ReadString(bytesRead);
            }
        }

        private async void Listen()
        {
            try
            {
                if (serialPort != null)
                {
                    dataReaderObject = new DataReader(serialPort.InputStream);

                    // keep reading the serial input
                    while (true)
                    {
                        await ReadAsync(ReadCancellationTokenSource.Token);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType().Name == "TaskCanceledException")
                {
                    AddDataLogText("Reading task was cancelled, closing device and cleaning up");
                    if (serialPort != null)
                    {
                        serialPort.Dispose();
                    }
                    serialPort = null;
                    AddDataLogText("Disconnected from serialDevice");
                }
                else
                {
                    AddDataLogText("Cancel error: " + ex.Message);
                }
            }
            finally
            {
                // Cleanup once complete
                if (dataReaderObject != null)
                {
                    dataReaderObject.DetachStream();
                    dataReaderObject = null;
                }
            }
        }

    }
}
