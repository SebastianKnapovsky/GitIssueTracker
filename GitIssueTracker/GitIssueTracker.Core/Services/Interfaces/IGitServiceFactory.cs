using GitIssueTracker.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitIssueTracker.Core.Services.Interfaces
{
    public interface IGitServiceFactory
    {
        IGitService GetService(GitServiceType type);
    }
}
