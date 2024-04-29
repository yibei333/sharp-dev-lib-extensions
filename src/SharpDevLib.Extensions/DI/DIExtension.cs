using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SharpDevLib.Extensions.DI;

/// <summary>
/// di extension
/// </summary>
public static class DIExtension
{
    /// <summary>
    /// add services by assembly
    /// </summary>
    /// <param name="services">service collection</param>
    /// <param name="assemblies">assemblies to find implemention</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddByAssembly(this IServiceCollection services, params Assembly[] assemblies) => services.AddByAssembly(ServiceLifetime.Scoped, assemblies);

    /// <summary>
    /// add services by assembly
    /// </summary>
    /// <param name="services">service collection</param>
    /// <param name="serviceLifetime">service life time</param>
    /// <param name="assemblies">assemblies to find implemention</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddByAssembly(this IServiceCollection services, ServiceLifetime serviceLifetime, Assembly[] assemblies) => services.AddByAssembly(assemblies, serviceLifetime);

    /// <summary>
    /// add services by assembly
    /// </summary>
    /// <param name="services">service collection</param>
    /// <param name="assembly">assembly to find implemention</param>
    /// <param name="ignoredTypes">ignored types to add</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddByAssembly(this IServiceCollection services, Assembly assembly, params Type[] ignoredTypes) => services.AddByAssembly(assembly, ServiceLifetime.Scoped, ignoredTypes);

    /// <summary>
    /// add services by assembly
    /// </summary>
    /// <param name="services">service collection</param>
    /// <param name="assembly">assembly to find implemention</param>
    /// <param name="lifetime">service life time</param>
    /// <param name="ignoredTypes">ignored types to add</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddByAssembly(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime, params Type[] ignoredTypes) => services.AddByAssembly(new Assembly[] { assembly }, lifetime, ignoredTypes);

    /// <summary>
    /// add services by assembly
    /// </summary>
    /// <param name="services">service collection</param>
    /// <param name="assemblies">assemblies to find implemention</param>
    /// <param name="ignoredTypes">ignored types to add</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddByAssembly(this IServiceCollection services, Assembly[] assemblies, params Type[] ignoredTypes) => services.AddByAssembly(assemblies, ServiceLifetime.Scoped, ignoredTypes);

    /// <summary>
    /// add services by assembly
    /// </summary>
    /// <param name="services">service collection</param>
    /// <param name="assemblies">assemblies to find implemention</param>
    /// <param name="lifetime">service life time</param>
    /// <param name="ignoredTypes">ignored types to add</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddByAssembly(this IServiceCollection services, Assembly[] assemblies, ServiceLifetime lifetime, params Type[] ignoredTypes)
    {
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes().Where(x => !ignoredTypes.Contains(x) && x.IsPublic && !x.IsAbstract && x.GetInterfaces().Any()).ToList();
            types.ForEach(x =>
            {
                services.AddByImplemention(x, lifetime);
            });
        }
        return services;
    }

    /// <summary>
    /// add services by implemention
    /// </summary>
    /// <param name="services">service collection</param>
    /// <param name="lifetime">service life time</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddByImplemention<TImplemention>(this IServiceCollection services, ServiceLifetime lifetime) where TImplemention : class => services.AddByImplemention(typeof(TImplemention), lifetime);

    /// <summary>
    /// add services by implemention
    /// </summary>
    /// <param name="services">service collection</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddByImplemention<TImplemention>(this IServiceCollection services) where TImplemention : class => services.AddByImplemention<TImplemention>(ServiceLifetime.Scoped);

    /// <summary>
    /// add services by implemention
    /// </summary>
    /// <param name="services">service collection</param>
    /// <param name="implementionType">implemention type</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddByImplemention(this IServiceCollection services, Type implementionType) => services.AddByImplemention(implementionType, ServiceLifetime.Scoped);

    /// <summary>
    /// add services by implemention
    /// </summary>
    /// <param name="services">service collection</param>
    /// <param name="lifetime">service life time</param>
    /// <param name="implementionType">implemention type</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddByImplemention(this IServiceCollection services, Type implementionType, ServiceLifetime lifetime)
    {
        var interfaces = implementionType.GetInterfaces().Where(x => x.IsPublic).ToList();
        interfaces.ForEach(y =>
        {
            if (implementionType.IsGenericType && !y.IsGenericType) return;

            //todo:y.FullName is null means interface generic argument is dynamic type?
            var serviceType = y.FullName.IsNull() ? y.GetGenericTypeDefinition() : y;
            services.Add(new ServiceDescriptor(serviceType, implementionType, lifetime));
        });
        return services;
    }

    /// <summary>
    /// add services by assembly
    /// </summary>
    /// <param name="services">service collection</param>
    /// <param name="lifetime">service life time</param>
    /// <param name="assemblies">assemblies to find implement type by interface</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddByInterface<TService>(this IServiceCollection services, ServiceLifetime lifetime, params Assembly[] assemblies) where TService : class => services.AddByInterface(typeof(TService), lifetime, assemblies);

    /// <summary>
    /// add services by assembly
    /// </summary>
    /// <param name="services">service collection</param>
    /// <param name="assemblies">assemblies to find implement type by interface</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddByInterface<TService>(this IServiceCollection services, params Assembly[] assemblies) where TService : class => services.AddByInterface<TService>(ServiceLifetime.Scoped, assemblies);

    /// <summary>
    /// add services by assembly
    /// </summary>
    /// <param name="services">service collection</param>
    /// <param name="type">implemention type</param>
    /// <param name="assemblies">assemblies to find implement type by interface</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddByInterface(this IServiceCollection services, Type type, params Assembly[] assemblies) => services.AddByInterface(type, ServiceLifetime.Scoped, assemblies);

    /// <summary>
    /// add services by assembly
    /// </summary>
    /// <param name="services">service collection</param>
    /// <param name="lifetime">service life time</param>
    /// <param name="interfaceType">implemention type</param>
    /// <param name="assemblies">assemblies to find implement type by interface</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddByInterface(this IServiceCollection services, Type interfaceType, ServiceLifetime lifetime, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes().Where(x => x.IsClass && x.IsPublic && !x.IsAbstract && x.GetInterface(interfaceType.FullName!).NotNull()).ToList();
            types.ForEach(x =>
            {
                services.AddByImplemention(x, lifetime);
            });
        }
        return services;
    }
}
