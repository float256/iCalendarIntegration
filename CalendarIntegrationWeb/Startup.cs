using CalendarIntegrationCore.Services;
using CalendarIntegrationCore.Services.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using CalendarIntegrationCore.Models;
using Microsoft.Extensions.Logging;
using CalendarIntegrationWeb.Services;

namespace CalendarIntegrationWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:default"]));
            services.AddScoped<IHotelRepository, HotelRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IBookingInfoRepository, BookingInfoRepository>();
            services.AddScoped<ICalendarParser, CalendarParser>();
            services.AddScoped<IAvailabilityInfoReceiver, AvailabilityInfoReceiver>();
            services.AddScoped<IAvailabilityInfoSaver, AvailabilityInfoSaver>();
            services.AddScoped<IAvailabilityInfoDataProcessor, AvailabilityInfoDataProcessor>();
            services.AddScoped<IAvailabilityInfoService, AvailabilityInfoService>();
            services.Configure<SendAvailabilityInfoBackgroundServiceOptions>(Configuration.GetSection("SendAvailabilityInfoHostedServiceOptions"));
            services.AddHostedService<SendAvailabilityInfoBackgroundService>();
            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var appWorkingMode = Environment.GetEnvironmentVariable("APP_WORKING_MODE");
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (!env.IsProduction())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
