using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Payments.EnZona.Extensions;
using Nop.Plugin.Payments.EnZona.Models;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Plugin.Payments.EnZona.Services
{
    public partial class EnZonaHttpClient
    {
        #region Fields
        private readonly HttpClient _httpClient;
        private readonly PaymentsEnZonaSettings _paymentsEnZonaSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly IFailureManagerService _failureManagerService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public EnZonaHttpClient(HttpClient httpClient,
            PaymentsEnZonaSettings paymentsEnZonaSettings,
            IHttpContextAccessor httpContextAccessor,
            ILogger logger,
            IWorkContext workContext,
            IFailureManagerService failureManagerService,
            ILocalizationService localizationService)
        {

            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, $"nopCommerce-{NopVersion.CURRENT_VERSION}");
            _paymentsEnZonaSettings = paymentsEnZonaSettings ?? throw new ArgumentNullException(nameof(paymentsEnZonaSettings));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _workContext = workContext ?? throw new ArgumentNullException(nameof(workContext));
            _failureManagerService = failureManagerService ?? throw new ArgumentNullException(nameof(failureManagerService));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
        }
        #endregion

        #region Methods

        public async Task<HttpResponseMessage> PostRequestAsync(string endpoint, HttpContent requestContent)
        {
            var response = await _httpClient.PostAsync(endpoint, requestContent);
            return response;
        }
        public async Task<HttpResponseMessage> PostRequestAsync(string endpoint, StringContent requestContent)
        {
            var response = await _httpClient.PostAsync(endpoint, requestContent);
            return response;
        }
        public async Task<Result<T>> DealWithResponseAsync<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            return response.StatusCode switch
            {
                HttpStatusCode.OK => new Result<T>(JsonConvertExtensions.DeserializeForApiEnZona<T>(content), response.StatusCode),
                HttpStatusCode.Unauthorized => new Result<T>(default, response.StatusCode, errorMessage: await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.InvalidCredentials")),
                HttpStatusCode.Forbidden => new Result<T>(default, response.StatusCode, errorMessage: await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.Forbidden")),
                HttpStatusCode.ServiceUnavailable => new Result<T>(default, response.StatusCode, errorMessage: await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.UnavailableService")),
                HttpStatusCode.BadRequest => new Result<T>(default, response.StatusCode, 
                        //errorMessage: JsonConvertExtensions.DeserializeForApiEnZona<Fault>(content, Formatting.Indented).ToString() + JsonConvertExtensions.DeserializeForApiEnZona<OauthError>(content, Formatting.Indented).ToString()),
                        errorMessage: content),
                _ => new Result<T>(default, response.StatusCode, errorMessage: await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.SomethingHappen")),
            };
        }
        public async Task<Result<Token>> AcquireTokenAsync()
        {
            try
            {
                var baseUrl = _paymentsEnZonaSettings.UseSandbox ? $"{_paymentsEnZonaSettings.SandboxApiTokenUrl}" : $"{_paymentsEnZonaSettings.ApiTokenUrl}";
                var requestContent = new StringContent($"scope=enzona_business_payment&grant_type=client_credentials", Encoding.UTF8, MimeTypes.ApplicationXWwwFormUrlencoded);
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {_paymentsEnZonaSettings.CodedCredentials}");

                var response = await _failureManagerService.ExecuteWaitAndRetryAsync(async () => await PostRequestAsync(baseUrl, requestContent));
                var con = await response.Content.ReadAsStringAsync();
                return await DealWithResponseAsync<Token>(response);
            }
            catch (HttpRequestException ex)
            {
                await _logger.ErrorAsync(ex.Message, ex, await _workContext.GetCurrentCustomerAsync());
                return new Result<Token>(null, HttpStatusCode.InternalServerError, false, ex.Message);
            }

        }

        public async Task<Result<PaymentResponse>> CreatePaymentAsync(string endpoint, Payment payment, HttpContent requestContent = null, string accessToken = null)
        {
            try
            {
                var baseUrl = _paymentsEnZonaSettings.UseSandbox ? $"{_paymentsEnZonaSettings.SandboxApiPaymentUrl}" : $"{_paymentsEnZonaSettings.ApiPaymentUrl}";
                endpoint = string.IsNullOrEmpty(endpoint) ? baseUrl : endpoint;

                var paymentAsJson = JsonConvertExtensions.SerializeForApiEnZona(payment);
                requestContent = new StringContent($"{paymentAsJson}", Encoding.UTF8, MimeTypes.ApplicationJson);
                var request = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = requestContent };

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{accessToken}");
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _failureManagerService.ExecuteWaitAndRetryAsync(async () => await PostRequestAsync(endpoint, requestContent));
                var con = await response.Content.ReadAsStringAsync();
                return await DealWithResponseAsync<PaymentResponse>(response);
            }
            catch (HttpRequestException ex)
            {
                var errorMessage = await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.ErrorOnRequest");
                await _logger.ErrorAsync(ex.Message, ex, await _workContext.GetCurrentCustomerAsync());

                return new Result<PaymentResponse>(null, HttpStatusCode.InternalServerError, false, errorMessage + ex.Message);
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex, await _workContext.GetCurrentCustomerAsync());
                return new Result<PaymentResponse>(null, HttpStatusCode.InternalServerError, false, ex.Message);
            }
        }

        public HttpRequestHeaders Headers => _httpClient.DefaultRequestHeaders;
        #endregion
    }
}