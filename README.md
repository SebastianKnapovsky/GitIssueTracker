# 🐙 GitIssueTracker

GitIssueTracker (G.I.T.)

**GitIssueTracker** is a REST API that allows managing issues in GitHub and GitLab repositories

---

## 🔧 Technologies

- ASP.NET Core 8 (API)
- C#
- GitHub API / GitLab API
- Swagger
- Moq / xUnit (testy jednostkowe)

## 🧠 Architecture

The project is split into 3 logical layers:

GitIssueTracker.Api       - Entry point – Web API, controllers, config
GitIssueTracker.Core      - Business logic, models, interfaces 
GitIssueTracker.Tests     - Unit tests using xUnit and Moq  

**Design Patterns:**

- Strategy / Plugin (`IIssueProvider`)
- Template Method (`BaseIssueProvider`)
- Dependency Injection (built-in DI container)
- Single Responsibility Principle (SRP)
- Open/Closed Principle (easy extensibility)

## 📍 API Endpoints

| Method | URL                                                 | Description              |
|--------|------------------------------------------------------|--------------------------|
| POST   | `/api/issues/{platform}/{repository}`               | Create issue             |
| PUT    | `/api/issues/{platform}/{repository}/{issueNumber}` | Update issue             |
| DELETE | `/api/issues/{platform}/{repository}/{issueNumber}` | Close issue              |


## ➕ Adding a New Platform (e.g., Bitbucket)

1. Create a new class: `BitbucketIssueProvider : BaseIssueProvider, IIssueProvider`
2. Implement `CreateAsync`, `UpdateAsync`, `CloseAsync`
3. Register it in DI (`Program.cs`)
4. Add to `IssuePlatform` enum
5. Add token in `appsettings.Development.json`

## 🔐 API Token Configuration

`appsettings.Development.json`:

"GitServices": {
  "GitHub": {
    "Token": "GITHUB_TOKEN"
  },
  "GitLab": {
    "Token": "GITLAB_TOKEN"
  }
}


## 📦 Required NuGet Packages

The project depends on the following NuGet packages:

- Microsoft.Extensions.Logging.Abstractions — logging abstraction for injecting ILogger
- Microsoft.AspNetCore.Mvc.Core — core framework for Web API controllers
- System.Text.Json — built-in JSON serialization/deserialization
- Moq — mocking framework for unit tests
- xunit, xunit.runner.visualstudio — unit testing framework
- Microsoft.NET.Test.Sdk — enables test execution

After cloning the repository, restore all packages:

dotnet restore


## 🧪 Tests

The project includes unit tests focused on core logic:
- IssueService — verifies correct delegation to platform-specific providers


