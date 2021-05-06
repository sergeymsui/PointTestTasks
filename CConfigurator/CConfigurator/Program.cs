using System;
using System.Collections.Generic;

using System.Data;
using System.Data.SqlClient;

using System.Threading;


namespace CConfigurator
{
    class Program
    {
        /// <summary>
        /// Наименование БД
        /// </summary>
        public const string DATABASE_NAME = "configurator_database";
        public static void Main(string[] args)
        {
            /// Запрос параметров

            Console.WriteLine("Host:");
            string host = Console.ReadLine();

            Console.WriteLine("User:");
            string user = Console.ReadLine();

            /// Запрос пароля
            Console.WriteLine("Password:");

            ConsoleKeyInfo key; string pass = "";
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace)
                {
                    pass += key.KeyChar;
                }
                else
                {
                    pass = pass.Substring(0, pass.Length - 1);
                }
            } while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();

            /// Ввод колисечтва заказов
            Console.WriteLine("Orders count");
            int ordersCount = Int32.Parse(Console.ReadLine());

            /// Коннект к серверу
            SqlConnection serverConnection = DBSQLServerUtils.GetServerConnection(host, user, pass);

            try
            {
                Program.Notify("Openning Connection ...");
                serverConnection.Open();
                Program.Notify("Connection successful!");

                /// Создание БД
                Program.CreateDatabase(serverConnection);
            }
            catch (Exception err)
            {
                Program.Notify("Error: " + err.Message);
            }

            serverConnection.Close();

            /// Коннект к базе
            SqlConnection dbConnection = DBSQLServerUtils.GetDBConnection(host, Program.DATABASE_NAME, user, pass);

            try
            {
                Program.Notify("Openning Connection ...");
                dbConnection.Open();
                Program.Notify("Connection successful!");
            }
            catch (Exception err)
            {
                Program.Notify("Error: " + err.Message);
            }

            /// Создание таблиц
            Program.CreateTables(dbConnection);

            /// Заполнение таблицы клиентов
            Program.GenerateCustomers(dbConnection);

            /// Заполнение таблицы заказов
            Program.GenerateOrders(dbConnection, ordersCount);

            dbConnection.Close();
        }

        /// <summary>
        /// Лист ID клиентов
        /// </summary>
        public static List<decimal> m_ids = new List<decimal>();

        /// <summary>
        /// Уведомления (Метод сделан как логгер)
        /// </summary>
        /// <param name="message"></param>
        public static void Notify(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Метод для создания базы
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateDatabase(SqlConnection connection)
        {
            Notify("CreateDatabase ...");

            if (connection.State != ConnectionState.Open)
                connection.Open();

            const string sql = "CREATE DATABASE " + DATABASE_NAME;

            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
            Notify("Finish create database ...");
        }

        /// <summary>
        /// Метод создания таблиц
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateTables(SqlConnection connection)
        {
            Notify("Customers and Orders table creating...");
            if (connection.State != ConnectionState.Open)
                connection.Open();

            SqlCommand cmd;

            string customers = "create table dbo.Customers (" +
                "ID decimal constraint Customers_pk primary key nonclustered," +
                "LastName nvarchar(64)," +
                "FirstName nvarchar(64)," +
                "MiddleName nvarchar(64)," +
                "Sex nvarchar(32)," +
                "BirthDate DATETIME," +
                "RegistrationDate DATETIME )";

            cmd = new SqlCommand(customers, connection);
            Notify("Customers table creating...");
            cmd.ExecuteNonQuery();

            string orders = "create table dbo.Orders (" +
                "ID decimal constraint Orders_pk primary key nonclustered," +
                "CustomerID decimal," +
                "OrderDate DATETIME," +
                "Price decimal )";

            cmd = new SqlCommand(orders, connection);
            Notify("Orders table creating...");
            cmd.ExecuteNonQuery();

            Notify("Finish");
        }

        /// <summary>
        /// Заполнение таблиц клиентов
        /// </summary>
        /// <param name="connection"></param>
        public static void GenerateCustomers(SqlConnection connection)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            Notify("GenerateCustomers");

            /// Progress bar для отображения текущего процесса
            ProgressBar progress = new ProgressBar();

            /// Число клиентов
            var CustomersCount = 15;

            const string customerSql = "INSERT INTO dbo.Customers(ID,LastName,FirstName,MiddleName,Sex,BirthDate,RegistrationDate) VALUES(@p1,@p2,@p3,@p4,@p5,@p6,@p7)";

            for (int i = 0; i < CustomersCount; i++)
            {
                SqlCommand cmd = new SqlCommand(customerSql, connection);
                cmd.Parameters.Add("@p1", SqlDbType.Decimal).Value = i;
                Program.m_ids.Add(i);

                cmd.Parameters.Add("@p2", SqlDbType.NVarChar, 64).Value = "Фамилия " + i.ToString();
                cmd.Parameters.Add("@p3", SqlDbType.NVarChar, 64).Value = "Имя " + i.ToString();
                cmd.Parameters.Add("@p4", SqlDbType.NVarChar, 64).Value = "Отчество " + i.ToString();

                string s = (i % 2 == 0) ? "Male" : "Female";
                cmd.Parameters.Add("@p5", SqlDbType.NVarChar, 32).Value = s;
                cmd.Parameters.Add("@p6", SqlDbType.DateTime).Value = (DateTime.Now.Date);
                cmd.Parameters.Add("@p7", SqlDbType.DateTime).Value = (DateTime.Now.Date);
                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();

                /// Смена статуса
                progress.Report((double) i / CustomersCount);
            }

            /// 100% завершение
            progress.Report(1);
            Thread.Sleep(1000);
            Notify("GenerateCustomers finish");
        }

        /// <summary>
        /// Метод заполнения таблицы заказов
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="orderCount"></param>
        public static void GenerateOrders(SqlConnection connection, int orderCount = 20)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            Notify("GenerateOrders");

            /// Progress bar для отображения текущего процесса
            ProgressBar progress = new ProgressBar();

            const string orderSql = "INSERT INTO dbo.Orders(ID,CustomerID,OrderDate,Price) VALUES(@p1,@p2,@p3,@p4)";

            /// Рандомизатор для хаотичного
            /// - выбора клиента
            Random getrandom = new Random();
            /// - задания цены
            Random getprice = new Random();

            for (int i = 0; i < orderCount; i++)
            {
                SqlCommand cmd = new SqlCommand(orderSql, connection);

                /// Рандомный индекс
                int index = (int)getrandom.Next(0, m_ids.Count);

                /// Рандомный ID клиента
                decimal CustomerID = (decimal) m_ids[index];
                
                /// ID
                cmd.Parameters.Add("@p1", SqlDbType.Decimal).Value = (decimal) i + 1;

                /// CustomerID
                cmd.Parameters.Add("@p2", SqlDbType.Decimal).Value = CustomerID;

                /// Дата текущая
                cmd.Parameters.Add("@p3", SqlDbType.DateTime).Value = (DateTime.Now.Date);

                /// Цена
                decimal price = (decimal)getprice.NextDouble() * 1000;
                cmd.Parameters.Add("@p4", SqlDbType.Decimal).Value = price;


                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                /// Обновляем процент завершения
                progress.Report((double) i / orderCount);
            }

            /// 100% завершение
            progress.Report(1);
            Thread.Sleep(1000);
            Notify("GenerateOrders end");
        }
    }

    

}
