using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using RedditPostbot.Models;
using RedditPostbot.Reddit;
using RedditPostbot.Settings;
using RedditPostbot.Telegram;

namespace RedditPostbot
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load settings
            SettingsController.GetInstance();

            // Start Telegram client
            var telegramClient = new TelegramClient();

            // Start to watch for reddit
            var watcher = RedditWatcher.GetInstance();

            // Print out news to telegram
            watcher.OnNewsUpdated += telegramClient.NotifyUsers;

            watcher.StartWatch();
            Console.ReadLine();
        }
    }
}
