﻿using Microsoft.EntityFrameworkCore;
using Presentation.ActionFilters;
using Repositories.Contracts;
using Repositories.EFCore;
using Services;
using Services.Contracts;

namespace first_Application.Extensions
{
    public static class ServicesExtensions
    {
        public static void ConfigureSqlContext(this IServiceCollection services,
            IConfiguration configuration) =>
            services.AddDbContext<RepositoryContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("sqlConnection")));

        public static void ConfigureRepositoryManager(this IServiceCollection services)=>
            services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureServiceManager(this IServiceCollection services) =>
            services.AddScoped<IServiceManager, ServiceManager>();

        public static void ConfigureLoggerManager(this IServiceCollection services) =>
            services.AddSingleton<ILoggerService, LoggerManager>();

        public static void ConfigureActionFilters(this IServiceCollection services)
        {
            services.AddScoped<ValidationFilterAttribute>();
            services.AddSingleton<LogFilterAttribute>();
        }
    }
}
