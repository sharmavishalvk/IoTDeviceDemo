using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTDeviceDemo
{
    public class WeatherData
    {
        public string deviceid { get; set; }
        public double temperature { get; set; }
        public double pressure { get; set; }
        public DateTime createdAt { get; set; }
    }

}
