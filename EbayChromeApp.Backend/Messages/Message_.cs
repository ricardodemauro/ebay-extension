﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbayChromeApp.Backend.Messages
{
    public class Message<T> : Message
    {
        public T Data { get; set; }
    }
}
