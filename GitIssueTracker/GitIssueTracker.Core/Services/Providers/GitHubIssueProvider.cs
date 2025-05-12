using GitIssueTracker.Core.Enums;
using GitIssueTracker.Core.Models;
using GitIssueTracker.Core.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace GitIssueTracker.Core.Services.Providers
{
    public class GitHubIssueProvider : BaseIssueProvider, IIssueProvider
    {
        public IssuePlatform Platform => IssuePlatform.GitHub;
        private readonly ILogger<GitHubIssueProvider> _logger;

        public GitHubIssueProvider(HttpClient client, IConfiguration config, ILogger<GitHubIssueProvider> logger)
            : base(client, logger)
        {
            _logger = logger;
            var token = config["GitServices:GitHub:Token"];
            client.BaseAddress = new Uri("https://api.github.com");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("GitIssueTracker", "1.0"));
        }

        public async Task<IssueResponse> CreateAsync(string repository, IssueRequest issue)
        {
            try
            {
                _logger.LogInformation("Creating GitHub issue in repository {Repository}", repository);
                repository = Uri.UnescapeDataString(repository);

                var json = await SendRequestAsync(HttpMethod.Post, $"/repos/{repository}/issues", new
                {
                    title = issue.Title,
                    body = issue.Description
                });

                return new IssueResponse
                {
                    IssueNumber = json.GetProperty("number").GetInt32(),
                    Url = json.GetProperty("html_url").GetString(),
                    Status = IssueStatus.Open
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create GitHub issue in repository {Repository}", repository);
                throw;
            }
        }

        public async Task<IssueResponse> UpdateAsync(string repository, int issueNumber, IssueRequest issue)
        {
            try
            {
                _logger.LogInformation("Updating GitHub issue #{IssueNumber} in repository {Repository}", issueNumber, repository);
                repository = Uri.UnescapeDataString(repository);

                var json = await SendRequestAsync(HttpMethod.Patch, $"/repos/{repository}/issues/{issueNumber}", new
                {
                    title = issue.Title,
                    body = issue.Description
                });

                var state = json.GetProperty("state").GetString();

                return new IssueResponse
                {
                    IssueNumber = json.GetProperty("number").GetInt32(),
                    Url = json.GetProperty("html_url").GetString(),
                    Status = state == "closed" ? IssueStatus.Closed : IssueStatus.Open
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update GitHub issue #{IssueNumber} in repository {Repository}", issueNumber, repository);
                throw;
            }
        }

        public async Task<bool> CloseAsync(string repository, int issueNumber)
        {
            try
            {
                _logger.LogInformation("Closing GitHub issue #{IssueNumber} in repository {Repository}", issueNumber, repository);
                repository = Uri.UnescapeDataString(repository);

                await SendRequestAsync(HttpMethod.Patch, $"/repos/{repository}/issues/{issueNumber}", new
                {
                    state = "closed"
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to close GitHub issue #{IssueNumber} in repository {Repository}", issueNumber, repository);
                throw;
            }
        }
    }
}
