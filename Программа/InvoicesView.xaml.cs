using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace GasCompanyApp
{
    public partial class InvoicesView : UserControl
    {
        private int _userId;
        private bool _isEditMode = false;
        private int _currentInvoiceId = 0;
        string connectionString = "Data Source=DESKTOP-A27DH7D\\SQLEXPRESS;Initial Catalog=GasCompanyDB;Integrated Security=True;TrustServerCertificate=True";

        public InvoicesView(int userId)
        {
            InitializeComponent();
            _userId = userId;
            dpPeriod.SelectedDate = DateTime.Now;
            LoadInvoices();
            LoadApartments();
            LoadTariffs();
        }

        private void LoadInvoices(string searchTerm = "", DateTime? period = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT i.invoice_id, 
                                           a.apartment_number + ', ' + b.house_number + ISNULL(b.building_letter, '') + ', ' + s.street_name AS ApartmentInfo,
                                           i.period, t.tariff_name AS TariffName, 
                                           i.consumption, i.amount, i.issue_date, i.due_date, i.status
                                    FROM Invoices i
                                    JOIN Apartments a ON i.apartment_id = a.apartment_id
                                    JOIN Buildings b ON a.building_id = b.building_id
                                    JOIN Streets s ON b.street_id = s.street_id
                                    JOIN Tariffs t ON i.tariff_id = t.tariff_id
                                    WHERE (@searchTerm = '' 
                                    OR a.apartment_number LIKE '%' + @searchTerm + '%'
                                    OR b.house_number LIKE '%' + @searchTerm + '%'
                                    OR s.street_name LIKE '%' + @searchTerm + '%')
                                    AND (@period IS NULL OR CONVERT(varchar(7), i.period, 126) = CONVERT(varchar(7), @period, 126))";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@searchTerm", searchTerm ?? "");
                    command.Parameters.AddWithValue("@period", period ?? (object)DBNull.Value);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    invoicesGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке счетов: {ex.Message}", "Ошибка",
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
                                           a.apartment_number + ', ' + b.house_number + ISNULL(b.building_letter, '') + ', ' + s.street_name AS ApartmentInfo
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

        private void LoadTariffs()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT tariff_id, tariff_name, rate 
                                    FROM Tariffs 
                                    WHERE end_date IS NULL OR end_date >= GETDATE()";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    cmbTariffs.ItemsSource = dt.DefaultView;
                    cmbTariffs.SelectedValuePath = "tariff_id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке тарифов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _isEditMode = false;
            _currentInvoiceId = 0;
            ClearForm();
            dpInvoicePeriod.SelectedDate = DateTime.Now;
            editForm.Visibility = Visibility.Visible;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (invoicesGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите счет для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _isEditMode = true;
            DataRowView row = (DataRowView)invoicesGrid.SelectedItem;
            _currentInvoiceId = (int)row["invoice_id"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT apartment_id, period, tariff_id, 
                                           consumption, amount, status 
                                    FROM Invoices 
                                    WHERE invoice_id = @invoiceId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@invoiceId", _currentInvoiceId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cmbApartments.SelectedValue = reader["apartment_id"];
                            dpInvoicePeriod.SelectedDate = (DateTime)reader["period"];
                            cmbTariffs.SelectedValue = reader["tariff_id"];
                            txtConsumption.Text = reader["consumption"].ToString();

                            // Установка статуса
                            string status = reader["status"].ToString();
                            foreach (ComboBoxItem item in cmbStatus.Items)
                            {
                                if (item.Content.ToString() == status)
                                {
                                    cmbStatus.SelectedItem = item;
                                    break;
                                }
                            }
                        }
                    }
                }

                editForm.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных счета: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (invoicesGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите счет для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить этот счет?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DataRowView row = (DataRowView)invoicesGrid.SelectedItem;
                    int invoiceId = (int)row["invoice_id"];

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM Invoices WHERE invoice_id = @invoiceId";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@invoiceId", invoiceId);
                        command.ExecuteNonQuery();

                        LogUserAction(_userId, "DELETE", "Invoices", invoiceId);
                        LoadInvoices();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении счета: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (cmbApartments.SelectedItem == null ||
                dpInvoicePeriod.SelectedDate == null ||
                cmbTariffs.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtConsumption.Text) ||
                cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtConsumption.Text, out decimal consumption) || consumption <= 0)
            {
                MessageBox.Show("Введите корректное значение потребления (положительное число)",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Получаем тариф для расчета суммы
                    decimal rate = 0;
                    string rateQuery = "SELECT rate FROM Tariffs WHERE tariff_id = @tariffId";
                    SqlCommand rateCommand = new SqlCommand(rateQuery, connection);
                    rateCommand.Parameters.AddWithValue("@tariffId", cmbTariffs.SelectedValue);
                    rate = Convert.ToDecimal(rateCommand.ExecuteScalar());

                    decimal amount = consumption * rate;
                    DateTime issueDate = DateTime.Now;
                    DateTime dueDate = issueDate.AddDays(14); // Срок оплаты - 14 дней с даты выдачи
                    string status = (cmbStatus.SelectedItem as ComboBoxItem).Content.ToString();

                    if (_isEditMode)
                    {
                        // Обновление существующего счета
                        string query = @"UPDATE Invoices SET 
                                        apartment_id = @apartmentId, 
                                        period = @period, 
                                        tariff_id = @tariffId, 
                                        consumption = @consumption, 
                                        amount = @amount, 
                                        status = @status 
                                        WHERE invoice_id = @invoiceId";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@apartmentId", cmbApartments.SelectedValue);
                        command.Parameters.AddWithValue("@period", dpInvoicePeriod.SelectedDate);
                        command.Parameters.AddWithValue("@tariffId", cmbTariffs.SelectedValue);
                        command.Parameters.AddWithValue("@consumption", consumption);
                        command.Parameters.AddWithValue("@amount", amount);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@invoiceId", _currentInvoiceId);

                        command.ExecuteNonQuery();
                        LogUserAction(_userId, "UPDATE", "Invoices", _currentInvoiceId);
                    }
                    else
                    {
                        // Добавление нового счета
                        string query = @"INSERT INTO Invoices 
                                        (apartment_id, period, tariff_id, consumption, 
                                        amount, issue_date, due_date, status, created_by) 
                                        VALUES 
                                        (@apartmentId, @period, @tariffId, @consumption, 
                                        @amount, @issueDate, @dueDate, @status, @createdBy);
                                        SELECT SCOPE_IDENTITY();";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@apartmentId", cmbApartments.SelectedValue);
                        command.Parameters.AddWithValue("@period", dpInvoicePeriod.SelectedDate);
                        command.Parameters.AddWithValue("@tariffId", cmbTariffs.SelectedValue);
                        command.Parameters.AddWithValue("@consumption", consumption);
                        command.Parameters.AddWithValue("@amount", amount);
                        command.Parameters.AddWithValue("@issueDate", issueDate);
                        command.Parameters.AddWithValue("@dueDate", dueDate);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@createdBy", _userId);

                        int newId = Convert.ToInt32(command.ExecuteScalar());
                        LogUserAction(_userId, "CREATE", "Invoices", newId);
                    }

                    editForm.Visibility = Visibility.Collapsed;
                    LoadInvoices();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении счета: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            editForm.Visibility = Visibility.Collapsed;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadInvoices();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            LoadInvoices(txtSearch.Text);
        }

        private void ResetSearchButton_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            dpPeriod.SelectedDate = DateTime.Now;
            LoadInvoices();
        }

        private void FilterByPeriod_Click(object sender, RoutedEventArgs e)
        {
            if (dpPeriod.SelectedDate != null)
            {
                LoadInvoices("", dpPeriod.SelectedDate);
            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (invoicesGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите счет для печати", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView row = (DataRowView)invoicesGrid.SelectedItem;
            int invoiceId = (int)row["invoice_id"];

            try
            {
                // Получаем полные данные о счете
                DataTable invoiceData = GetInvoiceDetails(invoiceId);
                if (invoiceData.Rows.Count == 0)
                {
                    MessageBox.Show("Не удалось загрузить данные счета", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DataRow invoice = invoiceData.Rows[0];

                // Создаем документ для печати
                FlowDocument document = new FlowDocument();
                document.PagePadding = new Thickness(50);
                document.ColumnGap = 0;
                document.ColumnWidth = 500;

                // Заголовок документа
                Paragraph header = new Paragraph(new Run("ГАЗОВАЯ КОМПАНИЯ"));
                header.FontSize = 18;
                header.FontWeight = FontWeights.Bold;
                header.TextAlignment = TextAlignment.Center;
                header.Margin = new Thickness(0, 0, 0, 20);
                document.Blocks.Add(header);

                // Подзаголовок
                Paragraph subheader = new Paragraph(new Run("СЧЕТ НА ОПЛАТУ"));
                subheader.FontSize = 16;
                subheader.FontWeight = FontWeights.Bold;
                subheader.TextAlignment = TextAlignment.Center;
                subheader.Margin = new Thickness(0, 0, 0, 30);
                document.Blocks.Add(subheader);

                // Таблица с данными
                Table table = new Table();
                table.CellSpacing = 0;
                table.Margin = new Thickness(0, 0, 0, 30);

                // Колонки таблицы
                table.Columns.Add(new TableColumn() { Width = new GridLength(150) });
                table.Columns.Add(new TableColumn() { Width = new GridLength(350) });

                // Строки таблицы
                table.RowGroups.Add(new TableRowGroup());

                // Добавляем данные
                AddTableRow(table, "Номер счета:", invoice["invoice_id"].ToString());
                AddTableRow(table, "Дата выдачи:", ((DateTime)invoice["issue_date"]).ToString("dd.MM.yyyy"));
                AddTableRow(table, "Срок оплаты:", ((DateTime)invoice["due_date"]).ToString("dd.MM.yyyy"));
                AddTableRow(table, "Клиент:", invoice["client_name"].ToString());
                AddTableRow(table, "Адрес:", invoice["ApartmentInfo"].ToString());
                AddTableRow(table, "Период:", ((DateTime)invoice["period"]).ToString("MM.yyyy"));
                AddTableRow(table, "Тариф:", invoice["tariff_name"].ToString());
                AddTableRow(table, "Потребление (м³):", invoice["consumption"].ToString());
                AddTableRow(table, "Сумма к оплате:", ((decimal)invoice["amount"]).ToString("N2") + " руб.");
                AddTableRow(table, "Статус:", invoice["status"].ToString());

                document.Blocks.Add(table);

                // Подпись
                Paragraph sign = new Paragraph(new Run("Подпись ответственного лица: ___________________"));
                sign.FontSize = 12;
                sign.Margin = new Thickness(0, 30, 0, 0);
                document.Blocks.Add(sign);

                // Печать документа
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, "Счет на оплату");
                    LogUserAction(_userId, "PRINT", "Invoices", invoiceId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при печати счета: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private DataTable GetInvoiceDetails(int invoiceId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT i.invoice_id, i.issue_date, i.due_date, i.period, 
                            i.consumption, i.amount, i.status,
                            a.apartment_number + ', ' + b.house_number + ISNULL(b.building_letter, '') + ', ' + s.street_name AS ApartmentInfo,
                            t.tariff_name,
                            c.last_name + ' ' + c.first_name + ' ' + ISNULL(c.middle_name, '') AS client_name
                            FROM Invoices i
                            JOIN Apartments a ON i.apartment_id = a.apartment_id
                            JOIN Buildings b ON a.building_id = b.building_id
                            JOIN Streets s ON b.street_id = s.street_id
                            JOIN Tariffs t ON i.tariff_id = t.tariff_id
                            JOIN Clients c ON a.client_id = c.client_id
                            WHERE i.invoice_id = @invoiceId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных счета: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return dt;
        }

        private void AddTableRow(Table table, string label, string value)
        {
            TableRow row = new TableRow();

            row.Cells.Add(new TableCell(new Paragraph(new Run(label))
            {
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 5, 10, 5)
            }));

            row.Cells.Add(new TableCell(new Paragraph(new Run(value))
            {
                Margin = new Thickness(0, 5, 0, 5)
            }));

            table.RowGroups[0].Rows.Add(row);
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadInvoices(txtSearch.Text);
            }
        }

        private void ClearForm()
        {
            cmbApartments.SelectedIndex = -1;
            dpInvoicePeriod.SelectedDate = DateTime.Now;
            cmbTariffs.SelectedIndex = -1;
            txtConsumption.Text = "";
            cmbStatus.SelectedIndex = 0;
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

        private void invoicesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Можно добавить дополнительную логику при выборе счета
        }
    }
}