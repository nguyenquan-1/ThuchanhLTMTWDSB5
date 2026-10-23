
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Lab05.DAL;
using Lab05.DAL.Entities;

namespace Lab05.BUS
{
    public class StudentService
    {
        public List<Student> GetAll()
        {
            using (StudentModel context = new StudentModel())
            {
                return context.Students
                    .Include(s => s.Faculty)
                    .Include(s => s.Major)
                    .ToList();
            }
        }

        // Lấy sinh viên chưa đăng ký chuyên ngành
        public List<Student> GetAllHasNoMajor()
        {
            using (StudentModel context = new StudentModel())
            {
                return context.Students
                    .Include(s => s.Faculty)
                    .Where(s => s.MajorID == null)
                    .ToList();
            }
        }

        // Lấy sinh viên chưa đăng ký chuyên ngành theo khoa
        public List<Student> GetAllHasNoMajor(int facultyID)
        {
            using (StudentModel context = new StudentModel())
            {
                return context.Students
                    .Include(s => s.Faculty)
                    .Where(s => s.FacultyID == facultyID
                             && s.MajorID == null)
                    .ToList();
            }
        }

        // Tìm sinh viên theo mã
        public Student FindById(string studentID)
        {
            using (StudentModel context = new StudentModel())
            {
                return context.Students
                    .Include(s => s.Faculty)
                    .Include(s => s.Major)
                    .FirstOrDefault(s => s.StudentID == studentID);
            }
        }

        // Thêm hoặc cập nhật sinh viên
        public void InsertUpdate(Student student)
        {
            using (StudentModel context = new StudentModel())
            {
                Student existing = context.Students
                    .FirstOrDefault(s => s.StudentID == student.StudentID);

                if (existing == null)
                {
                    context.Students.Add(student);
                }
                else
                {
                    existing.FullName = student.FullName;
                    existing.FacultyID = student.FacultyID;
                    existing.AverageScore = student.AverageScore;
                    existing.MajorID = student.MajorID;
                }

                context.SaveChanges();
            }
        }

        // Đăng ký chuyên ngành cho nhiều sinh viên
        public void RegisterMajor(List<string> studentIDs, int majorID)
        {
            using (StudentModel context = new StudentModel())
            {
                Major major = context.Majors
                    .FirstOrDefault(m => m.MajorID == majorID);

                if (major == null)
                    throw new System.Exception("Không tìm thấy chuyên ngành.");

                foreach (string id in studentIDs)
                {
                    Student student = context.Students
                        .FirstOrDefault(s => s.StudentID == id);

                    if (student != null
                        && student.FacultyID == major.FacultyID
                        && student.MajorID == null)
                    {
                        student.MajorID = majorID;
                    }
                }

                context.SaveChanges();
            }
        }
    }
}

