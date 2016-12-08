using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Http;
using lab8.Functional;
using lab8.Properties;
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
            { "помоги", _sh.Greeting },
            { "делать", _sh.Greeting },
            { "команды", _sh.Help },
            { "умеешь", _sh.Help },
            { "привет", _sh.Hello },
            { "здравствуй", _sh.Hello },
            { "здоров", _sh.Hello },
            { "декан", _sh.GetDeansOfficeSchedule },
            { "столов", _sh.GetDiningHallMenu },
            { "кушать", _sh.GetDiningHallMenu },
            { "голод",  _sh.GetDiningHallMenu },
            { "жрать", _sh.GetDiningHallMenu },
            { "есть", _sh.GetDiningHallMenu },
            { "пары", _sh.GetSchedule },
            { "расписание", _sh.GetSchedule },
            { "идти",  _sh.GetWeather },
            { "погода",  _sh.GetWeather },
            { "спать",  _sh.GetWeather },
            { "никуда",  _sh.GetWeather },
            { "дела", _sh.HowAreYou },
            { "start", _sh.Greeting },
            { "help", _sh.Help },
            { "reset", _sh.Reset }
        };

        public async Task<string> Reply(string msg, StudentHelper _sh)
        {
            var a = msg.ToLower().Split(' ');

            if (a.IsPresent("зовут"))
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
                return await _sh.GetLecturerSchedule(a.TakeName("препод"));
            var commands = Commands(_sh);
            foreach (var cmd in commands)
                if (a.IsPresent(cmd.Key))
                    return await cmd.Value.Invoke();
            return Resources.errorMsg;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}