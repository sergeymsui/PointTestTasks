using System;
using System.Windows;

using System.Windows.Input;

using System.Data;
using System.Data.SqlClient;


namespace GuiReviewer
{
    /// <summary>
    /// Основное окно приложения
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Коннект к базе
        /// </summary>
        private SqlConnection connection;

        /// <summary>
        /// Таблица Customers
        /// </summary>
        private DataTable customerTable;

        /// Параметры подключения к БД
        private string Host;
        private string Base;
        private string User;
        private string Password;

        public MainWindow()
        {
            InitializeComponent();

            /// Диалог инициализации параметров подключения к БД
            DBDialog dialog = new DBDialog();
            dialog.ShowDialog();

            this.Host = dialog.Host;
            this.Base = dialog.Base;
            this.User = dialog.User;
            this.Password = dialog.Password;

            /// Коннект к БД
            connection = DBSQLServerUtils.GetDBConnection(Host, Base, User, Password);

            try
            {
                connection.Open();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }

            /// Запрос Customers
            const string sqlCustomers = "SELECT ID,LastName,FirstName,MiddleName,Sex,BirthDate,RegistrationDate FROM Customers";
            SqlCommand createCommand = new SqlCommand(sqlCustomers, connection);
            createCommand.ExecuteNonQuery();

            /// Адаптер для таблицы
            SqlDataAdapter dataAdp = new SqlDataAdapter(createCommand);

            customerTable = new DataTable("Customers"); // В скобках указываем название таблицы
            
            dataAdp.Fill(customerTable);

            /// Только для чтения
            customerTable.Columns[0].ReadOnly = true;
            customerTable.Columns[1].ReadOnly = true;
            customerTable.Columns[2].ReadOnly = true;
            customerTable.Columns[3].ReadOnly = true;
            customerTable.Columns[4].ReadOnly = true;
            customerTable.Columns[5].ReadOnly = true;
            customerTable.Columns[6].ReadOnly = true;

            /// Переименование заголовков столбцов
            customerTable.Columns[0].ColumnName = "ID";
            customerTable.Columns[1].ColumnName = "Фамилия";
            customerTable.Columns[2].ColumnName = "Имя";
            customerTable.Columns[3].ColumnName = "Отчество";
            customerTable.Columns[4].ColumnName = "Пол";
            customerTable.Columns[5].ColumnName = "День рождения";
            customerTable.Columns[6].ColumnName = "Дата регистрации";

            /// Занесение данных в DataGrid
            dataGrid.ItemsSource = customerTable.DefaultView;
        }

        /// <summary>
        /// Метод для заполнения таблицы заказов
        /// </summary>
        /// <param name="ID">ID клиента</param>
        private void fillOrderTable(string ID)
        {
            /// Запрос к БД
            string sqlOrders = "SELECT ID,CustomerID,OrderDate,Price FROM Orders WHERE CustomerID=" + ID;

            SqlCommand createCommand = new SqlCommand(sqlOrders, connection);
            createCommand.ExecuteNonQuery();

            SqlDataAdapter dataAdp = new SqlDataAdapter(createCommand);
            DataTable dt = new DataTable("Orders");

            dataAdp.Fill(dt);

            /// Занесение данных в DataGrid
            dataGrid2.ItemsSource = dt.DefaultView;
        }

        ~MainWindow()
        {
            connection.Close();
        }

        /// <summary>
        /// Обработчик двойного клика по строке таблицы клиентов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Customer_Click(object sender, MouseButtonEventArgs e)
        {
            /// Номер строки
            int currentRowIndex = dataGrid.Items.IndexOf(dataGrid.CurrentItem);

            /// Если currentRowIndex выходит за пределы таблицы данных
            if (currentRowIndex > customerTable.Rows.Count-1) return;

            /// ID клиента
            decimal value = customerTable.Rows[currentRowIndex].Field<decimal>(0);
            /// Перезаполнение таблицы заказов
            fillOrderTable(value.ToString());
        }
    }
}
