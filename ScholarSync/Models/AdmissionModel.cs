using System;

namespace ScholarSync.Models
{
    public class AdmissionModel
    {
        public string Id { get; set; }
        public string StudentId { get; set; }
        public string ProgramId { get; set; }
        public int SessionYear { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string Status { get; set; }
        public string FormNumber { get; set; }
    }
}
