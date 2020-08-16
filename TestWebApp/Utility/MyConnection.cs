using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApp.Utility
{
    public class MyConnection
    {
        private IConfiguration _configuration;
        public MyConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IDbConnection GetConnection()
        {
            string conString = _configuration.GetValue<string>("DefaultConnection");
            IDbConnection con = new SqlConnection(conString);
            return con;
        }
    }
}
