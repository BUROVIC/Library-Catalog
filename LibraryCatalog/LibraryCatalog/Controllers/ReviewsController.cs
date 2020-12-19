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
	public class ReviewsController : ControllerBase
	{
		private readonly LibraryCatalogDbContext _dataContext;

		private readonly IMapper _mapper;

		public ReviewsController(LibraryCatalogDbContext dataContext, IMapper mapper)
		{
			_dataContext = dataContext;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IEnumerable<ReviewBriefDto>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			var reviews = await _dataContext.Reviews.ToListAsync(cancellationToken);

			return reviews.Select(review => _mapper.Map<ReviewBriefDto>(review));
		}

		[HttpPost]
		public async Task PostAsync(ReviewDto reviewDto, CancellationToken cancellationToken = default)
		{
			var review = new Review
			{
				ReviewerName = reviewDto.ReviewerName,
				IsPositive = reviewDto.IsPositive,
				Comment = reviewDto.Comment,
				Publication = reviewDto.PublicationId != null
					? await _dataContext.Publications.SingleAsync(publication =>
							publication.Id == reviewDto.PublicationId, cancellationToken
					)
					: null
			};

			await _dataContext.Reviews.AddAsync(review, cancellationToken);
			await _dataContext.SaveChangesAsync(cancellationToken);
		}

		[HttpGet("{id}")]
		public async Task<ReviewDto> GetAsync(int id, CancellationToken cancellationToken = default)
		{
			return _mapper.Map<ReviewDto>(
				await _dataContext.Reviews
					.Include(review => review.Publication)
					.SingleAsync(review => review.Id == id, cancellationToken)
			);
		}

		[HttpPut("{id}")]
		public async Task PutAsync(int id, ReviewDto reviewDto, CancellationToken cancellationToken = default)
		{
			var reviewToUpdate = await _dataContext.Reviews
				.Include(review => review.Publication)
				.SingleAsync(review => review.Id == id, cancellationToken);

			reviewToUpdate.ReviewerName = reviewDto.ReviewerName;
			reviewToUpdate.IsPositive = reviewDto.IsPositive;
			reviewToUpdate.Comment = reviewDto.Comment;
			reviewToUpdate.Publication = reviewDto.PublicationId != null
				? await _dataContext.Publications.SingleAsync(publication =>
						publication.Id == reviewDto.PublicationId, cancellationToken
				)
				: null;

			await _dataContext.SaveChangesAsync(cancellationToken);
		}

		[HttpDelete("{id}")]
		public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
		{
			_dataContext.Reviews.Remove(
				await _dataContext.Reviews.SingleAsync(review => review.Id == id, cancellationToken)
			);

			await _dataContext.SaveChangesAsync(cancellationToken);
		}
	}
}