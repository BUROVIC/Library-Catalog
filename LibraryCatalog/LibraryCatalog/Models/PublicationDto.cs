using System.Collections.Generic;

namespace LibraryCatalog.Models
{
    public class PublicationDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public IEnumerable<int> AuthorsIds { get; set; }
        
        public IEnumerable<int> ReviewsIds { get; set; }

        public int? PublisherId { get; set; }
    }
}