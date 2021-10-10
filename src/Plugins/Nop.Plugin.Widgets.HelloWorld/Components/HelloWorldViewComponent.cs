
using Microsoft.AspNetCore.Mvc;
using Nop.Data;
using Nop.Plugin.Widgets.HelloWorld.Domains;
using Nop.Services.Customers;
using Nop.Services.Authentication;
using Nop.Web.Framework.Components;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.HelloWorld.Components
{
    //[ViewComponent(Name = "Custom")]
    public class HelloWorldViewComponent : NopViewComponent
    {
        private readonly IRepository<Presentacion> _presentacionRepository;
        private readonly ICustomerService _customerService;
        private readonly IAuthenticationService _authenticationService;
        public HelloWorldViewComponent(ICustomerService customerService,
            IAuthenticationService authenticationService,
            IRepository<Presentacion> privateRepository)
        {
            _customerService = customerService;
            _authenticationService = authenticationService;
            _presentacionRepository = privateRepository;
        }

        //public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        public async Task<IViewComponentResult> InvokeAsync(int productId)
        {
            var customer = await _authenticationService.GetAuthenticatedCustomerAsync();
            var presentacion = await _presentacionRepository.Table.FirstOrDefaultAsync();

            ViewData["nombre"] = customer == null ? "Desconocido" : await _customerService.GetCustomerFullNameAsync(customer);
            ViewData["mensaje"] = presentacion?.Mensaje ?? "No existe mensaje configurado";
            
            return View("~/Plugins/Widgets.HelloWorld/Views/Presentacion.cshtml");
        }
    }
}
