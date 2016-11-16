using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditPostbot.Models;

namespace RedditPostbot
{
    class TelegramMessageQueue
    {
        //public List<string> users;

        public void MakeMessageQueue(List<TopicData> topics)
        {
            foreach (var topic in topics)
                Console.WriteLine($"!{topic.Subreddit}! [{topic.Author}] {topic.Title} {topic.Name}");
        }
    }
}
