using GitIssueTracker.Core.Enums;
using GitIssueTracker.Core.Models;

namespace GitIssueTracker.Core.Services.Interfaces
{
    public interface IIssueProvider
    {
        IssuePlatform Platform { get; }
        Task<IssueResponse> CreateAsync(string repository, IssueRequest issue);
        Task<IssueResponse> UpdateAsync(string repository, int issueNumber, IssueRequest issue);
        Task<bool> CloseAsync(string repository, int issueNumber);
    }
}
