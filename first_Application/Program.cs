using AspNetCoreRateLimit;
using first_Application.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NLog;
using Presentation.ActionFilters;
using Repositories.EFCore;
using Services;
using Services.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(),"/nlog.config"));

builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true; // Header'daki Accept'e farklý formatlarý kabul edeceðini söyler.
    config.ReturnHttpNotAcceptable = true;
config.CacheProfiles.Add("5mins", new CacheProfile() {Duration=300});// Eðer ilgili yapýlandýrma yoksa 406 döner.
})
    .AddCustomCsvFormatter() // CSV formatlayýcýyý ekler.
    .AddXmlDataContractSerializerFormatters() // XML dönüþü yapmamýzý saðlar.
    .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly) // Belirtilen assembly'i ekler.
    .AddNewtonsoftJson(opt=>
    opt.SerializerSettings.ReferenceLoopHandling=Newtonsoft.Json.ReferenceLoopHandling.Ignore); // JSON formatlayýcýyý ekler.



builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureLoggerManager();
builder.Services.ConfigureActionFilters();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureCors();
builder.Services.ConfigureDataShaper();
builder.Services.AddCustomMediaTypes();
builder.Services.AddScoped<IProductLinks, ProductLinks>();
builder.Services.ConfigureVersioning();
builder.Services.ConfigureResponseCache();
builder.Services.ConfigureHttpCacheHeader();
builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimitOptions();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpContextAccessor();


builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);


var app = builder.Build();

var logger= app.Services.GetRequiredService<ILoggerService>();
app.ConfigureExceptionHandler(logger);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(s =>
    {
        s.SwaggerEndpoint("/swagger/v1/swagger.json","Products V1");
        s.SwaggerEndpoint("/swagger/v2/swagger.json","Products V2");
    });
}
if (app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseIpRateLimiting();
app.UseCors("CorsPolicy");
app.UseResponseCaching();
app.UseHttpCacheHeaders();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
