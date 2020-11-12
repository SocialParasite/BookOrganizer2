using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.Shared;
using FluentAssertions;
using System;
using BookOrganizer2.Domain.Exceptions;
using Xunit;

namespace BookOrganizer2.DomainTests
{
    public class FormatTests
    {
        private Format CreateFormat()
            => Format.Create(new FormatId(SequentialGuid.NewSequentialGuid()), "Format");

        [Theory]
        [InlineData("A")]
        [InlineData("OK")]
        [InlineData("paperback")]
        [InlineData("JamesJohnHarryMichaelWilliamDavi")]
        public void Valid_name(string name)
        {
            var sut = CreateFormat();
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
            var sut = CreateFormat();
            Action action = () => sut.SetName(name);

            action.Should().Throw<InvalidNameException>();
        }
    }
}
