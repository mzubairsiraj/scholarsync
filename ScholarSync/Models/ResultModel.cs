using System;

namespace ScholarSync.Models
{
    
    public class ResultModel
    {
        public string Id { get; set; }
        public string EnrollmentId { get; set; }
        public string ExamType { get; set; }
        public int TotalMarks { get; set; }
        public decimal ObtainedMarks { get; set; }
        public decimal? GPA { get; set; }
        public string GradeLetter { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
