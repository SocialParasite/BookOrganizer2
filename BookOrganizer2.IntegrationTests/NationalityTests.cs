using BookOrganizer2.DA.Repositories;
using BookOrganizer2.Domain.AuthorProfile.NationalityProfile;
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
        public async Task Nationality_inserted_to_database()
        {
            var nationality = await NationalityHelpers.CreateValidNationality();
            var repository = new NationalityRepository(_fixture.Context);

            (await repository.ExistsAsync(nationality.Id)).Should().BeTrue();
        }

        [Fact]
        public void Invalid_Nationality()
        {
            Func<Task> action = async () => { await NationalityHelpers.CreateInvalidNationality(); };
            action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task Update_Nationality()
        {
            var nationality = await NationalityHelpers.CreateValidNationality();
            nationality.Name.Should().Be("american");

            var sut = Nationality.Create(nationality.Id, "russian");

            await NationalityHelpers.UpdateNationality(sut);

            await _fixture.Context.Entry(nationality).ReloadAsync();

            nationality.Name.Should().Be("russian");
        }

        [Fact]
        public async Task Remove_Nationality()
        {
            var nationality = await NationalityHelpers.CreateValidNationality();

            var repository = new NationalityRepository(_fixture.Context);
            (await repository.ExistsAsync(nationality.Id)).Should().BeTrue();

            await NationalityHelpers.RemoveNationality(nationality.Id);

            var sut = await repository.GetAsync(nationality.Id);
            await _fixture.Context.Entry(sut).ReloadAsync();
            (await repository.ExistsAsync(nationality.Id)).Should().BeFalse();
        }
    }
}