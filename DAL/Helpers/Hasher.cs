using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace DAL.Helpers;

public static class Hasher
{
    public static string HashString(string str)
    {
        byte[] salt = Encoding.ASCII.GetBytes("ScrumPoll");

        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: str,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

        return hashed;
    }
}
