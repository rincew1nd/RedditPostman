using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RedditPostbot.Models;
using Telegram.Bot.Types;

namespace RedditPostbot.Telegram.Commands
{
    public abstract class Command : ICommand
    {
        protected Regex CommandRegex;
        protected TelegramUser User;
        
        public void Parse(Message message, TelegramUser user)
        {
            User = user;

            var command = message.Text;
            if (CommandRegex.IsMatch(command))
                Do(CommandArgs(command));
        }

        protected abstract void Do(List<string> args);

        public List<string> CommandArgs(string command)
        {
            return command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();
        }
    }
}
