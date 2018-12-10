using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CornerStoneApril30.Models
{
    public enum BusType
    {
        Van, Bus
    }
    public class Bus
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Number")]
        public int ID { get; set; }
        public BusType BusType { get; set; }
        public int Capacity { get; set; }
        public string NumberPlate { get; set; }
        public ICollection<Trip> Trips { get; set; }

        public string IDwithCapacity
        {
            get
            {
                return ID.ToString() + " - " + BusType + " Seats (" + Capacity.ToString()+ ")";
            }
        }
    }
}
