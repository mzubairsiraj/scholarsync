using System;
using System.Collections.Generic;

namespace ScholarSync.Models
{
    /// <summary>
    /// Model for student transcript data
    /// </summary>
    public class TranscriptModel
    {
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string RollNumber { get; set; }
        public string Program { get; set; }
        public string CNIC { get; set; }
        public string Department { get; set; }
        
        public List<SemesterTranscriptData> Semesters { get; set; }
        public decimal OverallCGPA { get; set; }
        public int TotalCreditHours { get; set; }
        
        public TranscriptModel()
        {
            Semesters = new List<SemesterTranscriptData>();
        }
    }

    public class SemesterTranscriptData
    {
        public string SemesterId { get; set; }
        public string SemesterName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
        public List<SubjectResultData> Subjects { get; set; }
        public decimal SemesterGPA { get; set; }
        public int SemesterCreditHours { get; set; }
        
        public SemesterTranscriptData()
        {
            Subjects = new List<SubjectResultData>();
        }
    }

   
    public class SubjectResultData
    {
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public int CreditHours { get; set; }
        public int TotalMarks { get; set; }
        public decimal ObtainedMarks { get; set; }
        public decimal Percentage { get; set; }
        public decimal GPA { get; set; }
        public string GradeLetter { get; set; }
    }
}
