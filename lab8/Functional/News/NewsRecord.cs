using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lab8.Functional.News
{
    public class NewsRecord
    {
        public string Title { get; set; } ///заголовок статьи
        public string Date { get; set; }  ///Дата размещения
        public string Description { get; set; } ///Описание
        public string Url { get; set; }  ///адрес статьич - ок
    }
}