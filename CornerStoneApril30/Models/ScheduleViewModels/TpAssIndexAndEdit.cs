using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CornerStoneApril30.Models.ScheduleViewModels
{
    public class TpAssIndexAndEdit
    {
        public IEnumerable<TripAssignment> TripAssignments { get; set; }
        public TripAssignment TripAssignment { get; set; }
    }
}
