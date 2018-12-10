using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CornerStoneApril30.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
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

        public Driver Driver { get; set; }
    }
}
