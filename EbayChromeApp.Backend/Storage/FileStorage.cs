using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbayChromeApp.Backend.Models;
using EbayChromeApp.Backend.Options;
using Newtonsoft.Json;

namespace EbayChromeApp.Backend.Storage
{
    public class FileStorage : IStorage
    {
        private readonly EnvorinmentOptions _envorinmentOptions;

        public FileStorage(EnvorinmentOptions envorinmentOptions)
        {
            _envorinmentOptions = envorinmentOptions;
        }

        protected string GetFullPath(string key)
        {
            return Path.Combine(_envorinmentOptions.AppDataDirectory, $"{key}.data");
        }

        protected bool Contains(string key)
        {
            string path = GetFullPath(key);
            return File.Exists(path);
        }

        public bool ContainsProduct(string keyword)
        {
            return Contains($"prd_{keyword}");
        }

        public bool ContainsSlug(string keyword)
        {
            return Contains($"slug_{keyword}");
        }

        public Task<Product> GetProduct(string keyword)
        {
            return GetData<Product>(keyword);
        }

        public Task<SlugCollection> GetSlug(string keyword)
        {
            return GetData<SlugCollection>(keyword);
        }

        protected async Task<T> GetData<T>(string key)
        {
            if (!Contains(key))
            {
                return default(T);
            }
            string path = GetFullPath(key);
            string content = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<T>(key);
        }

        public Task SetProduct(string keyword, Product product)
        {
            return Set($"prd_{keyword}", product);
        }

        public Task SetSlug(string keyword, SlugCollection slugs)
        {
            return Set($"slug_{keyword}", slugs);
        }

        protected async Task Set<T>(string key, T data)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            string path = GetFullPath(key);
            if (Contains(key))
            {
                File.Delete(path);
            }
            string content = JsonConvert.SerializeObject(data);
            await File.WriteAllTextAsync(path, content);
        }
    }
}
