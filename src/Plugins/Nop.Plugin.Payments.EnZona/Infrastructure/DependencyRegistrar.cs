using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Payments.EnZona.Services;

namespace Nop.Plugin.Payments.EnZona.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="appSettings">App settings</param>
        public void Register(IServiceCollection services, ITypeFinder typeFinder, AppSettings appSettings)
        {
            //services.AddScoped<CustomModelFactory, ICustomerModelFactory>();
            services.AddScoped<IPaymentResponseService, PaymentResponseService>();
            services.AddScoped<IPaymentDetailsService, PaymentDetailsService>();
            services.AddScoped<IFailureManagerService, FailureManagerService>();
            services.AddScoped<IXmlService, XmlService>();
        }

        public int Order => 1;
    }
}
