using System;
using System.Text;
using System.Threading.Tasks;
using lab8.Functional.OpenWeatherMap;
using lab8.Properties;

namespace lab8.Functional
{
    public class StudentHelper
    {
        public string Name { get; set;  }
        public string Group { get; set;  }
        public string Course { get; set; }

        public StudentHelper()
        {
            Name = "Аноним";
            Group = Course = "0";
        }

        public string SetName(string name)
        {
            Name = char.ToUpper(name[0]) + name.Remove(0, 1);
            return Resources.ntmyMsg + Name + "!";
        }

        public string SetGroup(string group)
        {
            Group = group;
            return Resources.okayMsg;
        }

        public string SetCourse(string course)
        {
            Course = course;
            return Resources.okayMsg;
        }

        public async Task<string> Hello() => $"Привет, {Name}";

        public async  Task<string> Help() => Resources.helpMsg;

        public async Task<string> Greeting() => Resources.greetingMsg;

        public async Task<string> HowAreYou() => Resources.allrightMsg;

        public async Task<string> GetDeansOfficeSchedule()
            => DeansOffice.WhatSchedule(DateTime.Now.DayOfWeek);

        public async Task<string> GetDiningHallMenu() 
            => DiningHall.WhatToEat(DateTime.Now.DayOfWeek);


        public async Task<string> GetSchedule()
        {
            throw new NotImplementedException();
        }

        public async Task<String> GetLecturerSchedule()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetWeather()
        {
            var owm = new WeatherClient(Resources.wmKey);
            var res = await owm.Forecast(Resources.city);

            var weather = res[0];
            var sb = new StringBuilder();

            sb.Append(weather.Temp < -10
                ? weather.Temp + Resources.coldMsg
                : (DateTime.Now.DayOfWeek == DayOfWeek.Sunday ?
                weather.Temp + Resources.warmMsg
                : weather.Temp + Resources.studyMsg));
            sb.Append(weather.Humidity <= 50 
                ? Resources.sunnyMsg
                : (weather.Temp > 0 ? Resources.rainMsg  : Resources.snowMsg));
            return sb.Length == 0 ? Resources.errorMsg : sb.ToString();
        }
    }
}