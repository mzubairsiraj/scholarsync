using System;

namespace ScholarSync.Models
{
   
    public class TeacherViewModel
    {
        // Teacher Basic Info
        public string Id { get; set; }
        public string UserId { get; set; }
        public string DepartmentId { get; set; }
        public string CNIC { get; set; }
        public string FullName { get; set; }
        public string Designation { get; set; }
        public string Specialization { get; set; }

        // User Info
        public string Email { get; set; }

        // Computed Properties
        public string DisplayName
        {
            get { return $"{FullName} ({CNIC})"; }
        }
    }
}

