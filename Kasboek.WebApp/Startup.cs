﻿using AutoMapper;
using Kasboek.WebApp.Data;
using Kasboek.WebApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace Kasboek.WebApp
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
            services.AddDbContext<KasboekDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("KasboekDatabase")));

            services.AddAutoMapper();
            services.AddMvc(options => options.MaxModelValidationErrors = 2000);

            services.AddScoped<IInstellingenService, InstellingenService>();
            services.AddScoped<ICategorieenService, CategorieenService>();
            services.AddScoped<IRekeningenService, RekeningenService>();
            services.AddScoped<ITransactiesService, TransactiesService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var nederlands = new CultureInfo("nl-NL");
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(nederlands),
                SupportedCultures = new[] { nederlands },
                SupportedUICultures = new[] { nederlands }
            });

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
