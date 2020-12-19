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
	public class PublicationsController : ControllerBase
	{
		private readonly LibraryCatalogDbContext _dataContext;

		private readonly IMapper _mapper;

		public PublicationsController(LibraryCatalogDbContext dataContext, IMapper mapper)
		{
			_dataContext = dataContext;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IEnumerable<PublicationBriefDto>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			var publications = await _dataContext.Publications.ToListAsync(cancellationToken);

			return publications.Select(publication => _mapper.Map<PublicationBriefDto>(publication));
		}

		[HttpPost]
		public async Task PostAsync(PublicationDto publicationDto, CancellationToken cancellationToken = default)
		{
			var publication = new Publication
			{
				Title = publicationDto.Title,
				Description = publicationDto.Description,
				Authors = (await Task.WhenAll(
					publicationDto.AuthorsIds.Select(async authorId =>
						await _dataContext.Authors.SingleAsync(author => author.Id == authorId, cancellationToken)
					)
				)).ToList(),
				Reviews = (await Task.WhenAll(
					publicationDto.ReviewsIds.Select(async reviewId =>
						await _dataContext.Reviews.SingleAsync(review => review.Id == reviewId, cancellationToken)
					)
				)).ToList(),
				Publisher = publicationDto.PublisherId != null
					? await _dataContext.Publishers.SingleAsync(publisher =>
							publisher.Id == publicationDto.PublisherId, cancellationToken
					)
					: null
			};

			await _dataContext.Publications.AddAsync(publication, cancellationToken);
			await _dataContext.SaveChangesAsync(cancellationToken);
		}

		[HttpGet("{id}")]
		public async Task<PublicationDto> GetAsync(int id, CancellationToken cancellationToken = default)
		{
			return _mapper.Map<PublicationDto>(
				await _dataContext.Publications
					.Include(publication => publication.Authors)
					.Include(publication => publication.Reviews)
					.Include(publication => publication.Publisher)
					.SingleAsync(publication => publication.Id == id, cancellationToken)
			);
		}

		[HttpPut("{id}")]
		public async Task PutAsync(int id, PublicationDto publicationDto, CancellationToken cancellationToken = default)
		{
			var publicationToUpdate = await _dataContext.Publications
				.Include(publication => publication.Authors)
				.Include(publication => publication.Reviews)
				.Include(publication => publication.Publisher)
				.SingleAsync(publication => publication.Id == id, cancellationToken);

			publicationToUpdate.Title = publicationDto.Title;
			publicationToUpdate.Description = publicationDto.Description;
			publicationToUpdate.Authors = (await Task.WhenAll(
				publicationDto.AuthorsIds.Select(async authorId =>
					await _dataContext.Authors.SingleAsync(author => author.Id == authorId, cancellationToken)
				)
			)).ToList();
			publicationToUpdate.Reviews = (await Task.WhenAll(
				publicationDto.ReviewsIds.Select(async reviewId =>
					await _dataContext.Reviews.SingleAsync(review => review.Id == reviewId, cancellationToken)
				)
			)).ToList();
			publicationToUpdate.Publisher = publicationDto.PublisherId != null
				? await _dataContext.Publishers.SingleAsync(publisher =>
						publisher.Id == publicationDto.PublisherId, cancellationToken
				)
				: null;

			await _dataContext.SaveChangesAsync(cancellationToken);
		}

		[HttpDelete("{id}")]
		public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
		{
			_dataContext.Publications.Remove(
				await _dataContext.Publications.SingleAsync(author => author.Id == id, cancellationToken)
			);

			await _dataContext.SaveChangesAsync(cancellationToken);
		}
	}
}