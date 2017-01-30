using System;
using System.Diagnostics.Eventing.Reader;
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
        public enum Degrees { Undefined, Bachelor, Master }

        public string Name { get; set; }
        public int Group { get; set; }
        public int Course { get; set; }

        public Degrees Degree { get; set; }

        public StudentHelper()
        {
            Name = "Аноним";
            Group = Course = 0;
            Degree = Degrees.Undefined;
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
            if (f && g > 0 && g <= 9)
                return @"Запомнил, группа " + Group;
            return @"Неверный формат группы";
        }

        public string SetCourse(string course)
        {
            int c;
            var f = int.TryParse(course, out c);
            Course = c;
            if (f && c > 0 && c < 5)
                return @"Запомнил, курс " + Course;
            return  @"Неверный формат курса";
        }

        public string SetDegree(Degrees degree)
        {
            Degree = degree;
            return degree == Degrees.Bachelor ? 
                @"Запомнил, Вы - бакалавр" : @"Запомнил, Вы - магистр";
        }

        public string Hello(string nickname) => 
            Name != "Аноним" 
            ? $"Привет, {Name}" 
            : $"Привет, {(nickname != null && nickname.Trim() != "" ? nickname : "Аноним")}";

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
            var mmcsc = new MmcsClient();
            if (Course == 0 || Group == 0)
                return @"Укажите курс и группу";
            if (Degree == Degrees.Undefined)
                return @"Укажите, /bachelor вы или  /master";
            var c = Degree == Degrees.Bachelor ? Course : Course + 5;
            var res = await mmcsc.StudentSchedule(c, Group, (int)DateTime.Now.DayOfWeek - 1);
            var answ = new StringBuilder();
            res.ForEach(item => answ.Append(item));
            answ.Append("");
            return answ.Length == 0 ? @"Сегодня выходной :)" : answ.ToString();
        }
        public async Task<string> GetLecturerSchedule(string name)
        {
            if (name.Length == 0)
                return @"Введите имя преподавателя.";
            var mmcsc = new MmcsClient();
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