using System.Security.Cryptography;
using System.Text;

public static class OtpGenerator_cl
{
    public static string GenerateNumericCode(int length)
    {
        if (length <= 0)
            return "";

        byte[] buffer = new byte[length];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(buffer);
        }

        StringBuilder sb = new StringBuilder(length);
        for (int i = 0; i < buffer.Length; i++)
        {
            sb.Append((buffer[i] % 10).ToString());
        }

        return sb.ToString();
    }
}
