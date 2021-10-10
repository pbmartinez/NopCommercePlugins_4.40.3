using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Nop.Core.Domain.Orders;
using Nop.Services.Localization;
using Nop.Services.Payments;

namespace Nop.Plugin.Payments.EnZona.Services
{
    public interface IXmlService
    {
        Task<Order> UpdateOrderPaymentDetailsAsync(string transactionUuid, Order order);
    }
    public class XmlService : IXmlService
    {

        private readonly IPaymentResponseService _paymentResponseService;
        private readonly ILocalizationService _localizationService;

        public XmlService(IPaymentResponseService paymentResponseService,
            ILocalizationService localizationService)
        {
            _paymentResponseService = paymentResponseService ?? throw new ArgumentNullException(nameof(paymentResponseService));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
        }

        public async Task<Order> UpdateOrderPaymentDetailsAsync(string transactionUuid, Order order)
        {

            var paymentResponse = await _paymentResponseService.GetByTransaccionUuid(transactionUuid);
            if (order == null)
                throw new ArgumentNullException(string.Format(await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.NullObject"), nameof(order)));
            if (paymentResponse == null)
                throw new NullReferenceException(string.Format(await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.NullObject"), nameof(paymentResponse)));

            var diccionario = new Dictionary<string, object>
            {
                { await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Labels.Invoice"), paymentResponse.InvoiceNumber },
                { await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Labels.Transaction"), paymentResponse.TransactionUuid },
                { await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Labels.Order"), order.OrderGuid.ToString() }
            };
            var dicSerializer = new DictionarySerializer(diccionario);
            var xmlSerializer = new XmlSerializer(typeof(DictionarySerializer));
            using var textWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(textWriter);
            xmlSerializer.Serialize(xmlWriter, dicSerializer);
            var xmlText = textWriter.ToString();
            order.CustomValuesXml = xmlText;
            return order;
        }
    }
}
