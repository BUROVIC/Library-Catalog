using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryCatalog.Data.Entities
{
	public class Publication
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public List<Author> Authors { get; set; }

		public List<Review> Reviews { get; set; }

		public Publisher Publisher { get; set; }
	}
}