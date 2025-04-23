using GitIssueTracker.Core.Models;
using GitIssueTracker.Core.Services.Interfaces;
using Microsoft.Extensions.Configuration;
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
    public class GitLabService 
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://api.github.com";
        private readonly string _token;
        private readonly ILogger<GitLabService> _logger;

        public GitLabService(HttpClient httpClient, IConfiguration config, ILogger<GitLabService> logger)
        {
            _httpClient = httpClient;
            _token = config["GitServices:GitLab:Token"] ?? throw new ArgumentException("Brak tokena GitLab w konfiguracji.");
            _logger = logger;

            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("GitIssueTracker", "1.0"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }
        
    }
}
