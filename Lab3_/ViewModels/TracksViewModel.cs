using System.Collections.Generic;
using Lab3_.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab3_.ViewModels
{
    public class TracksViewModel
    {
        public IEnumerable<Track> Tracks { get; set; }
        public TrackViewModel TrackViewModel { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public SelectList ListYears { get; set; }
    }
}