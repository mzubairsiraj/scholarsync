using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Syncfusion.WinForms.Controls;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Input;
using ScholarSync.Commons;
using ScholarSync.Configuration;
using ScholarSync.Infrastructure.Services;
using ScholarSync.Infrastructure.Security;
using ScholarSync.UIComponents;

namespace ScholarSync.Controls.Auth
{
    public partial class RegisterControl : UserControl
    {
        // UI Constants
        private const int CARD_WIDTH = 550;
        private const int CARD_HEIGHT = 680;
        private const int LOGO_SIZE = 70;

        // UI Components
        private GradientPanel mainPanel;
        private GradientPanel cardPanel;
        
        // Logo and Title
        private PictureBox logoBox;
        private AutoLabel collegeNameLabel;
        
        // Form Controls
        private AutoLabel fullNameLabel;
        private TextBoxExt fullNameTextBox; 
        
        private AutoLabel emailLabel;
        private TextBoxExt emailTextBox; 
        
        private AutoLabel passwordLabel;
        private TextBoxExt passwordTextBox; 
        
        private AutoLabel confirmPasswordLabel;
        private TextBoxExt confirmPasswordTextBox; 
        
        // Profile Image
        private AutoLabel profileImageLabel;
        private PictureBox profileImageBox;
        private SfButton uploadImageButton;
        
        // Action Buttons
        private SfButton registerButton;
        private SfButton backToLoginButton;
        
        // Events
        public event EventHandler OnBackToLogin;

        // Data
        private string selectedImagePath = null;

        public RegisterControl()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = ConfigurationConstants.SSLightGrayBackground;

            CreateMainPanel();
            CreateCardPanel();
            CreateLogoSection();
            CreateFormControls();
        }

        private void CreateMainPanel()
        {
            mainPanel = SfUIHelper.CreateGradientPanel(
                Size.Empty,
                Point.Empty,
                ConfigurationConstants.SSLightGrayBackground
            );
            mainPanel.Dock = DockStyle.Fill;
            this.Controls.Add(mainPanel);
        }

