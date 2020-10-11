using BookOrganizer2.Domain;
using BookOrganizer2.Domain.Exceptions;
using FluentAssertions;
using System;
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
        [InlineData("Åke")]
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
        [InlineData("Åkerman")]
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
        public void Valid_Birthday()
        {
            var author = new Author();
            author.SetDateOfBirth(new DateTime(1950, 12, 24));

            author.DateOfBirth.Should().HaveYear(1950);
            author.DateOfBirth.Should().HaveMonth(12);
            author.DateOfBirth.Should().HaveDay(24);
        }

        [Fact]
        public void Birthday_Can_Be_Null()
        {
            var author = new Author();
            author.SetDateOfBirth(null);

            author.DateOfBirth.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        public void Valid_Biography(string bio)
        {
            var author = new Author();
            author.SetBiography(bio);

            author.Biography.Should().BeOfType<string>();
            author.Biography.Should().BeEmpty();
        }

        [Theory]
        [InlineData("fake.jpg")]
        [InlineData("fake.JPG")]
        [InlineData("fake.jpeg")]
        [InlineData("fake.png")]
        [InlineData("fake.gif")]
        public void Valid_mugshot_path(string file)
        {
            var author = new Author();
            var pic = @$"C:\temp\testingAuthorPicsPath\{file}";
            author.SetMugshotPath(pic);

            author.MugshotPath.Should().Be(pic);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(@"C:\secret.doc%00.exe")]
        [InlineData(@"C:\fake.bmp")]
        public void Invalid_mugshot_path(string path)
        {
            var author = new Author();
            var pic = path;
            Action action = () => author.SetMugshotPath(pic);

            action.Should().Throw<Exception>();
        }
    }
}
