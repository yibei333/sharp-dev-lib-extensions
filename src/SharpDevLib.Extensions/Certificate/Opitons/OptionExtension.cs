using System.Security.Cryptography.X509Certificates;

namespace SharpDevLib.Extensions.Certificate.Opitons;

internal static class OptionExtension
{
    public static X500DistinguishedName CreateX500DistinguishedName(this SubjectOption option)
    {
        if (option.IsNull()) throw new ArgumentNullException(nameof(option));
        var collection = new List<string>();
        if (option.CommonName.NotEmpty()) collection.Add($"CN = {option.CommonName}");
        if (option.Country.NotEmpty()) collection.Add($"C = {option.Country}");
        if (option.Province.NotEmpty()) collection.Add($"ST = {option.Province}");
        if (option.City.NotEmpty()) collection.Add($"L = {option.City}");
        if (option.Organization.NotEmpty()) collection.Add($"O = {option.Organization}");
        if (option.OrganizationalUnit.NotEmpty()) collection.Add($"OU = {option.OrganizationalUnit}");
        if (collection.IsEmpty()) throw new Exception($"subject info can not be empty");
        return new X500DistinguishedName(string.Join(",", collection));
    }
}
