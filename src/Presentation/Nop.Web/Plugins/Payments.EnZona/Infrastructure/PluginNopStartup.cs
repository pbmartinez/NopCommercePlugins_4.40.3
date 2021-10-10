using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.EnZona.Services;

namespace Nop.Plugin.Payments.EnZona.Infrastructure
{
    public class PluginNopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ViewLocationExpander());
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            //services.AddHttpClient<EnZonaHttpClient>().WithProxy();
            services.AddHttpClient<EnZonaHttpClient>();
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        public int Order => 11;
    }
}