using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ScholarSync.Commons;
using ScholarSync.Models;
using ScholarSync.Repositories;

namespace ScholarSync.Controls.Teacher
{
    public partial class UpdateTeacherControl : UserControl
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Panel searchPanel;
        private Panel formPanel;
        private Panel buttonPanel;
        
        // Search Controls
        private Label lblSearchTeacher;
        private TextBox txtSearchTeacher;
        private Button btnSearchTeacher;
        private Button btnClearSearch;
        private Label lblSearchHint;
        
        // Teacher Information Display
        private Label lblCurrentTeacher;
        
        // Form Controls - Personal Information
        private TextBox txtCNIC;
        private TextBox txtFullName;
        
        // Professional Information
        private TextBox txtDesignation;
        private TextBox txtSpecialization;
        
        // Account Information
        private TextBox txtEmail;
        private CheckBox chkChangePassword;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        
        // Buttons
        private Button btnUpdate;
        private Button btnReset;
        
        // State
        private string currentTeacherId = "";
        private string currentUserId = "";
        private bool teacherLoaded = false;

        // Loading Indicator
        private LoadingIndicator loadingIndicator;

        // Repository
        private TeacherRepository teacherRepository;

        public UpdateTeacherControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.BackColor = ConfigurationConstants.SSWhiteColor;
            this.AutoScroll = true;
            
            // Check if user is Admin or Teacher
            if (!SessionManager.IsAdmin() && !SessionManager.IsTeacher())
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
            
