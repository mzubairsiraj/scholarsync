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
    public partial class ResetPasswordControl : UserControl
    {
        // UI Constants
        private const int CARD_WIDTH = 450;
        private const int CARD_HEIGHT = 520;
        private const int LOGO_SIZE = 70;

        // UI Components
        private GradientPanel mainPanel;
        private GradientPanel cardPanel;
        
        // Logo and Title
        private PictureBox logoBox;
        private AutoLabel titleLabel;
        private AutoLabel descriptionLabel;
        
        // Form Controls
        private AutoLabel newPasswordLabel;
        private TextBoxExt newPasswordTextBox;
        private AutoLabel confirmPasswordLabel;
        private TextBoxExt confirmPasswordTextBox;
        private SfButton resetPasswordButton;
        
        // Events
        public event EventHandler OnPasswordReset;

        public ResetPasswordControl()
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
                "Reset Password",
                Point.Empty,
                16
            );
            titleLabel.Location = new Point((CARD_WIDTH - titleLabel.Width) / 2, currentY);
            cardPanel.Controls.Add(titleLabel);

            currentY += 35;

            // Description
            descriptionLabel = SfUIHelper.CreateLabel(
                "Enter your new password below.",
                Point.Empty,
                CARD_WIDTH - 60,
                25,
                FontStyle.Regular
            );
            descriptionLabel.ForeColor = Color.Gray;
            descriptionLabel.AutoSize = true; 
            descriptionLabel.Location = new Point((CARD_WIDTH - descriptionLabel.Width) / 2, currentY);
            cardPanel.Controls.Add(descriptionLabel);
        }

        private void CreateFormControls()
        {
            int currentY = 180;
            int marginX = 40;
            int width = CARD_WIDTH - (marginX * 2);

            // New Password Label
            newPasswordLabel = SfUIHelper.CreateLabel(
                "New Password",
                new Point(marginX, currentY),
                width,
                20,
                FontStyle.Bold
            );
            cardPanel.Controls.Add(newPasswordLabel);
            currentY += 25;

            // New Password Input
            var newPassControl = SfUIHelper.CreateRoundedTextBox(
                "",
                new Point(marginX, currentY),
                width,
                45,
                true // isPassword
            );
            newPasswordTextBox = newPassControl.textBox;
            cardPanel.Controls.Add(newPassControl.container);

            currentY += 60;

            // Confirm Password Label
            confirmPasswordLabel = SfUIHelper.CreateLabel(
                "Confirm Password",
                new Point(marginX, currentY),
                width,
                20,
                FontStyle.Bold
            );
            cardPanel.Controls.Add(confirmPasswordLabel);
            currentY += 25;

            // Confirm Password Input
            var confirmPassControl = SfUIHelper.CreateRoundedTextBox(
                "",
                new Point(marginX, currentY),
                width,
                45,
                true // isPassword
            );
            confirmPasswordTextBox = confirmPassControl.textBox;
            cardPanel.Controls.Add(confirmPassControl.container);

            currentY += 70;

            // Reset Button
            resetPasswordButton = SfUIHelper.CreateModernPrimaryButton(
                "Reset Password",
                new Point(marginX, currentY),
                width,
                48
            );
            resetPasswordButton.Click += ResetPasswordButton_Click;
            cardPanel.Controls.Add(resetPasswordButton);
        }

        private void ResetPasswordButton_Click(object sender, EventArgs e)
        {
            string newPass = newPasswordTextBox.Text;
            string confirmPass = confirmPasswordTextBox.Text;

            // Validation
            if (string.IsNullOrWhiteSpace(newPass) || newPass.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPass != confirmPass)
            {
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Success
            MessageBox.Show("Your password has been reset successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            // Navigate back to login
            OnPasswordReset?.Invoke(this, EventArgs.Empty);
        }
    }
}
