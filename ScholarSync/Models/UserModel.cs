using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using ScholarSync.Commons;
using ScholarSync.Db_Connection;

namespace ScholarSync.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string CNIC { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        

        
    }
}



