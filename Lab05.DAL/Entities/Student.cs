using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab05.DAL.Entities
{
    public class Student
    {
        [Key]
        [StringLength(20)]
        public string StudentID { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        public int FacultyID { get; set; }

        public double AverageScore { get; set; }

        public int? MajorID { get; set; }

        [ForeignKey("FacultyID")]
        public virtual Faculty Faculty { get; set; }

        [ForeignKey("MajorID")]
        public virtual Major Major { get; set; }
    }
}
