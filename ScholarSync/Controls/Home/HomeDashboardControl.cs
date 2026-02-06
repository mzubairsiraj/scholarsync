using System;
using System.Drawing;
using System.Windows.Forms;
using ScholarSync.Commons;
using ScholarSync.Models;
using ScholarSync.Db_Connection;
using Npgsql;

namespace ScholarSync.Controls.Home
{
    public partial class HomeDashboardControl : UserControl
    {
        private Panel headerPanel;
        private Label lblWelcome;
        private Label lblDateTime;
        private Timer timeTimer;

        // Summary Cards Panel
        private Panel summaryCardsPanel;
        private Panel studentCard;
        private Panel teacherCard;
        private Panel userInfoCard;
        private Panel quickActionsCard;

        // User Information
        private string currentUserName = "Guest";
        private string currentUserRole = "Unknown";

        // Statistics Data
        private int totalStudents = 0;
        private int studentsThisMonth = 0;
        private int totalTeachers = 0;
        private int teachersThisMonth = 0;
        private int totalPrograms = 0;
        private int totalActiveUsers = 0;

        // Loading Indicator
        private LoadingIndicator loadingIndicator;

        // Reference to parent dashboard for navigation
        public event EventHandler<string> OnQuickActionClicked;

        public HomeDashboardControl()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = ConfigurationConstants.SSWhiteColor;
            this.AutoScroll = true;
            
            // Load user from session
            LoadUserFromSession();
            
            InitializeUI();
            StartTimerUpdate();
            
            // Initialize loading indicator
            loadingIndicator = new LoadingIndicator();
            this.Controls.Add(loadingIndicator);
            loadingIndicator.BringToFront();
            
            // Load statistics from database
            LoadDashboardStatistics();
            
            // Subscribe to session events
            SessionManager.OnUserLoggedIn += SessionManager_OnUserLoggedIn;
            SessionManager.OnUserLoggedOut += SessionManager_OnUserLoggedOut;
        }

        private void LoadUserFromSession()
        {
            if (SessionManager.IsLoggedIn)
            {
                currentUserName = SessionManager.GetUserName();
                currentUserRole = SessionManager.GetUserRole();
            }
        }

