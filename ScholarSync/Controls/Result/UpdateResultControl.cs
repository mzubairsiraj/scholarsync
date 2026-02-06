using System;
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
    public partial class UpdateResultControl : UserControl
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Panel searchPanel;
        private Panel resultPanel;
        private Panel buttonPanel;
        
        // Search Controls
        private Label lblSearchCriteria;
        private TextBox txtSearchStudent;
        private Button btnSearchResult;
        private Button btnClearSearch;
        private Label lblSearchStatus;
        
        // Student Information Display
        private Label lblStudentName;
        private Label lblStudentRollNo;
        private Label lblSubjectName;
        private Label lblSubjectCode;
        private Label lblSemester;
        
        // Exam Information
        private ComboBox cmbExamType;
        private TextBox txtTotalMarks;
        private TextBox txtObtainedMarks;
        private TextBox txtRemarks;
        
        // Result Summary
        private Label lblCalculatedGPA;
        private Label lblCalculatedGrade;
        private Label lblCalculatedStatus;
        
        // Buttons
        private Button btnCalculate;
        private Button btnUpdateResult;
        private Button btnDeleteResult;
        private Button btnReset;
        
        // State
        private string currentResultId = null;
        private EnrollmentViewModel currentResultData = null;
        private bool resultLoaded = false;

        // Loading Indicator
        private LoadingIndicator loadingIndicator;

        // Repository
        private EnrollmentRepository enrollmentRepository;

        public UpdateResultControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.BackColor = ConfigurationConstants.SSWhiteColor;
            this.AutoScroll = true;

            enrollmentRepository = new EnrollmentRepository();

            InitializeUI();

            loadingIndicator = new LoadingIndicator();
            this.Controls.Add(loadingIndicator);
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
                Text = "Update Student Result",
                Font = new Font("Arial", 22, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(30, 0),
                Size = new Size(500, 80),
                TextAlign = ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.Add(titleLabel);
            this.Controls.Add(headerPanel);

            // Search Panel
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

            // Result Panel
            resultPanel = new Panel
            {
                Dock = DockStyle.None,
                BackColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(10, 260),
                Size = new Size(ConfigurationConstants.ScreenWidth - 300, ConfigurationConstants.ScreenHeight - 440),
                AutoScroll = true,
                Padding = new Padding(40, 30, 40, 30),
                Enabled = false
            };
            
            InitializeResultPanel();
            this.Controls.Add(resultPanel);

            // Button Panel
            buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 100,
                BackColor = ConfigurationConstants.SSLightGrayBackground,
                Padding = new Padding(40, 25, 40, 25)
            };

            btnUpdateResult = UIHelper.CreateButton("Update Result", ConfigurationConstants.SSDarkNavyColor, new Point(40, 25));
            btnUpdateResult.Click += BtnUpdateResult_Click;
            btnUpdateResult.Enabled = false;
            btnUpdateResult.ForeColor = ConfigurationConstants.SSWhiteColor;

            btnDeleteResult = UIHelper.CreateButton("Delete Result", ConfigurationConstants.SSErrorRed, new Point(260, 25));
            btnDeleteResult.Click += BtnDeleteResult_Click;
            btnDeleteResult.Enabled = false;
            btnDeleteResult.ForeColor = ConfigurationConstants.SSWhiteColor;

            btnReset = UIHelper.CreateButton("Reset Form", ConfigurationConstants.SSDarkBlueColor, new Point(480, 25));
            btnReset.Click += BtnReset_Click;
            btnReset.Enabled = false;
            btnReset.ForeColor = ConfigurationConstants.SSWhiteColor;

            buttonPanel.Controls.Add(btnUpdateResult);
            buttonPanel.Controls.Add(btnDeleteResult);
            buttonPanel.Controls.Add(btnReset);
            this.Controls.Add(buttonPanel);
        }

        private void InitializeSearchControls()
        {
            int yPos = 20;

            lblSearchCriteria = new Label
            {
                Text = "Search Result:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(130, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtSearchStudent = new TextBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(150, yPos),
                Size = new Size(350, 40),
                ForeColor = Color.Gray,
                Text = "Enter Roll Number, Subject Code...",
                Tag = "Enter Roll Number, Subject Code..."
            };

            txtSearchStudent.Enter += TxtSearchStudent_Enter;
            txtSearchStudent.Leave += TxtSearchStudent_Leave;
            txtSearchStudent.KeyPress += TxtSearchStudent_KeyPress;

            btnSearchResult = UIHelper.CreateButton("Search", ConfigurationConstants.SSDarkNavyColor, new Point(520, yPos), new Size(120, 40));
            btnSearchResult.Click += BtnSearchResult_Click;

            btnClearSearch = UIHelper.CreateButton("Clear", ConfigurationConstants.SSDarkBlueColor, new Point(660, yPos), new Size(120, 40));
            btnClearSearch.Click += BtnClearSearch_Click;

            lblSearchStatus = new Label
            {
                Text = "No result loaded",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSErrorRed,
                Location = new Point(800, yPos),
                Size = new Size(300, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            searchPanel.Controls.Add(lblSearchCriteria);
            searchPanel.Controls.Add(txtSearchStudent);
            searchPanel.Controls.Add(btnSearchResult);
            searchPanel.Controls.Add(btnClearSearch);
            searchPanel.Controls.Add(lblSearchStatus);
        }

        private void InitializeResultPanel()
        {
            int yPos = 0;

            // Student & Subject Information Section
            Label lblStudentInfo = new Label
            {
                Text = "Student & Subject Information",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(0, yPos),
                Size = new Size(400, 35),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblStudentInfo);
            yPos += 45;

            lblStudentName = new Label
            {
                Text = "Name: -",
                Font = new Font("Arial", 11, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(400, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblStudentName);

            lblStudentRollNo = new Label
            {
                Text = "Roll No: -",
                Font = new Font("Arial", 11, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(450, yPos),
                Size = new Size(300, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblStudentRollNo);
            yPos += 35;

            lblSubjectName = new Label
            {
                Text = "Subject: -",
                Font = new Font("Arial", 11, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(400, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblSubjectName);

            lblSubjectCode = new Label
            {
                Text = "Code: -",
                Font = new Font("Arial", 11, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(450, yPos),
                Size = new Size(150, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblSubjectCode);

            lblSemester = new Label
            {
                Text = "Semester: -",
                Font = new Font("Arial", 11, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(620, yPos),
                Size = new Size(200, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblSemester);
            yPos += 50;

            // Result Information Section
            Label lblResultInfo = new Label
            {
                Text = "Result Information",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(0, yPos),
                Size = new Size(300, 35),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblResultInfo);
            yPos += 45;

            // Exam Type
            Label lblExamType = new Label
            {
                Text = "Exam Type:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(100, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblExamType);

            cmbExamType = new ComboBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(140, yPos),
                Size = new Size(200, 40),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbExamType.Items.AddRange(new string[] { "Mid Term", "Final Term", "Sessional" });
            resultPanel.Controls.Add(cmbExamType);
            yPos += 50;

            // Total Marks and Obtained Marks
            Label lblTotalMarks = new Label
            {
                Text = "Total Marks:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(100, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblTotalMarks);

            txtTotalMarks = new TextBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(140, yPos),
                Size = new Size(200, 40)
            };
            resultPanel.Controls.Add(txtTotalMarks);

            Label lblObtainedMarks = new Label
            {
                Text = "Obtained:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(370, yPos),
                Size = new Size(100, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblObtainedMarks);

            txtObtainedMarks = new TextBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(470, yPos),
                Size = new Size(200, 40)
            };
            resultPanel.Controls.Add(txtObtainedMarks);

            btnCalculate = UIHelper.CreateButton("Calculate", ConfigurationConstants.SSDarkBlueColor, new Point(690, yPos), new Size(120, 40));
            btnCalculate.Click += BtnCalculate_Click;
            resultPanel.Controls.Add(btnCalculate);
            yPos += 60;

            // Remarks
            Label lblRemarks = new Label
            {
                Text = "Remarks:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(100, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblRemarks);

            txtRemarks = new TextBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(140, yPos),
                Size = new Size(530, 40),
                ForeColor = Color.Gray,
                Text = "Optional remarks...",
                Tag = "Optional remarks..."
            };
            UIHelper.SetupPlaceholderBehavior(txtRemarks, "Optional remarks...");
            resultPanel.Controls.Add(txtRemarks);
            yPos += 60;

            // Calculated Results Section
            Label lblCalculations = new Label
            {
                Text = "Calculated Results",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(0, yPos),
                Size = new Size(300, 35),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblCalculations);
            yPos += 45;

            Panel summaryPanel = new Panel
            {
                Location = new Point(0, yPos),
                Size = new Size(700, 120),
                BackColor = ConfigurationConstants.SSLightGrayBackground,
                Padding = new Padding(20)
            };

            lblCalculatedGPA = CreateSummaryLabel("GPA: -", 20, 20, summaryPanel);
            lblCalculatedGrade = CreateSummaryLabel("Grade: -", 20, 50, summaryPanel);
            lblCalculatedStatus = CreateSummaryLabel("Status: -", 350, 20, summaryPanel);
            lblCalculatedStatus.Font = new Font("Arial", 12, FontStyle.Bold);

            resultPanel.Controls.Add(summaryPanel);
        }

        private Label CreateSummaryLabel(string text, int x, int y, Panel parent)
        {
            Label lbl = new Label
            {
                Text = text,
                Font = new Font("Arial", 11, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(x, y),
                Size = new Size(300, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            parent.Controls.Add(lbl);
            return lbl;
        }

        private void TxtSearchStudent_Enter(object sender, EventArgs e)
        {
            if (txtSearchStudent.Text == txtSearchStudent.Tag.ToString())
            {
                txtSearchStudent.Text = "";
                txtSearchStudent.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            }
        }

        private void TxtSearchStudent_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearchStudent.Text))
            {
                txtSearchStudent.Text = txtSearchStudent.Tag.ToString();
                txtSearchStudent.ForeColor = Color.Gray;
            }
        }

        private void TxtSearchStudent_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSearchResult.PerformClick();
                e.Handled = true;
            }
        }

        private void BtnSearchResult_Click(object sender, EventArgs e)
        {
            if (UIHelper.IsPlaceholderText(txtSearchStudent))
            {
                MessageBox.Show("Please enter search criteria.", "Search Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSearchStudent.Focus();
                return;
            }

            SearchResult(txtSearchStudent.Text.Trim());
        }

        private void SearchResult(string searchTerm)
        {
            loadingIndicator.Show("Searching Result");

            try
            {
                var allResults = enrollmentRepository.GetAllEnrollments();

                var foundResults = allResults.Where(r =>
                    r.StudentRollNumber.ToLower().Contains(searchTerm.ToLower()) ||
                    r.SubjectCode.ToLower().Contains(searchTerm.ToLower()) ||
                    r.SubjectName.ToLower().Contains(searchTerm.ToLower())
                ).ToList();

                if (foundResults.Count == 0)
                {
                    loadingIndicator.Hide();
                    MessageBox.Show("No result found matching the search criteria.",
                        "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (foundResults.Count > 1)
                {
                    loadingIndicator.Hide();
                    using (var selectForm = new ResultSelectionForm(foundResults))
                    {
                        if (selectForm.ShowDialog() == DialogResult.OK)
                        {
                            LoadResult(selectForm.SelectedResult);
                        }
                    }
                }
                else
                {
                    LoadResult(foundResults[0]);
                    loadingIndicator.Hide();
                }
            }
            catch (Exception ex)
            {
                loadingIndicator.Hide();
                MessageBox.Show($"Error searching result: {ex.Message}",
                    "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadResult(EnrollmentViewModel result)
        {
            currentResultId = result.ResultId;
            currentResultData = result;
            resultLoaded = true;

            lblStudentName.Text = $"Name: {result.StudentName}";
            lblStudentRollNo.Text = $"Roll No: {result.StudentRollNumber}";
            lblSubjectName.Text = $"Subject: {result.SubjectName}";
            lblSubjectCode.Text = $"Code: {result.SubjectCode}";
            lblSemester.Text = $"Semester: {result.SemesterName ?? "N/A"}";

            cmbExamType.SelectedItem = result.ExamType;
            txtTotalMarks.Text = result.TotalMarks?.ToString() ?? "100";
            txtObtainedMarks.Text = result.ObtainedMarks?.ToString() ?? "";
            
            if (!string.IsNullOrEmpty(result.Remarks) && result.Remarks != "N/A")
            {
                txtRemarks.Text = result.Remarks;
                txtRemarks.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            }

            CalculateGrade();

            resultPanel.Enabled = true;
            btnUpdateResult.Enabled = true;
            btnDeleteResult.Enabled = true;
            btnReset.Enabled = true;

            lblSearchStatus.Text = $"Result: {result.StudentName} - {result.SubjectCode}";
            lblSearchStatus.ForeColor = ConfigurationConstants.SSSuccessGreen;

            MessageBox.Show("Result loaded successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            CalculateGrade();
        }

        private void CalculateGrade()
        {
            if (!decimal.TryParse(txtTotalMarks.Text, out decimal totalMarks) || totalMarks <= 0)
            {
                MessageBox.Show("Please enter a valid total marks.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtObtainedMarks.Text, out decimal obtainedMarks) || obtainedMarks < 0)
            {
                MessageBox.Show("Please enter valid obtained marks.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (obtainedMarks > totalMarks)
            {
                MessageBox.Show("Obtained marks cannot be greater than total marks.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal percentage = (obtainedMarks / totalMarks) * 100;
            decimal gpa = CalculateGPA(percentage);
            string grade = CalculateGradeLetter(percentage);
            string status = percentage >= 50 ? "PASS" : "FAIL";

            lblCalculatedGPA.Text = $"GPA: {gpa:F2} / 4.0";
            lblCalculatedGrade.Text = $"Grade: {grade}";
            lblCalculatedStatus.Text = $"Status: {status}";
            lblCalculatedStatus.ForeColor = status == "PASS" ?
                ConfigurationConstants.SSSuccessGreen : ConfigurationConstants.SSErrorRed;
        }

        private decimal CalculateGPA(decimal percentage)
        {
            if (percentage >= 85) return 4.0M;
            if (percentage >= 80) return 3.7M;
            if (percentage >= 75) return 3.3M;
            if (percentage >= 70) return 3.0M;
            if (percentage >= 65) return 2.7M;
            if (percentage >= 60) return 2.3M;
            if (percentage >= 50) return 2.0M;
            return 0.0M;
        }

        private string CalculateGradeLetter(decimal percentage)
        {
            if (percentage >= 85) return "A+";
            if (percentage >= 80) return "A";
            if (percentage >= 75) return "B+";
            if (percentage >= 70) return "B";
            if (percentage >= 65) return "C+";
            if (percentage >= 60) return "C";
            if (percentage >= 50) return "D";
            return "F";
        }

        private void BtnUpdateResult_Click(object sender, EventArgs e)
        {
            if (!resultLoaded || currentResultId == null)
            {
                MessageBox.Show("Please search and load a result first.", "No Result Loaded",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ValidateResult())
            {
                DialogResult confirmResult = MessageBox.Show(
                    "Are you sure you want to update this result?",
                    "Confirm Update",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmResult == DialogResult.Yes)
                {
                    UpdateResult();
                }
            }
        }

        private void UpdateResult()
        {
            loadingIndicator.Show("Updating Result");

            try
            {
                decimal totalMarks = decimal.Parse(txtTotalMarks.Text);
                decimal obtainedMarks = decimal.Parse(txtObtainedMarks.Text);
                decimal percentage = (obtainedMarks / totalMarks) * 100;

                ResultModel result = new ResultModel
                {
                    EnrollmentId = currentResultData.Id,
                    ExamType = cmbExamType.SelectedItem?.ToString(),
                    TotalMarks = (int)totalMarks,
                    ObtainedMarks = obtainedMarks,
                    GPA = CalculateGPA(percentage),
                    GradeLetter = CalculateGradeLetter(percentage),
                    Remarks = UIHelper.IsPlaceholderText(txtRemarks) ? null : txtRemarks.Text
                };

                bool success = enrollmentRepository.UpdateResultById(currentResultId, result);

                loadingIndicator.Hide();

                if (success)
                {
                    MessageBox.Show("Result updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to update result. Please try again.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                loadingIndicator.Hide();
                MessageBox.Show($"Error updating result: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDeleteResult_Click(object sender, EventArgs e)
        {
            if (!resultLoaded || currentResultId == null)
            {
                MessageBox.Show("Please search and load a result first.", "No Result Loaded",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirmResult = MessageBox.Show(
                $"Are you sure you want to delete this result?\n\n" +
                $"Student: {currentResultData.StudentName}\n" +
                $"Subject: {currentResultData.SubjectName}\n" +
                $"This action cannot be undone!",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
                DeleteResult();
            }
        }

        private void DeleteResult()
        {
            loadingIndicator.Show("Deleting Result");

            try
            {
                bool success = enrollmentRepository.DeleteResult(currentResultId);

                loadingIndicator.Hide();

                if (success)
                {
                    MessageBox.Show("Result deleted successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to delete result. Please try again.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                loadingIndicator.Hide();
                MessageBox.Show($"Error deleting result: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClearSearch_Click(object sender, EventArgs e)
        {
            UIHelper.ClearToPlaceholder(txtSearchStudent);
            ClearForm();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (!resultLoaded) return;

            DialogResult result = MessageBox.Show(
                "Are you sure you want to reset the form? All changes will be lost.",
                "Reset Form",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                ClearForm();
            }
        }

        private bool ValidateResult()
        {
            if (cmbExamType.SelectedIndex == -1)
            {
                MessageBox.Show("Please select Exam Type.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbExamType.Focus();
                return false;
            }

            if (!decimal.TryParse(txtTotalMarks.Text, out decimal totalMarks) || totalMarks <= 0)
            {
                MessageBox.Show("Please enter a valid total marks.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTotalMarks.Focus();
                return false;
            }

            if (!decimal.TryParse(txtObtainedMarks.Text, out decimal obtainedMarks) || obtainedMarks < 0)
            {
                MessageBox.Show("Please enter valid obtained marks.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtObtainedMarks.Focus();
                return false;
            }

            if (obtainedMarks > totalMarks)
            {
                MessageBox.Show("Obtained marks cannot be greater than total marks.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtObtainedMarks.Focus();
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            resultPanel.Enabled = false;
            btnUpdateResult.Enabled = false;
            btnDeleteResult.Enabled = false;
            btnReset.Enabled = false;

            lblSearchStatus.Text = "No result loaded";
            lblSearchStatus.ForeColor = ConfigurationConstants.SSErrorRed;

            lblStudentName.Text = "Name: -";
            lblStudentRollNo.Text = "Roll No: -";
            lblSubjectName.Text = "Subject: -";
            lblSubjectCode.Text = "Code: -";
            lblSemester.Text = "Semester: -";

            cmbExamType.SelectedIndex = -1;
            txtTotalMarks.Clear();
            txtObtainedMarks.Clear();
            UIHelper.ClearToPlaceholder(txtRemarks);

            lblCalculatedGPA.Text = "GPA: -";
            lblCalculatedGrade.Text = "Grade: -";
            lblCalculatedStatus.Text = "Status: -";
            lblCalculatedStatus.ForeColor = ConfigurationConstants.SSDarkNavyColor;

            resultLoaded = false;
            currentResultId = null;
            currentResultData = null;
        }
    }
}
