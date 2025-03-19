using App.Business.DTOs.Commons;
using App.Business.Helpers;
using App.Business.Services;
using App.Business.Services.ExternalServices.Abstractions;
using App.Business.Services.ExternalServices.Interfaces;
using App.Business.Services.InternalServices.Abstractions;
using App.Business.Services.InternalServices.Interfaces;
using App.Business.Validators.Commons;
using App.DAL.Repositories.Implementations;
using App.DAL.Repositories.Interfaces;
using App.Shared.Implementations;
using App.Shared.Interfaces;
using AspNetCoreRateLimit;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace App.Business
{
    public static class BusinessDependencyInjection
    {
        public static IServiceCollection AddBusiness(this IServiceCollection services)
        {
            services.AddServices();
            services.RegisterAutoMapper(); 
            services.Configure<IpRateLimitOptions>(options =>
            {
                options.GeneralRules = new List<RateLimitRule>
                {
                    new RateLimitRule
                    {
                        Endpoint = "*",
                        Limit = 100, 
                        Period = "10s" 
                    }
                };
            });
            services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            services.AddControllers(options =>
            {
                options.Conventions.Add(new PluralizedRouteConvention());
                options.ModelValidatorProviders.Clear();
            })
           .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<BaseEntityValidator<BaseEntityDTO>>())
           .AddJsonOptions(options =>
           {
               options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
           });

            return services;
        }

        private static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IClaimService, ClaimService>();

            var internalServices = new Dictionary<Type, Type>
            {
                { typeof(IGraduateService), typeof(GraduateService) },
                { typeof(IAccountService), typeof(AccountService) },

            };

            foreach (var (interfaceType, implementationType) in internalServices)
            {
                services.AddScoped(interfaceType, implementationType);
            }
            // External Services 
            services.AddScoped<IFileManagerService, FileManagerService>();
        }

        private static void RegisterAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(BusinessDependencyInjection));
        }
    }
}
