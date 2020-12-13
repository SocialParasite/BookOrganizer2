using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.Shared;
using FluentAssertions;
using System;
using BookOrganizer2.Domain.Exceptions;
using Xunit;

namespace BookOrganizer2.DomainTests
{
    [Trait("Unit", "Unit")]
    public class GenreTests
    {
        private Genre CreateGenre()
            => Genre.Create(new GenreId(SequentialGuid.NewSequentialGuid()), "New genre");

        [Theory]
        [InlineData("A")]
        [InlineData("genre")]
        [InlineData("new genre")]
        [InlineData("JamesJohnHarryMichaelWilliamDavi")]
        public void Valid_name(string name)
        {
            var sut = CreateGenre();
            sut.SetName(name);
            sut.Name.Should().Be(name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("@")]
        [InlineData("123")]
        [InlineData("JamesJohnHarryMichaelWilliamDavid")]
        public void InValid_name(string name)
        {
            var sut = CreateGenre();
            Action action = () => sut.SetName(name);

            action.Should().Throw<InvalidNameException>();
        }
    }
}
