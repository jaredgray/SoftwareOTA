using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Cors.Infrastructure;
using SoftwareOTA.Model;
using SoftwareOTA.Repository;
using SoftwareOTA.Contracts;
using Newtonsoft.Json;

namespace SoftwareOTA
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            var policy = new CorsPolicy();
            policy.Headers.Add("*");
            policy.Methods.Add("*");
            policy.Origins.Add("*");
            policy.SupportsCredentials = true;
            services.AddCors(configure => configure.AddPolicy("AllowAllClients", policy));

            services.AddOptions();
            //services.Configure<ConfigurationOptions>(Configuration);
            services.Configure<ConfigurationOptions>(Configuration.GetSection("ConfigurationOptions"));
            // Add framework services.
            services.AddMvc();
            ConfigureDependencies(services);
        }

        public void ConfigureDependencies(IServiceCollection services)
        {
            services.AddScoped<ConfigurationOptions, ConfigurationOptions>();
            services.AddScoped<IPackageRepository, PackageRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseCors("AllowAllClients");
            app.UseMvc();
            app.UseStaticFiles();
        }
    }
}
