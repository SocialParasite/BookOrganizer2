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
        [InlineData("James-John-Harry-Michael-William-David-Richard-Joseph-Thomas-Joe")]
        public void Valid_first_name(string name)
        {
            var sut = new Author();
            sut.SetFirstName(name);
            sut.FirstName.Should().Be(name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("James-John-Harry-Michael-William-David-Richard-Joseph-Thomas-Matt")]
        public void InValid_first_name(string name)
        {
            var sut = new Author();
            Action action = () => sut.SetFirstName(name);

            action.Should().Throw<InvalidFirstNameException>();
        }
    }
}
