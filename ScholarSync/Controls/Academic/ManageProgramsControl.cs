using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ScholarSync.Commons;
using ScholarSync.Models;
using ScholarSync.Repositories;

namespace ScholarSync.Controls.Academic
{
    public partial class ManageProgramsControl : UserControl
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Panel formPanel;
        private Panel gridPanel;
        
        // Form Controls
        private Label lblProgramName;
        private TextBox txtProgramName;
        private Label lblDepartment;
        private ComboBox cmbDepartment;
        private Label lblDurationYears;
        private ComboBox cmbDurationYears;
        
        // Buttons
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnClear;
        
        // DataGridView
        private DataGridView dgvPrograms;
        
        // Repositories
        private ProgramRepository programRepository;
        private DepartmentRepository departmentRepository;
        
        // State
        private string selectedProgramId = "";
        
        // Loading Indicator
        private LoadingIndicator loadingIndicator;
        
        public ManageProgramsControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.BackColor = ConfigurationConstants.SSWhiteColor;
            
            // Check access rights
            if (!SessionManager.IsAdmin())
            {
                ShowAccessDenied();
                return;
            }
            
            programRepository = new ProgramRepository();
            departmentRepository = new DepartmentRepository();
            
            // Initialize loading indicator first
            loadingIndicator = new LoadingIndicator();
            this.Controls.Add(loadingIndicator);
            loadingIndicator.BringToFront();
            
