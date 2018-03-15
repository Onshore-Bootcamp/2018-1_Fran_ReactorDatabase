using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactorDB_DAL.Models
{
    public class UserDO
    {
        public int UserID { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public int RoleID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }

        public string Email { get; set; }

        public bool Subscription { get; set; }
    }
}
