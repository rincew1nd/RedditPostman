using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedditPostbot.Models;
using RedditPostbot.Reddit;
using RedditPostbot.Settings;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace RedditPostbot.Telegram
{
    class TelegramClient
    {
        private readonly TelegramBotClient _client;
        private readonly List<TelegramUser> Users;

        public TelegramClient()
        {
            Users = SettingsController.SettingsStore.TelegramUsers;

            _client = new TelegramBotClient(SettingsController.SettingsStore.TelegramSettings.ApiKey);
            _client.OnMessage += HandleMessage;
            var me = _client.GetMeAsync().Result;
            Console.WriteLine($"Telegram client started! Name is {me.FirstName}.");

            _client.StartReceiving();
        }
        
        public async void HandleMessage(object sender, MessageEventArgs args)
        {
            var message = args.Message;
            var messageBuilder = new StringBuilder();

            if (message != null && message.Type == MessageType.TextMessage)
            {
                var user = Users.FirstOrDefault(z =>
                    z.Username == message.From.Username &&
                    z.ChatId == message.Chat.Id);

                if (user == null)
                {
                    user = new TelegramUser()
                    {
                        Username = message.From.Username,
                        ChatId = message.Chat.Id,
                        BeginTime = new TimeSpan(0, 0, 0),
                        EndTime = new TimeSpan(24, 0, 0),
                        LastRecievedNews = new Hashtable(),
                        Subreddits = new List<string>()
                    }; 

                    Users.Add(user);
                }

                if (message.Text.StartsWith("/subscribe"))
                {
                    var addedSubreddits = new List<string>();
                    var ignoredSubreddits = new List<string>();
                    var messageParts = message.Text.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var subreddit in messageParts.Skip(1).ToList())
                        if (user.Subreddits.Contains(subreddit))
                            ignoredSubreddits.Add(subreddit);
                        else
                        {
                            user.Subreddits.Add(subreddit);
                            addedSubreddits.Add(subreddit);
                        }

                    SettingsController.GetInstance().SaveSettings();

                    if (addedSubreddits.Any())
                        messageBuilder.Append($"Subscribed to: {string.Join(", ", addedSubreddits)}\n");
                    if (ignoredSubreddits.Any())
                    messageBuilder.Append($"Already subscribed to: {string.Join(", ", ignoredSubreddits)}");
                    
                    SettingsController.SettingsStore.RedditSettings.WatchedSubreddits =
                        SettingsController.SettingsStore.RedditSettings.WatchedSubreddits
                        .Union(addedSubreddits).ToList();

                    await _client.SendTextMessageAsync(message.Chat.Id, messageBuilder.ToString());
                } else if (message.Text.StartsWith("/unsubscribe"))
                {
                    var deletedSubreddits = new List<string>();
                    var ignoredSubreddits = new List<string>();
                    var messageParts = message.Text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    
                    foreach (var subreddit in messageParts.Skip(1).ToList())
                        if (!user.Subreddits.Contains(subreddit))
                            ignoredSubreddits.Add(subreddit);
                        else
                        {
                            user.Subreddits.Remove(subreddit);
                            deletedSubreddits.Add(subreddit);
                        }

                    SettingsController.GetInstance().SaveSettings();

                    if (deletedSubreddits.Any())
                        messageBuilder.Append($"Unsubscribed from: {string.Join(", ", deletedSubreddits)}\n");
                    if (ignoredSubreddits.Any())
                        messageBuilder.Append($"Wasn't subscribed to: {string.Join(", ", ignoredSubreddits)}");

                    await _client.SendTextMessageAsync(message.Chat.Id, messageBuilder.ToString());
                } else if (message.Text.StartsWith("/timeinterval"))
                {
                    await _client.SendTextMessageAsync(message.Chat.Id,
                        $"Not implemented right now :<");
                } else if (message.Text.StartsWith("/stop"))
                {
                    user.Subreddits.Clear();

                    await _client.SendTextMessageAsync(message.Chat.Id,
                        $"You are unsubscribed from all subreddits.\nGood bye.");
                }
                else
                {
                    var helpMessage = "Usage:\n" +
                        "/subscribe subreddit1 subreddit2 - subscribe to subreddits\n" +
                        "/unsubscribe subreddit1 subreddit2 - subscribe to subreddits\n" +
                        "/timeinterval 12.00am 03.23pm GTM+3 - set time interval when you will recieve message\n" +
                        "/stop - stop using this bot\n" +
                        "/help or /h for this help message";
                    await _client.SendTextMessageAsync(message.Chat.Id, helpMessage);
                }
            }
        }

        public async void NotifyUsers(List<RedditTopic> topics)
        {
            foreach (var redditTopic in topics)
            {
                var users = Users.Where(u => u.Subreddits.Contains(redditTopic.Subreddit)).ToList();
                foreach (var telegramUser in users)
                {
                    await _client.SendTextMessageAsync(telegramUser.ChatId,
                        redditTopic.MakeMessage(),
                        parseMode: ParseMode.Html);
                }
            }
        }
    }
}
