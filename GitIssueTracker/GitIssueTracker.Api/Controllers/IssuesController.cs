using GitIssueTracker.Core.Enums;
using GitIssueTracker.Core.Models;
using GitIssueTracker.Core.Services;
using GitIssueTracker.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GitIssueTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IssuesController : Controller
    {
        private readonly IGitServiceFactory _gitServiceFactory;
        private readonly ILogger<IssuesController> _logger;
        public IssuesController(IGitServiceFactory gitServiceFactory, ILogger<IssuesController> logger)
        {
            _gitServiceFactory = gitServiceFactory;
            _logger = logger;
        }

        [HttpPost("repository")]
        public async Task<IActionResult> CreateIssue(string repository, [FromBody] IssueRequest request, [FromQuery] GitServiceType type = GitServiceType.GitHub)
        {
            _logger.LogInformation("Tworzenie zgłoszenia w repozytorium {Repository}", repository);

            try
            {
                var service = _gitServiceFactory.GetService(type);
                var response = await service.CreateIssueAsync(repository, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas tworzenia zgłoszenia w repozytorium {Repository}", repository);
                return StatusCode(500, "Wystąpił błąd podczas tworzenia zgłoszenia.");
            }
        }
        [HttpPut("{repository}/{issueNumber}")]
        public async Task<IActionResult> UpdateIssue(string repository, int issueNumber, [FromBody] IssueRequest request, [FromQuery] GitServiceType type = GitServiceType.GitHub)
        {
            _logger.LogInformation("Aktualizacja zgłoszenia #{IssueNumber} w repo {Repository}", issueNumber, repository);

            try
            {
                var service = _gitServiceFactory.GetService(type);
                var response = await service.UpdateIssueAsync(repository, issueNumber, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas aktualizacji zgłoszenia #{IssueNumber} w repo {Repository}", issueNumber, repository);
                return StatusCode(500, "Wystąpił błąd podczas aktualizacji zgłoszenia.");
            }
        }
        [HttpDelete("{repository}/{issueNumber}")]
        public async Task<IActionResult> CloseIssue(string repository, int issueNumber, [FromQuery] GitServiceType type = GitServiceType.GitHub)
        {
            _logger.LogInformation("Zamykanie zgłoszenia #{IssueNumber} w repo {Repository}", issueNumber, repository);

            try
            {
                var service = _gitServiceFactory.GetService(type);
                var result = await service.CloseIssueAsync(repository, issueNumber);
                if (result)
                {
                    return NoContent();
                }

                _logger.LogWarning("Zamknięcie zgłoszenia #{IssueNumber} nie powiodło się w repo {Repository}", issueNumber, repository);
                return StatusCode(500, "Nie udało się zamknąć zgłoszenia.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas zamykania zgłoszenia #{IssueNumber} w repo {Repository}", issueNumber, repository);
                return StatusCode(500, "Wystąpił błąd podczas zamykania zgłoszenia.");
            }
        }
    }
}
