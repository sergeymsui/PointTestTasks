using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace CConfigurator
{
    class DBSQLServerUtils
    {

        public static SqlConnection GetDBConnection(string datasource, string database, string username, string password)
        {
            string connString = @"Data Source=" + datasource + ";Initial Catalog="
                        + database + ";Persist Security Info=True;User ID=" + username + ";Password=" + password;// + ";Port=" + port;

            SqlConnection conn = new SqlConnection(connString);

            return conn;
        }

        public static SqlConnection GetServerConnection(string datasource, string username, string password)
        {
            string connString = @"Data Source=" + datasource + ";Persist Security Info=True;User ID=" + username + ";Password=" + password;// + ";Port=" + port;

            SqlConnection conn = new SqlConnection(connString);

            return conn;
        }



    }
}
