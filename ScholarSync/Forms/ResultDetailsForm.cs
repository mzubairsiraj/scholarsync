using System;
using System.Drawing;
using System.Windows.Forms;
using ScholarSync.Commons;

namespace ScholarSync.Forms
{
    public partial class ResultDetailsForm : Form
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Button btnClose;
        
        private Panel contentPanel;
        private Panel studentPanel;
        private Panel subjectPanel;
        private Panel performancePanel;
        
        public ResultDetailsForm(
            string studentName,
            string rollNumber,
            string subjectCode,
            string subjectName,
            string semester,
            string totalMarks,
            string obtainedMarks,
            string percentage,
            string gpa,
            string grade,
            string status,
            string remarks)
        {
            InitializeComponent();
            InitializeUI(studentName, rollNumber, subjectCode, subjectName, semester, 
                         totalMarks, obtainedMarks, percentage, gpa, grade, status, remarks);
        }

        private void InitializeComponent()
        {
            this.Size = new Size(600, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = ConfigurationConstants.SSWhiteColor;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
        }

        private void InitializeUI(
            string studentName,
            string rollNumber,
            string subjectCode,
            string subjectName,
            string semester,
            string totalMarks,
            string obtainedMarks,
            string percentage,
            string gpa,
            string grade,
            string status,
            string remarks)
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = ConfigurationConstants.SSDarkNavyColor
            };

            titleLabel = new Label
            {
                Text = "Result Details",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(20, 0),
                Size = new Size(450, 70),
                TextAlign = ContentAlignment.MiddleLeft
            };

