using System;

namespace ScholarSync.Models
{
    public class SubjectModel
    {
        public string Id { get; set; }
        public string DeptId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int CreditHours { get; set; }
    }
}
