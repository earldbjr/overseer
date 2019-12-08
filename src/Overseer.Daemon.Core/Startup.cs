using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Hosting;
using Nancy.Owin;
using Overseer.Daemon.Bootstrapping;
using Overseer.Daemon.Hubs;
using Overseer.Models;
using System;
using Newtonsoft.Json;

namespace Overseer.Daemon
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;        
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "OverseerUI";                
            });

            services.AddAuthentication(OverseerAuthenticationOptions.Setup)
                .UseOverseerAuthentication();

            services.AddSignalR().AddNewtonsoftJsonProtocol(options =>
            {
                options.PayloadSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            services.Configure<IISServerOptions>(options => options.AllowSynchronousIO = true);
            services.Configure<KestrelServerOptions>(options => options.AllowSynchronousIO = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, OverseerBootstrapper bootstrapper, IHubContext<StatusHub> statusHub)
        {
            bootstrapper.Container.Register<Action<MachineStatus>>((c, n) =>
            {
                return status => StatusHub.PushStatusUpdate(statusHub, status);
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Map("/api", builder =>
            {
                builder.UseAuthentication();
                builder.UseOwin(owin => owin.UseNancy(options => options.Bootstrapper = bootstrapper));
            });

            app.Map("/push", builder =>
            {
                builder.UseRouting();
                builder.UseEndpoints(endpoints =>
                {
                    endpoints.MapHub<StatusHub>("/status");
                });
            });

            app.UseSpaStaticFiles();
            app.UseSpa(spa => 
            {
                if (env.IsDevelopment())
                {
                    spa.Options.SourcePath = "../OverseerUI";
                    spa.UseAngularCliServer(npmScript: "start");
                } 
                else
                {
                    spa.Options.SourcePath = "OverseerUI";
                }
            });
        }
    }
}
