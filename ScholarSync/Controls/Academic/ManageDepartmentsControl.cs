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
    public partial class ManageDepartmentsControl : UserControl
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Panel formPanel;
        private Panel gridPanel;
        
        // Form Controls
        private Label lblDepartmentName;
        private TextBox txtDepartmentName;
        private Label lblDepartmentCode;
        private TextBox txtDepartmentCode;
        private Label lblHeadOfDepartment;
        private ComboBox cmbHeadOfDepartment;
        private CheckBox chkIsActive;
        
        // Buttons
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnClear;
        
        // DataGridView
        private DataGridView dgvDepartments;
        
        // Loading Indicator
        private LoadingIndicator loadingIndicator;
        
        // Repositories
        private DepartmentRepository departmentRepository;
        private TeacherRepository teacherRepository;
        
        // State
        private string selectedDepartmentId = "";
       

        public ManageDepartmentsControl()
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
            
            // Initialize repositories
            departmentRepository = new DepartmentRepository();
            teacherRepository = new TeacherRepository();
            
            InitializeUI();
            
            // Initialize loading indicator
            loadingIndicator = new LoadingIndicator();
            this.Controls.Add(loadingIndicator);
            
            LoadDepartments();
            LoadTeachers();
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
                Text = "! Access Denied",
                Font = new Font("Arial", 32, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSErrorRed,
                Location = new Point(0, 200),
                Size = new Size(ConfigurationConstants.ScreenWidth - 300, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblMessage = new Label
            {
                Text = "Only Administrators can manage departments.\n\nPlease contact your system administrator for access.",
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
                Text = "Manage Departments",
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
                Size = new Size(ConfigurationConstants.ScreenWidth - 300, 200),
                Padding = new Padding(20)
            };
            
            InitializeFormControls();
            this.Controls.Add(formPanel);

            // Grid Panel
            gridPanel = new Panel
            {
                Dock = DockStyle.None,
                BackColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(10, 320),
                Size = new Size(ConfigurationConstants.ScreenWidth - 300, ConfigurationConstants.ScreenHeight - 500),
                Padding = new Padding(20)
            };
            
            InitializeDataGridView();
            this.Controls.Add(gridPanel);
        }

        private void InitializeFormControls()
        {
            int yPos = 20;
            int labelWidth = 150;
            int controlWidth = 300;
            int controlHeight = 40;
            int xLabelPos = 20;
            int xControlPos = 180;

            // Department Name
            lblDepartmentName = new Label
            {
                Text = "Department Name:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(xLabelPos, yPos),
                Size = new Size(labelWidth, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtDepartmentName = new TextBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(xControlPos, yPos),
                Size = new Size(controlWidth, controlHeight)
            };

            formPanel.Controls.Add(lblDepartmentName);
            formPanel.Controls.Add(txtDepartmentName);

            // Department Code
            lblDepartmentCode = new Label
            {
                Text = "Department Code:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(xLabelPos + 500, yPos),
                Size = new Size(labelWidth, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtDepartmentCode = new TextBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(xControlPos + 500, yPos),
                Size = new Size(200, controlHeight)
            };

            formPanel.Controls.Add(lblDepartmentCode);
            formPanel.Controls.Add(txtDepartmentCode);

            yPos += 60;

            // Head of Department
            lblHeadOfDepartment = new Label
            {
                Text = "Head of Department:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(xLabelPos, yPos),
                Size = new Size(labelWidth, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbHeadOfDepartment = new ComboBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(xControlPos, yPos),
                Size = new Size(controlWidth, controlHeight),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            formPanel.Controls.Add(lblHeadOfDepartment);
            formPanel.Controls.Add(cmbHeadOfDepartment);

            // Is Active Checkbox
            chkIsActive = new CheckBox
            {
                Text = "Active",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(xLabelPos + 500, yPos),
                Size = new Size(100, 30),
                Checked = true
            };

            formPanel.Controls.Add(chkIsActive);

            yPos += 60;

            // Buttons
            btnAdd = UIHelper.CreateButton("Add Department", ConfigurationConstants.SSSuccessGreen, new Point(xLabelPos, yPos), new Size(170, 45));
            btnAdd.Click += BtnAdd_Click;

            btnUpdate = UIHelper.CreateButton("Update Department", ConfigurationConstants.SSDarkNavyColor, new Point(xLabelPos + 190, yPos), new Size(180, 45));
            btnUpdate.Click += BtnUpdate_Click;
            btnUpdate.Enabled = false;

            btnDelete = UIHelper.CreateButton("Delete Department", ConfigurationConstants.SSErrorRed, new Point(xLabelPos + 390, yPos), new Size(180, 45));
            btnDelete.Click += BtnDelete_Click;
            btnDelete.Enabled = false;

            btnClear = UIHelper.CreateButton("Clear Form", ConfigurationConstants.SSDarkBlueColor, new Point(xLabelPos + 590, yPos), new Size(140, 45));
            btnClear.Click += BtnClear_Click;

            formPanel.Controls.Add(btnAdd);
            formPanel.Controls.Add(btnUpdate);
            formPanel.Controls.Add(btnDelete);
            formPanel.Controls.Add(btnClear);
        }

        private void InitializeDataGridView()
        {
            dgvDepartments = new DataGridView
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

            dgvDepartments.ColumnHeadersDefaultCellStyle.BackColor = ConfigurationConstants.SSDarkNavyColor;
            dgvDepartments.ColumnHeadersDefaultCellStyle.ForeColor = ConfigurationConstants.SSWhiteColor;
            dgvDepartments.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 11, FontStyle.Bold);
            dgvDepartments.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvDepartments.ColumnHeadersHeight = 45;

            dgvDepartments.DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Regular);
            dgvDepartments.DefaultCellStyle.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            dgvDepartments.DefaultCellStyle.SelectionBackColor = ConfigurationConstants.SSLightNavyColor;
            dgvDepartments.DefaultCellStyle.SelectionForeColor = ConfigurationConstants.SSWhiteColor;
            dgvDepartments.DefaultCellStyle.Padding = new Padding(5);

            dgvDepartments.AlternatingRowsDefaultCellStyle.BackColor = ConfigurationConstants.SSLightGrayBackground;
            dgvDepartments.GridColor = ConfigurationConstants.SSBorderGray;

            dgvDepartments.CellClick += DgvDepartments_CellClick;

            gridPanel.Controls.Add(dgvDepartments);
        }

        private void LoadTeachers()
        {
            try
            {
                cmbHeadOfDepartment.Items.Clear();
                cmbHeadOfDepartment.Items.Add("-- Select Teacher --");

                List<TeacherViewModel> teachers = teacherRepository.GetAllTeachers();
                foreach (var teacher in teachers)
                {
                    cmbHeadOfDepartment.Items.Add($"{teacher.FullName} ({teacher.Designation ?? "N/A"})");
                    cmbHeadOfDepartment.Tag = cmbHeadOfDepartment.Tag == null ? teacher.Id : cmbHeadOfDepartment.Tag + "|" + teacher.Id;
                }

                cmbHeadOfDepartment.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading teachers: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDepartments()
        {
            loadingIndicator.Show("Loading Departments");

            try
            {
                List<DepartmentModel> departments = departmentRepository.GetAllDepartments();
                DataTable dt = ConvertToDataTable(departments);

                dgvDepartments.DataSource = dt;
                ConfigureDataGridViewColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading departments: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                loadingIndicator.Hide();
            }
        }

        private DataTable ConvertToDataTable(List<DepartmentModel> departments)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Head ID", typeof(string));
            dt.Columns.Add("Status", typeof(string));

            foreach (DepartmentModel dept in departments)
            {
                dt.Rows.Add(
                    dept.Id,
                    dept.Name,
                    dept.Code,
                    dept.HeadOfDepartmentId ?? "N/A",
                    dept.IsActive ? "Active" : "Inactive"
                );
            }

            return dt;
        }

        private void ConfigureDataGridViewColumns()
        {
            if (dgvDepartments.Columns["Id"] != null)
                dgvDepartments.Columns["Id"].Visible = false;
            if (dgvDepartments.Columns["Head ID"] != null)
                dgvDepartments.Columns["Head ID"].Visible = false;

            if (dgvDepartments.Columns["Name"] != null)
            {
                dgvDepartments.Columns["Name"].HeaderText = "Department Name";
                dgvDepartments.Columns["Name"].FillWeight = 150;
            }
            if (dgvDepartments.Columns["Code"] != null)
            {
                dgvDepartments.Columns["Code"].HeaderText = "Code";
                dgvDepartments.Columns["Code"].FillWeight = 70;
            }
            if (dgvDepartments.Columns["Status"] != null)
            {
                dgvDepartments.Columns["Status"].FillWeight = 70;
            }
        }

        private void DgvDepartments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvDepartments.Rows[e.RowIndex];
                
                selectedDepartmentId = row.Cells["Id"].Value?.ToString();
                txtDepartmentName.Text = row.Cells["Name"].Value?.ToString();
                txtDepartmentCode.Text = row.Cells["Code"].Value?.ToString();
                chkIsActive.Checked = row.Cells["Status"].Value?.ToString() == "Active";

               
                btnAdd.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            loadingIndicator.Show("Adding Department");

            try
            {
                DepartmentModel department = new DepartmentModel
                {
                    Name = txtDepartmentName.Text.Trim(),
                    Code = txtDepartmentCode.Text.Trim().ToUpper(),
                    HeadOfDepartmentId = GetSelectedTeacherId(),
                    IsActive = chkIsActive.Checked
                };

                string departmentId = departmentRepository.AddDepartment(department);

                if (!string.IsNullOrEmpty(departmentId))
                {
                    MessageBox.Show("Department added successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    ClearForm();
                    LoadDepartments();
                }
                else
                {
                    MessageBox.Show("Failed to add department.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding department: {ex.Message}",
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

            loadingIndicator.Show("Updating Department");

            try
            {
                DepartmentModel department = new DepartmentModel
                {
                    Id = selectedDepartmentId,
                    Name = txtDepartmentName.Text.Trim(),
                    Code = txtDepartmentCode.Text.Trim().ToUpper(),
                    HeadOfDepartmentId = GetSelectedTeacherId(),
                    IsActive = chkIsActive.Checked
                };

                bool success = departmentRepository.UpdateDepartment(department);

                if (success)
                {
                    MessageBox.Show("Department updated successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    ClearForm();
                    LoadDepartments();
                }
                else
                {
                    MessageBox.Show("Failed to update department.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating department: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                loadingIndicator.Hide();
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to deactivate this department?\n\nIt will be marked as inactive but not permanently deleted.",
                "Confirm Deactivate",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                loadingIndicator.Show("Deactivating Department");

                try
                {
                    bool success = departmentRepository.DeleteDepartment(selectedDepartmentId);

                    if (success)
                    {
                        MessageBox.Show("Department deactivated successfully!",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        ClearForm();
                        LoadDepartments();
                    }
                    else
                    {
                        MessageBox.Show("Failed to deactivate department.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deactivating department: {ex.Message}",
                        "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    loadingIndicator.Hide();
                }
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtDepartmentName.Text))
            {
                MessageBox.Show("Please enter department name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDepartmentName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDepartmentCode.Text))
            {
                MessageBox.Show("Please enter department code.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDepartmentCode.Focus();
                return false;
            }

            return true;
        }

        private string GetSelectedTeacherId()
        {
            if (cmbHeadOfDepartment.SelectedIndex <= 0)
                return null;

            string tagData = cmbHeadOfDepartment.Tag?.ToString();
            if (string.IsNullOrEmpty(tagData))
                return null;

            string[] teacherIds = tagData.Split('|');
            int index = cmbHeadOfDepartment.SelectedIndex - 1;
            
            if (index >= 0 && index < teacherIds.Length)
                return teacherIds[index];

            return null;
        }

        private void ClearForm()
        {
            txtDepartmentName.Clear();
            txtDepartmentCode.Clear();
            cmbHeadOfDepartment.SelectedIndex = 0;
            chkIsActive.Checked = true;

            selectedDepartmentId = "";
            
            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }
    }
}
