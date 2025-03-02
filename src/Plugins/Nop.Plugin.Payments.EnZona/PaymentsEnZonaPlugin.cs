using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.EnZona.Extensions;
using Nop.Plugin.Payments.EnZona.Models;
using Nop.Plugin.Payments.EnZona.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Payments.EnZona
{
    /// <summary>
    /// Rename this file and change to the correct type
    /// </summary>
    public class PaymentsEnZonaPlugin : BasePlugin, IPaymentMethod, IAdminMenuPlugin
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _loggerService;
        private readonly IPaymentResponseService _paymentReponseService;
        private readonly IPaymentService _paymentService;
        private readonly IPaymentDetailsService _paymentDetailsService;
        private readonly INotificationService _notificationService;
        private readonly EnZonaHttpClient _enZonaHttpClient;
        private readonly PaymentsEnZonaSettings _paymentsEnZonaSettings;

        #endregion

        #region Ctor
        public PaymentsEnZonaPlugin(IWebHelper webHelper,
            ISettingService settingService,
            ILocalizationService localizationService,
            IHttpContextAccessor httpContextAccessor,
            ILogger loggerService,
            IPaymentResponseService paymentReponseService,
            IPaymentService paymentService,
            IPaymentDetailsService paymentDetailsService,
            INotificationService notificationService,
            EnZonaHttpClient enZonaHttpClient,
            PaymentsEnZonaSettings paymentsEnZonaSettings)
        {
            _webHelper = webHelper;
            _settingService = settingService;
            _localizationService = localizationService;
            _enZonaHttpClient = enZonaHttpClient;
            _paymentsEnZonaSettings = paymentsEnZonaSettings;
            _httpContextAccessor = httpContextAccessor;
            _loggerService = loggerService;
            _paymentReponseService = paymentReponseService;
            _paymentService = paymentService;
            _paymentDetailsService = paymentDetailsService;
            _notificationService = notificationService;
        }
        #endregion

        #region BasePlugin Implementation
        public override string GetConfigurationPageUrl()
        {
            var path = $"{_webHelper.GetStoreLocation()}Admin/PaymentEnZona/Configure";
            return path;
        }

        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new PaymentsEnZonaSettings
            {
                UseSandbox = true,
                SandboxApiPaymentUrl = "https://apisandbox.enzona.net/payment/v1.0.0/payments",
                ApiPaymentUrl = "https://api.enzona.net/payment/v1.0.0/payments",
                SandboxApiTokenUrl = "https://apisandbox.enzona.net/token",
                ApiTokenUrl = "https://apisandbox.enzona.net/token",
                PassProductNamesAndTotals = true,
                AdditionalFee = 0,
                WaitingTime = 5,
                RetryTimes = 3
            });

            //locales
            await _localizationService.AddLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Payments.EnZona.Fields.AdditionalFee"] = "Comisión adicional",
                ["Plugins.Payments.EnZona.Fields.AdditionalFee.Hint"] = "Entrar comisión adicional para cobrárselo a los clientes.",
                ["Plugins.Payments.EnZona.Fields.AdditionalFeePercentage"] = "Comisión adicional. Usar porcentaje.",
                ["Plugins.Payments.EnZona.Fields.AdditionalFeePercentage.Hint"] = "Determina si se aplica una tarifa adicional porcentual al total del pedido. Si no está habilitado, se usa un valor fijo.",
                ["Plugins.Payments.EnZona.Fields.PassProductNamesAndTotals"] = "Enviar detalles de la compra",
                ["Plugins.Payments.EnZona.Fields.PassProductNamesAndTotals.Hint"] = "Determina si se envian los detalles de la compra a EnZona",
                ["Plugins.Payments.EnZona.Fields.RedirectionTip"] = "Será redirigido al sitio de EnZona para completar el pedido.",
                ["Plugins.Payments.EnZona.Fields.UseSandbox"] = "Usar Sandbox",
                ["Plugins.Payments.EnZona.Fields.UseSandbox.Hint"] = "Marque para habilitar Sandbox (entorno de prueba).",
                ["Plugins.Payments.EnZona.Fields.SandboxApiPaymentUrl"] = "URL de Sandbox de Api de pago.",
                ["Plugins.Payments.EnZona.Fields.SandboxApiPaymentUrl.Hint"] = "URL de Sandbox de Api de pago. (entorno de prueba).",
                ["Plugins.Payments.EnZona.Fields.ApiPaymentUrl"] = "URL de EnZona entorno real.",
                ["Plugins.Payments.EnZona.Fields.ApiPaymentUrl.Hint"] = "URL de EnZona entorno real.",
                ["Plugins.Payments.EnZona.Fields.SandboxApiTokenUrl"] = "URL de Token Sandbox.",
                ["Plugins.Payments.EnZona.Fields.SandboxApiTokenUrl.Hint"] = "URL de Token Sandbox. (entorno de prueba).",
                ["Plugins.Payments.EnZona.Fields.ApiTokenUrl"] = "URL de Token entorno real.",
                ["Plugins.Payments.EnZona.Fields.ApiTokenUrl.Hint"] = "URL de EnZona para el manejo del Token entorno real.",
                ["Plugins.Payments.EnZona.Fields.CodedCredentials"] = "Credenciales",
                ["Plugins.Payments.EnZona.Fields.CodedCredentials.Hint"] = "Sus credenciales condificadas en base a 64 bit.",
                ["Plugins.Payments.EnZona.Fields.MerchantOpId"] = "Id de Operación",
                ["Plugins.Payments.EnZona.Fields.MerchantOpId.Hint"] = "Id de Operación del Comercio Registrado.",
                ["Plugins.Payments.EnZona.Fields.WaitingTime"] = "Tiempo de espera",
                ["Plugins.Payments.EnZona.Fields.WaitingTime.Hint"] = "Tiempo de espera en segundos entre una peticiones a la api de EnZona.",
                ["Plugins.Payments.EnZona.Fields.RetryTimes"] = "Intentos",
                ["Plugins.Payments.EnZona.Fields.RetryTimes.Hint"] = "Intentos de peticiones a realizar antes de darse por vencido con la api de EnZona.",


                ["Plugins.Payments.EnZona.Messages.FieldRequired"] = "El campo es requerido.",
                ["Plugins.Payments.EnZona.Messages.RangeOnPercentage"] = "El rango de por ciento no es válido.",
                ["Plugins.Payments.EnZona.Messages.ErrorCancellingPaidOrders"] = "Las órdenes pagadas no pueden ser canceladas.",
                ["Plugins.Payments.EnZona.Messages.ErrorPayingCancelledOrders"] = "Las órdenes canceladas no pueden ser pagadas. Intente adicionando una orden nueva.",
                ["Plugins.Payments.EnZona.Messages.ErrorResourceNotFound"] = "El recurso solicitado no existe.",
                ["Plugins.Payments.EnZona.Messages.SuccessOrderCancellation"] = "La orden ha sido cancelada correctamente.",
                ["Plugins.Payments.EnZona.Messages.SuccessOrderCompletion"] = "La orden ha sido completada correctamente.",
                ["Plugins.Payments.EnZona.Messages.NullObject"] = "El recurso no puede ser null {0}.",
                ["Plugins.Payments.EnZona.Messages.ErrorOnRetry"] = "{0}. Intentando {1} {2} de {3}. Esperando {4} segundos para intentar de nuevo.",
                ["Plugins.Payments.EnZona.Messages.ErrorOnRequest"] = "No es posible establecer comunicación con el servidor de EnZona para realizar el pago.",
                ["Plugins.Payments.EnZona.Messages.InvalidCredentials"] = "Las credenciales del comercio proporcionadas a EnZona  no son válidas.",
                ["Plugins.Payments.EnZona.Messages.Forbidden"] = "Las credenciales proporcionadas no cuenta con permiso para acceder al recurso solicitado.",
                ["Plugins.Payments.EnZona.Messages.UnavailableService"] = "El servicio de EnZona no se encuentra disponible.",


                ["Plugins.Payments.EnZona.Labels.Invoice"] = "Factura",
                ["Plugins.Payments.EnZona.Labels.Transaction"] = "Transacción",
                ["Plugins.Payments.EnZona.Labels.Order"] = "Orden",


                ["Plugins.Payments.EnZona.Instructions"] = @"
                    <p>
	                    <br />Para utilizar este plugin debe completar las siguientes instrucciones:<br />
	                    <br />1. Autentíquese en el siguiente link (<a href=""https://bulevar.enzona.net"" target=""_blank"">aquí</a>) y regístrese como un nuevo comercio.
	                    <br />2. Debe vincular el nuevo comercio con las api de pago de enzona.
	                    <br />3. Obtener la consumer_key y consumer_secret.
	                    <br />4. Codificarlos en una cadena en base a 64 con el siguiente formato <b>consumer_key:consumer_secret</b>.
	                    <br />5. Establecer dicha cadena codificada como la cadena credenciales en la configuracion del plugin.
	                    <br />
                    </p>",
                ["Plugins.Payments.EnZona.PaymentMethodDescription"] = "Usted será redireccionado al sitio de EnZona para realizar el pago",

            });
            await base.InstallAsync();
        }
        public override async Task UninstallAsync()
        {
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Payments.EnZona");
            await base.UninstallAsync();
        }

        #endregion

        #region IPaymentMethod Implementation
        public bool SupportCapture => false;

        public bool SupportPartiallyRefund => false;

        public bool SupportRefund => false;

        public bool SupportVoid => false;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public bool SkipPaymentInfo => false;

        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return Task.FromResult(new CancelRecurringPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }

        public Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //let's ensure that at least 5 seconds passed after order is placed
            //P.S. there's no any particular reason for that. we just do it
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds < 5)
                return Task.FromResult(false);

            return Task.FromResult(true);
        }

        public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            return Task.FromResult(new CapturePaymentResult { Errors = new[] { "Capture method not supported" } });
        }

        public async Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return await _paymentService.CalculateAdditionalFeeAsync(cart,
                _paymentsEnZonaSettings.AdditionalFee, _paymentsEnZonaSettings.AdditionalFeePercentage);
        }

        public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            return Task.FromResult(new ProcessPaymentRequest());
        }

        public async Task<string> GetPaymentMethodDescriptionAsync()
        {
            return await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.PaymentMethodDescription");
        }

        public string GetPublicViewComponentName()
        {
            return "PaymentEnZona";
        }

        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(false);
        }

        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //2do paso en el payment process
            var paymentResponse = await _paymentReponseService.GetByTransaccionUuid(postProcessPaymentRequest.Order.AuthorizationTransactionId);
            var links = JsonConvertExtensions.DeserializeForApiEnZona<List<Link>>(paymentResponse.LinksAsJson);

            //cada pago se crea con varios links, uno de ellos es el de confirmar el pago
            //que es a donde se redirige al cliente para que complete el pago
            var link = links.FirstOrDefault(l => l.Rel == LinkRelation.confirm.ToString());
            _httpContextAccessor.HttpContext.Response.Redirect(link.Href);
            await Task.CompletedTask;
            //3er paso en el payment process se concreta en el PaymentEnZonaController
            //al recibir la respuesta de la pasarela de pago
            //ya sea success or fail en Complete or Cancel respectivamente, actualizando el payment en la base de datos de nopcommerce
        }

        public async Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            //1er paso en el payment process
            //este paso se ejecuta primero que el PostProcessPaymentAsync
            try
            {
                var processPaymentResult = new ProcessPaymentResult();

                // Aqui es donde se crea el pago            
                var resultToken = await _enZonaHttpClient.AcquireTokenAsync();
                if (resultToken.Success && resultToken.StatusCode == HttpStatusCode.OK)
                {
                    var pago = await _paymentDetailsService.GetPaymentAsync(processPaymentRequest);

                    //aqui se crea el pago en la pasarela de pago
                    var resultPayment = await _enZonaHttpClient.CreatePaymentAsync(null, pago, accessToken: resultToken.Value.AccessToken);
                    switch (resultPayment.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            processPaymentResult.AuthorizationTransactionId = resultPayment.Value.TransactionUuid;
                            //en caso de que se haya creado el pago correctamente en la pasarela se guarda en la base de datos de nopcommerce
                            await _paymentReponseService.InsertAsync(resultPayment.Value);
                            break;

                        default:
                            processPaymentResult.AddError(resultPayment.ErrorMessage + JsonConvertExtensions.SerializeForApiEnZona(pago));
                            await _loggerService.ErrorAsync(resultPayment.ErrorMessage + JsonConvertExtensions.SerializeForApiEnZona(pago));
                            break;
                    }
                }
                else
                {
                    processPaymentResult.AddError(resultToken.ErrorMessage);
                }
                return processPaymentResult;
            }
            catch (HttpRequestException)
            {
                var errorMessage = await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.ErrorOnRequest");
                return new ProcessPaymentResult { Errors = new List<string> { errorMessage } };
            }

        }

        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            throw new System.NotImplementedException();
        }

        public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            return await Task.FromResult(new List<string>());
        }

        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region Admin Menu Implementation
        public Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                SystemName = "EnZona",
                Title = "EnZona Plugin",
                Visible = true,
                IconClass = "far fa-dot-circle",
                ChildNodes = new List<SiteMapNode>()
                {
                    new SiteMapNode
                    {
                        SystemName = "EnZona",
                        Title = "Transacciones",
                        ControllerName = "EnZonaPaymentResponse",
                        ActionName = "List",
                        Visible = true,
                        IconClass = "far fa-circle",
                        RouteValues = new RouteValueDictionary() { { "area", "Admin" } }
                    }
                }
            };

            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");
            if (pluginNode != null)
                pluginNode.ChildNodes.Add(menuItem);
            else
                rootNode.ChildNodes.Add(menuItem);

            return Task.CompletedTask;
        }
        #endregion

    }
}
