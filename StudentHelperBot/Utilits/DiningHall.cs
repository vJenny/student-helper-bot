using System;
using System.Collections.Generic;

namespace StudentHelperBot.Utilits
{
    public static class DiningHall
    {
        private static readonly Dictionary<DayOfWeek, string> Menu = new Dictionary<DayOfWeek, string>
        {
            { DayOfWeek.Monday, "винегрет, борщ со сметаной, " +
                                "котлеты мясные по-домашнему, пюре картофельное, " +
                                "компот из сухофруктов, хлеб домашний."},
            { DayOfWeek.Tuesday, "салат из огурцов с яйцом, " +
                                 "суп гороховый, котлеты куриные," +
                                 "рис отварной, компот из сухофруктов" +
                                 "хлеб домашний." },
            { DayOfWeek.Wednesday, "салат из квашеной капусты," +
                                   "суп с лапшой и грибами," +
                                   "тефтели с соусом," +
                                   "компот из сухофруктов, хлеб домашний." },
            { DayOfWeek.Thursday, "свекла по-корейски, рассольник со сметаной," +
                                  "куры жареные, макароны отварные," +
                                  "компот из сухофруктов, хлеб домашний." },
            { DayOfWeek.Friday, "салат 'Пестрый', свекольник со сметаной," +
                                "зразы мясные с яйцом, картофель отварной," +
                                "компот из сухофруктов, хлаб домашний." },
            { DayOfWeek.Saturday, "салат оливъе, суп с лапшой и помидорами," +
                                  "зразы картофельные с мясом, капуста тушеная," +
                                  "компот из сухофруктов, хлеб домашний." },
            { DayOfWeek.Sunday, "Столовая не работает" }
        };

        public static string WhatToEat(DayOfWeek day) =>
            day == DayOfWeek.Sunday ? Menu[day] : "Сегодня в меню: " + Menu[day];
    }
}