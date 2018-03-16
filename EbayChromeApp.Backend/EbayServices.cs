using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EbayChromeApp.Backend
{
    public static class EbayServices
    {
        static Uri URI_SLUG = new Uri("https://autosug.ebay.com/autosug");

        public static async Task<List<string>> GetSlugsAsync(string keyword)
        {
            List<string> slugCollection = new List<string>();

            HttpClient client = new HttpClient();
            client.BaseAddress = URI_SLUG;
            string content = await client.GetStringAsync($"?kwd={keyword}&_jgr=1&sId=0&_ch=0&callback=nil");

            //const content = body.replace("/**/nil(","").replace("}})", "}}");
            if (Regex.IsMatch(content, @"/\*\*/nil\("))
            {
                content = content.Replace("/**/nil(", "").Replace("}})", "}}");
                JObject jObject = JObject.Parse(content);
                var data = jObject["res"]["sug"].Select(p => p.ToString()).ToList();
                return data;
            }
            return await Task.FromResult(slugCollection);
        }
    }
}
