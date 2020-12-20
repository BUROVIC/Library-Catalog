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
	public class ReviewsControllerTest : ControllerTestBase
	{
		private static ReviewDto ComposeTestReviewDto(Publication publication) => new ReviewDto
		{
			ReviewerName = "TestReviewReviewerName",
			IsPositive = true,
			PublicationId = publication.Id
		};

		private static void AssertAreEqual(ReviewDto reviewDto, Review review)
		{
			Assert.AreEqual(reviewDto.ReviewerName, review.ReviewerName);
			Assert.AreEqual(reviewDto.IsPositive, review.IsPositive);
			Assert.AreEqual(reviewDto.PublicationId, review.Publication.Id);
		}

		[Test]
		public async Task ShouldGetAllReviewsProperly()
		{
			var reviews = EntitiesGenerationRange.Select(_ => new Review()).ToList();

			var context = ComposeEmptyDataContext();
			await context.Reviews.AddRangeAsync(reviews);
			await context.SaveChangesAsync();

			var mapperMock = new Mock<IMapper>();
			reviews.ForEach(review =>
				mapperMock.Setup(mapper => mapper.Map<ReviewBriefDto>(review)).Verifiable());

			foreach (var _ in await new ReviewsController(context, mapperMock.Object).GetAllAsync())
			{
			}

			reviews.ForEach(review =>
				mapperMock.Verify(mapper => mapper.Map<ReviewBriefDto>(review), Times.Once));
		}

		[Test]
		public async Task ShouldPostReviewProperly()
		{
			var publication = new Publication();

			var context = ComposeEmptyDataContext();
			await context.Publications.AddAsync(publication);
			await context.SaveChangesAsync();

			var reviewDto = ComposeTestReviewDto(publication);

			await new ReviewsController(context, null).PostAsync(reviewDto);

			var review = await context.Reviews.SingleAsync();
			AssertAreEqual(reviewDto, review);
		}

		[Test]
		public async Task ShouldGetReviewProperly()
		{
			var review = new Review();

			var context = ComposeEmptyDataContext();
			await context.Reviews.AddAsync(review);
			await context.SaveChangesAsync();

			var mapperMock = new Mock<IMapper>();
			mapperMock.Setup(mapper => mapper.Map<ReviewDto>(review)).Verifiable();

			await new ReviewsController(context, mapperMock.Object).GetAsync(review.Id);

			mapperMock.Verify(mapper => mapper.Map<ReviewDto>(review), Times.Once);
		}

		[Test]
		public void ShouldThrowExceptionWhenTryingToGetReviewThatDoesNotExist()
		{
			var context = ComposeEmptyDataContext();

			Assert.ThrowsAsync<InvalidOperationException>(
				() => new ReviewsController(context, null).GetAsync(ComposeRandomId())
			);
		}

		[Test]
		public async Task ShouldPutReviewProperly()
		{
			var review = new Review();
			var publication = new Publication();

			var context = ComposeEmptyDataContext();
			await context.Reviews.AddAsync(review);
			await context.Publications.AddAsync(publication);
			await context.SaveChangesAsync();

			var reviewDto = ComposeTestReviewDto(publication);

			await new ReviewsController(context, null).PutAsync(review.Id, reviewDto);

			AssertAreEqual(reviewDto, review);
		}

		[Test]
		public void ShouldThrowExceptionWhenTryingToPutReviewThatDoesNotExist()
		{
			var context = ComposeEmptyDataContext();

			Assert.ThrowsAsync<InvalidOperationException>(
				() => new ReviewsController(context, null).PutAsync(ComposeRandomId(), new ReviewDto())
			);
		}

		[Test]
		public async Task ShouldDeleteReviewProperly()
		{
			var review = new Review();

			var context = ComposeEmptyDataContext();
			await context.Reviews.AddAsync(review);
			await context.SaveChangesAsync();

			await new ReviewsController(context, null).DeleteAsync(review.Id);

			Assert.IsEmpty(context.Reviews);
		}

		[Test]
		public void ShouldThrowExceptionWhenTryingToDeleteReviewThatDoesNotExist()
		{
			var context = ComposeEmptyDataContext();

			Assert.ThrowsAsync<InvalidOperationException>(
				() => new ReviewsController(context, null).DeleteAsync(ComposeRandomId())
			);
		}
	}
}