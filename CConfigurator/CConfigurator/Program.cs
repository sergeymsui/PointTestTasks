using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;

using System.Diagnostics;

using System.IO;
using System.Threading;


namespace CConfigurator
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Host:");
            string host = Console.ReadLine();

            Console.WriteLine("User:");
            string user = Console.ReadLine();

            Console.WriteLine("Password:");
            //string password = Console.ReadLine();

            ConsoleKeyInfo key; string pass = "";
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace) // Backspace Should Not Work
                {
                    pass += key.KeyChar;
                    //Console.Write("*");
                }
                else
                {
                    pass = pass.Substring(0, pass.Length - 1);
                }
            } while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();



            SqlConnection serverConnection = DBSQLServerUtils.GetServerConnection(host, user, pass);

            try
            {
                Program.Notify("Openning Connection ...");
                serverConnection.Open();
                Program.Notify("Connection successful!");

                Program.CreateDatabase(serverConnection);
            }
            catch (Exception err)
            {
                Program.Notify("Error: " + err.Message);
            }

            serverConnection.Close();

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

            Program.CreateTables(dbConnection);

            Program.GenerateCustomers(dbConnection);

            int orderCount = 40;

            Program.GenerateOrders(dbConnection, orderCount);

            /*
            Thread ordersThread = new Thread(() => Program.GenerateOrders(dbConnection, orderCount));
            ordersThread.IsBackground = true;
            ordersThread.Join();
            */
        }

        public const string DATABASE_NAME = "configurator_database";

        public static List<decimal> m_ids = new List<decimal>();

        public static void Notify(string message)
        {
            Console.WriteLine(message);
        }


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



        public static void GenerateCustomers(SqlConnection connection)
        {
            Notify("GenerateCustomers start");

            //this.Dispatcher.Invoke(() => { this.PrintMessage("GenerateCustomers start"); });
            if (connection.State != ConnectionState.Open)
                connection.Open();

            const string customerSql = "INSERT INTO dbo.Customers(ID,LastName,FirstName,MiddleName,Sex,BirthDate,RegistrationDate) VALUES(@p1,@p2,@p3,@p4,@p5,@p6,@p7)";

            for (int i = 0; i < 15; i++)
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
            }
            Notify("GenerateCustomers finish");
        }

        public static void GenerateOrders(SqlConnection connection, int orderCount = 20)
        {
            Notify("GenerateOrders start");

            if (connection.State != ConnectionState.Open)
                connection.Open();

            const string orderSql = "INSERT INTO dbo.Orders(ID,CustomerID,OrderDate,Price) VALUES(@p1,@p2,@p3,@p4)";

            Random getrandom = new Random();
            Random getprice = new Random();


            for (int i = 0; i < orderCount; i++)
            {
                SqlCommand cmd = new SqlCommand(orderSql, connection);

                // Рандомный индекс
                int index = (int)getrandom.Next(0, m_ids.Count);

                // Рандомный ID клиента
                decimal CustomerID = (decimal) m_ids[index];

                double percent = 100 * i / orderCount;
                Program.Notify(i.ToString() + " adding...");

                cmd.Parameters.Add("@p1", SqlDbType.Decimal).Value = (decimal)i + 1;

                cmd.Parameters.Add("@p2", SqlDbType.Decimal).Value = CustomerID;
                cmd.Parameters.Add("@p3", SqlDbType.DateTime).Value = (DateTime.Now.Date);

                decimal price = (decimal)getprice.NextDouble() * 1000;
                cmd.Parameters.Add("@p4", SqlDbType.Decimal).Value = price;


                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                Thread.Sleep(10);
            }
            Notify("GenerateOrders end");
        }
    }

    

}
