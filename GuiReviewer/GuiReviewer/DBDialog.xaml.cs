using System;
using System.Windows;


namespace GuiReviewer
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
        /// Наименование БД
        /// </summary>
        public string Base { get; set; }

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
            this.Base = baseBox.Text;
            this.User = userBox.Text;
            this.Password = passwordBox.Password;
            this.Close();
        }
    }
}
