using System;
using System.Collections.Generic;

namespace lab8.Functional
{
    public static class Schedule
    {
        private static readonly Dictionary<DayOfWeek, string> Schedule9 = new Dictionary<DayOfWeek, string>
        {
            { DayOfWeek.Monday,
                "8:00 - Комп.графика практика (202), " +
                "9:50 - Комп.графика практика (202) / Комп.графика лекция (312), " +
                "11:55 - Комп.графика лекция (312), " +
                "13:45 - Инт.системы семинар (313) / БЖД (311), " + 
                "15:50 - БЖД (311)"},
            { DayOfWeek.Tuesday,
                "Можно расслабиться, занятий нет :)"},
            { DayOfWeek.Wednesday,
                "9:50 - Инт.системы практика (202), " +
                "11:55 - Инт.системы лекция (322), " +
                "13:45 - Разраб.компиляторов лекция (312), " +
                "15:50 - Разраб.компиляторов практика (202) / - "},
            { DayOfWeek.Thursday,
                "Можно расслабиться, занятий нет :)"},
            { DayOfWeek.Friday,
                "11:55 - Функ.программирование практика (202), " +
                "13:45 - Прогр.инженерия практика (ЛОС 1,2) / Функ.программирование лекция (312)"},
            { DayOfWeek.Saturday,
                "9:50 - Функ.программирование лекция (312), " +
                "11:55 - Прогр.инженерия лекция (312)"},
            { DayOfWeek.Sunday, "Ну какие пары? Воскресенье же!" }
        };

        private static readonly Dictionary<DayOfWeek, string> Schedule8 = new Dictionary<DayOfWeek, string>
        {
            { DayOfWeek.Monday,
                "9:50 - - / Комп.графика лекция (312), " +
                "11:55 - Комп.графика лекция (312), " +
                "13:45 - Комп.графика практика (202), " +
                "15:50 - Инт.системы практика (202)," +
                "17:40 - Инт.системы семинар (310)"},
            { DayOfWeek.Tuesday,
                "Можно расслабиться, занятий нет :)"},
            { DayOfWeek.Wednesday,
                "8:00 - - / БЖД (311), " +
                "9:50 - БЖД (311), " +
                "11:55 - Инт.системы лекция (322), " +
                "13:45 - Разраб.компиляторов лекция (312)" },
            { DayOfWeek.Thursday,
                "Можно расслабиться, занятий нет :)"},
            { DayOfWeek.Friday,
                "11:55 - - / Комп.графика практика (101, 102), " +
                "13:45 - Разраб.компиляторов практика (118) / Функ.программирование лекция (312), " +
                "15:50 - Прогр.инженерия практика (ЛОС 1,2)"},
            { DayOfWeek.Saturday,
                "8:00 - Функ.программирование практика (202), " +
                "9:50 - Функ.программирование лекция (312), " +
                "11:55 - Прогр.инженерия лекция (312)"},
            { DayOfWeek.Sunday, "Ну какие пары? Воскресенье же!" }
        };

        public static string WhatSchedule(DayOfWeek day, string group, string course)
        {
            if (course.Contains("4") && (group.Contains("8") || group.Contains("9")))
                return group.Contains("8")  ? Schedule8[day] : Schedule9[day];
            return $"Информацию по расписанию {@group}.{course} не завезли :(";
        } 
    }
}