using GitIssueTracker.Core.Enums;
using GitIssueTracker.Core.Models;

namespace GitIssueTracker.Core.Services.Interfaces
{
    public interface IIssueService
    {
        Task<IssueResponse> CreateIssueAsync(IssuePlatform platform, string repository, IssueRequest issue);
        Task<IssueResponse> UpdateIssueAsync(IssuePlatform platform, string repository, int issueNumber, IssueRequest issue);
        Task<bool> CloseIssueAsync(IssuePlatform platform, string repository, int issueNumber);
    }
}
