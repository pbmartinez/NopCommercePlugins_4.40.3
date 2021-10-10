using FluentValidation;
using Nop.Plugin.Payments.EnZona.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.EnZona.Validators
{
    public class ConfigurationModelValidator : BaseNopValidator<ConfigurationModel>
    {
        public ConfigurationModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.ApiPaymentUrl)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.FieldRequired"));

            RuleFor(x => x.ApiTokenUrl)
               .NotEmpty()
               .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.FieldRequired"));

            RuleFor(x => x.CodedCredentials)
               .NotEmpty()
               .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.FieldRequired"));

            RuleFor(x => x.MerchantOpId)
               .NotEmpty()
               .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.FieldRequired"));

            RuleFor(x => x.SandboxApiPaymentUrl)
               .NotEmpty()
               .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.FieldRequired"));

            RuleFor(x => x.SandboxApiTokenUrl)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.FieldRequired"));

            RuleFor(x => x.AdditionalFee)
                .ExclusiveBetween(0, 100)
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.RangeOnPercentage"))
                .When(x => x.AdditionalFeePercentage);

        }


    }
}
