using Microsoft.Win32;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GasCompanyApp
{
    public partial class AdminView : UserControl
    {
        private int _userId;
        private bool _isEditMode = false;
        private int _currentUserId = 0;
        string connectionString = "Data Source=DESKTOP-A27DH7D\\SQLEXPRESS;Initial Catalog=GasCompanyDB;Integrated Security=True;TrustServerCertificate=True";

        public AdminView(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadUsers();
        }

        private void LoadUsers(string searchTerm = "")
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT user_id, username, first_name, last_name, email, 
                                    role, is_active, last_login 
                                    FROM SystemUsers 
                                    WHERE @searchTerm = '' 
                                    OR username LIKE '%' + @searchTerm + '%' 
                                    OR last_name LIKE '%' + @searchTerm + '%' 
                                    OR first_name LIKE '%' + @searchTerm + '%' 
                                    OR email LIKE '%' + @searchTerm + '%'";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@searchTerm", searchTerm ?? "");

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    usersGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пользователей: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _isEditMode = false;
            _currentUserId = 0;
            ClearForm();
            editForm.Visibility = Visibility.Visible;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (usersGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _isEditMode = true;
            DataRowView row = (DataRowView)usersGrid.SelectedItem;
            _currentUserId = (int)row["user_id"];

            txtUsername.Text = row["username"].ToString();
            txtLastName.Text = row["last_name"].ToString();
            txtFirstName.Text = row["first_name"].ToString();
            txtEmail.Text = row["email"].ToString();

            // Установка роли
            string role = row["role"].ToString();
            foreach (ComboBoxItem item in cmbRole.Items)
            {
                if (item.Content.ToString() == role)
                {
                    cmbRole.SelectedItem = item;
                    break;
                }
            }

            chkIsActive.IsChecked = (bool)row["is_active"];
            editForm.Visibility = Visibility.Visible;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (usersGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить этого пользователя?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DataRowView row = (DataRowView)usersGrid.SelectedItem;
                    int userId = (int)row["user_id"];

                    // Проверка, что пользователь не удаляет сам себя
                    if (userId == _userId)
                    {
                        MessageBox.Show("Вы не можете удалить самого себя!", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM SystemUsers WHERE user_id = @userId";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@userId", userId);
                        command.ExecuteNonQuery();

                        LogUserAction(_userId, "DELETE", "SystemUsers", userId);
                        LoadUsers();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                cmbRole.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Для нового пользователя проверяем пароль
            if (!_isEditMode && string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Введите пароль для нового пользователя", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (_isEditMode)
                    {
                        // Обновление существующего пользователя
                        string query = @"UPDATE SystemUsers SET 
                                        username = @username, 
                                        first_name = @firstName, 
                                        last_name = @lastName, 
                                        email = @email, 
                                        role = @role, 
                                        is_active = @isActive,
                                        modified_at = GETDATE()";

                        // Добавляем обновление пароля, только если он введен
                        if (!string.IsNullOrWhiteSpace(txtPassword.Password))
                        {
                            query += ", password_hash = @password";
                        }

                        query += " WHERE user_id = @userId";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@username", txtUsername.Text);
                        command.Parameters.AddWithValue("@firstName", txtFirstName.Text);
                        command.Parameters.AddWithValue("@lastName", txtLastName.Text);
                        command.Parameters.AddWithValue("@email", txtEmail.Text);
                        command.Parameters.AddWithValue("@role", ((ComboBoxItem)cmbRole.SelectedItem).Content.ToString());
                        command.Parameters.AddWithValue("@isActive", chkIsActive.IsChecked ?? true);
                        command.Parameters.AddWithValue("@userId", _currentUserId);

                        if (!string.IsNullOrWhiteSpace(txtPassword.Password))
                        {
                            command.Parameters.AddWithValue("@password", txtPassword.Password);
                        }

                        command.ExecuteNonQuery();
                        LogUserAction(_userId, "UPDATE", "SystemUsers", _currentUserId);
                    }
                    else
                    {
                        // Добавление нового пользователя
                        string query = @"INSERT INTO SystemUsers 
                                        (username, password_hash, first_name, last_name, email, 
                                        role, is_active, created_at) 
                                        VALUES 
                                        (@username, @password, @firstName, @lastName, @email, 
                                        @role, @isActive, GETDATE());
                                        SELECT SCOPE_IDENTITY();";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@username", txtUsername.Text);
                        command.Parameters.AddWithValue("@password", txtPassword.Password);
                        command.Parameters.AddWithValue("@firstName", txtFirstName.Text);
                        command.Parameters.AddWithValue("@lastName", txtLastName.Text);
                        command.Parameters.AddWithValue("@email", txtEmail.Text);
                        command.Parameters.AddWithValue("@role", ((ComboBoxItem)cmbRole.SelectedItem).Content.ToString());
                        command.Parameters.AddWithValue("@isActive", chkIsActive.IsChecked ?? true);

                        int newId = Convert.ToInt32(command.ExecuteScalar());
                        LogUserAction(_userId, "CREATE", "SystemUsers", newId);
                    }

                    editForm.Visibility = Visibility.Collapsed;
                    LoadUsers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении пользователя: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            editForm.Visibility = Visibility.Collapsed;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers(txtSearch.Text);
        }

        private void ResetSearchButton_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            LoadUsers();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadUsers(txtSearch.Text);
            }
        }

        private void ClearForm()
        {
            txtUsername.Text = "";
            txtPassword.Password = "";
            txtLastName.Text = "";
            txtFirstName.Text = "";
            txtEmail.Text = "";
            cmbRole.SelectedIndex = 0;
            chkIsActive.IsChecked = true;
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

        private void usersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Можно добавить дополнительную логику при выборе пользователя
        }
        private void BackupButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Диалог для выбора места сохранения резервной копии
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Backup files (*.bak)|*.bak",
                    Title = "Создание резервной копии базы данных",
                    FileName = $"GasCompanyDB_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string backupPath = saveFileDialog.FileName;

                    // Выполнение резервного копирования
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string backupQuery = $"BACKUP DATABASE [GasCompanyDB] TO DISK = '{backupPath}' WITH FORMAT, MEDIANAME = 'GasCompanyBackup', NAME = 'Полная резервная копия GasCompanyDB';";

                        using (SqlCommand command = new SqlCommand(backupQuery, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show($"Резервная копия успешно создана: {backupPath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Логирование действия
                    LogUserAction(_userId, "BACKUP", "Database", 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании резервной копии: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}