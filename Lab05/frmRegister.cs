using System;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Lab05.DAL;

namespace Lab05.GUI
{
    public partial class frmRegister : Form
    {
        private ComboBox cmbFacultyCode;
        private ComboBox cmbMajorCode;
        private DataGridView gridRegister;
        private Button btnRegisterCode;
        private Button btnCloseCode;
        private Label lblFacultyCode;
        private Label lblMajorCode;

        private DataTable studentTable = new DataTable();
        private bool isLoading = false;

        public frmRegister()
        {
            InitializeComponent();

            // Dùng giao diện tạo bằng code thay cho giao diện Designer cũ.
            Controls.Clear();
            CreateInterface();

            Load += frmRegister_Load;
            cmbFacultyCode.SelectedIndexChanged +=
                cmbFacultyCode_SelectedIndexChanged;

            btnRegisterCode.Click += btnRegisterCode_Click;
            btnCloseCode.Click += btnCloseCode_Click;
        }

        // =====================================
        // TẠO GIAO DIỆN
        // =====================================
        private void CreateInterface()
        {
            Text = "ĐĂNG KÝ CHUYÊN NGÀNH";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(950, 600);
            MinimumSize = new Size(800, 500);
            Font = new Font("Segoe UI", 10);
            BackColor = Color.White;

            lblFacultyCode = new Label
            {
                Text = "Khoa:",
                Location = new Point(25, 25),
                Size = new Size(90, 30)
            };

            cmbFacultyCode = new ComboBox
            {
                Location = new Point(110, 22),
                Size = new Size(280, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            lblMajorCode = new Label
            {
                Text = "Chuyên ngành:",
                Location = new Point(420, 25),
                Size = new Size(130, 30)
            };

            cmbMajorCode = new ComboBox
            {
                Location = new Point(550, 22),
                Size = new Size(340, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            gridRegister = new DataGridView
            {
                Location = new Point(25, 75),
                Size = new Size(870, 400),
                Anchor = AnchorStyles.Top |
                         AnchorStyles.Bottom |
                         AnchorStyles.Left |
                         AnchorStyles.Right,

                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = false,
                ReadOnly = false,
                MultiSelect = false,
                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,
                EditMode = DataGridViewEditMode.EditOnEnter,
                RowHeadersVisible = false,
                BackgroundColor = Color.White
            };

            // Cột checkbox cho phép người dùng tích chọn.
            DataGridViewCheckBoxColumn colChoose =
                new DataGridViewCheckBoxColumn
                {
                    Name = "colChooseCode",
                    HeaderText = "Chọn",
                    DataPropertyName = "Chon",
                    ReadOnly = false,
                    ThreeState = false,
                    TrueValue = true,
                    FalseValue = false,
                    FillWeight = 45
                };

            DataGridViewTextBoxColumn colStudentID =
                new DataGridViewTextBoxColumn
                {
                    Name = "colStudentIDCode",
                    HeaderText = "MSSV",
                    DataPropertyName = "MSSV",
                    ReadOnly = true,
                    FillWeight = 90
                };

            DataGridViewTextBoxColumn colFullName =
                new DataGridViewTextBoxColumn
                {
                    Name = "colFullNameCode",
                    HeaderText = "Họ tên",
                    DataPropertyName = "HoTen",
                    ReadOnly = true,
                    FillWeight = 160
                };

            DataGridViewTextBoxColumn colFaculty =
                new DataGridViewTextBoxColumn
                {
                    Name = "colFacultyCode",
                    HeaderText = "Khoa",
                    DataPropertyName = "Khoa",
                    ReadOnly = true,
                    FillWeight = 120
                };

            DataGridViewTextBoxColumn colScore =
                new DataGridViewTextBoxColumn
                {
                    Name = "colScoreCode",
                    HeaderText = "Điểm trung bình",
                    DataPropertyName = "DiemTrungBinh",
                    ReadOnly = true,
                    FillWeight = 90,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Format = "0.00"
                    }
                };

            gridRegister.Columns.AddRange(
                colChoose,
                colStudentID,
                colFullName,
                colFaculty,
                colScore
            );

            // Quan trọng: lưu giá trị checkbox ngay khi tích.
            gridRegister.CurrentCellDirtyStateChanged +=
                gridRegister_CurrentCellDirtyStateChanged;

            gridRegister.CellContentClick +=
                gridRegister_CellContentClick;

            btnRegisterCode = new Button
            {
                Text = "Đăng ký",
                Location = new Point(600, 500),
                Size = new Size(130, 38),
                Anchor = AnchorStyles.Bottom |
                         AnchorStyles.Right,
                BackColor = Color.LightGreen
            };

            btnCloseCode = new Button
            {
                Text = "Đóng",
                Location = new Point(755, 500),
                Size = new Size(130, 38),
                Anchor = AnchorStyles.Bottom |
                         AnchorStyles.Right
            };

            Controls.Add(lblFacultyCode);
            Controls.Add(cmbFacultyCode);
            Controls.Add(lblMajorCode);
            Controls.Add(cmbMajorCode);
            Controls.Add(gridRegister);
            Controls.Add(btnRegisterCode);
            Controls.Add(btnCloseCode);
        }

        // =====================================
        // XỬ LÝ CHECKBOX
        // =====================================
        private void gridRegister_CurrentCellDirtyStateChanged(
            object sender, EventArgs e)
        {
            if (gridRegister.IsCurrentCellDirty)
            {
                gridRegister.CommitEdit(
                    DataGridViewDataErrorContexts.Commit);
            }
        }

        private void gridRegister_CellContentClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 &&
                e.ColumnIndex >= 0 &&
                gridRegister.Columns[e.ColumnIndex].Name ==
                "colChooseCode")
            {
                gridRegister.CommitEdit(
                    DataGridViewDataErrorContexts.Commit);
            }
        }

        // =====================================
        // TẢI FORM
        // =====================================
        private void frmRegister_Load(object sender, EventArgs e)
        {
            LoadFaculties();
        }

        // =====================================
        // TẢI DANH SÁCH KHOA
        // =====================================
        private void LoadFaculties()
        {
            try
            {
                isLoading = true;

                using (StudentModel db = new StudentModel())
                {
                    var faculties = db.Faculties
                        .AsNoTracking()
                        .Select(f => new
                        {
                            f.FacultyID,
                            f.FacultyName
                        })
                        .ToList();

                    cmbFacultyCode.DataSource = null;
                    cmbFacultyCode.DisplayMember = "FacultyName";
                    cmbFacultyCode.ValueMember = "FacultyID";
                    cmbFacultyCode.DataSource = faculties;
                    cmbFacultyCode.SelectedIndex = -1;
                }

                cmbMajorCode.DataSource = null;
                gridRegister.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải khoa: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                isLoading = false;
            }
        }

        // =====================================
        // CHỌN KHOA
        // =====================================
        private void cmbFacultyCode_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            if (isLoading)
                return;

            if (cmbFacultyCode.SelectedValue == null ||
                !(cmbFacultyCode.SelectedValue is int))
            {
                cmbMajorCode.DataSource = null;
                gridRegister.DataSource = null;
                return;
            }

            int facultyID =
                Convert.ToInt32(cmbFacultyCode.SelectedValue);

            LoadMajors(facultyID);
            LoadUnregisteredStudents(facultyID);
        }

