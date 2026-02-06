using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ScholarSync.Commons;
using ScholarSync.Models;
using ScholarSync.Db_Connection;
using ScholarSync.Repositories;
using Npgsql;

namespace ScholarSync.Controls.Student
{
    public partial class DeleteStudentControl : UserControl
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Panel searchPanel;
        private Panel detailsPanel;
        private Panel buttonPanel;
        
        // Search Controls
        private Label lblSearchStudent;
        private TextBox txtSearchStudent;
        private Button btnSearchStudent;
        private Button btnClearSearch;
        private Label lblSearchHint;
        
        // Student Information Display
        private Label lblCurrentStudent;
        
        // Details Display Controls
        private Label lblDetailsTitle;
        private DataGridView dgvStudentDetails;
        
        // Buttons
        private Button btnDelete;
        private Button btnCancel;
        
        // State
        private string currentStudentId = "";
        private string currentUserId = "";
        private bool studentLoaded = false;
        private DataTable studentDetailsTable;

        // Loading Indicator
        private LoadingIndicator loadingIndicator;

        // Repository
        private StudentRepository studentRepository;

        public DeleteStudentControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.BackColor = ConfigurationConstants.SSWhiteColor;

            // Check if user is Admin
            if (!SessionManager.IsAdmin())
            {
                ShowAccessDenied();
                return;
            }

            // Initialize repository
            studentRepository = new StudentRepository();

            InitializeUI();

