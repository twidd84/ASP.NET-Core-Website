using System.Diagnostics;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Configuration;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.IO;
using CornerStoneApril30.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CornerStoneApril30.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPOI.SS.UserModel;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

namespace CornerStoneApril30.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        //
        //My Attempt
        //public IWorkbook wb = null;
        public Student[] studentList = new Student[12];

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Post(IFormFile files, DateTime PickupTime, int numRows)
        {
            ViewData["DateFilter"] = PickupTime;
            ViewData["NumRows"] = numRows; //Redundant way and not used for now. The method doesn't need a numRows passed in anymore.
            numRows -= 3;
            DateTime setDateMon = DateTime.Parse("01-01-0001");
            DateTime setDateTue = DateTime.Parse("01-01-0001");
            DateTime setDateWed = DateTime.Parse("01-01-0001");
            DateTime setDateThu = DateTime.Parse("01-01-0001");
            DateTime setDateFri = DateTime.Parse("01-01-0001");

            //wipe excel days for that date loaded to start afresh. Incase they make a mistake uploading the wrong list for that day.
            foreach (var s in _context.Students)
            {
                var result = _context.Students.SingleOrDefault(b => b.ID == s.ID);
                if (result != null)
                {
                    //Updating Students dates of availability
                    if (PickupTime.DayOfWeek == DayOfWeek.Monday)
                    {
                        result.Monday = setDateMon;
                    }
                    else if (PickupTime.DayOfWeek == DayOfWeek.Tuesday)
                    {
                        result.Tuesday = setDateTue;
                    }
                    else if (PickupTime.DayOfWeek == DayOfWeek.Wednesday)
                    {
                        result.Wednesday = setDateWed;
                    }
                    else if (PickupTime.DayOfWeek == DayOfWeek.Thursday)
                    {
                        result.Thursday = setDateThu;
                    }
                    else if (PickupTime.DayOfWeek == DayOfWeek.Friday)
                    {
                        result.Friday = setDateFri;
                    }
                }
            }
            //
            //long size = files.Sum(f => f.Length);
            // full path to file in temp location
            var filePath = Path.GetTempFileName();
            ISheet sheet;

            //foreach (var formFile in files)
            //{
            if (files.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await files.CopyToAsync(stream);
                    //My code below
                    stream.Position = 0;
                    IWorkbook wb;
                    //Handles both XSSF and HSSF automatically :) :) :)
                    wb = WorkbookFactory.Create(stream);
                    sheet = wb.GetSheetAt(0);
                    int count = 0;
                    for (int r = 3; r <= sheet.LastRowNum; r++)
                    {
                        IRow row = sheet.GetRow(r);
                        if (row.GetCell(0) == null)
                        {
                            break;
                        }
                        string nameRead = row.GetCell(0).StringCellValue;
                        string schoolName = row.GetCell(1).StringCellValue;
                        int idx = 0;
                        string schoolnotime = "";
                        string schoolwithtime = "";
                        idx = schoolName.Length;
                        idx = idx - 5;
                        schoolwithtime = schoolName.Substring(0, idx).TrimEnd().ToUpper();
                        schoolnotime = schoolName.ToUpper();
                        string[] parts = nameRead.Split(" ");
                        string firstMidName = parts[0];
                        string lastName = parts[1];
                        //toggle to work for adding or updating New student so far...
                        bool toggle = false;
                        int schoolID = 0;
                        foreach (var schoolOut in _context.School)
                        {
                            if (schoolOut.Name.ToUpper() == schoolnotime)
                            {
                                schoolID = schoolOut.ID;
                            }
                            else if (schoolOut.Name.ToUpper() == schoolwithtime)
                            {
                                schoolID = schoolOut.ID;
                            }
                        }
                        foreach (var s in _context.Students)
                        {
                            if (s.FirstMidName.ToString() == firstMidName && s.LastName.ToString() == lastName)
                            {
                                toggle = true;
                                var result = _context.Students.SingleOrDefault(b => b.FirstMidName == firstMidName && b.LastName == lastName);
                                if (result != null)
                                {
                                    result.SchoolID = schoolID;
                                    //Updating Students dates of availability
                                    if (PickupTime.DayOfWeek == DayOfWeek.Monday)
                                    {
                                        setDateMon = PickupTime;
                                        result.Monday = setDateMon;
                                    }
                                    else if (PickupTime.DayOfWeek == DayOfWeek.Tuesday)
                                    {
                                        setDateTue = PickupTime;
                                        result.Tuesday = setDateTue;
                                    }
                                    else if (PickupTime.DayOfWeek == DayOfWeek.Wednesday)
                                    {
                                        setDateWed = PickupTime;
                                        result.Wednesday = setDateWed;
                                    }
                                    else if (PickupTime.DayOfWeek == DayOfWeek.Thursday)
                                    {
                                        setDateThu = PickupTime;
                                        result.Thursday = setDateThu;
                                    }
                                    else if (PickupTime.DayOfWeek == DayOfWeek.Friday)
                                    {
                                        setDateFri = PickupTime;
                                        result.Friday = setDateFri;
                                    }
                                }
                            }
                        }
                        if (toggle == false)
                        {
                            //Updating Students dates of availability
                            if (PickupTime.DayOfWeek == DayOfWeek.Monday)
                            {
                                setDateMon = PickupTime;
                            }
                            else if (PickupTime.DayOfWeek == DayOfWeek.Tuesday)
                            {
                                setDateTue = PickupTime;
                            }
                            else if (PickupTime.DayOfWeek == DayOfWeek.Wednesday)
                            {
                                setDateWed = PickupTime;
                            }
                            else if (PickupTime.DayOfWeek == DayOfWeek.Thursday)
                            {
                                setDateThu = PickupTime;
                            }
                            else if (PickupTime.DayOfWeek == DayOfWeek.Friday)
                            {
                                setDateFri = PickupTime;
                            }
                            _context.Students.Add(new Student
                            {
                                LastName = lastName,
                                FirstMidName = firstMidName,
                                Age = DateTime.Parse("01-01-2018"),
                                Monday = setDateMon,
                                Tuesday = setDateTue,
                                Wednesday = setDateWed,
                                Thursday = setDateThu,
                                Friday = setDateFri,
                                SchoolID = schoolID
                            });
                        }
                        count++;
                        toggle = false;
                        //This break has been substituted for another way
                        //if (count == numRows)
                        //{
                        //    break;
                        //}
                    }
                    _context.SaveChanges();
                    wb.Close();
                    ViewData["UploadComplete"] = "Upload Successful! Ready to schedule trips for " + PickupTime.DayOfWeek + ", " + PickupTime.ToShortDateString();
                }
            }
            return View("Index");

            //process uploaded files
            //Don't rely on or trust the FileName property without validation.
            //return Ok(new
            //{
            //    //count = files.Count,
            //    //size,
            //    filePath
            //});
        }
    }
}
