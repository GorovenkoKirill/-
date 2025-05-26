using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GasCompanyApp
{
    public partial class MeterReadingsView : UserControl
    {
        private int _userId;
        private bool _isEditMode = false;
        private int _currentReadingId = 0;
        private decimal _previousReading = 0;
        string connectionString = "Data Source=DESKTOP-A27DH7D\\SQLEXPRESS;Initial Catalog=GasCompanyDB;Integrated Security=True;TrustServerCertificate=True";

        public MeterReadingsView(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadReadings();
            LoadMeters();
            ConfigureButtonsForRole();
        }

        private void ConfigureButtonsForRole()
        {
            // Для техника показываем кнопку подтверждения
            // Для других ролей скрываем
            btnVerify.Visibility = Visibility.Collapsed;

            // Здесь можно добавить логику для разных ролей
            // Например, если пользователь техник, показываем кнопку подтверждения
            // if (_userRole == "Техник") { btnVerify.Visibility = Visibility.Visible; }
        }

        private void LoadReadings(string searchTerm = "")
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT r.reading_id, 
                                           mt.type_name + ' (№' + gm.serial_number + ')' AS MeterInfo,
                                           r.reading_date, r.current_reading, r.consumption, 
                                           r.is_verified, r.verification_date,
                                           su.last_name + ' ' + su.first_name AS VerifiedBy
                                    FROM MeterReadings r
                                    JOIN GasMeters gm ON r.meter_id = gm.meter_id
                                    JOIN MeterTypes mt ON gm.meter_type_id = mt.meter_type_id
                                    LEFT JOIN SystemUsers su ON r.verified_by = su.user_id
                                    WHERE @searchTerm = '' 
                                    OR gm.serial_number LIKE '%' + @searchTerm + '%' 
                                    OR mt.type_name LIKE '%' + @searchTerm + '%' 
                                    ORDER BY r.reading_date DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@searchTerm", searchTerm ?? "");

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    readingsGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке показаний: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadMeters()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT gm.meter_id, 
                                           mt.type_name + ' (№' + gm.serial_number + ')' AS MeterInfo
                                    FROM GasMeters gm
                                    JOIN MeterTypes mt ON gm.meter_type_id = mt.meter_type_id
                                    WHERE gm.is_active = 1";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    cmbMeters.ItemsSource = dt.DefaultView;
                    cmbMeters.SelectedValuePath = "meter_id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке счетчиков: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _isEditMode = false;
            _currentReadingId = 0;
            ClearForm();
            editForm.Visibility = Visibility.Visible;
            dpReadingDate.SelectedDate = DateTime.Today;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (readingsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите показание для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _isEditMode = true;
            DataRowView row = (DataRowView)readingsGrid.SelectedItem;
            _currentReadingId = (int)row["reading_id"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT meter_id, reading_date, current_reading, consumption FROM MeterReadings WHERE reading_id = @readingId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@readingId", _currentReadingId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cmbMeters.SelectedValue = reader["meter_id"];
                            dpReadingDate.SelectedDate = (DateTime)reader["reading_date"];
                            txtCurrentReading.Text = reader["current_reading"].ToString();
                            txtConsumption.Text = reader["consumption"].ToString();
                        }
                    }
                }

                editForm.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных показания: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            if (readingsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите показание для подтверждения", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Подтвердить выбранные показания?",
                "Подтверждение показаний", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DataRowView row = (DataRowView)readingsGrid.SelectedItem;
                    int readingId = (int)row["reading_id"];

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = @"UPDATE MeterReadings SET 
                                        is_verified = 1, 
                                        verification_date = GETDATE(), 
                                        verified_by = @userId 
                                        WHERE reading_id = @readingId";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@readingId", readingId);
                        command.Parameters.AddWithValue("@userId", _userId);
                        command.ExecuteNonQuery();

                        LogUserAction(_userId, "UPDATE", "MeterReadings", readingId);
                        LoadReadings();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при подтверждении показаний: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (readingsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите показание для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить это показание?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DataRowView row = (DataRowView)readingsGrid.SelectedItem;
                    int readingId = (int)row["reading_id"];

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM MeterReadings WHERE reading_id = @readingId";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@readingId", readingId);
                        command.ExecuteNonQuery();

                        LogUserAction(_userId, "DELETE", "MeterReadings", readingId);
                        LoadReadings();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении показания: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (cmbMeters.SelectedItem == null ||
                dpReadingDate.SelectedDate == null ||
                string.IsNullOrWhiteSpace(txtCurrentReading.Text))
            {
                MessageBox.Show("Заполните все обязательные поля (Счетчик, Дата показания, Текущее показание)",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtCurrentReading.Text, out decimal currentReading) || currentReading < 0)
            {
                MessageBox.Show("Введите корректное текущее показание (положительное число)",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Получаем предыдущее показание для расчета расхода
                    decimal consumption = 0;
                    if (_isEditMode)
                    {
                        // Для редактирования берем предыдущее показание из базы
                        string prevQuery = @"SELECT TOP 1 current_reading 
                                            FROM MeterReadings 
                                            WHERE meter_id = @meterId 
                                            AND reading_date < @readingDate
                                            ORDER BY reading_date DESC";
                        SqlCommand prevCommand = new SqlCommand(prevQuery, connection);
                        prevCommand.Parameters.AddWithValue("@meterId", cmbMeters.SelectedValue);
                        prevCommand.Parameters.AddWithValue("@readingDate", dpReadingDate.SelectedDate);

                        object prevReading = prevCommand.ExecuteScalar();
                        if (prevReading != null)
                        {
                            consumption = currentReading - Convert.ToDecimal(prevReading);
                        }
                    }
                    else
                    {
                        // Для нового показания берем предыдущее показание из базы
                        string prevQuery = @"SELECT TOP 1 current_reading 
                                            FROM MeterReadings 
                                            WHERE meter_id = @meterId
                                            ORDER BY reading_date DESC";
                        SqlCommand prevCommand = new SqlCommand(prevQuery, connection);
                        prevCommand.Parameters.AddWithValue("@meterId", cmbMeters.SelectedValue);

                        object prevReading = prevCommand.ExecuteScalar();
                        if (prevReading != null)
                        {
                            consumption = currentReading - Convert.ToDecimal(prevReading);
                        }
                        else
                        {
                            // Если это первое показание для счетчика, берем начальное значение
                            string initialQuery = "SELECT initial_reading FROM GasMeters WHERE meter_id = @meterId";
                            SqlCommand initialCommand = new SqlCommand(initialQuery, connection);
                            initialCommand.Parameters.AddWithValue("@meterId", cmbMeters.SelectedValue);

                            object initialReading = initialCommand.ExecuteScalar();
                            if (initialReading != null)
                            {
                                consumption = currentReading - Convert.ToDecimal(initialReading);
                            }
                        }
                    }

                    if (_isEditMode)
                    {
                        // Обновление существующего показания
                        string query = @"UPDATE MeterReadings SET 
                                        meter_id = @meterId, 
                                        reading_date = @readingDate, 
                                        current_reading = @currentReading, 
                                        consumption = @consumption,
                                        is_verified = 0,
                                        verification_date = NULL,
                                        verified_by = NULL
                                        WHERE reading_id = @readingId";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@meterId", cmbMeters.SelectedValue);
                        command.Parameters.AddWithValue("@readingDate", dpReadingDate.SelectedDate);
                        command.Parameters.AddWithValue("@currentReading", currentReading);
                        command.Parameters.AddWithValue("@consumption", consumption);
                        command.Parameters.AddWithValue("@readingId", _currentReadingId);

                        command.ExecuteNonQuery();
                        LogUserAction(_userId, "UPDATE", "MeterReadings", _currentReadingId);
                    }
                    else
                    {
                        // Добавление нового показания
                        string query = @"INSERT INTO MeterReadings 
                                        (meter_id, reading_date, current_reading, consumption, is_verified) 
                                        VALUES 
                                        (@meterId, @readingDate, @currentReading, @consumption, 0);
                                        SELECT SCOPE_IDENTITY();";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@meterId", cmbMeters.SelectedValue);
                        command.Parameters.AddWithValue("@readingDate", dpReadingDate.SelectedDate);
                        command.Parameters.AddWithValue("@currentReading", currentReading);
                        command.Parameters.AddWithValue("@consumption", consumption);

                        int newId = Convert.ToInt32(command.ExecuteScalar());
                        LogUserAction(_userId, "CREATE", "MeterReadings", newId);
                    }

                    editForm.Visibility = Visibility.Collapsed;
                    LoadReadings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении показания: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            editForm.Visibility = Visibility.Collapsed;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadReadings();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            LoadReadings(txtSearch.Text);
        }

        private void ResetSearchButton_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            LoadReadings();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadReadings(txtSearch.Text);
            }
        }

        private void ClearForm()
        {
            cmbMeters.SelectedIndex = -1;
            dpReadingDate.SelectedDate = null;
            txtCurrentReading.Text = "";
            txtConsumption.Text = "";
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

        private void readingsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Можно добавить дополнительную логику при выборе показания
        }
    }
}