using BookOrganizer2.DA.Repositories;
using BookOrganizer2.Domain.BookProfile.LanguageProfile;
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
        public async Task Language_inserted_to_database()
        {
            var language = await LanguageHelpers.CreateValidLanguage();
            var repository = new LanguageRepository(_fixture.Context);

            (await repository.ExistsAsync(language.Id)).Should().BeTrue();
        }

        [Fact]
        public void Invalid_Language()
        {
            Func<Task> action = async () => { await LanguageHelpers.CreateInvalidLanguage(); };
            action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task Update_Language()
        {
            var language = await LanguageHelpers.CreateValidLanguage();
            language.Name.Should().Be("pig latin");

            var sut = Language.Create(language.Id, "english");

            await LanguageHelpers.UpdateLanguage(sut);

            await _fixture.Context.Entry(language).ReloadAsync();

            language.Name.Should().Be("english");
        }

        [Fact]
        public async Task Remove_Language()
        {
            var language = await LanguageHelpers.CreateValidLanguage();

            var repository = new LanguageRepository(_fixture.Context);
            (await repository.ExistsAsync(language.Id)).Should().BeTrue();

            await LanguageHelpers.RemoveLanguage(language.Id);

            var sut = await repository.GetAsync(language.Id);
            await _fixture.Context.Entry(sut).ReloadAsync();
            (await repository.ExistsAsync(language.Id)).Should().BeFalse();
        }
    }
}