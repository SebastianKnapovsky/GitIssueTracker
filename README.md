# GitIssueTracker
GitIssueTracker (G.I.T.)

# 🐙 GitIssueTracker

**GitIssueTracker** to aplikacja webowa API, pozwalająca zarządzać zgłoszeniami (issues) w repozytoriach(GitHub/GitLab).

## 🔧 Technologie

- ASP.NET Core 8 (API)
- C#
- GitHub API / GitLab API
- Swagger
- Moq / xUnit (testy jednostkowe)

## 🧠 Architektura

Projekt został podzielony na 3 warstwy:

GitIssueTracker.Api       - aplikacja API (kontrolery, konfiguracja, Swagger)
GitIssueTracker.Core      - logika biznesowa, modele, serwisy i interfejsy
GitIssueTracker.Tests     - testy jednostkowe (xUnit + Moq)

Rozdzielenie warstw:
- ✅ Zachowanie zasady **Single Responsibility**
- ✅ Ułatwia testowanie i rozwój (np. dodanie innych systemów zarządzania issue)
- ✅ Umożliwia wykorzystanie serwisów jako biblioteki

## 🔁 Endpointy

GitHub:
- POST /api/issues/github/{repository} – tworzenie zgłoszenia
- PUT /api/issues/github/{repository}/{issueNumber} – aktualizacja zgłoszenia
- DELETE /api/issues/github/{repository}/{issueNumber} – zamknięcie zgłoszenia

GitLab:
- POST /api/issues/gitlab/{repository} – tworzenie zgłoszenia
- PUT /api/issues/gitlab/{repository}/{issueNumber} – aktualizacja zgłoszenia
- DELETE /api/issues/gitlab/{repository}/{issueNumber} – zamknięcie zgłoszenia

## ➕ Dodanie nowego silnika

Aby dodać obsługę kolejnego systemu zarządzania zgłoszeniami (np. Bitbucket), wykonaj poniższe kroki:

1. **Utwórz interfejs**
   - W katalogu `GitIssueTracker.Core/Services/Interfaces` dodaj nowy interfejs `IBitbucketService`
   - Zdefiniuj metody:
     - `Task<IssueResponse> CreateIssueAsync(string repository, IssueRequest issue);`
     - `Task<IssueResponse> UpdateIssueAsync(string repository, int issueNumber, IssueRequest issue);`
     - `Task<bool> CloseIssueAsync(string repository, int issueNumber);`

2. **Zaimplementuj klasę serwisu**
   - W katalogu `GitIssueTracker.Core/Services` utwórz klasę `BitbucketService`
   - Klasa powinna implementować `IBitbucketService`
   - Obsługuje zapytania HTTP do API Bitbucketa (użyj `HttpClient`)

3. **Zarejestruj serwis w DI**
   - W pliku `Program.cs` dodaj:
     ```csharp
     builder.Services.AddHttpClient<IBitbucketService, BitbucketService>();
     ```

4. **Dodaj konfigurację tokena**
   - W pliku `appsettings.Development.json` dodaj:
     ```json
     "GitServices": {
       "Bitbucket": {
         "Token": "TWÓJ_BITBUCKET_TOKEN"
       }
     }
     ```

5. **Dodaj metody do kontrolera**
   - W `IssuesController.cs` dodaj:
     - `CreateIssueBitbucket(...)`
     - `UpdateIssueBitbucket(...)`
     - `CloseIssueBitbucket(...)`

6. **Dodaj testy jednostkowe**
   - Utwórz plik `BitbucketServiceTests.cs` w `GitIssueTracker.Tests`
   - Dodaj testy kontrolera do `IssuesControllerTests.cs`
   
## 📈 Plan rozwoju

- [ ] Dodanie wsparcia dla Bitbucket / Azure DevOps
- [ ] Integracja z front-endem (React lub Blazor)
- [ ] Autoryzacja użytkowników
- [ ] Testy integracyjne
- [ ] CI/CD z GitHub Actions
- [ ] Ujednolicenie logiki w serwisach (np. wyodrębnienie wspólnych metod bazowych)
- [ ] Refaktoryzacja kodu pod wzorce projektowe (np. Template Method / Strategy)

## 📦 Wymagane paczki NuGet

Projekt wymaga następujących bibliotek:

- Microsoft.Extensions.Logging.Abstractions — logger 
- Microsoft.AspNetCore.Mvc.Core — kontrolery Web API
- System.Text.Json — serializacja i deserializacja JSON
- Moq — mockowanie w testach
- xunit, xunit.runner.visualstudio — framework testowy
- Microsoft.NET.Test.Sdk — uruchamianie testów

Po sklonowaniu repo, zainstaluj zależności za pomocą:

dotnet restore

## 🔐 Ustawienia API tokenów

Do działania wymagane są tokeny do GitHub i GitLab. Należy je umieścić w pliku `appsettings.Development.json`:

"GitServices": {
  "GitHub": {
    "Token": "TWÓJ_GITHUB_TOKEN"
  },
  "GitLab": {
    "Token": "TWÓJ_GITLAB_TOKEN"
  }
}

## 🧪 Testy

Projekt posiada komplet testów jednostkowych dla:
- GitHubService, GitLabService — testy z mockowaniem HttpClient
- IssuesController — testy z mockowaniem serwisów i loggera


