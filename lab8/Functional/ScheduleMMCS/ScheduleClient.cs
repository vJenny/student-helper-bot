using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;

namespace lab8.Functional
{
    public class MMCSClient
    {
        private readonly HttpClient _cli = new HttpClient();

        public async Task<LessonRecord[]> StudentSchedule(int course, int group, int day)
        {
            var groups = await _cli.GetStringAsync($"http://users.mmcs.sfedu.ru:3000/APIv0/group/list/{course}");
            dynamic groupsData = Newtonsoft.Json.JsonConvert.DeserializeObject(groups);

            var gid = "";
            foreach (var g in groupsData)
            {
                if (g.num == group.ToString())
                    gid = g.id;
            }

            return await GetLessons($"http://users.mmcs.sfedu.ru:3000/APIv0/schedule/group/{gid}", day);
        }

        private async Task<LessonRecord[]> GetLessons(string request, int day)
        {
            var res = await _cli.GetStringAsync(request);
            var f = new List<LessonRecord>();

            dynamic x = Newtonsoft.Json.JsonConvert.DeserializeObject(res);
            var lessons = x.lessons;
            var curricula = x.curricula;


            foreach (var c in curricula)
            {
                var id = c.lessonid.ToString();

                var timeslot = new TimeSlot();
                foreach (var l in lessons)
                    if (l.id.ToString() == id)
                        timeslot = TimeSlot.Parse(l.timeslot.ToString());

                f.Add(new LessonRecord(
                    c.subjectname.ToString(),
                    c.roomname.ToString(),
                    c.teachername.ToString(),
                    timeslot
                ));
            }

            return f
                .Where(z => z.Time.Position == day)
                .OrderBy(z => z.Time.Start)
                .ToArray();
        }

        public async Task<LessonRecord[]> TeacherSchedule(string namePattern, int day)
            => await TeacherSchedule(new Regex(namePattern), day);

        private async Task<LessonRecord[]> TeacherSch(string id, int day)
        {
            return await GetLessons($"http://users.mmcs.sfedu.ru:3000/APIv1/schedule/teacher/{id}", day);
        }

        // TODO: Возвращает только первого преподавателя, удовл. шаблоную
        public async Task<LessonRecord[]> TeacherSchedule(Regex namePattern, int day)
        {
            var data = await _cli.GetStringAsync(@"http://users.mmcs.sfedu.ru:3000/APIv0/teacher/list");
            dynamic teachers = Newtonsoft.Json.JsonConvert.DeserializeObject(data);

            foreach (var t in teachers)
            {
                var name = t.name.ToString();

                if (namePattern.IsMatch(name))
                    return await TeacherSch(t.id.ToString(), day);
                
            }

            return new LessonRecord[] { };
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
            var start = TimeSpan.Parse(spl[1]);
            var end = TimeSpan.Parse(spl[2]);
            var week = (LessonWeek)Enum.Parse(typeof(LessonWeek), spl[3], true);

            return new TimeSlot(pos, start, end, week);
        }

        public TimeSlot(int pos, TimeSpan start, TimeSpan end, LessonWeek week) : this()
        {
            Week = week;
            Position = pos;
            Start = start;
            End = end;
        }

        public LessonWeek Week { get; }
        public int Position { get; }
        public TimeSpan Start { get; }
        public TimeSpan End { get; }

        public override string ToString()
        {
            var week = Week == LessonWeek.Lower ? "нижняя неделя" : (Week == LessonWeek.Upper ? "верхняя неделя" : "");
            return $"{Position}: {Start} -- {End} | {week}";
        }
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

        public override string ToString()
        {
            return $"{ Time } -- {SubjectName} -- {RoomName} -- {TeacherName}";
        }
    }
}