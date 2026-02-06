using System;
using System.Drawing;
using System.Windows.Forms;
using ScholarSync.Commons;

namespace ScholarSync.Controls
{
    /// <summary>
    /// Global loading indicator for database operations
    /// Shows a semi-transparent overlay with animated loading message
    /// </summary>
    public class LoadingIndicator : Panel
    {
        private Panel loadingPanel;
        private Label lblLoading;
        private Label lblMessage;
        private Timer animationTimer;
        private int dotCount = 0;
        private string baseMessage = "Loading";

        public LoadingIndicator()
        {
            InitializeLoadingIndicator();
        }

        private void InitializeLoadingIndicator()
        {
            // Main overlay panel (semi-transparent)
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(128, 0, 0, 0);
            this.Visible = false;
            this.BringToFront();

            // Loading content panel
            loadingPanel = new Panel
            {
                Size = new Size(300, 150),
                BackColor = ConfigurationConstants.SSWhiteColor,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Center the loading panel
            loadingPanel.Location = new Point(
                (this.Width - loadingPanel.Width) / 2,
                (this.Height - loadingPanel.Height) / 2
            );

            // Loading icon/text
            lblLoading = new Label
            {
                Text = "?",
                Font = new Font("Arial", 48, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(0, 20),
                Size = new Size(300, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Loading message
            lblMessage = new Label
            {
                Text = "Loading...",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(0, 90),
                Size = new Size(300, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            loadingPanel.Controls.Add(lblLoading);
            loadingPanel.Controls.Add(lblMessage);
            this.Controls.Add(loadingPanel);

            // Animation timer
            animationTimer = new Timer();
            animationTimer.Interval = 500;
            animationTimer.Tick += AnimationTimer_Tick;

            // Handle resize to keep loading panel centered
            this.Resize += LoadingIndicator_Resize;
        }

        private void LoadingIndicator_Resize(object sender, EventArgs e)
        {
            if (loadingPanel != null)
            {
                loadingPanel.Location = new Point(
                    (this.Width - loadingPanel.Width) / 2,
                    (this.Height - loadingPanel.Height) / 2
                );
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            dotCount = (dotCount + 1) % 4;
            string dots = new string('.', dotCount);
            lblMessage.Text = baseMessage + dots;
        }


        public void Show(string message = "Loading")
        {
            baseMessage = message;
            lblMessage.Text = message + "...";
            dotCount = 0;
            this.Visible = true;
            this.BringToFront();
            animationTimer.Start();
            
            // Force the UI to refresh and display the loading indicator
            this.Refresh();
            Application.DoEvents();
        }

        public new void Hide()
        {
            animationTimer.Stop();
            this.Visible = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (animationTimer != null)
                {
                    animationTimer.Stop();
                    animationTimer.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
