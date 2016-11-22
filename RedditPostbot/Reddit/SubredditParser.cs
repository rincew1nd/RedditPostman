using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using RedditPostbot.Models;
using RedditPostbot.Reddit;
using RedditPostbot.Settings;

namespace RedditPostbot
{
    public class SubredditParser
    {
        private static SubredditParser _instance;
        private static readonly object _instanceLock = new object();

        private readonly WebClient _webClient;
        private static readonly object _webClientLock = new object();

        private const string SubredditUrl = "https://www.reddit.com/r/{0}/new.json";
        private const string SubredditUrlParametrs = "https://www.reddit.com/r/{0}/new.json?before={1}&after={2}";

        private readonly Hashtable _subredditsLastestNews;

        public SubredditParser()
        {
            _webClient = new WebClient();
            _subredditsLastestNews = SettingsController.SettingsStore.RedditSettings.LastSubredditsNews;
        }

        public static SubredditParser GetInstance()
        {
            if (_instance == null)
                lock (_instanceLock)
                    if (_instance == null)
                        _instance = new SubredditParser();
            return _instance;
        }

        public List<RedditTopic> GetSubredditNewTopics(string subreddit)
        {
            var subredditLastNewsId = GetSubredditLastNewsId(subreddit);

            var rawJson = GetSubredditJson(subreddit, subredditLastNewsId);
            var model = new JavaScriptSerializer().Deserialize<SubredditJsonModel>(rawJson);

            if (model == null) return new List<RedditTopic>();
            UpdateLastNewsId(model, subredditLastNewsId);
            return model.data.children.Select(z => z.data).ToList();
        }

        public void AddNewSubreddit(string subreddit)
        {
            if (GetSubredditLastNewsId(subreddit) != null) return;

            var rawJson = GetSubredditJson(subreddit);
            var model = new JavaScriptSerializer().Deserialize<SubredditJsonModel>(rawJson);
            UpdateLastNewsId(model);
        }

        private string GetSubredditJson(string subreddit, string lastId = null)
        {
            lock (_webClientLock)
            {
                var url = lastId == null
                    ? string.Format(SubredditUrl, subreddit)
                    : string.Format(SubredditUrlParametrs, subreddit, lastId, null);
                try
                {
                    return _webClient.DownloadString(url);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message} ({url})");
                    return "";
                }
            }
        }

        private void UpdateLastNewsId(SubredditJsonModel model, string subredditLastNewsId = null)
        {
            if (!model.data.children.Any()) return;

            var lastNews = model.data.children.First().data;
            if (string.IsNullOrEmpty(subredditLastNewsId))
                _subredditsLastestNews.Add(lastNews.Subreddit, lastNews.Name);
            else
                _subredditsLastestNews[lastNews.Subreddit.ToLower()] = lastNews.Name;
        }

        private string GetSubredditLastNewsId(string subreddit)
        {
            return _subredditsLastestNews.ContainsKey(subreddit) ?
                   _subredditsLastestNews[subreddit].ToString() : null;
        }
    }
}
