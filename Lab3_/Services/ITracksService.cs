using System.Collections.Generic;
using Lab3_.ViewModels;

namespace Lab3_.Services
{
    public interface ITracksService
    {
        HomeViewModel GetHomeViewModel(string cacheKey);
        List<string> GetGenres();

        List<TrackViewModel> SearchTracks(string performer, string genre);
    }
}