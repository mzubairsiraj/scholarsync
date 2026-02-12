using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScholarSync.Commons;
using ScholarSync.Controls.Student;
using ScholarSync.Controls.Teacher;
using ScholarSync.Controls.Result;
using ScholarSync.Controls.Home;
using ScholarSync.Controls.Academic;

namespace ScholarSync.Forms
{
        public partial class DashBoardForm : Form
    {
        WelcomeForm welcomeForm;
        
        // Sidebar components
        Panel sideBarPanel;
        Panel sideBarLogoContainer;
        PictureBox sideBarLogoPictureBox;
        Label sideBarLogoLabel;
        
        // Content area
        Panel contentPanel;
        
        // Menu buttons and dropdowns
        Dictionary<string, Button> menuButtons;
        Dictionary<string, Panel> dropdownPanels;
        
        // Constants
        private const int SIDEBAR_WIDTH = 250;
        private const int MARGIN = 10;
        private const int PADDING = 15;
        private const int BUTTON_HEIGHT = 50;
        private const int LOGO_CONTAINER_HEIGHT = 70;
        private const int DROPDOWN_ITEM_HEIGHT = 40;
        
        public DashBoardForm(object senderObj)
        {
            welcomeForm = senderObj as WelcomeForm;
            InitializeComponent();
            this.Icon = (senderObj as WelcomeForm).Icon;
            this.Text = "Dashboard | ScholarSync";
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(ConfigurationConstants.ScreenWidth, ConfigurationConstants.ScreenHeight);
            this.BackColor = ConfigurationConstants.SSWhiteColor;
            
            menuButtons = new Dictionary<string, Button>();
            dropdownPanels = new Dictionary<string, Panel>();
            
            InitUI();
            this.FormClosing += new FormClosingEventHandler(DashBoardForm_FormClosing);
        }
        
