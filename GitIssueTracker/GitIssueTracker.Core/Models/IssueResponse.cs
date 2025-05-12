using GitIssueTracker.Core.Enums;

namespace GitIssueTracker.Core.Models
{
    public class IssueResponse
    {
        /// <summary>
        /// Numer zgłoszenia w systemie zewnętrznym.
        /// </summary>
        public int IssueNumber { get; set; }
        public string? Url { get; set; }
        public IssueStatus Status { get; set; }
    }
}
