using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbayChromeApp.Backend.Models;
using EbayChromeApp.Backend.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EbayChromeApp.Backend.Services
{
    public class CachedEbayService : InternetEbayService, IEbayService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly EnvorinmentOptions _envOptions;

        public CachedEbayService(IOptions<EbayServiceOptions> ebayOptions, IOptions<EnvorinmentOptions> envOptions, IMemoryCache memoryCache, ILogger<InternetEbayService> logger)
            : base(ebayOptions, logger)
        {
            _memoryCache = memoryCache;
            _envOptions = envOptions.Value;
        }

        public async override Task<Product> GetProductAsync(string keyword, int retryTime = 0)
        {
            string key = GetCachedKey(keyword);

            var product = await _memoryCache.GetOrCreateAsync(key, (entry) =>
            {
                if (entry != null)
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_envOptions.MaxMinutesInCache);
                    entry.Priority = CacheItemPriority.Low;
                }
                return base.GetProductAsync(keyword, retryTime);
            });

            return product;
        }

        public async override Task<SlugCollection> GetSlugsAsync(string keyword)
        {
            string key = GetCachedKey(keyword);

            var slugCollection = await _memoryCache.GetOrCreateAsync(key, (entry) =>
            {
                if (entry != null)
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_envOptions.MaxMinutesInCache);
                    entry.Priority = CacheItemPriority.Normal;
                }
                return base.GetSlugsAsync(keyword);
            });

            return slugCollection;
        }

        internal static string GetCachedKey(string keyword)
        {
            var words = keyword.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join("", words);
        }
    }
}
