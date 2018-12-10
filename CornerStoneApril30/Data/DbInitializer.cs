using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CornerStoneApril30.Models;
using Microsoft.EntityFrameworkCore;

namespace CornerStoneApril30.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            //Look for any students
        //    if (context.Students.Any())
        //    {
        //        return;   // DB has been seeded
        //    }
        //    var students = new Student[]
        //        {

        //    new Student { FirstMidName = "James H", LastName = "Williams", Age = DateTime.Parse("03-01-2000"), SchoolID = 1, CsvTime = "3:00" },
        //    new Student { FirstMidName = "Tom H", LastName = "Smith", Age = DateTime.Parse("04-02-2001"), SchoolID = 1, CsvTime = "3:00" },
        //    new Student { FirstMidName = "Sarah J", LastName = "Boal", Age = DateTime.Parse("05-03-2001"), SchoolID = 1, CsvTime = "3:00" },
        //    new Student { FirstMidName = "Jane R", LastName = "Wilks", Age = DateTime.Parse("06-04-2002"), SchoolID = 1, CsvTime = "3:00" },
        //    new Student { FirstMidName = "Bobby E", LastName = "Berry", Age = DateTime.Parse("05-12-2002"), SchoolID = 1, CsvTime = "3:00" },
        //    new Student { FirstMidName = "Jordan W", LastName = "Gordan", Age = DateTime.Parse("01-07-2001"), SchoolID = 1, CsvTime = "3:00" },
        //    new Student { FirstMidName = "Alice F", LastName = "White", Age = DateTime.Parse("07-08-2001"), SchoolID = 3, CsvTime = "3:15" },
        //    new Student { FirstMidName = "Sarah F", LastName = "Koogh", Age = DateTime.Parse("09-09-2004"), SchoolID = 3, CsvTime = "3:15" },
        //    new Student { FirstMidName = "Sally G", LastName = "Bromley", Age = DateTime.Parse("23-10-2005"), SchoolID = 2, CsvTime = "3:10" }

        //};
        //    foreach (Student s in students)
        //    {
        //        context.Students.Add(s);
        //    }
        //    context.SaveChanges();


            if (context.School.Any())
            {
                return;
            }
            var schools = new School[]
            {
            new School{Name="James Hargest Senior",Address="288 Layard St, Hargest, Invercargill 9810",Phone="03-217 6129"},
            new School{Name="Boys High",Address="181 Herbert St, Gladstone, Invercargill 9810",Phone="03-211 3003"},
            new School{Name="Girls high",Address="328 Tweed St, Georgetown, Invercargill 9812",Phone="03-211 6030"},
            new School{Name="Ascot Community",Address="580 Tay St, Hawthorndale, Invercargill 9810",Phone="03-217 5196"},
            new School{Name="Myross Bush",Address="288 Mill Rd N, Invercargill 9872",Phone="03-230 4817"},
            new School{Name="Waverley Park",Address="55 Eden Cres, Waverley, Invercargill 9810",Phone="03-217 9332"},
            new School{Name="Donovan",Address="200 Drury Ln, Grasmere, Invercargill 9810",Phone="03-215 9664"},
            new School{Name="Sacred Heart",Address="435 North Rd, Waikiwi, Invercargill 9810",Phone="03-215 7317"},
            new School{Name="St Johns",Address="349 Dee St, Avenal, Invercargill 9810",Phone="03-218 7759"},
            new School{Name="7th Day Adventist",Address="28 Bainfield Rd, Waikiwi, Invercargill 9810",Phone="03-211 6030"},
            new School{Name="St Josephs",Address="70 Eye St, Appleby, Invercargill 9812",Phone="03-218 6574"},
            new School{Name="Middle",Address="31 Jed St, Invercargill, 9810",Phone="03-218 6444"},
            new School{Name="Waihopai",Address="121 Herbert St, Gladstone, Invercargill 9810",Phone="03-218 4228"},
            new School{Name="Tisbury",Address="11 RD, 3 Boundary Rd, Tisbury, Invercargill 9877",Phone="03-216 8213"},
            new School{Name="Aurora College",Address="234 Regent St, Heidelberg, Invercargill 9812",Phone="03 211 6040"},
            new School{Name="Newfield Park",Address="82 Wilfrid St, Newfield, Invercargill 9812",Phone="03-216 9601"},
            new School{Name="Te Wharekura",Address="734 Tweed St, Newfield, Invercargill 9812",Phone="03-216 7701"},
            new School{Name="Otatara",Address="146 Dunns Rd, Otatara 9879",Phone="03-213 1009"},
            new School{Name="St Patricks",Address="161 Metzger St, Newfield, Invercargill 9812",Phone="03-216 8505"},
            new School{Name="Fernworth",Address="288 Pomona St, Strathern, Invercargill 9812",Phone="03-216 9659"},
            new School{Name="New River",Address="117 Elizabeth St, Appleby, Invercargill 9812",Phone="03-211 0035"},
            new School{Name="St Theresa's",Address="161 King St, Windsor, Invercargill 9810",Phone="03-217 6502"},
            new School{Name="Salford",Address="110 Lamond St, Hargest, Invercargill 9810",Phone="03-217 9521"},
            new School{Name="Windsor North",Address="91 Chelmsford St, Windsor, Invercargill 9810",Phone="03-217 8819"},
            new School{Name="James Hargest Jnr",Address="6 Layard St, Rosedale, Invercargill 9810",Phone="03-217 9250"},
            new School{Name="Verdon",Address="210 Rockdale Rd, Rockdale, Invercargill 9840",Phone="03-216 9039"}
            };
            foreach (School s in schools)
            {
                context.School.Add(s);
            }
            context.SaveChanges();

            if (context.Parents.Any())
            {
                return;
            }
            var parents = new Parent[]
            {
            new Parent{LastName="Willy",FirstMidName="Andrew K",RelationToChild=RelationToChild.Father},
            new Parent{LastName="Wild",FirstMidName="Abby S",RelationToChild=RelationToChild.Mother},
            new Parent{LastName="Williams",FirstMidName="John K",RelationToChild=RelationToChild.Father},
            new Parent{LastName="Wally",FirstMidName="Catherine J",RelationToChild=RelationToChild.Mother}

            };
            foreach (Parent p in parents)
            {
                context.Parents.Add(p);
            }
            context.SaveChanges();


            if (context.Buses.Any())
            {
                return;
            }


            var buses = new Bus[]
        {
            new Bus{ID=112,BusType=BusType.Van,Capacity=12,NumberPlate="CWT445"},
            new Bus{ID=212,BusType=BusType.Van,Capacity=12,NumberPlate="CDT544"},
            new Bus{ID=312,BusType=BusType.Van,Capacity=12,NumberPlate="DFT566"},
            new Bus{ID=412,BusType=BusType.Van,Capacity=12,NumberPlate="DFT566"},
            new Bus{ID=512,BusType=BusType.Van,Capacity=12,NumberPlate="DFT566"},
            new Bus{ID=612,BusType=BusType.Van,Capacity=12,NumberPlate="DFT566"},
            new Bus{ID=708,BusType=BusType.Van,Capacity=8,NumberPlate="DFG344"},
            new Bus{ID=808,BusType=BusType.Van,Capacity=8,NumberPlate="XSW344"},
            new Bus{ID=923,BusType=BusType.Bus,Capacity=23,NumberPlate="QSE334"}

        };
            foreach (Bus b in buses)
            {
                context.Buses.Add(b);
            }
            context.SaveChanges();
        }

    }
}
