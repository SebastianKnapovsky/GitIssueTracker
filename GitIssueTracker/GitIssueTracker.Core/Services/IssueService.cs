using GitIssueTracker.Core.Enums;
using GitIssueTracker.Core.Models;
using GitIssueTracker.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace GitIssueTracker.Core.Services
{
    public class IssueService : IIssueService
    {
        private readonly Dictionary<IssuePlatform, IIssueProvider> _providers;
        private readonly ILogger<IssueService> _logger;

        public IssueService(IEnumerable<IIssueProvider> providers, ILogger<IssueService> logger)
        {
            _providers = providers.ToDictionary(p => p.Platform);
            _logger = logger;
        }

        public Task<IssueResponse> CreateIssueAsync(IssuePlatform platform, string repository, IssueRequest issue)
        {
            return GetProvider(platform).CreateAsync(repository, issue);
        }
        
        public Task<IssueResponse> UpdateIssueAsync(IssuePlatform platform, string repository, int issueNumber, IssueRequest issue)
        {
            return GetProvider(platform).UpdateAsync(repository, issueNumber, issue);
        }
         
        public Task<bool> CloseIssueAsync(IssuePlatform platform, string repository, int issueNumber)
        {
            return GetProvider(platform).CloseAsync(repository, issueNumber);
        }
          
        private IIssueProvider GetProvider(IssuePlatform platform)
        {
            if (_providers.TryGetValue(platform, out var provider))
            {
                return provider;
            }

            _logger.LogError("Unsupported platform requested: {Platform}", platform);
            throw new NotSupportedException($"Platform {platform} is not supported.");
        }
    }
}
