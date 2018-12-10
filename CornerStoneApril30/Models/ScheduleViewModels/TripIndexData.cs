using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CornerStoneApril30.Models.ScheduleViewModels
{
    public class TripIndexData
    {
        public IEnumerable<Trip> Trips { get; set;}
        public IEnumerable<School> Schools { get; set; }
        public IEnumerable<Driver> Drivers { get; set; }
        public IEnumerable<Student> Students { get; set; }
        public IEnumerable<ParentAssignment> ParentAssignments { get; set; }
        public IEnumerable<TripAssignment> TripAssignments { get; set; }
    }
}
