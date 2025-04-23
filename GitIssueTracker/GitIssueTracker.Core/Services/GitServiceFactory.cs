using GitIssueTracker.Core.Enums;
using GitIssueTracker.Core.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitIssueTracker.Core.Services
{
    public class GitServiceFactory : IGitServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public GitServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IGitService GetService(GitServiceType type)
        {
            return type switch
            {
                GitServiceType.GitHub => _serviceProvider.GetRequiredService<GitHubService>(),
                GitServiceType.GitLab => _serviceProvider.GetRequiredService<GitLabService>(),
                _ => throw new NotSupportedException($"Serwis '{type}' nie jest wspierany.")
            };
        }
    }
}
