using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CornerStoneApril30.Models
{
    public enum RelationToChild
    {
        Mother,Father,Caregiver,Grandmother,Grandfather
    }
    public class Parent
    {
        public int ParentID { get; set; }
        [Required]
        [StringLength(60)]
        [DisplayName("Surname")]
        public string LastName { get; set; }
        [Required]
        [Column("FirstName")]
        [DisplayName("First & Middle Names")]
        [StringLength(60, ErrorMessage = "First name cannot be longer than 50 characters.")]
        public string FirstMidName { get; set; }
        [StringLength(15)]
        public string Phone { get; set; }
        [DisplayName("Full Name")]
        public string Fullname
        {
            get
            {
                return LastName + ", " + FirstMidName;
            }
        }
        public RelationToChild? RelationToChild { get; set; }
        public ICollection<ParentAssignment> ParentAssignments { get; set; }
    }
}
