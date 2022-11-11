using System.Threading.Tasks;
using Lab3_.Data;
using Lab3_.Infrastructure.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab3_.Controllers
{
    [TypeFilter(typeof(TimingLogAttribute))]
    public class PerformersController : Controller
    {
        private readonly RadiostationContext _context;

        public PerformersController(RadiostationContext context)
        {
            _context = context;
        }

        // GET: Fuels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Performers.AsNoTracking().ToListAsync());
        }
    }
}