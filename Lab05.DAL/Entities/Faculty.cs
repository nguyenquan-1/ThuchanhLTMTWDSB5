using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lab05.DAL.Entities
{
    public class Faculty
    {
        [Key]
        public int FacultyID { get; set; }

        [Required]
        [StringLength(100)]
        public string FacultyName { get; set; }

        public virtual ICollection<Student> Students { get; set; }
            = new HashSet<Student>();

        public virtual ICollection<Major> Majors { get; set; }
            = new HashSet<Major>();
    }
}
