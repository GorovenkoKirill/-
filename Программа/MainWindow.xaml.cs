using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace GasCompanyApp
{
    public partial class MainWindow : Window
    {
        private int _userId;
        private string _userRole;

        public MainWindow(int userId, string userRole)
        {
            InitializeComponent();
            _userId = userId;
            _userRole = userRole;


            // Установка контекста данных для отображения текущего пользователя
            this.DataContext = new { CurrentUser = $"Пользователь: {_userRole}" };

            // Настройка видимости кнопок в зависимости от роли
            ConfigureMenuForRole();

        }
        private void ShowClients_Click(object sender, RoutedEventArgs e)
        {
            mainContent.Content = new ClientsView(_userId);
        }

        string connectionString = "Data Source=DESKTOP-A27DH7D\\SQLEXPRESS;Initial Catalog=GasCompanyDB;Integrated Security=True;TrustServerCertificate=True";
        private void ConfigureMenuForRole()
        {
            // Скрываем все кнопки сначала
            btnClients.Visibility = Visibility.Collapsed;
            btnApartments.Visibility = Visibility.Collapsed;
            btnBuildings.Visibility = Visibility.Collapsed;
            btnMeters.Visibility = Visibility.Collapsed;
            btnReadings.Visibility = Visibility.Collapsed;
            btnInvoices.Visibility = Visibility.Collapsed;
            btnPayments.Visibility = Visibility.Collapsed;
            btnReports.Visibility = Visibility.Collapsed;
            btnAdmin.Visibility = Visibility.Collapsed;

            // Включаем кнопки в зависимости от роли
            switch (_userRole)
            {
                case "Администратор":
                    btnClients.Visibility = Visibility.Visible;
                    btnApartments.Visibility = Visibility.Visible;
                    btnBuildings.Visibility = Visibility.Visible;
                    btnMeters.Visibility = Visibility.Visible;
                    btnReadings.Visibility = Visibility.Visible;
                    btnInvoices.Visibility = Visibility.Visible;
                    btnPayments.Visibility = Visibility.Visible;
                    btnReports.Visibility = Visibility.Visible;
                    btnAdmin.Visibility = Visibility.Visible;
                    break;
                case "Менеджер":
                    btnClients.Visibility = Visibility.Visible;
                    btnApartments.Visibility = Visibility.Visible;
                    btnBuildings.Visibility = Visibility.Visible;
                    btnReports.Visibility = Visibility.Visible;
                    break;
                case "Техник":
                    btnMeters.Visibility = Visibility.Visible;
                    btnReadings.Visibility = Visibility.Visible;
                    break;
                case "Бухгалтер":
                    btnInvoices.Visibility = Visibility.Visible;
                    btnPayments.Visibility = Visibility.Visible;
                    btnReports.Visibility = Visibility.Visible;
                    break;
                case "Оператор":
                    btnClients.Visibility = Visibility.Visible;
                    btnApartments.Visibility = Visibility.Visible;
                    btnInvoices.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void NavigateButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string pageTag = button.Tag.ToString();
            NavigateToPage(pageTag);
        }

        private void NavigateToPage(string pageTag)
        {
            try
            {
                UserControl control = null;

                switch (pageTag)
                {
                    case "Clients":
                        control = new ClientsView(_userId);
                        break;
                    case "Apartments":
                        control = new ApartmentsView(_userId);
                        break;
                    case "Buildings":
                        control = new BuildingsView(_userId);
                        break;
                    case "Meters":
                        control = new MetersView(_userId);
                        break;
                    case "Readings":
                        control = new MeterReadingsView(_userId);
                        break;
                    case "Invoices":
                        control = new InvoicesView(_userId);
                        break;
                    case "Payments":
                        control = new PaymentsView(_userId);
                        break;
                    case "Reports":
                        control = new ReportsView(_userId);
                        break;
                    case "Admin":
                        control = new AdminView(_userId);
                        break;
                    default:
                        return;
                }

                if (control != null)
                {
                    mainContent.Content = control;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии страницы: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Логирование выхода
            LogUserAction(_userId, "LOGOUT", "SystemUsers", _userId);

            // Открываем окно авторизации
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Логирование выхода при закрытии окна
            LogUserAction(_userId, "LOGOUT", "SystemUsers", _userId);
        }

        private void LogUserAction(int userId, string actionType, string tableName, int recordId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO AuditLog (user_id, action_type, table_name, record_id, action_date)
                                    VALUES (@userId, @actionType, @tableName, @recordId, GETDATE())";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@actionType", actionType);
                        command.Parameters.AddWithValue("@tableName", tableName);
                        command.Parameters.AddWithValue("@recordId", recordId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи в лог: {ex.Message}");
            }
        }
    }
}