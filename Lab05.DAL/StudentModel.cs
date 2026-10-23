
using System.Data.Entity;
using Lab05.DAL.Entities;

namespace Lab05.DAL
{
    public class StudentModel : DbContext
    {
        public StudentModel()
            : base("name=StudentContextDB")
        {
            Database.SetInitializer(
                new CreateDatabaseIfNotExists<StudentModel>());
        }

        public virtual DbSet<Faculty> Faculties { get; set; }
        public virtual DbSet<Major> Majors { get; set; }
        public virtual DbSet<Student> Students { get; set; }
    }
}

