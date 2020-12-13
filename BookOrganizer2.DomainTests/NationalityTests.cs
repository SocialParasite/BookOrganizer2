using BookOrganizer2.Domain.AuthorProfile.NationalityProfile;
using BookOrganizer2.Domain.Shared;
using FluentAssertions;
using System;
using BookOrganizer2.Domain.Exceptions;
using Xunit;

namespace BookOrganizer2.DomainTests
{
    [Trait("Unit", "Unit")]
    public class NationalityTests
    {
        private Nationality CreateNationality()
            => Nationality.Create(new NationalityId(SequentialGuid.NewSequentialGuid()), "Nameless");

        [Theory]
        [InlineData("A")]
        [InlineData("UK")]
        [InlineData("british")]
        [InlineData("JamesJohnHarryMichaelWilliamDavi")]
        public void Valid_name(string name)
        {
            var sut = CreateNationality();
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
            var sut = CreateNationality();
            Action action = () => sut.SetName(name);

            action.Should().Throw<InvalidNameException>();
        }
    }
}
