using Microsoft.JSInterop;
using System.Threading.Tasks;

public class ApplicationInsightsService
{
    private readonly IJSRuntime _jsRuntime;

    public ApplicationInsightsService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task TrackEvent(string eventName, object properties = null)
    {
        await _jsRuntime.InvokeVoidAsync("appInsights.trackEvent", new { name = eventName, properties });
    }

    public async Task TrackException(string message)
    {
        await _jsRuntime.InvokeVoidAsync("appInsights.trackException", new { exception = new { message } });
    }

    public async Task TrackPageView(string pageName)
    {
        await _jsRuntime.InvokeVoidAsync("appInsights.trackPageView", new { name = pageName });
    }
}
