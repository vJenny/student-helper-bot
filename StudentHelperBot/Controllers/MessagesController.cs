using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using StudentHelperBot.Utilits;
using Microsoft.Bot.Connector;

namespace StudentHelperBot.Controllers
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

        public async Task<string> Reply(string msg, StudentHelper sh)
        {
            var message = msg.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (message.Length == 0)
                return @"Введите команду :)";
            if (message[0][0] != '/')
                return @"Команда должна начинаться с '/'";
            switch (message[0])
            {
                case "/имя":
                    return sh.SetName(message.TakeName());
                case "/курс":
                    return sh.SetCourse(message.TakeNextNumber());
                case "/группа":
                    return sh.SetGroup(message.TakeNextNumber());
                case "/преподаватель":
                    return await sh.GetLecturerSchedule(message.TakeName());
                case "/справка":
                    return sh.Help();
                case "/деканат":
                    return sh.GetDeansOfficeSchedule();
                case "/столовая":
                    return sh.GetDiningHallMenu();
                case "/привет":
                    return sh.Hello();
                case "/расписание":
                    return await sh.GetSchedule();
                case "/погода":
                    return await sh.GetWeather();
                case "/сброс":
                    return await sh.Reset();
                default:
                    return "Неизвестная команда :(";
            }
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