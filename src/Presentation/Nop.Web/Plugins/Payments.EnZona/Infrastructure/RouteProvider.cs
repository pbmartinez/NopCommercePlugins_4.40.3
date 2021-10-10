using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.EnZona.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //Complete
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.EnZona.Complete", "Plugins/PaymentEnZona/Complete",
                 new { controller = "PaymentEnzona", action = "Complete" });
            //Cancel
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.EnZona.Cancel", "Plugins/PaymentEnZona/Cancel",
                 new { controller = "PaymentEnzona", action = "Cancel" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => -1;
    }
}
