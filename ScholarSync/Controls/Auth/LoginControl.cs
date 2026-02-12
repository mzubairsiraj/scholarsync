using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Syncfusion.WinForms.Controls;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Input;
using Syncfusion.WinForms.ListView;
using ScholarSync.Commons;
using ScholarSync.Configuration;
using ScholarSync.UIComponents;

namespace ScholarSync.Controls.Auth
{
    /// <summary>
    /// Modern Login Control - Windows 11 Style
    /// Uses modular SfUIHelper for consistent UI components
    /// </summary>
    public partial class LoginControl : UserControl
    {
        // UI Constants
        private const int CARD_WIDTH = 520;
        private const int CARD_HEIGHT = 640;
        private const int LOGO_SIZE = 80;

        // UI Components
        private GradientPanel mainPanel;
        private GradientPanel cardPanel;
        
        // Logo and Title
        private PictureBox logoBox;
        private AutoLabel collegeNameLabel;
        private AutoLabel loginTitleLabel;
        
        // Form Controls
        private AutoLabel roleLabel;
        private SfComboBox roleComboBox;
        private AutoLabel emailLabel;
        private TextBoxExt emailTextBox; // Changed from SfTextBox
        private AutoLabel passwordLabel;
        private TextBoxExt passwordTextBox; // Changed from SfTextBox
        private SfButton loginButton;
        private SfButton registerButton;
        private SfButton forgotPasswordButton;
        
        // Team Info
        private AutoLabel teamLabel;

        // Events
        public event EventHandler<LoginEventArgs> OnLoginAttempt;
        public event EventHandler OnRegisterClicked;
        public event EventHandler OnForgotPasswordClicked;

        public LoginControl()
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
            CreateLoginForm();
            CreateTeamInfo();
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
            int currentY = 30;

            // Logo
            logoBox = new PictureBox
            {
                Size = new Size(LOGO_SIZE, LOGO_SIZE),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = ConfigurationConstants.LogoImage,
                Location = new Point((CARD_WIDTH - LOGO_SIZE) / 2, currentY)
            };
            cardPanel.Controls.Add(logoBox);

            currentY += LOGO_SIZE + 15;

            // College Name - CENTERED
            var config = AppConfiguration.Instance;
            string collegeName = config.AppSettings?.ApplicationName ?? "ScholarSync";
            
            collegeNameLabel = SfUIHelper.CreateHeadingLabel(
                collegeName,
                Point.Empty,
                18
            );
            // Center label manually since AutoLabel AutoSize=true
            collegeNameLabel.Location = new Point((CARD_WIDTH - collegeNameLabel.Width) / 2, currentY);
            cardPanel.Controls.Add(collegeNameLabel);

            currentY += collegeNameLabel.Height + 10;

            // Login Title - CENTERED
            loginTitleLabel = SfUIHelper.CreateLabel(
                "Login to Your Account",
                Point.Empty, 
                CARD_WIDTH, // Ignored if AutoSize=true but used for manual calculation
                30,
                FontStyle.Regular
            );
            loginTitleLabel.Font = new Font("Segoe UI", 14, FontStyle.Regular);
            loginTitleLabel.ForeColor = Color.FromArgb(100, 100, 100);
            loginTitleLabel.AutoSize = true;
            
            // Add first to measure width
            cardPanel.Controls.Add(loginTitleLabel); 
            loginTitleLabel.Location = new Point((CARD_WIDTH - loginTitleLabel.Width) / 2, currentY);
        }

