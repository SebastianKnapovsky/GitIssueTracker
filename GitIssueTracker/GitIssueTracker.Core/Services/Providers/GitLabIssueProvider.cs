using GitIssueTracker.Core.Enums;
using GitIssueTracker.Core.Models;
using GitIssueTracker.Core.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace GitIssueTracker.Core.Services.Providers
{
    public class GitLabIssueProvider : BaseIssueProvider, IIssueProvider
    {
        public IssuePlatform Platform => IssuePlatform.GitLab;
        private readonly ILogger<GitLabIssueProvider> _logger;

        public GitLabIssueProvider(HttpClient client, IConfiguration config, ILogger<GitLabIssueProvider> logger)
            : base(client, logger)
        {
            _logger = logger;
            var token = config["GitServices:GitLab:Token"];
            client.BaseAddress = new Uri("https://gitlab.com/api/v4/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<IssueResponse> CreateAsync(string repository, IssueRequest issue)
        {
            try
            {
                _logger.LogInformation("Creating GitLab issue in project {Repository}", repository);

                var json = await SendRequestAsync(HttpMethod.Post, $"projects/{repository}/issues", new
                {
                    title = issue.Title,
                    description = issue.Description
                });

                return new IssueResponse
                {
                    IssueNumber = json.GetProperty("iid").GetInt32(),
                    Url = json.GetProperty("web_url").GetString(),
                    Status = IssueStatus.Open
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create GitLab issue in project {Repository}", repository);
                throw;
            }
        }

        public async Task<IssueResponse> UpdateAsync(string repository, int issueNumber, IssueRequest issue)
        {
            try
            {
                _logger.LogInformation("Updating GitLab issue #{IssueNumber} in project {Repository}", issueNumber, repository);

                var json = await SendRequestAsync(HttpMethod.Put, $"projects/{repository}/issues/{issueNumber}", new
                {
                    title = issue.Title,
                    description = issue.Description
                });

                return new IssueResponse
                {
                    IssueNumber = json.GetProperty("iid").GetInt32(),
                    Url = json.GetProperty("web_url").GetString(),
                    Status = IssueStatus.Open
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update GitLab issue #{IssueNumber} in project {Repository}", issueNumber, repository);
                throw;
            }
        }

        public async Task<bool> CloseAsync(string repository, int issueNumber)
        {
            try
            {
                _logger.LogInformation("Closing GitLab issue #{IssueNumber} in project {Repository}", issueNumber, repository);

                await SendRequestAsync(HttpMethod.Put, $"projects/{repository}/issues/{issueNumber}", new
                {
                    state_event = "close"
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to close GitLab issue #{IssueNumber} in project {Repository}", issueNumber, repository);
                throw;
            }
        }
    }
}
