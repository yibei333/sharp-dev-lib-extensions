using Microsoft.Extensions.DependencyInjection;
using SharpDevLib.Extensions.Encryption.Implemention.RsaKeyImp;

namespace SharpDevLib.Extensions.Encryption;

/// <summary>
/// encryption service di register extension
/// </summary>
public static class EncryptionExtension
{
    /// <summary>
    /// add encryption service
    /// </summary>
    /// <param name="services">service collection</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddEncryption(this IServiceCollection services)
    {
        services.AddScoped<IEncryption, EncryptionImp>();
        services.AddScoped<IRsaKey, RsaKey>();
        services.AddScoped<ISymmetricEncryption, SymmetricEncryptionImp>();
        services.AddScoped<IAsymmetricEncryption, AsymmetricEncryptionImp>();
        return services;
    }
}
