using BookOrganizer2.Domain;
using BookOrganizer2.Domain.Exceptions;
using FluentAssertions;
using System;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.Shared;
using Xunit;

namespace BookOrganizer2.DomainTests
{
    public class AuthorTests
    {
        private Author CreateAuthor()
            => Author.Create(new AuthorId(SequentialGuid.NewSequentialGuid()), "Name", "Less");

        [Theory]
        [InlineData("A")]
        [InlineData("John")]
        [InlineData("John John")]
        [InlineData("John-John")]
        [InlineData("Åke")]
        [InlineData("JamesJohnHarryMichaelWilliamDavidRichardJosephThomasJoeMarkDerek")]
        public void Valid_first_name(string name)
        {
            var sut = CreateAuthor();
            sut.SetFirstName(name);
            sut.FirstName.Should().Be(name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("@")]
        [InlineData("123")]
        [InlineData("JamesJohnHarryMichaelWilliamDavidRichardJosephThomasMattMarkDerek")]
        public void InValid_first_name(string name)
        {
            var sut = CreateAuthor();
            Action action = () => sut.SetFirstName(name);

            action.Should().Throw<InvalidFirstNameException>();
        }

        [Theory]
        [InlineData("A")]
        [InlineData("Wayne")]
        [InlineData("Duke Wayne")]
        [InlineData("John-John")]
        [InlineData("Åkerman")]
        [InlineData("WolfeschlegelsteinhausenbergerdorffWolfeschlegelsteinhausenberge")]
        public void Valid_last_name(string name)
        {
            var sut = CreateAuthor();
            sut.SetLastName(name);
            sut.LastName.Should().Be(name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("@")]
        [InlineData("123")]
        [InlineData("WolfeschlegelsteinhausenbergerdorffWolfeschlegelsteinhausenberger")]
        public void InValid_last_name(string name)
        {
            var sut = CreateAuthor();
            Action action = () => sut.SetLastName(name);

            action.Should().Throw<InvalidLastNameException>();
        }

        [Fact]
        public void Valid_Birthday()
        {
            var sut = CreateAuthor();
            sut.SetDateOfBirth(new DateTime(1950, 12, 24));

            sut.DateOfBirth.Should().HaveYear(1950);
            sut.DateOfBirth.Should().HaveMonth(12);
            sut.DateOfBirth.Should().HaveDay(24);
        }

        [Fact]
        public void Birthday_Can_Be_Null()
        {
            var sut = CreateAuthor();
            sut.SetDateOfBirth(null);

            sut.DateOfBirth.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        public void Valid_Biography(string bio)
        {
            var sut = CreateAuthor();
            sut.SetBiography(bio);

            sut.Biography.Should().BeOfType<string>();
            sut.Biography.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        public void Valid_Notes(string notes)
        {
            var sut = CreateAuthor();
            sut.SetNotes(notes);

            sut.Notes.Should().BeOfType<string>();
            sut.Notes.Should().BeEmpty();
        }

        [Theory]
        [InlineData("fake.jpg")]
        [InlineData("fake.JPG")]
        [InlineData("fake.jpeg")]
        [InlineData("fake.png")]
        [InlineData("fake.gif")]
        public void Valid_mugshot_path(string file)
        {
            var sut = CreateAuthor();
            var pic = @$"C:\temp\testingsutPicsPath\{file}";
            sut.SetMugshotPath(pic);

            sut.MugshotPath.Should().Be(pic);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(@"C:\secret.doc%00.exe")]
        [InlineData(@"C:\fake.bmp")]
        [InlineData(@"C:\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long123\real.jpg")]
        public void Invalid_mugshot_path(string path)
        {
            var sut = CreateAuthor();
            var pic = path;
            Action action = () => sut.SetMugshotPath(pic);

            action.Should().Throw<Exception>();
        }
    }
}
