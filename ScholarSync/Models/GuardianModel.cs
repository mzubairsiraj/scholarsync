using System;

namespace ScholarSync.Models
{
    public class GuardianModel
    {
        public string Id { get; set; }
        public string StudentId { get; set; }
        public string Relation { get; set; }
        public string FullName { get; set; }
        public string CNIC { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public bool IsDeceased { get; set; }
    }
}
