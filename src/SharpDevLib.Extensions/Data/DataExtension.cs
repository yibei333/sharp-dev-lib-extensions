using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharpDevLib.Extensions.Data.Implemention;
using System.Data.Common;
using System.Reflection;

namespace SharpDevLib.Extensions.Data;

/// <summary>
/// data service di extension
/// </summary>
public static class DataExtension
{
    /// <summary>
    /// add sql helper
    /// </summary>
    /// <param name="services">service collection</param>
    /// <param name="dbProviderFactory">db provider factory</param>
    /// <param name="connectionString">db connection string</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddSqlHelper(this IServiceCollection services, DbProviderFactory dbProviderFactory, string connectionString)
    {
        services.AddSingleton(new SqlHelperOption(dbProviderFactory, connectionString));
        services.AddScoped<ISqlHelper, SqlHelper>();
        return services;
    }

    /// <summary>
    /// add sql helper
    /// </summary>
    /// <param name="services">service collection</param>
    /// <param name="dbProviderFactory">db provider factory</param>
    /// <param name="connectionStringResolver">database connection string resolver,effect when ConnectionString is null or emtpy</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddSqlHelper(this IServiceCollection services, DbProviderFactory dbProviderFactory, ISqlHelperConnectionStringResolver connectionStringResolver)
    {
        services.AddSingleton(new SqlHelperOption(dbProviderFactory, connectionStringResolver));
        services.AddScoped<ISqlHelper, SqlHelper>();
        return services;
    }

    /// <summary>
    /// add data service
    /// (
    /// notice:
    ///     start project data provider and design libaray version should greator than 6.0.0 
    ///     or 
    ///     implmente 'IDesignTimeDbContextFactory'[https://docs.microsoft.com/zh-cn/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli],
    ///     otherwise 
    ///     migration will cause '在创建DbContext的时候无法创建服务DbContextOptions' error
    /// )
    /// </summary>
    /// <typeparam name="TDbContext">dbcontext type</typeparam>
    /// <typeparam name="TMigration">migration type</typeparam>
    /// <param name="services"></param>
    /// <param name="optionsBuilder">dbcontext options builder</param>
    /// <param name="repositoryAssemblies">add repository by assembly,if not set,find repositories by dbcontext assembly</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddData<TDbContext, TMigration>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsBuilder, params Assembly[] repositoryAssemblies) where TDbContext : DbContext, IDbContext where TMigration : DataMigration<TDbContext>
    {
        services.AddData<TDbContext>(optionsBuilder, repositoryAssemblies);
        services.AddScoped<DataMigration, TMigration>();
        services.AddScoped<DataMigration<TDbContext>, TMigration>();
        return services;
    }

    /// <summary>
    /// add data service
    /// (
    /// notice:
    ///     start project data provider and design libaray version should greator than 6.0.0 
    ///     or 
    ///     implmente 'IDesignTimeDbContextFactory'[https://docs.microsoft.com/zh-cn/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli],
    ///     otherwise 
    ///     migration will cause '在创建DbContext的时候无法创建服务DbContextOptions' error
    /// )
    /// </summary>
    /// <typeparam name="TDbContext">dbcontext type</typeparam>
    /// <param name="services"></param>
    /// <param name="optionsBuilder">dbcontext options builder</param>
    /// <param name="repositoryAssemblies">add repository by assembly,if not set,find repositories by dbcontext assembly</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddData<TDbContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsBuilder, params Assembly[] repositoryAssemblies) where TDbContext : DbContext, IDbContext
    {
        services.AddDbContext<TDbContext>(optionsBuilder);
        services.AddScoped<IDbContext, TDbContext>();
        services.AddSingleDbContext<TDbContext>(repositoryAssemblies);
        services.AddMultipleDbContext<TDbContext>(repositoryAssemblies);
        return services;
    }

    private static void AddSingleDbContext<TDbContext>(this IServiceCollection services, params Assembly[] repositoryAssemblies)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddByInterface(typeof(IRepository<>), repositoryAssemblies == null || !repositoryAssemblies.Any() ? new[] { typeof(TDbContext).Assembly } : repositoryAssemblies);
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ISqlRepository, SqlRepository>();
    }

    private static void AddMultipleDbContext<TDbContext>(this IServiceCollection services, params Assembly[] repositoryAssemblies) where TDbContext : DbContext, IDbContext
    {
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddByInterface(typeof(IRepository<,>), repositoryAssemblies == null || !repositoryAssemblies.Any() ? new[] { typeof(TDbContext).Assembly } : repositoryAssemblies);
        services.AddScoped<IUnitOfWork<TDbContext>, UnitOfWork<TDbContext>>();
        services.AddScoped<ISqlRepository<TDbContext>, SqlRepository<TDbContext>>();
    }

    private static IServiceCollection AddByInterface(this IServiceCollection services, Type type, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes().Where(x => x.IsClass && x.IsPublic && !x.IsAbstract && x.GetInterface(type.FullName!) != null).ToList();
            types.ForEach(x =>
            {
                var interfaces = x.GetInterfaces().Where(x => x.IsPublic).ToList();
                interfaces.ForEach(y =>
                {
                    if (x.IsGenericType && !y.IsGenericType) return;

                    //todo:y.FullName is null means interface generic argument is dynamic type?
                    var serviceType = y.FullName == null ? y.GetGenericTypeDefinition() : y;
                    services.Add(new ServiceDescriptor(serviceType, x, ServiceLifetime.Scoped));
                });
            });
        }
        return services;
    }

    internal static void EnsureIsSingleContext(this IServiceProvider serviceProvider)
    {
        var contextList = serviceProvider.GetServices<IDbContext>();
        if (!contextList.Any()) throw new Exception("DbContext not detected,if you have DbContext,please implement interface 'SharpDevLib.Data.IDbContext'");
        if (contextList.Count() == 1) return;
        var contextTypeList = contextList.Select(x => x.GetType().FullName).Distinct().ToList();
        if (contextTypeList.Count == 1) return;
        throw new Exception($"multiple dbcontext detected('[{(string.Join("],[", contextTypeList))}]'),use generic type(IRepository<TEntity,TDbContext>,ISqlRepository<TDbContext>,IUnitOfWork<TDbContext>,DataMigration<TDbContext>,...) instead");
    }

    internal static void EnsureTypeContainsPublicParamterLessConstructor(this Type? type)
    {
        if (type.IsNull()) throw new ArgumentNullException(nameof(type));
        if (type.GetConstructors().All((ConstructorInfo x) => x.GetParameters().Any() && x.IsPublic)) throw new MissingMethodException("type [" + type!.FullName + "] must have a public parameterless constructor");
    }

    /// <summary>
    /// when app start,auto migrate data
    /// </summary>
    /// <typeparam name="TDbContext">dbcontext type</typeparam>
    /// <param name="app">app builder</param>
    /// <returns>app builder</returns>
    public static IApplicationBuilder MigrateData<TDbContext>(this IApplicationBuilder app) where TDbContext : DbContext, IDbContext
    {
        var migration = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<DataMigration<TDbContext>>();
        migration.Migrate();
        return app;
    }

    /// <summary>
    /// when app start,auto migrate data
    /// </summary>
    /// <param name="app">app builder</param>
    /// <returns>app builder</returns>
    public static IApplicationBuilder MigrateData(this IApplicationBuilder app)
    {
        var migration = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<DataMigration>();
        migration.Migrate();
        return app;
    }
}