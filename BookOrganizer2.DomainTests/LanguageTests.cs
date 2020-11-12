using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using BookOrganizer2.Domain.Shared;
using FluentAssertions;
using System;
using BookOrganizer2.Domain.Exceptions;
using Xunit;

namespace BookOrganizer2.DomainTests
{
    public class LanguageTests
    {
        private Language CreateLanguage()
            => Language.Create(new LanguageId(SequentialGuid.NewSequentialGuid()), "Pig latin");

        [Theory]
        [InlineData("E")]
        [InlineData("EN")]
        [InlineData("latin")]
        [InlineData("JamesJohnHarryMichaelWilliamDavi")]
        public void Valid_name(string name)
        {
            var sut = CreateLanguage();
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
            var sut = CreateLanguage();
            Action action = () => sut.SetName(name);

            action.Should().Throw<InvalidNameException>();
        }
    }
}