            // Apply role-based access control
            ApplyRoleBasedAccess();
        }

        private void ApplyRoleBasedAccess()
        {
            if (SessionManager.IsAdmin())
            {
                // Admin has full access
                titleLabel.Text = "Update Teacher Information (Administrator)";
            }
            else if (SessionManager.IsTeacher())
            {
                // Teacher can only view - disable update button
                titleLabel.Text = "View Teacher Information (Teacher - Read Only)";
                btnUpdate.Enabled = false;
                btnReset.Text = "Refresh";
            }
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
                Text = "You do not have permission to access teacher information.\n\nPlease contact your system administrator.",
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
                Text = "Update Teacher Information",
                Font = new Font("Arial", 22, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(30, 0),
                Size = new Size(600, 80),
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

            // Form Panel
            formPanel = new Panel
            {
                Dock = DockStyle.None,
                BackColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(10, 240),
                Size = new Size(ConfigurationConstants.ScreenWidth - 300, ConfigurationConstants.ScreenHeight - 420),
                AutoScroll = true,
                Padding = new Padding(40, 30, 40, 30),
                Enabled = false
            };
            
            CreateFormControls();
            this.Controls.Add(formPanel);

            // Button Panel
            buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 100,
                BackColor = ConfigurationConstants.SSLightGrayBackground,
                Padding = new Padding(40, 25, 40, 25)
            };

            btnUpdate = UIHelper.CreateButton("Update Teacher", ConfigurationConstants.SSDarkNavyColor, new Point(40, 25));
            btnUpdate.Click += BtnUpdate_Click;
            btnUpdate.Enabled = false;
            btnUpdate.ForeColor = ConfigurationConstants.SSWhiteColor;

            btnReset = UIHelper.CreateButton("Reset Form", ConfigurationConstants.SSDarkBlueColor, new Point(260, 25));
            btnReset.Click += BtnReset_Click;
            btnReset.Enabled = false;
            btnReset.ForeColor = ConfigurationConstants.SSWhiteColor;

            buttonPanel.Controls.Add(btnUpdate);
            buttonPanel.Controls.Add(btnReset);
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
                Text = "Search by CNIC or Name to load teacher information",
                Font = new Font("Arial", 9, FontStyle.Italic),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(150, yPos + 45),
                Size = new Size(400, 20),
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

        private void CreateFormControls()
        {
            int yPosition = 0;
            int leftColumnX = 0;
            int rightColumnX = UIHelper.LABEL_WIDTH + UIHelper.CONTROL_WIDTH + UIHelper.HORIZONTAL_SPACING + UIHelper.COLUMN_SPACING;

            // Personal Information Section
            yPosition = UIHelper.CreateSectionHeader(formPanel, "Personal Information", yPosition);
            
            UIHelper.CreateFormRow(formPanel, "CNIC:", ref txtCNIC, leftColumnX, yPosition, "12345-6789012-3");
            UIHelper.CreateFormRow(formPanel, "Full Name:", ref txtFullName, rightColumnX, yPosition, "Enter full name");
            yPosition += UIHelper.VERTICAL_SPACING + UIHelper.SECTION_SPACING;

            // Professional Information Section
            yPosition = UIHelper.CreateSectionHeader(formPanel, "Professional Information", yPosition);
            
            UIHelper.CreateFormRow(formPanel, "Designation:", ref txtDesignation, leftColumnX, yPosition, "e.g. Professor, Lecturer");
            UIHelper.CreateFormRow(formPanel, "Specialization:", ref txtSpecialization, rightColumnX, yPosition, "e.g. Machine Learning");
            yPosition += UIHelper.VERTICAL_SPACING + UIHelper.SECTION_SPACING;

            // Account Information Section
            yPosition = UIHelper.CreateSectionHeader(formPanel, "Account Information", yPosition);
            
            UIHelper.CreateFormRow(formPanel, "Email:", ref txtEmail, leftColumnX, yPosition, "teacher@example.com");
            UIHelper.CreateCheckBoxRow(formPanel, "Change Password:", ref chkChangePassword, rightColumnX, yPosition);
            chkChangePassword.CheckedChanged += ChkChangePassword_CheckedChanged;
            yPosition += UIHelper.VERTICAL_SPACING;

            UIHelper.CreatePasswordRow(formPanel, "New Password:", ref txtPassword, leftColumnX, yPosition);
            txtPassword.Enabled = false;
            UIHelper.CreatePasswordRow(formPanel, "Confirm Password:", ref txtConfirmPassword, rightColumnX, yPosition);
            txtConfirmPassword.Enabled = false;
            yPosition += UIHelper.VERTICAL_SPACING + UIHelper.SECTION_SPACING;
        }

        private void ChkChangePassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.Enabled = chkChangePassword.Checked;
            txtConfirmPassword.Enabled = chkChangePassword.Checked;
            
            if (!chkChangePassword.Checked)
            {
                txtPassword.Clear();
                txtConfirmPassword.Clear();
            }
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

                    // Populate form fields with teacher data
                    PopulateFormFields(teacher);

                    formPanel.Enabled = true;
                    btnUpdate.Enabled = SessionManager.IsAdmin();
                    btnReset.Enabled = true;

                    lblCurrentTeacher.Text = $"Current: {teacher.FullName} ({teacher.CNIC})";
                    lblCurrentTeacher.ForeColor = ConfigurationConstants.SSSuccessGreen;

                    loadingIndicator.Hide();
                    MessageBox.Show("Teacher data loaded successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void PopulateFormFields(TeacherViewModel teacher)
        {
            txtCNIC.Text = teacher.CNIC;
            txtCNIC.ForeColor = ConfigurationConstants.SSDarkBlueColor;

            txtFullName.Text = teacher.FullName;
            txtFullName.ForeColor = ConfigurationConstants.SSDarkBlueColor;

            if (!string.IsNullOrEmpty(teacher.Designation))
            {
                txtDesignation.Text = teacher.Designation;
                txtDesignation.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            }

            if (!string.IsNullOrEmpty(teacher.Specialization))
            {
                txtSpecialization.Text = teacher.Specialization;
                txtSpecialization.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            }

            if (!string.IsNullOrEmpty(teacher.Email))
            {
                txtEmail.Text = teacher.Email;
                txtEmail.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            }
        }

        private void BtnClearSearch_Click(object sender, EventArgs e)
        {
            UIHelper.ClearToPlaceholder(txtSearchTeacher);
            ClearForm();
            formPanel.Enabled = false;
            btnUpdate.Enabled = false;
            btnUpdate.ForeColor = ConfigurationConstants.SSWhiteColor;
            btnReset.Enabled = false;
            btnReset.ForeColor = ConfigurationConstants.SSWhiteColor;
            lblCurrentTeacher.Text = "No teacher loaded";
            lblCurrentTeacher.ForeColor = ConfigurationConstants.SSErrorRed;
            teacherLoaded = false;
            currentTeacherId = "";
            currentUserId = "";
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (!teacherLoaded)
            {
                MessageBox.Show("Please search and load a teacher first.", "No Teacher Loaded", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!SessionManager.IsAdmin())
            {
                MessageBox.Show("Only Administrators can update teacher information.", "Access Denied",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ValidateForm())
            {
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to update this teacher's information?",
                    "Confirm Update",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    UpdateTeacher();
                }
            }
        }

        private void UpdateTeacher()
        {
            loadingIndicator.Show("Updating Teacher");

            try
            {
                // Build TeacherModel from form data
                TeacherModel teacher = new TeacherModel
                {
                    Id = currentTeacherId,
                    UserId = currentUserId,
                    CNIC = txtCNIC.Text.Replace("-", "").Trim(),
                    FullName = txtFullName.Text.Trim(),
                    Designation = UIHelper.IsPlaceholderText(txtDesignation) ? null : txtDesignation.Text.Trim(),
                    Specialization = UIHelper.IsPlaceholderText(txtSpecialization) ? null : txtSpecialization.Text.Trim()
                };

                string email = txtEmail.Text.Trim();
                string newPassword = null;
                if (chkChangePassword.Checked && !string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    newPassword = txtPassword.Text;
                }

                // Use repository to update teacher
                bool success = teacherRepository.UpdateTeacher(teacher, email, newPassword);

                loadingIndicator.Hide();

                if (success)
                {
                    MessageBox.Show("Teacher information updated successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    chkChangePassword.Checked = false;
                    txtPassword.Clear();
                    txtConfirmPassword.Clear();
                }
            }
            catch (Exception ex)
            {
                loadingIndicator.Hide();
                MessageBox.Show($"Error updating teacher: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (!teacherLoaded)
            {
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to reset the form? All changes will be lost.",
                "Reset Form",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                LoadTeacherData(txtCNIC.Text.Trim());
            }
        }

        private bool ValidateForm()
        {
            if (UIHelper.IsPlaceholderText(txtCNIC))
            {
                MessageBox.Show("Please enter CNIC.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCNIC.Focus();
                return false;
            }

            if (UIHelper.IsPlaceholderText(txtFullName))
            {
                MessageBox.Show("Please enter Full Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return false;
            }

            if (UIHelper.IsPlaceholderText(txtEmail))
            {
                MessageBox.Show("Please enter Email.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            if (chkChangePassword.Checked)
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Please enter Password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return false;
                }

                if (txtPassword.Text.Length < 6)
                {
                    MessageBox.Show("Password must be at least 6 characters long.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return false;
                }

                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    MessageBox.Show("Password and Confirm Password do not match.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtConfirmPassword.Focus();
                    return false;
                }
            }

            return true;
        }

        private void ClearForm()
        {
            UIHelper.ClearToPlaceholder(txtCNIC);
            UIHelper.ClearToPlaceholder(txtFullName);
            UIHelper.ClearToPlaceholder(txtDesignation);
            UIHelper.ClearToPlaceholder(txtSpecialization);
            UIHelper.ClearToPlaceholder(txtEmail);
            chkChangePassword.Checked = false;
            txtPassword.Clear();
            txtConfirmPassword.Clear();
        }
    }
}
