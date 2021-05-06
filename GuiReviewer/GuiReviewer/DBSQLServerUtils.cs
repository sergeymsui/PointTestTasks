using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace GuiReviewer
{
    /// <summary>
    /// Класс для коннекта к БД
    /// </summary>
    class DBSQLServerUtils
    {
        /// <summary>
        /// Метод создания коннекта к БД
        /// </summary>
        /// <param name="datasource">Адрес</param>
        /// <param name="database">Наименование БД</param>
        /// <param name="username">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        /// <returns>Возвращает SqlConnection</returns>
        public static SqlConnection GetDBConnection(string datasource, string database, string username, string password)
        {
            string connString = @"Data Source=" + datasource + ";Initial Catalog="
                        + database + ";Persist Security Info=True;User ID=" + username + ";Password=" + password;

            SqlConnection conn = new SqlConnection(connString);
            return conn;
        }
    }
}
