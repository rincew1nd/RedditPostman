using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RedditPostbot.Enums;
using RedditPostbot.Models;
using RedditPostbot.Reddit;
using RedditPostbot.Settings;
using Timer = System.Timers.Timer;

namespace RedditPostbot
{
    public class RedditWatcher
    {
        private static RedditWatcher _instance;
        private static object _instanceLock = new object();

        public delegate void NewsFound(List<RedditTopic> topics);
        public event NewsFound OnNewsUpdated;

        private WatchState _watchState;
        private object _watchStateLock = new object();

        private RedditSettings _redditSettings;
        private List<Task> _taskCollection;

        public RedditWatcher() { }

        public static RedditWatcher GetInstance()
        {
            if (_instance == null)
                lock (_instanceLock)
                    if (_instance == null)
                        _instance = new RedditWatcher();
            return _instance;
        }

        public void StartWatch()
        {
            lock (_watchStateLock)
            {
                if (_watchState != WatchState.Stoped)
                {
                    throw new Exception("Reddit watcher already running!");
                }

                _watchState = WatchState.Starting;
                _taskCollection = new List<Task>();
                _redditSettings = SettingsController.SettingsStore.RedditSettings;

                _redditSettings.WatchedSubreddits = new List<string>();
                foreach (var subreddit in SettingsController.SettingsStore.TelegramUsers.Select(u => u.Subreddits))
                    _redditSettings.WatchedSubreddits =
                        _redditSettings.WatchedSubreddits.Union(subreddit).ToList();

                _watchState = WatchState.Running;
                new Thread(GetNews).Start();
            }
        }

        public void GetNews()
        {
            var topics = new List<RedditTopic>();
            
            foreach (var subredditName in _redditSettings.WatchedSubreddits)
            {
                _taskCollection.Add(Task.Factory.StartNew(() =>
                {
                    topics.AddRange(SubredditParser.GetInstance().GetSubredditNewTopics(subredditName));
                }));
            }
            Task.WaitAll(_taskCollection.ToArray());
            
            OnNewsUpdated.Invoke(topics);
            SettingsController.GetInstance().SaveSettings();

            Thread.Sleep(60000);
            new Thread(GetNews).Start();
        }
    }
}
