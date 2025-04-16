using System.Net;
using System.Net.Http.Json;
using DosProtection.ProofOfWork;

namespace TestEnvironment.Client.Services;

public class PowHttpClient(HttpClient httpClient)
{
    public Task<HttpResponseMessage> GetWithPowAsync(string requestUri)
    {
        return SendWithPowAsync(HttpMethod.Get, requestUri);
    }

    public Task<HttpResponseMessage> PostWithPowAsync(string requestUri, object? content = null)
    {
        return SendWithPowAsync(HttpMethod.Post, requestUri, content);
    }

    public Task<HttpResponseMessage> PutWithPowAsync(string requestUri, object? content = null)
    {
        return SendWithPowAsync(HttpMethod.Put, requestUri, content);
    }

    public Task<HttpResponseMessage> DeleteWithPowAsync(string requestUri)
    {
        return SendWithPowAsync(HttpMethod.Delete, requestUri);
    }

    private async Task<HttpResponseMessage> SendWithPowAsync(HttpMethod method, string requestUri, object? content = null)
    {
        var initialResponse = await SendRequestAsync(method, requestUri, content);
        if (initialResponse.StatusCode != HttpStatusCode.Unauthorized)
            return initialResponse;

        var statement = await initialResponse.Content.ReadFromJsonAsync<PowChallengeStatement>();
        if (statement?.Challenge == null)
            return initialResponse;

        var solution = PowChallenge.SolveChallenge(statement);
        var retryResponse = await SendRequestWithHeadersAsync(method, requestUri, solution, content);
        return retryResponse;
    }

    private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, string requestUri, object? content)
    {
        var request = new HttpRequestMessage(method, requestUri);
        if (content != null)
            request.Content = JsonContent.Create(content);
        return await httpClient.SendAsync(request);
    }

    private async Task<HttpResponseMessage> SendRequestWithHeadersAsync(
        HttpMethod method,
        string requestUri,
        PowChallengeSolution solution,
        object? content)
    {
        var request = new HttpRequestMessage(method, requestUri);
        request.Headers.Add("PoW-Challenge", solution.Challenge);
        request.Headers.Add("PoW-Nonce", solution.Nonce.ToString());

        if (content != null)
            request.Content = JsonContent.Create(content);

        return await httpClient.SendAsync(request);
    }
}
