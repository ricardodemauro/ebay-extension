using Microsoft.EntityFrameworkCore;
using EbayChromeApp.Backend.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using EbayChromeApp.Backend.Infrastructure;

namespace EbayChromeApp.Backend.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IMemoryCache _memoryCache;

        public HomeController(AppDbContext db, IMemoryCache memoryCache)
        {
            _db = db;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Content("It's working");
        }

        [HttpDelete("cache/{id}")]
        public IActionResult CacheDelete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            _memoryCache.Remove(id);
            return Ok();
        }

        [HttpGet("cache/{id}")]
        public IActionResult Cache(string id)
        {
            bool containsKey = _memoryCache.TryGetValue(id, out object @value);
            if (containsKey)
            {
                return Ok(value);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("requests")]
        public async Task<IActionResult> LastRequests(int top = 1000)
        {
            var searchCollection = await _db.Search
                .Take(top)
                .ToListAsync();

            return Ok(searchCollection);
        }

        [HttpGet("log/{ip}")]
        public async Task<IActionResult> Log(string ip)
        {
            var searchCollection = await _db.Search
                .Where(c => c.IP == ip)
                .Take(100)
                .ToListAsync();

            return Ok(searchCollection);
        }

        [HttpDelete("log/{ip}/{last}")]
        public async Task<IActionResult> LogDelete(string ip, int last)
        {
            var searchCollection = await _db.Search
                .Where(c => c.IP == ip)
                .Take(last)
                .ToListAsync();

            _db.Search.RemoveRange(searchCollection);
            await _db.SaveChangesAsync();

            return Ok(searchCollection);
        }
    }
}
