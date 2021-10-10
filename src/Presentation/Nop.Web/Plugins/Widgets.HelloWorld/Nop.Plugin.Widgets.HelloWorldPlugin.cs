using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.HelloWorld
{
    /// <summary>
    /// Rename this file and change to the correct type
    /// </summary>
    public class CustomPlugin : BasePlugin, IWidgetPlugin
    {
        private const string CONFIGURE_CONTROLLER_NAME = "HelloWorldPlugin";
        private const string CONFIGURE_ACTION_NAME = "Configure";
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;


        public CustomPlugin(ILocalizationService localizationService, IWebHelper webHelper)
        {
            _localizationService = localizationService;
            _webHelper = webHelper;
        }

        public bool HideInWidgetList => false;

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "HelloWorld";
        }
        

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            //return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.HeadHtmlTag });
            return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.HomepageTop });
        }

        public override async Task InstallAsync()
        {
            await _localizationService.AddLocaleResourceAsync(new Dictionary<string, string>()
            {
                ["Plugin.Widgets.HelloWorld.Fields.Mensaje"] = "Mensaje",
                ["Plugin.Widgets.HelloWorld.Fields.Mensaje.Hint"] = "Mensaje que se despliega al usuario",
                ["Plugin.Widgets.HelloWorld.Fields.Mensaje.Required"] = "Campo requerido",

                ["Plugin.Widgets.HelloWorld.Messages.Success"] = "La presentacion ha sido guardada correctamente",
                ["Plugin.Widgets.HelloWorld.Messages.Warning"] = "Existen errores de validación",


            });
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _localizationService.DeleteLocaleResourceAsync("Plugin.Widgets.HelloWorld");
            await base.InstallAsync();
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/{CONFIGURE_CONTROLLER_NAME}/{CONFIGURE_ACTION_NAME}";
        }
    }
}
