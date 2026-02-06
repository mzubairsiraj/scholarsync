using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ScholarSync.Commons;
using ScholarSync.Models;
using ScholarSync.Repositories;

namespace ScholarSync.Controls.Teacher
{
    public partial class ListTeachersControl : UserControl
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Panel searchPanel;
        private Panel gridPanel;
        private Panel paginationPanel;
        
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnClearSearch;
        private ComboBox cmbFilterDepartment;
        private ComboBox cmbFilterDesignation;
        private Label lblSearchLabel;
        private Label lblFilterDepartment;
        private Label lblFilterDesignation;
        private Label lblTotalRecords;
        
        private DataGridView dgvTeachers;
        
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
        private TeacherRepository teacherRepository;

        public ListTeachersControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.BackColor = ConfigurationConstants.SSWhiteColor;
            
            // Initialize repository
            teacherRepository = new TeacherRepository();
            
            InitializeUI();
            
            // Initialize loading indicator
            loadingIndicator = new LoadingIndicator();
            this.Controls.Add(loadingIndicator);
            
            LoadTeachers();
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
                Text = "Teachers List",
                Font = new Font("Arial", 22, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(30, 0),
                Size = new Size(400, 80),
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
                Text = "Search by Name, CNIC, Employee ID...",
                Tag = "Search by Name, CNIC, Employee ID...",
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

            lblFilterDepartment = new Label
            {
                Text = "Department:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(labelWidth, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbFilterDepartment = new ComboBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(120, yPos),
                Size = new Size(250, controlHeight),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilterDepartment.Items.AddRange(new string[] { "All Departments", "Computer Science", "Information Technology", "Software Engineering", "Mathematics", "Physics" });
            cmbFilterDepartment.SelectedIndex = 0;
            cmbFilterDepartment.SelectedIndexChanged += FilterChanged;

            lblFilterDesignation = new Label
            {
                Text = "Designation:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(400, yPos),
                Size = new Size(labelWidth, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbFilterDesignation = new ComboBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(510, yPos),
                Size = new Size(200, controlHeight),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilterDesignation.Items.AddRange(new string[] { "All Designations", "Professor", "Associate Professor", "Assistant Professor", "Lecturer", "Visiting Faculty" });
            cmbFilterDesignation.SelectedIndex = 0;
            cmbFilterDesignation.SelectedIndexChanged += FilterChanged;

            lblTotalRecords = new Label
            {
                Text = "Total Records: 0",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(730, yPos),
                Size = new Size(200, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            searchPanel.Controls.Add(lblFilterDepartment);
            searchPanel.Controls.Add(cmbFilterDepartment);
            searchPanel.Controls.Add(lblFilterDesignation);
            searchPanel.Controls.Add(cmbFilterDesignation);
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
            dgvTeachers = new DataGridView
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

            dgvTeachers.ColumnHeadersDefaultCellStyle.BackColor = ConfigurationConstants.SSDarkNavyColor;
            dgvTeachers.ColumnHeadersDefaultCellStyle.ForeColor = ConfigurationConstants.SSWhiteColor;
            dgvTeachers.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 11, FontStyle.Bold);
            dgvTeachers.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvTeachers.ColumnHeadersHeight = 45;

            dgvTeachers.DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Regular);
            dgvTeachers.DefaultCellStyle.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            dgvTeachers.DefaultCellStyle.SelectionBackColor = ConfigurationConstants.SSLightNavyColor;
            dgvTeachers.DefaultCellStyle.SelectionForeColor = ConfigurationConstants.SSWhiteColor;
            dgvTeachers.DefaultCellStyle.Padding = new Padding(5);

            dgvTeachers.AlternatingRowsDefaultCellStyle.BackColor = ConfigurationConstants.SSLightGrayBackground;
            dgvTeachers.GridColor = ConfigurationConstants.SSBorderGray;

            gridPanel.Controls.Add(dgvTeachers);
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

        private void LoadSampleData()
        {
            loadingIndicator.Show("Loading Teachers");

            try
            {
                // Use repository to get all teachers
                List<TeacherViewModel> teachers = teacherRepository.GetAllTeachers();
                DataTable dt = ConvertToDataTable(teachers);

                dgvTeachers.DataSource = dt;
                totalRecords = dt.Rows.Count;

                ConfigureDataGridViewColumns();
                UpdatePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading teachers: {ex.Message}",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                loadingIndicator.Hide();
            }
        }

        private void LoadTeachers()
        {
            LoadSampleData();
        }

        /// <summary>
        /// Configures DataGridView columns visibility and formatting
        /// </summary>
        private void ConfigureDataGridViewColumns()
        {
            // Hide ID columns
            if (dgvTeachers.Columns["Id"] != null)
                dgvTeachers.Columns["Id"].Visible = false;
            if (dgvTeachers.Columns["UserId"] != null)
                dgvTeachers.Columns["UserId"].Visible = false;
            if (dgvTeachers.Columns["DepartmentId"] != null)
                dgvTeachers.Columns["DepartmentId"].Visible = false;

            // Configure visible columns
            if (dgvTeachers.Columns["CNIC"] != null)
            {
                dgvTeachers.Columns["CNIC"].HeaderText = "CNIC";
                dgvTeachers.Columns["CNIC"].FillWeight = 100;
            }
            if (dgvTeachers.Columns["FullName"] != null)
            {
                dgvTeachers.Columns["FullName"].HeaderText = "Full Name";
                dgvTeachers.Columns["FullName"].FillWeight = 130;
            }
            if (dgvTeachers.Columns["Designation"] != null)
            {
                dgvTeachers.Columns["Designation"].HeaderText = "Designation";
                dgvTeachers.Columns["Designation"].FillWeight = 120;
            }
            if (dgvTeachers.Columns["Specialization"] != null)
            {
                dgvTeachers.Columns["Specialization"].HeaderText = "Specialization";
                dgvTeachers.Columns["Specialization"].FillWeight = 120;
            }
            if (dgvTeachers.Columns["Email"] != null)
            {
                dgvTeachers.Columns["Email"].HeaderText = "Email";
                dgvTeachers.Columns["Email"].FillWeight = 150;
            }
        }

        private DataTable ConvertToDataTable(List<TeacherViewModel> teachers)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("UserId", typeof(string));
            dt.Columns.Add("DepartmentId", typeof(string));
            dt.Columns.Add("CNIC", typeof(string));
            dt.Columns.Add("FullName", typeof(string));
            dt.Columns.Add("Designation", typeof(string));
            dt.Columns.Add("Specialization", typeof(string));
            dt.Columns.Add("Email", typeof(string));

            foreach (TeacherViewModel teacher in teachers)
            {
                dt.Rows.Add(
                    teacher.Id,
                    teacher.UserId,
                    teacher.DepartmentId,
                    teacher.CNIC,
                    teacher.FullName,
                    teacher.Designation ?? "N/A",
                    teacher.Specialization ?? "N/A",
                    teacher.Email
                );
            }

            return dt;
        }

        private void SearchTeachers(string searchTerm)
        {
            loadingIndicator.Show("Searching");

            try
            {
                // Use repository to get all teachers
                List<TeacherViewModel> teachers = teacherRepository.GetAllTeachers();

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    teachers = teachers.FindAll(t =>
                        (t.FullName != null && t.FullName.ToLower().Contains(searchTerm)) ||
                        t.CNIC.Contains(searchTerm) ||
                        (t.Email != null && t.Email.ToLower().Contains(searchTerm))
                    );
                }

                DataTable dt = ConvertToDataTable(teachers);
                dgvTeachers.DataSource = dt;
                totalRecords = dt.Rows.Count;
                ConfigureDataGridViewColumns();
                UpdatePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching teachers: {ex.Message}",
                    "Search Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                loadingIndicator.Hide();
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == txtSearch.Tag.ToString() || string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                LoadTeachers();
                return;
            }

            SearchTeachers(txtSearch.Text.Trim());
        }

        private void BtnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = txtSearch.Tag.ToString();
            txtSearch.ForeColor = Color.Gray;
            cmbFilterDepartment.SelectedIndex = 0;
            cmbFilterDesignation.SelectedIndex = 0;
            LoadTeachers();
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            DataTable dt = (DataTable)dgvTeachers.DataSource;
            if (dt == null) return;

            DataView dv = dt.DefaultView;
            string filter = "";

            if (cmbFilterDesignation.SelectedIndex > 0)
            {
                filter = string.Format("[Designation] = '{0}'", cmbFilterDesignation.SelectedItem.ToString().Replace("'", "''"));
            }

            dv.RowFilter = filter;
            totalRecords = dv.Count;
            UpdatePagination();
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
