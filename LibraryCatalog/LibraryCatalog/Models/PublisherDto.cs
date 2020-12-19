using System.Collections.Generic;

namespace LibraryCatalog.Models
{
	public class PublisherDto
	{
		public string Name { get; set; }

		public string Email { get; set; }

		public IEnumerable<int> PublicationsIds { get; set; }
	}
}