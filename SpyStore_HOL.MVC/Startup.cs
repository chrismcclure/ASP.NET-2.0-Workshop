﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpyStore_HOL.DAL.EF;
using SpyStore_HOL.DAL.EF.Initialization;
using SpyStore_HOL.DAL.Repos;
using SpyStore_HOL.DAL.Repos.Interfaces;

namespace SpyStore_HOL.MVC
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
            services.AddMvc();
            services.AddDbContextPool<StoreContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SpyStore"), o => o.EnableRetryOnFailure()).ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning))); services.AddScoped<ICategoryRepo, CategoryRepo>();
            services.AddScoped<IProductRepo, ProductRepo>(); services.AddScoped<ICustomerRepo, CustomerRepo>(); services.AddScoped<IShoppingCartRepo, ShoppingCartRepo>(); services.AddScoped<IOrderRepo, OrderRepo>(); services.AddScoped<IOrderDetailRepo, OrderDetailRepo>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                   .CreateScope())
                {
                    StoreDataInitializer.InitializeData(serviceScope.ServiceProvider.GetRequiredService<StoreContext>());
                }
            }
            else
            {
                app.UseExceptionHandler("/Products/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Products}/{action=Index}/{id?}");
            });

        }
    }
}
