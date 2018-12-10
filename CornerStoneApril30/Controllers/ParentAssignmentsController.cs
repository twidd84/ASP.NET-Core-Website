using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CornerStoneApril30.Data;
using CornerStoneApril30.Models;

namespace CornerStoneApril30.Controllers
{
    public class ParentAssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParentAssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ParentAssignments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ParentAssignments.Include(p => p.Parent).Include(p => p.Student)
                .ThenInclude(p => p.School)
                .AsNoTracking();
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ParentAssignments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parentAssignment = await _context.ParentAssignments
                .Include(p => p.Parent)
                .Include(p => p.Student)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (parentAssignment == null)
            {
                return NotFound();
            }

            return View(parentAssignment);
        }

        // GET: ParentAssignments/Create
        public IActionResult Create()
        {
            ViewData["ParentID"] = new SelectList(_context.Parents.OrderBy(s => s.LastName), "ParentID", "Fullname");
            ViewData["StudentID"] = new SelectList(_context.Students.OrderBy(s => s.LastName), "ID", "Fullname");
            return View();
        }

        // POST: ParentAssignments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ParentID,StudentID")] ParentAssignment parentAssignment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(parentAssignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentID"] = new SelectList(_context.Parents, "ParentID", "Fullname", parentAssignment.ParentID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "Fullname", parentAssignment.StudentID);
            return View(parentAssignment);
        }

        // GET: ParentAssignments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parentAssignment = await _context.ParentAssignments.SingleOrDefaultAsync(m => m.ID == id);
            if (parentAssignment == null)
            {
                return NotFound();
            }
            ViewData["ParentID"] = new SelectList(_context.Parents.OrderBy(s => s.LastName), "ParentID", "Fullname", parentAssignment.ParentID);
            ViewData["StudentID"] = new SelectList(_context.Students.OrderBy(s => s.LastName), "ID", "Fullname", parentAssignment.StudentID);
            return View(parentAssignment);
        }

        // POST: ParentAssignments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ParentID,StudentID")] ParentAssignment parentAssignment)
        {
            if (id != parentAssignment.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(parentAssignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParentAssignmentExists(parentAssignment.ID))
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
            ViewData["ParentID"] = new SelectList(_context.Parents, "ParentID", "Fullname", parentAssignment.ParentID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "Fullname", parentAssignment.StudentID);
            return View(parentAssignment);
        }

        // GET: ParentAssignments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parentAssignment = await _context.ParentAssignments
                .Include(p => p.Parent)
                .Include(p => p.Student)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (parentAssignment == null)
            {
                return NotFound();
            }

            return View(parentAssignment);
        }

        // POST: ParentAssignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var parentAssignment = await _context.ParentAssignments.SingleOrDefaultAsync(m => m.ID == id);
            _context.ParentAssignments.Remove(parentAssignment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ParentAssignmentExists(int id)
        {
            return _context.ParentAssignments.Any(e => e.ID == id);
        }
    }
}
