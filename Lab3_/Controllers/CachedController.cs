using System.Linq;
using Lab3_.Data;
using Lab3_.Infrastructure.Filters;
using Lab3_.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab3_.Controllers
{
    public class CachedController : Controller
    {
        private readonly RadiostationContext _db;

        public CachedController(RadiostationContext context)
        {
            _db = context;
        }

        [TypeFilter(typeof(CacheResourceFilterAttribute))]
        public IActionResult Index()
        {
            const int numberRows = 10;
            var genres = _db.Genres.AsNoTracking().Take(numberRows).ToList();
            var performers = _db.Performers.AsNoTracking().Take(numberRows).ToList();
            var tracks = _db.Tracks
                .Include(t => t.Genre)
                .Include(t => t.Performer)
                .Select(t => new TrackViewModel
                {
                    Id = t.Id,
                    Duration = t.Duration,
                    CreationDate = t.CreationDate,
                    Genre = t.Genre.Name,
                    Name = t.Name,
                    Performer = t.Performer.Name,
                    Rating = t.Rating
                })
                .Take(numberRows)
                .ToList();

            var homeViewModel = new HomeViewModel
            {
                Performers = performers,
                Genres = genres,
                Tracks = tracks
            };
            return View("~/Views/Home/Index.cshtml", homeViewModel);
        }
    }
}