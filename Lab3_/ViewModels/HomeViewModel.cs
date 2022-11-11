using System.Collections.Generic;
using Lab3_.Models;

namespace Lab3_.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Performer> Performers { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
        public IEnumerable<TrackViewModel> Tracks { get; set; }
    }
}