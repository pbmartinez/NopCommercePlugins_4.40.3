using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Widgets.HelloWorld.Domains;

namespace Nop.Plugin.Widgets.HelloWorld.Infrastructure
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
            services.AddScoped<IRepository<Presentacion>, EntityRepository<Presentacion>>();
        }

        public int Order => 1;
    }
}
