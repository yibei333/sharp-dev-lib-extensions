using Org.BouncyCastle.OpenSsl;

namespace SharpDevLib.Extensions.Encryption.Implemention.RsaKeyImp;

internal class RsaPassword : IPasswordFinder
{
    private readonly char[] _password;

    public RsaPassword(char[] word)
    {
        _password = word.DeepClone();
    }

    public char[] GetPassword() => _password.DeepClone();
}