        private void DashBoardForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to exit ScholarSync?", 
                "Exit ScholarSync", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }
            welcomeForm.Close();
        }
        
        private void InitUI()
        {
            // Create main content area (right side)
            contentPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = this.ClientSize.Width - SIDEBAR_WIDTH,
                BackColor = ConfigurationConstants.SSWhiteColor,
                
            };
            this.Controls.Add(contentPanel);
            
            // Create and render sidebar
            RenderSideBar();
            
            // Load Home Dashboard by default
            LoadHomeDashboard();
        }
        
        private void RenderSideBar()
        {
            // Main sidebar panel
            sideBarPanel = new Panel
            {
                Width = SIDEBAR_WIDTH,
                BackColor = ConfigurationConstants.SSLightNavyColor,
                Dock = DockStyle.Left,
                Padding = new Padding(0)
            };
            this.Controls.Add(sideBarPanel);
            sideBarPanel.BringToFront();
            
            // Logo container
            RenderLogoContainer();
            
            // Menu buttons
            int currentY = LOGO_CONTAINER_HEIGHT + MARGIN;
            
            // Home menu button (no dropdown)
            currentY = CreateMenuButton("Home", currentY, false, null);
            
            // Student menu with dropdown
            currentY = CreateMenuButton("Student", currentY, true, new string[] 
            { 
                "Register Student", 
                "List Students", 
                "Update Student",
                "Delete Student"
            });
            
            // Teacher menu with dropdown
            currentY = CreateMenuButton("Teacher", currentY, true, new string[] 
            { 
                "Create Teacher", 
                "List Teachers", 
                "Update Teacher",
                "Delete Teacher"
            });
            
            // Academic menu with dropdown
            currentY = CreateMenuButton("Academic", currentY, true, new string[] 
            { 
                "Manage Departments", 
                "Manage Programs",
                "Manage Subjects",
                "Manage Semesters"
            });
            
            // Result menu with dropdown
            currentY = CreateMenuButton("Result", currentY, true, new string[] 
            { 
                "Add Result", 
                "View Results", 
                "Update Result",
                "Generate Transcript"
            });
            
            // Logout button (no dropdown)
            CreateMenuButton("Logout", currentY, false, null);
        }
        
        private void RenderLogoContainer()
        {
            sideBarLogoContainer = new Panel
            {
                Width = SIDEBAR_WIDTH,
                Height = LOGO_CONTAINER_HEIGHT,
                BackColor = ConfigurationConstants.SSDarkNavyColor,
                Dock = DockStyle.Top,
                Padding = new Padding(PADDING)
            };
            sideBarPanel.Controls.Add(sideBarLogoContainer);
            
            // Logo image
            sideBarLogoPictureBox = new PictureBox
            {
                Image = ConfigurationConstants.LogoImage,
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(PADDING, (LOGO_CONTAINER_HEIGHT - 40) / 2),
                Size = new Size(40, 40)
            };
            sideBarLogoContainer.Controls.Add(sideBarLogoPictureBox);
            
            // Logo label
            sideBarLogoLabel = new Label
            {
                Text = "GCMS DIK",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(PADDING + 50, 0),
                Size = new Size(SIDEBAR_WIDTH - (PADDING + 50), LOGO_CONTAINER_HEIGHT),
                TextAlign = ContentAlignment.MiddleLeft
            };
            sideBarLogoContainer.Controls.Add(sideBarLogoLabel);
        }
        
        private int CreateMenuButton(string buttonName, int yPosition, bool hasDropdown, string[] dropdownItems)
        {
            // Main menu button
            Button menuButton = new Button
            {
                Text = hasDropdown ? buttonName + "  ▼" : buttonName,
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                BackColor = ConfigurationConstants.SSWhiteColor,
                Width = SIDEBAR_WIDTH,
                Height = BUTTON_HEIGHT,
                Location = new Point(0, yPosition),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(PADDING, 0, 0, 0),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Tag = buttonName
            };
            
            menuButton.FlatAppearance.BorderSize = 0;
            menuButton.FlatAppearance.MouseOverBackColor = ConfigurationConstants.SSDarkBlueColor;
            menuButton.FlatAppearance.MouseDownBackColor = ConfigurationConstants.SSDarkBlueColor;
            
            menuButton.MouseEnter += (s, e) => 
            {
                menuButton.ForeColor = ConfigurationConstants.SSWhiteColor;
            };
            
            menuButton.MouseLeave += (s, e) => 
            {
                if (!IsDropdownOpen(buttonName))
                {
                    menuButton.ForeColor = ConfigurationConstants.SSDarkBlueColor;
                }
            };
            
            sideBarPanel.Controls.Add(menuButton);
            menuButtons[buttonName] = menuButton;
            
            int nextY = yPosition + BUTTON_HEIGHT;
            
            // Create dropdown panel if needed
            if (hasDropdown && dropdownItems != null && dropdownItems.Length > 0)
            {
                Panel dropdownPanel = new Panel
                {
                    Width = SIDEBAR_WIDTH,
                    Height = 0, // Initially collapsed
                    Location = new Point(0, nextY),
                    BackColor = ConfigurationConstants.SSDarkBlueColor,
                    Visible = false,
                    Tag = buttonName
                };
                
                // Add dropdown items
                for (int i = 0; i < dropdownItems.Length; i++)
                {
                    Label dropdownItem = new Label
                    {
                        Text = "    " + dropdownItems[i],
                        Font = new Font("Arial", 10, FontStyle.Regular),
                        ForeColor = ConfigurationConstants.SSWhiteColor,
                        BackColor = ConfigurationConstants.SSDarkBlueColor,
                        Width = SIDEBAR_WIDTH,
                        Height = DROPDOWN_ITEM_HEIGHT,
                        Location = new Point(0, i * DROPDOWN_ITEM_HEIGHT),
                        TextAlign = ContentAlignment.MiddleLeft,
                        Cursor = Cursors.Hand,
                        Padding = new Padding(PADDING * 2, 0, 0, 0),
                        Tag = dropdownItems[i]
                    };
                    
                    dropdownItem.MouseEnter += (s, e) => 
                    {
                        dropdownItem.BackColor = ConfigurationConstants.SSLightNavyColor;
                    };
                    
                    dropdownItem.MouseLeave += (s, e) => 
                    {
                        dropdownItem.BackColor = ConfigurationConstants.SSDarkBlueColor;
                    };
                    
                    dropdownItem.Click += DropdownItem_Click;
                    
                    dropdownPanel.Controls.Add(dropdownItem);
                }
                
                sideBarPanel.Controls.Add(dropdownPanel);
                dropdownPanels[buttonName] = dropdownPanel;
                
                // Add click event to toggle dropdown
                menuButton.Click += (s, e) => ToggleDropdown(buttonName);
            }
            else if (buttonName == "Home")
            {
                menuButton.Click += (s, e) => LoadHomeDashboard();
            }
            else if (buttonName == "Logout")
            {
                menuButton.Click += (s, e) => 
                {
                    DialogResult result = MessageBox.Show(
                        "Are you sure you want to logout?", 
                        "Logout", 
                        MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        // Logout from session
                        SessionManager.Logout();
                        
                        this.Hide();
                        welcomeForm.Show();
                    }
                };
            }
            
            return nextY + MARGIN;
        }
        
        private void ToggleDropdown(string buttonName)
        {
            if (!dropdownPanels.ContainsKey(buttonName))
                return;
            
            Panel dropdown = dropdownPanels[buttonName];
            Button button = menuButtons[buttonName];
            
            // Close all other dropdowns
            foreach (var kvp in dropdownPanels)
            {
                if (kvp.Key != buttonName && kvp.Value.Visible)
                {
                    CollapseDropdown(kvp.Key);
                }
            }
            
            // Toggle current dropdown
            if (dropdown.Visible)
            {
                CollapseDropdown(buttonName);
            }
            else
            {
                ExpandDropdown(buttonName);
            }
        }
        
        private void ExpandDropdown(string buttonName)
        {
            Panel dropdown = dropdownPanels[buttonName];
            Button button = menuButtons[buttonName];
            
            int targetHeight = dropdown.Controls.Count * DROPDOWN_ITEM_HEIGHT;
            
            dropdown.Visible = true;
            dropdown.Height = targetHeight;
            button.Text = buttonName + "  ▲";
            button.ForeColor = ConfigurationConstants.SSWhiteColor;
            button.BackColor = ConfigurationConstants.SSDarkBlueColor;
            
            // Shift buttons below
            AdjustButtonPositions(buttonName, targetHeight, true);
        }
        
        private void CollapseDropdown(string buttonName)
        {
            Panel dropdown = dropdownPanels[buttonName];
            Button button = menuButtons[buttonName];
            
            int currentHeight = dropdown.Height;
            
            dropdown.Height = 0;
            dropdown.Visible = false;
            button.Text = buttonName + "  ▼";
            button.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            button.BackColor = ConfigurationConstants.SSWhiteColor;
            
            // Shift buttons below
            AdjustButtonPositions(buttonName, currentHeight, false);
        }
        
        private void AdjustButtonPositions(string changedButton, int heightChange, bool expanding)
        {
            int changedButtonIndex = GetButtonIndex(changedButton);
            int adjustment = expanding ? heightChange : -heightChange;
            
            foreach (var kvp in menuButtons)
            {
                if (GetButtonIndex(kvp.Key) > changedButtonIndex)
                {
                    kvp.Value.Top += adjustment;
                    
                    if (dropdownPanels.ContainsKey(kvp.Key))
                    {
                        dropdownPanels[kvp.Key].Top += adjustment;
                    }
                }
            }
        }
        
        private int GetButtonIndex(string buttonName)
        {
            string[] order = { "Home", "Student", "Teacher", "Academic", "Result", "Logout" };
            return Array.IndexOf(order, buttonName);
        }
        
        private bool IsDropdownOpen(string buttonName)
        {
            return dropdownPanels.ContainsKey(buttonName) && dropdownPanels[buttonName].Visible;
        }
        
        private void DropdownItem_Click(object sender, EventArgs e)
        {
            Label clickedItem = sender as Label;
            if (clickedItem != null)
            {
                string itemText = clickedItem.Tag as string;
                LoadContentControl(itemText);
            }
        }

        private void LoadContentControl(string controlName)
        {
            // Clear existing content
            contentPanel.Controls.Clear();

            UserControl contentControl = null;

            // Create appropriate control based on name
            switch (controlName)
            {
                case "Register Student":
                    contentControl = new RegisterStudentControl();
                    break;
                case "List Students":
                    contentControl = new ListStudentsControl();
                    break;
                case "Update Student":
                    contentControl = new UpdateStudentControl();
                    break;
                case "Delete Student":
                    contentControl = new DeleteStudentControl();
                    break;
                case "Create Teacher":
                    contentControl = new CreateTeacherControl();
                    break;
                case "List Teachers":
                    contentControl = new ListTeachersControl();
                    break;
                case "Update Teacher":
                    contentControl = new UpdateTeacherControl();
                    break;
                case "Delete Teacher":
                    contentControl = new DeleteTeacherControl();
                    break;
                case "Manage Departments":
                    contentControl = new ManageDepartmentsControl();
                    break;
                case "Manage Programs":
                    contentControl = new ManageProgramsControl();
                    break;
                case "Manage Subjects":
                    contentControl = new ManageSubjectsControl();
                    break;
                case "Manage Semesters":
                    contentControl = new ManageSemestersControl();
                    break;
                case "Add Result":
                    contentControl = new AddResultControl();
                    break;
                case "View Results":
                    contentControl = new ListResultsControl();
                    break;
                case "Update Result":
                    contentControl = new UpdateResultControl();
                    break;
                case "Generate Transcript":
                    contentControl = new GenerateTranscriptControl();
                    break;
                default:
                    ShowPlaceholder(controlName);
                    return;
            }

            if (contentControl != null)
            {
                contentControl.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(contentControl);
            }
        }

        private void LoadHomeDashboard()
        {
            // Clear existing content
            contentPanel.Controls.Clear();

            // Create and add Home Dashboard
            HomeDashboardControl homeControl = new HomeDashboardControl();
            homeControl.Dock = DockStyle.Fill;
            
            // Subscribe to quick action events
            homeControl.OnQuickActionClicked += HomeControl_OnQuickActionClicked;
            
            contentPanel.Controls.Add(homeControl);
        }

        private void HomeControl_OnQuickActionClicked(object sender, string actionName)
        {
            // Navigate to the appropriate control based on action name
            LoadContentControl(actionName);
        }

        private void ShowPlaceholder(string featureName)
        {
            Panel placeholder = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ConfigurationConstants.SSWhiteColor
            };

            Label titleLabel = new Label
            {
                Text = featureName,
                Font = new Font("Arial", 28, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                AutoSize = true
            };

            Label messageLabel = new Label
            {
                Text = "This feature will be implemented soon.",
                Font = new Font("Arial", 14, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                AutoSize = true
            };

            placeholder.Controls.Add(titleLabel);
            placeholder.Controls.Add(messageLabel);

            placeholder.Resize += (s, e) =>
            {
                titleLabel.Location = new Point(
                    (placeholder.Width - titleLabel.Width) / 2,
                    (placeholder.Height - titleLabel.Height - 50) / 2
                );
                messageLabel.Location = new Point(
                    (placeholder.Width - messageLabel.Width) / 2,
                    titleLabel.Bottom + 20
                );
            };

            contentPanel.Controls.Add(placeholder);
        }
    }
}