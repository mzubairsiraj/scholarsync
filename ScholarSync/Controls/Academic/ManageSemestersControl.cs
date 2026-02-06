using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ScholarSync.Commons;
using ScholarSync.Models;
using ScholarSync.Repositories;

namespace ScholarSync.Controls.Academic
{
    public partial class ManageSemestersControl : UserControl
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Panel formPanel;
        private Panel gridPanel;
        
        // Form Controls
        private Label lblSemesterName;
        private ComboBox cmbSemesterName;
        private Label lblStartDate;
        private DateTimePicker dtpStartDate;
        private Label lblEndDate;
        private DateTimePicker dtpEndDate;
        private CheckBox chkIsCurrent;
        
        // Buttons
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnClear;
        private Button btnSetCurrent;
        
        // DataGridView
        private DataGridView dgvSemesters;
        
        // Loading Indicator
        private LoadingIndicator loadingIndicator;
        
        // Repository
        private SemesterRepository semesterRepository;
        
        // State
        private string selectedSemesterId = "";

        public ManageSemestersControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.BackColor = ConfigurationConstants.SSWhiteColor;
            
            // Check if user is Admin
            if (!SessionManager.IsAdmin())
            {
                ShowAccessDenied();
                return;
            }
            
            // Initialize repository
            semesterRepository = new SemesterRepository();
            
            InitializeUI();
            
            // Initialize loading indicator
            loadingIndicator = new LoadingIndicator();
            this.Controls.Add(loadingIndicator);
            
