using System.Xml;

namespace Nop.Plugin.Payments.EnZona.Extensions
{
    public static class XmlExtensions
    {
        public const string KEY_CUSTOM_VALUES_ITEM = "item";
        public static string GetPaymentResponseAsJson(string xmlText)
        {
            var root = new XmlDocument();
            root.LoadXml(xmlText);

            var items = root.GetElementsByTagName(KEY_CUSTOM_VALUES_ITEM);

            var paymentResponseAsJson = items.Item(0).LastChild.InnerText;
            return paymentResponseAsJson;
        }
    }
}
