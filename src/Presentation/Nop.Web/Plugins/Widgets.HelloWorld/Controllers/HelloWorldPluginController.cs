using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Data;
using Nop.Plugin.Widgets.HelloWorld.Domains;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.HelloWorld.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class HelloWorldPluginController: BasePluginController
    {
        private readonly IRepository<Presentacion> _presentacionRepository;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;


        public HelloWorldPluginController(IRepository<Presentacion> presentacionRepository,
            INotificationService notificationService,
            ILocalizationService localizationService)
        {
            _presentacionRepository = presentacionRepository;
            _notificationService = notificationService;
            _localizationService = localizationService;
        }

        [HttpGet]
        public async Task<ActionResult> Configure()
        {
            return await Task.FromResult(View());
        }

        [HttpPost]
        public async Task<ActionResult> Configure(Presentacion presentacion)
        {
            if (!ModelState.IsValid)
            {
                _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Plugin.Widgets.HelloWorld.Messages.Warning"));
                return View(presentacion);
            }

            if (presentacion.Id <= 0)
            {
                await _presentacionRepository.InsertAsync(presentacion);
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugin.Widgets.HelloWorld.Messages.Success"));
            }
            else
            {
                await _presentacionRepository.UpdateAsync(presentacion);
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugin.Widgets.HelloWorld.Messages.Success"));
            }
            return View();
        }

    }
}
