using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ScholarSync.Commons;
using ScholarSync.Models;
using ScholarSync.Repositories;

namespace ScholarSync.Controls.Teacher
{
    public partial class CreateTeacherControl : UserControl
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Panel formPanel;
        private Panel buttonPanel;
        private Button submitButton;
        private Button resetButton;

        // Personal Information
        private TextBox txtCNIC;
        private TextBox txtFullName;
        
        // Professional Information
        private TextBox txtDesignation;
        private TextBox txtSpecialization;

        // Account Information
        private TextBox txtEmail;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;

        // Loading Indicator
        private LoadingIndicator loadingIndicator;

        // Repository
        private TeacherRepository teacherRepository;

        public CreateTeacherControl()
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
                Text = "Only Administrators can create teachers.\n\nPlease contact your system administrator for access.",
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
                BackColor = ConfigurationConstants.SSDarkNavyColor,
            };

            titleLabel = new Label
            {
                Text = "Teacher Registration Form",
                Font = new Font("Arial", 22, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(30, 0, 0, 0)
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
                Size = new Size(ConfigurationConstants.ScreenWidth - 300, ConfigurationConstants.ScreenHeight - 300),
                Padding = new Padding(40, 30, 40, 30)
            };
            this.Controls.Add(formPanel);

            // Button Panel
            buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 100,
                BackColor = ConfigurationConstants.SSLightGrayBackground,
                
            };

            submitButton = UIHelper.CreateButton("Register Teacher", ConfigurationConstants.SSDarkNavyColor, new Point(40, 25));
            submitButton.Click += SubmitButton_Click;
            submitButton.ForeColor = ConfigurationConstants.SSWhiteColor;

            resetButton = UIHelper.CreateButton("Reset Form", ConfigurationConstants.SSDarkBlueColor, new Point(submitButton.Right + 20, 25));
            resetButton.Click += ResetButton_Click;
            resetButton.ForeColor = ConfigurationConstants.SSWhiteColor;

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
            
            UIHelper.CreateFormRow(formPanel, "CNIC:", ref txtCNIC, leftColumnX, yPosition, "12345-6789012-3");
            UIHelper.CreateFormRow(formPanel, "Full Name:", ref txtFullName, rightColumnX, yPosition, "Enter full name");
            yPosition += UIHelper.VERTICAL_SPACING + UIHelper.SECTION_SPACING;

            // Professional Information Section
            yPosition = UIHelper.CreateSectionHeader(formPanel, "Professional Information", yPosition);
            
            UIHelper.CreateFormRow(formPanel, "Designation:", ref txtDesignation, leftColumnX, yPosition, "e.g. Professor, Lecturer");
            UIHelper.CreateFormRow(formPanel, "Specialization:", ref txtSpecialization, rightColumnX, yPosition, "e.g. Machine Learning, Networks");
            yPosition += UIHelper.VERTICAL_SPACING + UIHelper.SECTION_SPACING;

            // Account Information Section
            yPosition = UIHelper.CreateSectionHeader(formPanel, "Account Information", yPosition);
            
            UIHelper.CreateFormRow(formPanel, "Email:", ref txtEmail, leftColumnX, yPosition, "teacher@example.com");
            UIHelper.CreatePasswordRow(formPanel, "Password:", ref txtPassword, rightColumnX, yPosition);
            yPosition += UIHelper.VERTICAL_SPACING;

            UIHelper.CreatePasswordRow(formPanel, "Confirm Password:", ref txtConfirmPassword, leftColumnX, yPosition);
            yPosition += UIHelper.VERTICAL_SPACING + UIHelper.SECTION_SPACING;
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                if (RegisterTeacher())
                {
                    MessageBox.Show("Teacher registered successfully!", 
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                }
            }
        }

        private bool RegisterTeacher()
        {
            loadingIndicator.Show("Registering Teacher");

            try
            {
                // Step 1: Build Teacher Model from form data
                TeacherModel teacher = new TeacherModel
                {
                    CNIC = txtCNIC.Text.Replace("-", "").Trim(),
                    FullName = txtFullName.Text.Trim(),
                    Designation = UIHelper.IsPlaceholderText(txtDesignation) ? null : txtDesignation.Text.Trim(),
                    Specialization = UIHelper.IsPlaceholderText(txtSpecialization) ? null : txtSpecialization.Text.Trim()
                };

                // Step 2: Get account information
                string email = txtEmail.Text.Trim();
                string password = txtPassword.Text;

                // Step 3: Call repository to register teacher
                loadingIndicator.Hide();
                
                string teacherId = teacherRepository.RegisterTeacher(teacher, email, password);

                return !string.IsNullOrEmpty(teacherId);
            }
            catch (Exception ex)
            {
                loadingIndicator.Hide();
                MessageBox.Show($"Error registering teacher: {ex.Message}",
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
            if (UIHelper.IsPlaceholderText(txtCNIC))
            {
                MessageBox.Show("Please enter CNIC.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCNIC.Focus();
                return false;
            }

            // Validate CNIC format
            string cnicDigits = txtCNIC.Text.Replace("-", "").Trim();
            if (cnicDigits.Length != 13 || !cnicDigits.All(char.IsDigit))
            {
                MessageBox.Show("Please enter a valid CNIC (13 digits).\nFormat: 12345-6789012-3", "Invalid CNIC", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            return true;
        }

        private void ClearForm()
        {
            UIHelper.ClearToPlaceholder(txtCNIC);
            UIHelper.ClearToPlaceholder(txtFullName);

            UIHelper.ClearToPlaceholder(txtDesignation);
            UIHelper.ClearToPlaceholder(txtSpecialization);

            UIHelper.ClearToPlaceholder(txtEmail);
            txtPassword.Clear();
            txtConfirmPassword.Clear();
        }
    }
}
