using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Nop.Plugin.Widgets.HelloWorld.Domains;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Widgets.HelloWorld.Validators
{
    public class PresentacionValidator : BaseNopValidator<Presentacion>
    {
        public PresentacionValidator(ILocalizationService localizationService)
        {
            RuleFor(a => a.Mensaje)
                .NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugin.Widgets.HelloWorld.Fields.Mensaje.Required"));
        }


    }
}
