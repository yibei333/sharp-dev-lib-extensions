namespace SharpDevLib.Extensions.Encryption;

internal class EncryptionImp : IEncryption
{
    public EncryptionImp(ISymmetricEncryption symmetricEncryption, IAsymmetricEncryption asymmetricEncryption)
    {
        Symmetric = symmetricEncryption;
        Asymmetric = asymmetricEncryption;
    }

    public ISymmetricEncryption Symmetric { get; }

    public IAsymmetricEncryption Asymmetric { get; }
}
