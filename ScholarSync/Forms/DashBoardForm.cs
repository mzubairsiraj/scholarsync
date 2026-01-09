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

namespace ScholarSync.Forms
{
     public partial class DashBoardForm : Form
    {
        WelcomeForm welcomeForm;
        Panel sideBarPanel;

        Panel sideBarLogoContainer;

        PictureBox sideBarLogoPictureBox;
        Label sideBarLogoLabel;

        // SideBar Buttons -> 
        string [] sideBarButtonNames = { "Home", "Students" ,"Teachers", "Results", "Attendaance", "Logout" };


        public DashBoardForm(object senderObj)
        {
            welcomeForm = senderObj as WelcomeForm;
            InitializeComponent();
            this.Icon = (senderObj as WelcomeForm).Icon;
            this.Text = "Dashboard | ScholarSync";
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(ConfigurationConstants.ScreenWidth, ConfigurationConstants.ScreenHeight);
            this.BackColor = ConfigurationConstants.SSWhiteColor;
            InitUI();
            this.FormClosing += new FormClosingEventHandler(DashBoardForm_FormClosing);
           

        }
        private void DashBoardForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            DialogResult result = MessageBox.Show("Are you sure you want to exit ScholarSync?", "Exit ScholarSync", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }
           welcomeForm.Close();

        }
        private void RenderSideBar()
        {
            const int SIDE_BAR_WIDTH = 250;
            const int MARGIN_SIZE = 2;
            string isActive = "Home";

            sideBarPanel = new Panel{

                Size = new Size(SIDE_BAR_WIDTH, this.Height),
                BackColor = ConfigurationConstants.SSLightNavyColor,
                Dock = DockStyle.Left
            };


           sideBarLogoContainer = new Panel
            {
                Size = new Size(SIDE_BAR_WIDTH, 50),
                BackColor = ConfigurationConstants.SSDarkNavyColor,
                Dock = DockStyle.Top
            };


            this.Controls.Add(sideBarPanel);
            sideBarPanel.Controls.Add(sideBarLogoContainer);


            // Side Bar Logo 

            sideBarLogoPictureBox = new PictureBox
            {
                Image = new Bitmap(@"../../Resourses/Logo.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point((SIDE_BAR_WIDTH / 6) - 20 , (sideBarLogoContainer.Height - 40) / 2),
                Size = new Size(40, 40)
            };
            sideBarLogoLabel = new Label
            {
                Text = "GCMS DIK",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point((SIDE_BAR_WIDTH / 6) + 30, (sideBarLogoContainer.Height - 40) / 2),
                AutoSize = false,
                Size = new Size(SIDE_BAR_WIDTH / 2, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };

            sideBarLogoContainer.Controls.Add(sideBarLogoPictureBox);
            sideBarLogoContainer.Controls.Add(sideBarLogoLabel);

            // Side Bar Buttons

            for (int idx = 0; idx < sideBarButtonNames.Length;idx++)
            {
                Color BACKGROUND = isActive == sideBarButtonNames[idx] ? ConfigurationConstants.SSDarkBlueColor : ConfigurationConstants.SSWhiteColor;
                Color FORGROUND = isActive == sideBarButtonNames[idx] ? ConfigurationConstants.SSWhiteColor : ConfigurationConstants.SSDarkBlueColor;
                Label sideBarButton = new Label
                {
                    Text = sideBarButtonNames[idx],
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    ForeColor = FORGROUND,
                    BackColor = BACKGROUND,
                    Size = new Size(SIDE_BAR_WIDTH , 40),
                    Location = new Point(0,(51  + ((40 + MARGIN_SIZE) * idx))),
                    TextAlign = ContentAlignment.MiddleCenter,

                    Cursor = Cursors.Hand,
                    
                    
                };

                sideBarPanel.Controls.Add(sideBarButton);
            }
        }
        private void InitUI()
        {
            // Render Side Bar
            RenderSideBar();
        }
    }
}