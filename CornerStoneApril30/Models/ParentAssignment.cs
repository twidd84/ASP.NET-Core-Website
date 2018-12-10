using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CornerStoneApril30.Models
{
    public class ParentAssignment
    {
        [Key]
        public int ID { get; set; }
        public int ParentID { get; set; }
        public int StudentID { get; set; }
        public Parent Parent { get; set; }
        public Student Student { get; set; }
    }
}
