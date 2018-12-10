using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CornerStoneApril30.Models
{
    public class School
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "School Name")]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string Address { get; set; }
        [StringLength(14)]
        public string Phone { get; set; }
        [Display(Name = "School & Address")]
        public string NameWithLocation
        {
            get
            {
                return Name + ", " + Address;
            }
        }

        public ICollection<Student> Students { get; set; }
    }

    public class SchoolComparer : IEqualityComparer<School>
    {
        public bool Equals(School x, School y)
        {
            if (x.ID == y.ID
                    && x.Name.ToLower() == y.Name.ToLower())
                return true;

            return false;
        }

        public int GetHashCode(School obj)
        {
            return obj.ID.GetHashCode();
        }
    }
}