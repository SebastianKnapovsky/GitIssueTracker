using GitIssueTracker.Core.Models;
using GitIssueTracker.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GitIssueTracker.Core.Services
{
    public class GitHubService : IGitService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://api.github.com";
        private readonly string _token;
        private readonly ILogger<GitHubService> _logger;

        public GitHubService(HttpClient httpClient, string token,  ILogger<GitHubService> logger)
        {
            _httpClient = httpClient;
            _token = token;
            _logger = logger;

            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("GitIssueTracker", "1.0"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }
        public async Task<IssueResponse> CreateIssueAsync(string repository, IssueRequest issue)
        {
            var url = $"{_baseUrl}/repos/{repository}/issues";

            var payload = new
            {
                title = issue.Title,
                body = issue.Description
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                _logger.LogInformation("Pomyślnie utworzono issue dla repo {Repository}", repository);

                return new IssueResponse
                {
                    IssueNumber = root.GetProperty("number").GetInt32(),
                    Url = root.GetProperty("html_url").GetString(),
                    Status = root.GetProperty("state").GetString()
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Błąd zapytania do GitHub API podczas tworzenia issue dla repo {Repository}", repository);
                throw new ApplicationException("GitHub API error");
            }
        }
        public async Task<IssueResponse> UpdateIssueAsync(string repository, int issueNumber, IssueRequest issue)
        {
            var url = $"{_baseUrl}/repos/{repository}/issues/{issueNumber}";

            var payload = new
            {
                title = issue.Title,
                body = issue.Description
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PatchAsync(url, content);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                _logger.LogInformation("Zaktualizowano issue #{IssueNumber} w repo {Repository}", issueNumber, repository);

                return new IssueResponse
                {
                    IssueNumber = root.GetProperty("number").GetInt32(),
                    Url = root.GetProperty("html_url").GetString(),
                    Status = root.GetProperty("state").GetString()
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Błąd podczas aktualizacji issue #{IssueNumber} w repo {Repository}", issueNumber, repository);
                throw new ApplicationException("Błąd podczas aktualizacji zgłoszenia");
            }
        }
        public async Task<bool> CloseIssueAsync(string repository, int issueNumber)
        {
            var url = $"{_baseUrl}/repos/{repository}/issues/{issueNumber}";

            var payload = new { state = "closed" };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PatchAsync(url, content);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Zamknięto issue #{IssueNumber} w repo {Repository}", issueNumber, repository);

                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Błąd podczas zamykania issue #{IssueNumber} w repo {Repository}", issueNumber, repository);
                return false;
            }
        }
    }
}
