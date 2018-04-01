using EbayChromeApp.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbayChromeApp.Backend.Services
{
    public interface IEbayService
    {
        Task<SlugCollection> GetSlugsAsync(string keyword);

        Task<Product> GetProductAsync(string keyword, int retryTime = 0);
    }
}
