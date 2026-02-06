using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ScholarSync.Commons;
using ScholarSync.Models;
using ScholarSync.Repositories;
using ScholarSync.Forms;

namespace ScholarSync.Controls.Result
{
    public partial class ListResultsControl : UserControl
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Panel searchPanel;
        private Panel gridPanel;
        private Panel paginationPanel;
        
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnClearSearch;
        private ComboBox cmbFilterSemester;
        private ComboBox cmbFilterStatus;
        private Label lblSearchLabel;
        private Label lblFilterSemester;
        private Label lblFilterStatus;
        private Label lblTotalRecords;
        
        private DataGridView dgvResults;
        
        private Button btnFirst;
        private Button btnPrevious;
        private Button btnNext;
        private Button btnLast;
        private Label lblPageInfo;
        private ComboBox cmbPageSize;
        private Label lblPageSize;
        
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalRecords = 0;
        private int totalPages = 0;

        // Loading Indicator
        private LoadingIndicator loadingIndicator;

        // Repository
        private EnrollmentRepository enrollmentRepository;

        public ListResultsControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.BackColor = ConfigurationConstants.SSWhiteColor;
            
            // Initialize repository
            enrollmentRepository = new EnrollmentRepository();
            
            InitializeUI();
            
            // Initialize loading indicator
            loadingIndicator = new LoadingIndicator();
            this.Controls.Add(loadingIndicator);
            
