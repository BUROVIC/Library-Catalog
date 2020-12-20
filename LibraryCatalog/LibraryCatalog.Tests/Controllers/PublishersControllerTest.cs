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
	public class PublishersControllerTest : ControllerTestBase
	{
		private static PublisherDto ComposeTestPublisherDto(IEnumerable<Publication> publications) => new PublisherDto
		{
			Name = "TestPublisherName",
			Email = "TestPublisherEmail",
			PublicationsIds = publications.Select(publication => publication.Id)
		};

		private static void AssertAreEqual(PublisherDto publisherDto, Publisher publisher)
		{
			Assert.AreEqual(publisherDto.Name, publisher.Name);
			Assert.AreEqual(publisherDto.Email, publisher.Email);
			Assert.AreEqual(publisherDto.PublicationsIds, publisher.Publications.Select(publication => publication.Id));
		}

		[Test]
		public async Task ShouldGetAllPublishersProperly()
		{
			var publishers = EntitiesGenerationRange.Select(_ => new Publisher()).ToList();

			var context = ComposeEmptyDataContext();
			await context.Publishers.AddRangeAsync(publishers);
			await context.SaveChangesAsync();

			var mapperMock = new Mock<IMapper>();
			publishers.ForEach(publisher =>
				mapperMock.Setup(mapper => mapper.Map<PublisherBriefDto>(publisher)).Verifiable());

			foreach (var _ in await new PublishersController(context, mapperMock.Object).GetAllAsync())
			{
			}

			publishers.ForEach(publisher =>
				mapperMock.Verify(mapper => mapper.Map<PublisherBriefDto>(publisher), Times.Once));
		}

		[Test]
		public async Task ShouldPostPublisherProperly()
		{
			var publications = EntitiesGenerationRange.Select(_ => new Publication()).ToList();

			var context = ComposeEmptyDataContext();
			await context.Publications.AddRangeAsync(publications);
			await context.SaveChangesAsync();

			var publisherDto = ComposeTestPublisherDto(publications);

			await new PublishersController(context, null).PostAsync(publisherDto);

			var publisher = await context.Publishers.SingleAsync();
			AssertAreEqual(publisherDto, publisher);
		}

		[Test]
		public async Task ShouldGetPublisherProperly()
		{
			var publisher = new Publisher();

			var context = ComposeEmptyDataContext();
			await context.Publishers.AddAsync(publisher);
			await context.SaveChangesAsync();

			var mapperMock = new Mock<IMapper>();
			mapperMock.Setup(mapper => mapper.Map<PublisherDto>(publisher)).Verifiable();

			await new PublishersController(context, mapperMock.Object).GetAsync(publisher.Id);

			mapperMock.Verify(mapper => mapper.Map<PublisherDto>(publisher), Times.Once);
		}

		[Test]
		public void ShouldThrowExceptionWhenTryingToGetPublisherThatDoesNotExist()
		{
			var context = ComposeEmptyDataContext();

			Assert.ThrowsAsync<InvalidOperationException>(
				() => new PublishersController(context, null).GetAsync(ComposeRandomId())
			);
		}

		[Test]
		public async Task ShouldPutPublisherProperly()
		{
			var publisher = new Publisher();
			var publications = EntitiesGenerationRange.Select(_ => new Publication()).ToList();

			var context = ComposeEmptyDataContext();
			await context.Publishers.AddAsync(publisher);
			await context.Publications.AddRangeAsync(publications);
			await context.SaveChangesAsync();

			var publisherDto = ComposeTestPublisherDto(publications);

			await new PublishersController(context, null).PutAsync(publisher.Id, publisherDto);

			AssertAreEqual(publisherDto, publisher);
		}

		[Test]
		public void ShouldThrowExceptionWhenTryingToPutPublisherThatDoesNotExist()
		{
			var context = ComposeEmptyDataContext();

			Assert.ThrowsAsync<InvalidOperationException>(
				() => new PublishersController(context, null).PutAsync(ComposeRandomId(), new PublisherDto())
			);
		}

		[Test]
		public async Task ShouldDeletePublisherProperly()
		{
			var publisher = new Publisher();

			var context = ComposeEmptyDataContext();
			await context.Publishers.AddAsync(publisher);
			await context.SaveChangesAsync();

			await new PublishersController(context, null).DeleteAsync(publisher.Id);

			Assert.IsEmpty(context.Publishers);
		}

		[Test]
		public void ShouldThrowExceptionWhenTryingToDeletePublisherThatDoesNotExist()
		{
			var context = ComposeEmptyDataContext();

			Assert.ThrowsAsync<InvalidOperationException>(
				() => new PublishersController(context, null).DeleteAsync(ComposeRandomId())
			);
		}
	}
}