using EbayChromeApp.Backend.Models;
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
    public static class EbayServices
    {
        static Uri URI_SLUG = new Uri("https://autosug.ebay.com/autosug");
        static Uri URI_FIND = new Uri("https://svcs.ebay.com/services/search/FindingService/v1");

        static char[] LETTERS = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'w', 'x', 'y', 'z' };

        static int MAX_RETRY = 3;

        public static async Task<SlugCollection> GetSlugsAsync(string keyword)
        {
            SlugCollection slugCollection = new SlugCollection();

            HttpClient client = new HttpClient();
            client.BaseAddress = URI_SLUG;
            foreach (char letter in LETTERS)
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
                    Trace.TraceError($"Error trying to get slug word {query}. Exception {ex.Message}");
                }
            }

            return slugCollection;
        }

        public static async Task<Product> GetProductAsync(string keyword, int retryTime = 0)
        {
            if (retryTime < MAX_RETRY)
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = URI_FIND;
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
                    Trace.TraceError($"Error trying to get product {keyword}. Exception {ex.Message}");
                    return await GetProductAsync(keyword, retryTime++);
                }

            }
            return new Product { Name = keyword, TotalEntries = string.Empty };
        }
    }
}
