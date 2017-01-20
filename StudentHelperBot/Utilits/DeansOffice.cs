using System;

namespace StudentHelperBot.Utilits
{
    public static class DeansOffice
    {
        public static string WhatSchedule(DayOfWeek day) =>
            day == DayOfWeek.Saturday || day == DayOfWeek.Sunday ?
            "Деканат сегодня не работает" : "Деканат работает с 9:00 до 16:00";
    }
}
