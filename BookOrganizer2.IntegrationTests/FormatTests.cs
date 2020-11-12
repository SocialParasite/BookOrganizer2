using BookOrganizer2.DA.Repositories;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.IntegrationTests.Helpers;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BookOrganizer2.IntegrationTests
{
    public sealed partial class DatabaseTests
    {
        [Fact]
        public async Task Format_inserted_to_database()
        {
            var format = await FormatHelpers.CreateValidFormat();
            var repository = new FormatRepository(_fixture.Context);

            (await repository.ExistsAsync(format.Id)).Should().BeTrue();
        }

        [Fact]
        public void Invalid_Format()
        {
            Func<Task> action = async () => { await FormatHelpers.CreateInvalidFormat(); };
            action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task Update_Format()
        {
            var format = await FormatHelpers.CreateValidFormat();
            format.Name.Should().Be("paperback");

            var sut = Format.Create(format.Id, "hardcover");

            await FormatHelpers.UpdateFormat(sut);

            await _fixture.Context.Entry(format).ReloadAsync();

            format.Name.Should().Be("hardcover");
        }

        [Fact]
        public async Task Remove_Format()
        {
            var format = await FormatHelpers.CreateValidFormat();

            var repository = new FormatRepository(_fixture.Context);
            (await repository.ExistsAsync(format.Id)).Should().BeTrue();

            await FormatHelpers.RemoveFormat(format.Id);

            var sut = await repository.GetAsync(format.Id);
            await _fixture.Context.Entry(sut).ReloadAsync();
            (await repository.ExistsAsync(format.Id)).Should().BeFalse();
        }
    }
}