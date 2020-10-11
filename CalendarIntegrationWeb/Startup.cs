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
using CalendarIntegrationCore.Services.DataProcessing;
using CalendarIntegrationCore.Services.DataRetrieving;
using CalendarIntegrationCore.Services.DataSaving;
using CalendarIntegrationCore.Services.InitializationHandlers;
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
            
            services.AddCors(o => o.AddPolicy("DefaultCorsPolicy", builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins("http://localhost:4200");
            }));
            
            services.Configure<DateSynchronizationCommonOptions>(Configuration.GetSection("DateSynchronizationCommonOptions"));
            services.Configure<DownloadAvailabilityInfoBackgroundServiceOptions>(Configuration.GetSection("DownloadAvailabilityInfoBackgroundServiceOptions"));
            services.Configure<UploadAvailabilityInfoBackgroundServiceOptions>(Configuration.GetSection("UploadAvailabilityInfoBackgroundServiceOptions"));
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
            
            app.UseCors("DefaultCorsPolicy");
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
