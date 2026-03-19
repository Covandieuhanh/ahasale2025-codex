using System;
using System.Security.Cryptography;

public static class PinSecurity_cl
{
    private const string Prefix = "PBKDF2$";
    private const int Iterations = 120000;
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int MinPinLength = 4;
    private const int MaxPinLength = 4;

    public static bool IsValidPinFormat(string pin)
    {
        if (string.IsNullOrWhiteSpace(pin)) return false;
        pin = pin.Trim();
        if (pin.Length < MinPinLength || pin.Length > MaxPinLength) return false;

        for (int i = 0; i < pin.Length; i++)
        {
            if (!char.IsDigit(pin[i])) return false;
        }

        return true;
    }

    public static string HashPin(string pin)
    {
        if (!IsValidPinFormat(pin))
            throw new ArgumentException("PIN không hợp lệ.", "pin");

        pin = pin.Trim();
        byte[] salt = new byte[SaltSize];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        byte[] hash;
        using (var pbkdf2 = new Rfc2898DeriveBytes(pin, salt, Iterations))
        {
            hash = pbkdf2.GetBytes(HashSize);
        }

        return Prefix + Iterations + "$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(hash);
    }

    public static bool VerifyPin(string storedValue, string inputPin)
    {
        if (string.IsNullOrWhiteSpace(storedValue) || string.IsNullOrWhiteSpace(inputPin))
            return false;

        inputPin = inputPin.Trim();
        if (!IsValidPinFormat(inputPin)) return false;

        storedValue = storedValue.Trim();

        if (storedValue.StartsWith(Prefix, StringComparison.Ordinal))
        {
            return VerifyHashedPin(storedValue, inputPin);
        }

        // Tương thích dữ liệu cũ (plaintext)
        return string.Equals(storedValue, inputPin, StringComparison.Ordinal);
    }

    public static bool VerifyAndUpgrade(taikhoan_tb account, string inputPin)
    {
        if (account == null) return false;

        string storedValue = (account.mapin_thanhtoan ?? string.Empty).Trim();
        if (!VerifyPin(storedValue, inputPin)) return false;

        if (!storedValue.StartsWith(Prefix, StringComparison.Ordinal))
        {
            account.mapin_thanhtoan = HashPin(inputPin.Trim());
        }

        return true;
    }

    public static string GenerateRandomNumericPin(int length)
    {
        if (length < MinPinLength || length > MaxPinLength)
            throw new ArgumentOutOfRangeException("length", "Độ dài PIN phải đúng 4 chữ số.");

        byte[] random = new byte[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(random);
        }

        char[] digits = new char[length];
        bool allZero = true;

        for (int i = 0; i < length; i++)
        {
            int d = random[i] % 10;
            digits[i] = (char)('0' + d);
            if (d != 0) allZero = false;
        }

        if (allZero && length > 0)
        {
            digits[0] = '1';
        }

        return new string(digits);
    }

    private static bool VerifyHashedPin(string storedValue, string inputPin)
    {
        // Format: PBKDF2$iterations$salt$hash
        string[] parts = storedValue.Split('$');
        if (parts.Length != 4) return false;

        int iterations;
        if (!int.TryParse(parts[1], out iterations) || iterations < 1000)
            return false;

        byte[] salt;
        byte[] expectedHash;

        try
        {
            salt = Convert.FromBase64String(parts[2]);
            expectedHash = Convert.FromBase64String(parts[3]);
        }
        catch
        {
            return false;
        }

        if (salt.Length < 8 || expectedHash.Length < 16) return false;

        byte[] actualHash;
        using (var pbkdf2 = new Rfc2898DeriveBytes(inputPin, salt, iterations))
        {
            actualHash = pbkdf2.GetBytes(expectedHash.Length);
        }

        return FixedTimeEquals(expectedHash, actualHash);
    }

    private static bool FixedTimeEquals(byte[] a, byte[] b)
    {
        if (a == null || b == null || a.Length != b.Length) return false;

        int diff = 0;
        for (int i = 0; i < a.Length; i++)
        {
            diff |= a[i] ^ b[i];
        }

        return diff == 0;
    }
}
