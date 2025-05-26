using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using System.Windows.Documents;
using System.IO.Packaging;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using System.Diagnostics;


namespace GasCompanyApp
{
    public partial class ReportsView : UserControl
    {
        private int _userId;
        string connectionString = "Data Source=DESKTOP-A27DH7D\\SQLEXPRESS;Initial Catalog=GasCompanyDB;Integrated Security=True;TrustServerCertificate=True";

        public ReportsView(int userId)
        {
            InitializeComponent();
            _userId = userId;

            // Установка дат по умолчанию
            dpStartDate.SelectedDate = DateTime.Today.AddMonths(-1);
            dpEndDate.SelectedDate = DateTime.Today;
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            if (cmbReportType.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип отчета", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null)
            {
                MessageBox.Show("Укажите период для отчета", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime startDate = dpStartDate.SelectedDate.Value;
            DateTime endDate = dpEndDate.SelectedDate.Value;

            if (startDate > endDate)
            {
                MessageBox.Show("Дата начала периода не может быть позже даты окончания", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string reportType = (cmbReportType.SelectedItem as ComboBoxItem).Content.ToString();
                DataTable reportData = new DataTable();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;

                    switch (reportType)
                    {
                        case "Потребление газа по месяцам":
                            command.CommandText = @"SELECT 
                                                YEAR(mr.reading_date) AS Год,
                                                MONTH(mr.reading_date) AS Месяц,
                                                SUM(mr.consumption) AS 'Общее потребление (м³)',
                                                AVG(mr.consumption) AS 'Среднее потребление (м³)',
                                                COUNT(DISTINCT a.apartment_id) AS 'Кол-во квартир'
                                            FROM MeterReadings mr
                                            JOIN GasMeters gm ON mr.meter_id = gm.meter_id
                                            JOIN Apartments a ON gm.apartment_id = a.apartment_id
                                            WHERE mr.reading_date BETWEEN @startDate AND @endDate
                                            GROUP BY YEAR(mr.reading_date), MONTH(mr.reading_date)
                                            ORDER BY Год, Месяц";
                            break;

                        case "Платежи по клиентам":
                            command.CommandText = @"SELECT 
                                                c.last_name + ' ' + c.first_name AS Клиент,
                                                a.apartment_number AS Квартира,
                                                COUNT(p.payment_id) AS 'Кол-во платежей',
                                                SUM(p.amount) AS 'Сумма платежей',
                                                MAX(p.payment_date) AS 'Последний платеж'
                                            FROM Payments p
                                            JOIN Invoices i ON p.invoice_id = i.invoice_id
                                            JOIN Apartments a ON i.apartment_id = a.apartment_id
                                            JOIN Clients c ON a.client_id = c.client_id
                                            WHERE p.payment_date BETWEEN @startDate AND @endDate
                                            GROUP BY c.last_name, c.first_name, a.apartment_number
                                            ORDER BY Клиент";
                            break;

                        case "Задолженности по квартирам":
                            command.CommandText = @"SELECT 
                                                a.apartment_number AS Квартира,
                                                c.last_name + ' ' + c.first_name AS Клиент,
                                                COUNT(i.invoice_id) AS 'Кол-во неоплаченных счетов',
                                                SUM(i.amount) AS 'Сумма задолженности',
                                                MAX(i.due_date) AS 'Крайний срок оплаты'
                                            FROM Invoices i
                                            JOIN Apartments a ON i.apartment_id = a.apartment_id
                                            JOIN Clients c ON a.client_id = c.client_id
                                            WHERE i.status IN ('Не оплачено', 'Частично оплачено')
                                            GROUP BY a.apartment_number, c.last_name, c.first_name
                                            ORDER BY 'Сумма задолженности' DESC";
                            break;

                        case "Показания счетчиков":
                            command.CommandText = @"SELECT 
                                                a.apartment_number AS Квартира,
                                                gm.serial_number AS 'Серийный номер',
                                                mt.type_name AS 'Тип счетчика',
                                                mr.reading_date AS 'Дата показания',
                                                mr.current_reading AS 'Текущее показание',
                                                mr.consumption AS 'Потребление (м³)',
                                                CASE WHEN mr.is_verified = 1 THEN 'Да' ELSE 'Нет' END AS 'Проверено'
                                            FROM MeterReadings mr
                                            JOIN GasMeters gm ON mr.meter_id = gm.meter_id
                                            JOIN MeterTypes mt ON gm.meter_type_id = mt.meter_type_id
                                            JOIN Apartments a ON gm.apartment_id = a.apartment_id
                                            WHERE mr.reading_date BETWEEN @startDate AND @endDate
                                            ORDER BY mr.reading_date DESC";
                            break;
                    }

                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(reportData);
                }

                reportGrid.ItemsSource = reportData.DefaultView;
                LogUserAction(_userId, "ACCESS", "Reports", 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            if (reportGrid.ItemsSource == null)
            {
                MessageBox.Show("Нет данных для экспорта", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV файл (*.csv)|*.csv";
                saveFileDialog.FileName = $"Отчет_{DateTime.Now:yyyyMMddHHmmss}.csv";

                if (saveFileDialog.ShowDialog() == true)
                {
                    DataView dataView = reportGrid.ItemsSource as DataView;
                    if (dataView == null) return;

                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName, false, System.Text.Encoding.UTF8))
                    {
                        // Записываем заголовки колонок
                        for (int i = 0; i < dataView.Table.Columns.Count; i++)
                        {
                            writer.Write(dataView.Table.Columns[i].ColumnName);
                            if (i < dataView.Table.Columns.Count - 1)
                                writer.Write(",");
                        }
                        writer.WriteLine();

                        // Записываем данные
                        foreach (DataRowView rowView in dataView)
                        {
                            for (int i = 0; i < dataView.Table.Columns.Count; i++)
                            {
                                string value = rowView[i].ToString();

                                // Экранируем кавычки и запятые
                                if (value.Contains(",") || value.Contains("\""))
                                {
                                    value = "\"" + value.Replace("\"", "\"\"") + "\"";
                                }

                                writer.Write(value);
                                if (i < dataView.Table.Columns.Count - 1)
                                    writer.Write(",");
                            }
                            writer.WriteLine();
                        }
                    }

                    MessageBox.Show("Экспорт в CSV успешно выполнен", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    LogUserAction(_userId, "EXPORT", "Reports", 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в CSV: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportToPDF_Click(object sender, RoutedEventArgs e)
        {
            if (reportGrid.ItemsSource == null)
            {
                MessageBox.Show("Нет данных для экспорта", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF файл (*.pdf)|*.pdf";
                saveFileDialog.FileName = $"Отчет_{DateTime.Now:yyyyMMddHHmmss}.pdf";

                if (saveFileDialog.ShowDialog() == true)
                {
                    // Создаем документ для печати (аналогично методу PrintReport_Click)
                    DataView dataView = reportGrid.ItemsSource as DataView;
                    if (dataView == null) return;

                    string reportType = (cmbReportType.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Отчет";

                    FlowDocument document = CreateFlowDocument(dataView, reportType);

                    // Сохраняем документ как XPS, а затем конвертируем в PDF
                    string tempXpsFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".xps");

                    using (Package package = Package.Open(tempXpsFile, FileMode.Create))
                    {
                        using (XpsDocument xpsDoc = new XpsDocument(package, CompressionOption.Maximum))
                        {
                            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
                            writer.Write(((IDocumentPaginatorSource)document).DocumentPaginator);
                        }
                    }

                    // Конвертируем XPS в PDF
                    ConvertXpsToPdf(tempXpsFile, saveFileDialog.FileName);

                    // Удаляем временный XPS файл
                    File.Delete(tempXpsFile);

                    MessageBox.Show("Экспорт в PDF успешно выполнен", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    LogUserAction(_userId, "EXPORT", "Reports", 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в PDF: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private FlowDocument CreateFlowDocument(DataView dataView, string reportType)
        {
            FlowDocument document = new FlowDocument();
            document.PagePadding = new Thickness(50);
            document.ColumnGap = 0;
            document.ColumnWidth = 700;

            // Заголовок документа
            Paragraph header = new Paragraph(new Run("ГАЗОВАЯ КОМПАНИЯ"));
            header.FontSize = 18;
            header.FontWeight = FontWeights.Bold;
            header.TextAlignment = TextAlignment.Center;
            header.Margin = new Thickness(0, 0, 0, 20);
            document.Blocks.Add(header);

            // Название отчета
            Paragraph reportHeader = new Paragraph(new Run(reportType));
            reportHeader.FontSize = 16;
            reportHeader.FontWeight = FontWeights.Bold;
            reportHeader.TextAlignment = TextAlignment.Center;
            reportHeader.Margin = new Thickness(0, 0, 0, 10);
            document.Blocks.Add(reportHeader);

            // Период отчета
            if (dpStartDate.SelectedDate != null && dpEndDate.SelectedDate != null)
            {
                Paragraph period = new Paragraph(new Run($"Период: с {dpStartDate.SelectedDate.Value:dd.MM.yyyy} по {dpEndDate.SelectedDate.Value:dd.MM.yyyy}"));
                period.FontSize = 12;
                period.TextAlignment = TextAlignment.Center;
                period.Margin = new Thickness(0, 0, 0, 20);
                document.Blocks.Add(period);
            }

            // Дата формирования
            Paragraph date = new Paragraph(new Run($"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}"));
            date.FontSize = 12;
            date.TextAlignment = TextAlignment.Center;
            date.Margin = new Thickness(0, 0, 0, 30);
            document.Blocks.Add(date);

            // Таблица с данными
            Table table = new Table();
            table.CellSpacing = 0;
            table.Margin = new Thickness(0, 0, 0, 30);

            // Добавляем колонки
            foreach (DataColumn column in dataView.Table.Columns)
            {
                table.Columns.Add(new TableColumn() { Width = GridLength.Auto });
            }

            // Строки таблицы
            table.RowGroups.Add(new TableRowGroup());

            // Заголовки колонок
            TableRow headerRow = new TableRow();
            foreach (DataColumn column in dataView.Table.Columns)
            {
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run(column.ColumnName))
                {
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5)
                }));
            }
            table.RowGroups[0].Rows.Add(headerRow);

            // Данные
            foreach (DataRowView rowView in dataView)
            {
                TableRow row = new TableRow();
                foreach (DataColumn column in dataView.Table.Columns)
                {
                    string value = rowView[column.ColumnName].ToString();
                    if (column.DataType == typeof(DateTime))
                    {
                        DateTime dateValue = (DateTime)rowView[column.ColumnName];
                        value = dateValue.ToString("dd.MM.yyyy");
                    }
                    else if (column.DataType == typeof(decimal))
                    {
                        decimal decimalValue = (decimal)rowView[column.ColumnName];
                        value = decimalValue.ToString("N2");
                    }

                    row.Cells.Add(new TableCell(new Paragraph(new Run(value))
                    {
                        Margin = new Thickness(5)
                    }));
                }
                table.RowGroups[0].Rows.Add(row);
            }

            document.Blocks.Add(table);

            // Подпись
            Paragraph sign = new Paragraph(new Run("Ответственный: ___________________"));
            sign.FontSize = 12;
            sign.Margin = new Thickness(0, 30, 0, 0);
            document.Blocks.Add(sign);

            return document;
        }

        private void ConvertXpsToPdf(string xpsFilePath, string pdfFilePath)
        {
            // Этот метод использует встроенные возможности Windows

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command \"Add-Type -AssemblyName System.Printing; " +
                           $"$xpsDoc = New-Object System.Windows.Xps.Packaging.XpsDocument('{xpsFilePath}', [System.IO.FileAccess]::Read); " +
                           "$pdfWriter = [System.Windows.Xps.XpsDocumentWriter]::Create('{pdfFilePath}'); " +
                           "$pdfWriter.Write($xpsDoc.GetFixedDocumentSequence().DocumentPaginator); " +
                           "$xpsDoc.Close()\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                process.WaitForExit();
            }
        }

        private void PrintReport_Click(object sender, RoutedEventArgs e)
        {
            if (reportGrid.ItemsSource == null)
            {
                MessageBox.Show("Нет данных для печати", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                DataView dataView = reportGrid.ItemsSource as DataView;
                if (dataView == null) return;

                string reportType = (cmbReportType.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Отчет";

                // Создаем документ для печати
                FlowDocument document = new FlowDocument();
                document.PagePadding = new Thickness(50);
                document.ColumnGap = 0;
                document.ColumnWidth = 700;

                // Заголовок документа
                Paragraph header = new Paragraph(new Run("ГАЗОВАЯ КОМПАНИЯ"));
                header.FontSize = 18;
                header.FontWeight = FontWeights.Bold;
                header.TextAlignment = TextAlignment.Center;
                header.Margin = new Thickness(0, 0, 0, 20);
                document.Blocks.Add(header);

                // Название отчета
                Paragraph reportHeader = new Paragraph(new Run(reportType));
                reportHeader.FontSize = 16;
                reportHeader.FontWeight = FontWeights.Bold;
                reportHeader.TextAlignment = TextAlignment.Center;
                reportHeader.Margin = new Thickness(0, 0, 0, 10);
                document.Blocks.Add(reportHeader);

                // Период отчета
                if (dpStartDate.SelectedDate != null && dpEndDate.SelectedDate != null)
                {
                    Paragraph period = new Paragraph(new Run($"Период: с {dpStartDate.SelectedDate.Value:dd.MM.yyyy} по {dpEndDate.SelectedDate.Value:dd.MM.yyyy}"));
                    period.FontSize = 12;
                    period.TextAlignment = TextAlignment.Center;
                    period.Margin = new Thickness(0, 0, 0, 20);
                    document.Blocks.Add(period);
                }

                // Дата формирования
                Paragraph date = new Paragraph(new Run($"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}"));
                date.FontSize = 12;
                date.TextAlignment = TextAlignment.Center;
                date.Margin = new Thickness(0, 0, 0, 30);
                document.Blocks.Add(date);

                // Таблица с данными
                Table table = new Table();
                table.CellSpacing = 0;
                table.Margin = new Thickness(0, 0, 0, 30);

                // Добавляем колонки
                foreach (DataColumn column in dataView.Table.Columns)
                {
                    table.Columns.Add(new TableColumn() { Width = GridLength.Auto });
                }

                // Строки таблицы
                table.RowGroups.Add(new TableRowGroup());

                // Заголовки колонок
                TableRow headerRow = new TableRow();
                foreach (DataColumn column in dataView.Table.Columns)
                {
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run(column.ColumnName))
                    {
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(5)
                    }));
                }
                table.RowGroups[0].Rows.Add(headerRow);

                // Данные
                foreach (DataRowView rowView in dataView)
                {
                    TableRow row = new TableRow();
                    foreach (DataColumn column in dataView.Table.Columns)
                    {
                        string value = rowView[column.ColumnName].ToString();
                        if (column.DataType == typeof(DateTime))
                        {
                            DateTime dateValue = (DateTime)rowView[column.ColumnName];
                            value = dateValue.ToString("dd.MM.yyyy");
                        }
                        else if (column.DataType == typeof(decimal))
                        {
                            decimal decimalValue = (decimal)rowView[column.ColumnName];
                            value = decimalValue.ToString("N2");
                        }

                        row.Cells.Add(new TableCell(new Paragraph(new Run(value))
                        {
                            Margin = new Thickness(5)
                        }));
                    }
                    table.RowGroups[0].Rows.Add(row);
                }

                document.Blocks.Add(table);

                // Подпись
                Paragraph sign = new Paragraph(new Run("Ответственный: ___________________"));
                sign.FontSize = 12;
                sign.Margin = new Thickness(0, 30, 0, 0);
                document.Blocks.Add(sign);

                // Печать документа
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, reportType);
                    LogUserAction(_userId, "PRINT", "Reports", 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при печати отчета: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
                Console.WriteLine($"Ошибка при записи в лог: {ex.Message}");
            }
        }
    }
}