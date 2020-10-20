using System.Collections.Generic;

namespace LibraryCatalog.Data.Entities
{
    public class Publication
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public IEnumerable<Author> Authors { get; set; }

        public Publisher Publisher { get; set; }
    }
}