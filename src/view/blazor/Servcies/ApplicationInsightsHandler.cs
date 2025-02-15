
using System.Diagnostics;

namespace LendingView.Servcies
{
    public class ApplicationInsightsHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var activity = new Activity("HttpRequest");
            activity.Start();

            var response = await base.SendAsync(request, cancellationToken);

            activity.Stop();
            return response;
        }
    }
}
