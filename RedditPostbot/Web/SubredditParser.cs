using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using RedditPostbot.Models;

namespace RedditPostbot
{
    public class SubredditParser
    {
        private static SubredditParser _instance;
        private static object _instanceLock = new object();

        private WebClient _webClient;
        private static object _webClientLock = new object();

        private Hashtable _hashTable;
        private string _subredditUrl = "https://www.reddit.com/r/{0}/new.json";
        private string _subredditUrlParametrs = "https://www.reddit.com/r/{0}/new.json?before={1}&after={2}";

        public SubredditParser()
        {
            _webClient = new WebClient();
            _hashTable = new Hashtable();
        }

        public static SubredditParser GetInstance()
        {
            if (_instance == null)
                lock (_instanceLock)
                    if (_instance == null)
                        _instance = new SubredditParser();
            return _instance;
        }

        public List<TopicData> GetSubredditNewTopics(string subreddit)
        {
            lock (_webClientLock)
            {
                while (_webClient.IsBusy) { Console.WriteLine("Im busy! >:3"); }
                SubredditJsonModel model;
                if (_hashTable.ContainsKey(subreddit))
                {
                    var rawJson = _webClient.DownloadString(string.Format(_subredditUrlParametrs, subreddit, _hashTable[subreddit], ""));
                    model = new JavaScriptSerializer().Deserialize<SubredditJsonModel>(rawJson);
                    if (model.data.children.Any())
                        _hashTable[subreddit] = model.data.children.First().data.Name;
                }
                else
                {
                    var rawJson = _webClient.DownloadString(string.Format(_subredditUrl, subreddit));
                    model = new JavaScriptSerializer().Deserialize<SubredditJsonModel>(rawJson);
                    _hashTable.Add(subreddit, model.data.children.First().data.Name);
                }
                return model.data.children.Select(z => z.data).ToList();
            }
        }
    }
}
