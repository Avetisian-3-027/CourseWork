using LibraryApp.Domain;
using LibraryApp.Services;

namespace LibraryApp.UI
{
    public class AddEditBookForm : Form
    {
        private readonly BookService _bookService;
        private readonly AuthorService _authorService;
        private readonly GenreService _genreService;
        private readonly int? _bookId;

        private TextBox _txtTitle, _txtGenre, _txtYear, _txtAuthor;
        private Button _btnSave;

        public AddEditBookForm(BookService bookService, GenreService genreService, AuthorService authorService, int? bookId = null)
        {
            _bookService = bookService;
            _authorService = authorService;
            _genreService = genreService;
            _bookId = bookId;

            Text = _bookId.HasValue ? "Редагувати книгу" : "Додати книгу";
            this.ShowIcon = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            Width = 400;
            Height = 300;
            StartPosition = FormStartPosition.CenterParent;

            InitializeControls();
            Load += async (s, e) => await LoadDataAsync();
        }

        private void InitializeControls()
        {
            _txtTitle = new TextBox { Left = 120, Top = 20, Width = 200 };
            _txtGenre = new TextBox { Left = 120, Top = 60, Width = 200 };
            _txtYear = new TextBox { Left = 120, Top = 100, Width = 200 };
            _txtAuthor = new TextBox { Left = 120, Top = 140, Width = 200 };
            _btnSave = new Button { Text = "Зберегти", Left = 120, Top = 180, Width = 100, Height = 30 };

            Controls.Add(new Label { Text = "Назва:", Left = 20, Top = 20 });
            Controls.Add(new Label { Text = "Жанр:", Left = 20, Top = 60 });
            Controls.Add(new Label { Text = "Рік:", Left = 20, Top = 100 });
            Controls.Add(new Label { Text = "Автор:", Left = 20, Top = 140 });
            Controls.AddRange(new Control[] { _txtTitle, _txtGenre, _txtYear, _txtAuthor, _btnSave });

            _btnSave.Click += async (s, e) => await SaveAsync();
        }

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            if (!_bookId.HasValue) return;
            var book = await _bookService.GetByIdAsync(_bookId.Value);
            if (book != null)
            {
                _txtTitle.Text = book.Title;
                _txtGenre.Text = book.Genre?.GenreType ?? ""; ;
                _txtYear.Text = book.Year.ToString();
                _txtAuthor.Text = book.Author?.Name ?? "";
            }
        }

        private async System.Threading.Tasks.Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(_txtTitle.Text) ||
                string.IsNullOrWhiteSpace(_txtGenre.Text) ||
                string.IsNullOrWhiteSpace(_txtYear.Text) ||
                string.IsNullOrWhiteSpace(_txtAuthor.Text))
            {
                MessageBox.Show("Усі поля обов'язкові");
                return;
            }

            if (!int.TryParse(_txtYear.Text, out int year))
            {
                MessageBox.Show("Рік повинен бути числом!", "Помилка вводу", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _txtYear.Focus();
                return;
            }

            var genre = (await _genreService.GetAllAsync())
                .FirstOrDefault(g => g.GenreType.Equals(_txtGenre.Text, StringComparison.OrdinalIgnoreCase));
            if (genre == null)
                genre = new Genre { GenreType = _txtGenre.Text };

            var author = (await _authorService.GetAllAsync())
                .FirstOrDefault(a => a.Name.Equals(_txtAuthor.Text, StringComparison.OrdinalIgnoreCase));
            if (author == null)
                author = new Author { Name = _txtAuthor.Text };

            // Змінні для збереження старих ідентифікаторів при редагуванні
            int? oldAuthorId = null;
            int? oldGenreId = null;

            if (_bookId.HasValue)
            {
                var book = await _bookService.GetByIdAsync(_bookId.Value);
                if (book != null)
                {
                    // Зберігаємо старі ID для подальшої перевірки
                    oldAuthorId = book.AuthorId;
                    oldGenreId = book.GenreId;

                    // якщо пов'язані сутності нові — спочатку зберегти їх щоб отримати Id
                    if (author.Id == 0)
                        await _authorService.AddAsync(author);
                    else
                        await _authorService.UpdateAsync(author);

                    if (genre.Id == 0)
                        await _genreService.AddAsync(genre);
                    else
                        await _genreService.UpdateAsync(genre);

                    // потім призначаємо і оновлюємо книгу
                    book.Title = _txtTitle.Text;
                    book.AuthorId = author.Id;
                    book.GenreId = genre.Id;
                    book.Year = year;
                    book.Author = author;
                    book.Genre = genre;

                    await _bookService.UpdateAsync(book);

                    // Після успішного оновлення книги перевіряємо та видаляємо старі сутності, якщо вони більше не використовуються
                    if (oldAuthorId.HasValue && oldAuthorId.Value != author.Id)
                    {
                        await _authorService.DeleteAuthorIfNoBooksAsync(oldAuthorId.Value);
                    }
                    if (oldGenreId.HasValue && oldGenreId.Value != genre.Id)
                    {
                        await _genreService.DeleteGenreIfNoBooksAsync(oldGenreId.Value);
                    }
                }
            }
            else
            {
                // Для додавання нової книги
                if (author.Id == 0 && genre.Id == 0)
                {
                    var book = new Book
                    {
                        Title = _txtTitle.Text,
                        Genre = genre,
                        Year = year,
                        Author = author
                    };
                    await _bookService.AddAsync(book);
                }
                else
                {
                    // якщо один з зв'язаних вже існує, переконаємось, що у них є Id
                    if (author.Id == 0)
                        await _authorService.AddAsync(author);
                    if (genre.Id == 0)
                        await _genreService.AddAsync(genre);

                    var book = new Book
                    {
                        Title = _txtTitle.Text,
                        GenreId = genre.Id,
                        AuthorId = author.Id,
                        Year = year
                    };
                    await _bookService.AddAsync(book);
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

    }
}
