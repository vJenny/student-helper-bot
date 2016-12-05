using System;
using System.Text;
using System.Threading.Tasks;
using lab8.Functional.OpenWeatherMap;

namespace lab8.Functional
{
    public class StudentHelper
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public string Course { get; set; }

        public StudentHelper()
        {
            Name = "Аноним";
            Group = Course = "0";
        }

        public async Task<string> GetSchedule()
        {
            throw new NotImplementedException();
        }

        public async Task<String> GetDeanerySchedule()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetWeather()
        {
            var owm = new WeatherClient("522ea52cffe0c78955e0e319e1572a7b");
            var res = await owm.Forecast("rostov-on-don");
            var weather = res[0];
            var sb = new StringBuilder();
            sb.Append(weather.Temp < -10
                ? $"Ужасно холодно, на улице {weather.Temp}! Лучше сидеть дома. "
                : (DateTime.Now.DayOfWeek == DayOfWeek.Sunday ?
                $"Температура {weather.Temp}. Отличная погода, чтобы погулять. "
                : $"Температура {weather.Temp}. Отличная погода, чтобы идти на пары. "));
            if (weather.Humidity > 50)
                sb.Append(weather.Temp > 0
                    ? $"Возможен дождь! Захватите зонтик :)"
                    : $"Возможен снег! По пути можно играть в снежки! :)");
            return sb.Length == 0 ? "Глупый бот Вас не понимать. Пните разработчика :(" : sb.ToString();
        }
    }
}