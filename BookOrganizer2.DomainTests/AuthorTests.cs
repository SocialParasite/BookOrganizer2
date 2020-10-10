using BookOrganizer2.Domain;
using FluentAssertions;
using System;
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
    }
}
