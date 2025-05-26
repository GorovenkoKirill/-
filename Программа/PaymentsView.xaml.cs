using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace GasCompanyApp
{
    public partial class PaymentsView : UserControl
    {
        private int _userId;
        private int _currentPaymentId = 0;
        string connectionString = "Data Source=DESKTOP-A27DH7D\\SQLEXPRESS;Initial Catalog=GasCompanyDB;Integrated Security=True;TrustServerCertificate=True";

        public PaymentsView(int userId)
        {
            InitializeComponent();
            _userId = userId;

            // Инициализация фильтров
            dpStartDate.SelectedDate = DateTime.Today.AddMonths(-1);
            dpEndDate.SelectedDate = DateTime.Today;

            // Заполнение способов оплаты
            cmbPaymentMethod.ItemsSource = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Все методы", ""),
                new KeyValuePair<string, string>("Наличные", "Наличные"),
                new KeyValuePair<string, string>("Банковская карта", "Банковская карта"),
                new KeyValuePair<string, string>("Банковский перевод", "Банковский перевод"),
                new KeyValuePair<string, string>("Электронный платеж", "Электронный платеж")
            };
            cmbPaymentMethod.SelectedIndex = 0;

            // Заполнение способов оплаты в форме
            cmbPaymentMethods.ItemsSource = new List<string>()
            {
                "Наличные",
                "Банковская карта",
                "Банковский перевод",
                "Электронный платеж"
            };
            cmbPaymentMethods.SelectedIndex = 0;

            LoadPayments();
            LoadInvoices();
        }

        private void LoadPayments()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT p.payment_id, p.invoice_id, p.payment_date, p.amount, 
                                    p.payment_method, p.receipt_number,
                                    u.last_name + ' ' + u.first_name AS OperatorName
                                    FROM Payments p
                                    LEFT JOIN SystemUsers u ON p.operator_id = u.user_id
                                    WHERE (@startDate IS NULL OR p.payment_date >= @startDate)
                                    AND (@endDate IS NULL OR p.payment_date <= @endDate)
                                    AND (@paymentMethod = '' OR p.payment_method = @paymentMethod)
                                    ORDER BY p.payment_date DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@startDate", dpStartDate.SelectedDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@endDate", dpEndDate.SelectedDate?.AddDays(1) ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@paymentMethod", cmbPaymentMethod.SelectedValue?.ToString() ?? "");

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    paymentsGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке платежей: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadInvoices()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT i.invoice_id, 
                                    'Счет №' + CAST(i.invoice_id AS NVARCHAR) + ' от ' + 
                                    CONVERT(NVARCHAR, i.issue_date, 104) + ' (Кв.' + 
                                    a.apartment_number + ')' AS InvoiceInfo,
                                    i.amount - ISNULL((SELECT SUM(amount) FROM Payments WHERE invoice_id = i.invoice_id), 0) AS RemainingAmount
                                    FROM Invoices i
                                    JOIN Apartments a ON i.apartment_id = a.apartment_id
                                    WHERE i.status <> 'Оплачено'
                                    ORDER BY i.issue_date DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    cmbInvoices.ItemsSource = dt.DefaultView;
                    cmbInvoices.SelectedValuePath = "invoice_id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке счетов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (cmbInvoices.Items.Count == 0)
            {
                MessageBox.Show("Нет доступных счетов для оплаты", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _currentPaymentId = 0;
            ClearForm();
            editForm.Visibility = Visibility.Visible;
            cmbInvoices.SelectedIndex = 0;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (cmbInvoices.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtAmount.Text) ||
                cmbPaymentMethods.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную сумму платежа (положительное число)", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView selectedInvoice = (DataRowView)cmbInvoices.SelectedItem;
            decimal remainingAmount = (decimal)selectedInvoice["RemainingAmount"];

            if (amount > remainingAmount)
            {
                MessageBox.Show($"Сумма платежа не может превышать остаток по счету ({remainingAmount:N2})", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Добавление платежа
                    string query = @"INSERT INTO Payments 
                                    (invoice_id, payment_date, amount, payment_method, receipt_number, operator_id) 
                                    VALUES 
                                    (@invoiceId, GETDATE(), @amount, @paymentMethod, @receiptNumber, @operatorId);
                                    SELECT SCOPE_IDENTITY();";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@invoiceId", cmbInvoices.SelectedValue);
                    command.Parameters.AddWithValue("@amount", amount);
                    command.Parameters.AddWithValue("@paymentMethod", cmbPaymentMethods.SelectedItem.ToString());
                    command.Parameters.AddWithValue("@receiptNumber",
                        string.IsNullOrWhiteSpace(txtReceiptNumber.Text) ? DBNull.Value : (object)txtReceiptNumber.Text);
                    command.Parameters.AddWithValue("@operatorId", _userId);

                    int newId = Convert.ToInt32(command.ExecuteScalar());
                    LogUserAction(_userId, "CREATE", "Payments", newId);

                    // Обновление статуса счета, если он полностью оплачен
                    UpdateInvoiceStatus((int)cmbInvoices.SelectedValue);

                    editForm.Visibility = Visibility.Collapsed;
                    LoadPayments();
                    LoadInvoices(); // Обновляем список счетов после добавления платежа
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении платежа: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateInvoiceStatus(int invoiceId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Проверяем, полностью ли оплачен счет
                    string query = @"DECLARE @totalAmount DECIMAL(12, 2);
                                    DECLARE @paidAmount DECIMAL(12, 2);
                                    
                                    SELECT @totalAmount = amount FROM Invoices WHERE invoice_id = @invoiceId;
                                    SELECT @paidAmount = ISNULL(SUM(amount), 0) FROM Payments WHERE invoice_id = @invoiceId;
                                    
                                    IF @paidAmount >= @totalAmount
                                        UPDATE Invoices SET status = 'Оплачено' WHERE invoice_id = @invoiceId;
                                    ELSE IF @paidAmount > 0
                                        UPDATE Invoices SET status = 'Частично оплачено' WHERE invoice_id = @invoiceId;";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении статуса счета: {ex.Message}");
            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (paymentsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите платеж для печати квитанции", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView row = (DataRowView)paymentsGrid.SelectedItem;
            int paymentId = (int)row["payment_id"];

            try
            {
                // Получаем полные данные о платеже
                DataTable paymentData = GetPaymentDetails(paymentId);
                if (paymentData.Rows.Count == 0)
                {
                    MessageBox.Show("Не удалось загрузить данные платежа", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DataRow payment = paymentData.Rows[0];

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
                Paragraph subheader = new Paragraph(new Run("КВИТАНЦИЯ ОБ ОПЛАТЕ"));
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
                AddTableRow(table, "Номер квитанции:", payment["receipt_number"].ToString());
                AddTableRow(table, "Дата оплаты:", ((DateTime)payment["payment_date"]).ToString("dd.MM.yyyy HH:mm"));
                AddTableRow(table, "Номер счета:", payment["invoice_id"].ToString());
                AddTableRow(table, "Клиент:", payment["client_name"].ToString());
                AddTableRow(table, "Адрес:", payment["ApartmentInfo"].ToString());
                AddTableRow(table, "Сумма оплаты:", ((decimal)payment["amount"]).ToString("N2") + " руб.");
                AddTableRow(table, "Способ оплаты:", payment["payment_method"].ToString());
                AddTableRow(table, "Оператор:", payment["OperatorName"].ToString());

                document.Blocks.Add(table);

                // Подпись
                Paragraph sign = new Paragraph(new Run("Подпись оператора: ___________________"));
                sign.FontSize = 12;
                sign.Margin = new Thickness(0, 30, 0, 0);
                document.Blocks.Add(sign);

                // Печать документа
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, "Квитанция об оплате");
                    LogUserAction(_userId, "PRINT", "Payments", paymentId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при печати квитанции: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private DataTable GetPaymentDetails(int paymentId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT p.payment_id, p.invoice_id, p.payment_date, p.amount, 
                            p.payment_method, p.receipt_number,
                            u.last_name + ' ' + u.first_name AS OperatorName,
                            a.apartment_number + ', ' + b.house_number + ISNULL(b.building_letter, '') + ', ' + s.street_name AS ApartmentInfo,
                            c.last_name + ' ' + c.first_name + ' ' + ISNULL(c.middle_name, '') AS client_name
                            FROM Payments p
                            LEFT JOIN SystemUsers u ON p.operator_id = u.user_id
                            JOIN Invoices i ON p.invoice_id = i.invoice_id
                            JOIN Apartments a ON i.apartment_id = a.apartment_id
                            JOIN Buildings b ON a.building_id = b.building_id
                            JOIN Streets s ON b.street_id = s.street_id
                            JOIN Clients c ON a.client_id = c.client_id
                            WHERE p.payment_id = @paymentId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@paymentId", paymentId);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных платежа: {ex.Message}", "Ошибка",
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            editForm.Visibility = Visibility.Collapsed;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadPayments();
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            LoadPayments();
        }

        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            dpStartDate.SelectedDate = DateTime.Today.AddMonths(-1);
            dpEndDate.SelectedDate = DateTime.Today;
            cmbPaymentMethod.SelectedIndex = 0;
            LoadPayments();
        }

        private void ClearForm()
        {
            txtAmount.Text = "";
            txtReceiptNumber.Text = "";
            cmbPaymentMethods.SelectedIndex = 0;
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

        private void paymentsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Можно добавить дополнительную логику при выборе платежа
        }

        private void cmbPaymentMethods_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Показываем поле для номера квитанции только для некоторых способов оплаты
            if (cmbPaymentMethods.SelectedItem != null)
            {
                string method = cmbPaymentMethods.SelectedItem.ToString();
                lblReceiptNumber.Visibility = (method == "Банковский перевод" || method == "Электронный платеж") ?
                    Visibility.Visible : Visibility.Collapsed;
                txtReceiptNumber.Visibility = lblReceiptNumber.Visibility;
            }
        }
    }
}