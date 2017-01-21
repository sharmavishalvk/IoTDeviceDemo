using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IoTDeviceDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer timer = new DispatcherTimer();
        
        DeviceClient deviceClient;
        string iothubconnectionstring = "SCIoTDemo.azure-devices.net";
        string deviceid = "D2CDemo";
        string devicekey = "RFMqudHRArGRl1hX33xIy0hpoeS/+OBg/bCcF8Hw5QQ=";

        event EventHandler<string> MessageReceivedEvent;
        
        public MainPage()
        {
            this.InitializeComponent();

            // establish connection to IoT Hub
            deviceClient = DeviceClient.Create(iothubconnectionstring,
                new DeviceAuthenticationWithRegistrySymmetricKey(deviceid, devicekey));

            // listen to incoming message
            receivedatafromAzure();
            MessageReceivedEvent += MainPage_MessageReceivedEvent;

            // define the timer
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        
        private async void Timer_Tick(object sender, object e)
        {
            Random rnd = new Random();

            // simulate the weather data
            var temperature = rnd.Next(24, 40) + rnd.NextDouble();
            var pressure = rnd.Next(980, 1040) + rnd.NextDouble();

            temperatureTB.Text = temperature.ToString("0.00");
            pressureTB.Text = pressure.ToString("0.00");

            // create object
            WeatherData weatherdata = new WeatherData()
            {
                deviceid = deviceid,
                temperature = temperature,
                pressure = pressure,
                createdAt = DateTime.UtcNow
            };

            // send data to iot hub
            try
            {
                var msg = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(weatherdata)));
                await deviceClient.SendEventAsync(msg);
            }
            catch
            { }


        }

        private void MainPage_MessageReceivedEvent(object sender, string e)
        {
            messageTB.Text = e.ToString();
        }

        // listen to Azure IoT Hub message
        private async Task receivedatafromAzure()
        {
            try
            {
                Message receivedMessage;
                string messageData;

                while (true)
                {
                    receivedMessage = await deviceClient.ReceiveAsync();
                    if (receivedMessage != null)
                    {
                        messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                        this.OnMessageReceivedEvent(messageData);
                        await deviceClient.CompleteAsync(receivedMessage);
                    }
                }
            }
            catch
            { }
        }

        // to invoke the messagereceivedevent
        private void OnMessageReceivedEvent(string s)
        {
            MessageReceivedEvent?.Invoke(this, s);
        }
    }
}
