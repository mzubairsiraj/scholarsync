using System;

namespace ScholarSync.Models
{
    public class StudentModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ProgramId { get; set; }
        public string CNIC { get; set; }
        public string RollNumber { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string BloodGroup { get; set; }
        public string Religion { get; set; }
        public string Nationality { get; set; }
        public string DomicileDistrict { get; set; }
        public string MobileNo { get; set; }
        public string WhatsappNo { get; set; }
    }
}
