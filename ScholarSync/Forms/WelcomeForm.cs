using ScholarSync.Commons;
using ScholarSync.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


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
        Label userNameLabel;
        Label passwordLabel;
        Label userTypeLabel;
        Label forgotPasswordLabel;

        ComboBox userTypeComboBox;

        TextBox userNameTextBox;
        TextBox passwordTextBox;

        Button loginButton;




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

        private void RenderLeftPanel()
        {
            //Left Panel Content -> Logo, Title, Subtitle, View Manual Button, CTA Button
            int upperMargin = (leftPanel.Height / 10) + 80;


            int leftPanelCenterX = leftPanel.Width / 6;
            int leftPanelCenterY = (leftPanel.Height / 6) + upperMargin;


            PictureBox logoPictureBox = new PictureBox
            {
                Image = new Bitmap(@"D:\BS CS 5th\C Sharp Console Application\ScholarSync\ScholarSync\Resourses\Logo.png"),
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
                ForeColor = Color.White,
                Size = new Size(180, 40),
                Location = new Point(leftPanelCenterX + 50, leftPanelCenterY + 250),
                FlatStyle = FlatStyle.Flat
            };

            CtaBtn = new Button
            {
                Text = "Get Started",
                Font = new Font("Arial", 12, FontStyle.Regular),
                BackColor = ConfigurationConstants.SSDarkNavyColor,
                ForeColor = Color.White,
                Size = new Size(180, 40),
                Location = new Point(leftPanelCenterX + 250, leftPanelCenterY + 250),
                FlatStyle = FlatStyle.Flat
            };


            leftPanel.Controls.Add(logoPictureBox);
            leftPanel.Controls.Add(titleBox);
            leftPanel.Controls.Add(subtitleBox);
            leftPanel.Controls.Add(viewManualBtn);
            leftPanel.Controls.Add(CtaBtn);
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
                "Administrator",
                "Teacher",
                "Student",

            });
            userTypeComboBox.SelectedIndex = 0;



            userNameLabel = new Label
            {
                Text = "Username:",
                Font = new Font("Arial", 12, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(labelLeftMargin, topPaddingLoginCard + 120),
                Size = new Size(labelWidth, labelHeight),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };

            userNameTextBox = new TextBox
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

            forgotPasswordLabel = new Label
            {
                Text = "Forgot Password ?",
                Font = new Font("Arial", 10, FontStyle.Underline),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point((loginPanel.Width / 2) - 80, topPaddingLoginCard + 200),
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = true,
                Cursor = Cursors.Hand,
            };

            loginButton = new Button
            {
                Text = "Login",
                Font = new Font("Arial", 12, FontStyle.Regular),
                BackColor = ConfigurationConstants.SSDarkNavyColor,
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Size = new Size(inputWidth, inputHeight),
                Location = new Point((loginCardPanel.Width - inputWidth) / 2, topPaddingLoginCard + 250),
                FlatStyle = FlatStyle.Flat
            };

            loginCardPanel.Controls.Add(loginTitleBox);
            loginCardPanel.Controls.Add(userTypeLabel);
            loginCardPanel.Controls.Add(userTypeComboBox);
            loginCardPanel.Controls.Add(userNameLabel);
            loginCardPanel.Controls.Add(userNameTextBox);
            loginCardPanel.Controls.Add(passwordLabel);
            loginCardPanel.Controls.Add(passwordTextBox);
            loginCardPanel.Controls.Add(forgotPasswordLabel);
            loginCardPanel.Controls.Add(loginButton);


            loginPanel.Controls.Add(loginCardContainer);
            loginCardContainer.Controls.Add(loginCardPanel);



            //UI Logic can be added here for loginButton Click event and forgotPasswordLabel Click event
            loginButton.Click += (sender, eventArgs) =>
            {
                DashBoardForm dashBoardForm = new DashBoardForm(this);
                dashBoardForm.Show();
                this.Hide();
            };

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



        }


    }
    
}
