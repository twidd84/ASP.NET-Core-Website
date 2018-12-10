using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CornerStoneApril30.Models
{
    public enum StatusOfPickUp
    {
        Scheduled, PickedUp, NoShow, NoSchoolToday, Sick, Sport, Caregiver, Cancelled
    }
    public class TripAssignment
    {
        [Key]
        public int ID { get; set; }
        public int StudentID { get; set; }
        public int TripID { get; set; }
        public Student Student { get; set; }
        public Trip Trip { get; set; }
        public StatusOfPickUp StatusOfPickUp { get; set; }
        [DataType(DataType.Date)]
        public DateTime LoggedPickupTime { get; set; }
        public string LogTime
        {
            get
            {
                if (LoggedPickupTime.ToShortTimeString() == "12:00 AM")
                {
                    return "-";
                }
                else
                {
                    return LoggedPickupTime.ToString("h:mm tt", CultureInfo.GetCultureInfo("en-NZ"));
                }
            }
        }
    }
}
