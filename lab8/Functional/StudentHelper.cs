using System;
using System.Text;
using System.Threading.Tasks;
using Chronic;
using lab8.Functional.OpenWeatherMap;
using lab8.Functional.ScheduleMMCS;
using lab8.Properties;

namespace lab8.Functional
{
    public class StudentHelper
    {
        public string Name { get; set; }
        public int Group { get; set; }
        public int Course { get; set; }

        public StudentHelper()
        {
            Name = "Аноним";
            Group = Course = 0;
        }

        public string SetName(string name)
        {
            Name = char.ToUpper(name[0]) + name.Remove(0, 1);
            return Resources.ntmyMsg + Name + "!";
        }

        public string SetGroup(string group)
        {
            int g;
            var f = int.TryParse(group, out g);
            Group = g;
            return f ? Resources.okayMsg : "Неверный формат группы";
        }

        public string SetCourse(string course)
        {

            int c;
            var f = int.TryParse(course, out c);
            Course = c;
            return f ? Resources.okayMsg : "Неверный формат курса";
        }

        public async Task<string> Hello() => $"Привет, {Name}";

        public async Task<string> Help() => Resources.helpMsg;

        public async Task<string> Greeting() => Resources.greetingMsg;

        public async Task<string> HowAreYou() => Resources.allrightMsg;

        public async Task<string> GetDeansOfficeSchedule()
            => DeansOffice.WhatSchedule(DateTime.Now.DayOfWeek);

        public async Task<string> GetDiningHallMenu()
            => DiningHall.WhatToEat(DateTime.Now.DayOfWeek);


        public async Task<string> GetSchedule()
        {
            var mmcsc = new MMCSClient();
            if (Course == 0 || Group == 0)
                return "Укажите курс и группу";
            var res = await mmcsc.StudentSchedule(Course, Group, (int)DateTime.Now.DayOfWeek);
            var answ = new StringBuilder();
            res.ForEach(item => answ.Append(item));
            return answ.ToString();
        }

        public async Task<string> GetLecturerSchedule(string name)
        {
            var mmcsc = new MMCSClient();
            var res = await mmcsc.TeacherSchedule(name, (int) DateTime.Now.DayOfWeek);
            if (res.Length == 0)
                return $"Преподаватель {name} не найден";
            var answ = new StringBuilder();
            res.ForEach(item => answ.Append(item));
            return answ.ToString();
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
                : (weather.Temp > 0 ? Resources.rainMsg : Resources.snowMsg));
            return sb.Length == 0 ? Resources.errorMsg : sb.ToString();
        }
    }
}