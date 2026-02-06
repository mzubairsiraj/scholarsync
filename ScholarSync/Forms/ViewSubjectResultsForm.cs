using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ScholarSync.Commons;

namespace ScholarSync.Forms
{
    public partial class ViewSubjectResultsForm : Form
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Panel filterPanel;
        private Panel contentPanel;
        private Panel statsPanel;
        private DataGridView dgvSubjectResults;
        private Button btnClose;
        private Button btnExport;
        private Button btnRefresh;
        
        // Filter controls
        private ComboBox cmbSubjectFilter;
        private ComboBox cmbExamTypeFilter;
        private Label lblSubjectFilter;
        private Label lblExamTypeFilter;
        
        // Stats labels
        private Label lblTotalSubjects;
        private Label lblTotalStudents;
        private Label lblOverallAverage;
        private Label lblPassPercentage;
        
        private DataTable subjectResultsData;

        public ViewSubjectResultsForm(DataTable data)
        {
            subjectResultsData = data;
            InitializeComponent();
            this.Text = "Subject-wise Results Analysis | ScholarSync";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(1000, 600);
            this.Icon = ConfigurationConstants.LogoIcon;
            InitializeUI();
            LoadData();
        }

        private void InitializeUI()
        {
            // Header Panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = ConfigurationConstants.SSDarkNavyColor
            };

            titleLabel = new Label
            {
                Text = "Subject-wise Results Analysis",
                Font = new Font("Arial", 22, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            headerPanel.Controls.Add(titleLabel);
            this.Controls.Add(headerPanel);

            // Filter Panel
            filterPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = ConfigurationConstants.SSLightGrayBackground,
                Padding = new Padding(20, 15, 20, 15)
            };

            InitializeFilterControls();
            this.Controls.Add(filterPanel);

            // Stats Panel
            statsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = ConfigurationConstants.SSWhiteColor,
                Padding = new Padding(20)
            };

            InitializeStatsPanel();
            this.Controls.Add(statsPanel);

