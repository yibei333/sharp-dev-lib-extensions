using SharpDevLib.Extensions.Encryption.Implemention.AesImp;
using SharpDevLib.Extensions.Encryption.Implemention.DesImp;
using SharpDevLib.Extensions.Encryption.Implemention.TripleDesImp;

namespace SharpDevLib.Extensions.Encryption;

internal class SymmetricEncryptionImp : ISymmetricEncryption
{
    IEncryptionTransform<AesEncryptOption, AesDecryptOption> ISymmetricEncryption.Aes => new AesTransform();
    IEncryptionTransform<DesEncryptOption, DesDecryptOption> ISymmetricEncryption.Des => new DesTransform();
    IEncryptionTransform<TripleDesEncryptOption, TripleDesDecryptOption> ISymmetricEncryption.TripleDes => new TripleDesTransform();
}