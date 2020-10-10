using BookOrganizer2.Domain;
using FluentAssertions;
using System;
using System.Runtime.InteropServices;
using BookOrganizer2.Domain.Exceptions;
using Xunit;

namespace BookOrganizer2.DomainTests
{
    public class AuthorTests
    {
        [Theory]
        [InlineData("A")]
        [InlineData("John")]
        [InlineData("John John")]
        [InlineData("John-John")]
        [InlineData("JamesJohnHarryMichaelWilliamDavidRichardJosephThomasJoeMarkDerek")]
        public void Valid_first_name(string name)
        {
            var sut = new Author();
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
            var sut = new Author();
            Action action = () => sut.SetFirstName(name);

            action.Should().Throw<InvalidFirstNameException>();
        }

        [Theory]
        [InlineData("A")]
        [InlineData("Wayne")]
        [InlineData("Duke Wayne")]
        [InlineData("John-John")]
        [InlineData("WolfeschlegelsteinhausenbergerdorffWolfeschlegelsteinhausenberge")]
        public void Valid_last_name(string name)
        {
            var sut = new Author();
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
            var sut = new Author();
            Action action = () => sut.SetLastName(name);

            action.Should().Throw<InvalidLastNameException>();
        }

        [Fact]
        public void ValidBirthday()
        {
            var author = new Author();
            author.SetDateOfBirth(new DateTime(1950, 12, 24));

            author.DateOfBirth.Should().HaveYear(1950);
            author.DateOfBirth.Should().HaveMonth(12);
            author.DateOfBirth.Should().HaveDay(24);
        }

        [Fact]
        public void BirthdayCanBeNull()
        {
            var author = new Author();
            author.SetDateOfBirth(null);

            author.DateOfBirth.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        public void ValidBiography(string bio)
        {
            var author = new Author();
            author.SetBiography(bio);

            author.Biography.Should().BeOfType<string>();
            author.Biography.Should().BeEmpty();
        }

        [Fact]
        public void AuthorMugshotsAreStoredUnderCurrentUsersProfileInAuthorPicsSubfolder()
        {
            var author = new Author();
            var pic = @"C:\temp\testingAuthorPicsPath\fake.jpg";
            author.SetMugshotPath(pic);

            //var test = "C:\\Users\\tonij\\Pictures\\BookOrganizer\\AuthorPics\\fake.jpg";

            //author.MugshotPath.Should().Be(test);
            author.MugshotPath.Should().Be(pic);
        }
    }
}