            InitializeUI();
            LoadDepartments();
            LoadPrograms();
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
                Text = "⛔ Access Denied",
                Font = new Font("Arial", 32, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSErrorRed,
                Location = new Point(0, 200),
                Size = new Size(this.Width, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblMessage = new Label
            {
                Text = "Only Administrators can manage programs.\n\nPlease contact your system administrator for access.",
                Font = new Font("Arial", 14, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(0, 280),
                Size = new Size(this.Width, 80),
                TextAlign = ContentAlignment.TopCenter
            };

            accessDeniedPanel.Controls.Add(lblAccessDenied);
            accessDeniedPanel.Controls.Add(lblMessage);
            this.Controls.Add(accessDeniedPanel);
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
                Text = "Manage Programs",
                Font = new Font("Arial", 22, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(30, 0),
                Size = new Size(400, 80),
                TextAlign = ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.Add(titleLabel);
            this.Controls.Add(headerPanel);

            // Form Panel - Full Width
            formPanel = new Panel
            {
                Location = new Point(20, 100),
                Size = new Size(ConfigurationConstants.ScreenWidth - 40, 280),
                BackColor = ConfigurationConstants.SSLightGrayBackground,
                BorderStyle = BorderStyle.FixedSingle
            };

            InitializeFormControls();
            this.Controls.Add(formPanel);

            // Grid Panel - Below Form Panel, Full Width
            gridPanel = new Panel
            {
                Location = new Point(20, 400),
                Size = new Size(ConfigurationConstants.ScreenWidth - 40, ConfigurationConstants.ScreenHeight - 480),
                BackColor = ConfigurationConstants.SSWhiteColor,
                BorderStyle = BorderStyle.FixedSingle
            };

            InitializeDataGrid();
            this.Controls.Add(gridPanel);
        }

        private void InitializeFormControls()
        {
            int xLabelPos = 20;
            int xControlPos = 180;
            int yPos = 20;
            int verticalSpacing = 60;
            int controlWidth = formPanel.Width - xControlPos - 40;

            // Program Name
            lblProgramName = new Label
            {
                Text = "Program Name:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                Location = new Point(xLabelPos, yPos),
                Size = new Size(150, 30),
                ForeColor = ConfigurationConstants.SSDarkBlueColor
            };

            txtProgramName = new TextBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(xControlPos, yPos),
                Size = new Size(controlWidth, 40)
            };

            formPanel.Controls.Add(lblProgramName);
            formPanel.Controls.Add(txtProgramName);
            yPos += verticalSpacing;

            // Department
            lblDepartment = new Label
            {
                Text = "Department:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                Location = new Point(xLabelPos, yPos),
                Size = new Size(150, 30),
                ForeColor = ConfigurationConstants.SSDarkBlueColor
            };

            cmbDepartment = new ComboBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(xControlPos, yPos),
                Size = new Size(controlWidth / 2, 40),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            formPanel.Controls.Add(lblDepartment);
            formPanel.Controls.Add(cmbDepartment);

            // Duration Years - Same row as Department
            int durationLabelX = xControlPos + (controlWidth / 2) + 40;
            int durationControlX = durationLabelX + 140;

            lblDurationYears = new Label
            {
                Text = "Duration (Years):",
                Font = new Font("Arial", 11, FontStyle.Bold),
                Location = new Point(durationLabelX, yPos),
                Size = new Size(130, 30),
                ForeColor = ConfigurationConstants.SSDarkBlueColor
            };

            cmbDurationYears = new ComboBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(durationControlX, yPos),
                Size = new Size(150, 40),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbDurationYears.Items.AddRange(new object[] { 1, 2, 3, 4, 5, 6 });
            cmbDurationYears.SelectedIndex = 3; // Default to 4 years

            formPanel.Controls.Add(lblDurationYears);
            formPanel.Controls.Add(cmbDurationYears);
            yPos += verticalSpacing;

            // Buttons
            btnAdd = UIHelper.CreateButton("Add Program", ConfigurationConstants.SSDarkNavyColor, 
                new Point(xLabelPos, yPos), new Size(150, 45));
            btnAdd.Click += BtnAdd_Click;

            btnUpdate = UIHelper.CreateButton("Update Program", ConfigurationConstants.SSDarkBlueColor, 
                new Point(xLabelPos + 170, yPos), new Size(150, 45));
            btnUpdate.Click += BtnUpdate_Click;
            btnUpdate.Enabled = false;

            btnDelete = UIHelper.CreateButton("Delete Program", ConfigurationConstants.SSErrorRed, 
                new Point(xLabelPos + 340, yPos), new Size(150, 45));
            btnDelete.Click += BtnDelete_Click;
            btnDelete.Enabled = false;

            btnClear = UIHelper.CreateButton("Clear Form", ConfigurationConstants.SSLightNavyColor, 
                new Point(xLabelPos + 510, yPos), new Size(150, 45));
            btnClear.Click += BtnClear_Click;

            formPanel.Controls.Add(btnAdd);
            formPanel.Controls.Add(btnUpdate);
            formPanel.Controls.Add(btnDelete);
            formPanel.Controls.Add(btnClear);
        }

        private void InitializeDataGrid()
        {
            dgvPrograms = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(gridPanel.Width - 20, gridPanel.Height - 20),
                BackgroundColor = ConfigurationConstants.SSWhiteColor,
                BorderStyle = BorderStyle.None,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AllowUserToResizeRows = false,
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 35 }
            };

            // Define columns
            dgvPrograms.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "ID",
                DataPropertyName = "Id",
                Visible = false
            });

            dgvPrograms.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "Program Name",
                DataPropertyName = "Name",
                Width = 350,
                DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Arial", 10, FontStyle.Regular) }
            });

            dgvPrograms.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DepartmentName",
                HeaderText = "Department",
                DataPropertyName = "DepartmentName",
                Width = 250,
                DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Arial", 10, FontStyle.Regular) }
            });

            dgvPrograms.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DurationYears",
                HeaderText = "Duration (Years)",
                DataPropertyName = "DurationYears",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { 
                    Font = new Font("Arial", 10, FontStyle.Regular),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            // Style headers
            dgvPrograms.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = ConfigurationConstants.SSDarkNavyColor,
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            dgvPrograms.EnableHeadersVisualStyles = false;
            dgvPrograms.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F0F0F0");
            dgvPrograms.DefaultCellStyle.SelectionBackColor = ConfigurationConstants.SSDarkBlueColor;
            dgvPrograms.DefaultCellStyle.SelectionForeColor = ConfigurationConstants.SSWhiteColor;

            dgvPrograms.CellClick += DgvPrograms_CellClick;

            gridPanel.Controls.Add(dgvPrograms);
        }

        private void LoadDepartments()
        {
            try
            {
                List<DepartmentModel> departments = departmentRepository.GetAllDepartments();
                
                cmbDepartment.Items.Clear();
                cmbDepartment.Items.Add("-- Select Department --");
                
                foreach (var dept in departments.Where(d => d.IsActive))
                {
                    cmbDepartment.Items.Add(new { Text = dept.Name, Value = dept.Id });
                }
                
                cmbDepartment.DisplayMember = "Text";
                cmbDepartment.ValueMember = "Value";
                cmbDepartment.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading departments: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPrograms()
        {
            loadingIndicator.Show("Loading Programs");

            try
            {
                List<ProgramModel> programs = programRepository.GetAllPrograms();
                dgvPrograms.DataSource = programs;
                
                loadingIndicator.Hide();
            }
            catch (Exception ex)
            {
                loadingIndicator.Hide();
                MessageBox.Show($"Error loading programs: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            loadingIndicator.Show("Adding Program");

            try
            {
                var selectedDept = cmbDepartment.SelectedItem;
                var deptIdProperty = selectedDept.GetType().GetProperty("Value");
                string deptId = deptIdProperty?.GetValue(selectedDept, null)?.ToString();
                
                ProgramModel program = new ProgramModel
                {
                    Name = txtProgramName.Text.Trim(),
                    DeptId = deptId,
                    DurationYears = (int)cmbDurationYears.SelectedItem
                };

                string programId = programRepository.AddProgram(program);

                if (!string.IsNullOrEmpty(programId))
                {
                    MessageBox.Show("Program added successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    ClearForm();
                    LoadPrograms();
                }
                else
                {
                    MessageBox.Show("Failed to add program.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding program: {ex.Message}",
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

            loadingIndicator.Show("Updating Program");

            try
            {
                var selectedDept = cmbDepartment.SelectedItem;
                var deptIdProperty = selectedDept.GetType().GetProperty("Value");
                string deptId = deptIdProperty?.GetValue(selectedDept, null)?.ToString();
                
                ProgramModel program = new ProgramModel
                {
                    Id = selectedProgramId,
                    Name = txtProgramName.Text.Trim(),
                    DeptId = deptId,
                    DurationYears = (int)cmbDurationYears.SelectedItem
                };

                bool success = programRepository.UpdateProgram(program);

                if (success)
                {
                    MessageBox.Show("Program updated successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    ClearForm();
                    LoadPrograms();
                }
                else
                {
                    MessageBox.Show("Failed to update program.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating program: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                loadingIndicator.Hide();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this program?\n\nThis action cannot be undone and may affect related records (students enrolled in this program).",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                loadingIndicator.Show("Deleting Program");

                try
                {
                    bool success = programRepository.DeleteProgram(selectedProgramId);

                    if (success)
                    {
                        MessageBox.Show("Program deleted successfully!",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        ClearForm();
                        LoadPrograms();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete program.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting program: {ex.Message}\n\nThis program may have students enrolled and cannot be deleted.",
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

        private void DgvPrograms_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvPrograms.Rows[e.RowIndex];
                
                selectedProgramId = row.Cells["Id"].Value?.ToString() ?? "";
                txtProgramName.Text = row.Cells["Name"].Value?.ToString() ?? "";
                
                // Select department
                if (row.Cells["DepartmentName"].Value != null)
                {
                    string deptName = row.Cells["DepartmentName"].Value.ToString();
                    for (int i = 1; i < cmbDepartment.Items.Count; i++) // Start from 1 to skip "-- Select Department --"
                    {
                        var item = cmbDepartment.Items[i];
                        var itemType = item.GetType();
                        var textProperty = itemType.GetProperty("Text");
                        
                        if (textProperty != null)
                        {
                            string itemText = textProperty.GetValue(item, null)?.ToString();
                            if (itemText == deptName)
                            {
                                cmbDepartment.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
                
                // Select duration
                if (row.Cells["DurationYears"].Value != null)
                {
                    int duration = Convert.ToInt32(row.Cells["DurationYears"].Value);
                    cmbDurationYears.SelectedItem = duration;
                }

                btnAdd.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtProgramName.Text))
            {
                MessageBox.Show("Please enter program name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProgramName.Focus();
                return false;
            }

            if (cmbDepartment.SelectedIndex <= 0)
            {
                MessageBox.Show("Please select a department.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbDepartment.Focus();
                return false;
            }

            if (cmbDurationYears.SelectedIndex < 0)
            {
                MessageBox.Show("Please select duration in years.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbDurationYears.Focus();
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            txtProgramName.Clear();
            cmbDepartment.SelectedIndex = 0;
            cmbDurationYears.SelectedIndex = 3; // Default to 4 years
            
            selectedProgramId = "";
            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            
            txtProgramName.Focus();
        }
    }
}
