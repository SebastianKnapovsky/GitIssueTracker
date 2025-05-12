using GitIssueTracker.Core.Enums;
using GitIssueTracker.Core.Models;
using GitIssueTracker.Core.Services;
using GitIssueTracker.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

public class IssueServiceTests
{
    [Fact]
    public async Task CreateIssueAsync_DelegatesToCorrectProvider()
    {
        // Arrange
        var mockProvider = new Mock<IIssueProvider>();
        var expectedResponse = new IssueResponse { IssueNumber = 1, Status = IssueStatus.Open };

        mockProvider.Setup(p => p.Platform).Returns(IssuePlatform.GitHub);
        mockProvider.Setup(p => p.CreateAsync("repo", It.IsAny<IssueRequest>()))
                    .ReturnsAsync(expectedResponse);

        var service = new IssueService(new[] { mockProvider.Object }, Mock.Of<ILogger<IssueService>>());

        var request = new IssueRequest { Title = "Test", Description = "Desc" };

        // Act
        var result = await service.CreateIssueAsync(IssuePlatform.GitHub, "repo", request);

        // Assert
        Assert.Equal(expectedResponse.IssueNumber, result.IssueNumber);
        mockProvider.Verify(p => p.CreateAsync("repo", request), Times.Once);
    }
    [Fact]
    public async Task UpdateIssueAsync_CallsCorrectProvider()
    {
        // Arrange
        var mockProvider = new Mock<IIssueProvider>();
        mockProvider.Setup(p => p.Platform).Returns(IssuePlatform.GitHub);
        mockProvider.Setup(p => p.UpdateAsync("repo", 42, It.IsAny<IssueRequest>()))
                    .ReturnsAsync(new IssueResponse { IssueNumber = 42 });

        var service = new IssueService(new[] { mockProvider.Object }, Mock.Of<ILogger<IssueService>>());

        var request = new IssueRequest { Title = "a", Description = "b" };

        // Act
        var result = await service.UpdateIssueAsync(IssuePlatform.GitHub, "repo", 42, request);

        // Assert
        Assert.Equal(42, result.IssueNumber);
        mockProvider.Verify(p => p.UpdateAsync("repo", 42, request), Times.Once);
    }
    [Fact]
    public async Task CloseIssueAsync_ReturnsTrue_WhenSuccessful()
    {
        var mockProvider = new Mock<IIssueProvider>();
        mockProvider.Setup(p => p.Platform).Returns(IssuePlatform.GitHub);
        mockProvider.Setup(p => p.CloseAsync("repo", 42)).ReturnsAsync(true);

        var service = new IssueService(new[] { mockProvider.Object }, Mock.Of<ILogger<IssueService>>());

        var result = await service.CloseIssueAsync(IssuePlatform.GitHub, "repo", 42);

        Assert.True(result);
        mockProvider.Verify(p => p.CloseAsync("repo", 42), Times.Once);
    }
    [Fact]
    public async Task CreateIssueAsync_Throws_WhenPlatformNotSupported()
    {
        // Arrange
        var mockProvider = new Mock<IIssueProvider>();
        mockProvider.Setup(p => p.Platform).Returns(IssuePlatform.GitHub);

        var service = new IssueService(new[] { mockProvider.Object }, Mock.Of<ILogger<IssueService>>());

        var request = new IssueRequest { Title = "x", Description = "y" };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotSupportedException>(() =>
            service.CreateIssueAsync(IssuePlatform.GitLab, "repo", request));

        Assert.Contains("GitLab", ex.Message);
    }
}
