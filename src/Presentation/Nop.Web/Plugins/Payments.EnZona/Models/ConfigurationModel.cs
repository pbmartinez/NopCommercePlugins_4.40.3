using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;


namespace Nop.Plugin.Payments.EnZona.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.EnZona.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.EnZona.Fields.PassProductNamesAndTotals")]
        public bool PassProductNamesAndTotals { get; set; }
        public bool PassProductNamesAndTotals_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.EnZona.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.EnZona.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.EnZona.Fields.SandboxApiPaymentUrl")]
        public string SandboxApiPaymentUrl { get; set; }
        public bool SandboxApiPaymentUrl_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.EnZona.Fields.ApiPaymentUrl")]
        public string ApiPaymentUrl { get; set; }
        public bool ApiPaymentUrl_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.EnZona.Fields.SandboxApiTokenUrl")]
        public string SandboxApiTokenUrl { get; set; }
        public bool SandboxApiTokenUrl_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.EnZona.Fields.ApiTokenUrl")]
        public string ApiTokenUrl { get; set; }
        public bool ApiTokenUrl_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.EnZona.Fields.CodedCredentials")]
        public string CodedCredentials { get; set; }
        public bool CodedCredentials_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.EnZona.Fields.MerchantOpId")]
        public long MerchantOpId { get; set; }
        public bool MerchantOpId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.EnZona.Fields.WaitingTime")]
        public int WaitingTime { get; set; }
        public bool WaitingTime_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.EnZona.Fields.RetryTimes")]
        public int RetryTimes { get; set; }
        public bool RetryTimes_OverrideForStore { get; set; }

    }
}
