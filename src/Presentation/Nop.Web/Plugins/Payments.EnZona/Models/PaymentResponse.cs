using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Payments.EnZona.Extensions;

namespace Nop.Plugin.Payments.EnZona.Models
{
    public class PaymentResponse : BaseEntity
    {
        public string TransactionUuid { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int StatusCode { get; set; }
        public string StatusDenom { get; set; }
        public string Description { get; set; }
        public string InvoiceNumber { get; set; }
        public string MerchantOpId { get; set; }
        public string TerminalId { get; set; }
        public Amount Amount { get; set; }
        public List<Item> Items { get; set; }
        public List<Link> Links { get; set; }
        public double Commission { get; set; }

        private string _linksAsJson = "";
        public string LinksAsJson
        {
            get =>
                !string.IsNullOrEmpty(_linksAsJson)
                ? _linksAsJson
                : Links != null
                ? JsonConvertExtensions.SerializeForApiEnZona(Links)
                : "[]";

            set => _linksAsJson = value;
        }

        private string _amountAsJson = "";
        public string AmountAsJson
        {
            get =>
                !string.IsNullOrEmpty(_amountAsJson)
                ? _amountAsJson
                : Amount != null
                ? JsonConvertExtensions.SerializeForApiEnZona(Amount)
                : "0.00,[]";

            set => _amountAsJson = value;
        }
        private string _itemsAsJson = "";
        public string ItemsAsJson
        {
            get =>
                !string.IsNullOrEmpty(_itemsAsJson)
                ? _itemsAsJson
                : Items != null
                ? JsonConvert.SerializeObject(Items)
                : "[]";

            set => _itemsAsJson = value;
        }

    }
}
