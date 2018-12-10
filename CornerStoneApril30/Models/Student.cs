using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CornerStoneApril30.Models
{
    public class Student
    {
        public int ID { get; set; }
        [Required]
        [StringLength(60)]
        [DisplayName("Surname")]
        public string LastName { get; set; }
        [Required]
        [Column("FirstName")]
        [DisplayName("First & Middle Names")]
        [StringLength(60, ErrorMessage = "First name cannot be longer than 50 characters.")]
        public string FirstMidName { get; set; }
        [DisplayName("Full Name")]
        public string Fullname
        {
            get
            {
                return LastName + ", " + FirstMidName;
            }
        }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Age { get; set; }
        [Display(Name = "Age")]
        public int AgeNow
        {
            get
            {
                int age = DateTime.Now.Year - Age.Year;
                DateTime birthdate = Age.Date;
                DateTime birthday = new DateTime(DateTime.Now.Year, birthdate.Month, birthdate.Day);
                if (DateTime.Now < birthday) age--;
                return age;
            }
        }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Monday { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Tuesday { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Wednesday { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Thursday { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Friday { get; set; }
        public int SchoolID { get; set; }
        public School School { get; set; }
        public ICollection<ParentAssignment> ParentAssignments { get; set; }
        public ICollection<TripAssignment> TripAssignments { get; set; }

    }
}
