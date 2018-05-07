using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbayChromeApp.Backend.Messages
{
    public class MaxLimitMessage : Message<string>
    {
        public int Times { get; private set; }

        public MaxLimitMessage(int times = 5)
        {
            Code = 401;
            Data = "Max request limit reached. Try again tomorrow";
            Times = times;
        }
    }
}
