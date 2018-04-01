﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbayChromeApp.Backend.Options
{
    public class EbayServiceOptions
    {
        public string AppName { get; set; }

        public string SlugUri { get; set; }

        public string FindUri { get; set; }

        public int MaxRetry { get; set; }

        public string[] Letters { get; set; }
    }
}
