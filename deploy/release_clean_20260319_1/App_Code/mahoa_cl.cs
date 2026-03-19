using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;


public class mahoa_cl
{
    #region hàm mã hóa và giải mã của Bcorn
    //private static readonly byte[] EncryptionKey = Encoding.UTF8.GetBytes("YourEncryptionKey"); // Khóa mã hóa, hãy đảm bảo nó là một giá trị bí mật và an toàn

    private static readonly byte[] EncryptionKey = new byte[]// Khóa mã hóa, hãy đảm bảo nó là một giá trị bí mật và an toàn
{
    0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10,0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10
};

    public static string mahoa_Bcorn(string plainText)
    {
        if (plainText == "")
            return "";
        else
        {
            byte[] encryptedBytes;
            using (Aes aes = Aes.Create())
            {
                aes.Key = EncryptionKey;
                aes.Mode = CipherMode.CBC;
                // Tạo một vector khởi tạo ngẫu nhiên
                aes.GenerateIV();
                byte[] iv = aes.IV;
                // Mã hóa dữ liệu
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, iv);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        encryptedBytes = ms.ToArray();
                    }
                }
                // Kết hợp vector khởi tạo và dữ liệu mã hóa thành một chuỗi Base64
                byte[] combinedBytes = new byte[iv.Length + encryptedBytes.Length];
                Array.Copy(iv, 0, combinedBytes, 0, iv.Length);
                Array.Copy(encryptedBytes, 0, combinedBytes, iv.Length, encryptedBytes.Length);
                return Convert.ToBase64String(combinedBytes);
            }
        }
    }
    public static string giaima_Bcorn(string encryptedText)
    {
        try
        {
            if (encryptedText == "")
                return "";
            else
            {
                byte[] combinedBytes = Convert.FromBase64String(encryptedText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = EncryptionKey;
                    aes.Mode = CipherMode.CBC;

                    // Tách vector khởi tạo và dữ liệu mã hóa từ chuỗi Base64
                    byte[] iv = new byte[aes.BlockSize / 8];
                    byte[] encryptedBytes = new byte[combinedBytes.Length - iv.Length];
                    Array.Copy(combinedBytes, 0, iv, 0, iv.Length);
                    Array.Copy(combinedBytes, iv.Length, encryptedBytes, 0, encryptedBytes.Length);

                    // Giải mã dữ liệu
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, iv);
                    using (MemoryStream ms = new MemoryStream(encryptedBytes))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {

                                return sr.ReadToEnd();

                            }
                        }
                    }
                }
            }
        }
        catch
        {
            return "";
        }
    }
    #endregion
}