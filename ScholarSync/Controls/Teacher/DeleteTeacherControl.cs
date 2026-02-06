using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ScholarSync.Commons;
using ScholarSync.Models;
using ScholarSync.Repositories;

namespace ScholarSync.Controls.Teacher
{
    public partial class DeleteTeacherControl : UserControl
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Panel searchPanel;
        private Panel detailsPanel;
        private Panel buttonPanel;
        
        // Search Controls
        private Label lblSearchTeacher;
        private TextBox txtSearchTeacher;
        private Button btnSearchTeacher;
        private Button btnClearSearch;
        private Label lblSearchHint;
        
        // Teacher Information Display
        private Label lblCurrentTeacher;
        
        // Details Display Controls
        private Label lblDetailsTitle;
        private DataGridView dgvTeacherDetails;
        
        // Buttons
        private Button btnDelete;
        private Button btnCancel;
        
        // State
        private string currentTeacherId = "";
        private string currentUserId = "";
        private bool teacherLoaded = false;
        private DataTable teacherDetailsTable;

        // Loading Indicator
        private LoadingIndicator loadingIndicator;

        // Repository
        private TeacherRepository teacherRepository;

        public DeleteTeacherControl()
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
            teacherRepository = new TeacherRepository();
            
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
                Text = "Only Administrators can delete teachers.\n\nPlease contact your system administrator for access.",
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
                Text = "Delete Teacher",
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

            btnDelete = UIHelper.CreateButton("Delete Teacher", ConfigurationConstants.SSErrorRed, new Point(40, 25));
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

            lblSearchTeacher = new Label
            {
                Text = "Search Teacher:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(120, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtSearchTeacher = new TextBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(150, yPos),
                Size = new Size(300, 40),
                ForeColor = Color.Gray,
                Text = "Enter CNIC or Name...",
                Tag = "Enter CNIC or Name..."
            };

            txtSearchTeacher.Enter += TxtSearchTeacher_Enter;
            txtSearchTeacher.Leave += TxtSearchTeacher_Leave;
            txtSearchTeacher.KeyPress += TxtSearchTeacher_KeyPress;

            btnSearchTeacher = UIHelper.CreateButton("Search", ConfigurationConstants.SSDarkNavyColor, new Point(470, yPos), new Size(120, 40));
            btnSearchTeacher.Click += BtnSearchTeacher_Click;

            btnClearSearch = UIHelper.CreateButton("Clear", ConfigurationConstants.SSDarkBlueColor, new Point(610, yPos), new Size(120, 40));
            btnClearSearch.Click += BtnClearSearch_Click;

            lblSearchHint = new Label
            {
                Text = "? Warning: Deleting a teacher will permanently remove all their data",
                Font = new Font("Arial", 9, FontStyle.Italic),
                ForeColor = ConfigurationConstants.SSErrorRed,
                Location = new Point(150, yPos + 45),
                Size = new Size(600, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblCurrentTeacher = new Label
            {
                Text = "No teacher loaded",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSErrorRed,
                Location = new Point(750, yPos),
                Size = new Size(300, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            searchPanel.Controls.Add(lblSearchTeacher);
            searchPanel.Controls.Add(txtSearchTeacher);
            searchPanel.Controls.Add(btnSearchTeacher);
            searchPanel.Controls.Add(btnClearSearch);
            searchPanel.Controls.Add(lblSearchHint);
            searchPanel.Controls.Add(lblCurrentTeacher);
        }

        private void TxtSearchTeacher_Enter(object sender, EventArgs e)
        {
            if (txtSearchTeacher.Text == txtSearchTeacher.Tag.ToString())
            {
                txtSearchTeacher.Text = "";
                txtSearchTeacher.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            }
        }

        private void TxtSearchTeacher_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearchTeacher.Text))
            {
                txtSearchTeacher.Text = txtSearchTeacher.Tag.ToString();
                txtSearchTeacher.ForeColor = Color.Gray;
            }
        }

