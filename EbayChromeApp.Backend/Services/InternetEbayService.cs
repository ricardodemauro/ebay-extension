using EbayChromeApp.Backend.Models;
using EbayChromeApp.Backend.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EbayChromeApp.Backend.Services
{
    public class InternetEbayService : IEbayService
    {
        private readonly Uri _uriSlug;
        private readonly Uri _uriFind;

        private readonly string[] _letters;

        private readonly int _maxRetries;

        private readonly ILogger<InternetEbayService> _logger;

        public InternetEbayService(IOptions<EbayServiceOptions> ebayOptions, ILogger<InternetEbayService> logger)
        {
            var options = ebayOptions.Value;
            _uriFind = new Uri(options.FindUri);
            _uriSlug = new Uri(options.SlugUri);
            _maxRetries = options.MaxRetry;
            _letters = options.Letters;
            _logger = logger;
        }

        public virtual async Task<SlugCollection> GetSlugsAsync(string keyword)
        {
            SlugCollection slugCollection = new SlugCollection();

            HttpClient client = new HttpClient
            {
                BaseAddress = _uriSlug
            };
            foreach (string letter in _letters)
            {
                string query = $"{keyword} {letter}";
                try
                {
                    string content = await client.GetStringAsync($"?kwd={query}&_jgr=1&sId=0&_ch=0&callback=nil");

                    if (Regex.IsMatch(content, @"/\*\*/nil\("))
                    {
                        content = content.Replace("/**/nil(", "").Replace("}})", "}}");
                        JObject jObject = JObject.Parse(content);
                        slugCollection.AddRange(jObject["res"]["sug"].Select(p => p.ToString()).ToList());
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error trying to get slug word {query}. Exception {ex.Message}");
                }
            }

            return slugCollection;
        }

        public virtual async Task<Product> GetProductAsync(string keyword, int retryTime = 0)
        {
            if (retryTime < _maxRetries)
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = _uriFind
                };
                string pathUri = $"?SECURITY-APPNAME=RicardoM-sampleke-PRD-6f1a91299-0d0b7d55&OPERATION-NAME=findItemsByKeywords&SERVICE-VERSION=1.0.0&RESPONSE-DATA-FORMAT=JSON&REST-PAYLOAD&keywords={keyword}&paginationInput.entriesPerPage=2&GLOBAL-ID=EBAY-US&siteid=0";
                try
                {
                    string content = await client.GetStringAsync(pathUri);

                    JObject jObject = JObject.Parse(content);
                    string data = jObject["findItemsByKeywordsResponse"].First["paginationOutput"].First["totalEntries"].First.ToString();
                    return new Product { Name = keyword, TotalEntries = data };
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error trying to get product {keyword}. Exception {ex.Message}");
                    return await GetProductAsync(keyword, retryTime++);
                }

            }
            return new Product { Name = keyword, TotalEntries = string.Empty };
        }
    }
}