        private void CreateCardPanel()
        {
            cardPanel = SfUIHelper.CreateCardPanel(
                CARD_WIDTH,
                CARD_HEIGHT,
                Point.Empty,
                Color.White
            );

            // Apply rounded corners
            cardPanel.Paint += (s, e) =>
            {
                using (GraphicsPath path = GetRoundedRectPath(new Rectangle(0, 0, CARD_WIDTH - 1, CARD_HEIGHT - 1), 12))
                {
                    cardPanel.Region = new Region(path);
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (Pen pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            // Center the card in main panel
            mainPanel.Resize += (s, e) =>
            {
                SfUIHelper.CenterInParent(cardPanel, mainPanel);
            };

            mainPanel.Controls.Add(cardPanel);
        }

        private GraphicsPath GetRoundedRectPath(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();
            
            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }
            
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            
            return path;
        }

        private void CreateLogoSection()
        {
            int currentY = SfUIHelper.PADDING_MEDIUM;

            // Logo
            logoBox = new PictureBox
            {
                Size = new Size(LOGO_SIZE, LOGO_SIZE),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = ConfigurationConstants.LogoImage,
                Location = new Point((CARD_WIDTH - LOGO_SIZE) / 2, currentY)
            };
            cardPanel.Controls.Add(logoBox);

            currentY += LOGO_SIZE + SfUIHelper.MARGIN_SMALL;

            // College Name
            var config = AppConfiguration.Instance;
            string collegeName = config.AppSettings?.ApplicationName ?? "ScholarSync";
            
            collegeNameLabel = SfUIHelper.CreateHeadingLabel(
                collegeName,
                Point.Empty,
                SfUIHelper.FONT_SIZE_XLARGE
            );
            // Center the AutoSize label
            collegeNameLabel.Location = new Point((CARD_WIDTH - collegeNameLabel.Width) / 2, currentY);
            cardPanel.Controls.Add(collegeNameLabel);

            currentY += 30 + SfUIHelper.MARGIN_MEDIUM;

            // "Create Account" heading
            var createAccountLabel = SfUIHelper.CreateHeadingLabel(
                "Create Account",
                Point.Empty,
                SfUIHelper.FONT_SIZE_LARGE
            );
            createAccountLabel.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            
            // Add to panel to measure width
            cardPanel.Controls.Add(createAccountLabel);
            createAccountLabel.Location = new Point((CARD_WIDTH - createAccountLabel.Width) / 2, currentY);
        }

        private void CreateFormControls()
        {
            int currentY = 170;
            int startX = SfUIHelper.PADDING_LARGE;
            int totalWidth = CARD_WIDTH - (SfUIHelper.PADDING_LARGE * 2);
            int columnGap = SfUIHelper.HORIZONTAL_SPACING;
            int inputWidth = (totalWidth - columnGap) / 2;

            // --- Row 1: Full Name & Email ---
            
            // Increased spacing between Label and Input to prevent overlap
            int labelToInputSpacing = 30; 

            // Full Name (Left)
            fullNameLabel = SfUIHelper.CreateLabel(
                "Full Name",
                new Point(startX, currentY),
                inputWidth,
                SfUIHelper.HEIGHT_SMALL,
                FontStyle.Bold
            );
            cardPanel.Controls.Add(fullNameLabel);

            // Email (Right)
            emailLabel = SfUIHelper.CreateLabel(
                "Email Address",
                new Point(startX + inputWidth + columnGap, currentY),
                inputWidth,
                SfUIHelper.HEIGHT_SMALL,
                FontStyle.Bold
            );
            cardPanel.Controls.Add(emailLabel);

            // Inputs Row 1
            // Full Name Input
            var fullNameControl = SfUIHelper.CreateRoundedTextBox(
                "Full Name",
                new Point(startX, currentY + labelToInputSpacing),
                inputWidth,
                45
            );
            fullNameTextBox = fullNameControl.textBox;
            cardPanel.Controls.Add(fullNameControl.container);

            // Email Input
            var emailControl = SfUIHelper.CreateRoundedTextBox(
                "Email",
                new Point(startX + inputWidth + columnGap, currentY + labelToInputSpacing),
                inputWidth,
                45
            );
            emailTextBox = emailControl.textBox;
            cardPanel.Controls.Add(emailControl.container);

            currentY += labelToInputSpacing + 45 + SfUIHelper.VERTICAL_SPACING + 10;

            // --- Row 2: Password & Confirm Password ---

            // Password Label (Left)
            passwordLabel = SfUIHelper.CreateLabel(
                "Password",
                new Point(startX, currentY),
                inputWidth,
                SfUIHelper.HEIGHT_SMALL,
                FontStyle.Bold
            );
            cardPanel.Controls.Add(passwordLabel);

            // Confirm Password Label (Right)
            confirmPasswordLabel = SfUIHelper.CreateLabel(
                "Confirm Password",
                new Point(startX + inputWidth + columnGap, currentY),
                inputWidth,
                SfUIHelper.HEIGHT_SMALL,
                FontStyle.Bold
            );
            cardPanel.Controls.Add(confirmPasswordLabel);

            // Inputs Row 2
            // Password Input
            var passwordControl = SfUIHelper.CreateRoundedTextBox(
                "Password",
                new Point(startX, currentY + labelToInputSpacing),
                inputWidth,
                45,
                true // isPassword
            );
            passwordTextBox = passwordControl.textBox;
            cardPanel.Controls.Add(passwordControl.container);

            // Confirm Password Input
            var confirmControl = SfUIHelper.CreateRoundedTextBox(
                "Confirm",
                new Point(startX + inputWidth + columnGap, currentY + labelToInputSpacing),
                inputWidth,
                45,
                true // isPassword
            );
            confirmPasswordTextBox = confirmControl.textBox;
            cardPanel.Controls.Add(confirmControl.container);

            currentY += labelToInputSpacing + 45 + 20; // Reduced top margin for profile section

            // --- Row 3: Profile Image ---
            // Reduced spacing for profile section
            CreateProfileImageSection(startX, currentY, totalWidth);
        }

        private void CreateProfileImageSection(int x, int y, int width)
        {
            // Center the section
            int sectionWidth = 250;
            int sectionX = x + (width - sectionWidth) / 2;

            // Label
            profileImageLabel = SfUIHelper.CreateLabel(
                "Profile Image (Optional)",
                new Point(sectionX, y),
                sectionWidth,
                SfUIHelper.HEIGHT_SMALL,
                FontStyle.Bold
            );
            // Center alignment for label text isn't auto, so we center the control
            profileImageLabel.Location = new Point(x + (width - profileImageLabel.Width) / 2, y);
            cardPanel.Controls.Add(profileImageLabel);

            y += 25; // Smaller gap

            // Image preview box (centered)
            profileImageBox = new PictureBox
            {
                Size = new Size(60, 60), // Slightly smaller
                Location = new Point(x + (width - 60) / 2, y),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                Image = ImageHandler.GenerateInitialsImage("User", 60)
            };
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(0, 0, 58, 58);
                profileImageBox.Region = new Region(path);
            }
            cardPanel.Controls.Add(profileImageBox);

            y += 70; // Reduced gap

            // Upload button (centered)
            uploadImageButton = SfUIHelper.CreateSecondaryButton(
                "Choose Image",
                new Point(x + (width - 140) / 2, y),
                140,
                35
            );
            
            uploadImageButton.Style.BackColor = Color.White;
            uploadImageButton.Style.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            
            uploadImageButton.Click += UploadImageButton_Click;
            cardPanel.Controls.Add(uploadImageButton);
            
            y += 45; // Reduced gap before Register button

            CreateActionButtons(y);
        }

        private void CreateActionButtons(int currentY)
        {
            int buttonWidth = 300; 
            int buttonX = (CARD_WIDTH - buttonWidth) / 2;

            // Register Button
            registerButton = SfUIHelper.CreateModernPrimaryButton(
                "Create Account",
                new Point(buttonX, currentY),
                buttonWidth,
                50
            );
            registerButton.Click += RegisterButton_Click;
            cardPanel.Controls.Add(registerButton);

            currentY += 60; // Standard gap

            // Back to Login
            backToLoginButton = SfUIHelper.CreateSecondaryButton(
                "Already have an account? Login",
                new Point(buttonX, currentY),
                buttonWidth,
                30
            );
            
            // Make it look like a link
            backToLoginButton.Style.BackColor = Color.Transparent;
            backToLoginButton.Style.Border = new Pen(Color.Transparent, 0);
            backToLoginButton.Style.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            backToLoginButton.Font = new Font("Segoe UI", SfUIHelper.FONT_SIZE_MEDIUM, FontStyle.Underline);
            
            backToLoginButton.Click += BackToLoginButton_Click;
            cardPanel.Controls.Add(backToLoginButton);
        }

        #region Event Handlers

        private void UploadImageButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.Title = "Select Profile Image";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        selectedImagePath = openFileDialog.FileName;
                        
                        // Load and display preview
                        using (Image originalImage = Image.FromFile(selectedImagePath))
                        {
                            profileImageBox.Image = new Bitmap(originalImage, profileImageBox.Size);
                        }

                        uploadImageButton.Text = "Change Image";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Failed to load image: {ex.Message}",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (!ValidateInputs())
                return;

            try
            {
                // Get values
                string fullName = fullNameTextBox.Text.Trim();
                string email = emailTextBox.Text.Trim();
                string password = passwordTextBox.Text;

                // Validate password strength
                var passwordHasher = ServiceHelper.GetPasswordHasher();
                var validationResult = passwordHasher.ValidatePassword(password);

                if (!validationResult.IsValid)
                {
                    MessageBox.Show(
                        "Password does not meet requirements:\n\n" + string.Join("\n", validationResult.Errors),
                        "Weak Password",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Hash the password
                string hashedPassword = passwordHasher.HashPassword(password);

                // TODO: Save user to database
                // For now, show success message
                MessageBox.Show(
                    $"Account created successfully!\n\nName: {fullName}\nEmail: {email}\n\nPassword has been securely hashed.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // TODO: If image selected, save it
                if (!string.IsNullOrEmpty(selectedImagePath))
                {
                    // ImageHandler.SaveUserImage(userId, selectedImagePath);
                }

                // Clear form
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Registration failed: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BackToLoginButton_Click(object sender, EventArgs e)
        {
            OnBackToLogin?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Validation & Helpers

        private bool ValidateInputs()
        {
            // Check Full Name
            string fullName = fullNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(fullName))
            {
                ShowError("Please enter your full name");
                fullNameTextBox.Focus();
                return false;
            }

            // Check Email
            string email = emailTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError("Please enter your email address");
                emailTextBox.Focus();
                return false;
            }

            if (!IsValidEmail(email))
            {
                ShowError("Please enter a valid email address");
                emailTextBox.Focus();
                return false;
            }

            // Check Password
            if (string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                ShowError("Please enter a password");
                passwordTextBox.Focus();
                return false;
            }

            // Check Confirm Password
            if (passwordTextBox.Text != confirmPasswordTextBox.Text)
            {
                ShowError("Passwords do not match");
                confirmPasswordTextBox.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(
                message,
                "Validation Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void ClearForm()
        {
            fullNameTextBox.Clear();
            emailTextBox.Clear();
            passwordTextBox.Clear();
            confirmPasswordTextBox.Clear();

            selectedImagePath = null;
            profileImageBox.Image = ImageHandler.GenerateInitialsImage("User", 70);
            uploadImageButton.Text = "Choose Image";
        }

        #endregion
    }
}
