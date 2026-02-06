using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ScholarSync.Commons;
using ScholarSync.Models;
using ScholarSync.Db_Connection;
using ScholarSync.Repositories;
using Npgsql;

namespace ScholarSync.Controls.Student
{
    public partial class ListStudentsControl : UserControl
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Panel searchPanel;
        private Panel gridPanel;
        private Panel paginationPanel;
        
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnClearSearch;
        private ComboBox cmbFilterProgram;
        private ComboBox cmbFilterGender;
        private Label lblSearchLabel;
        private Label lblFilterProgram;
        private Label lblFilterGender;
        private Label lblTotalRecords;
        
        private DataGridView dgvStudents;
        
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

        private LoadingIndicator loadingIndicator;
        
        // Repository
        private StudentRepository studentRepository;

        public ListStudentsControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.BackColor = ConfigurationConstants.SSWhiteColor;
            
            // Initialize repository
            studentRepository = new StudentRepository();
            
            InitializeUI();
            
            // Initialize loading indicator
            loadingIndicator = new LoadingIndicator();
            this.Controls.Add(loadingIndicator);
            
            LoadPrograms();
            LoadStudents();
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
                Text = "Students List",
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
                Text = "Search by Name, CNIC, Roll Number...",
                Tag = "Search by Name, CNIC, Roll Number...",
               
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

