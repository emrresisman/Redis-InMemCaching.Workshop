using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Redis.Workshop.Extensions;
using Redis.Workshop.Models;
using System.Text;

namespace Redis.Workshop.Controllers
{
    public class ProductController : Controller
    {
        private IDistributedCache _distributedCache;
        public ProductController(IDistributedCache distributedCache )
        {
            _distributedCache = distributedCache;
        }
        public IActionResult Index()
        {
            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions();
            cacheEntryOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(1);
            _distributedCache.Set("times",Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString()),cacheEntryOptions);
            return View();
        }
        public IActionResult GetTimes()
        {
            ViewBag.Time= Encoding.UTF8.GetString(_distributedCache.Get("times"));
            return View();
        }
        public IActionResult Remove()
        {
            _distributedCache.Remove("times");

            return View();
        }
        public async Task<IActionResult> ComplexTypeCaching()
        {
            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions();
            cacheEntryOptions.AbsoluteExpiration = DateTime.Now.AddSeconds(5);
            Product product = new Product { Id=1,Name="Pencil",Price=100, CreatedDate = DateTime.Now};
            
            string jsonproduct=JsonConvert.SerializeObject(product);
            //Byte[] byteproduct = Encoding.UTF8.GetBytes(jsonproduct);
            await _distributedCache.SetStringAsync("product:1", jsonproduct, cacheEntryOptions);
            //_distributedCache.Set("product:1",byteproduct);
            return View();
        }
        public async Task<IActionResult> ShowCachedComplexType()
        {
            var product = _distributedCache.GetOrCreate("product:1", () => {
                Product product2 = new Product { Id = 1, Name = "Pencil", Price = 100, CreatedDate = DateTime.Now };

                return product2;
            });
            //Product product =  JsonConvert.DeserializeObject<Product>(jsonproduct);

            ViewBag.Product = product;
            return View();
        }
    }
}
