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
    public partial class ManageSubjectsControl : UserControl
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Panel formPanel;
        private Panel gridPanel;
        
        // Form Controls
        private Label lblDepartment;
        private ComboBox cmbDepartment;
        private Label lblSubjectName;
        private TextBox txtSubjectName;
        private Label lblSubjectCode;
        private TextBox txtSubjectCode;
        private Label lblCreditHours;
        private TextBox txtCreditHours;
        
        // Buttons
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnClear;
        
        // DataGridView
        private DataGridView dgvSubjects;
        
        // Loading Indicator
        private LoadingIndicator loadingIndicator;
        
        // Repositories
        private SubjectRepository subjectRepository;
        private DepartmentRepository departmentRepository;
        
        // State
        private string selectedSubjectId = "";

        public ManageSubjectsControl()
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
            subjectRepository = new SubjectRepository();
            departmentRepository = new DepartmentRepository();
            
            InitializeUI();
            
            // Initialize loading indicator
            loadingIndicator = new LoadingIndicator();
            this.Controls.Add(loadingIndicator);
            
            LoadDepartments();
            LoadSubjects();
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
                Text = "Only Administrators can manage subjects.\n\nPlease contact your system administrator for access.",
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
                Text = "Manage Subjects",
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
            int labelWidth = 120;
            int controlWidth = 300;
            int controlHeight = 40;
            int xLabelPos = 20;
            int xControlPos = 150;

            // Department
            lblDepartment = new Label
            {
                Text = "Department:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(xLabelPos, yPos),
                Size = new Size(labelWidth, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbDepartment = new ComboBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(xControlPos, yPos),
                Size = new Size(controlWidth, controlHeight),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            formPanel.Controls.Add(lblDepartment);
            formPanel.Controls.Add(cmbDepartment);

            // Credit Hours (next to Department)
            lblCreditHours = new Label
            {  
                Text = "Credit Hours:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(xLabelPos + 480, yPos),
                Size = new Size(labelWidth, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtCreditHours = new TextBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(xControlPos + 480, yPos),
                Size = new Size(100, controlHeight),
                Text = "3"
            };

            formPanel.Controls.Add(lblCreditHours);
            formPanel.Controls.Add(txtCreditHours);

            yPos += 60;

            // Subject Name
            lblSubjectName = new Label
            {
                Text = "Subject Name:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(xLabelPos, yPos),
                Size = new Size(labelWidth, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtSubjectName = new TextBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(xControlPos, yPos),
                Size = new Size(controlWidth, controlHeight)
            };

            formPanel.Controls.Add(lblSubjectName);
            formPanel.Controls.Add(txtSubjectName);

            // Subject Code
            lblSubjectCode = new Label
            {
                Text = "Subject Code:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(xLabelPos + 480, yPos),
                Size = new Size(labelWidth, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtSubjectCode = new TextBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(xControlPos + 480, yPos),
                Size = new Size(200, controlHeight)
            };

            formPanel.Controls.Add(lblSubjectCode);
            formPanel.Controls.Add(txtSubjectCode);

            yPos += 60;

            // Buttons
            btnAdd = UIHelper.CreateButton("Add Subject", ConfigurationConstants.SSSuccessGreen, new Point(xLabelPos, yPos), new Size(150, 45));
            btnAdd.Click += BtnAdd_Click;

            btnUpdate = UIHelper.CreateButton("Update Subject", ConfigurationConstants.SSDarkNavyColor, new Point(xLabelPos + 170, yPos), new Size(150, 45));
            btnUpdate.Click += BtnUpdate_Click;
            btnUpdate.Enabled = false;

            btnDelete = UIHelper.CreateButton("Delete Subject", ConfigurationConstants.SSErrorRed, new Point(xLabelPos + 340, yPos), new Size(150, 45));
            btnDelete.Click += BtnDelete_Click;
            btnDelete.Enabled = false;

            btnClear = UIHelper.CreateButton("Clear Form", ConfigurationConstants.SSDarkBlueColor, new Point(xLabelPos + 510, yPos), new Size(130, 45));
            btnClear.Click += BtnClear_Click;

            formPanel.Controls.Add(btnAdd);
            formPanel.Controls.Add(btnUpdate);
            formPanel.Controls.Add(btnDelete);
            formPanel.Controls.Add(btnClear);
        }

        private void InitializeDataGridView()
        {
            dgvSubjects = new DataGridView
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

            dgvSubjects.ColumnHeadersDefaultCellStyle.BackColor = ConfigurationConstants.SSDarkNavyColor;
            dgvSubjects.ColumnHeadersDefaultCellStyle.ForeColor = ConfigurationConstants.SSWhiteColor;
            dgvSubjects.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 11, FontStyle.Bold);
            dgvSubjects.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvSubjects.ColumnHeadersHeight = 45;

            dgvSubjects.DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Regular);
            dgvSubjects.DefaultCellStyle.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            dgvSubjects.DefaultCellStyle.SelectionBackColor = ConfigurationConstants.SSLightNavyColor;
            dgvSubjects.DefaultCellStyle.SelectionForeColor = ConfigurationConstants.SSWhiteColor;
            dgvSubjects.DefaultCellStyle.Padding = new Padding(5);

            dgvSubjects.AlternatingRowsDefaultCellStyle.BackColor = ConfigurationConstants.SSLightGrayBackground;
            dgvSubjects.GridColor = ConfigurationConstants.SSBorderGray;

            dgvSubjects.CellClick += DgvSubjects_CellClick;

            gridPanel.Controls.Add(dgvSubjects);
        }

        private void LoadDepartments()
        {
            try
            {
                cmbDepartment.Items.Clear();
                cmbDepartment.Items.Add("-- Select Department --");

                List<DepartmentModel> departments = departmentRepository.GetActiveDepartments();
                foreach (var dept in departments)
                {
                    cmbDepartment.Items.Add($"{dept.Name} ({dept.Code})");
                    cmbDepartment.Tag = cmbDepartment.Tag == null ? dept.Id : cmbDepartment.Tag + "|" + dept.Id;
                }

                cmbDepartment.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading departments: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSubjects()
        {
            loadingIndicator.Show("Loading Subjects");

            try
            {
                List<SubjectModel> subjects = subjectRepository.GetAllSubjects();
                
                // Get department names
                List<DepartmentModel> departments = departmentRepository.GetAllDepartments();
                
                DataTable dt = ConvertToDataTable(subjects, departments);

                dgvSubjects.DataSource = dt;
                ConfigureDataGridViewColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading subjects: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                loadingIndicator.Hide();
            }
        }

        private DataTable ConvertToDataTable(List<SubjectModel> subjects, List<DepartmentModel> departments)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Department ID", typeof(string));
            dt.Columns.Add("Department", typeof(string));
            dt.Columns.Add("Subject Code", typeof(string));
            dt.Columns.Add("Subject Name", typeof(string));
            dt.Columns.Add("Credit Hours", typeof(int));

            foreach (SubjectModel subject in subjects)
            {
                var dept = departments.Find(d => d.Id == subject.DeptId);
                
                dt.Rows.Add(
                    subject.Id,
                    subject.DeptId,
                    dept != null ? $"{dept.Name} ({dept.Code})" : "N/A",
                    subject.Code,
                    subject.Name,
                    subject.CreditHours
                );
            }

            return dt;
        }

        private void ConfigureDataGridViewColumns()
        {
            if (dgvSubjects.Columns["Id"] != null)
                dgvSubjects.Columns["Id"].Visible = false;
            if (dgvSubjects.Columns["Department ID"] != null)
                dgvSubjects.Columns["Department ID"].Visible = false;

            if (dgvSubjects.Columns["Department"] != null)
                dgvSubjects.Columns["Department"].FillWeight = 130;
            if (dgvSubjects.Columns["Subject Code"] != null)
                dgvSubjects.Columns["Subject Code"].FillWeight = 80;
            if (dgvSubjects.Columns["Subject Name"] != null)
                dgvSubjects.Columns["Subject Name"].FillWeight = 150;
            if (dgvSubjects.Columns["Credit Hours"] != null)
                dgvSubjects.Columns["Credit Hours"].FillWeight = 60;
        }

        private void DgvSubjects_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSubjects.Rows[e.RowIndex];
                
                selectedSubjectId = row.Cells["Id"].Value?.ToString();
                string deptId = row.Cells["Department ID"].Value?.ToString();
                txtSubjectName.Text = row.Cells["Subject Name"].Value?.ToString();
                txtSubjectCode.Text = row.Cells["Subject Code"].Value?.ToString();
                txtCreditHours.Text = row.Cells["Credit Hours"].Value?.ToString() ?? "3";

                // Set department
                SetDepartmentComboBox(deptId);

                btnAdd.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
            }
        }

        private void SetDepartmentComboBox(string deptId)
        {
            string tagData = cmbDepartment.Tag?.ToString();
            if (string.IsNullOrEmpty(tagData) || string.IsNullOrEmpty(deptId))
            {
                cmbDepartment.SelectedIndex = 0;
                return;
            }

            string[] deptIds = tagData.Split('|');
            for (int i = 0; i < deptIds.Length; i++)
            {
                if (deptIds[i] == deptId)
                {
                    cmbDepartment.SelectedIndex = i + 1;
                    return;
                }
            }

            cmbDepartment.SelectedIndex = 0;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            loadingIndicator.Show("Adding Subject");

            try
            {
                SubjectModel subject = new SubjectModel
                {
                    DeptId = GetSelectedDepartmentId(),
                    Name = txtSubjectName.Text.Trim(),
                    Code = txtSubjectCode.Text.Trim().ToUpper(),
                    CreditHours = int.TryParse(txtCreditHours.Text, out int credits) ? credits : 3
                };

                string subjectId = subjectRepository.AddSubject(subject);

                if (!string.IsNullOrEmpty(subjectId))
                {
                    MessageBox.Show("Subject added successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    ClearForm();
                    LoadSubjects();
                }
                else
                {
                    MessageBox.Show("Failed to add subject.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding subject: {ex.Message}",
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

            loadingIndicator.Show("Updating Subject");

            try
            {
                SubjectModel subject = new SubjectModel
                {
                    Id = selectedSubjectId,
                    DeptId = GetSelectedDepartmentId(),
                    Name = txtSubjectName.Text.Trim(),
                    Code = txtSubjectCode.Text.Trim().ToUpper(),
                    CreditHours = int.TryParse(txtCreditHours.Text, out int credits) ? credits : 3
                };

                bool success = subjectRepository.UpdateSubject(subject);

                if (success)
                {
                    MessageBox.Show("Subject updated successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    ClearForm();
                    LoadSubjects();
                }
                else
                {
                    MessageBox.Show("Failed to update subject.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating subject: {ex.Message}",
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
                "Are you sure you want to delete this subject?\n\nThis action cannot be undone and may affect related records.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                loadingIndicator.Show("Deleting Subject");

                try
                {
                    bool success = subjectRepository.DeleteSubject(selectedSubjectId);

                    if (success)
                    {
                        MessageBox.Show("Subject deleted successfully!",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        ClearForm();
                        LoadSubjects();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete subject.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting subject: {ex.Message}\n\nThis subject may be in use and cannot be deleted.",
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
            if (cmbDepartment.SelectedIndex <= 0)
            {
                MessageBox.Show("Please select a department.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbDepartment.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSubjectName.Text))
            {
                MessageBox.Show("Please enter subject name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSubjectName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSubjectCode.Text))
            {
                MessageBox.Show("Please enter subject code.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSubjectCode.Focus();
                return false;
            }

            if (!int.TryParse(txtCreditHours.Text, out int credits) || credits < 1 || credits > 6)
            {
                MessageBox.Show("Please enter valid credit hours (1-6).", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCreditHours.Focus();
                return false;
            }

            return true;
        }

        private string GetSelectedDepartmentId()
        {
            if (cmbDepartment.SelectedIndex <= 0)
                return null;

            string tagData = cmbDepartment.Tag?.ToString();
            if (string.IsNullOrEmpty(tagData))
                return null;

            string[] deptIds = tagData.Split('|');
            int index = cmbDepartment.SelectedIndex - 1;
            
            if (index >= 0 && index < deptIds.Length)
                return deptIds[index];

            return null;
        }

        private void ClearForm()
        {
            txtSubjectName.Clear();
            txtSubjectCode.Clear();
            txtCreditHours.Text = "3";
            cmbDepartment.SelectedIndex = 0;

            selectedSubjectId = "";
            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }
    }
}
