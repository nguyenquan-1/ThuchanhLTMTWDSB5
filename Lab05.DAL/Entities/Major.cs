using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab05.DAL.Entities
{
    public class Major
    {
        [Key]
        public int MajorID { get; set; }

        [Required]
        [StringLength(100)]
        public string MajorName { get; set; }

        public int FacultyID { get; set; }

        [ForeignKey("FacultyID")]
        public virtual Faculty Faculty { get; set; }
    }
}
