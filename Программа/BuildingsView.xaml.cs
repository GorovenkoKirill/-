using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GasCompanyApp
{
    public partial class BuildingsView : UserControl
    {
        private int _userId;
        private bool _isEditMode = false;
        private int _currentBuildingId = 0;
        string connectionString = "Data Source=DESKTOP-A27DH7D\\SQLEXPRESS;Initial Catalog=GasCompanyDB;Integrated Security=True;TrustServerCertificate=True";

        public BuildingsView(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadBuildings();
            LoadRegions();
        }

        private void LoadBuildings(string searchTerm = "")
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT b.building_id, r.region_name AS RegionName, 
                                           s.street_name AS StreetName, b.house_number, 
                                           b.building_letter, b.apartments_count
                                    FROM Buildings b
                                    JOIN Streets s ON b.street_id = s.street_id
                                    JOIN Regions r ON s.region_id = r.region_id
                                    WHERE @searchTerm = '' 
                                    OR s.street_name LIKE '%' + @searchTerm + '%' 
                                    OR b.house_number LIKE '%' + @searchTerm + '%' 
                                    OR r.region_name LIKE '%' + @searchTerm + '%'";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@searchTerm", searchTerm ?? "");

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    buildingsGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке зданий: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRegions()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT region_id, region_name FROM Regions";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    cmbRegions.ItemsSource = dt.DefaultView;
                    cmbRegions.SelectedValuePath = "region_id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке регионов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadStreets(int regionId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT street_id, street_name FROM Streets WHERE region_id = @regionId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@regionId", regionId);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    cmbStreets.ItemsSource = dt.DefaultView;
                    cmbStreets.SelectedValuePath = "street_id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке улиц: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmbRegions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbRegions.SelectedValue != null)
            {
                int regionId = (int)cmbRegions.SelectedValue;
                LoadStreets(regionId);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _isEditMode = false;
            _currentBuildingId = 0;
            ClearForm();
            editForm.Visibility = Visibility.Visible;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (buildingsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите здание для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _isEditMode = true;
            DataRowView row = (DataRowView)buildingsGrid.SelectedItem;
            _currentBuildingId = (int)row["building_id"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT b.street_id, s.region_id, b.house_number, 
                                            b.building_letter, b.apartments_count
                                     FROM Buildings b
                                     JOIN Streets s ON b.street_id = s.street_id
                                     WHERE building_id = @buildingId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@buildingId", _currentBuildingId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cmbRegions.SelectedValue = reader["region_id"];
                            cmbStreets.SelectedValue = reader["street_id"];
                            txtHouseNumber.Text = reader["house_number"].ToString();
                            txtBuildingLetter.Text = reader["building_letter"].ToString();
                            txtApartmentsCount.Text = reader["apartments_count"].ToString();
                        }
                    }
                }

                editForm.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных здания: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (buildingsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите здание для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить это здание?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DataRowView row = (DataRowView)buildingsGrid.SelectedItem;
                    int buildingId = (int)row["building_id"];

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM Buildings WHERE building_id = @buildingId";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@buildingId", buildingId);
                        command.ExecuteNonQuery();

                        LogUserAction(_userId, "DELETE", "Buildings", buildingId);
                        LoadBuildings();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении здания: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (cmbRegions.SelectedItem == null ||
                cmbStreets.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtHouseNumber.Text))
            {
                MessageBox.Show("Заполните все обязательные поля (Регион, Улица, Номер дома)",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtApartmentsCount.Text, out int apartmentsCount) || apartmentsCount < 0)
            {
                MessageBox.Show("Введите корректное количество квартир (целое неотрицательное число)",
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
                        // Обновление существующего здания
                        string query = @"UPDATE Buildings SET 
                                        street_id = @streetId, 
                                        house_number = @houseNumber, 
                                        building_letter = @buildingLetter, 
                                        apartments_count = @apartmentsCount 
                                        WHERE building_id = @buildingId";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@streetId", cmbStreets.SelectedValue);
                        command.Parameters.AddWithValue("@houseNumber", txtHouseNumber.Text);
                        command.Parameters.AddWithValue("@buildingLetter",
                            string.IsNullOrWhiteSpace(txtBuildingLetter.Text) ? DBNull.Value : (object)txtBuildingLetter.Text);
                        command.Parameters.AddWithValue("@apartmentsCount", apartmentsCount);
                        command.Parameters.AddWithValue("@buildingId", _currentBuildingId);

                        command.ExecuteNonQuery();
                        LogUserAction(_userId, "UPDATE", "Buildings", _currentBuildingId);
                    }
                    else
                    {
                        // Добавление нового здания
                        string query = @"INSERT INTO Buildings 
                                        (street_id, house_number, building_letter, apartments_count) 
                                        VALUES 
                                        (@streetId, @houseNumber, @buildingLetter, @apartmentsCount);
                                        SELECT SCOPE_IDENTITY();";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@streetId", cmbStreets.SelectedValue);
                        command.Parameters.AddWithValue("@houseNumber", txtHouseNumber.Text);
                        command.Parameters.AddWithValue("@buildingLetter",
                            string.IsNullOrWhiteSpace(txtBuildingLetter.Text) ? DBNull.Value : (object)txtBuildingLetter.Text);
                        command.Parameters.AddWithValue("@apartmentsCount", apartmentsCount);

                        int newId = Convert.ToInt32(command.ExecuteScalar());
                        LogUserAction(_userId, "CREATE", "Buildings", newId);
                    }

                    editForm.Visibility = Visibility.Collapsed;
                    LoadBuildings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении здания: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            editForm.Visibility = Visibility.Collapsed;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadBuildings();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            LoadBuildings(txtSearch.Text);
        }

        private void ResetSearchButton_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            LoadBuildings();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadBuildings(txtSearch.Text);
            }
        }

        private void ClearForm()
        {
            cmbRegions.SelectedIndex = -1;
            cmbStreets.ItemsSource = null;
            txtHouseNumber.Text = "";
            txtBuildingLetter.Text = "";
            txtApartmentsCount.Text = "0";
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

        private void buildingsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Можно добавить дополнительную логику при выборе здания
        }
    }
}