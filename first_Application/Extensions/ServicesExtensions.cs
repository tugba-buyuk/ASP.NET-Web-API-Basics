using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Presentation.ActionFilters;
using Presentation.Controllers;
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

        public static void ConfigureRepositoryManager(this IServiceCollection services) =>
            services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureServiceManager(this IServiceCollection services) =>
            services.AddScoped<IServiceManager, ServiceManager>();

        public static void ConfigureLoggerManager(this IServiceCollection services) =>
            services.AddSingleton<ILoggerService, LoggerManager>();

        public static void ConfigureActionFilters(this IServiceCollection services)
        {
            services.AddScoped<ValidationFilterAttribute>();
            services.AddSingleton<LogFilterAttribute>();
            services.AddScoped<ValidateMediaTypeAttribute>();
        }

        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders("X-Pagination"));
            });
        }
        public static void ConfigureDataShaper(this IServiceCollection services) =>
            services.AddScoped<IDataShaper<ProductDTO>, DataShaper<ProductDTO>>();

        public static void AddCustomMediaTypes(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(config =>
            {
                var systemJsonOutputFormatter = config.OutputFormatters.OfType<SystemTextJsonOutputFormatter>().FirstOrDefault();
                if (systemJsonOutputFormatter != null)
                {
                    systemJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.tb.hateoas+json");
                    systemJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.tb.apiroot+json");
                }

                var xmlOutputFormatter = config.OutputFormatters.OfType<XmlDataContractSerializerOutputFormatter>().FirstOrDefault();
                if (xmlOutputFormatter != null)
                {
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.tb.hateoas+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.tb.apiroot+xml");
                }
            });
        }

        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
                opt.Conventions.Controller<ProductsController>().HasApiVersion(new ApiVersion(1,0));
                opt.Conventions.Controller<ProductV2Controller>().HasDeprecatedApiVersion(new ApiVersion(2,0));
            });
        }

    }
}
