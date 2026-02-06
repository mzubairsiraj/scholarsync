using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using ScholarSync.Commons;
using ScholarSync.Models;
using ScholarSync.Repositories;
using ScholarSync.Services;

namespace ScholarSync.Controls.Result
{
    public partial class GenerateTranscriptControl : UserControl
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Panel searchPanel;
        private Panel optionsPanel;
        private Panel buttonPanel;

        private Label lblSearchStudent;
        private TextBox txtSearchStudent;
        private Button btnSearchStudent;
        private Button btnClearSearch;
        private Label lblSearchHint;
        private Label lblCurrentStudent;

        private Label lblStudentName;
        private Label lblStudentRollNo;
        private Label lblStudentProgram;

        private Label lblReportType;
        private ComboBox cmbReportType;
        private Label lblSemester;
        private ComboBox cmbSemester;

        private Button btnGeneratePdf;

        private LoadingIndicator loadingIndicator;
        private TranscriptRepository transcriptRepository;
        private TranscriptPdfService pdfService;

        private string currentStudentId = null;
        private bool studentLoaded = false;

        public GenerateTranscriptControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.BackColor = ConfigurationConstants.SSWhiteColor;
            this.AutoScroll = true;

            transcriptRepository = new TranscriptRepository();
            pdfService = new TranscriptPdfService();

            InitializeUI();

            loadingIndicator = new LoadingIndicator();
            this.Controls.Add(loadingIndicator);
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
                Text = "Generate Student Transcript",
                Font = new Font("Arial", 22, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(30, 0),
                Size = new Size(600, 80),
                TextAlign = ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.Add(titleLabel);
            this.Controls.Add(headerPanel);

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

            optionsPanel = new Panel
            {
                Dock = DockStyle.None,
                BackColor = ConfigurationConstants.SSWhiteColor,
                Location = new Point(10, 240),
                Size = new Size(ConfigurationConstants.ScreenWidth - 300, ConfigurationConstants.ScreenHeight - 420),
                AutoScroll = true,
                Padding = new Padding(40, 30, 40, 30),
                Enabled = false
            };

            InitializeOptionsPanel();
            this.Controls.Add(optionsPanel);

            buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 100,
                BackColor = ConfigurationConstants.SSLightGrayBackground,
                Padding = new Padding(40, 25, 40, 25)
            };

            btnGeneratePdf = UIHelper.CreateButton("Generate PDF Transcript", ConfigurationConstants.SSDarkNavyColor, new Point(40, 25), new Size(220, 50));
            btnGeneratePdf.Click += BtnGeneratePdf_Click;
            btnGeneratePdf.Enabled = false;
            btnGeneratePdf.ForeColor = ConfigurationConstants.SSWhiteColor;
            btnGeneratePdf.Font = new Font("Arial", 12, FontStyle.Bold);

            buttonPanel.Controls.Add(btnGeneratePdf);
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
                Size = new Size(130, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtSearchStudent = new TextBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(150, yPos),
                Size = new Size(350, 40),
                ForeColor = Color.Gray,
                Text = "Enter Roll Number or CNIC...",
                Tag = "Enter Roll Number or CNIC..."
            };

            txtSearchStudent.Enter += TxtSearchStudent_Enter;
            txtSearchStudent.Leave += TxtSearchStudent_Leave;
            txtSearchStudent.KeyPress += TxtSearchStudent_KeyPress;

            btnSearchStudent = UIHelper.CreateButton("Search", ConfigurationConstants.SSDarkNavyColor, new Point(520, yPos), new Size(120, 40));
            btnSearchStudent.Click += BtnSearchStudent_Click;

            btnClearSearch = UIHelper.CreateButton("Clear", ConfigurationConstants.SSDarkBlueColor, new Point(660, yPos), new Size(120, 40));
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
                Location = new Point(800, yPos),
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

