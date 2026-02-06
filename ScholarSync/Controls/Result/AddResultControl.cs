using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ScholarSync.Commons;
using ScholarSync.Models;
using ScholarSync.Repositories;

namespace ScholarSync.Controls.Result
{
    public partial class AddResultControl : UserControl
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Panel searchPanel;
        private Panel resultPanel;
        private Panel buttonPanel;
        
        // Search Controls
        private Label lblSearchStudent;
        private TextBox txtSearchStudent;
        private Button btnSearchStudent;
        private Button btnClearSearch;
        private Label lblSearchHint;
        
        // Student Information Display
        private Label lblCurrentStudent;
        private Label lblStudentName;
        private Label lblStudentRollNo;
        private Label lblStudentProgram;
        
        // Semester Selection
        private Label lblSelectSemester;
        private ComboBox cmbSemesters;
        private Label lblSemesterHint;
        
        // Exam Type Selection
        private Label lblSelectExamType;
        private ComboBox cmbExamType;
        private Label lblExamTypeHint;
        
        // Subject Selection
        private Label lblSelectSubject;
        private ComboBox cmbSubjects;
        private Label lblSubjectHint;
        
        // Result Input Fields
        private Label lblTotalMarks;
        private TextBox txtTotalMarks;
        private Label lblObtainedMarks;
        private TextBox txtObtainedMarks;
        private Label lblRemarks;
        private TextBox txtRemarks;
        
        // Calculated Fields Display
        private Label lblPercentageDisplay;
        private Label lblGPADisplay;
        private Label lblGradeDisplay;
        private Label lblStatusDisplay;
        
        // Buttons
        private Button btnSaveResult;
        private Button btnReset;
       // private Button btnCalculate;
        
        // State
        private string currentStudentId = "";
        private bool studentLoaded = false;
        private List<SubjectModel> availableSubjects;
        private string selectedSubjectId = "";
        private string selectedSemesterId = "";
        private List<SemesterModel> availableSemesters;

        // Loading Indicator
        private LoadingIndicator loadingIndicator;

        // Repositories
        private StudentRepository studentRepository;
        private EnrollmentRepository enrollmentRepository;
        private SubjectRepository subjectRepository;
        private SemesterRepository semesterRepository;

        public AddResultControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.BackColor = ConfigurationConstants.SSWhiteColor;
            this.AutoScroll = true;
            
            // Check if user is Admin or Teacher
            if (!SessionManager.IsAdmin() && !SessionManager.IsTeacher())
            {
                ShowAccessDenied();
                return;
            }
            
            // Initialize repositories
            studentRepository = new StudentRepository();
            enrollmentRepository = new EnrollmentRepository();
            subjectRepository = new SubjectRepository();
            semesterRepository = new SemesterRepository();
            
            InitializeUI();
            
            // Initialize loading indicator
            loadingIndicator = new LoadingIndicator();
            this.Controls.Add(loadingIndicator);
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
                Text = "Only Administrators and Teachers can add results.\n\nPlease contact your system administrator for access.",
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
                Text = SessionManager.IsAdmin() ? "Add Student Result (Administrator)" : "Add Student Result (Teacher)",
                Font = new Font("Arial", 22, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(30, 0),
                Size = new Size(700, 80),
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
                Size = new Size(ConfigurationConstants.ScreenWidth - 300, 120),
                Padding = new Padding(20)
            };
            
            InitializeSearchControls();
            this.Controls.Add(searchPanel);

            // Result Panel
            resultPanel = new Panel
            {
                Dock = DockStyle.None,
                BackColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(10, 240),
                Size = new Size(ConfigurationConstants.ScreenWidth - 300, ConfigurationConstants.ScreenHeight - 420),
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

            btnSaveResult = UIHelper.CreateButton("Save Result", ConfigurationConstants.SSDarkNavyColor, new Point(40, 25));
            btnSaveResult.Click += BtnSaveResult_Click;
            btnSaveResult.Enabled = false;
            btnSaveResult.ForeColor = ConfigurationConstants.SSWhiteColor;

            btnReset = UIHelper.CreateButton("Reset Form", ConfigurationConstants.SSDarkBlueColor, new Point(260, 25));
            btnReset.Click += BtnReset_Click;
            btnReset.Enabled = false;
            btnReset.ForeColor = ConfigurationConstants.SSWhiteColor;

            buttonPanel.Controls.Add(btnSaveResult);
            buttonPanel.Controls.Add(btnReset);
            this.Controls.Add(buttonPanel);
        }