            lblFilterProgram = new Label
            {
                Text = "Program:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(labelWidth, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbFilterProgram = new ComboBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(120, yPos),
                Size = new Size(250, controlHeight),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilterProgram.Items.AddRange(new string[] { "All Programs", "BS Computer Science", "BS Information Technology", "BS Software Engineering" });
            cmbFilterProgram.SelectedIndex = 0;
            cmbFilterProgram.SelectedIndexChanged += FilterChanged;

            lblFilterGender = new Label
            {
                Text = "Gender:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(400, yPos),
                Size = new Size(labelWidth, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbFilterGender = new ComboBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(500, yPos),
                Size = new Size(200, controlHeight),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilterGender.Items.AddRange(new string[] { "All Genders", "Male", "Female", "Other" });
            cmbFilterGender.SelectedIndex = 0;
            cmbFilterGender.SelectedIndexChanged += FilterChanged;

            lblTotalRecords = new Label
            {
                Text = "Total Records: 0",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(730, yPos),
                Size = new Size(200, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            searchPanel.Controls.Add(lblFilterProgram);
            searchPanel.Controls.Add(cmbFilterProgram);
            searchPanel.Controls.Add(lblFilterGender);
            searchPanel.Controls.Add(cmbFilterGender);
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
            dgvStudents = new DataGridView
            {
                //Dock = DockStyle.Fill,
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

            dgvStudents.ColumnHeadersDefaultCellStyle.BackColor = ConfigurationConstants.SSDarkNavyColor;
            dgvStudents.ColumnHeadersDefaultCellStyle.ForeColor = ConfigurationConstants.SSWhiteColor;
            dgvStudents.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 11, FontStyle.Bold);
            dgvStudents.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvStudents.ColumnHeadersHeight = 45;

            dgvStudents.DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Regular);
            dgvStudents.DefaultCellStyle.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            dgvStudents.DefaultCellStyle.SelectionBackColor = ConfigurationConstants.SSLightNavyColor;
            dgvStudents.DefaultCellStyle.SelectionForeColor = ConfigurationConstants.SSWhiteColor;
            dgvStudents.DefaultCellStyle.Padding = new Padding(5);

            dgvStudents.AlternatingRowsDefaultCellStyle.BackColor = ConfigurationConstants.SSLightGrayBackground;
            dgvStudents.GridColor = ConfigurationConstants.SSBorderGray;

            gridPanel.Controls.Add(dgvStudents);
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

        private void LoadPrograms()
        {
            loadingIndicator.Show("Loading Programs");

            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT name FROM programs WHERE is_active = true ORDER BY name";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            cmbFilterProgram.Items.Clear();
                            cmbFilterProgram.Items.Add("All Programs");

                            while (reader.Read())
                            {
                                cmbFilterProgram.Items.Add(reader["name"].ToString());
                            }

                            cmbFilterProgram.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading programs: {ex.Message}",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                loadingIndicator.Hide();
            }
        }

        private void LoadStudents()
        {
            loadingIndicator.Show("Loading Students");

            try
            {
                // Use repository to get all students
                List<StudentViewModel> students = studentRepository.GetAllStudents();
                DataTable dt = ConvertToDataTable(students);

                dgvStudents.DataSource = dt;
                totalRecords = dt.Rows.Count;

                ConfigureDataGridViewColumns();
                UpdatePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading students: {ex.Message}",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                loadingIndicator.Hide();
            }
        }

        /// <summary>
        /// Configures DataGridView columns visibility and formatting
        /// </summary>
        private void ConfigureDataGridViewColumns()
        {
            // Hide ID columns
            if (dgvStudents.Columns["Id"] != null)
                dgvStudents.Columns["Id"].Visible = false;
            if (dgvStudents.Columns["UserId"] != null)
                dgvStudents.Columns["UserId"].Visible = false;
            if (dgvStudents.Columns["ProgramId"] != null)
                dgvStudents.Columns["ProgramId"].Visible = false;

            // Configure visible columns
            if (dgvStudents.Columns["RollNumber"] != null)
            {
                dgvStudents.Columns["RollNumber"].HeaderText = "Roll Number";
                dgvStudents.Columns["RollNumber"].FillWeight = 80;
            }
            if (dgvStudents.Columns["FullName"] != null)
            {
                dgvStudents.Columns["FullName"].HeaderText = "Full Name";
                dgvStudents.Columns["FullName"].FillWeight = 120;
            }
            if (dgvStudents.Columns["CNIC"] != null)
                dgvStudents.Columns["CNIC"].FillWeight = 100;
            if (dgvStudents.Columns["Gender"] != null)
                dgvStudents.Columns["Gender"].FillWeight = 60;
            if (dgvStudents.Columns["ProgramName"] != null)
            {
                dgvStudents.Columns["ProgramName"].HeaderText = "Program";
                dgvStudents.Columns["ProgramName"].FillWeight = 150;
            }
            if (dgvStudents.Columns["MobileNo"] != null)
            {
                dgvStudents.Columns["MobileNo"].HeaderText = "Mobile No";
                dgvStudents.Columns["MobileNo"].FillWeight = 90;
            }
            if (dgvStudents.Columns["Email"] != null)
                dgvStudents.Columns["Email"].FillWeight = 120;

            // Hide detailed fields
            if (dgvStudents.Columns["DateOfBirth"] != null)
                dgvStudents.Columns["DateOfBirth"].Visible = false;
            if (dgvStudents.Columns["BloodGroup"] != null)
                dgvStudents.Columns["BloodGroup"].Visible = false;
            if (dgvStudents.Columns["Religion"] != null)
                dgvStudents.Columns["Religion"].Visible = false;
            if (dgvStudents.Columns["Nationality"] != null)
                dgvStudents.Columns["Nationality"].Visible = false;
            if (dgvStudents.Columns["DomicileDistrict"] != null)
                dgvStudents.Columns["DomicileDistrict"].Visible = false;
            if (dgvStudents.Columns["WhatsappNo"] != null)
                dgvStudents.Columns["WhatsappNo"].Visible = false;
            if (dgvStudents.Columns["DepartmentName"] != null)
                dgvStudents.Columns["DepartmentName"].Visible = false;
            if (dgvStudents.Columns["DepartmentCode"] != null)
                dgvStudents.Columns["DepartmentCode"].Visible = false;
        }

        private DataTable ConvertToDataTable(List<StudentViewModel> students)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("UserId", typeof(string));
            dt.Columns.Add("ProgramId", typeof(string));
            dt.Columns.Add("RollNumber", typeof(string));
            dt.Columns.Add("FullName", typeof(string));
            dt.Columns.Add("CNIC", typeof(string));
            dt.Columns.Add("Gender", typeof(string));
            dt.Columns.Add("DateOfBirth", typeof(DateTime));
            dt.Columns.Add("ProgramName", typeof(string));
            dt.Columns.Add("DepartmentName", typeof(string));
            dt.Columns.Add("DepartmentCode", typeof(string));
            dt.Columns.Add("MobileNo", typeof(string));
            dt.Columns.Add("WhatsappNo", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("BloodGroup", typeof(string));
            dt.Columns.Add("Religion", typeof(string));
            dt.Columns.Add("Nationality", typeof(string));
            dt.Columns.Add("DomicileDistrict", typeof(string));

            foreach (StudentViewModel student in students)
            {
                dt.Rows.Add(
                    student.Id,
                    student.UserId,
                    student.ProgramId,
                    student.RollNumber,
                    student.FullName,
                    student.CNIC,
                    student.Gender,
                    student.DateOfBirth,
                    student.ProgramName,
                    student.DepartmentName,
                    student.DepartmentCode,
                    student.MobileNo,
                    student.WhatsappNo,
                    student.Email,
                    student.BloodGroup,
                    student.Religion,
                    student.Nationality,
                    student.DomicileDistrict
                );
            }

            return dt;
        }

        private void SearchStudents(string searchTerm)
        {
            loadingIndicator.Show("Searching");

            try
            {
                // Use repository to get all students
                List<StudentViewModel> students = studentRepository.GetAllStudents();

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    students = students.FindAll(s =>
                        s.FullName.ToLower().Contains(searchTerm) ||
                        s.RollNumber.ToLower().Contains(searchTerm) ||
                        s.CNIC.Contains(searchTerm) ||
                        (s.Email != null && s.Email.ToLower().Contains(searchTerm))
                    );
                }

                DataTable dt = ConvertToDataTable(students);
                dgvStudents.DataSource = dt;
                totalRecords = dt.Rows.Count;
                ConfigureDataGridViewColumns();
                UpdatePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching students: {ex.Message}",
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
                LoadStudents();
                return;
            }

            SearchStudents(txtSearch.Text.Trim());
        }

        private void BtnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = txtSearch.Tag.ToString();
            txtSearch.ForeColor = Color.Gray;
            cmbFilterProgram.SelectedIndex = 0;
            cmbFilterGender.SelectedIndex = 0;
            LoadStudents();
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            DataTable dt = (DataTable)dgvStudents.DataSource;
            if (dt == null) return;

            DataView dv = dt.DefaultView;
            string filter = "";

            if (cmbFilterProgram.SelectedIndex > 0)
            {
                filter = string.Format("[ProgramName] = '{0}'", cmbFilterProgram.SelectedItem.ToString().Replace("'", "''"));
            }

            if (cmbFilterGender.SelectedIndex > 0)
            {
                if (filter.Length > 0) filter += " AND ";
                filter += string.Format("[Gender] = '{0}'", cmbFilterGender.SelectedItem.ToString());
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
