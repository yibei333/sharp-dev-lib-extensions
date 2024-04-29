using SharpDevLib.Extensions.Encryption.Implemention.RsaImp;

namespace SharpDevLib.Extensions.Encryption;

internal class AsymmetricEncryptionImp : IAsymmetricEncryption
{
    private readonly IRsaKey _rsaKey;

    public AsymmetricEncryptionImp(IRsaKey rsaKey)
    {
        _rsaKey = rsaKey;
    }

    public IRsaEncryptionTransform Rsa => new RsaTransform(_rsaKey);
}