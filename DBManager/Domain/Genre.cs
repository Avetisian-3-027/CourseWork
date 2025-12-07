using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryApp.Domain
{
    public class Genre
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string GenreType { get; set; } = string.Empty;
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
