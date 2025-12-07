using System.Collections.Generic;

namespace LibraryApp.Domain
{
    public class Genre : BaseEntity
    {
        public string GenreType { get; set; } = string.Empty;
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
