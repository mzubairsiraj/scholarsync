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
    public partial class OtpVerificationControl : UserControl
    {
        // UI Constants
        private const int CARD_WIDTH = 450;
        private const int CARD_HEIGHT = 450;
        private const int LOGO_SIZE = 70;

        // UI Components
        private GradientPanel mainPanel;
        private GradientPanel cardPanel;
        
        // Logo and Title
        private PictureBox logoBox;
        private AutoLabel titleLabel;
        private AutoLabel descriptionLabel;
        private AutoLabel emailDisplayLabel;
        
        // Form Controls
        private AutoLabel otpLabel;
        private TextBoxExt otpTextBox;
        private SfButton verifyButton;
        private SfButton resendButton;
        private SfButton backButton;
        
        // Events
        public event EventHandler OnBackToLogin;
        public event EventHandler<string> OnVerified; // Passes OTP code

        // Data
        private string targetEmail;

        public OtpVerificationControl()
        {
            InitializeUI();
        }

        public void SetTargetEmail(string email)
        {
            targetEmail = email;
            if (emailDisplayLabel != null)
            {
                emailDisplayLabel.Text = $"Sent to: {maskedEmail(email)}";
                
                // Recenter
                emailDisplayLabel.Location = new Point((CARD_WIDTH - emailDisplayLabel.Width) / 2, emailDisplayLabel.Location.Y);
            }
        }
        
        private string maskedEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@")) return email;
            var parts = email.Split('@');
            if (parts[0].Length <= 3) return email;
            
            return $"{parts[0].Substring(0, 3)}***@{parts[1]}";
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
                "Verify It's You",
                Point.Empty,
                16
            );
            titleLabel.Location = new Point((CARD_WIDTH - titleLabel.Width) / 2, currentY);
            cardPanel.Controls.Add(titleLabel);

            currentY += 35;

            // Description
            emailDisplayLabel = SfUIHelper.CreateLabel(
                "Enter the code sent to your email",
                Point.Empty,
                CARD_WIDTH - 60,
                25,
                FontStyle.Regular
            );
            emailDisplayLabel.ForeColor = Color.Gray;
            emailDisplayLabel.AutoSize = true; 
            emailDisplayLabel.Location = new Point((CARD_WIDTH - emailDisplayLabel.Width) / 2, currentY);
            cardPanel.Controls.Add(emailDisplayLabel);
        }

        private void CreateFormControls()
        {
            int currentY = 200;
            int marginX = 40;
            int width = CARD_WIDTH - (marginX * 2);

            // OTP Label
            otpLabel = SfUIHelper.CreateLabel(
                "Verification Code",
                new Point(marginX, currentY),
                width,
                20,
                FontStyle.Bold
            );
            cardPanel.Controls.Add(otpLabel);
            currentY += 25;

            // OTP Input
            var otpControl = SfUIHelper.CreateRoundedTextBox(
                "000000",
                new Point(marginX, currentY),
                width,
                45
            );
            otpTextBox = otpControl.textBox;
            otpTextBox.MaxLength = 6;
            otpTextBox.TextAlign = HorizontalAlignment.Center;
            otpTextBox.Font = new Font("Segoe UI", 16, FontStyle.Bold); // Larger font for OTP
            cardPanel.Controls.Add(otpControl.container);

            currentY += 60;

            // Verify Button
            verifyButton = SfUIHelper.CreateModernPrimaryButton(
                "Verify Code",
                new Point(marginX, currentY),
                width,
                48
            );
            verifyButton.Click += VerifyButton_Click;
            cardPanel.Controls.Add(verifyButton);

            currentY += 55;

            // Resend Code
            resendButton = SfUIHelper.CreateSecondaryButton(
                "Resend Code",
                new Point(marginX, currentY),
                width,
                30
            );
            resendButton.Style.BackColor = Color.Transparent;
            resendButton.Style.Border = new Pen(Color.Transparent, 0);
            resendButton.Style.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            resendButton.Font = new Font("Segoe UI", SfUIHelper.FONT_SIZE_MEDIUM, FontStyle.Regular);
            resendButton.Click += ResendButton_Click;
            cardPanel.Controls.Add(resendButton);

            currentY += 35;
            
            // Back to Login
            backButton = SfUIHelper.CreateSecondaryButton(
                "Back to Login",
                new Point(marginX, currentY),
                width,
                30
            );
            backButton.Style.BackColor = Color.Transparent;
            backButton.Style.Border = new Pen(Color.Transparent, 0);
            backButton.Style.ForeColor = Color.Gray;
            backButton.Click += BackButton_Click;
            cardPanel.Controls.Add(backButton);
        }

        private void VerifyButton_Click(object sender, EventArgs e)
        {
            string otp = otpTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(otp) || otp.Length < 6)
            {
                 MessageBox.Show("Please enter a valid 6-digit code.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 return;
            }

            OnVerified?.Invoke(this, otp);
        }

        private void ResendButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Verification code resent to {maskEmailForMsg(targetEmail)}", "Code Resent", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private string maskEmailForMsg(string email) => string.IsNullOrEmpty(email) ? "your email" : email;


        private void BackButton_Click(object sender, EventArgs e)
        {
            OnBackToLogin?.Invoke(this, EventArgs.Empty);
        }
    }
}
