using ScholarSync.Commons;
using ScholarSync.Forms;
using ScholarSync.Models;
using ScholarSync.Db_Connection;
using ScholarSync.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using System.IO;


namespace ScholarSync
{
    public partial class WelcomeForm : Form
    {
        //Containers for Left and Right Panels
        Panel rightPanel;
        Panel leftPanel;

        //Left Panel Controls
        PictureBox logoPictureBox;
        Label titleBox;
        Label subtitleBox;
        Button viewManualBtn;
        Button CtaBtn;


        // Right Panel Controls

        Panel loginPanel;
        Panel loginCardPanel;

        Label loginTitleBox;
        Label CnicLabel;
        Label passwordLabel;
        Label userTypeLabel;

        ComboBox userTypeComboBox;

        TextBox CnicTextBox;
        TextBox passwordTextBox;

        Button loginButton;

        // Loading Indicator
        LoadingIndicator loadingIndicator;



        // College Specific Information

        String collegeName = "Government College Of Management Sciences \n No-1 D-I-Khan";


        public WelcomeForm()
        {
            InitializeComponent();
            this.Icon = ConfigurationConstants.LogoIcon;
            this.Text = "Welcome To ScholarSync | College Management System";
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(ConfigurationConstants.ScreenWidth, ConfigurationConstants.ScreenHeight);
            InitUI();


             
        
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            // Validate User Type Selection
            if (userTypeComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a user type.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                userTypeComboBox.Focus();
                return;
            }

            string selectedUserType = userTypeComboBox.SelectedItem.ToString();

            // Validate only Administrator and Teacher are allowed
            if (selectedUserType != "Admin" && selectedUserType != "Teacher")
            {
                MessageBox.Show("Only Administrator and Teacher can login.", "Access Denied",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                userTypeComboBox.Focus();
                return;
            }

            // Validate CNIC Input
            string cnic = CnicTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(cnic))
            {
                MessageBox.Show("Please enter your CNIC.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CnicTextBox.Focus();
                return;
            }

            // Validate CNIC Format (13 digits with optional hyphens: 12345-6789012-3)
            string cnicDigitsOnly = cnic.Replace("-", "");
            if (cnicDigitsOnly.Length != 13 || !cnicDigitsOnly.All(char.IsDigit))
            {
                MessageBox.Show("Please enter a valid CNIC (13 digits).\nFormat: 12345-6789012-3", "Invalid CNIC",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CnicTextBox.Focus();
                return;
            }

            // Validate Password
            string password = passwordTextBox.Text;
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter your password.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                passwordTextBox.Focus();
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                passwordTextBox.Focus();
                return;
            }

            // Authenticate User
            AuthenticateUser(cnicDigitsOnly, password, selectedUserType);
        }

        private void AuthenticateUser(string cnic, string password, string userType)
        {
            // Show loading indicator
            loadingIndicator.Show("Authenticating");
            
            // Give the UI time to refresh and show the loading indicator
            System.Threading.Thread.Sleep(100);

            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT id, cnic, name, email, role, created_at, updated_at 
                                   FROM users 
                                   WHERE cnic = @cnic AND password = @password AND role = @role::user_role";

                    

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@cnic", cnic);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@role", userType);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // User authenticated successfully
                                UserModel currentUser = new UserModel
                                {
                                    Id = reader["id"].ToString(),
                                    CNIC = reader["cnic"].ToString(),
                                    UserName = reader["name"].ToString(),
                                    Email = reader["email"].ToString(),
                                    Role = reader["role"].ToString(),
                                    CreatedAt = Convert.ToDateTime(reader["created_at"]),
                                    UpdatedAt = Convert.ToDateTime(reader["updated_at"])
                                };

                                // Set user in global session
                                SessionManager.Login(currentUser);

                                // Hide loading indicator
                                loadingIndicator.Hide();

                                // Show success message
                                MessageBox.Show($"Welcome, {currentUser.UserName}!\n\nRole: {currentUser.Role}",
                                    "Login Successful",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                                // Open Dashboard
                                DashBoardForm dashBoardForm = new DashBoardForm(this);
                                dashBoardForm.Show();
                                this.Hide();
                            }
                            else
                            {
                                // Hide loading indicator
                                loadingIndicator.Hide();

                                // Authentication failed
                                MessageBox.Show("Invalid CNIC or Password.\n\nPlease check your credentials and try again.",
                                    "Login Failed",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                passwordTextBox.Clear();
                                passwordTextBox.Focus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Hide loading indicator
                loadingIndicator.Hide();

                MessageBox.Show($"Login Error: {ex.Message}\n\nPlease try again or contact administrator.",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void RenderLeftPanel()
        {
            //Left Panel Content -> Logo, Title, Subtitle, View Manual Button, CTA Button
            int upperMargin = (leftPanel.Height / 10) + 80;


            int leftPanelCenterX = leftPanel.Width / 6;
            int leftPanelCenterY = (leftPanel.Height / 6) + upperMargin;


            PictureBox logoPictureBox = new PictureBox
            {
                Image = new Bitmap(Path.Join(AppDomain.CurrentDomain.BaseDirectory,"../../../Resourses","logo.png")),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(leftPanelCenterX + 200, leftPanelCenterY - 100),

                Size = new Size(150, 150)
            };


            titleBox = new Label
            {
                Text = $"Welcome to ScholarSync \n {collegeName}",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(leftPanelCenterX - 100, leftPanelCenterY + 60),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true
            };
            subtitleBox = new Label
            {
                Text = "Empowering Education. Simplifying Management.",
                Font = new Font("Arial", 16, FontStyle.Italic),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(leftPanelCenterX + 20, leftPanelCenterY + 180),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true
            };

            viewManualBtn = new Button
            {
                Text = "View User Manual",
                Font = new Font("Arial", 12, FontStyle.Regular),
                BackColor = ConfigurationConstants.SSLightNavyColor,
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Size = new Size(180, 40),
                Location = new Point(leftPanelCenterX + 50, leftPanelCenterY + 250),
                FlatStyle = FlatStyle.Flat
            };

            CtaBtn = new Button
            {
                Text = "Get Started",
                Font = new Font("Arial", 12, FontStyle.Regular),
                BackColor = ConfigurationConstants.SSDarkNavyColor,
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Size = new Size(180, 40),
                Location = new Point(leftPanelCenterX + 250, leftPanelCenterY + 250),
                FlatStyle = FlatStyle.Flat
            };


            leftPanel.Controls.Add(logoPictureBox);
            leftPanel.Controls.Add(titleBox);
            leftPanel.Controls.Add(subtitleBox);
            leftPanel.Controls.Add(viewManualBtn);
            leftPanel.Controls.Add(CtaBtn);

            // Add click handler for View User Manual button
            viewManualBtn.Click += (sender, eventArgs) =>
            {
                try
                {
                    System.Diagnostics.Process.Start("https://github.com/mzubairsiraj/scholarsync/blob/master/README.md");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unable to open user manual.\n\nError: {ex.Message}\n\nPlease visit: https://github.com/mzubairsiraj/scholarsync",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            };
        }
        private void RenderRightPanel()
        {
            //Right Panel Content -> Login Panel Content is sub container of the Right Panel: So login Controls will be added to loginPanel

            int loginCardPanelWidth = 450;
            int loginCardPanelHeight = 460;

            int topPaddingLoginCard = ((loginPanel.Width - loginCardPanelWidth) / 2) - 80;

            int labelWidth = 100;
            int labelHeight = 30;

            int labelLeftMargin = 30;

            int inputWidth = 300;
            int inputHeight = 50;

            int inputLeftMargin = 130;


            Panel loginCardContainer = new Panel
            {
                Size = new Size(loginCardPanelWidth, loginCardPanelHeight),
                BackColor = ConfigurationConstants.SSLightNavyColor,
                Location = new Point((loginPanel.Width - loginCardPanelWidth) / 2, (loginPanel.Height - loginCardPanelHeight) / 2),
            };

            loginCardPanel = new Panel
            {
                Size = new Size(loginCardPanelWidth - 2, loginCardPanelHeight - 2),
                BackColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(1, 1),


            };


            loginTitleBox = new Label
            {
                Text = "User Login",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point((loginCardPanel.Width / 2) - 60, topPaddingLoginCard - 10),
                AutoSize = true
            };

            userTypeLabel = new Label
            {
                Text = "User Type:",
                Font = new Font("Arial", 12, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(labelLeftMargin, topPaddingLoginCard + 60),
                Size = new Size(labelWidth, labelHeight),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false,


            };

            userTypeComboBox = new ComboBox
            {
                Font = new Font("Arial", 15, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(inputLeftMargin, topPaddingLoginCard + 60),
                Size = new Size(inputWidth, inputHeight),
                DropDownStyle = ComboBoxStyle.DropDownList,
            };

            userTypeComboBox.Items.AddRange(new string[]
            {
                "Admin",
                "Teacher",

            });
            userTypeComboBox.SelectedIndex = 0;



            CnicLabel = new Label
            {
                Text = "Cnic:",
                Font = new Font("Arial", 12, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(labelLeftMargin, topPaddingLoginCard + 120),
                Size = new Size(labelWidth, labelHeight),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };

            CnicTextBox = new TextBox
            {
                Font = new Font("Arial", 15, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(inputLeftMargin, topPaddingLoginCard + 120),
                Size = new Size(inputWidth, inputHeight),
                
            };


            passwordLabel = new Label
            {
                Text = "Password:",
                Font = new Font("Arial", 12, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(labelLeftMargin, topPaddingLoginCard + 160),
                Size = new Size(labelWidth, labelHeight),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };

            passwordTextBox = new TextBox
            {
                Font = new Font("Arial", 15, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(inputLeftMargin, topPaddingLoginCard + 160),
                Size = new Size(inputWidth, inputHeight),
                UseSystemPasswordChar = true,
            };

            loginButton = new Button
            {
                Text = "Login",
                Font = new Font("Arial", 12, FontStyle.Regular),
                BackColor = ConfigurationConstants.SSDarkNavyColor,
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Size = new Size(inputWidth, inputHeight),
                Location = new Point((loginCardPanel.Width - inputWidth) / 2, topPaddingLoginCard + 220),
                FlatStyle = FlatStyle.Flat
            };

            loginCardPanel.Controls.Add(loginTitleBox);
            loginCardPanel.Controls.Add(userTypeLabel);
            loginCardPanel.Controls.Add(userTypeComboBox);
            loginCardPanel.Controls.Add(CnicLabel);
            loginCardPanel.Controls.Add(CnicTextBox);
            loginCardPanel.Controls.Add(passwordLabel);
            loginCardPanel.Controls.Add(passwordTextBox);
            loginCardPanel.Controls.Add(loginButton);


            loginPanel.Controls.Add(loginCardContainer);
            loginCardContainer.Controls.Add(loginCardPanel);



            //UI Logic can be added here for loginButton Click event
            loginButton.Click += LoginButton_Click;
        }
        private void InitUI()
        {
            leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = ConfigurationConstants.ScreenWidth / 2,
                BackColor = ConfigurationConstants.SSWhiteColor,
                
            };
            rightPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = ConfigurationConstants.ScreenWidth / 2,
                BackColor = ConfigurationConstants.SSDarkBlueColor,

            };

            loginPanel = new Panel
            {
                Size = new Size(400, 400),
                BackColor = ConfigurationConstants.SSWhiteColor,
                Width= ConfigurationConstants.ScreenWidth / 2,
                Height= ConfigurationConstants.ScreenHeight,
                Left = 1,
            };

            this.Controls.Add(leftPanel);
            this.Controls.Add(rightPanel);
            rightPanel.Controls.Add(loginPanel);


            //Left Panel Content
            RenderLeftPanel();
            RenderRightPanel();

            // Initialize loading indicator after all UI is created
            loadingIndicator = new LoadingIndicator();
            this.Controls.Add(loadingIndicator);
            loadingIndicator.BringToFront();
        }


    }
    
}
