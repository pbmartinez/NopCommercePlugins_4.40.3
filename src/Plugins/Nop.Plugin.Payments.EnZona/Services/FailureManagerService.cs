using System;
using System.Net.Http;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Polly;

namespace Nop.Plugin.Payments.EnZona.Services
{
    public interface IFailureManagerService
    {
        Task<T> ExecuteWaitAndRetryAsync<T>(Func<Task<T>> action);
    }
    public class FailureManagerService : IFailureManagerService
    {
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;

        public FailureManagerService(
            ILogger logger,
            IWorkContext workContext,
            ILocalizationService localizationService,
            ISettingService settingService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _workContext = workContext ?? throw new ArgumentNullException(nameof(workContext));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            
        }


        public async Task<T> ExecuteWaitAndRetryAsync<T>(Func<Task<T>> action)
        {
            try
            {
                var currentCustomer = await _workContext.GetCurrentCustomerAsync();
                var settings = await _settingService.LoadSettingAsync<PaymentsEnZonaSettings>();
                var retries = new TimeSpan[settings.RetryTimes];
                for (var i = 0; i < retries.Length; i++)
                {
                    retries[i] = TimeSpan.FromSeconds(settings.WaitingTime);
                }

                var policy = Policy
                    .Handle<HttpRequestException>()
                    .WaitAndRetryAsync(retries,
                    async (exception, timeSpan, retryCount, context) =>
                    {
                        var errorMessage = string.Format(format:
                                await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.ErrorOnRetry"), exception.Message, action.Method.Name, retryCount, settings.RetryTimes, timeSpan);


                        if (retryCount < settings.RetryTimes)
                            await _logger.WarningAsync(errorMessage, exception, currentCustomer);
                        else
                            await _logger.ErrorAsync(errorMessage, exception, currentCustomer);

                    });

                return await policy.ExecuteAsync(async () => await action.Invoke());
            }
            catch (HttpRequestException)
            {
                throw new HttpRequestException(await _localizationService.GetResourceAsync("Plugins.Payments.EnZona.Messages.ErrorOnRequest"));
            }
        }


    }
}
