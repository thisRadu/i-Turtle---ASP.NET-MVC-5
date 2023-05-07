using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using i_Turtle.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace i_Turtle.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class PatientsController : Controller
    {
        private readonly TurtleDbContext _context;

        public PatientsController(TurtleDbContext context)
        {
            _context = context;
        }

        // GET: Patients
     
      
        public async Task<IActionResult> Index(string searchString, int page = 1, int patientsPerPage = 10, string sortBy ="")
        {
            
            var patients = from p in _context.Patients
                           select p;
          
            if (!string.IsNullOrEmpty(searchString))
            {
                patients = patients.Where(p => p.Name.Contains(searchString));
            }

            int totalPatients = await patients.CountAsync();

            var viewModel = new PatientPaginationViewModel

            {
                Patients = await patients.Skip((page - 1) * patientsPerPage)
                                          .Take(patientsPerPage)
                                          .ToListAsync(),
                PageIndex = page,
                TotalPatients = totalPatients,
                PatientsPerPage = patientsPerPage,
                TotalPages = totalPatients / patientsPerPage,
                SortBy = sortBy
                
            };
            switch (sortBy)
            {
                case "": break;
                case "risk": viewModel.Patients = patients.OrderBy(p => p.RiscScale); break;
                case "name": viewModel.Patients = patients.OrderBy(p => p.Name); break;
                case "HandlingDate": viewModel.Patients = patients.OrderBy(p => p.HandlingDate); break;
            }

            return View(viewModel);
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ParentId,DoctorId,RiscScale,HandlingDate,BirthDate")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ParentId,DoctorId,RiscScale,HandlingDate,BirthDate")] Patient patient)
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
            return View(patient);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }

       
    }
}
