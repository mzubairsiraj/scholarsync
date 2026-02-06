using System;

namespace ScholarSync.Models
{
    public class DepartmentModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string HeadOfDepartmentId { get; set; }
        public bool IsActive { get; set; }
    }
}
