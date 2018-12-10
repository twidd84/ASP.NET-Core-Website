using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CornerStoneApril30.Data;
using CornerStoneApril30.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace CornerStoneApril30.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index(string searchStudent)
        {
            var applicationDbContext = _context.Students
               .Include(s => s.School)
               .Include(s => s.TripAssignments)
                   .ThenInclude(s => s.Trip)
                       .ThenInclude(s => s.Bus)
                       .AsNoTracking();
            var studentSchool = _context.Students
                .Include(g => g.School).OrderBy(g => g.School.Name).ThenBy(g => g.Fullname);
            ViewBag.StudentSchool = new SelectList(studentSchool, "Fullname", "Fullname", null, "School.Name");
            if (!String.IsNullOrEmpty(searchStudent))
            {
                applicationDbContext = applicationDbContext.Where(s => s.Fullname.ToUpper().Contains(searchStudent.ToUpper()));
            }
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.School)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["SchoolID"] = new SelectList(_context.School.OrderBy(s => s.Name), "ID", "Name");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,LastName,FirstMidName,Age,Monday,Tuesday,Wednesday,Thursday,Friday,SchoolID")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SchoolID"] = new SelectList(_context.School.OrderBy(s => s.Name), "ID", "Name", student.SchoolID);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.SingleOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["SchoolID"] = new SelectList(_context.School.OrderBy(s => s.Name), "ID", "Name", student.SchoolID);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LastName,FirstMidName,Age,Monday,Tuesday,Wednesday,Thursday,Friday,SchoolID")] Student student)
        {
            if (id != student.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.ID))
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
            ViewData["SchoolID"] = new SelectList(_context.School.OrderBy(s => s.Name), "ID", "Name", student.SchoolID);
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.School)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.SingleOrDefaultAsync(m => m.ID == id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Post Delete Batch
        [HttpPost, ActionName("DeleteBatch")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBatch(IFormCollection formCollection)
        {
            string[] ids = formCollection["ID"].ToString().Split(new char[] { ',' });
            foreach(string id in ids)
            {
                var students = await _context.Students
                    .Include(s => s.TripAssignments)
                    .SingleOrDefaultAsync(m => m.ID == int.Parse(id));
                _context.Students.Remove(students);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.ID == id);
        }
    }
}
