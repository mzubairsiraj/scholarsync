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
    public partial class UpdateStudentControl : UserControl
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Panel searchPanel;
        private Panel formPanel;
        private Panel buttonPanel;
        
        // Search Controls
        private Label lblSearchStudent;
        private TextBox txtSearchStudent;
        private Button btnSearchStudent;
        private Button btnClearSearch;
        private Label lblSearchHint;
        
        // Student Information Display
        private Label lblCurrentStudent;
        
        // Form Controls - Personal Information
        private TextBox txtCNIC;
        private TextBox txtRollNumber;
        private TextBox txtFullName;
        private DateTimePicker dtpDateOfBirth;
        private ComboBox cmbGender;
        private TextBox txtBloodGroup;
        private TextBox txtReligion;
        private TextBox txtNationality;
        private TextBox txtDomicileDistrict;
        private TextBox txtMobileNo;
        private TextBox txtWhatsAppNo;
        private ComboBox cmbProgram;
        
        // Account Information
        private TextBox txtEmail;
        private CheckBox chkChangePassword;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        
        // Guardian Information
        private ComboBox cmbGuardianRelation;
        private TextBox txtGuardianName;
        private TextBox txtGuardianCNIC;
        private TextBox txtGuardianMobile;
        private TextBox txtGuardianAddress;
        private CheckBox chkIsDeceased;
        
        // Buttons
        private Button btnUpdate;
        private Button btnReset;
        
        // State
        private string currentStudentId = "";
        private string currentUserId = "";
        private bool studentLoaded = false;

        // Loading Indicator
        private LoadingIndicator loadingIndicator;

        // Repository
        private StudentRepository studentRepository;

        public UpdateStudentControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.BackColor = ConfigurationConstants.SSWhiteColor;
            this.AutoScroll = true;
            
            // Initialize repository
            studentRepository = new StudentRepository();
            
            InitializeUI();
            
            // Initialize loading indicator
            loadingIndicator = new LoadingIndicator();
            this.Controls.Add(loadingIndicator);
            
            LoadPrograms();
            
            // Apply role-based access control
            ApplyRoleBasedAccess();
        }

        private void ApplyRoleBasedAccess()
        {
            if (SessionManager.IsAdmin())
            {
                // Admin has full access - all buttons enabled when student is loaded
                titleLabel.Text = "Update Student Information (Administrator)";
            }
            else if (SessionManager.IsTeacher())
            {
                // Teacher can only view - disable update button
                titleLabel.Text = "View Student Information (Teacher - Read Only)";
                btnUpdate.Enabled = false;
                btnReset.Text = "Refresh";
                
                // Expand header to show warning
                headerPanel.Height = 120;
                
                // Adjust positions of other panels
                searchPanel.Location = new Point(10, 140);
                formPanel.Location = new Point(10, 280);
                formPanel.Size = new Size(ConfigurationConstants.ScreenWidth - 300, ConfigurationConstants.ScreenHeight - 460);
                
                // Create warning banner below title
                Label lblReadOnly = new Label
                {
                    Text = "?? Read-Only Mode: Teachers can view student information but cannot make changes.",
                    Font = new Font("Arial", 11, FontStyle.Bold),
                    ForeColor = Color.DarkOrange,
                    BackColor = Color.FromArgb(255, 250, 205),
                    Location = new Point(30, 70),
                    Size = new Size(ConfigurationConstants.ScreenWidth - 360, 35),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(15, 8, 15, 8),
                    AutoSize = false
                };
                
                headerPanel.Controls.Add(lblReadOnly);
                lblReadOnly.BringToFront();
            }
            else
            {
                // No access for other roles
                ShowAccessDenied();
            }
        }

        private void MakeFormReadOnly()
        {
            // Disable all input controls for teachers
            txtCNIC.ReadOnly = true;
            txtRollNumber.ReadOnly = true;
            txtFullName.ReadOnly = true;
            dtpDateOfBirth.Enabled = false;
            cmbGender.Enabled = false;
            txtBloodGroup.ReadOnly = true;
            txtReligion.ReadOnly = true;
            txtNationality.ReadOnly = true;
            txtDomicileDistrict.ReadOnly = true;
            txtMobileNo.ReadOnly = true;
            txtWhatsAppNo.ReadOnly = true;
            cmbProgram.Enabled = false;
            
            txtEmail.ReadOnly = true;
            chkChangePassword.Enabled = false;
            txtPassword.Enabled = false;
            txtConfirmPassword.Enabled = false;
            
            cmbGuardianRelation.Enabled = false;
            txtGuardianName.ReadOnly = true;
            txtGuardianCNIC.ReadOnly = true;
            txtGuardianMobile.ReadOnly = true;
            txtGuardianAddress.ReadOnly = true;
            chkIsDeceased.Enabled = false;
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
                Text = "You do not have permission to access student information.\n\nPlease contact your system administrator.",
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

        private void LoadPrograms()
        {
            loadingIndicator.Show("Loading Programs");

            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT id, name FROM programs WHERE is_active = true ORDER BY name";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            cmbProgram.Items.Clear();

                            while (reader.Read())
                            {
                                cmbProgram.Items.Add(new ComboBoxItem
                                {
                                    Text = reader["name"].ToString(),
                                    Value = reader["id"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading programs: {ex.Message}",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                loadingIndicator.Hide();
            }
        }

        private class ComboBoxItem
        {
            public string Text { get; set; }
            public string Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
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
                Text = "Update Student Information",
                Font = new Font("Arial", 22, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(30, 20),
                Size = new Size(800, 40),
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

            btnUpdate = UIHelper.CreateButton("Update Student", ConfigurationConstants.SSDarkNavyColor, new Point(40, 25));
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
                Text = "Search by Roll Number or CNIC to load student information",
                Font = new Font("Arial", 9, FontStyle.Italic),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(150, yPos + 45),
                Size = new Size(400, 20),
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

        private void CreateFormControls()
        {
            int yPosition = 0;
            int leftColumnX = 0;
            int rightColumnX = UIHelper.LABEL_WIDTH + UIHelper.CONTROL_WIDTH + UIHelper.HORIZONTAL_SPACING + UIHelper.COLUMN_SPACING;

            // Personal Information Section
            yPosition = UIHelper.CreateSectionHeader(formPanel, "Personal Information", yPosition);
            
            UIHelper.CreateFormRow(formPanel, "CNIC:", ref txtCNIC, leftColumnX, yPosition, "12345-6789012-3");
            UIHelper.CreateFormRow(formPanel, "Roll Number:", ref txtRollNumber, rightColumnX, yPosition, "2024-CS-001");
            yPosition += UIHelper.VERTICAL_SPACING;

            UIHelper.CreateFormRow(formPanel, "Full Name:", ref txtFullName, leftColumnX, yPosition, "Enter full name");
            UIHelper.CreateDatePickerRow(formPanel, "Date of Birth:", ref dtpDateOfBirth, rightColumnX, yPosition);
            yPosition += UIHelper.VERTICAL_SPACING;

            UIHelper.CreateComboBoxRow(formPanel, "Gender:", ref cmbGender, leftColumnX, yPosition, new string[] { "Male", "Female", "Other" });
            UIHelper.CreateFormRow(formPanel, "Blood Group:", ref txtBloodGroup, rightColumnX, yPosition, "A+, B+, O+, etc.");
            yPosition += UIHelper.VERTICAL_SPACING;

            UIHelper.CreateFormRow(formPanel, "Religion:", ref txtReligion, leftColumnX, yPosition, "Islam, Christian, etc.");
            UIHelper.CreateFormRow(formPanel, "Nationality:", ref txtNationality, rightColumnX, yPosition, "Pakistani");
            yPosition += UIHelper.VERTICAL_SPACING;

            UIHelper.CreateFormRow(formPanel, "Domicile:", ref txtDomicileDistrict, leftColumnX, yPosition, "District name");
            UIHelper.CreateFormRow(formPanel, "Mobile No:", ref txtMobileNo, rightColumnX, yPosition, "03XX-XXXXXXX");
            yPosition += UIHelper.VERTICAL_SPACING;

            UIHelper.CreateFormRow(formPanel, "WhatsApp No:", ref txtWhatsAppNo, leftColumnX, yPosition, "03XX-XXXXXXX");
            
            // Create Program ComboBox manually for custom item storage
            Label lblProgram = new Label
            {
                Text = "Program:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(rightColumnX, yPosition),
                Size = new Size(UIHelper.LABEL_WIDTH, UIHelper.LABEL_HEIGHT),
                TextAlign = ContentAlignment.MiddleLeft
            };
            formPanel.Controls.Add(lblProgram);

            cmbProgram = new ComboBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(rightColumnX, yPosition + UIHelper.LABEL_HEIGHT),
                Size = new Size(UIHelper.CONTROL_WIDTH, UIHelper.CONTROL_HEIGHT),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            formPanel.Controls.Add(cmbProgram);
            
            yPosition += UIHelper.VERTICAL_SPACING + UIHelper.SECTION_SPACING;

            // Account Information Section
            yPosition = UIHelper.CreateSectionHeader(formPanel, "Account Information", yPosition);
            
            UIHelper.CreateFormRow(formPanel, "Email:", ref txtEmail, leftColumnX, yPosition, "student@example.com");
            UIHelper.CreateCheckBoxRow(formPanel, "Change Password:", ref chkChangePassword, rightColumnX, yPosition);
            chkChangePassword.CheckedChanged += ChkChangePassword_CheckedChanged;
            yPosition += UIHelper.VERTICAL_SPACING;

            UIHelper.CreatePasswordRow(formPanel, "New Password:", ref txtPassword, leftColumnX, yPosition);
            txtPassword.Enabled = false;
            UIHelper.CreatePasswordRow(formPanel, "Confirm Password:", ref txtConfirmPassword, rightColumnX, yPosition);
            txtConfirmPassword.Enabled = false;
            yPosition += UIHelper.VERTICAL_SPACING + UIHelper.SECTION_SPACING;

            // Guardian Information Section
            yPosition = UIHelper.CreateSectionHeader(formPanel, "Guardian Information", yPosition);
            
            UIHelper.CreateComboBoxRow(formPanel, "Relation:", ref cmbGuardianRelation, leftColumnX, yPosition, new string[] { "Father", "Mother", "Guardian" });
            UIHelper.CreateFormRow(formPanel, "Guardian Name:", ref txtGuardianName, rightColumnX, yPosition, "Enter guardian's full name");
            yPosition += UIHelper.VERTICAL_SPACING;

            UIHelper.CreateFormRow(formPanel, "Guardian CNIC:", ref txtGuardianCNIC, leftColumnX, yPosition, "12345-6789012-3");
            UIHelper.CreateFormRow(formPanel, "Guardian Mobile:", ref txtGuardianMobile, rightColumnX, yPosition, "03XX-XXXXXXX");
            yPosition += UIHelper.VERTICAL_SPACING;

            UIHelper.CreateFormRow(formPanel, "Guardian Address:", ref txtGuardianAddress, leftColumnX, yPosition, "Enter full address");
            UIHelper.CreateCheckBoxRow(formPanel, "Is Deceased:", ref chkIsDeceased, rightColumnX, yPosition);
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

        private void BtnSearchStudent_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearchStudent.Text) || txtSearchStudent.ForeColor == Color.Gray)
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

                    // Populate form fields with student data
                    PopulateFormFields(student);

                    // Load Guardian Data
                    LoadGuardianData();

                    formPanel.Enabled = true;
                    btnUpdate.Enabled = SessionManager.IsAdmin();
                    btnReset.Enabled = true;

                    // For teachers, make form read-only
                    if (SessionManager.IsTeacher())
                    {
                        MakeFormReadOnly();
                    }

                    lblCurrentStudent.Text = $"Current: {student.FullName} ({student.RollNumber})";
                    lblCurrentStudent.ForeColor = ConfigurationConstants.SSSuccessGreen;

                    loadingIndicator.Hide();
                    MessageBox.Show("Student data loaded successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        /// <summary>
        /// Populates form fields with student data from StudentViewModel
        /// </summary>
        private void PopulateFormFields(StudentViewModel student)
        {
            // Clear placeholder styling first
            txtCNIC.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            txtRollNumber.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            txtFullName.ForeColor = ConfigurationConstants.SSDarkBlueColor;

            // Personal Information
            txtCNIC.Text = student.CNIC;
            txtRollNumber.Text = student.RollNumber;
            txtFullName.Text = student.FullName;
            dtpDateOfBirth.Value = student.DateOfBirth;
            cmbGender.SelectedItem = student.Gender;

            // Optional fields
            if (!string.IsNullOrEmpty(student.BloodGroup))
            {
                txtBloodGroup.Text = student.BloodGroup;
                txtBloodGroup.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            }
            if (!string.IsNullOrEmpty(student.Religion))
            {
                txtReligion.Text = student.Religion;
                txtReligion.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            }
            if (!string.IsNullOrEmpty(student.Nationality))
            {
                txtNationality.Text = student.Nationality;
                txtNationality.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            }
            if (!string.IsNullOrEmpty(student.DomicileDistrict))
            {
                txtDomicileDistrict.Text = student.DomicileDistrict;
                txtDomicileDistrict.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            }

            txtMobileNo.Text = student.MobileNo;
            txtMobileNo.ForeColor = ConfigurationConstants.SSDarkBlueColor;

            if (!string.IsNullOrEmpty(student.WhatsappNo))
            {
                txtWhatsAppNo.Text = student.WhatsappNo;
                txtWhatsAppNo.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            }
            if (!string.IsNullOrEmpty(student.Email))
            {
                txtEmail.Text = student.Email;
                txtEmail.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            }

            // Set program
            foreach (ComboBoxItem item in cmbProgram.Items)
            {
                if (item.Value == student.ProgramId)
                {
                    cmbProgram.SelectedItem = item;
                    break;
                }
            }
        }

        /// <summary>
        /// Loads guardian data for the current student
        /// </summary>
        private void LoadGuardianData()
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string guardianQuery = @"SELECT relation::text, full_name, cnic, mobile_no, address, is_deceased
                                           FROM guardians
                                           WHERE student_id = @student_id
                                           LIMIT 1";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(guardianQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@student_id", Guid.Parse(currentStudentId));

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cmbGuardianRelation.SelectedItem = reader["relation"].ToString();
                                txtGuardianName.Text = reader["full_name"].ToString();
                                txtGuardianName.ForeColor = ConfigurationConstants.SSDarkBlueColor;

                                if (reader["cnic"] != DBNull.Value)
                                {
                                    txtGuardianCNIC.Text = reader["cnic"].ToString();
                                    txtGuardianCNIC.ForeColor = ConfigurationConstants.SSDarkBlueColor;
                                }

                                txtGuardianMobile.Text = reader["mobile_no"].ToString();
                                txtGuardianMobile.ForeColor = ConfigurationConstants.SSDarkBlueColor;

                                if (reader["address"] != DBNull.Value)
                                {
                                    txtGuardianAddress.Text = reader["address"].ToString();
                                    txtGuardianAddress.ForeColor = ConfigurationConstants.SSDarkBlueColor;
                                }

                                chkIsDeceased.Checked = Convert.ToBoolean(reader["is_deceased"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't stop the process - guardian is optional
                System.Diagnostics.Debug.WriteLine($"Error loading guardian: {ex.Message}");
            }
        }

        private void BtnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearchStudent.Text = txtSearchStudent.Tag.ToString();
            txtSearchStudent.ForeColor = Color.Gray;
            ClearForm();
            formPanel.Enabled = false;
            btnUpdate.Enabled = false;
            btnReset.Enabled = false;
            lblCurrentStudent.Text = "No student loaded";
            lblCurrentStudent.ForeColor = ConfigurationConstants.SSErrorRed;
            studentLoaded = false;
            currentStudentId = "";
            currentUserId = "";
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (!studentLoaded)
            {
                MessageBox.Show("Please search and load a student first.", "No Student Loaded", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ValidateForm())
            {
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to update this student's information?",
                    "Confirm Update",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    UpdateStudent();
                }
            }
        }

        private void UpdateStudent()
        {
            loadingIndicator.Show("Updating Student");

            try
            {
                // Build StudentModel from form data
                StudentModel student = new StudentModel
                {
                    Id = currentStudentId,
                    UserId = currentUserId,
                    CNIC = txtCNIC.Text.Replace("-", "").Trim(),
                    RollNumber = txtRollNumber.Text.Trim(),
                    FullName = txtFullName.Text.Trim(),
                    DateOfBirth = dtpDateOfBirth.Value.Date,
                    Gender = cmbGender.SelectedItem.ToString(),
                    BloodGroup = UIHelper.IsPlaceholderText(txtBloodGroup) ? null : txtBloodGroup.Text.Trim(),
                    Religion = UIHelper.IsPlaceholderText(txtReligion) ? null : txtReligion.Text.Trim(),
                    Nationality = UIHelper.IsPlaceholderText(txtNationality) ? null : txtNationality.Text.Trim(),
                    DomicileDistrict = UIHelper.IsPlaceholderText(txtDomicileDistrict) ? null : txtDomicileDistrict.Text.Trim(),
                    MobileNo = txtMobileNo.Text.Trim(),
                    WhatsappNo = UIHelper.IsPlaceholderText(txtWhatsAppNo) ? null : txtWhatsAppNo.Text.Trim(),
                    ProgramId = ((ComboBoxItem)cmbProgram.SelectedItem).Value
                };

                // Get email
                string email = txtEmail.Text.Trim();

                // Get new password (if changing)
                string newPassword = null;
                if (chkChangePassword.Checked && !string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    newPassword = txtPassword.Text;
                }

                // Use repository to update student
                bool success = studentRepository.UpdateStudent(student, email, newPassword);

                loadingIndicator.Hide();

                if (success)
                {
                    MessageBox.Show("Student information updated successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Clear password fields
                    chkChangePassword.Checked = false;
                    txtPassword.Clear();
                    txtConfirmPassword.Clear();
                }
            }
            catch (Exception ex)
            {
                loadingIndicator.Hide();
                MessageBox.Show($"Error updating student: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (!studentLoaded)
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
                string searchTerm = txtCNIC.Text.Replace("-", "").Trim();
                LoadStudentData(searchTerm);
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtCNIC.Text) || UIHelper.IsPlaceholderText(txtCNIC))
            {
                MessageBox.Show("Please enter CNIC.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCNIC.Focus();
                return false;
            }

            // Validate CNIC format
            string cnicDigits = txtCNIC.Text.Replace("-", "").Trim();
            if (cnicDigits.Length != 13 || !cnicDigits.All(char.IsDigit))
            {
                MessageBox.Show("Please enter a valid CNIC (13 digits).\nFormat: 12345-6789012-3", "Invalid CNIC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCNIC.Focus();
                return false;
            }

            if (UIHelper.IsPlaceholderText(txtRollNumber))
            {
                MessageBox.Show("Please enter Roll Number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRollNumber.Focus();
                return false;
            }

            if (UIHelper.IsPlaceholderText(txtFullName))
            {
                MessageBox.Show("Please enter Full Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return false;
            }

            if (cmbGender.SelectedIndex == -1)
            {
                MessageBox.Show("Please select Gender.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbGender.Focus();
                return false;
            }

            if (UIHelper.IsPlaceholderText(txtMobileNo))
            {
                MessageBox.Show("Please enter Mobile Number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMobileNo.Focus();
                return false;
            }

            if (cmbProgram.SelectedIndex == -1)
            {
                MessageBox.Show("Please select Program.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbProgram.Focus();
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
            UIHelper.ClearToPlaceholder(txtRollNumber);
            UIHelper.ClearToPlaceholder(txtFullName);
            dtpDateOfBirth.Value = DateTime.Now;
            cmbGender.SelectedIndex = -1;
            UIHelper.ClearToPlaceholder(txtBloodGroup);
            UIHelper.ClearToPlaceholder(txtReligion);
            UIHelper.ClearToPlaceholder(txtNationality);
            UIHelper.ClearToPlaceholder(txtDomicileDistrict);
            UIHelper.ClearToPlaceholder(txtMobileNo);
            UIHelper.ClearToPlaceholder(txtWhatsAppNo);
            cmbProgram.SelectedIndex = -1;

            UIHelper.ClearToPlaceholder(txtEmail);
            chkChangePassword.Checked = false;
            txtPassword.Clear();
            txtConfirmPassword.Clear();

            cmbGuardianRelation.SelectedIndex = -1;
            UIHelper.ClearToPlaceholder(txtGuardianName);
            UIHelper.ClearToPlaceholder(txtGuardianCNIC);
            UIHelper.ClearToPlaceholder(txtGuardianMobile);
            UIHelper.ClearToPlaceholder(txtGuardianAddress);
            chkIsDeceased.Checked = false;
        }
    }
}
