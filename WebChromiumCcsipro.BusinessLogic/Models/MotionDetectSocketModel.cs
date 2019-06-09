using System;
using System.Web.Script.Serialization;

namespace WebChromiumCcsipro.BusinessLogic.Models
{
    public class MotionDetectSocketModel
    {
        public int ObjectId { get; set; }
        public string VideoDeviceSource { get; set; }
        public string ZoneName { get; set; }
        [ScriptIgnore]
        public DateTime TimeMotion { get; set; }

        private string _time;
        public string Time
        {
            get { return _time; }
            set
            {
                _time = value;
                TimeMotion = DateTime.ParseExact(value, "dd.MM.yyyy HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture);
            }
        }
    }
}