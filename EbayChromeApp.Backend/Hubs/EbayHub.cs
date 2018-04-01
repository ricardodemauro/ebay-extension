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
        private readonly IEbayService _ebayService;

        public EbayHub(HttpContext context, WebSocket webSocket, IEbayService ebayService) : base(context, webSocket)
        {
            _ebayService = ebayService;
        }

        public async override Task<Message> OnGetMessage(MessageOperation message, CancellationToken cancellationToken)
        {
            if (message.Operation == "slug")
            {
                SlugCollection slugCollection = await _ebayService.GetSlugsAsync(message.Data);
                return new Message<SlugCollection> { Data = slugCollection };
            }
            else if (message.Operation == "product")
            {
                var result = await _ebayService.GetProductAsync(message.Data);
                return new Message<Product> { Data = result };
            }
            else if (message.Operation == "complete")
            {
                SlugCollection slugCollection = await _ebayService.GetSlugsAsync(message.Data);
                string[] slugArray = slugCollection.Distinct().ToArray();
                BlockingCollection<Product> productCollection = new BlockingCollection<Product>(slugArray.Length);

                for (int i = 0; i < slugArray.Length; i++)
                {
                    productCollection.Add(await _ebayService.GetProductAsync(slugArray[i]));
                }

                var orderedProductCollection = productCollection.OrderBy(p => p.Name).ToArray();

                return new Message<Product[]> { Data = orderedProductCollection };
            }

            return await base.OnGetMessage(message, cancellationToken);
        }
    }
}
