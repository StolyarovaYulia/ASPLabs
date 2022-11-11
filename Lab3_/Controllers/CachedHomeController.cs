using Lab3_.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Lab3_.Controllers
{
    // Выборка кэшированых данных из IMemoryCache
    public class CachedHomeController : Controller
    {
        private readonly IMemoryCache _memoryCache;

        public CachedHomeController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            //считывание данных из кэша
            var homeViewModel = _memoryCache.Get<HomeViewModel>("Tracks 10");
            return View("~/Views/Home/Index.cshtml", homeViewModel);
        }

        public IActionResult Info()
        {
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var isHaveUserAgent = HttpContext.Request.Headers.TryGetValue("user-agent", out var userAgent);
            var isHaveReffer = HttpContext.Request.Headers.TryGetValue("reffer", out var reffer);

            var viewModel = new InfoViewModel
            {
                Reffer = reffer,
                IpAddress = remoteIpAddress,
                UserAgent = userAgent
            };

            return View("~/Views/Home/Info.cshtml", viewModel);
        }
    }
}