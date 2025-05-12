using GitIssueTracker.Core.Services;
using GitIssueTracker.Core.Services.Interfaces;
using GitIssueTracker.Core.Services.Providers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GitIssueTracker API", Version = "v1" });
    c.MapType<GitIssueTracker.Core.Enums.IssuePlatform>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetNames(typeof(GitIssueTracker.Core.Enums.IssuePlatform))
                   .Select(name => new OpenApiString(name))
                   .Cast<IOpenApiAny>()
                   .ToList()
    });
});

builder.Services.AddHttpClient<GitHubIssueProvider>();
builder.Services.AddHttpClient<GitLabIssueProvider>();

builder.Services.AddSingleton<IIssueProvider, GitHubIssueProvider>();
builder.Services.AddSingleton<IIssueProvider, GitLabIssueProvider>();

builder.Services.AddSingleton<IIssueService, IssueService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
