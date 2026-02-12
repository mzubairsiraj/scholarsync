using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Syncfusion.WinForms.Controls;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Input;
using ScholarSync.Commons;
using ScholarSync.Configuration;
using ScholarSync.UIComponents;

namespace ScholarSync.Controls.Auth
{
    public partial class ForgotPasswordControl : UserControl
    {
        // UI Constants
        private const int CARD_WIDTH = 450;
        private const int CARD_HEIGHT = 420;
        private const int LOGO_SIZE = 70;

        // UI Components
        private GradientPanel mainPanel;
        private GradientPanel cardPanel;
        
        // Logo and Title
        private PictureBox logoBox;
        private AutoLabel titleLabel;
        private AutoLabel descriptionLabel;
        
        // Form Controls
        private AutoLabel emailLabel;
        private TextBoxExt emailTextBox;
        private SfButton sendCodeButton;
        private SfButton backToLoginButton;
        
        // Events
        public event EventHandler OnBackToLogin;
        public event EventHandler<string> OnVerificationRequested; // Passes email

        public ForgotPasswordControl()
        {
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

            currentY += LOGO_SIZE + 20;

            // Title
            titleLabel = SfUIHelper.CreateHeadingLabel(
                "Forgot Password?",
                Point.Empty,
                16
            );
            titleLabel.Location = new Point((CARD_WIDTH - titleLabel.Width) / 2, currentY);
            cardPanel.Controls.Add(titleLabel);

            currentY += 35;

            // Description
            string description = "Enter your email address to receive a verification code.";
            descriptionLabel = SfUIHelper.CreateLabel(
                description,
                Point.Empty,
                CARD_WIDTH - 60,
                40,
                FontStyle.Regular
            );
            descriptionLabel.ForeColor = Color.Gray;
            descriptionLabel.AutoSize = false;
            descriptionLabel.TextAlign = ContentAlignment.TopCenter;
            descriptionLabel.Location = new Point(30, currentY);
            cardPanel.Controls.Add(descriptionLabel);
        }

        private void CreateFormControls()
        {
            int currentY = 200;
            int marginX = 40;
            int width = CARD_WIDTH - (marginX * 2);

            // Email Label
            emailLabel = SfUIHelper.CreateLabel(
                "Email Address",
                new Point(marginX, currentY),
                width,
                20,
                FontStyle.Bold
            );
            cardPanel.Controls.Add(emailLabel);
            currentY += 25;

            // Email Input
            var emailControl = SfUIHelper.CreateRoundedTextBox(
                "Enter your email",
                new Point(marginX, currentY),
                width,
                45
            );
            emailTextBox = emailControl.textBox;
            cardPanel.Controls.Add(emailControl.container);

            currentY += 60;

            // Send Code Button
            sendCodeButton = SfUIHelper.CreateModernPrimaryButton(
                "Send Verification Code",
                new Point(marginX, currentY),
                width,
                48
            );
            sendCodeButton.Click += SendCodeButton_Click;
            cardPanel.Controls.Add(sendCodeButton);

            currentY += 60;

            // Back to Login
            backToLoginButton = SfUIHelper.CreateSecondaryButton(
                "Back to Login",
                new Point(marginX, currentY),
                width,
                30
            );
            
            backToLoginButton.Style.BackColor = Color.Transparent;
            backToLoginButton.Style.Border = new Pen(Color.Transparent, 0);
            backToLoginButton.Style.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            backToLoginButton.Font = new Font("Segoe UI", SfUIHelper.FONT_SIZE_MEDIUM, FontStyle.Regular);
            
            backToLoginButton.Click += BackToLoginButton_Click;
            cardPanel.Controls.Add(backToLoginButton);
        }

        private void SendCodeButton_Click(object sender, EventArgs e)
        {
             // Validate inputs
            string email = emailTextBox.Text.Trim();
             if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Please enter your email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

             // Simulating sending Code
             // In real app, call service here
             
             // Navigate to OTP Control
             OnVerificationRequested?.Invoke(this, email);
        }

        private void BackToLoginButton_Click(object sender, EventArgs e)
        {
            OnBackToLogin?.Invoke(this, EventArgs.Empty);
        }
    }
}
