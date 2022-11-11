using System;
using System.Collections.Generic;
using System.Linq;
using Lab3_.Data;
using Lab3_.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Lab3_.Services
{
    // Класс выборки 10 записей из таблиц 
    public class TracksService : ITracksService
    {
        private readonly RadiostationContext _db;
        private readonly IMemoryCache _cache;

        private const int NumberRows = 20;

        public TracksService(RadiostationContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }

        public HomeViewModel GetHomeViewModel(string cacheKey)
        {
            if (_cache.TryGetValue(cacheKey, out HomeViewModel result))
            {
                return result;
            }

            var genres = _db.Genres.AsNoTracking().Take(NumberRows).ToList();
            var performers = _db.Performers.AsNoTracking().Take(NumberRows).ToList();
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
                .Take(NumberRows)
                .ToList();

            var homeViewModel = new HomeViewModel
            {
                Performers = performers,
                Genres = genres,
                Tracks = cars
            };

            _cache.Set(cacheKey, result,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(2 * 26 + 240)));

            return homeViewModel;
        }

        public List<string> GetGenres()
        {
            var result = _db.Genres
                .Select(x => x.Name)
                .Distinct()
                .ToList();

            return result;
        }

        public List<TrackViewModel> SearchTracks(string performer, string genre)
        {
            performer = performer.ToLower();
            genre = genre.ToLower();

            var tracks = _db.Tracks
                .Include(x => x.Performer)
                .Include(x => x.Genre)
                .Where(x => x.Performer.Name.ToLower().StartsWith(performer) && x.Genre.Name.StartsWith(genre))
                .Select(x => new TrackViewModel
                {
                    Id = x.Id,
                    Duration = x.Duration,
                    CreationDate = x.CreationDate,
                    Genre = x.Genre.Name,
                    Name = x.Name,
                    Performer = x.Performer.Name,
                    Rating = x.Rating
                })
                .ToList();

            return tracks;
        }
    }
}