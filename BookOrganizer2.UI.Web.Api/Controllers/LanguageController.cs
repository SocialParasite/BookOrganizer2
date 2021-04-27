using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.UI.Web.Api.Common;
using Serilog;
using static BookOrganizer2.Domain.BookProfile.LanguageProfile.Events;

namespace BookOrganizer2.UI.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguageController : ControllerBase
    {
        // https://docs.docker.com/compose/aspnet-mssql-compose/

        private readonly ISimpleDomainService<Language, LanguageId> _service;
        private readonly ILanguageLookupDataService _languageLookupDataService;

        public LanguageController(ISimpleDomainService<Language, LanguageId> service, ILanguageLookupDataService languageLookupDataService)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _languageLookupDataService = languageLookupDataService ?? throw new ArgumentNullException(nameof(languageLookupDataService));
        }

        [HttpGet]
        public async Task<IEnumerable<LookupItem>> GetLanguages()
        {
            return await _languageLookupDataService.GetLanguageLookupAsync("");
        }

        [HttpGet("{id}")]
        public async ValueTask<Language> GetLanguage(Guid id)
        {
            return await _service.GetAsync(id);
        }

        [HttpPost]
        public async Task<ActionResult<Created>> Post(Commands.Create request)
        {
            try
            {
                await RequestHandler.HandleCommand(request, _service.Handle);

                // HACK: Future me, do something clever instead...
                if (!string.IsNullOrEmpty(request.Name))
                {
                    if (request.Id != null)
                        return new Created { Id = (Guid)request.Id, Name = request.Name };
                }
                return new BadRequestObjectResult(
                    new
                    {
                        error = "Language was not in valid state! Name should not be empty."
                    }
                );
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Somehow something happened while creating a new language =-O");

                throw;
            }
        }

        [Route("LanguageInfo")]
        [HttpPut]
        public async Task<IActionResult> Put(Commands.Update request)
            => await RequestHandler.HandleCommand(request, _service.Handle);

        [Route("Delete")]
        [HttpPost]
        public async Task<ActionResult<Deleted>> Delete(Commands.Delete request)
        {
            var result = await RequestHandler.HandleCommand(request, _service.Handle);

            if (result.GetType() == typeof(OkResult))
                return new Deleted { Id = request.Id };

            return new BadRequestObjectResult(new { error = "Error occurred during delete attempt." });
        }
    }
}
