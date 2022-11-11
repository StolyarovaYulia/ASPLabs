using System.Linq;
using Lab3_.Data;
using Lab3_.Infrastructure.Filters;
using Lab3_.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab3_.Controllers
{
    [ExceptionFilter]
    [TypeFilter(typeof(TimingLogAttribute))]
    public class HomeController : Controller
    {
        private readonly RadiostationContext _db;

        public HomeController(RadiostationContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            const int numberRows = 10;
            var genres = _db.Genres.AsNoTracking().Take(numberRows).ToList();
            var performers = _db.Performers.AsNoTracking().Take(numberRows).ToList();
            var cars = _db.Tracks
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
                Tracks = cars
            };

            return View(homeViewModel);
        }
    }
}