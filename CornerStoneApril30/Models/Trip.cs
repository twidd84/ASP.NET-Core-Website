using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CornerStoneApril30.Models
{
    public class Trip
    {
        public int ID { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PickupTime { get; set; }
        public bool Complete { get; set; }
        [Required]
        public int BusID { get; set; }
        public Bus Bus { get; set; }
        [Required]
        public string DriverID { get; set; }
        public Driver Driver { get; set; }
        public ICollection<TripAssignment> TripAssignments { get; set; }
        CultureInfo kiwiCulture = new CultureInfo("nl-NL");
        public string WeekdayDay
        {
            get
            {
                return PickupTime.DayOfWeek + " " + PickupTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
        }

    }
}
