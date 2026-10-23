
using System.Collections.Generic;
using System.Linq;
using Lab05.DAL;
using Lab05.DAL.Entities;

namespace Lab05.BUS
{
    public class FacultyService
    {
        public List<Faculty> GetAll()
        {
            using (StudentModel context = new StudentModel())
            {
                return context.Faculties
                    .OrderBy(f => f.FacultyName)
                    .ToList();
            }
        }
    }
}

