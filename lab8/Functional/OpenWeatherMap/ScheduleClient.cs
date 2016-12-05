using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace lab8.Functional.OpenWeatherMap
{
    public class MMCSClient
    {
        public string AppId { get; set; }

        private readonly HttpClient _cli = new HttpClient();

        public MMCSClient(string appId)
        {
            AppId = appId;
        }

        public async Task<LessonRecord[]> StudentSchedule(int group)
        {
            // TODO
            throw new NotImplementedException();

            var res = await _cli.GetStringAsync("");
            var f = new List<LessonRecord>();

            dynamic x = Newtonsoft.Json.JsonConvert.DeserializeObject(res);
            var lessons = x.lessons;
            var curricula = x.curricula;

            var i = 0;
            foreach(var c in curricula)
            {
                var timeslot = TimeSlot.Parse(lessons[i].timeslot);

                f.Add(new LessonRecord(
                    c.subjectname,
                    c.roomname,
                    c.teachername,
                    timeslot
                ));

                i++;
            }

            return f.ToArray();
        }
    }

    public enum LessonWeek
    {
        Upper,
        Lower, // ???
        Full
    }

    public struct TimeSlot
    {
        public static TimeSlot Parse(string s)
        {
            var clean = s.Substring(1, s.Length - 2);
            var spl = clean.Split(',');

            var pos = int.Parse(spl[0]);
            var start = DateTime.Parse(spl[1]);
            var end = DateTime.Parse(spl[2]);
            var week = (LessonWeek)Enum.Parse(typeof(LessonWeek), spl[3], true);

            return new TimeSlot(pos, start, end, week);
        }

        public TimeSlot(int pos, DateTime start, DateTime end, LessonWeek week) : this()
        {
            Week = week;
            Position = pos;
            Start = start;
            End = end;
        }

        public LessonWeek Week { get; }
        public int Position { get; }
        public DateTime Start { get; }
        public DateTime End { get; }
    }

    public struct LessonRecord
    {
        public LessonRecord(string name, string room, string teacher, TimeSlot time) : this()
        {
            Time = time;
            RoomName = room;
            SubjectName = name;
            TeacherName = teacher;
        }

        public TimeSlot Time { get; }
        public string TeacherName { get; }
        public string SubjectName { get; }
        public string RoomName { get; }
    }
}