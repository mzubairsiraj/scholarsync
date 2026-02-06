using System;

namespace ScholarSync.Models
{
    public class ProgramModel
    {
        public string Id { get; set; }
        public string DeptId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int DurationYears { get; set; }
        public bool IsActive { get; set; }
        
       
        public string DepartmentName { get; set; }
    }
}

