using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;



using System.Data;
using System.Data.SqlClient;

using System.Diagnostics;

using System.IO;
using System.Threading;

namespace GuiConfigurator
{
    /// <summary>
    /// Диалоговое окно - форма для параметров для подключения к БД
    /// </summary>
    public partial class DBDialog : Window
    {
        public DBDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Адрес
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Обработчик нажатия клавиши Connect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Host = hostBox.Text;
            this.User = userBox.Text;
            this.Password = passwordBox.Password;
            this.Close();
        }
    }
}
