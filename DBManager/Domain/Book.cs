using LibraryApp.Domain;

public class Book : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public uint Year { get; set; }

    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;

    public int GenreId { get; set; }
    public Genre Genre { get; set; } = null!;
}