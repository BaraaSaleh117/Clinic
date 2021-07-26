using Clinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Clinic.Data;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

namespace Clinic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            //string sortOrder
            //  int page = 1;
            //var query = _context.Doctor.AsNoTracking().OrderBy(s => s.FirstName);
            //  var model = await PagingList.CreateAsync(query,5,page);

            var applicationDbContext = _context.Appointment.Include(a => a.AppointmentType).Include(a => a.Doctor).Include(a => a.Patient).Where(a=> a.Reservation.Date == DateTime.Today.Date).OrderBy(a=>a.Reservation);
           
            return View(applicationDbContext.ToList());

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
