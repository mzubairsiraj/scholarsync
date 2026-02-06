using System;
using System.Collections.Generic;

namespace ScholarSync.Models
{
    
    public class StudentViewModel
    {
        
        public string Id { get; set; }
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

        // Program Info
        public string ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }

        // User Info
        public string UserId { get; set; }
        public string Email { get; set; }

        // Related Collections
        public List<GuardianModel> Guardians { get; set; }
        public List<AcademicHistoryModel> AcademicHistory { get; set; }
        public List<EnrollmentModel> Enrollments { get; set; }

        // Computed Properties
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        public string DisplayName
        {
            get { return $"{FullName} ({RollNumber})"; }
        }

        public StudentViewModel()
        {
            Guardians = new List<GuardianModel>();
            AcademicHistory = new List<AcademicHistoryModel>();
            Enrollments = new List<EnrollmentModel>();
        }
    }
}
