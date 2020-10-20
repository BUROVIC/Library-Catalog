using System.Collections.Generic;

namespace LibraryCatalog.Data.Entities
{
    public class Publisher
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public IEnumerable<Publication> Publications { get; set; }
    }
}