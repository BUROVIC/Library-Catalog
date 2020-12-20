using System;
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
	public class AuthorsControllerTest : ControllerTestBase
	{
		private static AuthorDto ComposeTestAuthorDto() => new AuthorDto
		{
			Name = "TestAuthorName",
			Biography = "TestAuthorBiography"
		};

		private static void AssertAreEqual(AuthorDto authorDto, Author author)
		{
			Assert.AreEqual(authorDto.Name, author.Name);
			Assert.AreEqual(authorDto.Biography, author.Biography);
		}

		[Test]
		public async Task ShouldGetAllAuthorsProperly()
		{
			var authors = EntitiesGenerationRange.Select(_ => new Author()).ToList();

			var context = ComposeEmptyDataContext();
			await context.Authors.AddRangeAsync(authors);
			await context.SaveChangesAsync();

			var mapperMock = new Mock<IMapper>();
			authors.ForEach(author => mapperMock.Setup(mapper => mapper.Map<AuthorBriefDto>(author)).Verifiable());

			foreach (var _ in await new AuthorsController(context, mapperMock.Object).GetAllAsync())
			{
			}

			authors.ForEach(author => mapperMock.Verify(mapper => mapper.Map<AuthorBriefDto>(author), Times.Once));
		}

		[Test]
		public async Task ShouldPostAuthorProperly()
		{
			var context = ComposeEmptyDataContext();

			var authorDto = ComposeTestAuthorDto();

			await new AuthorsController(context, null).PostAsync(authorDto);

			var author = await context.Authors.SingleAsync();
			AssertAreEqual(authorDto, author);
		}

		[Test]
		public async Task ShouldGetAuthorProperly()
		{
			var author = new Author();

			var context = ComposeEmptyDataContext();
			await context.Authors.AddAsync(author);
			await context.SaveChangesAsync();

			var mapperMock = new Mock<IMapper>();
			mapperMock.Setup(mapper => mapper.Map<AuthorDto>(author)).Verifiable();

			await new AuthorsController(context, mapperMock.Object).GetAsync(author.Id);

			mapperMock.Verify(mapper => mapper.Map<AuthorDto>(author), Times.Once);
		}

		[Test]
		public void ShouldThrowExceptionWhenTryingToGetAuthorThatDoesNotExist()
		{
			var context = ComposeEmptyDataContext();

			Assert.ThrowsAsync<InvalidOperationException>(
				() => new AuthorsController(context, null).GetAsync(ComposeRandomId())
			);
		}

		[Test]
		public async Task ShouldPutAuthorProperly()
		{
			var author = new Author();

			var context = ComposeEmptyDataContext();
			await context.Authors.AddAsync(author);
			await context.SaveChangesAsync();

			var authorDto = ComposeTestAuthorDto();

			await new AuthorsController(context, null).PutAsync(author.Id, authorDto);

			AssertAreEqual(authorDto, author);
		}

		[Test]
		public void ShouldThrowExceptionWhenTryingToPutAuthorThatDoesNotExist()
		{
			var context = ComposeEmptyDataContext();

			Assert.ThrowsAsync<InvalidOperationException>(
				() => new AuthorsController(context, null).PutAsync(ComposeRandomId(), new AuthorDto())
			);
		}

		[Test]
		public async Task ShouldDeleteAuthorProperly()
		{
			var author = new Author();

			var context = ComposeEmptyDataContext();
			await context.Authors.AddAsync(author);
			await context.SaveChangesAsync();

			await new AuthorsController(context, null).DeleteAsync(author.Id);

			Assert.IsEmpty(context.Authors);
		}

		[Test]
		public void ShouldThrowExceptionWhenTryingToDeleteAuthorThatDoesNotExist()
		{
			var context = ComposeEmptyDataContext();

			Assert.ThrowsAsync<InvalidOperationException>(
				() => new AuthorsController(context, null).DeleteAsync(ComposeRandomId())
			);
		}
	}
}