using System;
using System.Linq;
using System.Windows.Forms;
using System.Data.Entity;
using Lab05.DAL;
using Lab05.DAL.Entities;

namespace Lab05.GUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.Load += Form1_Load;

            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnReset.Click += btnReset_Click;

            dgvStudents.CellClick += dgvStudents_CellClick;
            cmbFaculty.SelectedIndexChanged += cmbFaculty_SelectedIndexChanged;
        }

        // =========================
        // KHI MỞ FORM
        // =========================
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                LoadFaculties();
                LoadStudents();
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải dữ liệu: " + ex.Message,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================
        // TẢI DANH SÁCH KHOA
        // =========================
        private void LoadFaculties()
        {
            using (StudentModel db = new StudentModel())
            {
                var faculties = db.Faculties
                    .AsNoTracking()
                    .OrderBy(f => f.FacultyName)
                    .ToList();

                cmbFaculty.DataSource = null;
                cmbFaculty.DisplayMember = "FacultyName";
                cmbFaculty.ValueMember = "FacultyID";
                cmbFaculty.DataSource = faculties;

                if (faculties.Count > 0)
                    cmbFaculty.SelectedIndex = 0;
                else
                    cmbFaculty.SelectedIndex = -1;

                if (faculties.Count == 0)
                {
                    cmbMajor.DataSource = null;

                    MessageBox.Show(
                        "Chưa có dữ liệu khoa trong bảng Faculties của database Lab05_EntityFramework.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }

            LoadMajorsForSelectedFaculty();
        }

        // =========================
        // TẢI CHUYÊN NGÀNH THEO KHOA
        // =========================
        private void cmbFaculty_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            LoadMajorsForSelectedFaculty();
        }

        private void LoadMajorsForSelectedFaculty()
        {
            if (cmbFaculty.SelectedIndex < 0 ||
                cmbFaculty.SelectedValue == null)
            {
                cmbMajor.DataSource = null;
                return;
            }

            int facultyID;

            if (!int.TryParse(
                Convert.ToString(cmbFaculty.SelectedValue),
                out facultyID))
            {
                cmbMajor.DataSource = null;
                return;
            }

            using (StudentModel db = new StudentModel())
            {
                var majors = db.Majors
                    .AsNoTracking()
                    .Where(m => m.FacultyID == facultyID)
                    .OrderBy(m => m.MajorName)
                    .ToList();

                cmbMajor.DataSource = null;
                cmbMajor.DisplayMember = "MajorName";
                cmbMajor.ValueMember = "MajorID";
                cmbMajor.DataSource = majors;

                if (majors.Count > 0)
                    cmbMajor.SelectedIndex = 0;
                else
                    cmbMajor.SelectedIndex = -1;
            }
        }

        // =========================
        // HIỂN THỊ DANH SÁCH SINH VIÊN
        // =========================
        private void LoadStudents()
        {
            using (StudentModel db = new StudentModel())
            {
                var students = db.Students
                    .Include(s => s.Faculty)
                    .Include(s => s.Major)
                    .ToList()
                    .Select(s => new
                    {
                        MSSV = s.StudentID,
                        HoTen = s.FullName,
                        Khoa = s.Faculty != null
                            ? s.Faculty.FacultyName : "",
                        ChuyenNganh = s.Major != null
                            ? s.Major.MajorName : "",
                        DiemTrungBinh = s.AverageScore,
                        FacultyID = s.FacultyID,
                        MajorID = s.MajorID
                    })
                    .ToList();

                dgvStudents.DataSource = students;
                dgvStudents.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;
                dgvStudents.ReadOnly = true;
                dgvStudents.SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect;
                dgvStudents.MultiSelect = false;
                dgvStudents.AllowUserToAddRows = false;
            }
        }

        // =========================
        // KIỂM TRA DỮ LIỆU
        // =========================
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtStudentID.Text))
            {
                MessageBox.Show("Vui lòng nhập mã sinh viên!");
                txtStudentID.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên sinh viên!");
                txtFullName.Focus();
                return false;
            }

            int facultyID;

            if (cmbFaculty.SelectedValue == null ||
                !int.TryParse(
                    Convert.ToString(cmbFaculty.SelectedValue),
                    out facultyID))
            {
                MessageBox.Show("Vui lòng chọn khoa!");
                cmbFaculty.Focus();
                return false;
            }

            double score;

            if (!double.TryParse(txtAverageScore.Text, out score) ||
                score < 0 || score > 10)
            {
                MessageBox.Show(
                    "Điểm trung bình phải là số từ 0 đến 10!");
                txtAverageScore.Focus();
                return false;
            }

            return true;
        }

        // =========================
        // THÊM SINH VIÊN
        // =========================
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            string studentID = txtStudentID.Text.Trim();

            try
            {
                using (StudentModel db = new StudentModel())
                {
                    bool exists = db.Students.Any(
                        s => s.StudentID == studentID);

                    if (exists)
                    {
                        MessageBox.Show("Mã sinh viên đã tồn tại!");
                        return;
                    }

                    Student student = new Student
                    {
                        StudentID = studentID,
                        FullName = txtFullName.Text.Trim(),
                        FacultyID = Convert.ToInt32(
                            cmbFaculty.SelectedValue),
                        AverageScore = double.Parse(
                            txtAverageScore.Text),
                        MajorID = cmbMajor.SelectedValue == null
                            ? (int?)null
                            : Convert.ToInt32(cmbMajor.SelectedValue)
                    };

                    db.Students.Add(student);
                    db.SaveChanges();
                }

                MessageBox.Show("Thêm sinh viên thành công!");
                LoadStudents();
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi thêm sinh viên: " + ex.Message);
            }
        }

        // =========================
        // SỬA SINH VIÊN
        // =========================
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            string studentID = txtStudentID.Text.Trim();

            try
            {
                using (StudentModel db = new StudentModel())
                {
                    Student student = db.Students.Find(studentID);

                    if (student == null)
                    {
                        MessageBox.Show("Không tìm thấy sinh viên!");
                        return;
                    }

                    student.FullName = txtFullName.Text.Trim();
                    student.FacultyID = Convert.ToInt32(
                        cmbFaculty.SelectedValue);
                    student.AverageScore = double.Parse(
                        txtAverageScore.Text);

                    student.MajorID = cmbMajor.SelectedValue == null
                        ? (int?)null
                        : Convert.ToInt32(cmbMajor.SelectedValue);

                    db.SaveChanges();
                }

                MessageBox.Show("Cập nhật sinh viên thành công!");
                LoadStudents();
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi cập nhật sinh viên: " + ex.Message);
            }
        }

        // =========================
        // XÓA SINH VIÊN
        // =========================
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStudentID.Text))
            {
                MessageBox.Show(
                    "Vui lòng chọn sinh viên cần xóa!");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa sinh viên này không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                using (StudentModel db = new StudentModel())
                {
                    Student student = db.Students.Find(
                        txtStudentID.Text.Trim());

                    if (student == null)
                    {
                        MessageBox.Show("Không tìm thấy sinh viên!");
                        return;
                    }

                    db.Students.Remove(student);
                    db.SaveChanges();
                }

                MessageBox.Show("Xóa sinh viên thành công!");
                LoadStudents();
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi xóa sinh viên: " + ex.Message);
            }
        }

        // =========================
        // CHỌN SINH VIÊN TRÊN BẢNG
        // =========================
        private void dgvStudents_CellClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dgvStudents.Rows[e.RowIndex];

            txtStudentID.Text =
                Convert.ToString(row.Cells["MSSV"].Value);

            txtFullName.Text =
                Convert.ToString(row.Cells["HoTen"].Value);

            txtAverageScore.Text =
                Convert.ToString(row.Cells["DiemTrungBinh"].Value);

            object facultyValue = row.Cells["FacultyID"].Value;
            object majorValue = row.Cells["MajorID"].Value;

            if (facultyValue != null &&
                facultyValue != DBNull.Value)
            {
                cmbFaculty.SelectedValue =
                    Convert.ToInt32(facultyValue);

                if (majorValue != null &&
                    majorValue != DBNull.Value)
                {
                    cmbMajor.SelectedValue =
                        Convert.ToInt32(majorValue);
                }
                else
                {
                    cmbMajor.SelectedIndex = -1;
                }
            }
        }

        // =========================
        // LÀM MỚI FORM
        // =========================
        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            txtStudentID.Clear();
            txtFullName.Clear();
            txtAverageScore.Clear();

            if (cmbFaculty.Items.Count > 0 &&
                cmbFaculty.SelectedIndex < 0)
            {
                cmbFaculty.SelectedIndex = 0;
            }
            else
            {
                LoadMajorsForSelectedFaculty();
            }

            dgvStudents.ClearSelection();
            txtStudentID.Focus();
        }

        private void LoadStudentsChuaDKChuyenNganh()
        {
            using (StudentModel db = new StudentModel())
            {
                var students = db.Students
                    .Include(s => s.Faculty)
                    .Include(s => s.Major)
                    .Where(s => s.MajorID == null)
                    .ToList()
                    .Select(s => new
                    {
                        MSSV = s.StudentID,
                        HoTen = s.FullName,
                        Khoa = s.Faculty != null
                            ? s.Faculty.FacultyName : "",
                        ChuyenNganh = s.Major != null
                            ? s.Major.MajorName : "",
                        DiemTrungBinh = s.AverageScore,
                        FacultyID = s.FacultyID,
                        MajorID = s.MajorID
                    })
                    .ToList();

                dgvStudents.DataSource = students;
            }
        }

        private void btnDangKyChuyenNganh_Click(object sender, EventArgs e)
        {
            frmRegister frm = new frmRegister();
            frm.ShowDialog();
        }
    }
}