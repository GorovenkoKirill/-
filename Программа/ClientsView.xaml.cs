using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GasCompanyApp
{
    public partial class ClientsView : UserControl
    {
        private int _userId;
        private bool _isEditMode = false;
        private int _currentClientId = 0;
        string connectionString = "Data Source=DESKTOP-A27DH7D\\SQLEXPRESS;Initial Catalog=GasCompanyDB;Integrated Security=True;TrustServerCertificate=True";

        public ClientsView(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadClients();
        }

        private void LoadClients(string searchTerm = "")
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT client_id, first_name, last_name, middle_name, phone, email, 
                                    registration_date, is_active 
                                    FROM Clients 
                                    WHERE @searchTerm = '' 
                                    OR last_name LIKE '%' + @searchTerm + '%' 
                                    OR first_name LIKE '%' + @searchTerm + '%' 
                                    OR phone LIKE '%' + @searchTerm + '%'";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@searchTerm", searchTerm ?? "");

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    clientsGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке клиентов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _isEditMode = false;
            _currentClientId = 0;
            ClearForm();
            editForm.Visibility = Visibility.Visible;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (clientsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _isEditMode = true;
            DataRowView row = (DataRowView)clientsGrid.SelectedItem;
            _currentClientId = (int)row["client_id"];

            txtLastName.Text = row["last_name"].ToString();
            txtFirstName.Text = row["first_name"].ToString();
            txtMiddleName.Text = row["middle_name"].ToString();
            txtPhone.Text = row["phone"].ToString();
            txtEmail.Text = row["email"].ToString();
            chkIsActive.IsChecked = (bool)row["is_active"];

            editForm.Visibility = Visibility.Visible;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (clientsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить этого клиента?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DataRowView row = (DataRowView)clientsGrid.SelectedItem;
                    int clientId = (int)row["client_id"];

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM Clients WHERE client_id = @clientId";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@clientId", clientId);
                        command.ExecuteNonQuery();

                        LogUserAction(_userId, "DELETE", "Clients", clientId);
                        LoadClients();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении клиента: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Заполните обязательные поля (Фамилия, Имя, Телефон)",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (_isEditMode)
                    {
                        // Обновление существующего клиента
                        string query = @"UPDATE Clients SET 
                                        last_name = @lastName, 
                                        first_name = @firstName, 
                                        middle_name = @middleName, 
                                        phone = @phone, 
                                        email = @email, 
                                        is_active = @isActive 
                                        WHERE client_id = @clientId";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@lastName", txtLastName.Text);
                        command.Parameters.AddWithValue("@firstName", txtFirstName.Text);
                        command.Parameters.AddWithValue("@middleName",
                            string.IsNullOrWhiteSpace(txtMiddleName.Text) ? DBNull.Value : (object)txtMiddleName.Text);
                        command.Parameters.AddWithValue("@phone", txtPhone.Text);
                        command.Parameters.AddWithValue("@email",
                            string.IsNullOrWhiteSpace(txtEmail.Text) ? DBNull.Value : (object)txtEmail.Text);
                        command.Parameters.AddWithValue("@isActive", chkIsActive.IsChecked ?? true);
                        command.Parameters.AddWithValue("@clientId", _currentClientId);

                        command.ExecuteNonQuery();
                        LogUserAction(_userId, "UPDATE", "Clients", _currentClientId);
                    }
                    else
                    {
                        // Добавление нового клиента
                        string query = @"INSERT INTO Clients 
                                        (last_name, first_name, middle_name, phone, email, 
                                        registration_date, is_active, created_by) 
                                        VALUES 
                                        (@lastName, @firstName, @middleName, @phone, @email, 
                                        GETDATE(), @isActive, @createdBy);
                                        SELECT SCOPE_IDENTITY();";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@lastName", txtLastName.Text);
                        command.Parameters.AddWithValue("@firstName", txtFirstName.Text);
                        command.Parameters.AddWithValue("@middleName",
                            string.IsNullOrWhiteSpace(txtMiddleName.Text) ? DBNull.Value : (object)txtMiddleName.Text);
                        command.Parameters.AddWithValue("@phone", txtPhone.Text);
                        command.Parameters.AddWithValue("@email",
                            string.IsNullOrWhiteSpace(txtEmail.Text) ? DBNull.Value : (object)txtEmail.Text);
                        command.Parameters.AddWithValue("@isActive", chkIsActive.IsChecked ?? true);
                        command.Parameters.AddWithValue("@createdBy", _userId);

                        int newId = Convert.ToInt32(command.ExecuteScalar());
                        LogUserAction(_userId, "CREATE", "Clients", newId);
                    }

                    editForm.Visibility = Visibility.Collapsed;
                    LoadClients();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении клиента: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            editForm.Visibility = Visibility.Collapsed;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadClients();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            LoadClients(txtSearch.Text);
        }

        private void ResetSearchButton_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            LoadClients();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadClients(txtSearch.Text);
            }
        }

        private void ClearForm()
        {
            txtLastName.Text = "";
            txtFirstName.Text = "";
            txtMiddleName.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
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

        private void clientsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Можно добавить дополнительную логику при выборе клиента
        }
    }
}