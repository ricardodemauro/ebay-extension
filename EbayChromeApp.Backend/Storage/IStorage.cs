using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbayChromeApp.Backend.Models;

namespace EbayChromeApp.Backend.Storage
{
    public interface IStorage
    {
        bool ContainsProduct(string keyword);
        Task<Product> GetProduct(string keyword);
        Task SetProduct(string keyword, Product product);
        bool ContainsSlug(string keyword);
        Task<SlugCollection> GetSlug(string keyword);
        Task SetSlug(string keyword, SlugCollection slugs);
    }
}
