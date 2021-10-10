using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.EnZona
{
    public class PaymentsEnZonaSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox (testing environment)
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to pass info about purchased items to EnZona
        /// </summary>
        public bool PassProductNamesAndTotals { get; set; }

        /// <summary>
        /// Gets or sets an additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }


        public string SandboxApiPaymentUrl { get; set; }


        public string ApiPaymentUrl { get; set; }


        /// <summary>
        /// Gets or sets credentials coded in Base64 string
        /// </summary>
        public string CodedCredentials { get; set; }

        public string SandboxApiTokenUrl { get; set; }

        public string ApiTokenUrl { get; set; }

        public long MerchantOpId { get; set; }

        public int WaitingTime { get; set; }
        public int RetryTimes { get; set; }
    }
}
