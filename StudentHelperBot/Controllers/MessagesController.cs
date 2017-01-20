﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using StudentHelperBot.Utilits;
using Microsoft.Bot.Connector;

namespace lab8.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private delegate Task<string> BotTask();

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            try
            {
                if (activity.Type == ActivityTypes.Message)
                {
                    var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                    var state = activity.GetStateClient();
                    var userData = await state.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);

                    var profile = $"profile{activity.Conversation.Id}";

                    var user = userData.GetProperty<StudentHelper>(profile) ?? new StudentHelper();

                    var text = await Reply(activity.Text, user);
                    var reply = activity.CreateReply(text);
                    userData.SetProperty(profile, user);
                    await state.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
                else
                {
                    HandleSystemMessage(activity);
                }
                var response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }
            catch
            {
                var response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }
        }

        private static Dictionary<string, BotTask> Commands(StudentHelper _sh) => new Dictionary<string, BotTask>
        {
            { "/about", _sh.Greeting },
            { "/help", _sh.Help },
            { "/name", _sh.Help },
            { "/course", _sh.Help },
            { "/group", _sh.Hello },
            { "/bachelor", _sh.Hello },
            { "/master", _sh.Hello },
            { "/dean", _sh.GetDeansOfficeSchedule },
            { "/canteen", _sh.GetDiningHallMenu },
            { "/schedule", _sh.GetDiningHallMenu },
            { "/teacher",  _sh.GetDiningHallMenu },
            { "/weather", _sh.GetDiningHallMenu },
            { "/city", _sh.GetDiningHallMenu },
            { "/reset", _sh.GetSchedule }
        };

        public async Task<string> Reply(string msg, StudentHelper sh)
        {
            var message = msg.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (message[0][0] != '/')
                return @"Команда должна начинаться с '/'";
            if (message[0] == "/name")
            {
                sh.SetName(string
                    .Join(" ", message
                    .Skip(1)
                    .Select(str => 
                    CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower()))));
            }

           /* if (a.IsPresent("зовут"))
                return _sh.SetName(a.NextTo("зовут"));
            if (a.IsPresent("имя"))
                return _sh.SetName(a.NextTo("имя"));
            if (a.IsPresent("групп"))
            {
                var g = a.PrevTo("групп");
                g = g == "" ? a.NextTo("групп") : g;
                return _sh.SetGroup(g);
            }
            if (a.IsPresent("курс"))
            {
                var c = a.PrevTo("курс");
                c = c == "" ? a.NextTo("курс") : c;
                return _sh.SetCourse(c);
            }
            if (a.IsPresent("препод"))
                return await _sh.GetLecturerSchedule(a.TakeName("препод")); */

            var commands = Commands(sh);
            /*foreach (var cmd in commands)
                if (a.IsPresent(cmd.Key))
                    return await cmd.Value.Invoke(); */
            return "";
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {}
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {}
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {}
            else if (message.Type == ActivityTypes.Typing)
            {}
            else if (message.Type == ActivityTypes.Ping)
            {}
            return null;
        }
    }
}