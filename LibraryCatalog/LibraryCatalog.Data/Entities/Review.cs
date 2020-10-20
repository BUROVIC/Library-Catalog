namespace LibraryCatalog.Data.Entities
{
    public class Review
    {
        public int Id { get; set; }

        public string ReviewerName { get; set; }

        public bool IsPositive { get; set; }

        public string Comment { get; set; }

        public Publication Publication { get; set; }
    }
}