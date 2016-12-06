using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using lab8.Functional;
using Microsoft.Bot.Connector;

namespace lab8.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private StudentHelper _sh = new StudentHelper();

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

        private async Task<string> Reply(string msg)
        {
            var a = msg.ToLower().Split(' ');
            if (a.IsPresent("помоги") || a.IsPresent("делать"))
                return @"Привет, я - твой бот-помощник.
                         Скажи, как тебя зовут, твой курс и группу,
                         тогда я смогу быть тебе полезным :)";
            if (a.IsPresent("команды") || a.IsPresent("умеешь"))
                return @"Я могу узнать Ваше расписание, 
                    рассказать, чем можно подкрепиться в столовой, 
                    подсказать часы работы деканата, 
                    узнать для Вас погоду.";
            if (a.IsPresent("привет") || a.IsPresent("здравствуй"))
                return $"Привет, " + _sh.Name;
            if (a.IsPresent("зовут")) 
            {
                _sh.Name = a.NextTo("зовут");
                return "Приятно познакомиться, " + _sh.Name;
            }
            if (a.IsPresent("имя"))
            {
                _sh.Name = a.NextTo("имя");
                return "Приятно познакомиться, " + _sh.Name;
            }
            if (a.IsPresent("дела")) return "Отлично! Я же бот.";
            if (a.IsPresent("групп"))
            {
                _sh.Group = a.NextTo("групп");
                return "Окей!";
            }
            if (a.IsPresent("курс"))
            {
                _sh.Course = a.NextTo("курс");
                return "Окей!";
            }
            if (a.IsPresent("деканат"))
                return DeansOffice.WhatSchedule(DateTime.Now.DayOfWeek);
            if (a.IsPresent("столов") || a.IsPresent("кушать") || a.IsPresent("голод"))
                return DiningHall.WhatToEat(DateTime.Now.DayOfWeek);
            if (a.IsPresent("пары") || a.IsPresent("расписание"))
                return Schedule.WhatSchedule(DateTime.Now.DayOfWeek, _sh.Group, _sh.Course);
            if (a.IsPresent("идти") || a.IsPresent("погода") || a.IsPresent("спать") || a.IsPresent("никуда"))
                return await _sh.BuildResult();
            return $"Глупый бот Вас не понимать. Пните разработчика :(";
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