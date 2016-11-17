using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditPostbot.Models;
using RedditPostbot.Reddit;
using RedditPostbot.Telegram;

namespace RedditPostbot.Settings
{
    public class SettingsStore
    {
        public List<TelegramUser> TelegramUsers;
        public TelegramSettings TelegramSettings;
        public RedditSettings RedditSettings;
    }
}
