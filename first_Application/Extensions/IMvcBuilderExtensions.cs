using first_Application.Utilities.Formatters;

namespace first_Application.Extensions
{
    public static class IMvcBuilderExtensions
    {
        public static IMvcBuilder AddCustomCsvFormatter(this IMvcBuilder builder) =>
            builder.AddMvcOptions(config => 
            config.OutputFormatters
            .Add(new CsvOutputFormatter()));
        
    }
}
