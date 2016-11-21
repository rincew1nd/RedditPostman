using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RedditPostbot.Reddit;
using RedditPostbot.Settings;

namespace RedditPostbot.Telegram.Commands
{
    class SubscribeCommand : Command
    {
        public SubscribeCommand() : base()
        {
            CommandRegex = new Regex("/subscribe( [\\w|\\d]+)+");
        }

        protected override void Do(List<string> args)
        {
            var messageBuilder = new StringBuilder();
            
            var addedSubreddits = new List<string>();
            var ignoredSubreddits = new List<string>();

            foreach (var subreddit in args)
            {
                if (User.Subreddits.Contains(subreddit))
                    ignoredSubreddits.Add(subreddit);
                else
                {
                    SettingsController.SettingsStore.RedditSettings.AddSubreddit(subreddit);
                    User.Subreddits.Add(subreddit);
                    addedSubreddits.Add(subreddit);
                }
            }
            
            SettingsController.GetInstance().SaveSettings();
            
            if (addedSubreddits.Any())
                messageBuilder.Append($"Subscribed to: {string.Join(", ", addedSubreddits)}\n");
            if (ignoredSubreddits.Any())
                messageBuilder.Append($"Already subscribed to: {string.Join(", ", ignoredSubreddits)}");
            
            SettingsController.SettingsStore.RedditSettings.WatchedSubreddits =
                SettingsController.SettingsStore.RedditSettings.WatchedSubreddits
                .Union(addedSubreddits).ToList();
            
            TelegramClient.SendMessage(User.ChatId, messageBuilder.ToString());
        }
    }
}
