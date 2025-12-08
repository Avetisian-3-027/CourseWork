using LibraryApp.Services;

namespace LibraryApp.UI
{
    public class BooksForm : Form
    {
        private readonly BookService _bookService;
        private readonly GenreService _genreService;
        private readonly AuthorService _authorService;
        private readonly ReportService _reportService;

        private ListView _lvBooks, _lvAuthors, _lvGenres;
        private Button _btnAdd, _btnEdit, _btnDelete, _btnReports;

        public BooksForm(BookService bookService, AuthorService authorService, GenreService genreService, ReportService reportService)
        {
            _bookService = bookService;
            _authorService = authorService;
            _reportService = reportService;
            _genreService = genreService;

            Text = "Бібліотека";
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Width = 800;
            this.Height = 530;
            StartPosition = FormStartPosition.CenterScreen;

            InitializeControls();
            Load += async (s, e) => await LoadAllAsync();
        }

        private void InitializeControls()
        {
            var tabControl = new TabControl
            {
                Left = 20,
                Top = 20,
                Width = 760,
                Height = 380
            };

            var tabBooks = new TabPage("Книги");
            var tabAuthors = new TabPage("Автори");
            var tabGenres = new TabPage("Жанри");

            tabControl.TabPages.Add(tabBooks);
            tabControl.TabPages.Add(tabAuthors);
            tabControl.TabPages.Add(tabGenres);
            
            _lvBooks = new ListView
            {
                Left = 0,
                Top = 0,
                Width = 740,
                Height = 350,
                View = View.Details,
                FullRowSelect = true
            };

            _lvBooks.Columns.Add("ID", 50);
            _lvBooks.Columns.Add("Назва", 250);
            _lvBooks.Columns.Add("Автор", 150);
            _lvBooks.Columns.Add("Жанр", 150);
            _lvBooks.Columns.Add("Рік видання", 100);

            tabBooks.Controls.Add(_lvBooks);
            
            _lvAuthors = new ListView
            {
                Left = 0,
                Top = 0,
                Width = 740,
                Height = 350,
                View = View.Details,
                FullRowSelect = true
            };

            _lvAuthors.Columns.Add("ID", 50);
            _lvAuthors.Columns.Add("Ім'я", 200);
            _lvAuthors.Columns.Add("Кількість книг", 150);

            tabAuthors.Controls.Add(_lvAuthors);

            _lvGenres = new ListView
            {
                Left = 0,
                Top = 0,
                Width = 740,
                Height = 350,
                View = View.Details,
                FullRowSelect = true
            };

            _lvGenres.Columns.Add("ID", 50);
            _lvGenres.Columns.Add("Жанр", 200);
            _lvGenres.Columns.Add("Кількість книг", 150);

            tabGenres.Controls.Add(_lvGenres);

            _btnAdd = new Button { Text = "Додати", Left = 20, Top = 420, Width = 110, Height = 30 };
            _btnEdit = new Button { Text = "Редагувати", Left = 140, Top = 420, Width = 110, Height = 30 };
            _btnDelete = new Button { Text = "Видалити", Left = 260, Top = 420, Width = 110, Height = 30 };
            _btnReports = new Button { Text = "Звіти", Left = 380, Top = 420, Width = 110, Height = 30 };

            _btnAdd.Click += async (s, e) => await AddBookAsync();
            _btnEdit.Click += async (s, e) => await EditBookAsync();
            _btnDelete.Click += async (s, e) => await DeleteBookAsync();
            _btnReports.Click += (s, e) => ShowReports();

            Controls.AddRange(new Control[] { tabControl, _btnAdd, _btnEdit, _btnDelete, _btnReports });
        }

        private async Task LoadAllAsync()
        {
            await LoadBooksAsync();
            await LoadAuthorsAsync();
            await LoadGenresAsync();
        }
        private async Task LoadBooksAsync()
        {
            _lvBooks.Items.Clear();
            var books = await _bookService.GetAllAsync();
            foreach (var b in books)
            {
                var item = new ListViewItem(b.Id.ToString());
                item.SubItems.Add(b.Title);
                item.SubItems.Add(b.Author?.Name ?? "");
                item.SubItems.Add(b.Genre?.GenreType ?? "");
                item.SubItems.Add(b.Year.ToString());
                _lvBooks.Items.Add(item);
            }
        }
        private async Task LoadAuthorsAsync()
        {
            _lvAuthors.Items.Clear();
            var authors = await _authorService.GetAllAsync();
            foreach (var b in authors)
            {
                var item = new ListViewItem(b.Id.ToString());
                item.SubItems.Add(b.Name);
                item.SubItems.Add(b.Books.Count.ToString());
                _lvAuthors.Items.Add(item);
            }
        }
        private async Task LoadGenresAsync()
        {
            _lvGenres.Items.Clear();
            var genres = await _genreService.GetAllAsync();
            foreach (var b in genres)
            {
                var item = new ListViewItem(b.Id.ToString());
                item.SubItems.Add(b.GenreType);
                item.SubItems.Add(b.Books.Count.ToString());
                _lvGenres.Items.Add(item);
            }
        }

        private async Task AddBookAsync()
        {
            var form = new AddEditBookForm(_bookService, _genreService, _authorService);
            if (form.ShowDialog() == DialogResult.OK)
                await LoadAllAsync();
        }

        private async Task EditBookAsync()
        {
            if (_lvBooks.SelectedItems.Count == 0) return;
            int id = int.Parse(_lvBooks.SelectedItems[0].Text);
            var form = new AddEditBookForm(_bookService, _genreService, _authorService, id);
            if (form.ShowDialog() == DialogResult.OK)
                await LoadAllAsync();
        }

        private async Task DeleteBookAsync()
        {
            if (_lvBooks.SelectedItems.Count == 0) return;
            int id = int.Parse(_lvBooks.SelectedItems[0].Text);
            await _bookService.DeleteAsync(id);
            await LoadAllAsync();
        }

        private void ShowReports()
        {
            var form = new ReportsForm(_reportService);
            form.ShowDialog();
        }
    }
}
