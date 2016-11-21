using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RedditPostbot.Telegram.Commands
{
    public class HelpCommand : Command
    {
        public HelpCommand() : base()
        {
            CommandRegex = new Regex("^/help$");
        }

        private const string HelpMessage = @"Usage:
            /subscribe subreddit1 subreddit2 - subscribe to subreddits
            /unsubscribe subreddit1 subreddit2 - subscribe to subreddits
            /timeinterval 12.00am 03.23pm GTM+3 - set time interval when you will recieve message
            /stop - stop using this bot
            /help or /h for this help message";

        protected override void Do(List<string> list) =>
            TelegramClient.SendMessage(User.ChatId, HelpMessage);
    }
}
