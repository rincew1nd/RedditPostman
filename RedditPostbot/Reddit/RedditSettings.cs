using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RedditPostbot.Models;
using RedditPostbot.Settings;

namespace RedditPostbot.Reddit
{
    public class RedditSettings
    {
        public Hashtable LastSubredditsNews;
        public List<string> WatchedSubreddits;
        
        public void DeleteSubreddit(string subreddit, TelegramUser user)
        {
            var users = SettingsController.SettingsStore.TelegramUsers
                .Where(u => u.Subreddits.Contains(subreddit)).ToList();
            if (users.Count == 1 && users.Contains(user))
            {
                WatchedSubreddits.Remove(subreddit);
                LastSubredditsNews.Remove(subreddit);
            }
        }

        public void AddSubreddit(string subreddit)
        {
            if (WatchedSubreddits.Contains(subreddit)) return;
            SubredditParser.GetInstance().AddNewSubreddit(subreddit);
            WatchedSubreddits.Add(subreddit);
        }
    }
}