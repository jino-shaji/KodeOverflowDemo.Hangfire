using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KodeOverflowDemo.Hangfire
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

            // Hangfire DB and other Configuration
            services.AddHangfire(
               x => x.UseSqlServerStorage(Configuration.GetConnectionString("KodeOverflowDemo_Entities"))
           );
            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            // Configuring Dashboard
            //app.UseHangfireDashboard("/KodeOverflowHangFireJobs");

            app.UseHangfireDashboard("/KodeOverflowHangFireJobs", new DashboardOptions
            {
                Authorization = new[] { new HangFireAuthorizationFilter() },
                IsReadOnlyFunc = (DashboardContext context) => true
            });

            BackgroundJob.Enqueue(() => Console.WriteLine("Greetings from Hangfire Demo By KodeOverflow!"));
            
            RecurringJob.AddOrUpdate(() => DoeSomeReccuringTask(), Cron.Daily);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        public Task DoeSomeReccuringTask()
        {
            Console.WriteLine("Reccuring task in every hour");
            return null;
        }
    }
}
