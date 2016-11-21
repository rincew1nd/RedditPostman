using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RedditPostbot.Settings;

namespace RedditPostbot.Telegram.Commands
{
    public class UnsubscribeCommand : Command
    {
        public UnsubscribeCommand() : base()
        {
            CommandRegex = new Regex("/unsubscribe( [\\w|\\d]+)+");
        }

        protected override void Do(List<string> args)
        {
            var messageBuilder = new StringBuilder();

            var deletedSubreddits = new List<string>();
            var ignoredSubreddits = new List<string>();

            foreach (var subreddit in args)
            {
                if (!User.Subreddits.Contains(subreddit))
                    ignoredSubreddits.Add(subreddit);
                else
                {
                    SettingsController.SettingsStore.RedditSettings.DeleteSubreddit(subreddit, User);
                    User.Subreddits.Remove(subreddit);
                    deletedSubreddits.Add(subreddit);
                }
            }

            SettingsController.GetInstance().SaveSettings();

            if (deletedSubreddits.Any())
                messageBuilder.Append($"Unsubscribed from: {string.Join(", ", deletedSubreddits)}\n");
            if (ignoredSubreddits.Any())
                messageBuilder.Append($"Wasn't subscribed to: {string.Join(", ", ignoredSubreddits)}");

            TelegramClient.SendMessage(User.ChatId, messageBuilder.ToString());
        }
    }
}
