using System;
using System.Linq;
using Lab3_.Models;

namespace Lab3_.Data
{
    public static class DbInitializer
    {
        public static void Initialize(RadiostationContext db)
        {
            db.Database.EnsureCreated();

            if (db.Performers.Any()) return;

            const int performerNumber = 35;
            const int genreNumber = 35;
            const int trackNumber = 300;

            var performers = Enumerable.Range(1, performerNumber)
                .Select(performerId => new Performer
                {
                    Name = "TestPerformer_" + performerId,
                    IsGroup = false,
                    Description = "TestDescription" + performerId,
                    GroupList = ""
                })
                .ToList();
            db.Performers.AddRange(performers);
            db.SaveChanges();

            var genres = Enumerable.Range(1, genreNumber)
                .Select(genreId => new Genre
                {
                    Description = "TestDescription" + genreId,
                    Name = "TestGenre" + genreId,
                })
                .ToList();
            db.Genres.AddRange(genres);
            db.SaveChanges();

            var tracks = Enumerable.Range(1, trackNumber)
                .Select(trackId => new Track
                {
                    Duration = new Random().Next(1, 10).ToString() + ":" + new Random().Next(1, 10).ToString() + new Random().Next(1, 10).ToString(),
                    CreationDate = DateTime.Now.AddDays(-new Random().Next(0, 10)),
                    GenreId = new Random().Next(1, genreNumber),
                    Name = "TestTrack" + trackId,
                    PerformerId = new Random().Next(1, performerNumber),
                    Rating = new Random().Next(1, 6)
                })
                .ToList();

            db.Tracks.AddRange(tracks);
            db.SaveChanges();
        }
    }
}