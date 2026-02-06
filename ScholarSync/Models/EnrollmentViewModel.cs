using System;

namespace ScholarSync.Models
{
    /// <summary>
    /// Comprehensive enrollment/result view model with all related data
    /// Used for displaying complete enrollment information
    /// </summary>
    public class EnrollmentViewModel
    {
        // Enrollment Info
        public string Id { get; set; }
        public string StudentId { get; set; }
        public string SubjectId { get; set; }
        public string SemesterId { get; set; }

        // Student Info
        public string StudentRollNumber { get; set; }
        public string StudentName { get; set; }
        public string StudentCNIC { get; set; }

        // Subject Info
        public string SubjectName { get; set; }
        public string SubjectCode { get; set; }
        public string DepartmentName { get; set; }

        // Semester Info
        public string SemesterName { get; set; }

        // Result Info (if exists)
        public string ResultId { get; set; }
        public string ExamType { get; set; }
        public int? TotalMarks { get; set; }
        public decimal? ObtainedMarks { get; set; }
        public decimal? GPA { get; set; }
        public string GradeLetter { get; set; }
        public string Remarks { get; set; }

        // Computed Properties
        public string ResultDisplay
        {
            get
            {
                if (!ObtainedMarks.HasValue)
                    return "Not Graded";
                
                return $"{ObtainedMarks.Value}/{TotalMarks ?? 100}";
            }
        }

        public string GradeStatus
        {
            get
            {
                if (!ObtainedMarks.HasValue)
                    return "Pending";
                
                if (ObtainedMarks.Value >= 50)
                    return "Pass";
                else
                    return "Fail";
            }
        }
    }
}

