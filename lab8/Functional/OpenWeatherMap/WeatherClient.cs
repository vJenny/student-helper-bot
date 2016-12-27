using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace lab8.Functional.OpenWeatherMap
{
    public class WeatherClient
    {
            public string AppId { get; set; }

            private readonly HttpClient _cli = new HttpClient();

            public WeatherClient(string appId)
            {
                AppId = appId;
            }

            public async Task<WeatherRecord[]> Forecast(string city)
            {
                var res = await _cli.GetStringAsync($"http://api.openweathermap.org/data/2.5/forecast/daily?q={city}&mode=json&units=metric&cnt=7&APPID={AppId}");
                var f = new List<WeatherRecord>();
                dynamic x = Newtonsoft.Json.JsonConvert.DeserializeObject(res);
                foreach (var z in x.list)
                {
                    f.Add(new WeatherRecord
                    {
                        When = Convert((long)z.dt),
                        Temp = z.temp.day,
                        NightTemp = z.temp.night,
                        Pressure = z.pressure,
                        Humidity = z.humidity,
                    });
                }
                return f.ToArray();
            }
            private static DateTime Convert(long x)
            {
                var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(x).ToLocalTime();
                return dtDateTime;
            }

        }
    }