            btnClose = new Button
            {
                Text = "?",
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                BackColor = ConfigurationConstants.SSDarkNavyColor,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(540, 15),
                Size = new Size(40, 40),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();
            btnClose.MouseEnter += (s, e) => btnClose.BackColor = ConfigurationConstants.SSErrorRed;
            btnClose.MouseLeave += (s, e) => btnClose.BackColor = ConfigurationConstants.SSDarkNavyColor;

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(btnClose);
            this.Controls.Add(headerPanel);

            contentPanel = new Panel
            {
                Location = new Point(0, 70),
                Size = new Size(600, 630),
                BackColor = ConfigurationConstants.SSWhiteColor,
                AutoScroll = true,
                Padding = new Padding(30)
            };

            int yPos = 20;

            studentPanel = CreateSectionPanel("Student Information", yPos);
            yPos = AddInfoRow(studentPanel, "Student Name:", studentName, 60);
            yPos = AddInfoRow(studentPanel, "Roll Number:", rollNumber, yPos);
            studentPanel.Height = yPos + 20;
            contentPanel.Controls.Add(studentPanel);
            yPos = studentPanel.Bottom + 20;

            subjectPanel = CreateSectionPanel("Subject Information", yPos);
            int subYPos = 60;
            subYPos = AddInfoRow(subjectPanel, "Subject Name:", subjectName, subYPos);
            subYPos = AddInfoRow(subjectPanel, "Subject Code:", subjectCode, subYPos);
            subYPos = AddInfoRow(subjectPanel, "Semester:", semester, subYPos);
            subjectPanel.Height = subYPos + 20;
            contentPanel.Controls.Add(subjectPanel);
            yPos = subjectPanel.Bottom + 20;

            performancePanel = CreateSectionPanel("Performance", yPos);
            int perfYPos = 60;
            perfYPos = AddInfoRow(performancePanel, "Total Marks:", totalMarks, perfYPos);
            perfYPos = AddInfoRow(performancePanel, "Obtained Marks:", obtainedMarks, perfYPos);
            perfYPos = AddInfoRow(performancePanel, "Percentage:", percentage, perfYPos);
            perfYPos = AddInfoRow(performancePanel, "GPA:", gpa + " / 4.0", perfYPos);
            perfYPos = AddInfoRow(performancePanel, "Grade:", grade, perfYPos);
            
            Label lblStatus = new Label
            {
                Text = "Status:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(30, perfYPos),
                Size = new Size(150, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblStatusValue = new Label
            {
                Text = status,
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = status == "Pass" ? ConfigurationConstants.SSSuccessGreen : 
                           status == "Fail" ? ConfigurationConstants.SSErrorRed : 
                           ConfigurationConstants.SSWarningOrange,
                Location = new Point(200, perfYPos),
                Size = new Size(300, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            performancePanel.Controls.Add(lblStatus);
            performancePanel.Controls.Add(lblStatusValue);
            perfYPos += 40;

            if (!string.IsNullOrEmpty(remarks) && remarks != "N/A")
            {
                Label lblRemarks = new Label
                {
                    Text = "Remarks:",
                    Font = new Font("Arial", 11, FontStyle.Bold),
                    ForeColor = ConfigurationConstants.SSDarkBlueColor,
                    Location = new Point(30, perfYPos),
                    Size = new Size(150, 30),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                Label lblRemarksValue = new Label
                {
                    Text = remarks,
                    Font = new Font("Arial", 11, FontStyle.Italic),
                    ForeColor = ConfigurationConstants.SSDarkBlueColor,
                    Location = new Point(200, perfYPos),
                    Size = new Size(300, 60),
                    TextAlign = ContentAlignment.TopLeft,
                    AutoSize = false
                };

                performancePanel.Controls.Add(lblRemarks);
                performancePanel.Controls.Add(lblRemarksValue);
                perfYPos += 70;
            }

            performancePanel.Height = perfYPos + 20;
            contentPanel.Controls.Add(performancePanel);

            Panel buttonPanel = new Panel
            {
                Location = new Point(0, performancePanel.Bottom + 30),
                Size = new Size(540, 60),
                BackColor = ConfigurationConstants.SSWhiteColor
            };

            Button btnOk = UIHelper.CreateButton("OK", ConfigurationConstants.SSDarkNavyColor, new Point(200, 10), new Size(140, 45));
            btnOk.ForeColor = ConfigurationConstants.SSWhiteColor;
            btnOk.Font = new Font("Arial", 12, FontStyle.Bold);
            btnOk.Click += (s, e) => this.Close();

            buttonPanel.Controls.Add(btnOk);
            contentPanel.Controls.Add(buttonPanel);

            this.Controls.Add(contentPanel);

            this.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
                    ConfigurationConstants.SSDarkNavyColor, 2, ButtonBorderStyle.Solid,
                    ConfigurationConstants.SSDarkNavyColor, 2, ButtonBorderStyle.Solid,
                    ConfigurationConstants.SSDarkNavyColor, 2, ButtonBorderStyle.Solid,
                    ConfigurationConstants.SSDarkNavyColor, 2, ButtonBorderStyle.Solid);
            };
        }

        private Panel CreateSectionPanel(string title, int yPosition)
        {
            Panel panel = new Panel
            {
                Location = new Point(0, yPosition),
                Size = new Size(540, 200),
                BackColor = ConfigurationConstants.SSLightGrayBackground,
                BorderStyle = BorderStyle.None
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(20, 15),
                Size = new Size(500, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Panel separatorLine = new Panel
            {
                Location = new Point(20, 45),
                Size = new Size(500, 2),
                BackColor = ConfigurationConstants.SSDarkNavyColor
            };

            panel.Controls.Add(lblTitle);
            panel.Controls.Add(separatorLine);

            return panel;
        }

        private int AddInfoRow(Panel parentPanel, string label, string value, int yPosition)
        {
            Label lblLabel = new Label
            {
                Text = label,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(30, yPosition),
                Size = new Size(150, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblValue = new Label
            {
                Text = value,
                Font = new Font("Arial", 11, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(200, yPosition),
                Size = new Size(300, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            parentPanel.Controls.Add(lblLabel);
            parentPanel.Controls.Add(lblValue);

            return yPosition + 40;
        }
    }
}
