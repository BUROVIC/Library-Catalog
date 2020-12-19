using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LibraryCatalog.Data;
using LibraryCatalog.Data.Entities;
using LibraryCatalog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryCatalog.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AuthorsController : ControllerBase
	{
		private readonly LibraryCatalogDbContext _dataContext;

		private readonly IMapper _mapper;

		public AuthorsController(LibraryCatalogDbContext dataContext, IMapper mapper)
		{
			_dataContext = dataContext;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IEnumerable<AuthorBriefDto>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			var authors = await _dataContext.Authors.ToListAsync(cancellationToken);

			return authors.Select(author => _mapper.Map<AuthorBriefDto>(author));
		}

		[HttpPost]
		public async Task PostAsync(AuthorDto authorDto, CancellationToken cancellationToken = default)
		{
			var author = new Author
			{
				Name = authorDto.Name,
				Biography = authorDto.Biography
			};

			await _dataContext.Authors.AddAsync(author, cancellationToken);
			await _dataContext.SaveChangesAsync(cancellationToken);
		}

		[HttpGet("{id}")]
		public async Task<AuthorDto> GetAsync(int id, CancellationToken cancellationToken = default)
		{
			return _mapper.Map<AuthorDto>(
				await _dataContext.Authors.SingleAsync(author => author.Id == id, cancellationToken)
			);
		}

		[HttpPut("{id}")]
		public async Task PutAsync(int id, AuthorDto authorDto, CancellationToken cancellationToken = default)
		{
			var authorToUpdate =
				await _dataContext.Authors.SingleAsync(author => author.Id == id, cancellationToken);

			authorToUpdate.Name = authorDto.Name;
			authorToUpdate.Biography = authorDto.Biography;

			await _dataContext.SaveChangesAsync(cancellationToken);
		}

		[HttpDelete("{id}")]
		public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
		{
			_dataContext.Authors.Remove(
				await _dataContext.Authors.SingleAsync(author => author.Id == id, cancellationToken)
			);

			await _dataContext.SaveChangesAsync(cancellationToken);
		}
	}
}