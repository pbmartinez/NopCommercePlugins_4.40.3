using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Payments.EnZona.Extensions;
using Nop.Plugin.Payments.EnZona.Models;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;

namespace Nop.Plugin.Payments.EnZona.Services
{
    public interface IPaymentDetailsService
    {
        Task<Payment> GetPaymentAsync(ProcessPaymentRequest processPaymentRequest);
        Task<string> GetCurrencyAsync();
        Task<int> GetInvoiceNumberAsync();
        Task<Amount> GetAmountAsync(ProcessPaymentRequest processPaymentRequest);
        Task<List<Item>> GetItemProductsAsync();
    }


    public class PaymentDetailsService : IPaymentDetailsService
    {
        private readonly IWorkContext _workContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IRepository<Order> _orderRepository;
        private readonly PaymentsEnZonaSettings _paymentsEnZonaSettings;
        private readonly IShippingService _shippingService;

        public PaymentDetailsService(IWorkContext workContext,
                                     IShoppingCartService shoppingCartService,
                                     IProductService productService,
                                     IOrderService orderService,
                                     IStoreContext storeContext,
                                     IWebHelper webHelper,
                                     IRepository<Order> orderRepository,
                                     PaymentsEnZonaSettings paymentsEnZonaSettings,
                                     IShippingService shippingService)
        {
            _workContext = workContext ?? throw new ArgumentNullException(nameof(workContext));
            _shoppingCartService = shoppingCartService ?? throw new ArgumentNullException(nameof(shoppingCartService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _storeContext = storeContext ?? throw new ArgumentNullException(nameof(storeContext));
            _webHelper = webHelper ?? throw new ArgumentNullException(nameof(webHelper));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _paymentsEnZonaSettings = paymentsEnZonaSettings ?? throw new ArgumentNullException(nameof(paymentsEnZonaSettings));
            _shippingService = shippingService;
        }

        public async Task<Payment> GetPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            var pago = new Payment
            {
                Description = "descripcion del pago",
                Currency = await GetCurrencyAsync(),
                Amount = await GetAmountAsync(processPaymentRequest),
                Items = await GetItemProductsAsync(),
                MerchantOpId = _paymentsEnZonaSettings.MerchantOpId,
                InvoiceNumber = await GetInvoiceNumberAsync(),
                ReturnUrl = $"{_webHelper.GetStoreLocation()}Plugins/PaymentEnZona/Complete",
                CancelUrl = $"{_webHelper.GetStoreLocation()}Plugins/PaymentEnZona/Cancel",
                TerminalId = 1,
                BuyerIdentityCode = ""
            };
            return pago;
        }

        public async Task<string> GetCurrencyAsync()
        {
            var currency = await _workContext.GetWorkingCurrencyAsync();
            return currency.CurrencyCode;
        }

        public async Task<int> GetInvoiceNumberAsync()
        {
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var numberOfOrders = await _orderRepository.Table.Where(o => o.StoreId == currentStore.Id).CountAsync();
            _ = int.TryParse($"{currentStore.Id * Math.Pow(10, 8) + numberOfOrders + 1}", out var salida);

            return salida;
        }

        public async Task<Amount> GetAmountAsync(ProcessPaymentRequest processPaymentRequest)
        {
            var items = await GetItemProductsAsync();
            var potencial = items.Aggregate(0.0, (suma, item) => suma += item.Quantity * item.Price);
            var shoppinCartItems = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, _storeContext.GetCurrentStore().Id);
            var shippingResponse = await _shippingService.GetShippingOptionsAsync(shoppinCartItems, null);
            var shippingAmmont = (double)shippingResponse.ShippingOptions.Aggregate(0m, (a, b) => a + b.Rate);
            var totalToPay = (double)processPaymentRequest.OrderTotal;
            
            var descuento = (shippingAmmont + potencial) > totalToPay
                ? (shippingAmmont + potencial) - totalToPay
                : 0.0;
            return new Amount
            {
                //Total = Math.Truncate(totalToPay * 100 / 100),
                //Details = new Details
                //{
                //    Discount = Math.Truncate(descuento *100/100),
                //    Shipping = Math.Truncate(shippingAmmont * 100 / 100),
                //    Tax = Math.Truncate(0.0 *100 / 100),
                //    Tip = Math.Truncate(0.0 * 100 / 100),
                //}
                Total = totalToPay,
                Details = new Details
                {
                    Discount = descuento,
                    Shipping = shippingAmmont,
                    Tax = 0.0,
                    Tip = 0.0,
                }
            };
        }
        public async Task<List<Item>> GetItemProductsAsync()
        {
            if (!_paymentsEnZonaSettings.PassProductNamesAndTotals)
                return new List<Item>();

            var shoppinCartItems = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, _storeContext.GetCurrentStore().Id);


            var items = new List<Item>(shoppinCartItems.Count);
            Parallel.ForEach(shoppinCartItems, async (item) =>
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                if (product != null)
                {
                    var frecuency = item.RentalStartDateUtc == null || item.RentalEndDateUtc == null
                    ? 1
                    : (item.RentalEndDateUtc.Value - item.RentalStartDateUtc.Value).TotalDays; 

                    items.Add(new Item
                    {
                        Description = product.ShortDescription?.Cleaned()?.Length > 50
                            ? product.ShortDescription?.Cleaned()?.Substring(0, 50)
                            : product.ShortDescription?.Cleaned(),

                        Name = product.Name?.Cleaned(),
                        Price = (double)product.Price * frecuency,
                        Quantity = shoppinCartItems.FirstOrDefault(sci => sci.ProductId == product.Id)?.Quantity ?? 0,
                        Tax = 0
                    });
                }
            });

            return items;
        }
    }
}
