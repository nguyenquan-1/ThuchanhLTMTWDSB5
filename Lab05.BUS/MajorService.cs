
using System.Collections.Generic;
using System.Linq;
using Lab05.DAL;
using Lab05.DAL.Entities;

namespace Lab05.BUS
{
    public class MajorService
    {
        public List<Major> GetAllByFaculty(int facultyID)
        {
            using (StudentModel context = new StudentModel())
            {
                return context.Majors
                    .Where(m => m.FacultyID == facultyID)
                    .OrderBy(m => m.MajorName)
                    .ToList();
            }
        }
    }
}

