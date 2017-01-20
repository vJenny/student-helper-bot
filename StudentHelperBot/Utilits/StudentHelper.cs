using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronic;
using StudentHelperBot.Properties;
using StudentHelperBot.Utilits.OpenWeatherMap;

namespace StudentHelperBot.Utilits
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
            return @"Приятно познакомиться, " + Name + "!";
        }

        public string SetGroup(string group)
        {
            int g;
            var f = int.TryParse(group, out g);
            Group = g;
            return f ? @"Запомнил, группа" + Group : @"Неверный формат группы";
        }

        public string SetCourse(string course)
        {

            int c;
            var f = int.TryParse(course, out c);
            Course = c;
            return f ? @"Запомнил, курс " + Course : @"Неверный формат курса";
        }

        public string Hello() => $"Привет, {Name}";

        public string Help() => Resources.helpMessage;

        public string GetDeansOfficeSchedule()
            => DeansOffice.WhatSchedule(DateTime.Now.DayOfWeek);

        public string GetDiningHallMenu()
            => DiningHall.WhatToEat(DateTime.Now.DayOfWeek);

        public string Reset()
        {
            Course = 0;
            Group = 0;
            Name = @"Аноним";
            return @"Настройки сброшены.";
        }

        public async Task<string> GetSchedule()
        {
            var mmcsc = new MMCSClient();
            if (Course == 0 || Group == 0)
                return @"Укажите курс и группу";
            var res = await mmcsc.StudentSchedule(Course, Group, (int)DateTime.Now.DayOfWeek - 1);
            var answ = new StringBuilder();
            res.ForEach(item => answ.Append(item));
            return answ.Length == 0 ? @"Сегодня выходной :)" : answ.ToString();
        }
        public async Task<string> GetLecturerSchedule(string name)
        {
            var mmcsc = new MMCSClient();
            var res = await mmcsc.TeacherSchedule(name, (int)DateTime.Now.DayOfWeek - 1);
            if (res == null)
                return @"Преподаватель не найден";
            if (res.Length == 0)
                return @"У этого преподавателя сегодня нет пар";
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
                ? @"На улице очень холодно, лучше оставайтесь дома :)" + Environment.NewLine
                : weather.Temp < 0
                ? @"Лёгкий морозец. Одевайтесь теплее." + Environment.NewLine
                : weather.Temp < 20
                ? @"Прохладная погода." + Environment.NewLine
                : @"На улице довольно жарко. Ура, лето!" + Environment.NewLine);
            sb.Append(weather.Humidity <= 70
                ? @" Осадков не ожидается." + Environment.NewLine
                : weather.Temp < 0
                    ? @" Возможен снег. Ура, можно играть в снежки!" + Environment.NewLine
                    : @" Возможен дождь. Захватите зонтик." + Environment.NewLine);
            sb.Append($" Температура воздуха {(int)weather.Temp}°C.");
            return sb.Length == 0 ? @"Что-то пошло не так :(" : sb.ToString(); 
        }
    }
}