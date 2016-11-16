using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedditPostbot
{
    class Program
    {
        static void Main(string[] args)
        {
            var watcher = RedditWatcher.GetInstance();
            watcher.StartWatch(new List<string>() {"3dshacks", "vitapiracy"});
            var messageQueue = new TelegramMessageQueue();
            watcher.OnNewsUpdated += messageQueue.MakeMessageQueue;

            Console.ReadLine();
        }
    }
}