        private void TxtSearchTeacher_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSearchTeacher.PerformClick();
                e.Handled = true;
            }
        }

        private void InitializeDetailsPanel()
        {
            lblDetailsTitle = new Label
            {
                Text = "Teacher Details",
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(0, 0),
                Size = new Size(300, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblWarning = new Label
            {
                Text = "? Review the information below before deleting this teacher",
                Font = new Font("Arial", 10, FontStyle.Italic),
                ForeColor = ConfigurationConstants.SSErrorRed,
                Location = new Point(0, 45),
                Size = new Size(600, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            dgvTeacherDetails = new DataGridView
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

            dgvTeacherDetails.DefaultCellStyle.Font = new Font("Arial", 11, FontStyle.Regular);
            dgvTeacherDetails.DefaultCellStyle.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            dgvTeacherDetails.DefaultCellStyle.SelectionBackColor = ConfigurationConstants.SSLightNavyColor;
            dgvTeacherDetails.DefaultCellStyle.SelectionForeColor = ConfigurationConstants.SSWhiteColor;
            dgvTeacherDetails.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);
            dgvTeacherDetails.AlternatingRowsDefaultCellStyle.BackColor = ConfigurationConstants.SSLightGrayBackground;
            dgvTeacherDetails.GridColor = ConfigurationConstants.SSBorderGray;

            detailsPanel.Controls.Add(lblDetailsTitle);
            detailsPanel.Controls.Add(lblWarning);
            detailsPanel.Controls.Add(dgvTeacherDetails);
        }

        private void BtnSearchTeacher_Click(object sender, EventArgs e)
        {
            if (UIHelper.IsPlaceholderText(txtSearchTeacher))
            {
                MessageBox.Show("Please enter a CNIC or Name to search.", "Search Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSearchTeacher.Focus();
                return;
            }

            string searchTerm = txtSearchTeacher.Text.Trim();
            LoadTeacherData(searchTerm);
        }

        private void LoadTeacherData(string searchTerm)
        {
            loadingIndicator.Show("Searching Teacher");

            try
            {
                // Use repository to search for teacher
                TeacherViewModel teacher = teacherRepository.SearchTeacher(searchTerm);

                if (teacher != null)
                {
                    currentTeacherId = teacher.Id;
                    currentUserId = teacher.UserId;
                    teacherLoaded = true;

                    // Create details table
                    teacherDetailsTable = new DataTable();
                    teacherDetailsTable.Columns.Add("Field", typeof(string));
                    teacherDetailsTable.Columns.Add("Value", typeof(string));

                    // Add teacher data from TeacherViewModel
                    teacherDetailsTable.Rows.Add("CNIC", teacher.CNIC);
                    teacherDetailsTable.Rows.Add("Full Name", teacher.FullName ?? "N/A");
                    teacherDetailsTable.Rows.Add("Designation", !string.IsNullOrEmpty(teacher.Designation) ? teacher.Designation : "N/A");
                    teacherDetailsTable.Rows.Add("Specialization", !string.IsNullOrEmpty(teacher.Specialization) ? teacher.Specialization : "N/A");
                    teacherDetailsTable.Rows.Add("Email", !string.IsNullOrEmpty(teacher.Email) ? teacher.Email : "N/A");

                    dgvTeacherDetails.DataSource = teacherDetailsTable;

                    // Style the columns
                    if (dgvTeacherDetails.Columns["Field"] != null)
                    {
                        dgvTeacherDetails.Columns["Field"].DefaultCellStyle.Font = new Font("Arial", 11, FontStyle.Bold);
                        dgvTeacherDetails.Columns["Field"].DefaultCellStyle.ForeColor = ConfigurationConstants.SSDarkNavyColor;
                        dgvTeacherDetails.Columns["Field"].FillWeight = 40;
                    }

                    if (dgvTeacherDetails.Columns["Value"] != null)
                    {
                        dgvTeacherDetails.Columns["Value"].FillWeight = 60;
                    }

                    detailsPanel.Visible = true;
                    btnDelete.Enabled = true;
                    btnDelete.ForeColor = ConfigurationConstants.SSWhiteColor;
                    btnCancel.Enabled = true;
                    btnCancel.ForeColor = ConfigurationConstants.SSWhiteColor;

                    lblCurrentTeacher.Text = $"Teacher: {teacher.FullName} ({teacher.CNIC})";
                    lblCurrentTeacher.ForeColor = ConfigurationConstants.SSWarningOrange;

                    loadingIndicator.Hide();
                    MessageBox.Show("Teacher data loaded successfully!\n\nReview the details carefully before deleting.",
                        "Teacher Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    loadingIndicator.Hide();
                    MessageBox.Show("No teacher found with this CNIC or Name.", "Not Found",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                loadingIndicator.Hide();
                MessageBox.Show($"Error loading teacher: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClearSearch_Click(object sender, EventArgs e)
        {
            UIHelper.ClearToPlaceholder(txtSearchTeacher);
            ClearTeacherData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (!teacherLoaded)
            {
                MessageBox.Show("Please search and load a teacher first.", "No Teacher Loaded", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "? WARNING ?\n\n" +
                "You are about to permanently delete this teacher and ALL associated data.\n\n" +
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
                    "Click YES to permanently delete this teacher.\n" +
                    "Click NO to cancel.",
                    "Final Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2);

                if (confirmAgain == DialogResult.Yes)
                {
                    DeleteTeacher();
                }
            }
        }

        private void DeleteTeacher()
        {
            loadingIndicator.Show("Deleting Teacher");

            try
            {
                // Use repository to delete teacher
                bool success = teacherRepository.DeleteTeacher(currentTeacherId, currentUserId);

                loadingIndicator.Hide();

                if (success)
                {
                    MessageBox.Show(
                        "Teacher deleted successfully!\n\n" +
                        "All associated data has been permanently removed.",
                        "Deletion Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearTeacherData();
                }
            }
            catch (Exception ex)
            {
                loadingIndicator.Hide();

                MessageBox.Show($"Error deleting teacher: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (!teacherLoaded)
            {
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to cancel?\n\n" +
                "The teacher data will be cleared but not deleted.",
                "Cancel Operation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                ClearTeacherData();
            }
        }

        private void ClearTeacherData()
        {
            detailsPanel.Visible = false;
            btnDelete.Enabled = false;
            btnDelete.ForeColor = ConfigurationConstants.SSWhiteColor;
            btnCancel.Enabled = false;
            btnCancel.ForeColor = ConfigurationConstants.SSWhiteColor;
            lblCurrentTeacher.Text = "No teacher loaded";
            lblCurrentTeacher.ForeColor = ConfigurationConstants.SSErrorRed;
            teacherLoaded = false;
            currentTeacherId = "";
            currentUserId = "";
            
            if (teacherDetailsTable != null)
            {
                teacherDetailsTable.Clear();
            }
        }
    }
}
