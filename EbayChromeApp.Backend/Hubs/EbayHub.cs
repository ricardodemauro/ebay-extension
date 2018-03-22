using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EbayChromeApp.Backend.Messages;
using EbayChromeApp.Backend.Models;
using EbayChromeApp.Backend.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace EbayChromeApp.Backend.Hubs
{
    public class EbayHub : Hub<MessageOperation, Message>
    {
        private readonly int _maxThreads = 10;

        public EbayHub(HttpContext context, WebSocket webSocket) : base(context, webSocket)
        {
        }

        public async override Task<Message> OnGetMessage(MessageOperation message, CancellationToken cancellationToken)
        {
            if (message.Operation == "slug")
            {
                SlugCollection slugCollection = await EbayServices.GetSlugsAsync(message.Data);
                return new Message<SlugCollection> { Data = slugCollection };
            }
            else if (message.Operation == "product")
            {
                var result = await EbayServices.GetProductAsync(message.Data);
                return new Message<Product> { Data = result };
            }
            else if (message.Operation == "complete")
            {
                SlugCollection slugCollection = await EbayServices.GetSlugsAsync(message.Data);
                string[] slugArray = slugCollection.Distinct().ToArray();
                BlockingCollection<Product> productCollection = new BlockingCollection<Product>(slugArray.Length);

                for (int i = 0; i < slugArray.Length; i++)
                {
                    productCollection.Add(await EbayServices.GetProductAsync(slugArray[i]));
                }

                return new Message<List<Product>> { Data = productCollection.ToList() };
            }

            return await base.OnGetMessage(message, cancellationToken);
        }

        static Task<Product> ProcessProduct(string keyword)
        {
            return EbayServices.GetProductAsync(keyword);
        }
    }
}
