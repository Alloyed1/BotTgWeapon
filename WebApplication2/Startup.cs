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
using Hangfire.SqlServer;

namespace WebApplication2
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
            
            services.AddDbContext<ApplicationContext>(options =>
                options.UseNpgsql("Host=45.144.64.224;Port=5432;Database=database;Username=postgres;Password=cw42puQAZ"));
            //services.AddHangfire(configuration => configuration
            //    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            //    .UseSimpleAssemblyNameTypeSerializer()
            //    .UseRecommendedSerializerSettings()
            //    .UseSqlServerStorage("Data Source=wpl33.hosting.reg.ru;Initial Catalog=u0865575_dbdb;User Id=u0865575_userDb;Password=J59&zx9i;", new SqlServerStorageOptions
            //    {
            //        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            //        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            //        QueuePollInterval = TimeSpan.Zero,
            //        UseRecommendedIsolationLevel = true,
            //        UsePageLocksOnDequeue = true,
            //        DisableGlobalLocks = true
            //    }));

            //services.AddHangfireServer();


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
            //app.UseHangfireDashboard();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            


            //RecurringJob.AddOrUpdate(

            //    () => HangfireTasks.Test(),
            //    Cron.MinuteInterval(2));


            //RecurringJob.AddOrUpdate(

            //() => VkBoy.CheckNewWeapon(),
            //Cron.MinuteInterval(11));


            //Bot Configurations
            Bot.GetBotClientAsync().GetAwaiter().GetResult();
        }
    }
}