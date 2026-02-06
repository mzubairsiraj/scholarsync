using System;

namespace ScholarSync.Models
{
    public class AcademicHistoryModel
    {
        public string Id { get; set; }
        public string StudentId { get; set; }
        public string DegreeTitle { get; set; }
        public string BoardUniversity { get; set; }
        public int PassingYear { get; set; }
        public string RollNumber { get; set; }
        public int TotalMarks { get; set; }
        public int ObtainedMarks { get; set; }
        public string GradeDivision { get; set; }
    }
}
