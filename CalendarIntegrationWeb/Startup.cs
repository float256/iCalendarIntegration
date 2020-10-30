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
using System.Collections.Generic;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.DataProcessing;
using CalendarIntegrationCore.Services.DataRetrieving;
using CalendarIntegrationCore.Services.DataSaving;
using CalendarIntegrationCore.Services.InitializationHandlers;
using CalendarIntegrationCore.Services.Observers;
using CalendarIntegrationCore.Services.StatusSaving;
using CalendarIntegrationWeb.Hubs;
using Microsoft.Extensions.Logging;
using CalendarIntegrationWeb.Services;
using CalendarIntegrationWeb.Services.BackgroundServices;
using CalendarIntegrationWeb.Services.DataUploading;
using TLConnect;

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
            services.AddScoped<IAvailabilityStatusMessageRepository, AvailabilityStatusMessageRepository>();
            
            services.AddScoped<ICalendarParser, CalendarParser>();
            services.AddScoped<ISoapRequestCreator, SoapRequestCreator>();
            services.AddScoped<IAvailabilityStatusMessageQueue, AvailabilityStatusMessageQueue>();
            services.AddScoped<ITodayBoundary, TodayBoundary>();
            services.AddScoped<IRoomUploadingStatusSaver, RoomUploadingStatusSaver>();
            
            services.AddScoped<IAvailabilityInfoReceiver, AvailabilityInfoReceiver>();
            services.AddScoped<IBookingInfoSaver, BookingInfoSaver>();
            services.AddScoped<IAvailabilityInfoSender, AvailabilityInfoSender>();
            services.AddScoped<IBookingInfoDataProcessor, BookingInfoDataProcessor>();
            services.AddScoped<IAvailabilityMessageConverter, AvailabilityMessageConverter>();
            services.AddScoped<IAvailabilityInfoSynchronizer, AvailabilityInfoSynchronizer>();
            services.AddScoped<IRoomUploadStatusRepository, RoomUploadStatusRepository>(); 
            services.AddScoped<IRoomAvailabilityInitializationHandler, RoomAvailabilityInitializationHandler>();
            
            services.AddScoped<ITLConnectService, TLConnectServiceClient>();
            services.AddHostedService<DownloadAvailabilityInfoBackgroundService>();
            services.AddHostedService<UploadAvailabilityInfoBackgroundService>();

            services.AddScoped<IRoomUploadStatusObserver, RoomUploadStatusObserver>();
            services.AddScoped(serviceProvider  => new List<IObserver<RoomUploadStatus>>
            {
                serviceProvider.GetService<IRoomUploadStatusObserver>()
            });

            services.Configure<DateSynchronizationCommonOptions>(Configuration.GetSection("DateSynchronizationCommonOptions"));
            services.Configure<DownloadAvailabilityInfoBackgroundServiceOptions>(Configuration.GetSection("DownloadAvailabilityInfoBackgroundServiceOptions"));
            services.Configure<UploadAvailabilityInfoBackgroundServiceOptions>(Configuration.GetSection("UploadAvailabilityInfoBackgroundServiceOptions"));
            
            services.AddScoped<ITLConnectService, TLConnectServiceClient>(
                sp => new TLConnectServiceClient(
                    TLConnectServiceClient.EndpointConfiguration.BasicHttpBinding_ITLConnectService,
                    Configuration.GetSection("TLConnectServiceURL").Value));
            services.AddHttpClient();

            services.AddSignalR();
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
            app.UseWebSockets();
            app.UseCors(builder =>
            {
                builder.WithOrigins("https://localhost:4200")
                    .AllowAnyHeader()
                    .WithMethods("GET", "POST")
                    .AllowCredentials();
            });
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<RoomUploadStatusHub>("/RoomUploadStatus");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (!env.IsProduction())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
