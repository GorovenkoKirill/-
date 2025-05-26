using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GasCompanyApp
{
    public partial class ApartmentsView : UserControl
    {
        private int _userId;
        private bool _isEditMode = false;
        private int _currentApartmentId = 0;
        string connectionString = "Data Source=DESKTOP-A27DH7D\\SQLEXPRESS;Initial Catalog=GasCompanyDB;Integrated Security=True;TrustServerCertificate=True";

        public ApartmentsView(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadApartments();
            LoadBuildings();
            LoadClients();
        }

        private void LoadApartments(string searchTerm = "")
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT a.apartment_id, 
                                           CAST(b.street_id AS NVARCHAR(10)) + '-' + b.house_number + ISNULL(b.building_letter, '') AS BuildingInfo,
                                           a.apartment_number, a.area, a.resident_count,
                                           c.last_name + ' ' + c.first_name + ISNULL(' ' + c.middle_name, '') AS ClientName
                                    FROM Apartments a
                                    JOIN Buildings b ON a.building_id = b.building_id
                                    JOIN Clients c ON a.client_id = c.client_id
                                    WHERE @searchTerm = '' 
                                    OR a.apartment_number LIKE '%' + @searchTerm + '%' 
                                    OR c.last_name LIKE '%' + @searchTerm + '%' 
                                    OR c.first_name LIKE '%' + @searchTerm + '%'";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@searchTerm", searchTerm ?? "");

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    apartmentsGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке квартир: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadBuildings()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT b.building_id, 
                                    s.street_name + ', ' + b.house_number + ISNULL(b.building_letter, '') AS BuildingInfo
                                    FROM Buildings b
                                    JOIN Streets s ON b.street_id = s.street_id";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    cmbBuildings.ItemsSource = dt.DefaultView;
                    cmbBuildings.SelectedValuePath = "building_id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке зданий: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadClients()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT client_id, 
                                    last_name + ' ' + first_name + ISNULL(' ' + middle_name, '') AS ClientName
                                    FROM Clients
                                    WHERE is_active = 1";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    cmbClients.ItemsSource = dt.DefaultView;
                    cmbClients.SelectedValuePath = "client_id";
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
            _currentApartmentId = 0;
            ClearForm();
            editForm.Visibility = Visibility.Visible;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (apartmentsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите квартиру для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _isEditMode = true;
            DataRowView row = (DataRowView)apartmentsGrid.SelectedItem;
            _currentApartmentId = (int)row["apartment_id"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT building_id, apartment_number, area, resident_count, client_id FROM Apartments WHERE apartment_id = @apartmentId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@apartmentId", _currentApartmentId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cmbBuildings.SelectedValue = reader["building_id"];
                            txtApartmentNumber.Text = reader["apartment_number"].ToString();
                            txtArea.Text = reader["area"].ToString();
                            txtResidentCount.Text = reader["resident_count"].ToString();
                            cmbClients.SelectedValue = reader["client_id"];
                        }
                    }
                }

                editForm.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных квартиры: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (apartmentsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите квартиру для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить эту квартиру?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DataRowView row = (DataRowView)apartmentsGrid.SelectedItem;
                    int apartmentId = (int)row["apartment_id"];

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM Apartments WHERE apartment_id = @apartmentId";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@apartmentId", apartmentId);
                        command.ExecuteNonQuery();

                        LogUserAction(_userId, "DELETE", "Apartments", apartmentId);
                        LoadApartments();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении квартиры: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (cmbBuildings.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtApartmentNumber.Text) ||
                string.IsNullOrWhiteSpace(txtArea.Text) ||
                string.IsNullOrWhiteSpace(txtResidentCount.Text) ||
                cmbClients.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtArea.Text, out decimal area) || area <= 0)
            {
                MessageBox.Show("Введите корректное значение площади (положительное число)",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtResidentCount.Text, out int residentCount) || residentCount <= 0)
            {
                MessageBox.Show("Введите корректное количество жильцов (целое положительное число)",
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
                        // Обновление существующей квартиры
                        string query = @"UPDATE Apartments SET 
                                        building_id = @buildingId, 
                                        apartment_number = @apartmentNumber, 
                                        area = @area, 
                                        resident_count = @residentCount, 
                                        client_id = @clientId 
                                        WHERE apartment_id = @apartmentId";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@buildingId", cmbBuildings.SelectedValue);
                        command.Parameters.AddWithValue("@apartmentNumber", txtApartmentNumber.Text);
                        command.Parameters.AddWithValue("@area", area);
                        command.Parameters.AddWithValue("@residentCount", residentCount);
                        command.Parameters.AddWithValue("@clientId", cmbClients.SelectedValue);
                        command.Parameters.AddWithValue("@apartmentId", _currentApartmentId);

                        command.ExecuteNonQuery();
                        LogUserAction(_userId, "UPDATE", "Apartments", _currentApartmentId);
                    }
                    else
                    {
                        // Добавление новой квартиры
                        string query = @"INSERT INTO Apartments 
                                        (building_id, apartment_number, area, resident_count, client_id) 
                                        VALUES 
                                        (@buildingId, @apartmentNumber, @area, @residentCount, @clientId);
                                        SELECT SCOPE_IDENTITY();";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@buildingId", cmbBuildings.SelectedValue);
                        command.Parameters.AddWithValue("@apartmentNumber", txtApartmentNumber.Text);
                        command.Parameters.AddWithValue("@area", area);
                        command.Parameters.AddWithValue("@residentCount", residentCount);
                        command.Parameters.AddWithValue("@clientId", cmbClients.SelectedValue);

                        int newId = Convert.ToInt32(command.ExecuteScalar());
                        LogUserAction(_userId, "CREATE", "Apartments", newId);
                    }

                    editForm.Visibility = Visibility.Collapsed;
                    LoadApartments();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении квартиры: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            editForm.Visibility = Visibility.Collapsed;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadApartments();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            LoadApartments(txtSearch.Text);
        }

        private void ResetSearchButton_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            LoadApartments();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadApartments(txtSearch.Text);
            }
        }

        private void ClearForm()
        {
            txtApartmentNumber.Text = "";
            txtArea.Text = "";
            txtResidentCount.Text = "1";
            cmbBuildings.SelectedIndex = -1;
            cmbClients.SelectedIndex = -1;
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

        private void apartmentsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Можно добавить дополнительную логику при выборе квартиры
        }
    }
}