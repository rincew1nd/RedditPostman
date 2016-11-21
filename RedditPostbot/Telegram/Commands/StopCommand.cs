using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RedditPostbot.Telegram.Commands
{
    public class StopCommand : Command
    {
        private const string StopMessage = "You are unsubscribed from all subreddits.\nGood bye.";

        public StopCommand() : base()
        {
            CommandRegex = new Regex("/stop");
        }
        
        protected override void Do(List<string> args)
        {
            User.Subreddits.Clear();
            TelegramClient.SendMessage(User.ChatId, StopMessage);
        }
    }
}