            // Initialize loading indicator
            loadingIndicator = new LoadingIndicator();
            this.Controls.Add(loadingIndicator);
        }

        private void ShowAccessDenied()
        {
            Panel accessDeniedPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ConfigurationConstants.SSWhiteColor
            };

            Label lblAccessDenied = new Label
            {
                Text = "?? Access Denied",
                Font = new Font("Arial", 32, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSErrorRed,
                Location = new Point(0, 200),
                Size = new Size(ConfigurationConstants.ScreenWidth - 300, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblMessage = new Label
            {
                Text = "Only Administrators can delete students.\n\nPlease contact your system administrator for access.",
                Font = new Font("Arial", 14, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(0, 280),
                Size = new Size(ConfigurationConstants.ScreenWidth - 300, 80),
                TextAlign = ContentAlignment.TopCenter
            };

            accessDeniedPanel.Controls.Add(lblAccessDenied);
            accessDeniedPanel.Controls.Add(lblMessage);
            this.Controls.Add(accessDeniedPanel);
        }

        private void InitializeComponent()
        {
        }

        private void InitializeUI()
        {
            // Header Panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = ConfigurationConstants.SSDarkNavyColor
            };

            titleLabel = new Label
            {
                Text = "Delete Student",
                Font = new Font("Arial", 22, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(30, 0),
                Size = new Size(400, 80),
                TextAlign = ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.Add(titleLabel);
            this.Controls.Add(headerPanel);

            // Search Panel
            searchPanel = new Panel
            {
                Dock = DockStyle.None,
                BackColor = ConfigurationConstants.SSLightGrayBackground,
                Location = new Point(10, 100),
                Size = new Size(ConfigurationConstants.ScreenWidth - 300, 120),
                Padding = new Padding(20)
            };
            
            InitializeSearchControls();
            this.Controls.Add(searchPanel);

            // Details Panel
            detailsPanel = new Panel
            {
                Dock = DockStyle.None,
                BackColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(10, 240),
                Size = new Size(ConfigurationConstants.ScreenWidth - 300, ConfigurationConstants.ScreenHeight - 420),
                Padding = new Padding(40, 30, 40, 30),
                Visible = false
            };
            
            InitializeDetailsPanel();
            this.Controls.Add(detailsPanel);

            // Button Panel
            buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 100,
                BackColor = ConfigurationConstants.SSLightGrayBackground,
                Padding = new Padding(40, 25, 40, 25)
            };

            btnDelete = UIHelper.CreateButton("Delete Student", ConfigurationConstants.SSErrorRed, new Point(40, 25));
            btnDelete.Click += BtnDelete_Click;
            btnDelete.Enabled = false;
            btnDelete.ForeColor = ConfigurationConstants.SSWhiteColor;

            btnCancel = UIHelper.CreateButton("Cancel", ConfigurationConstants.SSDarkBlueColor, new Point(260, 25));
            btnCancel.Click += BtnCancel_Click;
            btnCancel.Enabled = false;
            btnCancel.ForeColor = ConfigurationConstants.SSWhiteColor;

            buttonPanel.Controls.Add(btnDelete);
            buttonPanel.Controls.Add(btnCancel);
            this.Controls.Add(buttonPanel);
        }

        private void InitializeSearchControls()
        {
            int yPos = 20;

            lblSearchStudent = new Label
            {
                Text = "Search Student:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(120, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtSearchStudent = new TextBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(150, yPos),
                Size = new Size(300, 40),
                ForeColor = Color.Gray,
                Text = "Enter Roll Number or CNIC...",
                Tag = "Enter Roll Number or CNIC..."
            };

            txtSearchStudent.Enter += TxtSearchStudent_Enter;
            txtSearchStudent.Leave += TxtSearchStudent_Leave;
            txtSearchStudent.KeyPress += TxtSearchStudent_KeyPress;

            btnSearchStudent = UIHelper.CreateButton("Search", ConfigurationConstants.SSDarkNavyColor, new Point(470, yPos), new Size(120, 40));
            btnSearchStudent.Click += BtnSearchStudent_Click;

            btnClearSearch = UIHelper.CreateButton("Clear", ConfigurationConstants.SSDarkBlueColor, new Point(610, yPos), new Size(120, 40));
            btnClearSearch.Click += BtnClearSearch_Click;

            lblSearchHint = new Label
            {
                Text = "? Warning: Deleting a student will permanently remove all their data",
                Font = new Font("Arial", 9, FontStyle.Italic),
                ForeColor = ConfigurationConstants.SSErrorRed,
                Location = new Point(150, yPos + 45),
                Size = new Size(600, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblCurrentStudent = new Label
            {
                Text = "No student loaded",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSErrorRed,
                Location = new Point(750, yPos),
                Size = new Size(300, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            searchPanel.Controls.Add(lblSearchStudent);
            searchPanel.Controls.Add(txtSearchStudent);
            searchPanel.Controls.Add(btnSearchStudent);
            searchPanel.Controls.Add(btnClearSearch);
            searchPanel.Controls.Add(lblSearchHint);
            searchPanel.Controls.Add(lblCurrentStudent);
        }

        private void TxtSearchStudent_Enter(object sender, EventArgs e)
        {
            if (txtSearchStudent.Text == txtSearchStudent.Tag.ToString())
            {
                txtSearchStudent.Text = "";
                txtSearchStudent.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            }
        }

        private void TxtSearchStudent_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearchStudent.Text))
            {
                txtSearchStudent.Text = txtSearchStudent.Tag.ToString();
                txtSearchStudent.ForeColor = Color.Gray;
            }
        }

        private void TxtSearchStudent_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSearchStudent.PerformClick();
                e.Handled = true;
            }
        }

        private void InitializeDetailsPanel()
        {
            lblDetailsTitle = new Label
            {
                Text = "Student Details",
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(0, 0),
                Size = new Size(300, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblWarning = new Label
            {
                Text = "? Review the information below before deleting this student",
                Font = new Font("Arial", 10, FontStyle.Italic),
                ForeColor = ConfigurationConstants.SSErrorRed,
                Location = new Point(0, 45),
                Size = new Size(600, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            dgvStudentDetails = new DataGridView
            {
                Location = new Point(0, 80),
                Size = new Size(ConfigurationConstants.ScreenWidth - 400, ConfigurationConstants.ScreenHeight - 600),
                BackgroundColor = ConfigurationConstants.SSWhiteColor,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowTemplate = { Height = 35 }
            };

            dgvStudentDetails.DefaultCellStyle.Font = new Font("Arial", 11, FontStyle.Regular);
            dgvStudentDetails.DefaultCellStyle.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            dgvStudentDetails.DefaultCellStyle.SelectionBackColor = ConfigurationConstants.SSLightNavyColor;
            dgvStudentDetails.DefaultCellStyle.SelectionForeColor = ConfigurationConstants.SSWhiteColor;
            dgvStudentDetails.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);
            dgvStudentDetails.AlternatingRowsDefaultCellStyle.BackColor = ConfigurationConstants.SSLightGrayBackground;
            dgvStudentDetails.GridColor = ConfigurationConstants.SSBorderGray;

            detailsPanel.Controls.Add(lblDetailsTitle);
            detailsPanel.Controls.Add(lblWarning);
            detailsPanel.Controls.Add(dgvStudentDetails);
        }

        private void BtnSearchStudent_Click(object sender, EventArgs e)
        {
            if (UIHelper.IsPlaceholderText(txtSearchStudent))
            {
                MessageBox.Show("Please enter a Roll Number or CNIC to search.", "Search Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSearchStudent.Focus();
                return;
            }

            string searchTerm = txtSearchStudent.Text.Trim();
            LoadStudentData(searchTerm);
        }

        private void LoadStudentData(string searchTerm)
        {
            loadingIndicator.Show("Searching Student");

            try
            {
                // Use repository to search for student
                StudentViewModel student = studentRepository.SearchStudent(searchTerm);

                if (student != null)
                {
                    currentStudentId = student.Id;
                    currentUserId = student.UserId;
                    studentLoaded = true;

                    // Create details table
                    studentDetailsTable = new DataTable();
                    studentDetailsTable.Columns.Add("Field", typeof(string));
                    studentDetailsTable.Columns.Add("Value", typeof(string));

                    // Add student data from StudentViewModel
                    studentDetailsTable.Rows.Add("CNIC", student.CNIC);
                    studentDetailsTable.Rows.Add("Roll Number", student.RollNumber);
                    studentDetailsTable.Rows.Add("Full Name", student.FullName);
                    studentDetailsTable.Rows.Add("Date of Birth", student.DateOfBirth.ToString("dd-MMM-yyyy"));
                    studentDetailsTable.Rows.Add("Gender", student.Gender);
                    studentDetailsTable.Rows.Add("Blood Group", !string.IsNullOrEmpty(student.BloodGroup) ? student.BloodGroup : "N/A");
                    studentDetailsTable.Rows.Add("Religion", !string.IsNullOrEmpty(student.Religion) ? student.Religion : "N/A");
                    studentDetailsTable.Rows.Add("Nationality", !string.IsNullOrEmpty(student.Nationality) ? student.Nationality : "N/A");
                    studentDetailsTable.Rows.Add("Domicile District", !string.IsNullOrEmpty(student.DomicileDistrict) ? student.DomicileDistrict : "N/A");
                    studentDetailsTable.Rows.Add("Mobile No", student.MobileNo);
                    studentDetailsTable.Rows.Add("WhatsApp No", !string.IsNullOrEmpty(student.WhatsappNo) ? student.WhatsappNo : "N/A");
                    studentDetailsTable.Rows.Add("Program", student.ProgramName ?? "N/A");
                    studentDetailsTable.Rows.Add("Email", !string.IsNullOrEmpty(student.Email) ? student.Email : "N/A");

                    dgvStudentDetails.DataSource = studentDetailsTable;

                    // Style the columns
                    if (dgvStudentDetails.Columns["Field"] != null)
                    {
                        dgvStudentDetails.Columns["Field"].DefaultCellStyle.Font = new Font("Arial", 11, FontStyle.Bold);
                        dgvStudentDetails.Columns["Field"].DefaultCellStyle.ForeColor = ConfigurationConstants.SSDarkNavyColor;
                        dgvStudentDetails.Columns["Field"].FillWeight = 40;
                    }

                    if (dgvStudentDetails.Columns["Value"] != null)
                    {
                        dgvStudentDetails.Columns["Value"].FillWeight = 60;
                    }

                    detailsPanel.Visible = true;
                    btnDelete.Enabled = true;
                    btnDelete.ForeColor = ConfigurationConstants.SSWhiteColor;
                    btnCancel.Enabled = true;
                    btnCancel.ForeColor = ConfigurationConstants.SSWhiteColor;

                    lblCurrentStudent.Text = $"Student: {student.FullName} ({student.RollNumber})";
                    lblCurrentStudent.ForeColor = ConfigurationConstants.SSWarningOrange;

                    loadingIndicator.Hide();
                    MessageBox.Show("Student data loaded successfully!\n\nReview the details carefully before deleting.",
                        "Student Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    loadingIndicator.Hide();
                    MessageBox.Show("No student found with this Roll Number or CNIC.", "Not Found",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                loadingIndicator.Hide();
                MessageBox.Show($"Error loading student: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClearSearch_Click(object sender, EventArgs e)
        {
            UIHelper.ClearToPlaceholder(txtSearchStudent);
            ClearStudentData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (!studentLoaded)
            {
                MessageBox.Show("Please search and load a student first.", "No Student Loaded", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(currentStudentId) || string.IsNullOrEmpty(currentUserId))
            {
                MessageBox.Show($"Error: Student ID or User ID is empty.", "Data Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult result = MessageBox.Show(
                "?? WARNING ??\n\n" +
                "You are about to permanently delete this student and ALL associated data.\n\n" +
                "This action CANNOT be undone!\n\n" +
                "Are you absolutely sure you want to continue?",
                "Confirm Permanent Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                // Double confirmation for critical action
                DialogResult confirmAgain = MessageBox.Show(
                    "This is your final confirmation.\n\n" +
                    "Click YES to permanently delete this student.\n" +
                    "Click NO to cancel.",
                    "Final Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2);

                if (confirmAgain == DialogResult.Yes)
                {
                    DeleteStudent();
                }
            }
        }

        private void DeleteStudent()
        {
            loadingIndicator.Show("Deleting Student");

            try
            {
                // Use repository to delete student
                bool success = studentRepository.DeleteStudent(currentStudentId, currentUserId);

                loadingIndicator.Hide();

                if (success)
                {
                    MessageBox.Show(
                        "Student deleted successfully!\n\n" +
                        "All associated data has been permanently removed.",
                        "Deletion Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearStudentData();
                }
            }
            catch (Exception ex)
            {
                loadingIndicator.Hide();

                MessageBox.Show($"Error deleting student: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (!studentLoaded)
            {
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to cancel?\n\n" +
                "The student data will be cleared but not deleted.",
                "Cancel Operation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                ClearStudentData();
            }
        }

        private void ClearStudentData()
        {
            detailsPanel.Visible = false;
            btnDelete.Enabled = false;
            btnDelete.ForeColor = ConfigurationConstants.SSWhiteColor;
            btnCancel.Enabled = false;
            btnCancel.ForeColor = ConfigurationConstants.SSWhiteColor;
            lblCurrentStudent.Text = "No student loaded";
            lblCurrentStudent.ForeColor = ConfigurationConstants.SSErrorRed;
            studentLoaded = false;
            currentStudentId = "";
            currentUserId = "";
            
            if (studentDetailsTable != null)
            {
                studentDetailsTable.Clear();
            }
        }
    }
}
