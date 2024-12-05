using Microsoft.AspNetCore.Components;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class HttpClientInterceptor : DelegatingHandler
{
    private readonly HttpClientInterceptor _interceptor;
    private readonly NavigationManager _navManager;

    public HttpClientInterceptor( NavigationManager navManager )
    {
        _navManager = navManager;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Intercept and log the outgoing request
        Console.WriteLine($"Request: {request.Method} {request.RequestUri}");

        // Proceed with the request
        var response = await base.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            // Optionally, throw an exception on an unsuccessful status code
            var statusCode = response.StatusCode;
            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    _navManager.NavigateTo($"/error/{response.StatusCode}/{request.RequestUri} not found");
                    break;
                case HttpStatusCode.Unauthorized:
                    _navManager.NavigateTo($"/error/{response.StatusCode}/Not Authorized not found");
                    break;
                case HttpStatusCode.BadRequest:
                    _navManager.NavigateTo($"/error/{response.StatusCode}/{content}");
                    break;
                default:
                    _navManager.NavigateTo($"/error/{response.StatusCode}/Unhandled exception");
                    break;
            }
        }

        // Intercept and log the incoming response
        Console.WriteLine($"Response: {(int)response.StatusCode} {response.StatusCode}");

        // Optionally, modify the response
        return response;
    }
}
