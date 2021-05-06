using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace GuiConfigurator
{

    /// <summary>
    /// Класс для коннекта к БД
    /// </summary>
    class DBSQLServerUtils
    {
        /// <summary>
        /// Метод создания коннекта к БД
        /// </summary>
        /// <param name="datasource"></param>
        /// <param name="database"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static SqlConnection GetDBConnection(string datasource, string database, string username, string password)
        {
            string connString = @"Data Source=" + datasource + ";Initial Catalog="
                        + database + ";Persist Security Info=True;User ID=" + username + ";Password=" + password;

            SqlConnection conn = new SqlConnection(connString);

            return conn;
        }

        /// <summary>
        /// Метод создания коннекта к серверу
        /// </summary>
        /// <param name="datasource"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static SqlConnection GetServerConnection(string datasource, string username, string password)
        {
            string connString = @"Data Source=" + datasource + ";Persist Security Info=True;User ID=" + username + ";Password=" + password;// + ";Port=" + port;

            SqlConnection conn = new SqlConnection(connString);

            return conn;
        }



    }


}


