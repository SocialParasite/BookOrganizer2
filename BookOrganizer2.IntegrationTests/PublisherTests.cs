using BookOrganizer2.DA.Repositories;
using BookOrganizer2.Domain.PublisherProfile;
using BookOrganizer2.IntegrationTests.Helpers;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BookOrganizer2.IntegrationTests
{
    public sealed partial class DatabaseTests
    {
        [Trait("Integration", "DB\\Publisher")]
        [Fact]
        public async Task Publisher_inserted_to_database()
        {
            var publisher = await PublisherHelpers.CreateValidPublisher();
            var repository = new PublisherRepository(_fixture.Context);

            (await repository.ExistsAsync(publisher.Id)).Should().BeTrue();
        }

        [Trait("Integration", "DB\\Publisher")]
        [Fact]
        public void Invalid_Publisher()
        {
            Func<Task> action = async () => { await PublisherHelpers.CreateInvalidPublisher(); };
            action.Should().ThrowAsync<ArgumentException>();
        }

        [Trait("Integration", "DB\\Publisher")]
        [Fact]
        public async Task Update_Publisher()
        {
            var publisher = await PublisherHelpers.CreateValidPublisher();
            publisher.Name.Should().Be("SubPop");

            var sut = Publisher.Create(publisher.Id,
                "NewPop",
                @"\\pics\scott.jpg",
                "...");

            await PublisherHelpers.UpdatePublisher(sut);

            await _fixture.Context.Entry(publisher).ReloadAsync();

            publisher.Name.Should().Be("NewPop");
        }

        [Trait("Integration", "DB\\Publisher")]
        [Fact]
        public async Task Update_Publisher_Name()
        {
            var publisher = await PublisherHelpers.CreateValidPublisher();

            var repository = new PublisherRepository(_fixture.Context);
            (await repository.ExistsAsync(publisher.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(publisher.Id);

            var publisherId = sut.Id;

            sut.Should().NotBeNull();
            sut.Name.Should().Be("SubPop");

            await PublisherHelpers.UpdatePublisherName(sut.Id, "NewPop");

            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.Name.Should().Be("NewPop");
            sut.Id.Should().Be(publisherId);
        }

        [Trait("Integration", "DB\\Publisher")]
        [Fact]
        public async Task Update_Publisher_LogoPath()
        {
            var publisher = await PublisherHelpers.CreateValidPublisher();

            var repository = new PublisherRepository(_fixture.Context);
            (await repository.ExistsAsync(publisher.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(publisher.Id);

            var publisherId = sut.Id;

            sut.Should().NotBeNull();
            sut.LogoPath.Should().Be(@"\\filepath\file.jpg");

            await PublisherHelpers.UpdatePublisherLogoPath(sut.Id, @"\\filepath\newFile.jpg");
            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.LogoPath.Should().Be(@"\\filepath\newFile.jpg");
            sut.Id.Should().Be(publisherId);
        }

        [Trait("Integration", "DB\\Publisher")]
        [Fact]
        public async Task Update_Publisher_Description()
        {
            var publisher = await PublisherHelpers.CreateValidPublisher();

            var repository = new PublisherRepository(_fixture.Context);
            (await repository.ExistsAsync(publisher.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(publisher.Id);

            var publisherId = sut.Id;

            sut.Should().NotBeNull();
            sut.Description.Should().Contain("Best books in the world!");

            await PublisherHelpers.UpdatePublisherDescription(sut.Id, "Bacon ipsum...");

            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.Description.Should().Contain("Bacon ipsum");
            sut.Id.Should().Be(publisherId);
        }

        [Trait("Integration", "DB\\Publisher")]
        [Fact]
        public async Task Remove_Publisher()
        {
            var publisher = await PublisherHelpers.CreateValidPublisher();

            var repository = new PublisherRepository(_fixture.Context); 
            (await repository.ExistsAsync(publisher.Id)).Should().BeTrue();
            
            await PublisherHelpers.RemovePublisher(publisher.Id);

            var sut = await repository.GetAsync(publisher.Id);
            await _fixture.Context.Entry(sut).ReloadAsync();
            (await repository.ExistsAsync(publisher.Id)).Should().BeFalse();
        }
    }
}