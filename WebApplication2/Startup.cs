using System;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplication2.Models;
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
                WorkerCount = 10,
                SchedulePollingInterval = TimeSpan.FromMilliseconds(15000)
            };

            app.UseHangfireServer(documentOptions);
            app.UseHangfireDashboard();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            //RecurringJob.AddOrUpdate(

            // () => HangfireTasks.ParseAllAlbumsVkAsync(),
            //Cron.MinuteInterval(6));

            //RecurringJob.AddOrUpdate(

            //() => HangfireTasks.ParseComment(),
            //    "*/20 * * * * *");

            //RecurringJob.AddOrUpdate(

            //() => HangfireTasks.Notify(),
            //Cron.MinuteInterval(2));

            //RecurringJob.AddOrUpdate(

            //() => HangfireTasks.ParseKidals(),
            //Cron.MinuteInterval(30));

            //RecurringJob.AddOrUpdate(

            //() => HangfireTasks.ParseKidalId(),
            //Cron.MinuteInterval(5));

            RecurringJob.AddOrUpdate(

            () => HangfireTasks.ParseAllTopicVkAsync(),
            Cron.MinuteInterval(1));

            //Bot Configurations
            //Bot.GetBotClientAsync().GetAwaiter().GetResult();
        }
    }
}