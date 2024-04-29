using Microsoft.Extensions.DependencyInjection;

namespace SharpDevLib.Extensions.Excel;

/// <summary>
/// excel service di extension
/// </summary>
public static class ExcelExtension
{
    /// <summary>
    /// register excel service to service collection
    /// </summary>
    /// <param name="services">service collection</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddExcel(this IServiceCollection services)
    {
        services.AddScoped<IExcelService, ExcelService>();
        return services;
    }
}