        // =====================================
        // TẢI CHUYÊN NGÀNH THEO KHOA
        // =====================================
        private void LoadMajors(int facultyID)
        {
            try
            {
                using (StudentModel db = new StudentModel())
                {
                    var majors = db.Majors
                        .AsNoTracking()
                        .Where(m => m.FacultyID == facultyID)
                        .Select(m => new
                        {
                            m.MajorID,
                            m.MajorName
                        })
                        .ToList();

                    cmbMajorCode.DataSource = null;
                    cmbMajorCode.DisplayMember = "MajorName";
                    cmbMajorCode.ValueMember = "MajorID";
                    cmbMajorCode.DataSource = majors;
                    cmbMajorCode.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải chuyên ngành: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================
        // TẢI SINH VIÊN CHƯA CÓ CHUYÊN NGÀNH
        // =====================================
        private void LoadUnregisteredStudents(int facultyID)
        {
            try
            {
                using (StudentModel db = new StudentModel())
                {
                    var students = db.Students
                        .AsNoTracking()
                        .Include(s => s.Faculty)
                        .Where(s =>
                            s.FacultyID == facultyID &&
                            s.MajorID == null)
                        .ToList();

                    studentTable = new DataTable();
                    studentTable.Columns.Add(
                        "Chon", typeof(bool));
                    studentTable.Columns.Add(
                        "MSSV", typeof(string));
                    studentTable.Columns.Add(
                        "HoTen", typeof(string));
                    studentTable.Columns.Add(
                        "Khoa", typeof(string));
                    studentTable.Columns.Add(
                        "DiemTrungBinh", typeof(double));

                    foreach (var student in students)
                    {
                        studentTable.Rows.Add(
                            false,
                            student.StudentID,
                            student.FullName,
                            student.Faculty != null
                                ? student.Faculty.FacultyName
                                : "",
                            student.AverageScore
                        );
                    }

                    gridRegister.DataSource = null;
                    gridRegister.DataSource = studentTable;
                    gridRegister.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải sinh viên: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================
        // ĐĂNG KÝ CHUYÊN NGÀNH
        // =====================================
        private void btnRegisterCode_Click(
            object sender, EventArgs e)
        {
            if (cmbFacultyCode.SelectedValue == null ||
                !(cmbFacultyCode.SelectedValue is int))
            {
                MessageBox.Show("Vui lòng chọn khoa.");
                return;
            }

            if (cmbMajorCode.SelectedValue == null ||
                !(cmbMajorCode.SelectedValue is int))
            {
                MessageBox.Show("Vui lòng chọn chuyên ngành.");
                return;
            }

            try
            {
                gridRegister.EndEdit();

                var selectedIDs = studentTable.AsEnumerable()
                    .Where(r => r.Field<bool>("Chon"))
                    .Select(r => r.Field<string>("MSSV"))
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .Distinct()
                    .ToList();

                if (selectedIDs.Count == 0)
                {
                    MessageBox.Show(
                        "Vui lòng tích chọn ít nhất một sinh viên.");
                    return;
                }

                int facultyID =
                    Convert.ToInt32(cmbFacultyCode.SelectedValue);

                int majorID =
                    Convert.ToInt32(cmbMajorCode.SelectedValue);

                using (StudentModel db = new StudentModel())
                {
                    bool validMajor = db.Majors.Any(m =>
                        m.MajorID == majorID &&
                        m.FacultyID == facultyID);

                    if (!validMajor)
                    {
                        MessageBox.Show(
                            "Chuyên ngành không thuộc khoa đã chọn.");
                        return;
                    }

                    var students = db.Students
                        .Where(s =>
                            selectedIDs.Contains(s.StudentID) &&
                            s.FacultyID == facultyID &&
                            s.MajorID == null)
                        .ToList();

                    if (students.Count != selectedIDs.Count)
                    {
                        MessageBox.Show(
                            "Danh sách sinh viên đã thay đổi. " +
                            "Vui lòng chọn lại.");

                        LoadUnregisteredStudents(facultyID);
                        return;
                    }

                    foreach (var student in students)
                    {
                        student.MajorID = majorID;
                    }

                    db.SaveChanges();
                }

                MessageBox.Show(
                    "Đăng ký chuyên ngành thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadUnregisteredStudents(facultyID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi đăng ký: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================
        // ĐÓNG FORM
        // =====================================
        private void btnCloseCode_Click(
            object sender, EventArgs e)
        {
            Close();
        }
    }
}