        private void InitializeSearchControls()
        {
            int yPos = 20;

            lblSearchStudent = new Label
            {
                Text = "Search Student:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(120, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtSearchStudent = new TextBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(150, yPos),
                Size = new Size(300, 40),
                ForeColor = Color.Gray,
                Text = "Enter Roll Number or CNIC...",
                Tag = "Enter Roll Number or CNIC..."
            };

            txtSearchStudent.Enter += TxtSearchStudent_Enter;
            txtSearchStudent.Leave += TxtSearchStudent_Leave;
            txtSearchStudent.KeyPress += TxtSearchStudent_KeyPress;

            btnSearchStudent = UIHelper.CreateButton("Search", ConfigurationConstants.SSDarkNavyColor, new Point(470, yPos), new Size(120, 40));
            btnSearchStudent.Click += BtnSearchStudent_Click;

            btnClearSearch = UIHelper.CreateButton("Clear", ConfigurationConstants.SSDarkBlueColor, new Point(610, yPos), new Size(120, 40));
            btnClearSearch.Click += BtnClearSearch_Click;

            lblSearchHint = new Label
            {
                Text = "Search by Roll Number or CNIC to load student information",
                Font = new Font("Arial", 9, FontStyle.Italic),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(150, yPos + 45),
                Size = new Size(400, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblCurrentStudent = new Label
            {
                Text = "No student loaded",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSErrorRed,
                Location = new Point(750, yPos),
                Size = new Size(300, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            searchPanel.Controls.Add(lblSearchStudent);
            searchPanel.Controls.Add(txtSearchStudent);
            searchPanel.Controls.Add(btnSearchStudent);
            searchPanel.Controls.Add(btnClearSearch);
            searchPanel.Controls.Add(lblSearchHint);
            searchPanel.Controls.Add(lblCurrentStudent);
        }

        private void InitializeResultPanel()
        {
            int yPos = 0;

            // Student Information Section
            Label lblStudentInfo = new Label
            {
                Text = "Student Information",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(0, yPos),
                Size = new Size(300, 35),
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

            lblStudentProgram = new Label
            {
                Text = "Program: -",
                Font = new Font("Arial", 11, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(400, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblStudentProgram);
            yPos += 60;

            // Semester Selection Section
            Label lblSemesterSection = new Label
            {
                Text = "Select Semester",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(0, yPos),
                Size = new Size(300, 35),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblSemesterSection);
            yPos += 45;

            lblSelectSemester = new Label
            {
                Text = "Semester:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(100, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblSelectSemester);

            cmbSemesters = new ComboBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(130, yPos),
                Size = new Size(400, 40),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbSemesters.SelectedIndexChanged += CmbSemesters_SelectedIndexChanged;
            resultPanel.Controls.Add(cmbSemesters);

            lblSemesterHint = new Label
            {
                Text = "Select the semester for which you want to add results",
                Font = new Font("Arial", 9, FontStyle.Italic),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(130, yPos + 45),
                Size = new Size(400, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblSemesterHint);
            yPos += 90;

            // Exam Type Selection Section
            Label lblExamTypeSection = new Label
            {
                Text = "Select Exam Type",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(0, yPos),
                Size = new Size(300, 35),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblExamTypeSection);
            yPos += 45;

            lblSelectExamType = new Label
            {
                Text = "Exam Type:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(100, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblSelectExamType);

            cmbExamType = new ComboBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(130, yPos),
                Size = new Size(400, 40),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbExamType.Items.AddRange(new string[] { 
                "-- Select Exam Type --",
                "Mid Term",
                "Final Term",
                "Sessional",
                "Quiz",
                "Assignment"
            });
            cmbExamType.SelectedIndex = 0;
            cmbExamType.SelectedIndexChanged += CmbExamType_SelectedIndexChanged;
            resultPanel.Controls.Add(cmbExamType);

            lblExamTypeHint = new Label
            {
                Text = "Select the type of examination for this result",
                Font = new Font("Arial", 9, FontStyle.Italic),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(130, yPos + 45),
                Size = new Size(400, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblExamTypeHint);
            yPos += 90;

            // Subject Selection Section
            Label lblSubjectSection = new Label
            {
                Text = "Select Subject",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(0, yPos),
                Size = new Size(300, 35),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblSubjectSection);
            yPos += 45;

            lblSelectSubject = new Label
            {
                Text = "Subject:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(100, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblSelectSubject);

            cmbSubjects = new ComboBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(130, yPos),
                Size = new Size(400, 40),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbSubjects.SelectedIndexChanged += CmbSubjects_SelectedIndexChanged;
            resultPanel.Controls.Add(cmbSubjects);

            lblSubjectHint = new Label
            {
                Text = "Select a subject to enter result details",
                Font = new Font("Arial", 9, FontStyle.Italic),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(130, yPos + 45),
                Size = new Size(400, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblSubjectHint);
            yPos += 90;

            // Result Details Section
            Label lblResultDetails = new Label
            {
                Text = "Result Details",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(0, yPos),
                Size = new Size(300, 35),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblResultDetails);
            yPos += 45;

            // Total Marks
            lblTotalMarks = new Label
            {
                Text = "Total Marks:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(120, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblTotalMarks);

            txtTotalMarks = new TextBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(150, yPos),
                Size = new Size(150, 40),
                Text = "100"
            };
            txtTotalMarks.TextChanged += CalculateResult;
            resultPanel.Controls.Add(txtTotalMarks);

            // Obtained Marks
            lblObtainedMarks = new Label
            {
                Text = "Obtained Marks:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(350, yPos),
                Size = new Size(130, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblObtainedMarks);

            txtObtainedMarks = new TextBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(490, yPos),
                Size = new Size(150, 40),
                Text = "0"
            };
            txtObtainedMarks.TextChanged += CalculateResult;
            resultPanel.Controls.Add(txtObtainedMarks);
            yPos += 60;

            // Remarks
            lblRemarks = new Label
            {
                Text = "Remarks:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(120, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblRemarks);

            txtRemarks = new TextBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(150, yPos),
                Size = new Size(490, 40),
                ForeColor = Color.Gray,
                Text = "Optional remarks...",
                Tag = "Optional remarks..."
            };
            txtRemarks.Enter += (s, e) =>
            {
                if (txtRemarks.Text == txtRemarks.Tag.ToString())
                {
                    txtRemarks.Text = "";
                    txtRemarks.ForeColor = ConfigurationConstants.SSDarkBlueColor;
                }
            };
            txtRemarks.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtRemarks.Text))
                {
                    txtRemarks.Text = txtRemarks.Tag.ToString();
                    txtRemarks.ForeColor = Color.Gray;
                }
            };
            resultPanel.Controls.Add(txtRemarks);
            yPos += 80;

            // Calculated Results Section
            Label lblCalculated = new Label
            {
                Text = "Calculated Results",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(0, yPos),
                Size = new Size(300, 35),
                TextAlign = ContentAlignment.MiddleLeft
            };
            resultPanel.Controls.Add(lblCalculated);
            yPos += 45;

            Panel calculatedPanel = new Panel
            {
                Location = new Point(0, yPos),
                Size = new Size(700, 100),
                BackColor = ConfigurationConstants.SSLightGrayBackground,
                Padding = new Padding(20)
            };
            resultPanel.Controls.Add(calculatedPanel);

            lblPercentageDisplay = new Label
            {
                Text = "Percentage: 0.00%",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, 10),
                Size = new Size(300, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            calculatedPanel.Controls.Add(lblPercentageDisplay);

            lblGPADisplay = new Label
            {
                Text = "GPA: 0.00 / 4.0",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(350, 10),
                Size = new Size(300, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            calculatedPanel.Controls.Add(lblGPADisplay);

            lblGradeDisplay = new Label
            {
                Text = "Grade: -",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, 45),
                Size = new Size(300, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            calculatedPanel.Controls.Add(lblGradeDisplay);

            lblStatusDisplay = new Label
            {
                Text = "Status: -",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(350, 45),
                Size = new Size(300, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            calculatedPanel.Controls.Add(lblStatusDisplay);
        }

        private void InitializeSubjectsGrid(int yPos)
        {
            // Removed - no longer using grid
        }

        private Label CreateSummaryLabel(string text, int x, int y, Panel parent)
        {
            // Removed - no longer needed
            return null;
        }

        private void DgvSubjects_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Removed - no longer using grid
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
                btnSearchStudent.PerformClick();
                e.Handled = true;
            }
        }

        private void BtnSearchStudent_Click(object sender, EventArgs e)
        {
            if (UIHelper.IsPlaceholderText(txtSearchStudent))
            {
                MessageBox.Show("Please enter a Roll Number or CNIC to search.", "Search Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSearchStudent.Focus();
                return;
            }

            string searchTerm = txtSearchStudent.Text.Trim();
            LoadStudentData(searchTerm);
        }

        private void LoadStudentData(string searchTerm)
        {
            loadingIndicator.Show("Searching Student");

            try
            {
                // Search for student
                StudentViewModel student = studentRepository.SearchStudent(searchTerm);

                if (student != null)
                {
                    currentStudentId = student.Id;
                    studentLoaded = true;

                    lblStudentName.Text = $"Name: {student.FullName}";
                    lblStudentRollNo.Text = $"Roll No: {student.RollNumber}";
                    lblStudentProgram.Text = $"Program: {student.ProgramName}";

                    resultPanel.Enabled = true;
                    btnSaveResult.Enabled = true;
                    btnReset.Enabled = true;

                    lblCurrentStudent.Text = $"Student: {student.FullName} ({student.RollNumber})";
                    lblCurrentStudent.ForeColor = ConfigurationConstants.SSSuccessGreen;

                    // Load available semesters
                    LoadSemesters();
                    
                    // Load available subjects from database
                    LoadSubjects();

                    loadingIndicator.Hide();
                    MessageBox.Show("Student data loaded successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    loadingIndicator.Hide();
                    MessageBox.Show("No student found with this Roll Number or CNIC.", "Not Found",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                loadingIndicator.Hide();
                MessageBox.Show($"Error loading student: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSemesters()
        {
            try
            {
                // Load all semesters
                availableSemesters = semesterRepository.GetAllSemesters();
                
                cmbSemesters.Items.Clear();
                cmbSemesters.Items.Add("-- Select Semester --");
                
                if (availableSemesters.Count == 0)
                {
                    MessageBox.Show("No semesters found in the system.\n\nPlease contact administrator to add semesters.",
                        "No Semesters", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Populate semesters combo box
                foreach (var semester in availableSemesters)
                {
                    string status = semester.IsCurrent ? " (Current)" : "";
                    string year = semester.StartDate.Year.ToString();
                    cmbSemesters.Items.Add($"{semester.Name} {year}{status}");
                }
                
                // Auto-select current semester if exists
                int currentIndex = availableSemesters.FindIndex(s => s.IsCurrent);
                if (currentIndex >= 0)
                {
                    cmbSemesters.SelectedIndex = currentIndex + 1;
                }
                else
                {
                    cmbSemesters.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading semesters: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbSemesters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSemesters.SelectedIndex > 0)
            {
                selectedSemesterId = availableSemesters[cmbSemesters.SelectedIndex - 1].Id;
                
                // Enable subject selection
                cmbSubjects.Enabled = true;
                
                // Reload subjects for this semester if needed
                if (cmbSubjects.SelectedIndex > 0)
                {
                    CheckExistingResult();
                }
            }
            else
            {
                selectedSemesterId = "";
                cmbSubjects.Enabled = false;
                cmbSubjects.SelectedIndex = 0;
            }
        }

        private void CmbExamType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // When exam type changes, recheck for existing results
            if (cmbSubjects.SelectedIndex > 0 && cmbSemesters.SelectedIndex > 0)
            {
                CheckExistingResult();
            }
        }

        private void LoadSubjects()
        {
            try
            {
                // Load subjects for student's program
                availableSubjects = subjectRepository.GetSubjectsForStudentProgram(currentStudentId);
                
                cmbSubjects.Items.Clear();
                cmbSubjects.Items.Add("-- Select Subject --");
                
                if (availableSubjects.Count == 0)
                {
                    MessageBox.Show("No subjects found for this student's program.",
                        "No Subjects", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                
                // Populate subjects combo box
                foreach (var subject in availableSubjects)
                {
                    cmbSubjects.Items.Add($"{subject.Code} - {subject.Name}");
                }
                
                cmbSubjects.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading subjects: {ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbSubjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSubjects.SelectedIndex > 0)
            {
                selectedSubjectId = availableSubjects[cmbSubjects.SelectedIndex - 1].Id;
                
                // Check if result already exists for this enrollment
                CheckExistingResult();
            }
            else
            {
                selectedSubjectId = "";
                ClearResultFields();
            }
        }

        private void CheckExistingResult()
        {
            try
            {
                // Use selected semester instead of current semester
                if (string.IsNullOrEmpty(selectedSemesterId))
                {
                    return;
                }
                
                // Get exam type if selected
                string examType = cmbExamType.SelectedIndex > 0 ? cmbExamType.SelectedItem.ToString() : null;
                
                // Check if enrollment exists - ALWAYS fetch fresh data from database
                List<EnrollmentViewModel> enrollments = enrollmentRepository.GetEnrollmentsByStudent(
                    lblStudentRollNo.Text.Replace("Roll No: ", ""));
                
                // Find enrollment for this semester and subject
                var existingEnrollment = enrollments.Find(e => 
                    e.SubjectId == selectedSubjectId && 
                    e.SemesterId == selectedSemesterId);
                
                if (existingEnrollment != null && existingEnrollment.ResultId != null)
                {
                    // Load existing result from database
                    txtTotalMarks.Text = existingEnrollment.TotalMarks?.ToString() ?? "100";
                    txtObtainedMarks.Text = existingEnrollment.ObtainedMarks?.ToString("F2") ?? "0";
                    txtRemarks.Text = existingEnrollment.Remarks ?? txtRemarks.Tag.ToString();
                    if (txtRemarks.Text != txtRemarks.Tag.ToString())
                    {
                        txtRemarks.ForeColor = ConfigurationConstants.SSDarkBlueColor;
                    }
                    
                    lblSubjectHint.Text = "?? Result already exists for this subject in selected semester. Updating will overwrite existing data.";
                    lblSubjectHint.ForeColor = ConfigurationConstants.SSWarningOrange;
                }
                else
                {
                    // No existing result - clear all fields to default values
                    ClearResultFields();
                    lblSubjectHint.Text = "Enter result details for the selected subject";
                    lblSubjectHint.ForeColor = ConfigurationConstants.SSDarkBlueColor;
                }
            }
            catch (Exception ex)
            {
                // On error, also clear the fields to avoid showing stale data
                ClearResultFields();
                MessageBox.Show($"Error checking existing result: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearResultFields()
        {
            // Reset to default values
            txtTotalMarks.Text = "100";
            txtObtainedMarks.Text = "0";
            txtRemarks.Text = txtRemarks.Tag.ToString();
            txtRemarks.ForeColor = Color.Gray;
            
            // Clear all calculated displays
            lblPercentageDisplay.Text = "Percentage: 0.00%";
            lblGPADisplay.Text = "GPA: 0.00 / 4.0";
            lblGradeDisplay.Text = "Grade: -";
            lblStatusDisplay.Text = "Status: -";
            lblStatusDisplay.ForeColor = ConfigurationConstants.SSDarkBlueColor;
        }

        private void CalculateResult(object sender, EventArgs e)
        {
            try
            {
                int totalMarks;
                float obtainedMarks;
                
                if (!int.TryParse(txtTotalMarks.Text, out totalMarks) || totalMarks <= 0)
                {
                    ClearCalculatedFields();
                    return;
                }
                
                if (!float.TryParse(txtObtainedMarks.Text, out obtainedMarks) || obtainedMarks < 0)
                {
                    ClearCalculatedFields();
                    return;
                }
                
                // Calculate percentage, GPA and grade using Gomal University scale
                decimal percentage = (decimal)obtainedMarks / totalMarks * 100;
                decimal gpa = GPACalculator.CalculateGPA(percentage);
                string gradeLetter = GPACalculator.CalculateGradeLetter(percentage);
                string status = GPACalculator.IsPassing(percentage) ? "Pass" : "Fail";
                
                // Update display
                lblPercentageDisplay.Text = $"Percentage: {percentage:F2}%";
                lblGPADisplay.Text = $"GPA: {gpa:F2} / 4.0";
                lblGradeDisplay.Text = $"Grade: {gradeLetter}";
                lblStatusDisplay.Text = $"Status: {status}";
                lblStatusDisplay.ForeColor = status == "Pass" ? 
                    ConfigurationConstants.SSSuccessGreen : 
                    ConfigurationConstants.SSErrorRed;
            }
            catch
            {
                ClearCalculatedFields();
            }
        }

        private void ClearCalculatedFields()
        {
            lblPercentageDisplay.Text = "Percentage: 0.00%";
            lblGPADisplay.Text = "GPA: 0.00 / 4.0";
            lblGradeDisplay.Text = "Grade: -";
            lblStatusDisplay.Text = "Status: -";
            lblStatusDisplay.ForeColor = ConfigurationConstants.SSDarkBlueColor;
        }

        private void BtnClearSearch_Click(object sender, EventArgs e)
        {
            UIHelper.ClearToPlaceholder(txtSearchStudent);
            ClearForm();
        }

        private void BtnAddSubject_Click(object sender, EventArgs e)
        {
            // Removed - no longer using add/remove subject buttons
        }

        private void BtnRemoveSubject_Click(object sender, EventArgs e)
        {
            // Removed - no longer using add/remove subject buttons
        }

        private void BtnSaveResult_Click(object sender, EventArgs e)
        {
            if (!studentLoaded)
            {
                MessageBox.Show("Please search and load a student first.", "No Student Loaded", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ValidateResult())
            {
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to save these enrollments/results?",
                    "Confirm Save",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    SaveEnrollments();
                }
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (!studentLoaded) return;

            DialogResult result = MessageBox.Show(
                "Are you sure you want to reset the current result? All entered data will be cleared.",
                "Reset Result",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                cmbSubjects.SelectedIndex = 0;
                ClearResultFields();
            }
        }

        private bool ValidateResult()
        {
            if (cmbSemesters.SelectedIndex <= 0)
            {
                MessageBox.Show("Please select a semester.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbSemesters.Focus();
                return false;
            }

            if (cmbExamType.SelectedIndex <= 0)
            {
                MessageBox.Show("Please select an exam type.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbExamType.Focus();
                return false;
            }

            if (cmbSubjects.SelectedIndex <= 0)
            {
                MessageBox.Show("Please select a subject.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbSubjects.Focus();
                return false;
            }

            int totalMarks;
            if (!int.TryParse(txtTotalMarks.Text, out totalMarks) || totalMarks <= 0)
            {
                MessageBox.Show("Please enter valid total marks (must be greater than 0).", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTotalMarks.Focus();
                return false;
            }

            float obtainedMarks;
            if (!float.TryParse(txtObtainedMarks.Text, out obtainedMarks) || obtainedMarks < 0)
            {
                MessageBox.Show("Please enter valid obtained marks (must be 0 or greater).", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtObtainedMarks.Focus();
                return false;
            }

            if (obtainedMarks > totalMarks)
            {
                MessageBox.Show($"Obtained marks cannot be greater than total marks ({totalMarks}).", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtObtainedMarks.Focus();
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            resultPanel.Enabled = false;
            btnSaveResult.Enabled = false;
            btnReset.Enabled = false;
            
            lblCurrentStudent.Text = "No student loaded";
            lblCurrentStudent.ForeColor = ConfigurationConstants.SSErrorRed;
            
            lblStudentName.Text = "Name: -";
            lblStudentRollNo.Text = "Roll No: -";
            lblStudentProgram.Text = "Program: -";
            
            studentLoaded = false;
            currentStudentId = "";
            selectedSemesterId = "";
            
            cmbSemesters.Items.Clear();
            cmbSemesters.Items.Add("-- Select Semester --");
            cmbSemesters.SelectedIndex = 0;
            
            cmbExamType.Items.Clear();
            cmbExamType.Items.AddRange(new string[] { 
                "-- Select Exam Type --",
                "Mid Term",
                "Final Term",
                "Sessional",
                "Quiz",
                "Assignment"
            });
            cmbExamType.SelectedIndex = 0;
            
            cmbSubjects.Items.Clear();
            cmbSubjects.Items.Add("-- Select Subject --");
            cmbSubjects.SelectedIndex = 0;
            cmbSubjects.Enabled = false;
            
            ClearResultFields();
        }

        private void SaveEnrollments()
        {
            loadingIndicator.Show("Saving Result");

            try
            {
                // Use selected semester instead of current semester
                if (string.IsNullOrEmpty(selectedSemesterId))
                {
                    throw new Exception("Please select a semester first.");
                }
                
                string enrollmentId = "";
                bool isUpdate = false;
                
                // Step 1: Use the more reliable method to get enrollment by exact combination
                EnrollmentViewModel existingEnrollment = enrollmentRepository.GetEnrollmentByStudentSubjectSemester(
                    currentStudentId, selectedSubjectId, selectedSemesterId);
                
                if (existingEnrollment != null)
                {
                    // Use existing enrollment ID - enrollment already exists
                    enrollmentId = existingEnrollment.Id;
                    isUpdate = existingEnrollment.ResultId != null; // Result already exists
                }
                else
                {
                    // Enrollment doesn't exist - create new enrollment
                    try
                    {
                        EnrollmentModel enrollment = new EnrollmentModel
                        {
                            StudentId = currentStudentId,
                            SubjectId = selectedSubjectId,
                            SemesterId = selectedSemesterId
                        };

                        enrollmentId = enrollmentRepository.AddEnrollment(enrollment);
                        
                        if (string.IsNullOrEmpty(enrollmentId))
                        {
                            throw new Exception("Failed to create enrollment");
                        }
                    }
                    catch (Exception enrollEx)
                    {
                        // If enrollment creation failed due to duplicate, try to fetch it again
                        if (enrollEx.Message.Contains("duplicate") || enrollEx.Message.Contains("23505"))
                        {
                            // Use the direct method to get the enrollment that was created
                            existingEnrollment = enrollmentRepository.GetEnrollmentByStudentSubjectSemester(
                                currentStudentId, selectedSubjectId, selectedSemesterId);
                            
                            if (existingEnrollment != null)
                            {
                                enrollmentId = existingEnrollment.Id;
                                isUpdate = existingEnrollment.ResultId != null;
                            }
                            else
                            {
                                throw new Exception("Enrollment already exists but could not be retrieved. Please try again.");
                            }
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                // Step 2: Create/Update result
                int totalMarks = int.Parse(txtTotalMarks.Text);
                float obtainedMarks = float.Parse(txtObtainedMarks.Text);
                
                // Calculate percentage, GPA and grade using Gomal University scale
                decimal percentage = (decimal)obtainedMarks / totalMarks * 100;
                decimal gpa = GPACalculator.CalculateGPA(percentage);
                string gradeLetter = GPACalculator.CalculateGradeLetter(percentage);
                string status = GPACalculator.IsPassing(percentage) ? "Pass" : "Fail";
                string remarks = txtRemarks.Text == txtRemarks.Tag.ToString() ? 
                    status : 
                    txtRemarks.Text;
                
                ResultModel result = new ResultModel
                {
                    EnrollmentId = enrollmentId,
                    ExamType = cmbExamType.SelectedItem.ToString(),
                    TotalMarks = totalMarks,
                    ObtainedMarks = (decimal)obtainedMarks,
                    GPA = gpa,
                    GradeLetter = gradeLetter,
                    Remarks = remarks
                };

                // AddOrUpdateResult will automatically UPDATE if result exists
                string resultId = enrollmentRepository.AddOrUpdateResult(result);
                
                loadingIndicator.Hide();

                if (!string.IsNullOrEmpty(resultId))
                {
                    string action = isUpdate ? "UPDATED" : "SAVED";
                    string semesterName = cmbSemesters.SelectedItem.ToString().Replace("-- Select Semester --", "");
                    string examType = cmbExamType.SelectedItem.ToString();
                    
                    MessageBox.Show(
                        $"Result {action} successfully!\n\n" +
                        $"Action: {action}\n" +
                        $"Semester: {semesterName}\n" +
                        $"Exam Type: {examType}\n" +
                        $"Subject: {cmbSubjects.SelectedItem}\n" +
                        $"Marks: {obtainedMarks}/{totalMarks}\n" +
                        $"Percentage: {percentage:F2}%\n" +
                        $"GPA: {gpa:F2}\n" +
                        $"Grade: {gradeLetter}\n" +
                        $"Status: {status}\n\n" +
                        (isUpdate ? "?? Previous result has been replaced." : "? New result added."), 
                        "Success", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Information);
                    
                    // Reset for next subject
                    cmbExamType.SelectedIndex = 0;
                    cmbSubjects.SelectedIndex = 0;
                    ClearResultFields();
                }
                else
                {
                    MessageBox.Show("Failed to save result.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                loadingIndicator.Hide();
                MessageBox.Show($"Error saving result: {ex.Message}\n\n" +
                    "Note: Each student can have only ONE result per subject per semester.\n" +
                    "Saving will UPDATE any existing result for this combination.",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
