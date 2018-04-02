using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbayChromeApp.Backend.Models;
using EbayChromeApp.Backend.Options;
using EbayChromeApp.Backend.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EbayChromeApp.Backend.Services
{
    public class FileEbayService : InternetEbayService, IEbayService
    {
        private readonly IStorage _storage;

        public FileEbayService(IStorage storage, IOptions<EbayServiceOptions> ebayOptions, ILogger<InternetEbayService> logger)
            : base(ebayOptions, logger)
        {
            _storage = storage;
        }

        public override async Task<Product> GetProductAsync(string keyword, int retryTime = 0)
        {
            if (string.IsNullOrEmpty(keyword))
                throw new ArgumentNullException(nameof(keyword));

            if (_storage.ContainsProduct(keyword))
            {
                return await _storage.GetProduct(keyword);
            }
            Product product = await base.GetProductAsync(keyword, retryTime);
            await _storage.SetProduct(keyword, product);
            return product;
        }

        public override async Task<SlugCollection> GetSlugsAsync(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                throw new ArgumentNullException(nameof(keyword));

            if (_storage.ContainsSlug(keyword))
            {
                return await _storage.GetSlug(keyword);
            }
            SlugCollection slugs = await base.GetSlugsAsync(keyword);
            await _storage.SetSlug(keyword, slugs);
            return slugs;
        }
    }
}
