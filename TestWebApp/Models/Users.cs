using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApp.Models
{
    public class Users
    {
        public string Username { get; set; }
        public string Pwd { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<string> Roles = new List<string>();
    }
}
