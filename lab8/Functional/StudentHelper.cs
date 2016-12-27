using System;
using System.Text;
using System.Threading.Tasks;
using Chronic;
using lab8.Functional.OpenWeatherMap;
using lab8.Functional.ScheduleMMCS;
using lab8.Functional.News;
using lab8.Properties;

namespace lab8.Functional
{
    public class StudentHelper
    {
        public string Name { get; set; }
        public int Group { get; set; }
        public int Course { get; set; }
        public string City { get; set; } ///Город
        public int Date { get; set; } // Дата 


        public StudentHelper()
        {
            Name = "Аноним";
            Group = Course = 0;
            City = null; //по-умолчанию
            Date = -1;
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

        public string SetDate(string day)
        {
            switch (day)
            {
                case "сегодня":
                    Date = 0;
                    break;
                case "завтра":
                    Date = 1;
                    break;
                default:
                    break;
            }
            int res;
            if (Int32.TryParse(day, out res))
                if (res < 31)
                    Date = res - DateTime.Now.Day;
            return (Date < 0) ? "Что-то не так с Вашей датой! Какой сейчас год?" : Resources.okayMsg;

        }

        ///установили город
        public string SetCity(string city)
        {
            switch (city.ToLower())
            {
                case "москва":
                    City = "moscow";
                    break;
                case "ростов":
                    City = "rostov-on-don";
                    break;
                case "краснодар":
                    City = "krasnodar";
                    break;
                case "питер":
                    City = "Saint Petersburg";
                    break;
                case "сочи":
                    City = "sochi";
                    break;
                case "екатеринбург":
                    City = "yekaterinburg";
                    break;
                default:
                    break;
            }

            return (City == null) ? "Я не знаю такого города! Возможно он находится в другой галактике!" : Resources.okayMsg;
        }

        public string SetCourse(string course)
        {
            int c;
            var f = int.TryParse(course, out c);
            Course = c;
            return f ? Resources.okayMsg : "Неверный формат курса";
        }

        public async Task<string> BayMsg() => Resources.bayMsg;

        public async Task<string> TnkxMsg() => Resources.thnxMsg;

        public async Task<string> AllUnderstand() => Resources.allUnderstand;

        public async Task<string> WowCool() => Resources.wowСool;

        public async Task<string> JohnKonor() => Resources.johnKonor;

        public async Task<string> Danger() => Resources.danger;

        public async Task<string> HowSaving() => Resources.howSaving;

        public async Task<string> AboutSelf() => Resources.aboutSelf;

        public async Task<string> CareMsg() => Resources.careMsg;

        public async Task<string> Hello() => $"Привет, {Name}";

        public async Task<string> Help() => Resources.helpMsg;

        public async Task<string> Greeting() => Resources.greetingMsg;

        public async Task<string> HowAreYou() => Resources.allrightMsg;

        public async Task<string> GetDeansOfficeSchedule()
            => DeansOffice.WhatSchedule(DateTime.Now.DayOfWeek);

        public async Task<string> GetDiningHallMenu()
            => DiningHall.WhatToEat(DateTime.Now.DayOfWeek);

        public async Task<string> Reset()
        {
            Course = 0;
            Group = 0;
            Name = "Аноним";
            City = null;

            return "Настройки сброшены.";
        }

        /// <summary>
        /// Новости
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetNews()
        {
            var meduza = new NewsClient(Resources.newsKey); //новости с одной странице

            var news = await meduza.getNews(); ///именно новости

            var strBuild = new StringBuilder();

            strBuild.AppendLine("Последние новости:\r\n");

            foreach (var nnew in news)
            {
                strBuild.AppendLine("Заголовок:\r\n" + nnew.Title);
                strBuild.AppendLine("\r\nДата:\r\n" + nnew.Date);
                strBuild.AppendLine("\r\nОписание:\r\n" + nnew.Description);
                strBuild.AppendLine("\r\nURL:\r\n" + nnew.Url);
            }
                        
            return strBuild.ToString();
        }    
    
        public async Task<string> GetSchedule()
        {
            var mmcsc = new MMCSClient();
            if (Course == 0 || Group == 0)
                return "Укажите курс и группу";
            var res = await mmcsc.StudentSchedule(Course, Group, (int)DateTime.Now.DayOfWeek - 1);
            var answ = new StringBuilder();
            res.ForEach(item => answ.Append(item));
            return answ.ToString();
        }

        public async Task<string> GetLecturerSchedule(string name)
        {
            var mmcsc = new MMCSClient();
            var res = await mmcsc.TeacherSchedule(name, (int) DateTime.Now.DayOfWeek - 1);
            if (res == null)
                return $"Преподаватель не найден";
            if (res.Length == 0)
                return $"Нет пар";
            var answ = new StringBuilder();
            res.ForEach(item => answ.Append(item));
            return answ.ToString();
        }

        public string toNormalName(string city)
        {
            switch (city)
            {
                case "moscow":
                    return "Москва";
                case "rostov-on-don":
                    return "Ростов-на-Дону";
                case "Saint Petersburg":
                    return "Санкт-Петербург";
                case "krasnodar":
                    return "Краснодар";
                case "sochi":
                    return "Сочи";
                case "yekaterinburg":
                    return "Екатеринбург";
                case "Kazan":
                    return "Казань";
                case "Taganrog":
                    return "Таганрог";
                case "Novocherkassk":
                    return "Новочеркасск";
                default:
                    return "None";
            }
        }

        public async Task<string> GetWeather()
        {
            if (City == null)
                return "Для начала, введите название города, Сэр!";

            var owm = new WeatherClient(Resources.wmKey);
            var res = await owm.Forecast(City);

            if (Date < 0)
                return "Вам погоду на какой день? Можете спросить 'нужно сегодня', 'нужно завтра', 'нужно на 17'";

            var weather = res[Date];
            var sb = new StringBuilder();
            string curdate = "";
            if (Date == 0)
                curdate = "сегодня ";
            else if (Date == 1)
                curdate = "завтра ";
            else if (Date > 1)
            {
                DateTime cd = DateTime.Now.AddDays(Date);
                curdate = cd.Date.Day.ToString() + "." + cd.Date.Month.ToString() + " ";
            }
            sb.Append("(Погода в городе " + toNormalName(City) + " на " + curdate + " ) ");

            sb.Append("Погода в городе " + toNormalName(City) + " ");

            sb.Append(weather.Temp < -10
                ? " днем: " + weather.Temp + ", ночью: " + weather.NightTemp + " " +  Resources.coldMsg
                : (DateTime.Now.DayOfWeek == DayOfWeek.Sunday ?
                weather.Temp + Resources.warmMsg
                : " днем " + weather.Temp + ", ночью: " + weather.NightTemp + " " + Resources.studyMsg));
            sb.Append(weather.Humidity <= 50
                ? Resources.sunnyMsg
                : (weather.Temp > 0 ? Resources.rainMsg : Resources.snowMsg));
            return sb.Length == 0 ? Resources.errorMsg : sb.ToString();
        }
    }
}