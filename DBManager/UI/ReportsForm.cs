using LibraryApp.Services;
using System.Text;

namespace LibraryApp.UI
{
    public class ReportsForm : Form
    {
        private readonly ReportService _reportService;
        private ListBox _lstReports = null!;
        private Button _btnLoadStats = null!;
        private Button _btnOldestBook = null!;
        private Button _btnTopAuthor = null!;
        private Button _btnSaveToCsv = null!;
        private string currentReport = "";

        public ReportsForm(ReportService reportService)
        {
            _reportService = reportService;

            Text = "Звіт бібліотеки";
            this.ShowIcon = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            Width = 600;
            Height = 500;
            StartPosition = FormStartPosition.CenterParent;

            InitializeControls();
        }

        private void InitializeControls()
        {
            _lstReports = new ListBox { Left = 20, Top = 20, Width = 540, Height = 260 };

            _btnLoadStats = new Button { Text = "Статистика за жанрами", Left = 20, Top = 300, Width = 160, Height = 50 };
            _btnTopAuthor = new Button { Text = "Автор с найб. кіль. книжок", Left = 200, Top = 300, Width = 160, Height = 50 };
            _btnOldestBook = new Button { Text = "Сама стара книжка", Left = 380, Top = 300, Width = 160, Height = 50 };
            _btnSaveToCsv = new Button { Text = "Збереження звіту до CSV", Left = 200, Top = 370, Width = 160, Height = 50, Enabled = false };

            _btnLoadStats.Click += async (s, e) => await LoadGenreStatsAsync();
            _btnTopAuthor.Click += async (s, e) => await LoadTopAuthorAsync();
            _btnOldestBook.Click += async (s, e) => await LoadOldestBookAsync();
            _btnSaveToCsv.Click += BtnSaveToCsv_Click;

            Controls.AddRange(new Control[] { _lstReports, _btnLoadStats, _btnTopAuthor, _btnOldestBook, _btnSaveToCsv });
        }

        private async Task LoadGenreStatsAsync()
        {
            _lstReports.Items.Clear();
            var stats = await _reportService.GetBooksCountByGenreAsync();
            if (!stats.Any())
                _lstReports.Items.Add("Немає даних для відображення");
            else
                foreach (var kvp in stats)
                    _lstReports.Items.Add($"{kvp.Key}: {kvp.Value} книжок");

            UpdateCurrentReport();
        }

        private async Task LoadTopAuthorAsync()
        {
            _lstReports.Items.Clear();
            var author = await _reportService.GetAuthorWithMostBooksAsync();
            if (author == null)
                _lstReports.Items.Add("У базі даних немає авторів");
            else
            {
                _lstReports.Items.Add($"Автор з найбільшою кількістю книжок: {author.Name}");
                _lstReports.Items.Add($"Кількість книжок: {author.Books.Count}");
            }

            UpdateCurrentReport();
        }

        private async Task LoadOldestBookAsync()
        {
            _lstReports.Items.Clear();
            var book = await _reportService.GetOldestBookAsync();
            if (book == null)
                _lstReports.Items.Add("У базі немає даних...");
            else
            {
                _lstReports.Items.Add($"Найстаріша книга: {book.Title}");
                _lstReports.Items.Add($"Її автор: {book.Author?.Name}");
                _lstReports.Items.Add($"Рік видання: {book.Year}");
            }

            UpdateCurrentReport();
        }

        private void UpdateCurrentReport()
        {
            currentReport = string.Join("\n", _lstReports.Items.Cast<string>());
            _btnSaveToCsv.Enabled = _lstReports.Items.Count > 0;
        }

        private void BtnSaveToCsv_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentReport))
                return;

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "CSV file (*.csv)|*.csv";
                dialog.Title = "Зберегти звіт";
                dialog.FileName = "report.csv";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        System.IO.File.WriteAllText(dialog.FileName, ConvertReportToCsv(currentReport), Encoding.UTF8);
                        MessageBox.Show("Звіт успішно збережено!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка збереження файлу: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private string ConvertReportToCsv(string report)
        {
            var lines = report.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                    continue;

                var parts = trimmed.Split(new[] { ':' }, 2);
                string left = parts[0].Trim();
                string right = parts.Length > 1 ? parts[1].Trim() : "";

                left = left.Replace("\"", "\"\"");
                right = right.Replace("\"", "\"\"");

                sb.AppendLine($"\"{left}\";\"{right}\"");
            }

            return sb.ToString();
        }
    }
}