        private void LoadDashboardStatistics()
        {
            loadingIndicator.Show("Loading Dashboard Data");

            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    // Get total students count
                    string studentQuery = "SELECT COUNT(*) FROM students";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(studentQuery, conn))
                    {
                        totalStudents = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Get students added this month (if created_at column exists, otherwise skip)
                    try
                    {
                        string studentMonthQuery = @"SELECT COUNT(*) FROM students 
                                                    WHERE EXTRACT(MONTH FROM created_at) = EXTRACT(MONTH FROM CURRENT_DATE)
                                                    AND EXTRACT(YEAR FROM created_at) = EXTRACT(YEAR FROM CURRENT_DATE)";
                        using (NpgsqlCommand cmd = new NpgsqlCommand(studentMonthQuery, conn))
                        {
                            studentsThisMonth = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                    }
                    catch
                    {
                        // If created_at column doesn't exist, set to 0
                        studentsThisMonth = 0;
                    }

                    // Get total teachers count
                    string teacherQuery = "SELECT COUNT(*) FROM teachers";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(teacherQuery, conn))
                    {
                        totalTeachers = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Get teachers added this month (if created_at column exists, otherwise skip)
                    try
                    {
                        string teacherMonthQuery = @"SELECT COUNT(*) FROM teachers 
                                                    WHERE EXTRACT(MONTH FROM created_at) = EXTRACT(MONTH FROM CURRENT_DATE)
                                                    AND EXTRACT(YEAR FROM created_at) = EXTRACT(YEAR FROM CURRENT_DATE)";
                        using (NpgsqlCommand cmd = new NpgsqlCommand(teacherMonthQuery, conn))
                        {
                            teachersThisMonth = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                    }
                    catch
                    {
                        // If created_at column doesn't exist, set to 0
                        teachersThisMonth = 0;
                    }

                    // Get total programs
                    string programQuery = "SELECT COUNT(*) FROM programs WHERE is_active = true";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(programQuery, conn))
                    {
                        totalPrograms = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Get total active users
                    string userQuery = "SELECT COUNT(*) FROM users";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(userQuery, conn))
                    {
                        totalActiveUsers = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }

                // Refresh the UI with new data
                RefreshStatisticsCards();
                
                loadingIndicator.Hide();
            }
            catch (Exception ex)
            {
                loadingIndicator.Hide();
                MessageBox.Show($"Error loading dashboard statistics: {ex.Message}",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void RefreshStatisticsCards()
        {
            if (summaryCardsPanel == null) return;

            // Remove old cards
            summaryCardsPanel.Controls.Clear();

            // Re-create cards with updated data
            int cardWidth = 280;
            int cardHeight = 160;
            int spacing = 30;
            int yPos = 0;

            // Student Card with real data
            string studentSubtitle = studentsThisMonth > 0 
                ? $"+{studentsThisMonth} this month" 
                : "All time total";
            
            studentCard = CreateStatCard(
                "Total Students",
                totalStudents.ToString(),
                studentSubtitle,
                ConfigurationConstants.SSDarkNavyColor,
                0,
                yPos
            );

            // Teacher Card with real data
            string teacherSubtitle = teachersThisMonth > 0 
                ? $"+{teachersThisMonth} this month" 
                : "All time total";
            
            teacherCard = CreateStatCard(
                "Total Teachers",
                totalTeachers.ToString(),
                teacherSubtitle,
                ConfigurationConstants.SSDarkBlueColor,
                cardWidth + spacing,
                yPos
            );

            // User Info Card
            userInfoCard = CreateUserInfoCard((cardWidth + spacing) * 2, yPos, cardWidth * 2 + spacing, cardHeight);

            summaryCardsPanel.Controls.Add(studentCard);
            summaryCardsPanel.Controls.Add(teacherCard);
            summaryCardsPanel.Controls.Add(userInfoCard);

            // Row 2: Quick Actions Card
            yPos += cardHeight + spacing;
            int fullWidth = (cardWidth * 4) + (spacing * 3);
            quickActionsCard = CreateQuickActionsCard(0, yPos, fullWidth, cardWidth + 200);
            summaryCardsPanel.Controls.Add(quickActionsCard);
        }

        private void SessionManager_OnUserLoggedIn(object sender, UserModel user)
        {
            
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    currentUserName = user.UserName;
                    currentUserRole = user.Role;
                    RefreshUserInfo();
                    LoadDashboardStatistics();
                }));
            }
            else
            {
                currentUserName = user.UserName;
                currentUserRole = user.Role;
                RefreshUserInfo();
                LoadDashboardStatistics();
            }
        }

        private void SessionManager_OnUserLoggedOut(object sender, EventArgs e)
        {
            // Reset to default when logout occurs
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    currentUserName = "Guest";
                    currentUserRole = "Unknown";
                    RefreshUserInfo();
                }));
            }
            else
            {
                currentUserName = "Guest";
                currentUserRole = "Unknown";
                RefreshUserInfo();
            }
        }

        private void RefreshUserInfo()
        {
            if (lblWelcome != null)
            {
                lblWelcome.Text = $"Welcome back, {currentUserName}!";
            }

            if (userInfoCard != null && summaryCardsPanel != null)
            {
                // Refresh the user info card
                int cardWidth = 280;
                int spacing = 30;
                summaryCardsPanel.Controls.Remove(userInfoCard);
                userInfoCard = CreateUserInfoCard((cardWidth + spacing) * 2, 0, cardWidth * 2 + spacing, 160);
                summaryCardsPanel.Controls.Add(userInfoCard);
            }
        }

        private void InitializeUI()
        {
            // Header Panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = ConfigurationConstants.SSDarkNavyColor,
                Padding = new Padding(40, 20, 40, 20)
            };

            lblWelcome = new Label
            {
                Text = $"Welcome back, {currentUserName}!",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(40, 20),
                AutoSize = true
            };

            lblDateTime = new Label
            {
                Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy  |  hh:mm:ss tt"),
                Font = new Font("Arial", 12, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(40, 65),
                AutoSize = true
            };

            headerPanel.Controls.Add(lblWelcome);
            headerPanel.Controls.Add(lblDateTime);
            this.Controls.Add(headerPanel);

            
            summaryCardsPanel = new Panel
            {
                Location = new Point(40, 140),
                Size = new Size(ConfigurationConstants.ScreenWidth - 340, 600),
                BackColor = ConfigurationConstants.SSWhiteColor
            };
            this.Controls.Add(summaryCardsPanel);

            InitializeSummaryCards();
        }

        private void InitializeSummaryCards()
        {
            int cardWidth = 280;
            int cardHeight = 160;
            int spacing = 30;
            int yPos = 0;

            // Row 1: Statistics Cards (Students, Teachers) and User Info Card
            // Student Card - will show real data once loaded
            studentCard = CreateStatCard(
                "Total Students",
                totalStudents.ToString(),
                studentsThisMonth > 0 ? $"+{studentsThisMonth} this month" : "Loading...",
                ConfigurationConstants.SSDarkNavyColor,
                0,
                yPos
            );

            // Teacher Card - will show real data once loaded
            teacherCard = CreateStatCard(
                "Total Teachers",
                totalTeachers.ToString(),
                teachersThisMonth > 0 ? $"+{teachersThisMonth} this month" : "Loading...",
                ConfigurationConstants.SSDarkBlueColor,
                cardWidth + spacing,
                yPos
            );

            // User Info Card - on the right side of row 1
            userInfoCard = CreateUserInfoCard((cardWidth + spacing) * 2, yPos, cardWidth * 2 + spacing, cardHeight);
            
            summaryCardsPanel.Controls.Add(studentCard);
            summaryCardsPanel.Controls.Add(teacherCard);
            summaryCardsPanel.Controls.Add(userInfoCard);

            // Row 2: Quick Actions Card - Full width below
            yPos += cardHeight + spacing;

            // Quick Actions Card - Full width
            int fullWidth = (cardWidth * 4) + (spacing * 3);
            quickActionsCard = CreateQuickActionsCard(0, yPos, fullWidth, cardWidth + 200);
            summaryCardsPanel.Controls.Add(quickActionsCard);
        }

        private Panel CreateStatCard(string title, string value, string subtitle, Color accentColor, int x, int y)
        {
            Panel card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(280, 160),
                BackColor = ConfigurationConstants.SSWhiteColor,
                BorderStyle = BorderStyle.FixedSingle,
                Tag = title // Store title for click handling
            };

            // Accent bar on left
            Panel accentBar = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(8, 160),
                BackColor = accentColor
            };
            card.Controls.Add(accentBar);

            // Title
            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(25, 20),
                Size = new Size(240, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(lblTitle);

            // Value
            Label lblValue = new Label
            {
                Text = value,
                Font = new Font("Arial", 36, FontStyle.Bold),
                ForeColor = accentColor,
                Location = new Point(25, 50),
                Size = new Size(240, 50),
                TextAlign = ContentAlignment.MiddleLeft,
                Tag = "value" // Tag to identify for updates
            };
            card.Controls.Add(lblValue);

            // Subtitle
            Label lblSubtitle = new Label
            {
                Text = subtitle,
                Font = new Font("Arial", 10, FontStyle.Regular),
                ForeColor = Color.Gray,
                Location = new Point(25, 110),
                Size = new Size(240, 30),
                TextAlign = ContentAlignment.MiddleLeft,
                Tag = "subtitle" // Tag to identify for updates
            };
            card.Controls.Add(lblSubtitle);

            // Click to view details
            card.Click += StatCard_Click;
            lblTitle.Click += StatCard_Click;
            lblValue.Click += StatCard_Click;
            lblSubtitle.Click += StatCard_Click;

            // Hover effect
            card.MouseEnter += (s, e) =>
            {
                card.BackColor = ConfigurationConstants.SSLightGrayBackground;
                card.Cursor = Cursors.Hand;
            };

            card.MouseLeave += (s, e) =>
            {
                card.BackColor = ConfigurationConstants.SSWhiteColor;
            };

            return card;
        }

        private void StatCard_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            Panel card = control as Panel ?? control.Parent as Panel;

            if (card != null && card.Tag != null)
            {
                string cardTitle = card.Tag.ToString();
                
                if (cardTitle == "Total Students")
                {
                    OnQuickActionClicked?.Invoke(this, "List Students");
                }
                else if (cardTitle == "Total Teachers")
                {
                    OnQuickActionClicked?.Invoke(this, "List Teachers");
                }
            }
        }

        private Panel CreateUserInfoCard(int x, int y, int width, int height)
        {
            Panel card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = ConfigurationConstants.SSLightNavyColor,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(30)
            };

            // Title
            Label lblTitle = new Label
            {
                Text = "Current User Information",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(30, 20),
                Size = new Size(width - 60, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(lblTitle);

            // User Icon (simplified with label)
            Panel iconPanel = new Panel
            {
                Location = new Point(30, 60),
                Size = new Size(80, 80),
                BackColor = ConfigurationConstants.SSDarkNavyColor
            };

            Label iconLabel = new Label
            {
                Text = currentUserName.Substring(0, 1).ToUpper(),
                Font = new Font("Arial", 36, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            iconPanel.Controls.Add(iconLabel);
            card.Controls.Add(iconPanel);

            // User Details
            int detailX = 130;
            int detailY = 70;

            Label lblUserNameTitle = new Label
            {
                Text = "Name:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(detailX, detailY),
                Size = new Size(80, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(lblUserNameTitle);

            Label lblUserNameValue = new Label
            {
                Text = currentUserName,
                Font = new Font("Arial", 11, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(detailX + 85, detailY),
                Size = new Size(300, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(lblUserNameValue);

            Label lblRoleTitle = new Label
            {
                Text = "Role:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(detailX, detailY + 30),
                Size = new Size(80, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(lblRoleTitle);

            Label lblRoleValue = new Label
            {
                Text = currentUserRole,
                Font = new Font("Arial", 11, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSSuccessGreen,
                Location = new Point(detailX + 85, detailY + 30),
                Size = new Size(300, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(lblRoleValue);

            return card;
        }

        private Panel CreateQuickActionsCard(int x, int y, int width, int height)
        {
            Panel card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = ConfigurationConstants.SSWhiteColor,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(30)
            };

            // Title
            Label lblTitle = new Label
            {
                Text = "Quick Actions",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(30, 20),
                Size = new Size(width - 60, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(lblTitle);

            // Quick Action Buttons - 3 columns layout
            int btnY = 70;
            int btnSpacing = 20;
            int btnWidth = 280;
            int btnHeight = 45;
            int columnSpacing = 50;

            // Column 1 - Student Actions (Only show if admin)
            int col1X = 30;
            
            if (SessionManager.IsAdmin())
            {
                Button btnAddStudent = UIHelper.CreateButton("+ Register New Student", ConfigurationConstants.SSDarkNavyColor, new Point(col1X, btnY), new Size(btnWidth, btnHeight));
                btnAddStudent.Click += (s, e) => OnQuickActionClicked?.Invoke(this, "Register Student");
                card.Controls.Add(btnAddStudent);
            }

            btnY += btnHeight + btnSpacing;
            Button btnViewStudents = UIHelper.CreateButton("📋 View All Students", ConfigurationConstants.SSDarkNavyColor, new Point(col1X, btnY), new Size(btnWidth, btnHeight));
            btnViewStudents.Click += (s, e) => OnQuickActionClicked?.Invoke(this, "List Students");
            card.Controls.Add(btnViewStudents);

            btnY += btnHeight + btnSpacing;
            Button btnUpdateStudent = UIHelper.CreateButton("✏️ Update Student", ConfigurationConstants.SSDarkNavyColor, new Point(col1X, btnY), new Size(btnWidth, btnHeight));
            btnUpdateStudent.Click += (s, e) => OnQuickActionClicked?.Invoke(this, "Update Student");
            card.Controls.Add(btnUpdateStudent);

            // Column 2 - Teacher Actions
            int col2X = col1X + btnWidth + columnSpacing;
            btnY = 70;
            
            if (SessionManager.IsAdmin())
            {
                Button btnAddTeacher = UIHelper.CreateButton("+ Create New Teacher", ConfigurationConstants.SSDarkBlueColor, new Point(col2X, btnY), new Size(btnWidth, btnHeight));
                btnAddTeacher.Click += (s, e) => OnQuickActionClicked?.Invoke(this, "Create Teacher");
                card.Controls.Add(btnAddTeacher);
            }

            btnY += btnHeight + btnSpacing;
            Button btnViewTeachers = UIHelper.CreateButton("📋 View All Teachers", ConfigurationConstants.SSDarkBlueColor, new Point(col2X, btnY), new Size(btnWidth, btnHeight));
            btnViewTeachers.Click += (s, e) => OnQuickActionClicked?.Invoke(this, "List Teachers");
            card.Controls.Add(btnViewTeachers);

            btnY += btnHeight + btnSpacing;
            if (SessionManager.IsAdmin())
            {
                Button btnUpdateTeacher = UIHelper.CreateButton("✏️ Update Teacher", ConfigurationConstants.SSDarkBlueColor, new Point(col2X, btnY), new Size(btnWidth, btnHeight));
                btnUpdateTeacher.Click += (s, e) => OnQuickActionClicked?.Invoke(this, "Update Teacher");
                card.Controls.Add(btnUpdateTeacher);
            }

            // Column 3 - Result Actions
            int col3X = col2X + btnWidth + columnSpacing;
            btnY = 70;
            
            if (SessionManager.IsAdmin())
            {
                Button btnAddResult = UIHelper.CreateButton("+ Add Student Result", ConfigurationConstants.SSSuccessGreen, new Point(col3X, btnY), new Size(btnWidth, btnHeight));
                btnAddResult.Click += (s, e) => OnQuickActionClicked?.Invoke(this, "Add Result");
                card.Controls.Add(btnAddResult);
            }

            btnY += btnHeight + btnSpacing;
            Button btnViewResults = UIHelper.CreateButton("📋 View All Results", ConfigurationConstants.SSSuccessGreen, new Point(col3X, btnY), new Size(btnWidth, btnHeight));
            btnViewResults.Click += (s, e) => OnQuickActionClicked?.Invoke(this, "View Results");
            card.Controls.Add(btnViewResults);

            btnY += btnHeight + btnSpacing;
            if (SessionManager.IsAdmin())
            {
                Button btnUpdateResult = UIHelper.CreateButton("✏️ Update Result", ConfigurationConstants.SSSuccessGreen, new Point(col3X, btnY), new Size(btnWidth, btnHeight));
                btnUpdateResult.Click += (s, e) => OnQuickActionClicked?.Invoke(this, "Update Result");
                card.Controls.Add(btnUpdateResult);
            }

            return card;
        }

        private void StartTimerUpdate()
        {
            timeTimer = new Timer();
            timeTimer.Interval = 1000; // Update every second
            timeTimer.Tick += TimeTimer_Tick;
            timeTimer.Start();
        }

        private void TimeTimer_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy  |  hh:mm:ss tt");
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            // Unsubscribe from events
            SessionManager.OnUserLoggedIn -= SessionManager_OnUserLoggedIn;
            SessionManager.OnUserLoggedOut -= SessionManager_OnUserLoggedOut;
            
            if (timeTimer != null)
            {
                timeTimer.Stop();
                timeTimer.Dispose();
            }
            base.OnHandleDestroyed(e);
        }

        // Public method to manually set user information (optional - session is primary)
        public void SetUserInfo(string userName, string userRole)
        {
            currentUserName = userName;
            currentUserRole = userRole;
            RefreshUserInfo();
        }
    }
}