        private void InitializeOptionsPanel()
        {
            int yPos = 0;

            Label lblStudentInfo = new Label
            {
                Text = "Student Information",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(0, yPos),
                Size = new Size(300, 35),
                TextAlign = ContentAlignment.MiddleLeft
            };
            optionsPanel.Controls.Add(lblStudentInfo);
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
            optionsPanel.Controls.Add(lblStudentName);

            lblStudentRollNo = new Label
            {
                Text = "Roll No: -",
                Font = new Font("Arial", 11, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(450, yPos),
                Size = new Size(300, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            optionsPanel.Controls.Add(lblStudentRollNo);
            yPos += 35;

            lblStudentProgram = new Label
            {
                Text = "Program: -",
                Font = new Font("Arial", 11, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(600, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            optionsPanel.Controls.Add(lblStudentProgram);
            yPos += 60;

            Label lblTranscriptOptions = new Label
            {
                Text = "Transcript Options",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(0, yPos),
                Size = new Size(300, 35),
                TextAlign = ContentAlignment.MiddleLeft
            };
            optionsPanel.Controls.Add(lblTranscriptOptions);
            yPos += 45;

            lblReportType = new Label
            {
                Text = "Report Type:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(120, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            optionsPanel.Controls.Add(lblReportType);

            cmbReportType = new ComboBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(150, yPos),
                Size = new Size(300, 40),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbReportType.Items.AddRange(new string[] { "Full Transcript (All Semesters)", "Semester Wise Transcript" });
            cmbReportType.SelectedIndex = 0;
            cmbReportType.SelectedIndexChanged += CmbReportType_SelectedIndexChanged;
            optionsPanel.Controls.Add(cmbReportType);
            yPos += 60;

            lblSemester = new Label
            {
                Text = "Select Semester:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(20, yPos),
                Size = new Size(120, 30),
                TextAlign = ContentAlignment.MiddleLeft,
                Enabled = false
            };
            optionsPanel.Controls.Add(lblSemester);

            cmbSemester = new ComboBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(150, yPos),
                Size = new Size(300, 40),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };
            optionsPanel.Controls.Add(cmbSemester);
            yPos += 60;

            Panel infoPanel = new Panel
            {
                Location = new Point(0, yPos),
                Size = new Size(700, 150),
                BackColor = ConfigurationConstants.SSLightGrayBackground,
                Padding = new Padding(20)
            };

            Label lblInfo = new Label
            {
                Text = "?? Important Information:\n\n" +
                       "• Full Transcript: Generates a complete academic record for all semesters.\n" +
                       "• Semester Wise: Generates transcript for a specific semester only.\n" +
                       "• PDF will be saved to your selected location.\n" +
                       "• Ensure you have a PDF printer installed (e.g., Microsoft Print to PDF).",
                Font = new Font("Arial", 10, FontStyle.Regular),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(10, 10),
                Size = new Size(680, 130),
                TextAlign = ContentAlignment.TopLeft
            };
            infoPanel.Controls.Add(lblInfo);
            optionsPanel.Controls.Add(infoPanel);
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
                MessageBox.Show("Please enter Roll Number or CNIC to search.", "Search Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSearchStudent.Focus();
                return;
            }

            SearchStudent(txtSearchStudent.Text.Trim());
        }

        private void SearchStudent(string searchTerm)
        {
            loadingIndicator.Show("Searching Student");

            try
            {
                StudentRepository studentRepo = new StudentRepository();
                var students = studentRepo.SearchStudents(searchTerm);

                loadingIndicator.Hide();

                if (students.Count == 0)
                {
                    MessageBox.Show("No student found with the given Roll Number or CNIC.",
                        "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (students.Count > 1)
                {
                    MessageBox.Show("Multiple students found. Please enter more specific criteria.",
                        "Multiple Results", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                LoadStudent(students[0]);
            }
            catch (Exception ex)
            {
                loadingIndicator.Hide();
                MessageBox.Show($"Error searching student: {ex.Message}",
                    "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadStudent(StudentViewModel student)
        {
            currentStudentId = student.Id;
            studentLoaded = true;

            lblStudentName.Text = $"Name: {student.FullName}";
            lblStudentRollNo.Text = $"Roll No: {student.RollNumber}";
            lblStudentProgram.Text = $"Program: {student.ProgramName}";

            lblCurrentStudent.Text = $"Student: {student.FullName}";
            lblCurrentStudent.ForeColor = ConfigurationConstants.SSSuccessGreen;

            optionsPanel.Enabled = true;
            btnGeneratePdf.Enabled = true;

            LoadStudentSemesters();

            MessageBox.Show("Student loaded successfully! Select report options and generate transcript.",
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoadStudentSemesters()
        {
            try
            {
                var semesters = transcriptRepository.GetStudentSemesters(currentStudentId);
                
                cmbSemester.Items.Clear();
                foreach (var semester in semesters)
                {
                    cmbSemester.Items.Add(new ComboBoxItem
                    {
                        Text = semester.Name,
                        Value = semester.Id
                    });
                }

                if (cmbSemester.Items.Count > 0)
                {
                    cmbSemester.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading semesters: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isSemesterWise = cmbReportType.SelectedIndex == 1;
            lblSemester.Enabled = isSemesterWise;
            cmbSemester.Enabled = isSemesterWise;
        }

        private void BtnGeneratePdf_Click(object sender, EventArgs e)
        {
            if (!studentLoaded || currentStudentId == null)
            {
                MessageBox.Show("Please search and load a student first.", "No Student Loaded",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool isFullTranscript = cmbReportType.SelectedIndex == 0;

            if (!isFullTranscript && cmbSemester.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a semester.", "Semester Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            GenerateTranscript(isFullTranscript);
        }

        private void GenerateTranscript(bool isFullTranscript)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf|XPS Files (*.xps)|*.xps",
                DefaultExt = "pdf",
                FileName = isFullTranscript ? 
                    $"Transcript_Full_{lblStudentRollNo.Text.Replace("Roll No: ", "").Replace("/", "-")}.pdf" :
                    $"Transcript_Semester_{lblStudentRollNo.Text.Replace("Roll No: ", "").Replace("/", "-")}.pdf",
                Title = "Save Transcript As"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveDialog.FileName;

                // Check if file is locked
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        using (var stream = System.IO.File.Open(filePath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite))
                        {
                            // File is not locked
                        }
                    }
                    catch
                    {
                        MessageBox.Show("The file is currently open in another program. Please close it and try again.",
                            "File Locked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                loadingIndicator.Show("Generating Transcript");

                try
                {
                    TranscriptModel transcript;

                    if (isFullTranscript)
                    {
                        transcript = transcriptRepository.GetFullTranscript(currentStudentId);
                    }
                    else
                    {
                        ComboBoxItem selectedSemester = (ComboBoxItem)cmbSemester.SelectedItem;
                        transcript = transcriptRepository.GetSemesterTranscript(currentStudentId, selectedSemester.Value);
                    }

                    if (transcript == null || transcript.Semesters.Count == 0)
                    {
                        loadingIndicator.Hide();
                        MessageBox.Show("No results found for this student. Please ensure the student has completed courses with results.",
                            "No Records Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    bool success;
                    if (isFullTranscript)
                    {
                        success = pdfService.GenerateFullTranscript(transcript, filePath);
                    }
                    else
                    {
                        success = pdfService.GenerateSemesterTranscript(transcript, filePath);
                    }

                    loadingIndicator.Hide();

                    if (success)
                    {
                        DialogResult result = MessageBox.Show(
                            $"Transcript generated successfully!\n\nSaved to: {filePath}\n\nWould you like to open the file?",
                            "Success",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        if (result == DialogResult.Yes)
                        {
                            try
                            {
                                System.Diagnostics.Process.Start(filePath);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"File saved but could not be opened automatically: {ex.Message}",
                                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to generate transcript. Please ensure you have a PDF printer installed (e.g., Microsoft Print to PDF).",
                            "Generation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    loadingIndicator.Hide();
                    MessageBox.Show($"Error generating transcript: {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnClearSearch_Click(object sender, EventArgs e)
        {
            UIHelper.ClearToPlaceholder(txtSearchStudent);
            ClearForm();
        }

        private void ClearForm()
        {
            optionsPanel.Enabled = false;
            btnGeneratePdf.Enabled = false;

            lblCurrentStudent.Text = "No student loaded";
            lblCurrentStudent.ForeColor = ConfigurationConstants.SSErrorRed;

            lblStudentName.Text = "Name: -";
            lblStudentRollNo.Text = "Roll No: -";
            lblStudentProgram.Text = "Program: -";

            cmbReportType.SelectedIndex = 0;
            cmbSemester.Items.Clear();
            cmbSemester.Enabled = false;
            lblSemester.Enabled = false;

            studentLoaded = false;
            currentStudentId = null;
        }

        private class ComboBoxItem
        {
            public string Text { get; set; }
            public string Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}
