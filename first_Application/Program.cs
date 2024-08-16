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
    config.RespectBrowserAcceptHeader = true; // Header'daki Accept'e farkl� formatlar� kabul edece�ini s�yler.
    config.ReturnHttpNotAcceptable = true; // E�er ilgili yap�land�rma yoksa 406 d�ner.
})
    .AddCustomCsvFormatter() // CSV formatlay�c�y� ekler.
    .AddXmlDataContractSerializerFormatters() // XML d�n��� yapmam�z� sa�lar.
    .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly); // Belirtilen assembly'i ekler.
   /* .AddNewtonsoftJson();*/ // JSON formatlay�c�y� ekler.
    


builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
var app = builder.Build();

var logger= app.Services.GetRequiredService<ILoggerService>();
app.ConfigureExceptionHandler(logger);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
