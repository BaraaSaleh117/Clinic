using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Clinic.Data;
using Clinic.Models;
using PagedList;
using PagedList.Mvc;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace Clinic.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActionResult Country { get; private set; }

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Doctors
        public async Task<IActionResult> Index(string sortOrder, int ? Page)
        {

            ViewData["NameSortPram"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : " ";
            var Doctor = from  s in _context.Doctor
                         select s;
            switch (sortOrder)
            {
                case "name_desc":
                    Doctor = Doctor.OrderByDescending(s => s.FirstName);
                    break;
                default:
                    Doctor = Doctor.OrderBy(s => s.FirstName);
                    break;
            }
            

           
            return View( Doctor.ToList().ToPagedList(Page ?? 1,3)); 

        }

        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctor
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // GET: Doctors/Create
        public async Task<IActionResult> CreateAsync()
        {
            ViewData["CountryModel"] = new SelectList(await this.GetCountry(), "name", "name");
            ViewData["SpecializationId"] = new SelectList(_context.Specialization, "Id", "SpecializationName");
            return View();
        }

        // POST: Doctors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Address,Notes,MonthlySalary,PhoneNumber,IBAN,Email,SpecializationId,Country")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doctor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryModel"] = new SelectList(await this.GetCountry(), "name", "name");
            ViewData["SpecializationId"] = new SelectList(_context.Specialization, "Id", "SpecializationName", doctor.SpecializationId);
            return View(doctor);
        }

        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctor.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewData["CountryModel"] = new SelectList( await this.GetCountry(), "name", "name");
            ViewData["SpecializationId"] = new SelectList(_context.Specialization, "Id", "SpecializationName", doctor.SpecializationId);
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FirstName,LastName,Address,Notes,MonthlySalary,PhoneNumber,IBAN,Email,SpecializationId,Country")] Doctor doctor)
        {
            if (id != doctor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryModel"] = new SelectList( await this.GetCountry(), "name", "name");
            ViewData["SpecializationId"] = new SelectList(_context.Specialization, "Id", "SpecializationName", doctor.SpecializationId);
            return View(doctor);
        }

        // GET: Doctors/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctor
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var doctor = await _context.Doctor.FindAsync(id);
            _context.Doctor.Remove(doctor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(long id)
        {
            return _context.Doctor.Any(e => e.Id == id);
        }
        public IActionResult search()
        {
            return View();
        }

        [HttpPost]
        public  IActionResult search(String SearchName)
        {
            var result = _context.Doctor.Where(a => a.FirstName.Contains(SearchName.Trim()) || (a.LastName.Contains(SearchName.Trim()))).ToList();

            return View(result);
        }
        public async Task< IEnumerable<CountryModel>> GetCountry()
        {
            
                string url = "https://restcountries.eu/rest/v1/all";
            List<CountryModel> Country = new List<CountryModel>();

                // Web Request with the given url.
                WebRequest request = WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;

                WebResponse response = request.GetResponse();

                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string jsonResponse = null;

                // Store the json response into jsonResponse variable.
                jsonResponse = reader.ReadLine();

                if (jsonResponse != null)
                {
                    // Deserialize the jsonRespose object to the CountryModel. You're getting a JSON array [].
                    List<CountryModel> countryModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CountryModel>>(jsonResponse);

                    // Set the List Item with the countries.
                    IEnumerable<SelectListItem> countries = countryModel.Select(x => new SelectListItem() { Value = x.name, Text = x.name });

                    // Create a ViewBag property with the final content.
                    ViewBag.Countries = countries;
                }
                return Country;
            }
        }



    }

