using System;
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
    public partial class RegisterStudentControl : UserControl
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Panel formPanel;
        private Panel buttonPanel;
        private Button submitButton;
        private Button resetButton;

        // Personal Information
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
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;

        // Guardian Information
        private ComboBox cmbGuardianRelation;
        private TextBox txtGuardianName;
        private TextBox txtGuardianCNIC;
        private TextBox txtGuardianMobile;
        private TextBox txtGuardianAddress;
        private CheckBox chkIsDeceased;

        // Loading Indicator
        private LoadingIndicator loadingIndicator;

        // Repository
        private StudentRepository studentRepository;

        // Using constants from StudentControlHelper

        public RegisterStudentControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.BackColor = ConfigurationConstants.SSWhiteColor;
            this.AutoScroll = true;
            
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
            
            LoadPrograms();
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
                Text = "Only Administrators can register students.\n\nPlease contact your system administrator for access.",
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
                BackColor = ConfigurationConstants.SSDarkNavyColor,
            };

            titleLabel = new Label
            {
                Text = "Student Registration Form",
                Font = new Font("Arial", 22, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.Add(titleLabel);
            this.Controls.Add(headerPanel);

            // Form Panel with AutoScroll
            formPanel = new Panel
            {
                Dock = DockStyle.None,
                BackColor = ConfigurationConstants.SSWhiteColor,
                AutoScroll = true,
                Location = new Point(10, 100),
                Size = new Size(ConfigurationConstants.ScreenWidth - 300, ConfigurationConstants.ScreenHeight - 300)

            };
            this.Controls.Add(formPanel);

            // Button Panel
            buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 100,
                BackColor = ConfigurationConstants.SSLightGrayBackground,
                
            };

            submitButton = UIHelper.CreateButton("Register Student", ConfigurationConstants.SSDarkNavyColor, new Point(40, 25));
            submitButton.Click += SubmitButton_Click;

            resetButton = UIHelper.CreateButton("Reset Form", ConfigurationConstants.SSDarkBlueColor, new Point(submitButton.Right + 20, 25));
            resetButton.Click += ResetButton_Click;

            buttonPanel.Controls.Add(submitButton);
            buttonPanel.Controls.Add(resetButton);
            this.Controls.Add(buttonPanel);

            // Create form sections
            CreateFormControls();
        }

        private void CreateFormControls()
        {
            int yPosition = 0;
            int leftColumnX = 0;
            int rightColumnX = UIHelper.LABEL_WIDTH + UIHelper.CONTROL_WIDTH + UIHelper.HORIZONTAL_SPACING + UIHelper.COLUMN_SPACING;

            // Personal Information Section
            yPosition = UIHelper.CreateSectionHeader(formPanel, "Personal Information", yPosition);
            
            // Row 1: CNIC and Roll Number
            UIHelper.CreateFormRow(formPanel, "CNIC:", ref txtCNIC, leftColumnX, yPosition, "12345-6789012-3");
            UIHelper.CreateFormRow(formPanel, "Roll Number:", ref txtRollNumber, rightColumnX, yPosition, "2024-CS-001");
            yPosition += UIHelper.VERTICAL_SPACING;

            // Row 2: Full Name and Date of Birth
            UIHelper.CreateFormRow(formPanel, "Full Name:", ref txtFullName, leftColumnX, yPosition, "Enter full name");
            UIHelper.CreateDatePickerRow(formPanel, "Date of Birth:", ref dtpDateOfBirth, rightColumnX, yPosition);
            yPosition += UIHelper.VERTICAL_SPACING;

            // Row 3: Gender and Blood Group
            UIHelper.CreateComboBoxRow(formPanel, "Gender:", ref cmbGender, leftColumnX, yPosition, new string[] { "Male", "Female", "Other" });
            UIHelper.CreateFormRow(formPanel, "Blood Group:", ref txtBloodGroup, rightColumnX, yPosition, "A+, B+, O+, etc.");
            yPosition += UIHelper.VERTICAL_SPACING;

            // Row 4: Religion and Nationality
            UIHelper.CreateFormRow(formPanel, "Religion:", ref txtReligion, leftColumnX, yPosition, "Islam, Christian, etc.");
            UIHelper.CreateFormRow(formPanel, "Nationality:", ref txtNationality, rightColumnX, yPosition, "Pakistani");
            yPosition += UIHelper.VERTICAL_SPACING;

            // Row 5: Domicile District and Mobile No
            UIHelper.CreateFormRow(formPanel, "Domicile:", ref txtDomicileDistrict, leftColumnX, yPosition, "District name");
            UIHelper.CreateFormRow(formPanel, "Mobile No:", ref txtMobileNo, rightColumnX, yPosition, "03XX-XXXXXXX");
            yPosition += UIHelper.VERTICAL_SPACING;

            // Row 6: WhatsApp No and Program
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
            
            // Row 1: Email and Password
            UIHelper.CreateFormRow(formPanel, "Email:", ref txtEmail, leftColumnX, yPosition, "student@example.com");
            UIHelper.CreatePasswordRow(formPanel, "Password:", ref txtPassword, rightColumnX, yPosition);
            yPosition += UIHelper.VERTICAL_SPACING;

            // Row 2: Confirm Password (centered or left column)
            UIHelper.CreatePasswordRow(formPanel, "Confirm Password:", ref txtConfirmPassword, leftColumnX, yPosition);
            yPosition += UIHelper.VERTICAL_SPACING + UIHelper.SECTION_SPACING;

            // Guardian Information Section
            yPosition = UIHelper.CreateSectionHeader(formPanel, "Guardian Information", yPosition);
            
            // Row 1: Relation and Guardian Name
            UIHelper.CreateComboBoxRow(formPanel, "Relation:", ref cmbGuardianRelation, leftColumnX, yPosition, new string[] { "Father", "Mother", "Guardian" });
            UIHelper.CreateFormRow(formPanel, "Guardian Name:", ref txtGuardianName, rightColumnX, yPosition, "Enter guardian's full name");
            yPosition += UIHelper.VERTICAL_SPACING;

            // Row 2: Guardian CNIC and Guardian Mobile
            UIHelper.CreateFormRow(formPanel, "Guardian CNIC:", ref txtGuardianCNIC, leftColumnX, yPosition, "12345-6789012-3");
            UIHelper.CreateFormRow(formPanel, "Guardian Mobile:", ref txtGuardianMobile, rightColumnX, yPosition, "03XX-XXXXXXX");
            yPosition += UIHelper.VERTICAL_SPACING;

            // Row 3: Guardian Address (full width)
            UIHelper.CreateFormRow(formPanel, "Guardian Address:", ref txtGuardianAddress, leftColumnX, yPosition, "Enter full address");
            UIHelper.CreateCheckBoxRow(formPanel, "Is Deceased:", ref chkIsDeceased, rightColumnX, yPosition);
            yPosition += UIHelper.VERTICAL_SPACING + UIHelper.SECTION_SPACING;
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                loadingIndicator.Show("Registering Student");

                if (RegisterStudent())
                {
                    loadingIndicator.Hide();
                    
                    MessageBox.Show("Student registered successfully!", 
                        "Success", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Information);
                    
                    ClearForm();
                }
                else
                {
                    loadingIndicator.Hide();
                }
            }
        }

        private bool RegisterStudent()
        {
            try
            {
                // Step 1: Build Student Model from form data
                StudentModel student = new StudentModel
                {
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

                // Step 2: Get account information
                string email = txtEmail.Text.Trim();
                string password = txtPassword.Text;

                // Step 3: Build Guardian Model (if provided)
                GuardianModel guardian = null;
                if (cmbGuardianRelation.SelectedIndex != -1)
                {
                    guardian = new GuardianModel
                    {
                        Relation = cmbGuardianRelation.SelectedItem.ToString(),
                        FullName = txtGuardianName.Text.Trim(),
                        CNIC = UIHelper.IsPlaceholderText(txtGuardianCNIC) ? null : txtGuardianCNIC.Text.Replace("-", "").Trim(),
                        MobileNo = txtGuardianMobile.Text.Trim(),
                        Address = UIHelper.IsPlaceholderText(txtGuardianAddress) ? null : txtGuardianAddress.Text.Trim(),
                        IsDeceased = chkIsDeceased.Checked
                    };
                }

                // Step 4: Call repository to register student
                string studentId = studentRepository.RegisterStudent(student, email, password, guardian);

                return !string.IsNullOrEmpty(studentId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error registering student: {ex.Message}",
                    "Registration Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to reset the form? All data will be cleared.",
                "Reset Form",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                ClearForm();
            }
        }

        private bool ValidateForm()
        {
            // Validate CNIC
            if (UIHelper.IsPlaceholderText(txtCNIC))
            {
                MessageBox.Show("Please enter CNIC.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCNIC.Focus();
                return false;
            }

            string cnicDigits = txtCNIC.Text.Replace("-", "").Trim();
            if (cnicDigits.Length != 13 || !cnicDigits.All(char.IsDigit))
            {
                MessageBox.Show("Please enter a valid CNIC (13 digits).\nFormat: 12345-6789012-3", 
                    "Invalid CNIC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCNIC.Focus();
                return false;
            }

            // Validate Roll Number
            if (UIHelper.IsPlaceholderText(txtRollNumber))
            {
                MessageBox.Show("Please enter Roll Number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRollNumber.Focus();
                return false;
            }

            // Validate Full Name
            if (UIHelper.IsPlaceholderText(txtFullName))
            {
                MessageBox.Show("Please enter Full Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return false;
            }

            // Validate Gender
            if (cmbGender.SelectedIndex == -1)
            {
                MessageBox.Show("Please select Gender.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbGender.Focus();
                return false;
            }

            // Validate Mobile Number
            if (UIHelper.IsPlaceholderText(txtMobileNo))
            {
                MessageBox.Show("Please enter Mobile Number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMobileNo.Focus();
                return false;
            }

            // Validate Program
            if (cmbProgram.SelectedIndex == -1)
            {
                MessageBox.Show("Please select Program.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbProgram.Focus();
                return false;
            }

            // Validate Email
            if (UIHelper.IsPlaceholderText(txtEmail))
            {
                MessageBox.Show("Please enter Email.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            if (!txtEmail.Text.Contains("@") || !txtEmail.Text.Contains("."))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            // Validate Password
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter Password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

            if (txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

            // Validate Confirm Password
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Password and Confirm Password do not match.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return false;
            }

            // Validate Guardian Information (if provided)
            if (cmbGuardianRelation.SelectedIndex != -1)
            {
                if (UIHelper.IsPlaceholderText(txtGuardianName))
                {
                    MessageBox.Show("Please enter Guardian Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtGuardianName.Focus();
                    return false;
                }

                if (UIHelper.IsPlaceholderText(txtGuardianMobile))
                {
                    MessageBox.Show("Please enter Guardian Mobile Number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtGuardianMobile.Focus();
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
            dtpDateOfBirth.Value = DateTime.Now.AddYears(-18);
            cmbGender.SelectedIndex = -1;
            UIHelper.ClearToPlaceholder(txtBloodGroup);
            UIHelper.ClearToPlaceholder(txtReligion);
            UIHelper.ClearToPlaceholder(txtNationality);
            UIHelper.ClearToPlaceholder(txtDomicileDistrict);
            UIHelper.ClearToPlaceholder(txtMobileNo);
            UIHelper.ClearToPlaceholder(txtWhatsAppNo);
            cmbProgram.SelectedIndex = -1;

            UIHelper.ClearToPlaceholder(txtEmail);
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
