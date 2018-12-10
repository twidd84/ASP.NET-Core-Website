using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CornerStoneApril30.Data;
using CornerStoneApril30.Models;
using CornerStoneApril30.Models.ScheduleViewModels;
using Microsoft.AspNetCore.Authorization;

namespace CornerStoneApril30.Controllers
{
    [Authorize]
    public class TripAssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TripAssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TripAssignments
        public async Task<IActionResult> Index()
        {
            var viewModel = new TripIndexData();
            viewModel.TripAssignments = await _context.TripAssignments
                .Include(t => t.Student)
                    .ThenInclude(t => t.School)
                .Include(t => t.Trip)
                    .ThenInclude(t => t.Driver)
                        .ThenInclude(t => t.User)
                        .AsNoTracking()
                        .ToListAsync();
            return View(viewModel);
        }

        // GET: TripAssignments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tripAssignment = await _context.TripAssignments
                .Include(t => t.Student)
                .Include(t => t.Trip)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (tripAssignment == null)
            {
                return NotFound();
            }

            return View(tripAssignment);
        }

        //**********
        //Turned off for now - created in the Trips view instead
        // GET: TripAssignments/Create
        //[Authorize(Roles = "Admin")]
        //public IActionResult Create()
        //{
        //    ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FirstMidName");
        //    ViewData["TripID"] = new SelectList(_context.Trips, "ID", "WeekdayDay");
        //    return View();
        //}

        //// POST: TripAssignments/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ID,StudentID,TripID,StatusOfPickUp")] TripAssignment tripAssignment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(tripAssignment);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FirstMidName", tripAssignment.StudentID);
        //    ViewData["TripID"] = new SelectList(_context.Trips, "ID", "WeekdayDay", tripAssignment.TripID);
        //    return View(tripAssignment);
        //}

        // GET: TripAssignments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tripAssignment = await _context.TripAssignments.SingleOrDefaultAsync(m => m.ID == id);
            if (tripAssignment == null)
            {
                return NotFound();
            }
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FirstMidName", tripAssignment.StudentID);
            ViewData["TripID"] = new SelectList(_context.Trips, "ID", "WeekdayDay", tripAssignment.TripID);
            return View(tripAssignment);
        }

        // POST: TripAssignments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,StudentID,TripID,StatusOfPickUp,LoggedPickupTime")] TripAssignment tripAssignment)
        {
            if (id != tripAssignment.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tripAssignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TripAssignmentExists(tripAssignment.ID))
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
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FirstMidName", tripAssignment.StudentID);
            ViewData["TripID"] = new SelectList(_context.Trips, "ID", "WeekdayDay", tripAssignment.TripID);
            return View(tripAssignment);
        }

        // GET: TripAssignments/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tripAssignment = await _context.TripAssignments
                .Include(t => t.Student)
                .Include(t => t.Trip)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (tripAssignment == null)
            {
                return NotFound();
            }

            return View(tripAssignment);
        }

        // POST: TripAssignments/Delete/
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tripAssignment = await _context.TripAssignments.SingleOrDefaultAsync(m => m.ID == id);
            _context.TripAssignments.Remove(tripAssignment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TripAssignmentExists(int id)
        {
            return _context.TripAssignments.Any(e => e.ID == id);
        }
    }
}
