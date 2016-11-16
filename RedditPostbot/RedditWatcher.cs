using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RedditPostbot.Enums;
using RedditPostbot.Models;
using Timer = System.Timers.Timer;

namespace RedditPostbot
{
    public class RedditWatcher
    {
        private static RedditWatcher _instance;
        private static object _instanceLock = new object();

        public delegate void NewsFound(List<TopicData> topics);
        public event NewsFound OnNewsUpdated;

        public WatchState WatchState { get; private set; }
        private object _watchStateLock = new object();

        private List<string> _subreddits;

        public List<Task> taskCollection;

        public RedditWatcher() { }

        public static RedditWatcher GetInstance()
        {
            if (_instance == null)
                lock (_instanceLock)
                    if (_instance == null)
                        _instance = new RedditWatcher();
            return _instance;
        }

        public void StartWatch(List<string> subreddits)
        {
            lock (_watchStateLock)
            {
                if (WatchState != WatchState.Stoped)
                {
                    throw new Exception("Reddit watcher already running!");
                }

                WatchState = WatchState.Starting;
                _subreddits = subreddits;
                taskCollection = new List<Task>();

                WatchState = WatchState.Running;
                new Thread(GetNews).Start();
            }
        }

        public void GetNews()
        {
            var topics = new List<TopicData>();

            foreach (var subredditName in _subreddits)
            {
                taskCollection.Add(Task.Factory.StartNew(() =>
                {
                    topics.AddRange(SubredditParser.GetInstance().GetSubredditNewTopics(subredditName));
                }));
            }
            Task.WaitAll(taskCollection.ToArray());

            OnNewsUpdated.Invoke(topics);

            Thread.Sleep(60000);
            new Thread(GetNews).Start();
        }
    }
}
