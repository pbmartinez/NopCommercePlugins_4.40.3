using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.EnZona.Models;

namespace Nop.Plugin.Payments.EnZona.Extensions
{
    public static class OrderExtensions
    {
        public static Payment GetPaymentDetails(this Order order)
        {

            /*var pago = new Payment
            {
                Description = "Este es un pago manual para probar",
                Currency = "CUP",

                Amount = new Amount
                {
                    Total = 4.91,
                    Details = new Details
                    {
                        Discount = 0.00,
                        Shipping = 0.91,
                        Tax = 0.00,
                        Tip = 0.00
                    }
                },
                Items = new List<Item>
                {
                    new Item {
                        Description = "Este es uno de lo sitems",
                        Name = "Cremita de coco",
                        Price = 4.00,
                        Quantity = 1,
                        Tax = 0.00
                    }
                },
                MerchantOpId = 123456789123,
                InvoiceNumber = 1212,
                ReturnUrl = $"{_webHelper.GetStoreLocation()}Admin/PaymentEnZona/CompleteOrder",
                CancelUrl = $"{_webHelper.GetStoreLocation()}Admin/PaymentEnZona/CancelOrder",
                TerminalId = 12121,
                BuyerIdentityCode = ""
            };*/

            var payment = new Payment()
            {
                Description = order.CheckoutAttributeDescription,

            };




            return payment;
        }
    }
}
