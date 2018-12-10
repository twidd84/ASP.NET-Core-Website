using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CornerStoneApril30.Models
{
    public class Driver
    {      
        [StringLength(maximumLength: 8, MinimumLength = 8)]
        public string DriverLicence { get; set; }
        [Key]
        public string UserID { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<Trip> Trips { get; set; }
    }
}
