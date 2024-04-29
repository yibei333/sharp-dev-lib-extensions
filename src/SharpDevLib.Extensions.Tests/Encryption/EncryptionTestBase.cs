using Microsoft.Extensions.DependencyInjection;
using SharpDevLib.Extensions.Encryption;

namespace SharpDevLib.Extensions.Tests.Encryption;

public abstract class EncryptionTestBase
{
    protected readonly IEncryption _encryption;
    protected readonly IRsaKey _rsaKey;
    public EncryptionTestBase()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddEncryption();
        var provider = services.BuildServiceProvider();
        _encryption = provider.GetRequiredService<IEncryption>();
        _rsaKey = provider.GetRequiredService<IRsaKey>();
    }
}
