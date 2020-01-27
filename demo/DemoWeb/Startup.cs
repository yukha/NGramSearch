using DemoWeb.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace DemoWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton(new IntersectionCountSearchService("actors"));
            services.AddSingleton(new IntersectionCountSearchService("films"));

            services.AddSingleton(new SorensenDiceCoefficientSearchService("actors"));
            services.AddSingleton(new SorensenDiceCoefficientSearchService("films"));

            services.AddSingleton(new JaccardIndexSearchService("actors"));
            services.AddSingleton(new JaccardIndexSearchService("films"));

            services.AddTransient<Func<string, IntersectionCountSearchService>>(provider => name =>
            {
                var registeredServices = provider.GetServices<IntersectionCountSearchService>();
                return registeredServices.FirstOrDefault(s => s.Name == name);
            });

            services.AddTransient<Func<string, SorensenDiceCoefficientSearchService>>(provider => name =>
            {
                var registeredServices = provider.GetServices<SorensenDiceCoefficientSearchService>();
                return registeredServices.FirstOrDefault(s => s.Name == name);
            });

            services.AddTransient<Func<string, JaccardIndexSearchService>>(provider => name =>
            {
                var registeredServices = provider.GetServices<JaccardIndexSearchService>();
                return registeredServices.FirstOrDefault(s => s.Name == name);
            });


            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                    spa.Options.StartupTimeout = TimeSpan.FromSeconds(120);
                }
            });
        }
    }
}
