using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Clinic.Data;
using Clinic.Models;
using System.Net;
using System.Collections;
using System.IO;

namespace Clinic.Controllers
{
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Patient
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["NameSortPram"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : " ";
            var Patient = from s in _context.Patient
                          select s;
            switch (sortOrder)
            {
                case "name_desc":
                    Patient = Patient.OrderByDescending(s => s.FirstName);
                    break;
                default:
                    Patient = Patient.OrderBy(s => s.FirstName);
                    break;
            }


            return View(await Patient.AsNoTracking().ToListAsync());
        }


        // GET: Patient/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patient/Create

        public async Task<IActionResult> CreateAsync()
        {
            ViewData["CountryModel"] = new SelectList(await this.GetCountry(), "name", "name");
            return View();
        }

        // POST: Patient/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Birthday,Gender,PhoneNumber,Email,Address,RegisterationDate,SSN,Country")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryModel"] = new SelectList(await this.GetCountry(), "name", "name");
            return View(patient);
        }

        // GET: Patient/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            ViewData["CountryModel"] = new SelectList(await this.GetCountry(), "name", "name");
            return View(patient);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FirstName,LastName,Birthday,Gender,PhoneNumber,Email,Address,RegisterationDate,SSN,Country")] Patient patient)
        {
            if (id != patient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.Id))
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
            ViewData["CountryModel"] = new SelectList(await this.GetCountry(), "name", "name");
          
            return View(patient);
        }


        // GET: Patient/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var patient = await _context.Patient.FindAsync(id);
            _context.Patient.Remove(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(long id)
        {
            return _context.Patient.Any(e => e.Id == id);
        }

        public IActionResult search()
        {
            return View();
        }

        [HttpPost]
        public IActionResult search(String SearchName)
        {
            var result = _context.Patient.Where(a => a.FirstName.Contains(SearchName.Trim()) || (a.LastName.Contains(SearchName.Trim()))).ToList();

            return View(result);
        }



        public async Task<IEnumerable<CountryModel>> GetCountry()
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