            LoadSemesters();
        }

        private void ShowAccessDenied()
        {
            Panel accessDeniedPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ConfigurationConstants.SSWhiteColor
            };

            Label lblAccessDenied = new Label
            {
                Text = "?? Access Denied",
                Font = new Font("Arial", 32, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSErrorRed,
                Location = new Point(0, 200),
                Size = new Size(ConfigurationConstants.ScreenWidth - 300, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblMessage = new Label
            {
                Text = "Only Administrators can manage semesters.\n\nPlease contact your system administrator for access.",
                Font = new Font("Arial", 14, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(0, 280),
                Size = new Size(ConfigurationConstants.ScreenWidth - 300, 80),
                TextAlign = ContentAlignment.TopCenter
            };

            accessDeniedPanel.Controls.Add(lblAccessDenied);
            accessDeniedPanel.Controls.Add(lblMessage);
            this.Controls.Add(accessDeniedPanel);
        }

        private void InitializeComponent()
        {
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
                Text = "Manage Semesters",
                Font = new Font("Arial", 22, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(30, 0),
                Size = new Size(500, 80),
                TextAlign = ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.Add(titleLabel);
            this.Controls.Add(headerPanel);

            // Form Panel
            formPanel = new Panel
            {
                Dock = DockStyle.None,
                BackColor = ConfigurationConstants.SSLightGrayBackground,
                Location = new Point(10, 100),
                Size = new Size(ConfigurationConstants.ScreenWidth - 300, 260),
                Padding = new Padding(20)
            };
            
            InitializeFormControls();
            this.Controls.Add(formPanel);

            // Grid Panel
            gridPanel = new Panel
            {
                Dock = DockStyle.None,
                BackColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(10, 380),
                Size = new Size(ConfigurationConstants.ScreenWidth - 300, ConfigurationConstants.ScreenHeight - 560),
                Padding = new Padding(20)
            };
            
            InitializeDataGridView();
            this.Controls.Add(gridPanel);
        }

        private void InitializeFormControls()
        {
            int yPos = 20;
            int labelWidth = 150;
            int controlWidth = 250;
            int controlHeight = 40;
            int xLabelPos = 20;
            int xControlPos = 180;

            // Semester Name
            lblSemesterName = new Label
            {
                Text = "Semester Name:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(xLabelPos, yPos),
                Size = new Size(labelWidth, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbSemesterName = new ComboBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(xControlPos, yPos),
                Size = new Size(controlWidth, controlHeight),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            // Add Semester 1 through Semester 8
            for (int i = 1; i <= 8; i++)
            {
                cmbSemesterName.Items.Add($"Semester {i}");
            }
            cmbSemesterName.SelectedIndex = 0;

            formPanel.Controls.Add(lblSemesterName);
            formPanel.Controls.Add(cmbSemesterName);

            yPos += 60;

            // Start Date
            lblStartDate = new Label
            {
                Text = "Start Date:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(xLabelPos, yPos),
                Size = new Size(labelWidth, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            dtpStartDate = new DateTimePicker
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(xControlPos, yPos),
                Size = new Size(controlWidth, controlHeight),
                Format = DateTimePickerFormat.Short
            };

            formPanel.Controls.Add(lblStartDate);
            formPanel.Controls.Add(dtpStartDate);

            // End Date
            lblEndDate = new Label
            {
                Text = "End Date:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(xLabelPos + 500, yPos),
                Size = new Size(labelWidth, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            dtpEndDate = new DateTimePicker
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(xControlPos + 500, yPos),
                Size = new Size(controlWidth, controlHeight),
                Format = DateTimePickerFormat.Short
            };

            formPanel.Controls.Add(lblEndDate);
            formPanel.Controls.Add(dtpEndDate);

            yPos += 60;

            // Is Current Checkbox
            chkIsCurrent = new CheckBox
            {
                Text = "Set as Current Semester",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(xLabelPos, yPos),
                Size = new Size(250, 30),
                Checked = false
            };

            formPanel.Controls.Add(chkIsCurrent);

            yPos += 50;

            // Buttons
            btnAdd = UIHelper.CreateButton("Add Semester", ConfigurationConstants.SSSuccessGreen, new Point(xLabelPos, yPos), new Size(150, 45));
            btnAdd.Click += BtnAdd_Click;

            btnUpdate = UIHelper.CreateButton("Update Semester", ConfigurationConstants.SSDarkNavyColor, new Point(xLabelPos + 170, yPos), new Size(170, 45));
            btnUpdate.Click += BtnUpdate_Click;
            btnUpdate.Enabled = false;

            btnSetCurrent = UIHelper.CreateButton("Set as Current", ConfigurationConstants.SSWarningOrange, new Point(xLabelPos + 360, yPos), new Size(150, 45));
            btnSetCurrent.Click += BtnSetCurrent_Click;
            btnSetCurrent.Enabled = false;

            btnClear = UIHelper.CreateButton("Clear Form", ConfigurationConstants.SSDarkBlueColor, new Point(xLabelPos + 530, yPos), new Size(130, 45));
            btnClear.Click += BtnClear_Click;

            formPanel.Controls.Add(btnAdd);
            formPanel.Controls.Add(btnUpdate);
            formPanel.Controls.Add(btnSetCurrent);
            formPanel.Controls.Add(btnClear);
        }

        private void InitializeDataGridView()
        {
            dgvSemesters = new DataGridView
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
                Size = new Size(gridPanel.Width - 40, gridPanel.Height - 40),
            };

            dgvSemesters.ColumnHeadersDefaultCellStyle.BackColor = ConfigurationConstants.SSDarkNavyColor;
            dgvSemesters.ColumnHeadersDefaultCellStyle.ForeColor = ConfigurationConstants.SSWhiteColor;
            dgvSemesters.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 11, FontStyle.Bold);
            dgvSemesters.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvSemesters.ColumnHeadersHeight = 45;

            dgvSemesters.DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Regular);
            dgvSemesters.DefaultCellStyle.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            dgvSemesters.DefaultCellStyle.SelectionBackColor = ConfigurationConstants.SSLightNavyColor;
            dgvSemesters.DefaultCellStyle.SelectionForeColor = ConfigurationConstants.SSWhiteColor;
            dgvSemesters.DefaultCellStyle.Padding = new Padding(5);

            dgvSemesters.AlternatingRowsDefaultCellStyle.BackColor = ConfigurationConstants.SSLightGrayBackground;
            dgvSemesters.GridColor = ConfigurationConstants.SSBorderGray;

            dgvSemesters.CellClick += DgvSemesters_CellClick;

            gridPanel.Controls.Add(dgvSemesters);
        }

        private void LoadSemesters()
        {
            loadingIndicator.Show("Loading Semesters");

            try
            {
                List<SemesterModel> semesters = semesterRepository.GetAllSemesters();
                DataTable dt = ConvertToDataTable(semesters);

                dgvSemesters.DataSource = dt;
                ConfigureDataGridViewColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading semesters: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                loadingIndicator.Hide();
            }
        }

        private DataTable ConvertToDataTable(List<SemesterModel> semesters)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Start Date", typeof(string));
            dt.Columns.Add("End Date", typeof(string));
            dt.Columns.Add("Status", typeof(string));

            foreach (SemesterModel semester in semesters)
            {
                dt.Rows.Add(
                    semester.Id,
                    semester.Name,
                    semester.StartDate.ToString("MM/dd/yyyy"),
                    semester.EndDate.ToString("MM/dd/yyyy"),
                    semester.IsCurrent ? "Current" : "Inactive"
                );
            }

            return dt;
        }

        private void ConfigureDataGridViewColumns()
        {
            if (dgvSemesters.Columns["Id"] != null)
                dgvSemesters.Columns["Id"].Visible = false;

            if (dgvSemesters.Columns["Name"] != null)
            {
                dgvSemesters.Columns["Name"].HeaderText = "Semester";
                dgvSemesters.Columns["Name"].FillWeight = 100;
            }
            if (dgvSemesters.Columns["Start Date"] != null)
                dgvSemesters.Columns["Start Date"].FillWeight = 120;
            if (dgvSemesters.Columns["End Date"] != null)
                dgvSemesters.Columns["End Date"].FillWeight = 120;
            if (dgvSemesters.Columns["Status"] != null)
            {
                dgvSemesters.Columns["Status"].FillWeight = 80;
                
                // Color current semester row
                foreach (DataGridViewRow row in dgvSemesters.Rows)
                {
                    if (row.Cells["Status"].Value?.ToString() == "Current")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                        row.DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
                    }
                }
            }
        }

        private void DgvSemesters_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSemesters.Rows[e.RowIndex];
                
                selectedSemesterId = row.Cells["Id"].Value?.ToString();
                
                cmbSemesterName.SelectedItem = row.Cells["Name"].Value?.ToString();
                
                DateTime startDate;
                if (DateTime.TryParse(row.Cells["Start Date"].Value?.ToString(), out startDate))
                    dtpStartDate.Value = startDate;
                
                DateTime endDate;
                if (DateTime.TryParse(row.Cells["End Date"].Value?.ToString(), out endDate))
                    dtpEndDate.Value = endDate;
                
                chkIsCurrent.Checked = row.Cells["Status"].Value?.ToString() == "Current";

                btnAdd.Enabled = false;
                btnUpdate.Enabled = true;
                btnSetCurrent.Enabled = true;
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            loadingIndicator.Show("Adding Semester");

            try
            {
                SemesterModel semester = new SemesterModel
                {
                    Name = cmbSemesterName.SelectedItem.ToString(),
                    StartDate = dtpStartDate.Value,
                    EndDate = dtpEndDate.Value,
                    IsCurrent = chkIsCurrent.Checked
                };

                string semesterId = semesterRepository.AddSemester(semester);

                if (!string.IsNullOrEmpty(semesterId))
                {
                    MessageBox.Show("Semester added successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    ClearForm();
                    LoadSemesters();
                }
                else
                {
                    MessageBox.Show("Failed to add semester.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding semester: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                loadingIndicator.Hide();
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            loadingIndicator.Show("Updating Semester");

            try
            {
                SemesterModel semester = new SemesterModel
                {
                    Id = selectedSemesterId,
                    Name = cmbSemesterName.SelectedItem.ToString(),
                    StartDate = dtpStartDate.Value,
                    EndDate = dtpEndDate.Value,
                    IsCurrent = chkIsCurrent.Checked
                };

                bool success = semesterRepository.UpdateSemester(semester);

                if (success)
                {
                    MessageBox.Show("Semester updated successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    ClearForm();
                    LoadSemesters();
                }
                else
                {
                    MessageBox.Show("Failed to update semester.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating semester: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                loadingIndicator.Hide();
            }
        }

        private void BtnSetCurrent_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedSemesterId))
            {
                MessageBox.Show("Please select a semester first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to set this as the current semester?\n\nThis will unset any other current semester.",
                "Confirm Set Current",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                loadingIndicator.Show("Setting Current Semester");

                try
                {
                    bool success = semesterRepository.SetCurrentSemester(selectedSemesterId);

                    if (success)
                    {
                        MessageBox.Show("Current semester updated successfully!",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        ClearForm();
                        LoadSemesters();
                    }
                    else
                    {
                        MessageBox.Show("Failed to set current semester.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error setting current semester: {ex.Message}",
                        "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    loadingIndicator.Hide();
                }
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private bool ValidateForm()
        {
            if (cmbSemesterName.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a semester name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbSemesterName.Focus();
                return false;
            }

            if (dtpEndDate.Value <= dtpStartDate.Value)
            {
                MessageBox.Show("End date must be after start date.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpEndDate.Focus();
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            cmbSemesterName.SelectedIndex = 0;
            dtpStartDate.Value = DateTime.Now;
            dtpEndDate.Value = DateTime.Now.AddMonths(4);
            chkIsCurrent.Checked = false;

            selectedSemesterId = "";
            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnSetCurrent.Enabled = false;
        }
    }
}
