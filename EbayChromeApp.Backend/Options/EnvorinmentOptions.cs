using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbayChromeApp.Backend.Options
{
    public class EnvorinmentOptions
    {
        public string RootDirectory { get; set; }

        public string AppDataDirectory { get; set; }

        public int MaxMinutesInCache { get; set; }

        public int MaxCallTimes { get; set; }
    }
}
