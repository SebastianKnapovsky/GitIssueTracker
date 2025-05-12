using GitIssueTracker.Core.Enums;
using GitIssueTracker.Core.Models;
using GitIssueTracker.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GitIssueTracker.Api.Controllers
{
    [ApiController]
    [Route("api/issues")]
    public class IssuesController : ControllerBase
    {
        private readonly IIssueService _issueService;
        private readonly ILogger<IssuesController> _logger;

        public IssuesController(IIssueService issueService, ILogger<IssuesController> logger)
        {
            _issueService = issueService;
            _logger = logger;
        }

        [HttpPost("{platform}/{repository}")]
        public async Task<IActionResult> CreateIssue(IssuePlatform platform, string repository, [FromBody] IssueRequest request)
        {
            try
            {
                var result = await _issueService.CreateIssueAsync(platform, repository, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating issue for platform {Platform}", platform);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{platform}/{repository}/{issueNumber}")]
        public async Task<IActionResult> UpdateIssue(IssuePlatform platform, string repository, int issueNumber, [FromBody] IssueRequest request)
        {
            try
            {
                var result = await _issueService.UpdateIssueAsync(platform, repository, issueNumber, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating issue for platform {Platform}", platform);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{platform}/{repository}/{issueNumber}")]
        public async Task<IActionResult> CloseIssue(IssuePlatform platform, string repository, int issueNumber)
        {
            try
            {
                var result = await _issueService.CloseIssueAsync(platform, repository, issueNumber);
                return result ? NoContent() : StatusCode(500, "Could not close issue");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing issue for platform {Platform}", platform);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
