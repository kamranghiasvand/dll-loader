using System;
using DllService.service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DllService.Common;
using DllService.app.Validators;
using Microsoft.OpenApi.Models;
using DllService.app.Model.Binder;

namespace DllService
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
            services.AddMvc(options=>{
            options.ModelBinderProviders.Insert(0,new TypeLoadEntityBinderProvider());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<IDllContext, DllContext>();
            services.AddSingleton<TypeLoadValidator>();
            services.AddSingleton<DllValidator>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dll Loader Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dll Loader Api V1");
                c.RoutePrefix = string.Empty;
            });
            app.UseCommonExceptionHandler();
            app.UseHttpsRedirection();
            app.UseMvc();
            var context = (IDllContext)serviceProvider.GetService(typeof(IDllContext));
           
            var typeValidator= (TypeLoadValidator)serviceProvider.GetService(typeof(TypeLoadValidator));
            var dllValidator = (DllValidator)serviceProvider.GetService(typeof(DllValidator));
            context.Init();
            typeValidator.init();
            dllValidator.init();
        }
    }
}
