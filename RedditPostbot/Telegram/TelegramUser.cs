﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditPostbot.Models
{
    public class TelegramUser : UserBase
    {
        public string Username;
        public long ChatId;
    }
}
