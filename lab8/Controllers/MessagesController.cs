using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using lab8.Functional;
using Microsoft.Bot.Connector;
using AIMLbot;
using System.IO;

namespace lab8.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private StudentHelper _sh = new StudentHelper();
        private Bot _bot = new Bot();
        private User _user;

        public MessagesController()
        {
            _user = new User("user", _bot);

            _bot.loadSettings(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "AimlData", "config", "Settings.xml"));

            _bot.isAcceptingUserInput = false;
            _bot.loadAIMLFromFiles();
            _bot.isAcceptingUserInput = true;
        }

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

                _bot.DefaultPredicates.updateSetting("name", _sh.Name);
                _bot.DefaultPredicates.updateSetting("course", _sh.Course);
                _bot.DefaultPredicates.updateSetting("group", _sh.Group);

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

        /// <summary>
        /// Функция предобработки сообщения бота
        /// </summary>
        private async Task<string> BotOutputPreprocess(string message)
        {
            // Обрабатывает запросы вида: 1|2|3|4|5
            // Это могут быть текстовые сообщения или названия методов (Void -> Task<String>), 
            // которые есть у _sh. Это не обязательно может быть StudentHelper
            var spl = message.Split('|');
            var outMessage = new System.Text.StringBuilder();

            var t = _sh.GetType();
            foreach (var req in spl)
            {
                var m = t.GetMethod(req);

                if (m == null || !(m.ReturnType == typeof(Task<string>)))
                {
                    outMessage.Append(req + " ");
                }
                else
                {
                    var resp = m.Invoke(_sh, null) as Task<string>;
                    var s = await resp;
                    outMessage.Append(s + " ");
                }
            }

            return outMessage.ToString();
        }

        private async Task<string> Reply(string msg)
        {
            try
            {
                var req = new Request(msg, _user, _bot);
                var resp = _bot.Chat(req);

                _sh.Name = _bot.GlobalSettings.grabSetting("name");
                _sh.Group = _bot.GlobalSettings.grabSetting("group");
                _sh.Course = _bot.GlobalSettings.grabSetting("course");

                var res = await BotOutputPreprocess(resp.Output);

                return res;
            }
            catch(Exception e)
            {
#if DEBUG
                return e.Message;
#endif
            }
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