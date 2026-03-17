using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

/// <summary>
/// Summary description for encode_class
/// </summary>
public class encode_class
{
    public static byte[] encode_md5_data(string data)
    {
        System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashedBytes;
        System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
        hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(data));
        return hashedBytes;
    }
    public static string encode_md5(string _str)
    {
        return (BitConverter.ToString(encode_md5_data(_str)).Replace("-", ""));
    }
    public static string encode_sha1(string _str)
    {
        SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
        byte[] bs = System.Text.Encoding.UTF8.GetBytes(_str);
        bs = sha1.ComputeHash(bs);
        System.Text.StringBuilder s = new System.Text.StringBuilder();
        foreach (byte b in bs)
        {
            s.Append(b.ToString("x1"));
        }
        _str = s.ToString();
        return _str;
    }

    public static string encrypt(string _str)
    {
        string _pass = "Ng0Qu4ngB0n141!@#$";
        byte[] Results;
        System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
        // Buoc 1: Bam chuoi su dung MD5
        MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
        byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(_pass));
        // Step 2. Tao doi tuong TripleDESCryptoServiceProvider moi
        TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
        // Step 3. Cai dat bo ma hoa
        TDESAlgorithm.Key = TDESKey;
        TDESAlgorithm.Mode = CipherMode.ECB;
        TDESAlgorithm.Padding = PaddingMode.PKCS7;
        // Step 4. Convert chuoi (_str) thanh dang byte[]
        byte[] DataToEncrypt = UTF8.GetBytes(_str);
        // Step 5. Ma hoa chuoi
        try
        {
            ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
            Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
        }
        finally
        {
            // Xoa moi thong tin ve Triple DES va HashProvider de dam bao an toan
            TDESAlgorithm.Clear();
            HashProvider.Clear();
        }
        // Step 6. Tra ve chuoi da ma hoa bang thuat toan Base64
        return Convert.ToBase64String(Results);
    }
    public static string decrypt(string _str)
    {
        string _pass = "Ng0Qu4ngB0n141!@#$";
        byte[] Results;
        System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
        // Step 1. Bam chuoi su dung MD5
        MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
        byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(_pass));
        // Step 2. Tao doi tuong TripleDESCryptoServiceProvider moi
        TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
        // Step 3. Cai dat bo giai ma
        TDESAlgorithm.Key = TDESKey;
        TDESAlgorithm.Mode = CipherMode.ECB;
        TDESAlgorithm.Padding = PaddingMode.PKCS7;
        // Step 4. Convert chuoi (_str) thanh dang byte[]
        byte[] DataToDecrypt = Convert.FromBase64String(_str);
        // Step 5. Bat dau giai ma chuoi
        try
        {
            ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
            Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
        }
        finally
        {
            // Xoa moi thong tin ve Triple DES va HashProvider de dam bao an toan
            TDESAlgorithm.Clear();
            HashProvider.Clear();
        }
        // Step 6. Tra ve ket qua bang dinh dang UTF8
        return UTF8.GetString(Results);
    }
    //string Msg = "Day La Chuoi Can Ma Hoa";
    //string Password = "A123";
    //string EncryptedString = class_mahoa.mahoa_comatkhau(Msg, Password);
    //string DecryptedString = class_mahoa.giaima_comatkhau(EncryptedString, Password);
}