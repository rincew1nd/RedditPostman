using System;
using System.Security.Policy;
using System.Text;

namespace RedditPostbot.Models
{
    public class RedditTopic
    {
        public string Author;
        public string Id;
        public string Name;
        public string Title;
        public string SelfText;
        public string Subreddit;
        public string Permalink;
        public string Url;

        public string MakeMessage()
        {
            var permalink = Permalink.Substring(0, Permalink.Substring(0, Permalink.Length - 1).LastIndexOf('/'));
            var message = new StringBuilder();
            message.Append($"<i>New post in {Subreddit}!</i>\n\n");
            message.Append($"<b>{Title} by {Author}</b>\n\n");
            if ($"https://www.reddit.com{Permalink}" != Url)
                message.Append($"Content link: {Url}\n");
            message.Append($"Reddit post: http://reddit.com{permalink}");
            return message.ToString();
        }
    }
}