            // Content Panel
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ConfigurationConstants.SSWhiteColor,
                Padding = new Padding(20)
            };

            InitializeDataGridView();
            this.Controls.Add(contentPanel);

            // Bottom Button Panel
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = ConfigurationConstants.SSLightGrayBackground,
                Padding = new Padding(20, 15, 20, 15)
            };

            btnExport = UIHelper.CreateButton("Export to Excel", ConfigurationConstants.SSSuccessGreen, new Point(20, 15));
            btnExport.Click += BtnExport_Click;
            btnExport.ForeColor = ConfigurationConstants.SSWhiteColor;

            btnRefresh = UIHelper.CreateButton("Refresh Data", ConfigurationConstants.SSDarkBlueColor, new Point(240, 15));
            btnRefresh.Click += BtnRefresh_Click;
            btnRefresh.ForeColor = ConfigurationConstants.SSWhiteColor;

            btnClose = UIHelper.CreateButton("Close", ConfigurationConstants.SSErrorRed, new Point(460, 15));
            btnClose.Click += (s, e) => this.Close();
            btnClose.ForeColor = ConfigurationConstants.SSWhiteColor;

            buttonPanel.Controls.Add(btnExport);
            buttonPanel.Controls.Add(btnRefresh);
            buttonPanel.Controls.Add(btnClose);
            this.Controls.Add(buttonPanel);
        }

        private void InitializeFilterControls()
        {
            lblSubjectFilter = new Label
            {
                Text = "Filter by Subject:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, 20),
                Size = new Size(130, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbSubjectFilter = new ComboBox
            {
                Font = new Font("Arial", 11, FontStyle.Regular),
                Location = new Point(155, 20),
                Size = new Size(250, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbSubjectFilter.Items.Add("All Subjects");
            cmbSubjectFilter.SelectedIndex = 0;
            cmbSubjectFilter.SelectedIndexChanged += FilterChanged;

            lblExamTypeFilter = new Label
            {
                Text = "Exam Type:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(425, 20),
                Size = new Size(100, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbExamTypeFilter = new ComboBox
            {
                Font = new Font("Arial", 11, FontStyle.Regular),
                Location = new Point(530, 20),
                Size = new Size(180, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbExamTypeFilter.Items.AddRange(new string[] { "All Types", "Mid Term", "Final Term", "Sessional" });
            cmbExamTypeFilter.SelectedIndex = 0;
            cmbExamTypeFilter.SelectedIndexChanged += FilterChanged;

            filterPanel.Controls.Add(lblSubjectFilter);
            filterPanel.Controls.Add(cmbSubjectFilter);
            filterPanel.Controls.Add(lblExamTypeFilter);
            filterPanel.Controls.Add(cmbExamTypeFilter);
        }

        private void InitializeStatsPanel()
        {
            lblTotalSubjects = CreateStatsLabel("Total Subjects: 0", 20, 20);
            lblTotalStudents = CreateStatsLabel("Total Students: 0", 20, 50);
            lblOverallAverage = CreateStatsLabel("Overall Average: 0.00%", 350, 20);
            lblPassPercentage = CreateStatsLabel("Pass Rate: 0.00%", 350, 50);

            statsPanel.Controls.Add(lblTotalSubjects);
            statsPanel.Controls.Add(lblTotalStudents);
            statsPanel.Controls.Add(lblOverallAverage);
            statsPanel.Controls.Add(lblPassPercentage);
        }

        private Label CreateStatsLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(x, y),
                Size = new Size(300, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private void InitializeDataGridView()
        {
            dgvSubjectResults = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = ConfigurationConstants.SSWhiteColor,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                AllowUserToResizeRows = false,
                RowTemplate = { Height = 40 }
            };

            dgvSubjectResults.ColumnHeadersDefaultCellStyle.BackColor = ConfigurationConstants.SSDarkNavyColor;
            dgvSubjectResults.ColumnHeadersDefaultCellStyle.ForeColor = ConfigurationConstants.SSWhiteColor;
            dgvSubjectResults.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 11, FontStyle.Bold);
            dgvSubjectResults.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvSubjectResults.ColumnHeadersHeight = 45;

            dgvSubjectResults.DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Regular);
            dgvSubjectResults.DefaultCellStyle.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            dgvSubjectResults.DefaultCellStyle.SelectionBackColor = ConfigurationConstants.SSLightNavyColor;
            dgvSubjectResults.DefaultCellStyle.SelectionForeColor = ConfigurationConstants.SSWhiteColor;
            dgvSubjectResults.DefaultCellStyle.Padding = new Padding(5);

            dgvSubjectResults.AlternatingRowsDefaultCellStyle.BackColor = ConfigurationConstants.SSLightGrayBackground;
            dgvSubjectResults.GridColor = ConfigurationConstants.SSBorderGray;

            dgvSubjectResults.CellFormatting += DgvSubjectResults_CellFormatting;

            contentPanel.Controls.Add(dgvSubjectResults);
        }

        private void DgvSubjectResults_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvSubjectResults.Columns[e.ColumnIndex].Name == "Average Marks" && e.Value != null)
            {
                double avgMarks = Convert.ToDouble(e.Value);
                e.Value = avgMarks.ToString("F2");
                
                if (avgMarks >= 75)
                    e.CellStyle.ForeColor = ConfigurationConstants.SSSuccessGreen;
                else if (avgMarks >= 60)
                    e.CellStyle.ForeColor = ConfigurationConstants.SSDarkBlueColor;
                else
                    e.CellStyle.ForeColor = ConfigurationConstants.SSErrorRed;
                
                e.CellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            }
        }

        private void LoadData()
        {
            if (subjectResultsData == null || subjectResultsData.Rows.Count == 0)
            {
                MessageBox.Show("No subject results data available.", "No Data", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Populate subject filter dropdown
            DataView dv = new DataView(subjectResultsData);
            DataTable distinctSubjects = dv.ToTable(true, "Subject Name");
            foreach (DataRow row in distinctSubjects.Rows)
            {
                cmbSubjectFilter.Items.Add(row["Subject Name"].ToString());
            }

            // Set data source
            dgvSubjectResults.DataSource = subjectResultsData;

            // Configure column widths
            if (dgvSubjectResults.Columns["Subject Code"] != null)
                dgvSubjectResults.Columns["Subject Code"].FillWeight = 70;
            if (dgvSubjectResults.Columns["Subject Name"] != null)
                dgvSubjectResults.Columns["Subject Name"].FillWeight = 120;
            if (dgvSubjectResults.Columns["Exam Type"] != null)
                dgvSubjectResults.Columns["Exam Type"].FillWeight = 80;
            if (dgvSubjectResults.Columns["Total Students"] != null)
                dgvSubjectResults.Columns["Total Students"].FillWeight = 80;
            if (dgvSubjectResults.Columns["Passed"] != null)
                dgvSubjectResults.Columns["Passed"].FillWeight = 60;
            if (dgvSubjectResults.Columns["Failed"] != null)
                dgvSubjectResults.Columns["Failed"].FillWeight = 60;
            if (dgvSubjectResults.Columns["Average Marks"] != null)
                dgvSubjectResults.Columns["Average Marks"].FillWeight = 90;
            if (dgvSubjectResults.Columns["Highest Marks"] != null)
                dgvSubjectResults.Columns["Highest Marks"].FillWeight = 80;
            if (dgvSubjectResults.Columns["Lowest Marks"] != null)
                dgvSubjectResults.Columns["Lowest Marks"].FillWeight = 80;

            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            if (subjectResultsData == null || subjectResultsData.Rows.Count == 0)
                return;

            DataView dv = ((DataTable)dgvSubjectResults.DataSource).DefaultView;
            
            // Count unique subjects
            DataTable distinctSubjects = dv.ToTable(true, "Subject Code");
            int totalSubjects = distinctSubjects.Rows.Count;

            int totalStudents = 0;
            int totalPassed = 0;
            double totalAverage = 0;

            foreach (DataRowView row in dv)
            {
                totalStudents += Convert.ToInt32(row["Total Students"]);
                totalPassed += Convert.ToInt32(row["Passed"]);
                totalAverage += Convert.ToDouble(row["Average Marks"]);
            }

            double overallAverage = dv.Count > 0 ? totalAverage / dv.Count : 0;
            double passRate = totalStudents > 0 ? (double)totalPassed / totalStudents * 100 : 0;

            lblTotalSubjects.Text = $"Total Subjects: {totalSubjects}";
            lblTotalStudents.Text = $"Total Students: {totalStudents}";
            lblOverallAverage.Text = $"Overall Average: {overallAverage:F2}%";
            lblPassPercentage.Text = $"Pass Rate: {passRate:F2}%";

            // Color code pass rate
            if (passRate >= 75)
                lblPassPercentage.ForeColor = ConfigurationConstants.SSSuccessGreen;
            else if (passRate >= 50)
                lblPassPercentage.ForeColor = Color.Orange;
            else
                lblPassPercentage.ForeColor = ConfigurationConstants.SSErrorRed;
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            DataTable dt = subjectResultsData;
            if (dt == null) return;

            DataView dv = new DataView(dt);
            string filter = "";

            if (cmbSubjectFilter.SelectedIndex > 0)
            {
                string selectedSubject = cmbSubjectFilter.SelectedItem.ToString();
                filter = $"[Subject Name] = '{selectedSubject}'";
            }

            if (cmbExamTypeFilter.SelectedIndex > 0)
            {
                if (filter.Length > 0) filter += " AND ";
                filter += $"[Exam Type] = '{cmbExamTypeFilter.SelectedItem}'";
            }

            dv.RowFilter = filter;
            dgvSubjectResults.DataSource = dv;

            UpdateStatistics();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Export to Excel functionality will be implemented with database integration.", 
                "Export Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            cmbSubjectFilter.SelectedIndex = 0;
            cmbExamTypeFilter.SelectedIndex = 0;
            ApplyFilters();
            MessageBox.Show("Data refreshed successfully!", "Refresh", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
