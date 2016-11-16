using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditPostbot.Models
{
    public class SubredditJsonModel
    {
        public string kind;
        public SubredditDataJsonModel data;
    }

    public class SubredditDataJsonModel
    {
        public string modhash;
        public List<Topic> children;
        public string After;
        public string before;
    }
    public class Topic
    {
        public string kind;
        public TopicData data;
    }
    public class TopicData
    {
        public string Author;
        public string Id;
        public string Name;
        public string Title;
        public string SelfText;
        public string Subreddit;
    }
}
