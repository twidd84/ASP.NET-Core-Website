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
using CornerStoneApril30.Reports;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace CornerStoneApril30.Controllers
{
    public class TripsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TripsController(ApplicationDbContext context)
        {
            _context = context;
        }
        //DateTime now for New Zealand when on other country servers
        DateTime kiwiNow = DateTime.Now;

        // GET: Trips
        public async Task<IActionResult> Index(int? id, string searchStudent, DateTime? TripDate = null)
        {
            var viewModel = new TripIndexData();
            viewModel.Trips = await _context.Trips
                .Include(t => t.Bus)
                .Include(t => t.Driver)
                    .ThenInclude(t => t.User)
                .Include(t => t.TripAssignments)
                    .ThenInclude(t => t.Student)
                        .ThenInclude(t => t.ParentAssignments)
                            .ThenInclude(t => t.Parent)
                .Include(t => t.TripAssignments)
                    .ThenInclude(t => t.Student)
                        .ThenInclude(t => t.School)
                .AsNoTracking()
                .ToListAsync();
            viewModel.TripAssignments = await _context.TripAssignments
                .Include(g => g.Student)
                .Include(g => g.Trip)
                .AsNoTracking()
                .ToListAsync();
            if (id != null)
            {
                ViewData["TripID"] = id.Value;
                Trip trip = viewModel.Trips.Where(
                i => i.ID == id.Value).Single();
                viewModel.Students = trip.TripAssignments.Select(s => s.Student);
            }
            DateTime utcDateTime = kiwiNow.ToUniversalTime();
            string nzTimeZoneKey = "New Zealand Standard Time";
            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById(nzTimeZoneKey);
            DateTime nzDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, nzTimeZone);
            var monCount = _context.Students.Where(b => b.Monday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Monday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var tueCount = _context.Students.Where(b => b.Tuesday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Tuesday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var wedCount = _context.Students.Where(b => b.Wednesday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Wednesday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var thuCount = _context.Students.Where(b => b.Thursday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Thursday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var friCount = _context.Students.Where(b => b.Friday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Friday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            ViewData["MonCount"] = monCount >= 0 && _context.Students.Any(c => c.Monday.Date >= nzDateTime.Date) ? monCount.ToString() : "Past";
            ViewData["TueCount"] = tueCount >= 0 && _context.Students.Any(c => c.Tuesday.Date >= nzDateTime.Date) ? tueCount.ToString() : "Past";
            ViewData["WedCount"] = wedCount >= 0 && _context.Students.Any(c => c.Wednesday.Date >= nzDateTime.Date) ? wedCount.ToString() : "Past";
            ViewData["ThuCount"] = thuCount >= 0 && _context.Students.Any(c => c.Thursday.Date >= nzDateTime.Date) ? thuCount.ToString() : "Past";
            ViewData["FriCount"] = friCount >= 0 && _context.Students.Any(c => c.Friday.Date >= nzDateTime.Date) ? friCount.ToString() : "Past";
            ViewData["Drivers"] = _context.Drivers.Include(d => d.User).ToList().Distinct();
            ViewData["StudentID"] = new SelectList(_context.Students.OrderBy(s => s.LastName), "Fullname", "Fullname");
            //SelectList code below contains Schoolname combined with Student name
            //var studentSchool = _context.Students
            //      .Select(s => new SelectListItem
            //      {
            //          Value = s.Fullname,
            //          Text = s.Fullname,
            //          Group = new SelectListGroup { Name = s.School.Name }           
            //      });
            //ViewBag.StudentSchool = new SelectList(studentSchool, "Value", "Text",4,"Group");
            //SelectList code below contains Schoolname as the option group heading
            var studentSchool = _context.Students
                .Include(g => g.School).OrderBy(g => g.School.Name).ThenBy(g => g.Fullname);
            ViewBag.StudentSchool = new SelectList(studentSchool, "Fullname", "Fullname", null, "School.Name");
            if (!String.IsNullOrEmpty(searchStudent))
            {
                viewModel.Trips = viewModel.Trips.Where(a => a.TripAssignments.Any(t => t.Student.Fullname.ToUpper().Contains(searchStudent.ToUpper())));
                ViewData["SearchResult"] = searchStudent.ToString();
            }
            if (TripDate != null)
            {
                viewModel.Trips = viewModel.Trips.Where(a => a.PickupTime == TripDate);
                ViewData["DateResult"] = TripDate.Value.ToShortDateString();
            }
            //#region School buttons that click with students listed inside
            ////Below is to list the students names in each school into the index view.
            //List<Student> studentNames = new List<Student>();
            //var distinctSchools = _context.School.ToList().Distinct(new SchoolComparer());
            //ViewBag.SchoolNames = distinctSchools;
            //viewModel.Students = await _context.Students.ToListAsync();
            ////ViewData["StudentNames"] = viewModel.Students;
            //foreach (var schoolsDis in distinctSchools)
            //{
            //    ViewBag.SchoolNames = schoolsDis.Name;
            //    //ViewData["StudentCount"] = schoolsDis.Students.Count;
            //    foreach (var s in _context.Students.Where(g => g.School.Name == schoolsDis.Name))
            //    {
            //        studentNames.Add(s);
            //    }
            //}
            //ViewData["StudentNames"] = studentNames;
            //#endregion
            //if (studentID != null)
            //{
            //    ViewData["StudentID"] = studentID.Value;
            //    var selectedStudent = viewModel.Students.Where(x => x.ID == studentID).Single();
            //    await _context.Entry(selectedStudent).Collection(x => x.ParentAssignments).LoadAsync();
            //    foreach (ParentAssignment parentAssignment in selectedStudent.ParentAssignments)
            //    {
            //        await _context.Entry(parentAssignment).Reference(x => x.Parent).LoadAsync();
            //    }
            //    viewModel.ParentAssignments = selectedStudent.ParentAssignments;
            //}
            return View(viewModel);
        }
        // GET: Trips
        public async Task<IActionResult> Monday(int? id, string searchStudent, DateTime? TripDate = null)
        {
            var viewModel = new TripIndexData();
            viewModel.Trips = await _context.Trips
                .Include(t => t.Bus)
                .Include(t => t.Driver)
                    .ThenInclude(t => t.User)
                .Include(t => t.TripAssignments)
                    .ThenInclude(t => t.Student)
                        .ThenInclude(t => t.ParentAssignments)
                            .ThenInclude(t => t.Parent)
                .Include(t => t.TripAssignments)
                    .ThenInclude(t => t.Student)
                        .ThenInclude(t => t.School)
                .AsNoTracking()
                .ToListAsync();
            if (id != null)
            {
                ViewData["TripID"] = id.Value;
                Trip trip = viewModel.Trips.Where(
                i => i.ID == id.Value).Single();
                viewModel.Students = trip.TripAssignments.Select(s => s.Student);
            }
            DateTime utcDateTime = kiwiNow.ToUniversalTime();
            string nzTimeZoneKey = "New Zealand Standard Time";
            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById(nzTimeZoneKey);
            DateTime nzDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, nzTimeZone);
            var monCount = _context.Students.Where(b => b.Monday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Monday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var tueCount = _context.Students.Where(b => b.Tuesday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Tuesday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var wedCount = _context.Students.Where(b => b.Wednesday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Wednesday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var thuCount = _context.Students.Where(b => b.Thursday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Thursday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var friCount = _context.Students.Where(b => b.Friday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Friday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            ViewData["MonCount"] = monCount >= 0 && _context.Students.Any(c => c.Monday.Date >= nzDateTime.Date) ? monCount.ToString() : "Past";
            ViewData["TueCount"] = tueCount >= 0 && _context.Students.Any(c => c.Tuesday.Date >= nzDateTime.Date) ? tueCount.ToString() : "Past";
            ViewData["WedCount"] = wedCount >= 0 && _context.Students.Any(c => c.Wednesday.Date >= nzDateTime.Date) ? wedCount.ToString() : "Past";
            ViewData["ThuCount"] = thuCount >= 0 && _context.Students.Any(c => c.Thursday.Date >= nzDateTime.Date) ? thuCount.ToString() : "Past";
            ViewData["FriCount"] = friCount >= 0 && _context.Students.Any(c => c.Friday.Date >= nzDateTime.Date) ? friCount.ToString() : "Past";

            ViewData["Drivers"] = _context.Drivers.Include(d => d.User).ToList().Distinct();
            ViewData["StudentID"] = new SelectList(_context.Students.OrderBy(s => s.LastName), "Fullname", "Fullname");

            var studentSchool = _context.Students
                .Include(g => g.School).OrderBy(g => g.School.Name).ThenBy(g => g.Fullname);
            ViewBag.StudentSchool = new SelectList(studentSchool, "Fullname", "Fullname", null, "School.Name");
            if (!String.IsNullOrEmpty(searchStudent))
            {
                viewModel.Trips = viewModel.Trips.Where(a => a.TripAssignments.Any(t => t.Student.Fullname.ToUpper().Contains(searchStudent.ToUpper())) && a.PickupTime.DayOfWeek == DayOfWeek.Monday);
                ViewData["SearchResult"] = searchStudent.ToString();
            }
            if (TripDate != null)
            {
                viewModel.Trips = viewModel.Trips.Where(a => a.PickupTime == TripDate);
                ViewData["DateResult"] = TripDate.Value.ToShortDateString();
            }
            return View(viewModel);
        }
        public async Task<IActionResult> Tuesday(int? id, string searchStudent, DateTime? TripDate = null)
        {
            var viewModel = new TripIndexData();
            viewModel.Trips = await _context.Trips
                .Include(t => t.Bus)
                .Include(t => t.Driver)
                    .ThenInclude(t => t.User)
                .Include(t => t.TripAssignments)
                    .ThenInclude(t => t.Student)
                        .ThenInclude(t => t.ParentAssignments)
                            .ThenInclude(t => t.Parent)
                .Include(t => t.TripAssignments)
                    .ThenInclude(t => t.Student)
                        .ThenInclude(t => t.School)
                .AsNoTracking()
                .ToListAsync();
            if (id != null)
            {
                ViewData["TripID"] = id.Value;
                Trip trip = viewModel.Trips.Where(
                i => i.ID == id.Value).Single();
                viewModel.Students = trip.TripAssignments.Select(s => s.Student);
            }
            DateTime utcDateTime = kiwiNow.ToUniversalTime();
            string nzTimeZoneKey = "New Zealand Standard Time";
            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById(nzTimeZoneKey);
            DateTime nzDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, nzTimeZone);
            var monCount = _context.Students.Where(b => b.Monday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Monday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var tueCount = _context.Students.Where(b => b.Tuesday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Tuesday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var wedCount = _context.Students.Where(b => b.Wednesday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Wednesday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var thuCount = _context.Students.Where(b => b.Thursday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Thursday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var friCount = _context.Students.Where(b => b.Friday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Friday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            ViewData["MonCount"] = monCount >= 0 && _context.Students.Any(c => c.Monday.Date >= nzDateTime.Date) ? monCount.ToString() : "Past";
            ViewData["TueCount"] = tueCount >= 0 && _context.Students.Any(c => c.Tuesday.Date >= nzDateTime.Date) ? tueCount.ToString() : "Past";
            ViewData["WedCount"] = wedCount >= 0 && _context.Students.Any(c => c.Wednesday.Date >= nzDateTime.Date) ? wedCount.ToString() : "Past";
            ViewData["ThuCount"] = thuCount >= 0 && _context.Students.Any(c => c.Thursday.Date >= nzDateTime.Date) ? thuCount.ToString() : "Past";
            ViewData["FriCount"] = friCount >= 0 && _context.Students.Any(c => c.Friday.Date >= nzDateTime.Date) ? friCount.ToString() : "Past";

            ViewData["Drivers"] = _context.Drivers.Include(d => d.User).ToList().Distinct();
            ViewData["StudentID"] = new SelectList(_context.Students.OrderBy(s => s.LastName), "Fullname", "Fullname");

            var studentSchool = _context.Students
                .Include(g => g.School).OrderBy(g => g.School.Name).ThenBy(g => g.Fullname);
            ViewBag.StudentSchool = new SelectList(studentSchool, "Fullname", "Fullname", null, "School.Name");
            if (!String.IsNullOrEmpty(searchStudent))
            {
                viewModel.Trips = viewModel.Trips.Where(a => a.TripAssignments.Any(t => t.Student.Fullname.ToUpper().Contains(searchStudent.ToUpper())) && a.PickupTime.DayOfWeek == DayOfWeek.Tuesday);
                ViewData["SearchResult"] = searchStudent.ToString();
            }
            if (TripDate != null)
            {
                viewModel.Trips = viewModel.Trips.Where(a => a.PickupTime == TripDate);
                ViewData["DateResult"] = TripDate.Value.ToShortDateString();
            }
            return View(viewModel);
        }
        public async Task<IActionResult> Wednesday(int? id, string searchStudent, DateTime? TripDate = null)
        {
            var viewModel = new TripIndexData();
            viewModel.Trips = await _context.Trips
                .Include(t => t.Bus)
                .Include(t => t.Driver)
                    .ThenInclude(t => t.User)
                .Include(t => t.TripAssignments)
                    .ThenInclude(t => t.Student)
                        .ThenInclude(t => t.ParentAssignments)
                            .ThenInclude(t => t.Parent)
                .Include(t => t.TripAssignments)
                    .ThenInclude(t => t.Student)
                        .ThenInclude(t => t.School)
                .AsNoTracking()
                .ToListAsync();
            if (id != null)
            {
                ViewData["TripID"] = id.Value;
                Trip trip = viewModel.Trips.Where(
                i => i.ID == id.Value).Single();
                viewModel.Students = trip.TripAssignments.Select(s => s.Student);
            }
            DateTime utcDateTime = kiwiNow.ToUniversalTime();
            string nzTimeZoneKey = "New Zealand Standard Time";
            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById(nzTimeZoneKey);
            DateTime nzDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, nzTimeZone);
            var monCount = _context.Students.Where(b => b.Monday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Monday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var tueCount = _context.Students.Where(b => b.Tuesday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Tuesday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var wedCount = _context.Students.Where(b => b.Wednesday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Wednesday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var thuCount = _context.Students.Where(b => b.Thursday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Thursday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var friCount = _context.Students.Where(b => b.Friday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Friday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            ViewData["MonCount"] = monCount >= 0 && _context.Students.Any(c => c.Monday.Date >= nzDateTime.Date) ? monCount.ToString() : "Past";
            ViewData["TueCount"] = tueCount >= 0 && _context.Students.Any(c => c.Tuesday.Date >= nzDateTime.Date) ? tueCount.ToString() : "Past";
            ViewData["WedCount"] = wedCount >= 0 && _context.Students.Any(c => c.Wednesday.Date >= nzDateTime.Date) ? wedCount.ToString() : "Past";
            ViewData["ThuCount"] = thuCount >= 0 && _context.Students.Any(c => c.Thursday.Date >= nzDateTime.Date) ? thuCount.ToString() : "Past";
            ViewData["FriCount"] = friCount >= 0 && _context.Students.Any(c => c.Friday.Date >= nzDateTime.Date) ? friCount.ToString() : "Past";

            ViewData["Drivers"] = _context.Drivers.Include(d => d.User).ToList().Distinct();
            ViewData["StudentID"] = new SelectList(_context.Students.OrderBy(s => s.LastName), "Fullname", "Fullname");

            var studentSchool = _context.Students
                .Include(g => g.School).OrderBy(g => g.School.Name).ThenBy(g => g.Fullname);
            ViewBag.StudentSchool = new SelectList(studentSchool, "Fullname", "Fullname", null, "School.Name");
            if (!String.IsNullOrEmpty(searchStudent))
            {
                viewModel.Trips = viewModel.Trips.Where(a => a.TripAssignments.Any(t => t.Student.Fullname.ToUpper().Contains(searchStudent.ToUpper())) && a.PickupTime.DayOfWeek == DayOfWeek.Wednesday);
                ViewData["SearchResult"] = searchStudent.ToString();
            }
            if (TripDate != null)
            {
                viewModel.Trips = viewModel.Trips.Where(a => a.PickupTime == TripDate);
                ViewData["DateResult"] = TripDate.Value.ToShortDateString();
            }
            return View(viewModel);
        }
        public async Task<IActionResult> Thursday(int? id, string searchStudent, DateTime? TripDate = null)
        {
            var viewModel = new TripIndexData();
            viewModel.Trips = await _context.Trips
                .Include(t => t.Bus)
                .Include(t => t.Driver)
                    .ThenInclude(t => t.User)
                .Include(t => t.TripAssignments)
                    .ThenInclude(t => t.Student)
                        .ThenInclude(t => t.ParentAssignments)
                            .ThenInclude(t => t.Parent)
                .Include(t => t.TripAssignments)
                    .ThenInclude(t => t.Student)
                        .ThenInclude(t => t.School)
                .AsNoTracking()
                .ToListAsync();
            if (id != null)
            {
                ViewData["TripID"] = id.Value;
                Trip trip = viewModel.Trips.Where(
                i => i.ID == id.Value).Single();
                viewModel.Students = trip.TripAssignments.Select(s => s.Student);
            }
            DateTime utcDateTime = kiwiNow.ToUniversalTime();
            string nzTimeZoneKey = "New Zealand Standard Time";
            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById(nzTimeZoneKey);
            DateTime nzDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, nzTimeZone);
            var monCount = _context.Students.Where(b => b.Monday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Monday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var tueCount = _context.Students.Where(b => b.Tuesday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Tuesday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var wedCount = _context.Students.Where(b => b.Wednesday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Wednesday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var thuCount = _context.Students.Where(b => b.Thursday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Thursday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var friCount = _context.Students.Where(b => b.Friday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Friday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            ViewData["MonCount"] = monCount >= 0 && _context.Students.Any(c => c.Monday.Date >= nzDateTime.Date) ? monCount.ToString() : "Past";
            ViewData["TueCount"] = tueCount >= 0 && _context.Students.Any(c => c.Tuesday.Date >= nzDateTime.Date) ? tueCount.ToString() : "Past";
            ViewData["WedCount"] = wedCount >= 0 && _context.Students.Any(c => c.Wednesday.Date >= nzDateTime.Date) ? wedCount.ToString() : "Past";
            ViewData["ThuCount"] = thuCount >= 0 && _context.Students.Any(c => c.Thursday.Date >= nzDateTime.Date) ? thuCount.ToString() : "Past";
            ViewData["FriCount"] = friCount >= 0 && _context.Students.Any(c => c.Friday.Date >= nzDateTime.Date) ? friCount.ToString() : "Past";

            ViewData["Drivers"] = _context.Drivers.Include(d => d.User).ToList().Distinct();
            ViewData["StudentID"] = new SelectList(_context.Students.OrderBy(s => s.LastName), "Fullname", "Fullname");

            var studentSchool = _context.Students
                .Include(g => g.School).OrderBy(g => g.School.Name).ThenBy(g => g.Fullname);
            ViewBag.StudentSchool = new SelectList(studentSchool, "Fullname", "Fullname", null, "School.Name");
            if (!String.IsNullOrEmpty(searchStudent))
            {
                viewModel.Trips = viewModel.Trips.Where(a => a.TripAssignments.Any(t => t.Student.Fullname.ToUpper().Contains(searchStudent.ToUpper())) && a.PickupTime.DayOfWeek == DayOfWeek.Thursday);
                ViewData["SearchResult"] = searchStudent.ToString();
            }
            if (TripDate != null)
            {
                viewModel.Trips = viewModel.Trips.Where(a => a.PickupTime == TripDate);
                ViewData["DateResult"] = TripDate.Value.ToShortDateString();
            }
            return View(viewModel);
        }
        public async Task<IActionResult> Friday(int? id, string searchStudent, DateTime? TripDate = null)
        {
            var viewModel = new TripIndexData();
            viewModel.Trips = await _context.Trips
                .Include(t => t.Bus)
                .Include(t => t.Driver)
                    .ThenInclude(t => t.User)
                .Include(t => t.TripAssignments)
                    .ThenInclude(t => t.Student)
                        .ThenInclude(t => t.ParentAssignments)
                            .ThenInclude(t => t.Parent)
                .Include(t => t.TripAssignments)
                    .ThenInclude(t => t.Student)
                        .ThenInclude(t => t.School)
                .AsNoTracking()
                .ToListAsync();
            if (id != null)
            {
                ViewData["TripID"] = id.Value;
                Trip trip = viewModel.Trips.Where(
                i => i.ID == id.Value).Single();
                viewModel.Students = trip.TripAssignments.Select(s => s.Student);
            }
            DateTime utcDateTime = kiwiNow.ToUniversalTime();
            string nzTimeZoneKey = "New Zealand Standard Time";
            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById(nzTimeZoneKey);
            DateTime nzDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, nzTimeZone);
            var monCount = _context.Students.Where(b => b.Monday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Monday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var tueCount = _context.Students.Where(b => b.Tuesday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Tuesday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var wedCount = _context.Students.Where(b => b.Wednesday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Wednesday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var thuCount = _context.Students.Where(b => b.Thursday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Thursday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            var friCount = _context.Students.Where(b => b.Friday.Date >= nzDateTime.Date).Count() - _context.TripAssignments.Where(a => a.Trip.PickupTime.DayOfWeek == DayOfWeek.Friday && a.Trip.PickupTime.Date >= nzDateTime.Date).Count();
            ViewData["MonCount"] = monCount >= 0 && _context.Students.Any(c => c.Monday.Date >= nzDateTime.Date) ? monCount.ToString() : "Past";
            ViewData["TueCount"] = tueCount >= 0 && _context.Students.Any(c => c.Tuesday.Date >= nzDateTime.Date) ? tueCount.ToString() : "Past";
            ViewData["WedCount"] = wedCount >= 0 && _context.Students.Any(c => c.Wednesday.Date >= nzDateTime.Date) ? wedCount.ToString() : "Past";
            ViewData["ThuCount"] = thuCount >= 0 && _context.Students.Any(c => c.Thursday.Date >= nzDateTime.Date) ? thuCount.ToString() : "Past";
            ViewData["FriCount"] = friCount >= 0 && _context.Students.Any(c => c.Friday.Date >= nzDateTime.Date) ? friCount.ToString() : "Past";

            ViewData["Drivers"] = _context.Drivers.Include(d => d.User).ToList().Distinct();
            ViewData["StudentID"] = new SelectList(_context.Students.OrderBy(s => s.LastName), "Fullname", "Fullname");

            var studentSchool = _context.Students
                .Include(g => g.School).OrderBy(g => g.School.Name).ThenBy(g => g.Fullname);
            ViewBag.StudentSchool = new SelectList(studentSchool, "Fullname", "Fullname", null, "School.Name");
            if (!String.IsNullOrEmpty(searchStudent))
            {
                viewModel.Trips = viewModel.Trips.Where(a => a.TripAssignments.Any(t => t.Student.Fullname.ToUpper().Contains(searchStudent.ToUpper())) && a.PickupTime.DayOfWeek == DayOfWeek.Friday);
                ViewData["SearchResult"] = searchStudent.ToString();
            }
            if (TripDate != null)
            {
                viewModel.Trips = viewModel.Trips.Where(a => a.PickupTime == TripDate);
                ViewData["DateResult"] = TripDate.Value.ToShortDateString();
            }
            return View(viewModel);
        }

        // GET: Trips/Details/5
        [HttpGet]
        public async Task<IActionResult> RoleCall(int? id, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (id == null)
            {
                return NotFound();
            }
            var trip = await _context.Trips
                .Include(t => t.TripAssignments)
                    .ThenInclude(t => t.Student)
                        .ThenInclude(t => t.School)
                .Include(t => t.TripAssignments)
                    .ThenInclude(t => t.Student)
                        .ThenInclude(t => t.ParentAssignments)
                            .ThenInclude(t => t.Parent)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);
            if (trip == null)
            {
                return NotFound();
            }
            return View(trip);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RollCall(int id, [Bind("ID,PickupTime,Complete,BusID,DriverID")] Trip trip)
        {
            string day = "";
            if (id != trip.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    day = trip.PickupTime.DayOfWeek == DayOfWeek.Monday ? "Monday" :
                    trip.PickupTime.DayOfWeek == DayOfWeek.Tuesday ? "Tuesday" :
                    trip.PickupTime.DayOfWeek == DayOfWeek.Wednesday ? "Wednesday" :
                    trip.PickupTime.DayOfWeek == DayOfWeek.Thursday ? "Thursday" :
                    trip.PickupTime.DayOfWeek == DayOfWeek.Friday ? "Friday" : "Index";
                    _context.Update(trip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TripExists(trip.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction(day);
            }
            return View(trip);
        }
        // POST: TripAssignments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RollCallEdit(int id, [Bind("ID,StudentID,TripID,StatusOfPickUp,LoggedPickupTime")] TripAssignment tripAssignment, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            //Below var is just for the redirect ID
            var tripID = tripAssignment.TripID;
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
                //return RedirectToAction(nameof(Index));
                return Redirect("RoleCall/" + tripID + "?returnurl=" + returnUrl);
            }
            return View();
        }

        // GET: Trips/Create
        public IActionResult Create(DateTime? PickupTime = null, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["DateFilter"] = PickupTime;
            var trip = new Trip();
            trip.TripAssignments = new List<TripAssignment>();
            PopulateAssignedStudentData(trip, PickupTime);
            ViewBag.BusID = new SelectList(_context.Buses, "ID", "IDwithCapacity", trip.BusID, "Capacity");
            //ViewData["DriverID"] = new SelectList(_context.Drivers, "UserID", "UserID", trip.DriverID);
            ViewData["DriverID"] = _context.Drivers.Include(d => d.User).ToList();
            ViewData["Schools"] = _context.School.OrderBy(z => z.Name).ToList();
            ViewData["Trips"] = _context.Trips.ToList();
            ViewData["TripAssignments"] = _context.TripAssignments.ToList();
            if (PickupTime != null)
            {
                string day = PickupTime.Value.DayOfWeek == DayOfWeek.Monday ? "Monday" :
          PickupTime.Value.DayOfWeek == DayOfWeek.Tuesday ? "Tuesday" :
          PickupTime.Value.DayOfWeek == DayOfWeek.Wednesday ? "Wednesday" :
          PickupTime.Value.DayOfWeek == DayOfWeek.Thursday ? "Thursday" :
          PickupTime.Value.DayOfWeek == DayOfWeek.Friday ? "Friday" : "Index";
                ViewData["ReturnUrl"] = day;
            }
            return View();
        }
        //PopulateAssignedCourseData method Microsoft Tutorial
        public void PopulateAssignedStudentData(Trip trip, DateTime? PickupTime = null)
        {
            ViewData["DateFilter"] = PickupTime;
            var allStudents = _context.Students.Include(s => s.School)
                .Include(s => s.TripAssignments)
                .Where(s => s.Monday == PickupTime || s.Tuesday == PickupTime || s.Wednesday == PickupTime || s.Thursday == PickupTime || s.Friday == PickupTime);
            var allTrips = _context.TripAssignments.Where(a => a.Trip.PickupTime == PickupTime);
            var tripsStudents = new HashSet<int>(trip.TripAssignments.Select(s => s.StudentID));
            var viewModel = new List<AssignedTripData>();
            bool onOff = false;
            foreach (var student in allStudents)
            {
                //Where filters for students already in trips via the date passed in as PickupTime but still shows kids matching this TripID
                foreach (var tripss in allTrips.Where(z => z.StudentID == student.ID && z.TripID != trip.ID))
                {
                    onOff = true;
                }
                if (onOff == false && PickupTime != null)
                {
                    viewModel.Add(new AssignedTripData
                    {
                        StudentID = student.ID,
                        Student = student,
                        Assigned = tripsStudents.Contains(student.ID)
                    });
                }
                onOff = false;
            }
            ViewData["Students"] = viewModel;
        }

        // POST: Trips/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,PickupTime,Complete,BusID,DriverID")] Trip trip, string[] selectedStudents, DateTime PickupTime)
        {
            string day = PickupTime.DayOfWeek == DayOfWeek.Monday ? "Monday" :
                PickupTime.DayOfWeek == DayOfWeek.Tuesday ? "Tuesday" :
                PickupTime.DayOfWeek == DayOfWeek.Wednesday ? "Wednesday" :
                PickupTime.DayOfWeek == DayOfWeek.Thursday ? "Thursday" :
                PickupTime.DayOfWeek == DayOfWeek.Friday ? "Friday" : "Index";
            if (selectedStudents != null)
            {
                trip.TripAssignments = new List<TripAssignment>();
                foreach (var students in selectedStudents)
                {
                    var studentToAdd = new TripAssignment { TripID = trip.ID, StudentID = int.Parse(students), StatusOfPickUp = StatusOfPickUp.Scheduled };
                    trip.TripAssignments.Add(studentToAdd);
                }
            }
            if (ModelState.IsValid)
            {
                _context.Add(trip);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(day));
                return RedirectToAction(day);
            }
            ViewBag.BusID = new SelectList(_context.Buses, "ID", "IDwithCapacity", trip.BusID, "Capacity");
            ViewData["DriverID"] = _context.Drivers.Include(d => d.User).ToList();
            ViewData["Schools"] = _context.School.OrderBy(z => z.Name).ToList();
            //ViewData["DriverID"] = new SelectList(_context.Drivers, "UserID", "UserID", trip.DriverID);
            PopulateAssignedStudentData(trip, PickupTime);
            return View(trip);
        }

        // GET: Trips/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, DateTime PickupTime, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            //var PickupTime = DateTime.Parse("1/01/0001 12:00:00 AM");
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .Include(t => t.TripAssignments)
                    .ThenInclude(t => t.Student)
                        .ThenInclude(t => t.School)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);
            if (trip == null)
            {
                return NotFound();
            }
            if (id != null)
            {
                PickupTime = trip.PickupTime;
            }
            ViewBag.BusID = new SelectList(_context.Buses, "ID", "IDwithCapacity", trip.BusID, "Capacity");
            //ViewData["DriverID"] = new SelectList(_context.Drivers, "UserID", "UserID", trip.DriverID);
            //Custom list to get user name of driver from User's model
            ViewData["DriverID"] = _context.Drivers.Include(d => d.User).ToList();
            ViewData["Schools"] = _context.School.OrderBy(s => s.Name).ToList();
            PopulateAssignedStudentData(trip, PickupTime);
            return View(trip);
        }

        // POST: Trips/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedStudents, DateTime PickupTime)
        {
            string day = PickupTime.DayOfWeek == DayOfWeek.Monday ? "Monday" :
            PickupTime.DayOfWeek == DayOfWeek.Tuesday ? "Tuesday" :
            PickupTime.DayOfWeek == DayOfWeek.Wednesday ? "Wednesday" :
            PickupTime.DayOfWeek == DayOfWeek.Thursday ? "Thursday" :
            PickupTime.DayOfWeek == DayOfWeek.Friday ? "Friday" : "Index";
            if (id == null)
            {
                return NotFound();
            }

            var tripToUpdate = await _context.Trips
                .Include(t => t.TripAssignments)
                    .ThenInclude(t => t.Student)
                        .ThenInclude(t => t.School)
                .SingleOrDefaultAsync(s => s.ID == id);

            if (await TryUpdateModelAsync<Trip>(
                tripToUpdate,
                "",
                t => t.PickupTime, t => t.Complete, t => t.BusID, t => t.DriverID))
            {
                UpdateTripStudents(selectedStudents, tripToUpdate);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction(day);
            }
            ViewBag.BusID = new SelectList(_context.Buses, "ID", "IDwithCapacity", tripToUpdate.BusID, "Capacity");
            //Custom list to get user name of driver from User's model
            ViewData["DriverID"] = _context.Drivers.Include(d => d.User).ToList();
            ViewData["Schools"] = _context.School.OrderBy(s => s.Name).ToList();
            //ViewData["DriverID"] = new SelectList(_context.Drivers, "UserID", "UserID", tripToUpdate.DriverID);
            UpdateTripStudents(selectedStudents, tripToUpdate);
            PopulateAssignedStudentData(tripToUpdate, PickupTime);
            return View(tripToUpdate);
        }
        //
        // UpdateInstructorCourses Microsoft tutorial
        //
        private void UpdateTripStudents(string[] selectedStudents, Trip tripToUpdate)
        {
            if (selectedStudents == null)
            {
                tripToUpdate.TripAssignments = new List<TripAssignment>();
                return;
            }

            var selectedStudentsHS = new HashSet<string>(selectedStudents);
            var tripStudents = new HashSet<int>
                (tripToUpdate.TripAssignments.Select(s => s.Student.ID));
            foreach (var student in _context.Students)
            {
                if (selectedStudentsHS.Contains(student.ID.ToString()))
                {
                    if (!tripStudents.Contains(student.ID))
                    {
                        tripToUpdate.TripAssignments.Add(new TripAssignment { TripID = tripToUpdate.ID, StudentID = student.ID, StatusOfPickUp = StatusOfPickUp.Scheduled });
                    }
                }
                else
                {

                    if (tripStudents.Contains(student.ID))
                    {
                        TripAssignment studentToRemove = tripToUpdate.TripAssignments.SingleOrDefault(t => t.StudentID == student.ID);
                        _context.Remove(studentToRemove);
                    }
                }
            }
        }

        // GET: Trips/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .Include(t => t.Bus)
                .Include(t => t.Driver)
                    .ThenInclude(t => t.User)
                .Include(t => t.TripAssignments)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);
            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trip = await _context.Trips
                .Include(t => t.TripAssignments)
                .SingleOrDefaultAsync(m => m.ID == id);
            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //POST: Trips/DeleteBatch/id's Trial
        [HttpPost, ActionName("DeleteBatch")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBatch(IFormCollection formCollection)
        {
            string[] ids = formCollection["ID"].ToString().Split(new char[] { ',' });
            foreach (string id in ids)
            {
                var trip = await _context.Trips
                    .Include(t => t.TripAssignments)
                    .SingleOrDefaultAsync(m => m.ID == int.Parse(id));
                _context.Trips.Remove(trip);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //
        //Trip Reporting
        //
        public ActionResult Report(DateTime? DateSearch = null)
        {
            TripReport tripReport = new TripReport(DateSearch);
            byte[] abytes = tripReport.PrepareReport(getTrips());
            return File(abytes, "application/pdf");
        }
        public List<Trip> getTrips()
        {
            List<Trip> trips = new List<Trip>();
            trips = _context.Trips
                .Include(t => t.Bus)
                .Include(t => t.Driver)
                    .ThenInclude(t => t.User)
                .Include(t => t.TripAssignments)
                    .ThenInclude(t => t.Student)
                        .ThenInclude(t => t.ParentAssignments)
                            .ThenInclude(t => t.Parent)
                .Include(t => t.TripAssignments)
                    .ThenInclude(t => t.Student)
                        .ThenInclude(t => t.School)
                .AsNoTracking()
                .OrderBy(t => t.BusID)
                .ToList();
            ViewData["tripsList"] = trips;
            return trips;
        }

        private bool TripExists(int id)
        {
            return _context.Trips.Any(e => e.ID == id);
        }
        private bool TripAssignmentExists(int id)
        {
            return _context.TripAssignments.Any(e => e.ID == id);
        }
    }
}
