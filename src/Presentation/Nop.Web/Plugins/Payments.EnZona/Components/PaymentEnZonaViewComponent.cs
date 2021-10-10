using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.PayPalStandard.Components
{
    [ViewComponent(Name = "PaymentEnZona")]
    public class PaymentEnZonaViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Payments.EnZona/Views/PaymentInfo.cshtml");
        }
    }
}
