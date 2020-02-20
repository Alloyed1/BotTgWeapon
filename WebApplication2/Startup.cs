using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApplication2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hangfire.PostgreSql;

namespace WebApplication2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            StaticConfig = configuration;
        }

        public static IConfiguration StaticConfig { get; private set; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //services.AddDbContext<ApplicationContext>(options =>
            //    options.UseNpgsql("Host=45.144.64.224;Port=5432;Database=database;Username=postgres;Password=cw42puQAZ"));

            services.AddHangfire(config =>
                 config.UsePostgreSqlStorage("Host=45.144.64.224;Port=5432;Database=database;Username=postgres;Password=cw42puQAZ"));


            services.AddMemoryCache();
            services.AddControllers().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [Obsolete]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); 
            }

            app.UseRouting();
            var documentOptions = new BackgroundJobServerOptions
            {
                WorkerCount = 30,
            };

            app.UseHangfireServer(documentOptions);
            app.UseHangfireDashboard();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });


            RecurringJob.AddOrUpdate(

             () => HangfireTasks.ParseAllAlbumsVkAsync(),
            Cron.MinuteInterval(4));

            RecurringJob.AddOrUpdate(

            () => HangfireTasks.ParseComment(),
                Cron.MinuteInterval(1));

            RecurringJob.AddOrUpdate(

            () => HangfireTasks.Notify(),
            Cron.MinuteInterval(2));



            //Bot Configurations
            Bot.GetBotClientAsync().GetAwaiter().GetResult();
        }
    }
}