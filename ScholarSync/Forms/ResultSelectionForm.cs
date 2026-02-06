using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ScholarSync.Commons;
using ScholarSync.Models;

namespace ScholarSync.Forms
{
    public partial class ResultSelectionForm : Form
    {
        private DataGridView dgvResults;
        private Button btnSelect;
        private Button btnCancel;
        private Label lblTitle;
        private List<EnrollmentViewModel> results;

        public EnrollmentViewModel SelectedResult { get; private set; }

        public ResultSelectionForm(List<EnrollmentViewModel> searchResults)
        {
            results = searchResults;
            InitializeComponent();
            InitializeUI();
            LoadResults();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Text = "Select Result";
            this.BackColor = ConfigurationConstants.SSWhiteColor;
        }

        private void InitializeUI()
        {
            lblTitle = new Label
            {
                Text = $"Multiple results found ({results.Count}). Please select one:",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(20, 20),
                Size = new Size(850, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblTitle);

            dgvResults = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(850, 420),
                BackgroundColor = ConfigurationConstants.SSWhiteColor,
                BorderStyle = BorderStyle.FixedSingle,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                AllowUserToResizeRows = false,
                RowTemplate = { Height = 35 }
            };

            dgvResults.ColumnHeadersDefaultCellStyle.BackColor = ConfigurationConstants.SSDarkNavyColor;
            dgvResults.ColumnHeadersDefaultCellStyle.ForeColor = ConfigurationConstants.SSWhiteColor;
            dgvResults.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 11, FontStyle.Bold);
            dgvResults.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvResults.ColumnHeadersHeight = 40;

            dgvResults.DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Regular);
            dgvResults.DefaultCellStyle.ForeColor = ConfigurationConstants.SSDarkBlueColor;
            dgvResults.DefaultCellStyle.SelectionBackColor = ConfigurationConstants.SSLightNavyColor;
            dgvResults.DefaultCellStyle.SelectionForeColor = ConfigurationConstants.SSWhiteColor;
            dgvResults.AlternatingRowsDefaultCellStyle.BackColor = ConfigurationConstants.SSLightGrayBackground;
            dgvResults.GridColor = ConfigurationConstants.SSBorderGray;

            dgvResults.CellDoubleClick += DgvResults_CellDoubleClick;

            this.Controls.Add(dgvResults);

            btnSelect = UIHelper.CreateButton("Select", ConfigurationConstants.SSDarkNavyColor, new Point(560, 500), new Size(150, 45));
            btnSelect.Font = new Font("Arial", 12, FontStyle.Bold);
            btnSelect.ForeColor = ConfigurationConstants.SSWhiteColor;
            btnSelect.Click += BtnSelect_Click;
            this.Controls.Add(btnSelect);

            btnCancel = UIHelper.CreateButton("Cancel", ConfigurationConstants.SSDarkBlueColor, new Point(720, 500), new Size(150, 45));
            btnCancel.Font = new Font("Arial", 12, FontStyle.Bold);
            btnCancel.ForeColor = ConfigurationConstants.SSWhiteColor;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);
        }

        private void LoadResults()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ResultId", typeof(string));
            dt.Columns.Add("Student", typeof(string));
            dt.Columns.Add("Roll No", typeof(string));
            dt.Columns.Add("Subject", typeof(string));
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Semester", typeof(string));
            dt.Columns.Add("Marks", typeof(string));
            dt.Columns.Add("Grade", typeof(string));

            foreach (var result in results)
            {
                dt.Rows.Add(
                    result.ResultId,
                    result.StudentName,
                    result.StudentRollNumber,
                    result.SubjectName,
                    result.SubjectCode,
                    result.SemesterName ?? "N/A",
                    $"{result.ObtainedMarks ?? 0}/{result.TotalMarks ?? 100}",
                    result.GradeLetter ?? "N/A"
                );
            }

            dgvResults.DataSource = dt;

            if (dgvResults.Columns["ResultId"] != null)
                dgvResults.Columns["ResultId"].Visible = false;

            if (dgvResults.Rows.Count > 0)
                dgvResults.Rows[0].Selected = true;
        }

        private void DgvResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                SelectCurrentRow();
            }
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            SelectCurrentRow();
        }

        private void SelectCurrentRow()
        {
            if (dgvResults.SelectedRows.Count > 0)
            {
                string resultId = dgvResults.SelectedRows[0].Cells["ResultId"].Value.ToString();
                SelectedResult = results.FirstOrDefault(r => r.ResultId == resultId);
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
