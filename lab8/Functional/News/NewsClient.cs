using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace lab8.Functional.News
{
    public class NewsClient
    {
        public string AppID { get; set; } ///id

        private readonly HttpClient _cli = new HttpClient(); 

        public NewsClient(string appId)
        {
            AppID = appId;
        }

        /// <summary>
        /// Вытаскиваем новости
        /// </summary>
        public async Task<NewsRecord[]> getNews()
        {
            var result = await _cli.GetStringAsync($"http://newsapi.org/v1/articles?source=the-next-web&sortBy=latest&apiKey={AppID}");

            var news = new List<NewsRecord>();

            dynamic entity = Newtonsoft.Json.JsonConvert.DeserializeObject(result);

            foreach (var e in entity.articles)
            {
                news.Add(new NewsRecord
                {
                    Title = e.title,
                    Date = e.publishedAt,
                    Description = e.description,
                    Url = e.url,
                });
            }
            return news.ToArray();
        }
    }
}