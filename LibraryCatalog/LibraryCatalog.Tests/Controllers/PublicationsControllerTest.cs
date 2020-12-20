using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LibraryCatalog.Controllers;
using LibraryCatalog.Data.Entities;
using LibraryCatalog.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace LibraryCatalog.Tests.Controllers
{
	public class PublicationsControllerTest : ControllerTestBase
	{
		private static PublicationDto ComposeTestPublicationDto(
			IEnumerable<Author> authors,
			IEnumerable<Review> reviews,
			Publisher publisher) => new PublicationDto
		{
			Title = "TestPublicationTitle",
			Description = "TestPublicationDescription",
			AuthorsIds = authors.Select(author => author.Id),
			ReviewsIds = reviews.Select(review => review.Id),
			PublisherId = publisher.Id
		};

		private static void AssertAreEqual(PublicationDto publicationDto, Publication publication)
		{
			Assert.AreEqual(publicationDto.Title, publication.Title);
			Assert.AreEqual(publicationDto.Description, publication.Description);
			Assert.AreEqual(publicationDto.AuthorsIds, publication.Authors.Select(author => author.Id));
			Assert.AreEqual(publicationDto.ReviewsIds, publication.Reviews.Select(review => review.Id));
			Assert.AreEqual(publicationDto.PublisherId, publication.Publisher.Id);
		}

		[Test]
		public async Task ShouldGetAllPublicationsProperly()
		{
			var publications = EntitiesGenerationRange.Select(_ => new Publication()).ToList();

			var context = ComposeEmptyDataContext();
			await context.Publications.AddRangeAsync(publications);
			await context.SaveChangesAsync();

			var mapperMock = new Mock<IMapper>();
			publications.ForEach(publication =>
				mapperMock.Setup(mapper => mapper.Map<PublicationBriefDto>(publication)).Verifiable());

			foreach (var _ in await new PublicationsController(context, mapperMock.Object).GetAllAsync())
			{
			}

			publications.ForEach(publication =>
				mapperMock.Verify(mapper => mapper.Map<PublicationBriefDto>(publication), Times.Once));
		}

		[Test]
		public async Task ShouldPostPublicationProperly()
		{
			var authors = EntitiesGenerationRange.Select(_ => new Author()).ToList();
			var reviews = EntitiesGenerationRange.Select(_ => new Review()).ToList();
			var publisher = new Publisher();

			var context = ComposeEmptyDataContext();
			await context.Authors.AddRangeAsync(authors);
			await context.Reviews.AddRangeAsync(reviews);
			await context.Publishers.AddAsync(publisher);
			await context.SaveChangesAsync();

			var publicationDto = ComposeTestPublicationDto(authors, reviews, publisher);

			await new PublicationsController(context, null).PostAsync(publicationDto);

			var publication = await context.Publications.SingleAsync();
			AssertAreEqual(publicationDto, publication);
		}

		[Test]
		public async Task ShouldGetPublicationProperly()
		{
			var publication = new Publication();

			var context = ComposeEmptyDataContext();
			await context.Publications.AddAsync(publication);
			await context.SaveChangesAsync();

			var mapperMock = new Mock<IMapper>();
			mapperMock.Setup(mapper => mapper.Map<PublicationDto>(publication)).Verifiable();

			await new PublicationsController(context, mapperMock.Object).GetAsync(publication.Id);

			mapperMock.Verify(mapper => mapper.Map<PublicationDto>(publication), Times.Once);
		}

		[Test]
		public void ShouldThrowExceptionWhenTryingToGetPublicationThatDoesNotExist()
		{
			var context = ComposeEmptyDataContext();

			Assert.ThrowsAsync<InvalidOperationException>(
				() => new PublicationsController(context, null).GetAsync(ComposeRandomId())
			);
		}

		[Test]
		public async Task ShouldPutPublicationProperly()
		{
			var publication = new Publication();
			var authors = EntitiesGenerationRange.Select(_ => new Author()).ToList();
			var reviews = EntitiesGenerationRange.Select(_ => new Review()).ToList();
			var publisher = new Publisher();

			var context = ComposeEmptyDataContext();
			await context.Publications.AddAsync(publication);
			await context.Authors.AddRangeAsync(authors);
			await context.Reviews.AddRangeAsync(reviews);
			await context.Publishers.AddAsync(publisher);
			await context.SaveChangesAsync();

			var publicationDto = ComposeTestPublicationDto(authors, reviews, publisher);

			await new PublicationsController(context, null).PutAsync(publication.Id, publicationDto);

			AssertAreEqual(publicationDto, publication);
		}

		[Test]
		public void ShouldThrowExceptionWhenTryingToPutPublicationThatDoesNotExist()
		{
			var context = ComposeEmptyDataContext();

			Assert.ThrowsAsync<InvalidOperationException>(
				() => new PublicationsController(context, null).PutAsync(ComposeRandomId(), new PublicationDto())
			);
		}

		[Test]
		public async Task ShouldDeletePublicationProperly()
		{
			var publication = new Publication();

			var context = ComposeEmptyDataContext();
			await context.Publications.AddAsync(publication);
			await context.SaveChangesAsync();

			await new PublicationsController(context, null).DeleteAsync(publication.Id);

			Assert.IsEmpty(context.Publications);
		}

		[Test]
		public void ShouldThrowExceptionWhenTryingToDeletePublicationThatDoesNotExist()
		{
			var context = ComposeEmptyDataContext();

			Assert.ThrowsAsync<InvalidOperationException>(
				() => new PublicationsController(context, null).DeleteAsync(ComposeRandomId())
			);
		}
	}
}