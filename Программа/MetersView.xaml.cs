using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GasCompanyApp
{
    public partial class MetersView : UserControl
    {
        private int _userId;
        private bool _isEditMode = false;
        private int _currentMeterId = 0;
        string connectionString = "Data Source=DESKTOP-A27DH7D\\SQLEXPRESS;Initial Catalog=GasCompanyDB;Integrated Security=True;TrustServerCertificate=True";

        public MetersView(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadMeters();
            LoadApartments();
            LoadMeterTypes();
        }

        private void LoadMeters(string searchTerm = "")
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT m.meter_id, 
                                           a.apartment_number + ', ' + b.house_number + ', ' + s.street_name AS ApartmentInfo,
                                           mt.type_name AS MeterTypeName, m.serial_number, 
                                           m.installation_date, m.initial_reading, 
                                           m.last_verification_date, m.next_verification_date, 
                                           m.is_active
                                    FROM GasMeters m
                                    JOIN Apartments a ON m.apartment_id = a.apartment_id
                                    JOIN Buildings b ON a.building_id = b.building_id
                                    JOIN Streets s ON b.street_id = s.street_id
                                    JOIN MeterTypes mt ON m.meter_type_id = mt.meter_type_id
                                    WHERE @searchTerm = '' 
                                    OR m.serial_number LIKE '%' + @searchTerm + '%' 
                                    OR mt.type_name LIKE '%' + @searchTerm + '%' 
                                    OR a.apartment_number LIKE '%' + @searchTerm + '%'";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@searchTerm", searchTerm ?? "");

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    metersGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке счетчиков: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadApartments()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT a.apartment_id, 
                                           a.apartment_number + ', ' + b.house_number + ', ' + s.street_name AS ApartmentInfo
                                    FROM Apartments a
                                    JOIN Buildings b ON a.building_id = b.building_id
                                    JOIN Streets s ON b.street_id = s.street_id";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    cmbApartments.ItemsSource = dt.DefaultView;
                    cmbApartments.SelectedValuePath = "apartment_id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке квартир: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadMeterTypes()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT meter_type_id, type_name FROM MeterTypes";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    cmbMeterTypes.ItemsSource = dt.DefaultView;
                    cmbMeterTypes.SelectedValuePath = "meter_type_id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке типов счетчиков: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _isEditMode = false;
            _currentMeterId = 0;
            ClearForm();
            editForm.Visibility = Visibility.Visible;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (metersGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите счетчик для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _isEditMode = true;
            DataRowView row = (DataRowView)metersGrid.SelectedItem;
            _currentMeterId = (int)row["meter_id"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT apartment_id, meter_type_id, serial_number, 
                                             installation_date, initial_reading, 
                                             last_verification_date, next_verification_date, 
                                             is_active 
                                      FROM GasMeters 
                                      WHERE meter_id = @meterId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@meterId", _currentMeterId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cmbApartments.SelectedValue = reader["apartment_id"];
                            cmbMeterTypes.SelectedValue = reader["meter_type_id"];
                            txtSerialNumber.Text = reader["serial_number"].ToString();
                            dpInstallationDate.SelectedDate = reader.GetDateTime(reader.GetOrdinal("installation_date"));
                            txtInitialReading.Text = reader["initial_reading"].ToString();
                            dpLastVerificationDate.SelectedDate = reader.GetDateTime(reader.GetOrdinal("last_verification_date"));
                            dpNextVerificationDate.SelectedDate = reader.GetDateTime(reader.GetOrdinal("next_verification_date"));
                            chkIsActive.IsChecked = (bool)reader["is_active"];
                        }
                    }
                }

                editForm.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных счетчика: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (metersGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите счетчик для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить этот счетчик?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DataRowView row = (DataRowView)metersGrid.SelectedItem;
                    int meterId = (int)row["meter_id"];

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM GasMeters WHERE meter_id = @meterId";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@meterId", meterId);
                        command.ExecuteNonQuery();

                        LogUserAction(_userId, "DELETE", "GasMeters", meterId);
                        LoadMeters();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении счетчика: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (cmbApartments.SelectedItem == null ||
                cmbMeterTypes.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtSerialNumber.Text) ||
                dpInstallationDate.SelectedDate == null ||
                string.IsNullOrWhiteSpace(txtInitialReading.Text) ||
                dpLastVerificationDate.SelectedDate == null ||
                dpNextVerificationDate.SelectedDate == null)
            {
                MessageBox.Show("Заполните все обязательные поля",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtInitialReading.Text, out decimal initialReading) || initialReading < 0)
            {
                MessageBox.Show("Введите корректное начальное показание (положительное число)",
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
                        // Обновление существующего счетчика
                        string query = @"UPDATE GasMeters SET 
                                        apartment_id = @apartmentId, 
                                        meter_type_id = @meterTypeId, 
                                        serial_number = @serialNumber, 
                                        installation_date = @installationDate, 
                                        initial_reading = @initialReading, 
                                        last_verification_date = @lastVerificationDate, 
                                        next_verification_date = @nextVerificationDate, 
                                        is_active = @isActive 
                                        WHERE meter_id = @meterId";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@apartmentId", cmbApartments.SelectedValue);
                        command.Parameters.AddWithValue("@meterTypeId", cmbMeterTypes.SelectedValue);
                        command.Parameters.AddWithValue("@serialNumber", txtSerialNumber.Text);
                        command.Parameters.AddWithValue("@installationDate", dpInstallationDate.SelectedDate);
                        command.Parameters.AddWithValue("@initialReading", initialReading);
                        command.Parameters.AddWithValue("@lastVerificationDate", dpLastVerificationDate.SelectedDate);
                        command.Parameters.AddWithValue("@nextVerificationDate", dpNextVerificationDate.SelectedDate);
                        command.Parameters.AddWithValue("@isActive", chkIsActive.IsChecked ?? true);
                        command.Parameters.AddWithValue("@meterId", _currentMeterId);

                        command.ExecuteNonQuery();
                        LogUserAction(_userId, "UPDATE", "GasMeters", _currentMeterId);
                    }
                    else
                    {
                        // Добавление нового счетчика
                        string query = @"INSERT INTO GasMeters 
                                        (apartment_id, meter_type_id, serial_number, 
                                        installation_date, initial_reading, 
                                        last_verification_date, next_verification_date, 
                                        is_active) 
                                        VALUES 
                                        (@apartmentId, @meterTypeId, @serialNumber, 
                                        @installationDate, @initialReading, 
                                        @lastVerificationDate, @nextVerificationDate, 
                                        @isActive);
                                        SELECT SCOPE_IDENTITY();";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@apartmentId", cmbApartments.SelectedValue);
                        command.Parameters.AddWithValue("@meterTypeId", cmbMeterTypes.SelectedValue);
                        command.Parameters.AddWithValue("@serialNumber", txtSerialNumber.Text);
                        command.Parameters.AddWithValue("@installationDate", dpInstallationDate.SelectedDate);
                        command.Parameters.AddWithValue("@initialReading", initialReading);
                        command.Parameters.AddWithValue("@lastVerificationDate", dpLastVerificationDate.SelectedDate);
                        command.Parameters.AddWithValue("@nextVerificationDate", dpNextVerificationDate.SelectedDate);
                        command.Parameters.AddWithValue("@isActive", chkIsActive.IsChecked ?? true);

                        int newId = Convert.ToInt32(command.ExecuteScalar());
                        LogUserAction(_userId, "CREATE", "GasMeters", newId);
                    }

                    editForm.Visibility = Visibility.Collapsed;
                    LoadMeters();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении счетчика: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            editForm.Visibility = Visibility.Collapsed;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadMeters();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            LoadMeters(txtSearch.Text);
        }

        private void ResetSearchButton_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            LoadMeters();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadMeters(txtSearch.Text);
            }
        }

        private void ClearForm()
        {
            cmbApartments.SelectedIndex = -1;
            cmbMeterTypes.SelectedIndex = -1;
            txtSerialNumber.Text = "";
            dpInstallationDate.SelectedDate = DateTime.Today;
            txtInitialReading.Text = "0";
            dpLastVerificationDate.SelectedDate = DateTime.Today;
            dpNextVerificationDate.SelectedDate = DateTime.Today.AddYears(1);
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

        private void metersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Можно добавить дополнительную логику при выборе счетчика
        }
    }
}