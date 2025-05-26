using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace GasCompanyApp
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            txtUsername.Focus();

        }

        string connectionString = "Data Source=DESKTOP-A27DH7D\\SQLEXPRESS;Initial Catalog=GasCompanyDB;Integrated Security=True;TrustServerCertificate=True";

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblErrorMessage.Text = "Введите имя пользователя и пароль";
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT user_id, role FROM SystemUsers WHERE username = @username AND password_hash = @password AND is_active = 1";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userId = reader.GetInt32(0);
                                string role = reader.GetString(1);

                                // Закрываем DataReader перед выполнением следующего запроса
                                reader.Close();

                                // Обновляем дату последнего входа
                                string updateQuery = "UPDATE SystemUsers SET last_login = GETDATE() WHERE user_id = @userId";
                                using (var updateCommand = new SqlCommand(updateQuery, connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@userId", userId);
                                    updateCommand.ExecuteNonQuery();
                                }

                                LogUserAction(userId, "LOGIN", "SystemUsers", userId);

                                this.Hide();

                                var mainWindow = new MainWindow(userId, role);
                                mainWindow.Show();

                                this.Close();
                            }
                            else
                            {
                                lblErrorMessage.Text = "Неверное имя пользователя или пароль";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подключении к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                // Логирование ошибки в лог файл или другую систему
                Console.WriteLine($"Ошибка при записи в лог: {ex.Message}");
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }
    }
}