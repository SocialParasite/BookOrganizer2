using BookOrganizer2.DA.Repositories;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.IntegrationTests.Helpers;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BookOrganizer2.IntegrationTests
{
    public sealed partial class DatabaseTests
    {
        [Fact]
        public async Task Genre_inserted_to_database()
        {
            var genre = await GenreHelpers.CreateValidGenre();
            var repository = new GenreRepository(_fixture.Context);

            (await repository.ExistsAsync(genre.Id)).Should().BeTrue();
        }

        [Fact]
        public void Invalid_Genre()
        {
            Func<Task> action = async () => { await GenreHelpers.CreateInvalidGenre(); };
            action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task Update_Genre()
        {
            var genre = await GenreHelpers.CreateValidGenre();
            genre.Name.Should().Be("sci-fi");

            var sut = Genre.Create(genre.Id, "fantasy");

            await GenreHelpers.UpdateGenre(sut);

            await _fixture.Context.Entry(genre).ReloadAsync();

            genre.Name.Should().Be("fantasy");
        }

        [Fact]
        public async Task Remove_Genre()
        {
            var genre = await GenreHelpers.CreateValidGenre();

            var repository = new GenreRepository(_fixture.Context);
            (await repository.ExistsAsync(genre.Id)).Should().BeTrue();

            await GenreHelpers.RemoveGenre(genre.Id);

            var sut = await repository.GetAsync(genre.Id);
            await _fixture.Context.Entry(sut).ReloadAsync();
            (await repository.ExistsAsync(genre.Id)).Should().BeFalse();
        }
    }
}