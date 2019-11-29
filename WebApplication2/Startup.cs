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
            
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfireServer();
            
            services.AddControllers().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseHangfireDashboard();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            
            RecurringJob.AddOrUpdate(
                () => VkBoy.GetWeaponList(),
                Cron.MinuteInterval(30));
            
            RecurringJob.AddOrUpdate(
	            
                () => VkBoy.GetWeaponListComments(),
                Cron.MinuteInterval(55));

            RecurringJob.AddOrUpdate(
	            
                () => VkBoy.CheckNewWeapon(),
                Cron.MinuteInterval(30));
            
            //RecurringJob.AddOrUpdate(
	            
                //() => VkBoy.UpdateSite(),
                //Cron.MinuteInterval(10));

            
            //Bot Configurations
           Bot.GetBotClientAsync().Wait();
        }
    }
}