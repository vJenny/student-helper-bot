using System;
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

                    var text = await Reply(activity.Text, user, activity.Conversation.Name);
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

        public async Task<string> Reply(string msg, StudentHelper sh, string user)
        {
            var message = msg.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (message.Length == 0)
                return @"Введите команду :)";
            if (message[0][0] != '/')
                return @"Команда должна начинаться с '/'";
            switch (message[0])
            {
                case "/name":
                    return sh.SetName(message.TakeName());
                case "/course":
                    return sh.SetCourse(message.TakeNextNumber());
                case "/group":
                    return sh.SetGroup(message.TakeNextNumber());
                case "/lecturer":
                    return await sh.GetLecturerSchedule(message.TakeName());
                case "/help":
                    return sh.Help();
                case "/dean":
                    return sh.GetDeansOfficeSchedule();
                case "/canteen":
                    return sh.GetDiningHallMenu();
                case "/hello":
                    return sh.Hello(user);
                case "/schedule":
                    return await sh.GetSchedule();
                case "/weather":
                    return await sh.GetWeather();
                case "/reset":
                    return sh.Reset();
                case "/bachelor":
                    return sh.SetDegree(StudentHelper.Degrees.Bachelor);
                case "/master":
                    return sh.SetDegree(StudentHelper.Degrees.Master);
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