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
	public class PublishersController : ControllerBase
	{
		private readonly LibraryCatalogDbContext _dataContext;

		private readonly IMapper _mapper;

		public PublishersController(LibraryCatalogDbContext dataContext, IMapper mapper)
		{
			_dataContext = dataContext;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IEnumerable<PublisherBriefDto>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			var publishers = await _dataContext.Publishers.ToListAsync(cancellationToken);

			return publishers.Select(publisher => _mapper.Map<PublisherBriefDto>(publisher));
		}

		[HttpPost]
		public async Task PostAsync(PublisherDto publisherDto, CancellationToken cancellationToken = default)
		{
			var publisher = new Publisher
			{
				Name = publisherDto.Name,
				Email = publisherDto.Email,
				Publications = (await Task.WhenAll(
					publisherDto.PublicationsIds.Select(async publicationId =>
						await _dataContext.Publications.SingleAsync(
							publication => publication.Id == publicationId, cancellationToken
						)
					)
				)).ToList()
			};

			await _dataContext.Publishers.AddAsync(publisher, cancellationToken);
			await _dataContext.SaveChangesAsync(cancellationToken);
		}

		[HttpGet("{id}")]
		public async Task<PublisherDto> GetAsync(int id, CancellationToken cancellationToken = default)
		{
			return _mapper.Map<PublisherDto>(
				await _dataContext.Publishers
					.Include(publisher => publisher.Publications)
					.SingleAsync(publisher => publisher.Id == id, cancellationToken)
			);
		}

		[HttpPut("{id}")]
		public async Task PutAsync(int id, PublisherDto publisherDto, CancellationToken cancellationToken = default)
		{
			var publisherToUpdate =
				await _dataContext.Publishers
					.Include(publisher => publisher.Publications)
					.SingleAsync(publisher => publisher.Id == id, cancellationToken);

			publisherToUpdate.Name = publisherDto.Name;
			publisherToUpdate.Email = publisherDto.Email;
			publisherToUpdate.Publications = (await Task.WhenAll(
				publisherDto.PublicationsIds.Select(async publicationId =>
					await _dataContext.Publications.SingleAsync(
						publication => publication.Id == publicationId, cancellationToken
					)
				)
			)).ToList();

			await _dataContext.SaveChangesAsync(cancellationToken);
		}

		[HttpDelete("{id}")]
		public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
		{
			_dataContext.Publishers.Remove(
				await _dataContext.Publishers.SingleAsync(author => author.Id == id, cancellationToken)
			);

			await _dataContext.SaveChangesAsync(cancellationToken);
		}
	}
}