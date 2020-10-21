namespace LibraryCatalog.Models
{
    public class ReviewDto
    {
        public string ReviewerName { get; set; }

        public bool IsPositive { get; set; }

        public string Comment { get; set; }

        public int PublicationId { get; set; }
    }
}