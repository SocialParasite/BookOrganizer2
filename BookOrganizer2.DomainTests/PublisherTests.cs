using BookOrganizer2.Domain.PublisherProfile;
using BookOrganizer2.Domain.Exceptions;
using BookOrganizer2.Domain.Shared;
using FluentAssertions;
using System;
using Xunit;

namespace BookOrganizer2.DomainTests
{
    [Trait("Unit", "Unit")]
    public class PublisherTests
    {
        private Publisher CreatePublisher()
            => Publisher.Create(new PublisherId(SequentialGuid.NewSequentialGuid()), "Name", "Less");

        [Theory]
        [InlineData("A")]
        [InlineData("SubPop")]
        [InlineData("Sub Pop")]
        [InlineData("Sub-Pop")]
        [InlineData("Åke")]
        [InlineData("JamesJohnHarryMichaelWilliamDavidRichardJosephThomasJoeMarkDerek")]
        public void Valid_name(string name)
        {
            var sut = CreatePublisher();
            sut.SetName(name);
            sut.Name.Should().Be(name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("@")]
        [InlineData("123")]
        [InlineData("JamesJohnHarryMichaelWilliamDavidRichardJosephThomasMattMarkDerek")]
        public void InValid_name(string name)
        {
            var sut = CreatePublisher();
            Action action = () => sut.SetName(name);

            action.Should().Throw<InvalidFirstNameException>();
        }

        [Theory]
        [InlineData("")]
        public void Valid_Description(string description)
        {
            var sut = CreatePublisher();
            sut.SetDescription(description);

            sut.Description.Should().BeOfType<string>();
            sut.Description.Should().BeEmpty();
        }

        [Theory]
        [InlineData("fake.jpg")]
        [InlineData("fake.JPG")]
        [InlineData("fake.jpeg")]
        [InlineData("fake.png")]
        [InlineData("fake.gif")]
        public void Valid_logo_path(string file)
        {
            var sut = CreatePublisher();
            var pic = @$"C:\temp\testingsutPicsPath\{file}";
            sut.SetLogoPath(pic);

            sut.LogoPath.Should().Be(pic);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(@"C:\secret.doc%00.exe")]
        [InlineData(@"C:\fake.bmp")]
        [InlineData(@"C:\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long123\real.jpg")]
        public void Invalid_logo_path(string path)
        {
            var sut = CreatePublisher();
            var pic = path;
            Action action = () => sut.SetLogoPath(pic);

            action.Should().Throw<Exception>();
        }
    }
}
