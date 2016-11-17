using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditPostbot.Models
{
    public class UserBase
    {
        public string Id;
        public List<string> Subreddits;
        public Hashtable LastRecievedNews;
        public TimeSpan BeginTime;
        public TimeSpan EndTime;
    }
}
