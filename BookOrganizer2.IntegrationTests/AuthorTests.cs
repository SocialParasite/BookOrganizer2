using BookOrganizer2.DA.Repositories;
using BookOrganizer2.Domain.AuthorProfile;
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
        public async Task Author_inserted_to_database()
        {
            var author = await AuthorHelpers.CreateValidAuthor();
            var repository = new AuthorRepository(_fixture.Context);

            (await repository.ExistsAsync(author.Id)).Should().BeTrue();
        }

        [Fact]
        public async Task Author_with_all_properties_inserted_to_database()
        {
            var author = await AuthorHelpers.CreateValidAuthorWithNationality();
            var repository = new AuthorRepository(_fixture.Context);

            (await repository.ExistsAsync(author.Id)).Should().BeTrue();
            author = await repository.LoadAsync(author.Id);

            author.FirstName.Should().Be("Patrick");
            author.LastName.Should().Be("Rothfuss");
            author.DateOfBirth.Should().Be(new DateTime(1973, 6, 6));
            author.MugshotPath.Should().Be(@"\\filepath\file.jpg");
            author.Biography.Should().Be("There is no book number three.");
            author.Notes.Should().Be("...");
            author.Nationality.Name.Should().Be("american");
        }

        [Fact]
        public void Invalid_Author()
        {
            Func<Task> action = async () => { await AuthorHelpers.CreateInvalidAuthor(); };
            action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task Update_Author()
        {
            var author = await AuthorHelpers.CreateValidAuthor();
            author.LastName.Should().Be("Rothfuss");

            var sut = Author.Create(author.Id,
                "Scott", "Lynch",
                new DateTime(1978, 4, 2),
                "bio", @"\\pics\scott.jpg", "notes");

            await AuthorHelpers.UpdateAuthor(sut);

            await _fixture.Context.Entry(author).ReloadAsync();

            author.LastName.Should().Be("Lynch");
        }

        [Fact]
        public async Task Update_Author_FirstName()
        {
            var author = await AuthorHelpers.CreateValidAuthor();

            var repository = new AuthorRepository(_fixture.Context);
            (await repository.ExistsAsync(author.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(author.Id);

            var authorId = sut.Id;

            sut.Should().NotBeNull();
            sut.FirstName.Should().Be("Patrick");

            await AuthorHelpers.UpdateAuthorFirstName(sut.Id, "Brandon");

            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.FirstName.Should().Be("Brandon");
            sut.Id.Should().Be(authorId);
        }

        [Fact]
        public async Task Update_Author_LastName()
        {
            var author = await AuthorHelpers.CreateValidAuthor();

            var repository = new AuthorRepository(_fixture.Context);
            (await repository.ExistsAsync(author.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(author.Id);

            var authorId = sut.Id;

            sut.Should().NotBeNull();
            sut.LastName.Should().Be("Rothfuss");

            await AuthorHelpers.UpdateAuthorLastName(sut.Id, "Sanderson");

            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.LastName.Should().Be("Sanderson");
            sut.Id.Should().Be(authorId);
        }

        [Fact]
        public async Task Update_Author_DateOfBirth()
        {
            var author = await AuthorHelpers.CreateValidAuthor();

            var repository = new AuthorRepository(_fixture.Context);
            (await repository.ExistsAsync(author.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(author.Id);

            var authorId = sut.Id;

            sut.Should().NotBeNull();
            sut.DateOfBirth.Should().HaveYear(1973);
            sut.DateOfBirth.Should().HaveMonth(6);
            sut.DateOfBirth.Should().HaveDay(6);

            await AuthorHelpers.UpdateAuthorDateOfBirth(sut.Id, new DateTime(1975, 12, 19));

            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.DateOfBirth.Should().HaveYear(1975);
            sut.DateOfBirth.Should().HaveMonth(12);
            sut.DateOfBirth.Should().HaveDay(19);
            sut.Id.Should().Be(authorId);
        }

        [Fact]
        public async Task Update_Author_MugshotPath()
        {
            var author = await AuthorHelpers.CreateValidAuthor();

            var repository = new AuthorRepository(_fixture.Context);
            (await repository.ExistsAsync(author.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(author.Id);

            var authorId = sut.Id;

            sut.Should().NotBeNull();
            sut.MugshotPath.Should().Be(@"\\filepath\file.jpg");

            await AuthorHelpers.UpdateAuthorMugshotPath(sut.Id, @"\\filepath\newFile.jpg");
            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.MugshotPath.Should().Be(@"\\filepath\newFile.jpg");
            sut.Id.Should().Be(authorId);
        }

        [Fact]
        public async Task Update_Author_Biography()
        {
            var author = await AuthorHelpers.CreateValidAuthor();

            var repository = new AuthorRepository(_fixture.Context);
            (await repository.ExistsAsync(author.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(author.Id);

            var authorId = sut.Id;

            sut.Should().NotBeNull();
            sut.Biography.Should().Contain("There is no book number three.");

            await AuthorHelpers.UpdateAuthorBiography(sut.Id, "Bacon ipsum...");

            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.Biography.Should().Contain("Bacon ipsum");
            sut.Id.Should().Be(authorId);
        }

        [Fact]
        public async Task Update_Author_Notes()
        {
            var author = await AuthorHelpers.CreateValidAuthor();

            var repository = new AuthorRepository(_fixture.Context);
            (await repository.ExistsAsync(author.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(author.Id);

            var authorId = sut.Id;

            sut.Should().NotBeNull();
            sut.Notes.Should().Be("...");

            await AuthorHelpers.UpdateAuthorNotes(sut.Id, "Could I please have book number three?");

            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.Notes.Should().Contain("Could I please");
            sut.Id.Should().Be(authorId);
        }

        [Fact]
        public async Task Update_Author_Nationality()
        {
            var author = await AuthorHelpers.CreateValidAuthor();

            var repository = new AuthorRepository(_fixture.Context);
            (await repository.ExistsAsync(author.Id)).Should().BeTrue();

            var sut = await repository.LoadAsync(author.Id);

            var authorId = sut.Id;

            sut.Should().NotBeNull();
            sut.Nationality.Should().BeNull();

            var nationality = await NationalityHelpers.CreateValidNationality();
            await AuthorHelpers.UpdateAuthorNationality(sut.Id, nationality.Id);

            sut = await repository.LoadAsync(author.Id);
            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.Nationality.Id.Should().Be(nationality.Id);
            sut.Nationality.Name.Should().Be(nationality.Name);
            sut.Id.Should().Be(authorId);
        }

        [Fact]
        public async Task Remove_Author()
        {
            var author = await AuthorHelpers.CreateValidAuthor();

            var repository = new AuthorRepository(_fixture.Context); 
            (await repository.ExistsAsync(author.Id)).Should().BeTrue();
            
            await AuthorHelpers.RemoveAuthor(author.Id);

            var sut = await repository.GetAsync(author.Id);
            await _fixture.Context.Entry(sut).ReloadAsync();
            (await repository.ExistsAsync(author.Id)).Should().BeFalse();
        }
    }
}