            LoadResults();
        }

        private void InitializeComponent()
        {
        }

        private void InitializeUI()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = ConfigurationConstants.SSDarkNavyColor
            };

            titleLabel = new Label
            {
                Text = "Student Results List",
                Font = new Font("Arial", 22, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(30, 0),
                Size = new Size(450, 80),
                TextAlign = ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.Add(titleLabel);
            this.Controls.Add(headerPanel);

            searchPanel = new Panel
            {
                Dock = DockStyle.None,
                BackColor = ConfigurationConstants.SSLightGrayBackground,
                Location = new Point(10, 100),
                Size = new Size(ConfigurationConstants.ScreenWidth - 300, 140),
                Padding = new Padding(20)
            };
            
            InitializeSearchControls();
            this.Controls.Add(searchPanel);

            gridPanel = new Panel
            {
                Dock = DockStyle.None,
                BackColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(10, 260),
                Size = new Size(ConfigurationConstants.ScreenWidth - 300, ConfigurationConstants.ScreenHeight - 440),
                Padding = new Padding(20)
            };
            
            InitializeDataGridView();
            this.Controls.Add(gridPanel);

            paginationPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = ConfigurationConstants.SSLightGrayBackground,
                Padding = new Padding(20, 15, 20, 15)
            };
            
            InitializePaginationControls();
            this.Controls.Add(paginationPanel);
        }

        private void InitializeSearchControls()
        {
            int yPos = 20;
            int labelWidth = 100;
            int controlWidth = 300;
            int controlHeight = 40;

            lblSearchLabel = new Label
            {
                Text = "Search:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(labelWidth, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtSearch = new TextBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(120, yPos),
                Size = new Size(controlWidth, controlHeight),
                ForeColor = Color.Gray,
                Text = "Search by Name, Roll Number, Subject...",
                Tag = "Search by Name, Roll Number, Subject...",
            };

            txtSearch.Enter += TxtSearch_Enter;
            txtSearch.Leave += TxtSearch_Leave;
            txtSearch.KeyPress += TxtSearch_KeyPress;

            btnSearch = UIHelper.CreateButton("Search", ConfigurationConstants.SSDarkNavyColor, new Point(440, yPos), new Size(120, controlHeight));
            btnSearch.Click += BtnSearch_Click;

            btnClearSearch = UIHelper.CreateButton("Clear", ConfigurationConstants.SSDarkBlueColor, new Point(580, yPos), new Size(120, controlHeight));
            btnClearSearch.Click += BtnClearSearch_Click;

            searchPanel.Controls.Add(lblSearchLabel);
            searchPanel.Controls.Add(txtSearch);
            searchPanel.Controls.Add(btnSearch);
            searchPanel.Controls.Add(btnClearSearch);

            yPos += 60;

            lblFilterSemester = new Label
            {
                Text = "Semester:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(labelWidth, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbFilterSemester = new ComboBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(120, yPos),
                Size = new Size(200, controlHeight),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilterSemester.Items.Add("All Semesters");
            cmbFilterSemester.SelectedIndex = 0;
            cmbFilterSemester.SelectedIndexChanged += FilterChanged;

            lblFilterStatus = new Label
            {
                Text = "Status:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(350, yPos),
                Size = new Size(labelWidth, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbFilterStatus = new ComboBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(450, yPos),
                Size = new Size(120, controlHeight),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilterStatus.Items.AddRange(new string[] { "All Status", "Pass", "Fail", "Pending" });
            cmbFilterStatus.SelectedIndex = 0;
            cmbFilterStatus.SelectedIndexChanged += FilterChanged;

            lblTotalRecords = new Label
            {
                Text = "Total Records: 0",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(600, yPos),
                Size = new Size(200, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            searchPanel.Controls.Add(lblFilterSemester);
            searchPanel.Controls.Add(cmbFilterSemester);
            searchPanel.Controls.Add(lblFilterStatus);
            searchPanel.Controls.Add(cmbFilterStatus);
            searchPanel.Controls.Add(lblTotalRecords);
        }

        private void TxtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == txtSearch.Tag.ToString())
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            }
        }

        private void TxtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = txtSearch.Tag.ToString();
                txtSearch.ForeColor = Color.Gray;
            }
        }

        private void TxtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSearch.PerformClick();
                e.Handled = true;
            }
        }

        private void InitializeDataGridView()
        {
            dgvResults = new DataGridView
            {
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
                RowTemplate = { Height = 40 },
                Size = new Size(gridPanel.Width, gridPanel.Height),
            };

            dgvResults.ColumnHeadersDefaultCellStyle.BackColor = ConfigurationConstants.SSDarkNavyColor;
            dgvResults.ColumnHeadersDefaultCellStyle.ForeColor = ConfigurationConstants.SSWhiteColor;
            dgvResults.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 11, FontStyle.Bold);
            dgvResults.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvResults.ColumnHeadersHeight = 45;

            dgvResults.DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Regular);
            dgvResults.DefaultCellStyle.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            dgvResults.DefaultCellStyle.SelectionBackColor = ConfigurationConstants.SSLightNavyColor;
            dgvResults.DefaultCellStyle.SelectionForeColor = ConfigurationConstants.SSWhiteColor;
            dgvResults.DefaultCellStyle.Padding = new Padding(5);

            dgvResults.AlternatingRowsDefaultCellStyle.BackColor = ConfigurationConstants.SSLightGrayBackground;
            dgvResults.GridColor = ConfigurationConstants.SSBorderGray;

            // Add double-click event for viewing details
            dgvResults.CellDoubleClick += DgvResults_CellDoubleClick;

            gridPanel.Controls.Add(dgvResults);
        }

        private void DgvResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvResults.Rows[e.RowIndex];
                string studentName = row.Cells["Student Name"].Value?.ToString() ?? "N/A";
                string rollNumber = row.Cells["Roll Number"].Value?.ToString() ?? "N/A";
                string subjectCode = row.Cells["Subject Code"].Value?.ToString() ?? "N/A";
                string subjectName = row.Cells["Subject Name"].Value?.ToString() ?? "N/A";
                string semester = row.Cells["Semester"].Value?.ToString() ?? "N/A";
                string totalMarks = row.Cells["Total Marks"].Value?.ToString() ?? "N/A";
                string obtainedMarks = row.Cells["Obtained Marks"].Value?.ToString() ?? "N/A";
                string percentage = row.Cells["Percentage"].Value?.ToString() ?? "N/A";
                string gpa = row.Cells["GPA"].Value?.ToString() ?? "N/A";
                string grade = row.Cells["Grade"].Value?.ToString() ?? "N/A";
                string status = row.Cells["Status"].Value?.ToString() ?? "N/A";
                string remarks = row.Cells["Remarks"].Value?.ToString() ?? "N/A";
                
                using (ResultDetailsForm detailsForm = new ResultDetailsForm(
                    studentName, rollNumber, subjectCode, subjectName, semester,
                    totalMarks, obtainedMarks, percentage, gpa, grade, status, remarks))
                {
                    detailsForm.ShowDialog(this.ParentForm);
                }
            }
        }

        private void InitializePaginationControls()
        {
            lblPageSize = new Label
            {
                Text = "Rows per page:",
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, 20),
                Size = new Size(110, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbPageSize = new ComboBox
            {
                Font = new Font("Arial", 10, FontStyle.Regular),
                Location = new Point(135, 20),
                Size = new Size(80, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPageSize.Items.AddRange(new string[] { "10", "25", "50", "100" });
            cmbPageSize.SelectedIndex = 0;
            cmbPageSize.SelectedIndexChanged += PageSizeChanged;

            btnFirst = UIHelper.CreatePaginationButton("<<", 250);
            btnFirst.Click += BtnFirst_Click;

            btnPrevious = UIHelper.CreatePaginationButton("<", 310);
            btnPrevious.Click += BtnPrevious_Click;

            lblPageInfo = new Label
            {
                Text = "Page 1 of 1",
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(370, 20),
                Size = new Size(150, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            btnNext = UIHelper.CreatePaginationButton(">", 530);
            btnNext.Click += BtnNext_Click;

            btnLast = UIHelper.CreatePaginationButton(">>", 590);
            btnLast.Click += BtnLast_Click;

            paginationPanel.Controls.Add(lblPageSize);
            paginationPanel.Controls.Add(cmbPageSize);
            paginationPanel.Controls.Add(btnFirst);
            paginationPanel.Controls.Add(btnPrevious);
            paginationPanel.Controls.Add(lblPageInfo);
            paginationPanel.Controls.Add(btnNext);
            paginationPanel.Controls.Add(btnLast);
        }

        private void LoadResults()
        {
            loadingIndicator.Show("Loading Results");

            try
            {
                // Use repository to get all enrollments with results
                List<EnrollmentViewModel> enrollments = enrollmentRepository.GetAllEnrollments();
                DataTable dt = ConvertToDataTable(enrollments);

                dgvResults.DataSource = dt;
                totalRecords = dt.Rows.Count;

                ConfigureDataGridViewColumns();
                LoadSemesters();
                UpdatePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading results: {ex.Message}",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                loadingIndicator.Hide();
            }
        }

        private void LoadSemesters()
        {
            try
            {
                DataTable dt = (DataTable)dgvResults.DataSource;
                if (dt != null)
                {
                    var semesters = dt.AsEnumerable()
                        .Select(row => row.Field<string>("Semester"))
                        .Where(s => !string.IsNullOrEmpty(s))
                        .Distinct()
                        .OrderBy(s => s)
                        .ToList();

                    cmbFilterSemester.Items.Clear();
                    cmbFilterSemester.Items.Add("All Semesters");
                    foreach (var sem in semesters)
                    {
                        cmbFilterSemester.Items.Add(sem);
                    }
                    cmbFilterSemester.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                // Silent fail - not critical
            }
        }

        private DataTable ConvertToDataTable(List<EnrollmentViewModel> enrollments)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("EnrollmentId", typeof(string));
            dt.Columns.Add("Roll Number", typeof(string));
            dt.Columns.Add("Student Name", typeof(string));
            dt.Columns.Add("Subject Code", typeof(string));
            dt.Columns.Add("Subject Name", typeof(string));
            dt.Columns.Add("Semester", typeof(string));
            dt.Columns.Add("Total Marks", typeof(string));
            dt.Columns.Add("Obtained Marks", typeof(string));
            dt.Columns.Add("Percentage", typeof(string));
            dt.Columns.Add("GPA", typeof(string));
            dt.Columns.Add("Grade", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("Remarks", typeof(string));

            foreach (EnrollmentViewModel enrollment in enrollments)
            {
                string totalMarks = enrollment.TotalMarks.HasValue ? enrollment.TotalMarks.Value.ToString() : "N/A";
                string obtainedMarks = enrollment.ObtainedMarks.HasValue ? enrollment.ObtainedMarks.Value.ToString("F2") : "N/A";
                string percentage = "N/A";
                string gpa = enrollment.GPA.HasValue ? enrollment.GPA.Value.ToString("F2") : "N/A";
                string grade = enrollment.GradeLetter ?? "N/A";
                string status = enrollment.GradeStatus;
                string remarks = enrollment.Remarks ?? "N/A";

                if (enrollment.ObtainedMarks.HasValue && enrollment.TotalMarks.HasValue && enrollment.TotalMarks.Value > 0)
                {
                    decimal perc = (enrollment.ObtainedMarks.Value / enrollment.TotalMarks.Value) * 100;
                    percentage = $"{perc:F2}%";
                }

                dt.Rows.Add(
                    enrollment.ResultId ?? enrollment.Id,
                    enrollment.Id,
                    enrollment.StudentRollNumber,
                    enrollment.StudentName,
                    enrollment.SubjectCode,
                    enrollment.SubjectName,
                    enrollment.SemesterName ?? "N/A",
                    totalMarks,
                    obtainedMarks,
                    percentage,
                    gpa,
                    grade,
                    status,
                    remarks
                );
            }

            return dt;
        }

        private void ConfigureDataGridViewColumns()
        {
            // Hide ID columns
            if (dgvResults.Columns["Id"] != null)
                dgvResults.Columns["Id"].Visible = false;
            if (dgvResults.Columns["EnrollmentId"] != null)
                dgvResults.Columns["EnrollmentId"].Visible = false;

            // Configure visible columns
            if (dgvResults.Columns["Roll Number"] != null)
            {
                dgvResults.Columns["Roll Number"].HeaderText = "Roll Number";
                dgvResults.Columns["Roll Number"].FillWeight = 80;
            }
            if (dgvResults.Columns["Student Name"] != null)
            {
                dgvResults.Columns["Student Name"].HeaderText = "Student Name";
                dgvResults.Columns["Student Name"].FillWeight = 120;
            }
            if (dgvResults.Columns["Subject Code"] != null)
            {
                dgvResults.Columns["Subject Code"].HeaderText = "Subject Code";
                dgvResults.Columns["Subject Code"].FillWeight = 70;
            }
            if (dgvResults.Columns["Subject Name"] != null)
            {
                dgvResults.Columns["Subject Name"].HeaderText = "Subject Name";
                dgvResults.Columns["Subject Name"].FillWeight = 130;
            }
            if (dgvResults.Columns["Semester"] != null)
                dgvResults.Columns["Semester"].FillWeight = 70;
            if (dgvResults.Columns["Total Marks"] != null)
            {
                dgvResults.Columns["Total Marks"].HeaderText = "Total Marks";
                dgvResults.Columns["Total Marks"].FillWeight = 70;
            }
            if (dgvResults.Columns["Obtained Marks"] != null)
            {
                dgvResults.Columns["Obtained Marks"].HeaderText = "Obtained Marks";
                dgvResults.Columns["Obtained Marks"].FillWeight = 90;
            }
            if (dgvResults.Columns["Percentage"] != null)
                dgvResults.Columns["Percentage"].FillWeight = 70;
            if (dgvResults.Columns["GPA"] != null)
                dgvResults.Columns["GPA"].FillWeight = 50;
            if (dgvResults.Columns["Grade"] != null)
                dgvResults.Columns["Grade"].FillWeight = 50;
            if (dgvResults.Columns["Status"] != null)
                dgvResults.Columns["Status"].FillWeight = 60;
            if (dgvResults.Columns["Remarks"] != null)
                dgvResults.Columns["Remarks"].FillWeight = 70;

            // Color code the Status column
            foreach (DataGridViewRow row in dgvResults.Rows)
            {
                if (row.Cells["Status"].Value != null)
                {
                    string status = row.Cells["Status"].Value.ToString();
                    if (status == "Pass")
                    {
                        row.Cells["Status"].Style.ForeColor = ConfigurationConstants.SSSuccessGreen;
                        row.Cells["Status"].Style.Font = new Font("Arial", 10, FontStyle.Bold);
                    }
                    else if (status == "Fail")
                    {
                        row.Cells["Status"].Style.ForeColor = ConfigurationConstants.SSErrorRed;
                        row.Cells["Status"].Style.Font = new Font("Arial", 10, FontStyle.Bold);
                    }
                    else
                    {
                        row.Cells["Status"].Style.ForeColor = ConfigurationConstants.SSWarningOrange;
                        row.Cells["Status"].Style.Font = new Font("Arial", 10, FontStyle.Bold);
                    }
                }
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == txtSearch.Tag.ToString() || string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                LoadResults();
                return;
            }

            SearchResults(txtSearch.Text.Trim());
        }

        private void SearchResults(string searchTerm)
        {
            loadingIndicator.Show("Searching");

            try
            {
                // Use repository to get all enrollments
                List<EnrollmentViewModel> enrollments = enrollmentRepository.GetAllEnrollments();

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    enrollments = enrollments.Where(e =>
                        e.StudentName.ToLower().Contains(searchTerm) ||
                        e.StudentRollNumber.ToLower().Contains(searchTerm) ||
                        e.SubjectName.ToLower().Contains(searchTerm) ||
                        e.SubjectCode.ToLower().Contains(searchTerm)
                    ).ToList();
                }

                DataTable dt = ConvertToDataTable(enrollments);
                dgvResults.DataSource = dt;
                totalRecords = dt.Rows.Count;
                ConfigureDataGridViewColumns();
                UpdatePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching results: {ex.Message}",
                    "Search Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                loadingIndicator.Hide();
            }
        }

        private void BtnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = txtSearch.Tag.ToString();
            txtSearch.ForeColor = Color.Gray;
            cmbFilterSemester.SelectedIndex = 0;
            cmbFilterStatus.SelectedIndex = 0;
            LoadResults();
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            DataTable dt = (DataTable)dgvResults.DataSource;
            if (dt == null) return;

            DataView dv = dt.DefaultView;
            string filter = "";

            if (cmbFilterSemester.SelectedIndex > 0)
            {
                filter = string.Format("[Semester] = '{0}'", cmbFilterSemester.SelectedItem.ToString().Replace("'", "''"));
            }

            if (cmbFilterStatus.SelectedIndex > 0)
            {
                if (filter.Length > 0) filter += " AND ";
                filter += string.Format("[Status] = '{0}'", cmbFilterStatus.SelectedItem.ToString());
            }

            dv.RowFilter = filter;
            totalRecords = dv.Count;
            UpdatePagination();

            // Reapply color coding after filtering
            foreach (DataGridViewRow row in dgvResults.Rows)
            {
                if (row.Cells["Status"].Value != null)
                {
                    string status = row.Cells["Status"].Value.ToString();
                    if (status == "Pass")
                    {
                        row.Cells["Status"].Style.ForeColor = ConfigurationConstants.SSSuccessGreen;
                        row.Cells["Status"].Style.Font = new Font("Arial", 10, FontStyle.Bold);
                    }
                    else if (status == "Fail")
                    {
                        row.Cells["Status"].Style.ForeColor = ConfigurationConstants.SSErrorRed;
                        row.Cells["Status"].Style.Font = new Font("Arial", 10, FontStyle.Bold);
                    }
                    else
                    {
                        row.Cells["Status"].Style.ForeColor = ConfigurationConstants.SSWarningOrange;
                        row.Cells["Status"].Style.Font = new Font("Arial", 10, FontStyle.Bold);
                    }
                }
            }
        }

        private void PageSizeChanged(object sender, EventArgs e)
        {
            pageSize = int.Parse(cmbPageSize.SelectedItem.ToString());
            currentPage = 1;
            UpdatePagination();
        }

        private void BtnFirst_Click(object sender, EventArgs e)
        {
            GoToPage(1);
        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            GoToPage(currentPage - 1);
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            GoToPage(currentPage + 1);
        }

        private void BtnLast_Click(object sender, EventArgs e)
        {
            GoToPage(totalPages);
        }

        private void GoToPage(int page)
        {
            if (page < 1 || page > totalPages) return;
            currentPage = page;
            UpdatePagination();
        }

        private void UpdatePagination()
        {
            totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (totalPages == 0) totalPages = 1;
            if (currentPage > totalPages) currentPage = totalPages;

            lblPageInfo.Text = string.Format("Page {0} of {1}", currentPage, totalPages);
            lblTotalRecords.Text = string.Format("Total Records: {0}", totalRecords);

            btnFirst.Enabled = currentPage > 1;
            btnPrevious.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < totalPages;
            btnLast.Enabled = currentPage < totalPages;

            UIHelper.UpdateButtonState(btnFirst);
            UIHelper.UpdateButtonState(btnPrevious);
            UIHelper.UpdateButtonState(btnNext);
            UIHelper.UpdateButtonState(btnLast);
        }
    }
}
