using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace EbayChromeApp.Backend
{
    public class AppHub : Hub<MessageOperation, Message>
    {
        public AppHub(HttpContext context, WebSocket webSocket) : base(context, webSocket, AppHub.Get)
        {
        }

        public static async Task<Message> Get(MessageOperation message, CancellationToken cancellationToken)
        {
            var slugCollection = await EbayServices.GetSlugsAsync(message.Data);
            string data = JsonConvert.SerializeObject(slugCollection);
            return new Message { Data = data };
        }
    }
}
