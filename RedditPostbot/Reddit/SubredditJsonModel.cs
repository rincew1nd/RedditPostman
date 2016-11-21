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
        public List<TopicsJsonModel> children;
        public string After;
        public string before;
    }
    public class TopicsJsonModel
    {
        public string kind;
        public RedditTopic data;
    }
}
