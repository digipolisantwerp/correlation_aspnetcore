using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Digipolis.ServiceAgents;
using SampleApi1.ServiceAgents;
using Digipolis.ServiceAgents.Settings;
using Digipolis.Correlation;
using System.IO;

namespace SampleApi1
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCorrelation();

            //Add a single service agent with a delegate that sets the correlation on the http client.
            services.AddSingleServiceAgent<SampleApi2Agent>(settings =>
            {
                settings.Scheme = HttpSchema.Http;
                settings.Host = "localhost";
                settings.Port = "5001";
                settings.Path = "api/";
            }, (serviceProvider, client) => client.SetCorrelationValues(serviceProvider));

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCorrelation("SampleApi1");

            app.UseMvc();
        }
    }
}
