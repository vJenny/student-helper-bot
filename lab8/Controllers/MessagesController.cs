using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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
        private static StudentHelper _sh = new StudentHelper();
        private delegate Task<string> BotTask();

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                var state = activity.GetStateClient();
                var userData = await state.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);

                var user = userData.GetProperty<StudentHelper>("profile");
                if (user != null) _sh = user;

                var text = await Reply(activity.Text);
                var reply = activity.CreateReply(text);
                userData.SetProperty("profile", _sh);
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

        private static Dictionary<string, BotTask> Commands => new Dictionary<string, BotTask>
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
            { "дела", _sh.HowAreYou }
        };

        public async Task<string> Reply(string msg)
        {
            var a = msg.ToLower().Split(' ');

            if (a.IsPresent("зовут"))
                return _sh.SetName(a.NextTo("зовут"));
            if (a.IsPresent("имя"))
                return _sh.SetName(a.NextTo("имя"));
            if (a.IsPresent("групп"))
                return _sh.SetGroup(a.PrevTo("групп"));
            if (a.IsPresent("курс"))
                return _sh.SetCourse(a.PrevTo("курс"));
            foreach (var cmd in Commands)
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