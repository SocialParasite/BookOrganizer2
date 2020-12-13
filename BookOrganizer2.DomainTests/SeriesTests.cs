using System;
using System.Collections.Generic;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.SeriesProfile;
using BookOrganizer2.Domain.Exceptions;
using BookOrganizer2.Domain.Shared;
using FluentAssertions;
using Xunit;

namespace BookOrganizer2.DomainTests
{
    [Trait("Unit", "Unit")]
    public class SeriesTests
    {
        private Series CreateSeries()
            => Series.Create(new SeriesId(SequentialGuid.NewSequentialGuid()), "Name", "Less");

        [Theory]
        [InlineData("A")]
        [InlineData("Mistborn")]
        [InlineData("Raven's Shadow")]
        [InlineData("Songs of Ice and Fire")]
        [InlineData("Y: The Last Man")]
        [InlineData("Åke")]
        [InlineData("JamesJohnHarryMichaelWilliamDavidRichardJosephThomasJoeMarkDerekJamesJohnHarryMichaelWilliamDavidRichardJosephThomasJoeMarkDerekJamesJohnHarryMichaelWilliamDavidRichardJosephThomasJoeMarkDerekJamesJohnHarryMichaelWilliamDavidRichardJosephThomasJoeMarkDerek")]
        public void Valid_name(string name)
        {
            var sut = CreateSeries();
            sut.SetName(name);
            sut.Name.Should().Be(name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("JamesJohnHarryMichaelWilliamDavidRichardJosephThomasJoeMarkDerekJamesJohnHarryMichaelWilliamDavidRichardJosephThomasJoeMarkDerekJamesJohnHarryMichaelWilliamDavidRichardJosephThomasJoeMarkDerekJamesJohnHarryMichaelWilliamDavidRichardJosephThomasJoeMarkDereka")]
        public void InValid_name(string name)
        {
            var sut = CreateSeries();
            Action action = () => sut.SetName(name);

            action.Should().Throw<InvalidNameException>();
        }

        [Theory]
        [InlineData("")]
        public void Valid_Description(string description)
        {
            var sut = CreateSeries();
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
        public void Valid_picture_path(string file)
        {
            var sut = CreateSeries();
            var pic = @$"C:\temp\testingsutPicsPath\{file}";
            sut.SetPicturePath(pic);

            sut.PicturePath.Should().Be(pic);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(@"C:\secret.doc%00.exe")]
        [InlineData(@"C:\fake.bmp")]
        [InlineData(@"C:\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long123\real.jpg")]
        public void Invalid_picture_path(string path)
        {
            var sut = CreateSeries();
            var pic = path;
            Action action = () => sut.SetPicturePath(pic);

            action.Should().Throw<Exception>();
        }

        [Fact]
        public void Valid_ReadOrders()
        {
            var sut = CreateSeries();
            sut.Books.Count.Should().Be(0);

            var newBooks = new List<ReadOrder>
            {
                ReadOrder.NewReadOrder(Book.NewBook, Series.NewSeries, 3),
                ReadOrder.NewReadOrder(Book.NewBook, Series.NewSeries, 1)
            };

            sut.SetBooks(newBooks);

            sut.Books.Should().NotBeNull();
            sut.Books.Count.Should().Be(2);
        }

        //[Fact]
        //public void Series_in_order()
        //{
        //    var sut = CreateSeries();

        //    var newBooks = new List<ReadOrder>
        //    {
        //        ReadOrder.NewReadOrder(Book.NewBook, Series.NewSeries, 3),
        //        ReadOrder.NewReadOrder(Book.NewBook, Series.NewSeries, 1)
        //    };

        //    sut.SetBooks(newBooks);

        //    sut.Books.Should().NotBeNull();
        //    sut.Books.Count.Should().Be(2);
        //    sut.Books.First().Instalment.Should().Be(1);
        //}
    }
}
