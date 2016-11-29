using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using RedditPostbot.Models;
using RedditPostbot.Settings;
using RedditPostbot.Telegram.Commands;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using RedditPostbot.Utils;

namespace RedditPostbot.Telegram
{
    class TelegramClient
    {
        private static TelegramBotClient _client;
        private readonly List<TelegramUser> _users;
        private readonly List<Command> _commands;

        public TelegramClient()
        {
            _users = SettingsController.SettingsStore.TelegramUsers;

            _client = new TelegramBotClient(SettingsController.SettingsStore.TelegramSettings.ApiKey);
            _client.OnMessage += HandleMessage;
            var me = _client.GetMeAsync().Result;
            Console.WriteLine($"Telegram client started! Name is {me.FirstName}.");

            _commands = new List<Command>
            {
                new UnsubscribeCommand(),
                new SubscribeCommand(),
                new HelpCommand(),
                new StopCommand()
            };

            _client.StartReceiving();
        }
        
        public void HandleMessage(object sender, MessageEventArgs args)
        {
            var message = args.Message;

            if (message != null && message.Type == MessageType.TextMessage)
            {
                var user = FindUser(message);

                foreach (var command in _commands)
                    command.Parse(message, user);

                if (message.Text.StartsWith("/timeinterval"))
                {
                    SendMessage(message.Chat.Id, $"Not implemented right now :<");
                }
            }
        }

        public TelegramUser FindUser(Message message)
        {
            var user = _users.FirstOrDefault(z =>
                    z.Username == message.From.Username &&
                    z.ChatId == message.Chat.Id);
            if (user != null) return user;

            user = new TelegramUser()
            {
                Username = message.From.Username,
                ChatId = message.Chat.Id,
                BeginTime = new TimeSpan(0, 0, 0),
                EndTime = new TimeSpan(24, 0, 0),
                LastRecievedNews = new Hashtable(),
                Subreddits = new List<string>()
            };
            _users.Add(user);
            return user;
        }

        public void NotifyUsers(List<RedditTopic> topics)
        {
            foreach (var redditTopic in topics)
            {
                Console.WriteLine($"{redditTopic.Subreddit} updated ({redditTopic.Name})");
                var users = _users.Where(
                        u => u.Subreddits.Contains(redditTopic.Subreddit, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                users.ForEach(u => SendMessage(u.ChatId, redditTopic.MakeMessage()));
            }
        }

        public static void SendMessage(long chatId, string message)
        {
            _client.SendTextMessageAsync(chatId, message, parseMode: ParseMode.Html);
        }
    }
}
