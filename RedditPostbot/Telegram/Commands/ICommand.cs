using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditPostbot.Models;
using Telegram.Bot.Types;

namespace RedditPostbot.Telegram.Commands
{
    interface ICommand
    {
        void Parse(Message command, TelegramUser user);
    }
}
