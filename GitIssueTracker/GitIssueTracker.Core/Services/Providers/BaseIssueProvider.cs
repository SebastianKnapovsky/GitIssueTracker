using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace GitIssueTracker.Core.Services.Providers
{
    public abstract class BaseIssueProvider
    {
        protected readonly HttpClient Client;
        protected readonly ILogger Logger;

        protected BaseIssueProvider(HttpClient client, ILogger logger)
        {
            Client = client;
            Logger = logger;
        }

        protected async Task<JsonElement> SendRequestAsync(HttpMethod method, string url, object? payload = null)
        {
            var request = new HttpRequestMessage(method, url);
            if (payload != null)
            {
                request.Content = SerializePayload(payload);
            }
            var response = await Client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(content);
            return document.RootElement.Clone();
        }

        private StringContent SerializePayload(object payload)
        {
            return new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        }
    }
}
