using LibraryApp.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryApp.Domain
{
    public class Book
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;

        public int GenreId { get; set; }
        public Genre Genre { get; set; } = null!;
    }
}

