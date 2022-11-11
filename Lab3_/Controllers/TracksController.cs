using System;
using System.Linq;
using Lab3_.Data;
using Lab3_.Infrastructure;
using Lab3_.Infrastructure.Filters;
using Lab3_.Models;
using Lab3_.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab3_.Controllers
{
    [TypeFilter(typeof(TimingLogAttribute))] // Фильтр ресурсов
    [ExceptionFilter] // Фильтр исключений
    public class TracksController : Controller
    {
        private readonly RadiostationContext _context;
        private readonly int pageSize = 10; // количество элементов на странице
        private TrackViewModel _operation = new() { Genre = "" };

        public TracksController(RadiostationContext context)
        {
            _context = context;
        }

        // GET: Operations
        [SetToSession("SortState")] //Фильтр действий для сохранение в сессию состояния сортировки
        public IActionResult Index(SortState sortOrder, int page = 1)
        {
            // Считывание данных из сессии
            var sessionOperation = HttpContext.Session.Get("Car");
            var sessionSortState = HttpContext.Session.Get("SortState");
            if (sessionOperation != null)
                _operation = Transformations.DictionaryToObject<TrackViewModel>(sessionOperation);
            if (sessionSortState != null)
                if ((sessionSortState.Count > 0) & (sortOrder == SortState.No))
                    sortOrder = (SortState)Enum.Parse(typeof(SortState), sessionSortState["sortOrder"]);

            // Сортировка и фильтрация данных
            IQueryable<Track> context = _context.Tracks
                .Include(t => t.Performer)
                .Include(t => t.Genre);
            context = Sort_Search(context, sortOrder, _operation.Genre ?? "");

            // Разбиение на страницы
            var count = context.Count();
            context = context.Skip((page - 1) * pageSize).Take(pageSize);

            // Формирование модели для передачи представлению
            _operation.SortViewModel = new SortViewModel(sortOrder);
            var cars = new TracksViewModel
            {
                Tracks = context,
                PageViewModel = new PageViewModel(count, page, pageSize),
                TrackViewModel = _operation
            };
            return View(cars);
        }

        // Post: Operations
        [HttpPost]
        [SetToSession("Track")] //Фильтр действий для сохранение в сессию параметров отбора
        public IActionResult Index(TrackViewModel operation, int page = 1)
        {
            // Считывание данных из сессии
            var sessionSortState = HttpContext.Session.Get("SortState");
            var sortOrder = new SortState();
            if (sessionSortState.Count > 0)
                sortOrder = (SortState)Enum.Parse(typeof(SortState), sessionSortState["sortOrder"]);

            // Сортировка и фильтрация данных
            IQueryable<Track> context = _context.Tracks
                .Include(t => t.Performer)
                .Include(t => t.Genre);
            context = Sort_Search(context, sortOrder, operation.Genre ?? "");
            // Разбиение на страницы
            var count = context.Count();
            context = context.Skip((page - 1) * pageSize).Take(pageSize);
            // Формирование модели для передачи представлению
            operation.SortViewModel = new SortViewModel(sortOrder);
            var tracks = new TracksViewModel
            {
                Tracks = context,
                PageViewModel = new PageViewModel(count, page, pageSize),
                TrackViewModel = operation
            };

            return View(tracks);
        }

        // Сортировка и фильтрация данных
        private IQueryable<Track> Sort_Search(IQueryable<Track> operations, SortState sortOrder, string markSearchTankType)
        {
            switch (sortOrder)
            {
                case SortState.RatingAsc:
                    operations = operations.OrderBy(s => s.Rating);
                    break;
                case SortState.RatingDesc:
                    operations = operations.OrderByDescending(s => s.Rating);
                    break;
                case SortState.CreationAsc:
                    operations = operations.OrderBy(s => s.CreationDate);
                    break;
                case SortState.CreationDesc:
                    operations = operations.OrderByDescending(s => s.CreationDate);
                    break;
            }

            operations = operations
                .Include(t => t.Genre)
                .Include(t => t.Performer)
                .Where(t => t.Genre.Name.ToLower().Contains(markSearchTankType.ToLower()))
                .AsNoTracking();

            return operations;
        }
    }
}