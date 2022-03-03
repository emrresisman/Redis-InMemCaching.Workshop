using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace Redis.Workshop.Controllers
{
    public class FolderController : Controller
    {
        private IDistributedCache _distributedCache;
        public FolderController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ImageCacher()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/images1.jpg");
            byte[] imageBytes = System.IO.File.ReadAllBytes(path);
            _distributedCache.Set("image",imageBytes);
            return View();
        }
        public IActionResult Image()
        {
            byte[] imageBytes=_distributedCache.Get("image");

            return File(imageBytes,"image/jpg");
        }
        
    }
}
