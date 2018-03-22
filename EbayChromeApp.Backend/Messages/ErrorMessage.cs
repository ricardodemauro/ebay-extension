using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbayChromeApp.Backend.Messages
{
    public class ErrorMessage : Message<string>
    {
        public ErrorMessage(Exception ex)
        {
            this.Data = ex.Message;
        }
    }
}
