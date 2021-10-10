using System.Collections.Generic;

namespace Nop.Plugin.Payments.EnZona.Models
{
    public class Payment
    {
        public string Description { get; set; }
        public string Currency { get; set; }
        public Amount Amount { get; set; }
        public List<Item> Items { get; set; }
        public long MerchantOpId { get; set; }
        public long InvoiceNumber { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
        public int TerminalId { get; set; }
        public string BuyerIdentityCode { get; set; }

    }
}
