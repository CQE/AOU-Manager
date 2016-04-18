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
    public class SerialData
    {
        public bool Connected { get; private set; }
        public static string ResultMessage { get; private set; }

        private SerialDevice serialPort = null;
        private CancellationTokenSource ReadCancellationTokenSource;
        private DataWriter dataWriteObject = null;
        private DataReader dataReaderObject = null;

        private string textToSend;
        private string receivedText;

        public SerialData()
        {
            Connected = false;
            InitComPort();
        }

        ~SerialData()
        {
            CancelReadTask();
            if (serialPort != null)
            {
                serialPort.Dispose();
                serialPort = null;
            }
        }

        public bool SendData(string text_data)
        {
            textToSend += text_data;
            Send();
            return true;
        }

        private void CancelReadTask()
        {
            if (ReadCancellationTokenSource != null)
            {
                if (!ReadCancellationTokenSource.IsCancellationRequested)
                {
                    ReadCancellationTokenSource.Cancel();
                }
            }
        }

        private void ConfigureSerialPort()
        {
            // Configure serial settings
            if (serialPort != null) { 
                serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                serialPort.BaudRate = 9600;
                serialPort.Parity = SerialParity.None;
                serialPort.StopBits = SerialStopBitCount.One;
                serialPort.DataBits = 8;
                serialPort.Handshake = SerialHandshake.None;
            }
        }

        private async void InitComPort()
        {
            try
            {
                string aqs = SerialDevice.GetDeviceSelector();
                var dis = await DeviceInformation.FindAllAsync(aqs);

                DeviceInformation entry1;
                DeviceInformation entry2;

                for (int i = 0; i < dis.Count; i++)
                {
                    var dev = dis[i];
                }

                if (dis.Count > 0)
                {
                    entry1 = (DeviceInformation)dis[0];

                    if (dis.Count > 0)
                        entry2 = (DeviceInformation)dis[1];

                    serialPort = await SerialDevice.FromIdAsync(entry1.Id);
                    if (serialPort != null)
                    { 
                        ConfigureSerialPort();
                        ReadCancellationTokenSource = new CancellationTokenSource();
                        Listen();
                        Connected = true;
                    }
                    else
                    {
                        ResultMessage = "";
                        Connected = false;
                    }

                }
            }
            catch (Exception ex)
            {
                ResultMessage = ex.Message;
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

                // Launch an async task to complete the write operation
                storeAsyncTask = dataWriteObject.StoreAsync().AsTask();

                UInt32 bytesWritten = await storeAsyncTask;
                if (bytesWritten > 0)
                {
                    ResultMessage += ", " + bytesWritten.ToString();
                    ResultMessage += " bytes written successfully!";
                }
                textToSend = "";
            }
            else
            {
                ResultMessage += ", No bytes to write!";
            }
        }

        private async void Send()
        {
            try
            {
                if (Connected)
                {
                    // Init Datawriter object to stream output to serial port
                    // And start async write task
                    dataWriteObject = new DataWriter(serialPort.OutputStream);
                    await WriteAsync();
                }
                else
                {
                    ResultMessage += ", Not connected to serial port";
                }
            }
            catch (Exception ex)
            {
                ResultMessage += ", Error sending data: " + ex.Message;
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

        private async Task ReadAsync(CancellationToken cancellationToken)
        {
            Task<UInt32> loadAsyncTask;

            uint ReadBufferLength = 1024;

            // Throw Cancel exception if Canceled
            cancellationToken.ThrowIfCancellationRequested();

            dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;
            loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask(cancellationToken);

            // Launch the task and wait
            UInt32 bytesRead = await loadAsyncTask;
            if (bytesRead > 0)
            {
                receivedText += dataReaderObject.ReadString(bytesRead);
                ResultMessage += ", " + bytesRead + " bytes read";
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
                    ResultMessage = " Reading task was cancelled, closing device and cleaning up";
                    if (serialPort != null)
                    {
                        serialPort.Dispose();
                    }
                    serialPort = null;
                }
                else
                {
                    ResultMessage = " Cancel error: " + ex.Message;
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
