using BookOrganizer2.DA.Repositories;
using BookOrganizer2.Domain.BookProfile.SeriesProfile;
using BookOrganizer2.IntegrationTests.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookOrganizer2.Domain.BookProfile;
using Xunit;

namespace BookOrganizer2.IntegrationTests
{
    public sealed partial class DatabaseTests
    {
        [Trait("Integration", "DB\\Series")]
        [Fact]
        public async Task Series_inserted_to_database()
        {
            var series = await SeriesHelpers.CreateValidSeries();
            var repository = new SeriesRepository(_fixture.Context);

            (await repository.ExistsAsync(series.Id)).Should().BeTrue();
        }

        [Trait("Integration", "DB\\Series")]
        [Fact]
        public void Invalid_Series()
        {
            Func<Task> action = async () => { await SeriesHelpers.CreateInvalidSeries(); };
            action.Should().ThrowAsync<ArgumentException>();
        }

        [Trait("Integration", "DB\\Series")]
        [Fact]
        public async Task Update_Series()
        {
            var series = await SeriesHelpers.CreateValidSeries();
            series.Name.Should().Be("Series to be");

            var sut = Series.Create(series.Id,
                "Series of books",
                @"\\pics\logo.jpg",
                "...");

            await SeriesHelpers.UpdateSeries(sut);

            await _fixture.Context.Entry(series).ReloadAsync();

            series.Name.Should().Be("Series of books");
        }

        [Trait("Integration", "DB\\Series")]
        [Fact]
        public async Task Update_Series_Name()
        {
            var series = await SeriesHelpers.CreateValidSeries();

            var repository = new SeriesRepository(_fixture.Context);
            (await repository.ExistsAsync(series.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(series.Id);

            var seriesId = sut.Id;

            sut.Should().NotBeNull();
            sut.Name.Should().Be("Series to be");

            await SeriesHelpers.UpdateSeriesName(sut.Id, "Series of more books");

            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.Name.Should().Be("Series of more books");
            sut.Id.Should().Be(seriesId);
        }

        [Trait("Integration", "DB\\Series")]
        [Fact]
        public async Task Update_Series_LogoPath()
        {
            var series = await SeriesHelpers.CreateValidSeries();

            var repository = new SeriesRepository(_fixture.Context);
            (await repository.ExistsAsync(series.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(series.Id);

            var seriesId = sut.Id;

            sut.Should().NotBeNull();
            sut.PicturePath.Should().Be(@"\\filepath\file.jpg");

            await SeriesHelpers.UpdateSeriesPicturePath(sut.Id, @"\\filepath\newFile.jpg");
            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.PicturePath.Should().Be(@"\\filepath\newFile.jpg");
            sut.Id.Should().Be(seriesId);
        }

        [Trait("Integration", "DB\\Series")]
        [Fact]
        public async Task Update_Series_Description()
        {
            var series = await SeriesHelpers.CreateValidSeries();

            var repository = new SeriesRepository(_fixture.Context);
            (await repository.ExistsAsync(series.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(series.Id);

            var seriesId = sut.Id;

            sut.Should().NotBeNull();
            sut.Description.Should().Contain("Best books in the world!");

            await SeriesHelpers.UpdateSeriesDescription(sut.Id, "Bacon ipsum...");

            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.Description.Should().Contain("Bacon ipsum");
            sut.Id.Should().Be(seriesId);
        }

        [Trait("Integration", "DB\\Series")]
        [Fact]
        public async Task Update_Series_Books()
        {
            var series = await SeriesHelpers.CreateValidSeriesWithBooks();

            var repository = new SeriesRepository(_fixture.Context);
            (await repository.ExistsAsync(series.Id)).Should().BeTrue();

            var sut = await repository.LoadAsync(series.Id);

            var seriesId = sut.Id;

            sut.Should().NotBeNull();
            sut.Books.Count.Should().Be(2);

            // Add one more book to series
            var book1 = await BookHelpers.CreateValidBook();
            var book2 = await BookHelpers.CreateValidBook();
            var newReadOrder = new List<ReadOrder>
            {
                ReadOrder.NewReadOrder(book1, null, 3),
                ReadOrder.NewReadOrder(book2, null, 4)
            };
            await SeriesHelpers.UpdateSeriesReadOrder(sut.Id, newReadOrder);

            sut = await repository.LoadAsync(series.Id);

            sut.Books.Count.Should().Be(4);
            sut.Id.Should().Be(seriesId);
        }

        [Trait("Integration", "DB\\Series")]
        [Fact]
        public async Task Set_Series_Read_Order()
        {
            var series = await SeriesHelpers.CreateValidSeriesWithBooks();

            var repository = new SeriesRepository(_fixture.Context);
            (await repository.ExistsAsync(series.Id)).Should().BeTrue();

            var sut = await repository.LoadAsync(series.Id);

            var seriesId = sut.Id;

            sut.Should().NotBeNull();
            sut.Books.Count.Should().Be(2);
            sut.Books.SingleOrDefault(b => b.Instalment == 1)?.BooksId
                .Should().Be(sut.Books.SingleOrDefault(r => r.Book.Title == "Book 1")?.BooksId);
            sut.Books.SingleOrDefault(b => b.Instalment == 2)?.BooksId
                .Should().Be(sut.Books.SingleOrDefault(r => r.Book.Title == "Book 2")?.BooksId);

            // Re-order collection
            sut = await repository.LoadAsync(series.Id);

            sut.Books.SingleOrDefault(b => b.Instalment == 1).Instalment = 0;
            sut.Books.SingleOrDefault(b => b.Instalment == 2).Instalment = 1;
            sut.Books.SingleOrDefault(b => b.Instalment == 0).Instalment = 2;

            await repository.SaveAsync();
            sut = await repository.LoadAsync(series.Id);
            sut.Books.SingleOrDefault(b => b.Instalment == 2)?.BooksId
                .Should().Be(sut.Books.SingleOrDefault(r => r.Book.Title == "Book 1")?.BooksId);
            sut.Books.SingleOrDefault(b => b.Instalment == 1)?.BooksId
                .Should().Be(sut.Books.SingleOrDefault(r => r.Book.Title == "Book 2")?.BooksId);
        }

        [Trait("Integration", "DB\\Series")]
        [Fact]
        public async Task Remove_Series()
        {
            var series = await SeriesHelpers.CreateValidSeries();

            var repository = new SeriesRepository(_fixture.Context); 
            (await repository.ExistsAsync(series.Id)).Should().BeTrue();
            
            await SeriesHelpers.RemoveSeries(series.Id);

            var sut = await repository.GetAsync(series.Id);
            await _fixture.Context.Entry(sut).ReloadAsync();
            (await repository.ExistsAsync(series.Id)).Should().BeFalse();
        }
    }
}