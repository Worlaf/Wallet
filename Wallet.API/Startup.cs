using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using Wallet.API.App;
using Wallet.Data.Contexts;
using Wallet.Data.Infrastructure;
using Wallet.Data.Repositories;
using Wallet.Services.Handlers;
using Wallet.Services.Providers;

namespace Wallet.API
{
    public class Startup
    {
        private const string SwaggerApiDocumentationKey = "WalletApi";
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMediatR(typeof(CreateWalletHandler).Assembly);
            services.AddSingleton(Configuration);
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<WalletDbContext, WalletDbContext>();

            services.AddScoped<DbContextOptions>(sp =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<WalletDbContext>();
                optionsBuilder.UseSqlServer(sp.GetService<IConfiguration>().GetConnectionString(WalletDbContext.ConnectionStringName));

                return optionsBuilder.Options;
            });

            var repositoryTypes = typeof(WalletRepository).Assembly.GetTypes().Where(t => !t.IsAbstract && t.IsClass && t.Name.EndsWith("Repository"));
            foreach (var repositoryType in repositoryTypes)
            {
                RegisterScopedForImplementedInterfaces(repositoryType);
            }

            var serviceTypes = typeof(ConversionRateProvider).Assembly.GetTypes().Where(t => !t.IsAbstract && t.IsClass && t.Name.EndsWith("Provider"));
            foreach (var serviceType in serviceTypes)
            {
                RegisterScopedForImplementedInterfaces(serviceType);
            }

            void RegisterScopedForImplementedInterfaces(Type type)
            {
                foreach (var implementedInterface in type.GetInterfaces())
                {
                    services.AddScoped(implementedInterface, type);
                }
            }

            services.AddHttpClient();

            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc(SwaggerApiDocumentationKey, new OpenApiInfo {Title = "Wallet API", Version = "v1"});

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                o.IncludeXmlComments(xmlPath);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, WalletDbContext walletDbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                var extenderDbConnectionString = Configuration.GetConnectionString(WalletDbContext.ConnectionStringName);
                if (!SqlServerDbUtil.DatabaseExists(extenderDbConnectionString))
                {
                    SqlServerDbUtil.CreateDatabase(extenderDbConnectionString, collation: "SQL_Latin1_General_CP1_CI_AI");

                    walletDbContext.Database.EnsureCreated();
                }
            }

            app.UseHttpsRedirection();

            app.UseCustomExceptionHandler();

            app.UseRouting();

            if (!env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(o =>
                {
                    o.SwaggerEndpoint($"/swagger/{SwaggerApiDocumentationKey}/swagger.json", "Wallet API v1");
                    o.DocExpansion(DocExpansion.None);
                });
            }

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}