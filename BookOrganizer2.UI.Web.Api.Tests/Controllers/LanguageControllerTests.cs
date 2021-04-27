using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.Domain.Shared;
using BookOrganizer2.UI.Web.Api.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookOrganizer2.UI.Web.Api.Tests.Controllers
{
    [Trait("Unit tests", "WebApi/LanguageController")]
    public sealed class LanguageControllerTests
    {
        private static List<LookupItem> MockLanguages()
        {
            var mockLanguages = new List<LookupItem>();

            for (var i = 1; i <= 10; i++)
            {
                mockLanguages.Add(new LookupItem
                {
                    Id = SequentialGuid.NewSequentialGuid(),
                    DisplayMember = $"Language-{i}",
                    Picture = "",
                    InfoText = "",
                    ViewModelName = ""
                });
            }

            return mockLanguages;
        }

        [Fact]
        public async Task Get_All_Languages()
        {
            var mockLanguages = MockLanguages();
            var mockService = new Mock<ISimpleDomainService<Language, LanguageId>>();
            var lookupServiceMock = new Mock<ILanguageLookupDataService>();

            lookupServiceMock.Setup(srv => srv.GetLanguageLookupAsync(""))
                .Returns(() => Task.FromResult(mockLanguages.AsEnumerable()));

            var sut = new LanguageController(mockService.Object, lookupServiceMock.Object);

            var result = await sut.GetLanguages();

            result.Count().Should().Be(10);
            lookupServiceMock.Verify(mock => mock.GetLanguageLookupAsync(""), Times.Once());
        }

        [Fact]
        public async Task Valid_Id_Returns_Language()
        {
            var id = Guid.NewGuid();
            var language = Language.Create(id, "test");
            var mockService = new Mock<ISimpleDomainService<Language, LanguageId>>();
            var lookupServiceMock = new Mock<ILanguageLookupDataService>();

            mockService.Setup(s => s.GetAsync(id))
                .Returns(() => ValueTask.FromResult(language));

            var languageController = new LanguageController(mockService.Object, lookupServiceMock.Object);
            var result = await languageController.GetLanguage(id);

            mockService.Verify(mock => mock.GetAsync(id), Times.Once());

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Invalid_Id_Returns_Null()
        {
            var id = Guid.NewGuid();
            var language = Language.Create(id, "test");

            var mockService = new Mock<ISimpleDomainService<Language, LanguageId>>();
            var lookupServiceMock = new Mock<ILanguageLookupDataService>();

            mockService.Setup(s => s.GetAsync(Guid.NewGuid()))
                .Returns(() => ValueTask.FromResult(language));

            var languageController = new LanguageController(mockService.Object, lookupServiceMock.Object);

            var result = await languageController.GetLanguage(id);

            result.Should().BeNull();
        }

        [Fact]
        public async Task Create_New_Language()
        {
            var mockService = new Mock<ISimpleDomainService<Language, LanguageId>>();
            var lookupServiceMock = new Mock<ILanguageLookupDataService>();

            var cmd = new Commands.Create {Id = Guid.NewGuid(), Name = "Gibberish"};

            var created = new Events.Created { Id = (Guid)cmd.Id, Name = cmd.Name };

            mockService.Setup(s => s.Handle(cmd))
                .Returns(() => Task.FromResult(created));
            var languageController = new LanguageController(mockService.Object, lookupServiceMock.Object);

            var result = await languageController.Post(cmd);

            mockService.Verify(x => x.Handle(cmd), Times.Once());
            result.Value.Id.Should().Be(cmd.Id.ToString());
            result.Value.Name.Should().Be(cmd.Name);
        }

        [Fact]
        public async Task New_Language_Fails()
        {
            var mockService = new Mock<ISimpleDomainService<Language, LanguageId>>();
            var lookupServiceMock = new Mock<ILanguageLookupDataService>();

            var cmd = new Commands.Create { Id = null };

            mockService.Setup(s => s.Handle(cmd))
                .Returns(() => Task.FromException(new Exception()));
            var languageController = new LanguageController(mockService.Object, lookupServiceMock.Object);

            Func<Task> action = async () => await languageController.Post(cmd);
            action.Should().Throw<Exception>();

            mockService.Verify(x => x.Handle(cmd), Times.Once());
        }

        [Fact]
        public async Task Update_language()
        {
            var mockService = new Mock<ISimpleDomainService<Language, LanguageId>>();
            var lookupServiceMock = new Mock<ILanguageLookupDataService>();

            var cmd = new Commands.Update { Id = Guid.NewGuid(), Name = "Gibberish" };

            var languageController = new LanguageController(mockService.Object, lookupServiceMock.Object);

            var result = await languageController.Put(cmd);

            mockService.Verify(x => x.Handle(cmd), Times.Once());
            ((OkResult)result).StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Update_Language_Fails()
        {
            var mockService = new Mock<ISimpleDomainService<Language, LanguageId>>();
            var lookupServiceMock = new Mock<ILanguageLookupDataService>();

            var cmd = new Commands.Update { Id = Guid.NewGuid(), Name = "Gibberish" };

            mockService.Setup(s => s.Handle(cmd))
                .Returns(() => Task.FromException(new Exception()));
            var languageController = new LanguageController(mockService.Object, lookupServiceMock.Object);

            Func<Task> action = async () => await languageController.Put(cmd);
            action.Should().Throw<Exception>();

            mockService.Verify(x => x.Handle(cmd), Times.Once());
        }

        [Fact]
        public async Task Delete_language()
        {
            var mockService = new Mock<ISimpleDomainService<Language, LanguageId>>();
            var lookupServiceMock = new Mock<ILanguageLookupDataService>();

            var cmd = new Commands.Delete {Id = Guid.NewGuid() };

            var created = new Events.Deleted { Id = cmd.Id };

            mockService.Setup(s => s.Handle(cmd))
                .Returns(() => Task.FromResult(created));
            var languageController = new LanguageController(mockService.Object, lookupServiceMock.Object);

            var result = await languageController.Delete(cmd);
                
            mockService.Verify(x => x.Handle(cmd), Times.Once());
            result.Value.Id.Should().Be(cmd.Id);
        }
    }
}