        private void CreateLoginForm()
        {
            int currentY = 230;
            int labelX = 40;
            int inputX = 40;
            int inputWidth = CARD_WIDTH - 80;

            // 1. Role Label
            roleLabel = SfUIHelper.CreateLabel(
                "Select Role",
                new Point(labelX, currentY),
                inputWidth,
                20,
                FontStyle.Regular
            );
            cardPanel.Controls.Add(roleLabel);
            currentY += 25;

            // 2. Role ComboBox
            roleComboBox = SfUIHelper.CreateModernComboBox(
                new Point(inputX, currentY),
                inputWidth,
                45
            );
            roleComboBox.DataSource = new string[] { "Admin", "Teacher" };
            roleComboBox.SelectedIndex = 0;
            cardPanel.Controls.Add(roleComboBox);

            currentY += 55;

            // 3. Email Label
            emailLabel = SfUIHelper.CreateLabel(
                "Email Address",
                new Point(labelX, currentY),
                inputWidth,
                20,
                FontStyle.Regular
            );
            cardPanel.Controls.Add(emailLabel);
            currentY += 25;

            // 4. Email TextBox (Rounded)
            var emailControl = SfUIHelper.CreateRoundedTextBox(
                "Enter your email",
                new Point(inputX, currentY),
                inputWidth,
                45
            );
            emailTextBox = emailControl.textBox;
            cardPanel.Controls.Add(emailControl.container);

            currentY += 58;

            // 5. Password Label
            passwordLabel = SfUIHelper.CreateLabel(
                "Password",
                new Point(labelX, currentY),
                inputWidth,
                20,
                FontStyle.Regular
            );
            cardPanel.Controls.Add(passwordLabel);
            currentY += 25;

            // 6. Password TextBox (Rounded)
            var passwordControl = SfUIHelper.CreateRoundedTextBox(
                "",
                new Point(inputX, currentY),
                inputWidth,
                45,
                true // isPassword
            );
            passwordTextBox = passwordControl.textBox;
            cardPanel.Controls.Add(passwordControl.container);

            currentY += 55;
            
            // 7. Forgot Password Link
            forgotPasswordButton = SfUIHelper.CreateSecondaryButton(
                "Forgot Password?",
                new Point(inputX, currentY),
                inputWidth,
                30
            );
            forgotPasswordButton.Style.BackColor = Color.Transparent;
            forgotPasswordButton.Style.Border = new Pen(Color.Transparent, 0);
            forgotPasswordButton.Style.ForeColor = Color.Gray;
            forgotPasswordButton.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            forgotPasswordButton.Click += ForgotPasswordButton_Click;
            
            // Right align
            forgotPasswordButton.Width = 130;
            forgotPasswordButton.Location = new Point(inputX + inputWidth - 130, currentY);
            
            cardPanel.Controls.Add(forgotPasswordButton);
            
            currentY += 35;

            // 8. Login Button
            loginButton = SfUIHelper.CreateModernPrimaryButton(
                "Login",
                new Point(inputX, currentY),
                inputWidth,
                50
            );
            loginButton.Click += LoginButton_Click;
            cardPanel.Controls.Add(loginButton);

            currentY += 60;

            // 9. Register Button
            registerButton = SfUIHelper.CreateSecondaryButton(
                "Create New Account",
                new Point(inputX, currentY),
                inputWidth,
                46
            );
            // Override style for "Outlined" look if needed
            registerButton.Style.BackColor = Color.White;
            registerButton.Style.ForeColor = ConfigurationConstants.SSDarkNavyColor;
            registerButton.Style.Border = new Pen(ConfigurationConstants.SSBorderGray, 1);
            
            registerButton.Click += RegisterButton_Click;
            cardPanel.Controls.Add(registerButton);
        }

        private void CreateTeamInfo()
        {
            int teamY = CARD_HEIGHT - 40;
            
            teamLabel = SfUIHelper.CreateLabel(
                "Developed by: Muhammad Zubair, Hamna Iman & Ayesha Aziz",
                new Point(0, teamY),
                CARD_WIDTH,
                30,
                FontStyle.Italic
            );
            teamLabel.Font = new Font("Segoe UI", 8, FontStyle.Italic);
            teamLabel.ForeColor = Color.Gray;
            teamLabel.AutoSize = true;
            cardPanel.Controls.Add(teamLabel);
            
            // Center it
            teamLabel.Location = new Point((CARD_WIDTH - teamLabel.Width) / 2, teamY);
        }

        #region Event Handlers

        private void LoginButton_Click(object sender, EventArgs e)
        {
            // Get values
            string role = roleComboBox.SelectedItem?.ToString();
            string email = emailTextBox.Text.Trim();
            string password = passwordTextBox.Text;

            // Validate role
            if (string.IsNullOrEmpty(role))
            {
                ShowError("Please select your role");
                roleComboBox.Focus();
                return;
            }

            // Validate email
            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError("Please enter your email address");
                emailTextBox.Focus();
                return;
            }

            // Validate password
            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Please enter your password");
                passwordTextBox.Focus();
                return;
            }

            // Raise event for parent form to handle
            OnLoginAttempt?.Invoke(this, new LoginEventArgs
            {
                Role = role,
                Email = email,
                Password = password
            });
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            OnRegisterClicked?.Invoke(this, EventArgs.Empty);
        }

        private void ForgotPasswordButton_Click(object sender, EventArgs e)
        {
             OnForgotPasswordClicked?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Helpers

        private void ShowError(string message)
        {
            MessageBox.Show(
                message,
                "Validation Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Clears the login form
        /// </summary>
        public void ClearForm()
        {
            roleComboBox.SelectedIndex = 0;
            emailTextBox.Clear();
            passwordTextBox.Clear();
        }

        #endregion
    }

    /// <summary>
    /// Event args for login attempt
    /// </summary>
    public class LoginEventArgs : EventArgs
    {
        public string Role { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
