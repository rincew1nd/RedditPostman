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

        private readonly RedditSettings _redditSettings;

        public SubredditParser()
        {
            _webClient = new WebClient();
            _redditSettings = SettingsController.SettingsStore.RedditSettings;
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
            string rawJson;
            
            lock (_webClientLock)
            {
                rawJson =
                    _webClient.DownloadString(_redditSettings.LastSubredditsNews.ContainsKey(subreddit)
                        ? string.Format(SubredditUrlParametrs, subreddit,
                            _redditSettings.LastSubredditsNews[subreddit], "")
                        : string.Format(SubredditUrl, subreddit));
            }

            var model = new JavaScriptSerializer().Deserialize<SubredditJsonModel>(rawJson);
            if (model.data.children.Any())
            {
                if (_redditSettings.LastSubredditsNews.ContainsKey(subreddit))
                    _redditSettings.LastSubredditsNews[subreddit] =
                        model.data.children.First().data.Name;
                else
                    _redditSettings.LastSubredditsNews.Add(subreddit,
                        model.data.children.First().data.Name);
            }

            return model.data.children.Select(z => z.data).ToList();
        }
    }
}
