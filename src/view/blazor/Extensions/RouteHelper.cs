using System.Collections.Generic;

namespace LendingView.Extensions
{
    public static class RouteHelper
    {
        private static readonly HashSet<string> ExcludedRoutes = new HashSet<string>
    {
        "LendingView.Pages.Open", "LendingView.Pages.ConfirmBorrower",// Add the full type name of your excluded page
    };

        public static bool IsExcludedRoute(string pageType)
        {
            return ExcludedRoutes.Contains(pageType);
        }
    }

}
