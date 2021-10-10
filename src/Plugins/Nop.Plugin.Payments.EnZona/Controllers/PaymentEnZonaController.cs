using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.EnZona.Models;
using Nop.Plugin.Payments.EnZona.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.EnZona.Controllers
{
    public class PaymentEnZonaController : BasePaymentController
    {
        #region Fields
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IPaymentResponseService _paymentResponseService;
        private readonly IMapper _mapper;
        private readonly IXmlService _xmlService;
        #endregion

        #region Ctor

        public PaymentEnZonaController(
            IOrderService orderService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IPaymentResponseService paymentResponseService,
            IMapper mapper,
            IXmlService xmlService)
        {
            _orderService = orderService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _settingService = settingService;
            _storeContext = storeContext;
            _workContext = workContext;
            _paymentResponseService = paymentResponseService;
            _mapper = mapper;
            _xmlService = xmlService;
        }

        #endregion

        #region Configuration 
        [HttpGet]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var paymentsEnZonaSettings = await _settingService.LoadSettingAsync<PaymentsEnZonaSettings>(storeScope);

            var model = _mapper.Map<ConfigurationModel>(paymentsEnZonaSettings);

            if (storeScope <= 0)
                return View("~/Plugins/Payments.EnZona/Views/Configure.cshtml", model);

            model.UseSandbox_OverrideForStore = await _settingService.SettingExistsAsync(paymentsEnZonaSettings, x => x.UseSandbox, storeScope);
            model.PassProductNamesAndTotals_OverrideForStore = await _settingService.SettingExistsAsync(paymentsEnZonaSettings, x => x.PassProductNamesAndTotals, storeScope);
            model.AdditionalFee_OverrideForStore = await _settingService.SettingExistsAsync(paymentsEnZonaSettings, x => x.AdditionalFee, storeScope);
            model.AdditionalFeePercentage_OverrideForStore = await _settingService.SettingExistsAsync(paymentsEnZonaSettings, x => x.AdditionalFeePercentage, storeScope);
            model.SandboxApiPaymentUrl_OverrideForStore = await _settingService.SettingExistsAsync(paymentsEnZonaSettings, x => x.SandboxApiPaymentUrl, storeScope);
            model.ApiPaymentUrl_OverrideForStore = await _settingService.SettingExistsAsync(paymentsEnZonaSettings, x => x.ApiPaymentUrl, storeScope);
            model.SandboxApiTokenUrl_OverrideForStore = await _settingService.SettingExistsAsync(paymentsEnZonaSettings, x => x.SandboxApiTokenUrl, storeScope);
            model.ApiTokenUrl_OverrideForStore = await _settingService.SettingExistsAsync(paymentsEnZonaSettings, x => x.ApiTokenUrl, storeScope);
            model.CodedCredentials_OverrideForStore = await _settingService.SettingExistsAsync(paymentsEnZonaSettings, x => x.CodedCredentials, storeScope);
            model.MerchantOpId_OverrideForStore = await _settingService.SettingExistsAsync(paymentsEnZonaSettings, x => x.MerchantOpId, storeScope);
            model.WaitingTime_OverrideForStore = await _settingService.SettingExistsAsync(paymentsEnZonaSettings, x => x.WaitingTime, storeScope);
            model.RetryTimes_OverrideForStore = await _settingService.SettingExistsAsync(paymentsEnZonaSettings, x => x.RetryTimes, storeScope);


            return View("~/Plugins/Payments.EnZona/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var paymentsEnZonaSettings = await _settingService.LoadSettingAsync<PaymentsEnZonaSettings>(storeScope);

            paymentsEnZonaSettings = _mapper.Map<PaymentsEnZonaSettings>(model);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(paymentsEnZonaSettings, x => x.UseSandbox, model.UseSandbox_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paymentsEnZonaSettings, x => x.PassProductNamesAndTotals, model.PassProductNamesAndTotals_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paymentsEnZonaSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paymentsEnZonaSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paymentsEnZonaSettings, x => x.SandboxApiPaymentUrl, model.SandboxApiPaymentUrl_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paymentsEnZonaSettings, x => x.ApiPaymentUrl, model.ApiPaymentUrl_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paymentsEnZonaSettings, x => x.SandboxApiTokenUrl, model.SandboxApiTokenUrl_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paymentsEnZonaSettings, x => x.ApiTokenUrl, model.ApiTokenUrl_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paymentsEnZonaSettings, x => x.CodedCredentials, model.CodedCredentials_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paymentsEnZonaSettings, x => x.MerchantOpId, model.MerchantOpId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paymentsEnZonaSettings, x => x.WaitingTime, model.WaitingTime_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paymentsEnZonaSettings, x => x.RetryTimes, model.RetryTimes_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion

        #region Payment

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction_uuid"></param>
        /// <returns></returns>
        public async Task<IActionResult> Complete(string transaction_uuid)
        {
            if (string.IsNullOrEmpty(transaction_uuid) || string.IsNullOrWhiteSpace(transaction_uuid))
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.ErrorResourceNotFound"));
                return RedirectToRoute("Homepage");
            }
            var order = await (await _orderService.SearchOrdersAsync((await _storeContext.GetCurrentStoreAsync()).Id,
                customerId: (await _workContext.GetCurrentCustomerAsync()).Id))
                .FirstOrDefaultAwaitAsync(async o => await Task.FromResult(o.AuthorizationTransactionId == transaction_uuid));
            if (order == null)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.ErrorResourceNotFound"));
                return RedirectToRoute("Homepage");
            }
            if (order.OrderStatus == OrderStatus.Cancelled)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.ErrorPayingCancelledOrders"));
                return RedirectToRoute("OrderDetails", new { orderId = order.Id });
            }
            order.PaymentStatus = PaymentStatus.Paid;

            order = await _xmlService.UpdateOrderPaymentDetailsAsync(transaction_uuid, order);

            await _orderService.UpdateOrderAsync(order);
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.SuccessOrderCompletion"));

            return RedirectToRoute("OrderDetails", new { orderId = order.Id });
        }

        public async Task<IActionResult> Cancel(string transaction_uuid)
        {
            if (string.IsNullOrEmpty(transaction_uuid) || string.IsNullOrWhiteSpace(transaction_uuid))
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.ErrorResourceNotFound"));
                return RedirectToRoute("Homepage");
            }
            var order = await (await _orderService.SearchOrdersAsync((await _storeContext.GetCurrentStoreAsync()).Id,
                customerId: (await _workContext.GetCurrentCustomerAsync()).Id))
                .FirstOrDefaultAwaitAsync(async o => await Task.FromResult(o.AuthorizationTransactionId == transaction_uuid));
            if (order == null)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.ErrorResourceNotFound"));
                return RedirectToRoute("Homepage");
            }
            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.ErrorCancellingPaidOrders"));
                return RedirectToRoute("OrderDetails", new { orderId = order.Id });
            }
            order.PaymentStatus = PaymentStatus.Pending;
            order.OrderStatus = OrderStatus.Cancelled;
            await _orderService.UpdateOrderAsync(order);
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.SuccessOrderCancellation"));
            return RedirectToRoute("OrderDetails", new { orderId = order.Id });
        }
        #endregion
    }
}
