using Microsoft.EntityFrameworkCore;
using EbayChromeApp.Backend.Data;
using EbayChromeApp.Backend.Messages;
using EbayChromeApp.Backend.Models;
using EbayChromeApp.Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using EbayChromeApp.Backend.Options;
using Microsoft.Extensions.Options;

namespace EbayChromeApp.Backend.Hubs
{
    public class EbayHub : Hub<MessageOperation, Message>
    {
        private readonly IEbayService _ebayService;
        private readonly EnvorinmentOptions configuration;

        public EbayHub(HttpContext context, WebSocket webSocket, IEbayService ebayService, IOptions<EnvorinmentOptions> options) : base(context, webSocket)
        {
            _ebayService = ebayService;
            configuration = options.Value;
        }

        public async override Task<Message> OnGetMessage(MessageOperation message, CancellationToken cancellationToken)
        {
            Message outMessage = default(Message);

            int countCalls = await CheckDailyLimit(message);
            if (countCalls >= configuration.MaxCallTimes)
            {
                return new MaxLimitMessage(countCalls);
            }
            else if (message.Operation == "slug")
            {
                SlugCollection slugCollection = await _ebayService.GetSlugsAsync(message.Data);
                outMessage = new Message<SlugCollection> { Data = slugCollection };
            }
            else if (message.Operation == "product")
            {
                var result = await _ebayService.GetProductAsync(message.Data);
                outMessage = new Message<Product> { Data = result };
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

                outMessage = new Message<Product[]> { Data = orderedProductCollection };
            }

            await AddSearchToLog(message);

            if (outMessage != null)
            {
                return outMessage;
            }
            return await base.OnGetMessage(message, cancellationToken);
        }

        private async Task AddSearchToLog(MessageOperation message)
        {
            var db = _context.RequestServices.GetService<AppDbContext>();

            string ipAddress = message.IPAddress;

            db.Search.Add(new Search
            {
                Created = DateTime.UtcNow,
                IP = message.IPAddress,
            });

            await db.SaveChangesAsync();
        }

        private async Task<int> CheckDailyLimit(MessageOperation message)
        {
            var db = _context.RequestServices.GetService<AppDbContext>();

            string ipAddress = message.IPAddress;
            DateTime minDate = DateTime.UtcNow.AddDays(-1);
            DateTime maxDate = DateTime.UtcNow;

            var count = await db.Search
                .Where(c => c.IP == ipAddress && c.Created > minDate)
                .CountAsync();

            return count;
        }
    }
}
