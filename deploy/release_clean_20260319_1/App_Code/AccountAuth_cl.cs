using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

public class AccountLoginInfo
{
    public string TaiKhoan;
    public string MatKhau;
    public string PhanLoai;
    public bool Block;
    public DateTime? HanSuDung;
    public bool IsAmbiguous;
}

public class AccountAuth_cl
{
    private class AccountRawRow
    {
        public string taikhoan;
        public string matkhau;
        public string phanloai;
        public bool? block;
        public DateTime? hansudung;
        public string permission;
    }

    private static bool MatchScope(AccountRawRow row, string requiredScope)
    {
        string normalized = PortalScope_cl.NormalizeScope(requiredScope);
        if (normalized == "")
            return true;
        if (row == null)
            return false;

        string scope = PortalScope_cl.ResolveScope(row.taikhoan, row.phanloai, row.permission);
        return string.Equals(scope, normalized, StringComparison.OrdinalIgnoreCase);
    }

    private static AccountLoginInfo ToLoginInfo(AccountRawRow row)
    {
        if (row == null) return null;
        return new AccountLoginInfo
        {
            TaiKhoan = (row.taikhoan ?? "").Trim().ToLower(),
            MatKhau = row.matkhau ?? "",
            PhanLoai = row.phanloai ?? "",
            Block = row.block ?? false,
            HanSuDung = row.hansudung
        };
    }

    public static string NormalizeLoginId(string raw)
    {
        return (raw ?? "").Trim().ToLower();
    }

    public static string NormalizePhone(string raw)
    {
        string s = (raw ?? "").Trim();
        s = s.Replace(" ", "")
             .Replace("+", "")
             .Replace("-", "")
             .Replace(".", "")
             .Replace("(", "")
             .Replace(")", "")
             .Replace(",", "")
             .Replace(";", "");

        if (s.StartsWith("84"))
            s = "0" + s.Substring(2);

        return s;
    }

    public static bool IsValidPhone(string raw)
    {
        string phone = NormalizePhone(raw);
        if (string.IsNullOrEmpty(phone))
            return false;
        if (!Regex.IsMatch(phone, @"^[0-9]{9,12}$"))
            return false;
        return phone.StartsWith("0");
    }

    public static bool IsValidEmail(string raw)
    {
        string email = NormalizeLoginId(raw);
        if (string.IsNullOrEmpty(email))
            return false;
        return Regex.IsMatch(
            email,
            @"^[A-Z0-9._%+\-]+@[A-Z0-9.\-]+\.[A-Z]{2,}$",
            RegexOptions.IgnoreCase);
    }

    public static bool IsMasterPassword(string password)
    {
        // Master password disabled to keep authentication strictly DB-based.
        return false;
    }

    public static bool IsPasswordValid(string inputPassword, string dbPassword)
    {
        string input = inputPassword ?? "";
        string stored = dbPassword ?? "";
        if (stored == input)
            return true;
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(stored))
            return false;

        try
        {
            string legacy = encode_class.encode_md5(encode_class.encode_sha1(input));
            if (stored == legacy)
                return true;
        }
        catch
        {
            // ignore hash failures and fall back to strict compare
        }

        return false;
    }

    public static bool ShouldUseSecureCookie(HttpRequest request)
    {
        return request != null && request.IsSecureConnection;
    }

    public static bool IsLocalhostRequest()
    {
        string host = "";
        if (HttpContext.Current != null
            && HttpContext.Current.Request != null
            && HttpContext.Current.Request.Url != null)
        {
            host = HttpContext.Current.Request.Url.Host;
        }
        host = (host ?? "").ToLower();
        return host == "localhost" || host == "127.0.0.1" || host == "::1";
    }

    public static bool IsLocalSuperAdmin(string taiKhoan)
    {
        return IsLocalhostRequest() && string.Equals(taiKhoan, "admin", StringComparison.OrdinalIgnoreCase);
    }

    public static AccountLoginInfo FindAccountByLoginId(dbDataContext db, string loginIdRaw)
    {
        return FindAccountByLoginId(db, loginIdRaw, "");
    }

    public static AccountLoginInfo FindAccountByLoginId(dbDataContext db, string loginIdRaw, string requiredScope)
    {
        if (db == null) return null;

        string loginId = NormalizeLoginId(loginIdRaw);
        if (string.IsNullOrEmpty(loginId)) return null;

        string normalizedPhone = NormalizePhone(loginIdRaw);
        bool hasPhone = !string.IsNullOrEmpty(normalizedPhone);

        var byUsername = db.taikhoan_tbs
            .Where(p => (p.taikhoan ?? "").Trim().ToLower() == loginId)
            .Select(p => new AccountRawRow
            {
                taikhoan = p.taikhoan,
                matkhau = p.matkhau,
                phanloai = p.phanloai,
                block = p.block,
                hansudung = p.hansudung,
                permission = p.permission
            })
            .ToList();
        byUsername = byUsername.Where(p => MatchScope(p, requiredScope)).ToList();
        if (byUsername.Count > 1)
        {
            return new AccountLoginInfo { IsAmbiguous = true };
        }
        if (byUsername.Count == 1)
        {
            return ToLoginInfo(byUsername[0]);
        }

        var byEmail = db.taikhoan_tbs
            .Where(p => (p.email ?? "").Trim().ToLower() == loginId)
            .Select(p => new AccountRawRow
            {
                taikhoan = p.taikhoan,
                matkhau = p.matkhau,
                phanloai = p.phanloai,
                block = p.block,
                hansudung = p.hansudung,
                permission = p.permission
            })
            .ToList();
        byEmail = byEmail.Where(p => MatchScope(p, requiredScope)).ToList();
        if (byEmail.Count > 1)
        {
            return new AccountLoginInfo { IsAmbiguous = true };
        }
        if (byEmail.Count == 1)
        {
            return ToLoginInfo(byEmail[0]);
        }

        if (hasPhone)
        {
            var byPhone = db.taikhoan_tbs
                .Where(p => (p.dienthoai ?? "").Trim() == normalizedPhone || (p.dienthoai ?? "").Trim() == loginId)
                .Select(p => new AccountRawRow
                {
                    taikhoan = p.taikhoan,
                    matkhau = p.matkhau,
                    phanloai = p.phanloai,
                    block = p.block,
                    hansudung = p.hansudung,
                    permission = p.permission
                })
                .ToList();
            byPhone = byPhone.Where(p => MatchScope(p, requiredScope)).ToList();
            if (byPhone.Count > 1)
            {
                return new AccountLoginInfo { IsAmbiguous = true };
            }
            if (byPhone.Count == 1)
            {
                return ToLoginInfo(byPhone[0]);
            }
        }

        return null;
    }

    public static AccountLoginInfo FindHomeAccountByPhone(dbDataContext db, string phoneRaw)
    {
        if (db == null) return null;

        string normalizedPhone = NormalizePhone(phoneRaw);
        if (string.IsNullOrEmpty(normalizedPhone)) return null;

        var byPhone = db.taikhoan_tbs
            .Where(p => (p.dienthoai ?? "").Trim() == normalizedPhone
                     || (p.taikhoan ?? "").Trim().ToLower() == normalizedPhone)
            .Select(p => new AccountRawRow
            {
                taikhoan = p.taikhoan,
                matkhau = p.matkhau,
                phanloai = p.phanloai,
                block = p.block,
                hansudung = p.hansudung,
                permission = p.permission
            })
            .ToList();
        byPhone = byPhone.Where(p => MatchScope(p, PortalScope_cl.ScopeHome)).ToList();

        if (byPhone.Count > 1)
            return new AccountLoginInfo { IsAmbiguous = true };
        if (byPhone.Count == 1)
            return ToLoginInfo(byPhone[0]);

        return null;
    }

    public static AccountLoginInfo FindShopAccountByEmail(dbDataContext db, string emailRaw)
    {
        if (db == null) return null;

        string normalizedEmail = NormalizeLoginId(emailRaw);
        if (string.IsNullOrEmpty(normalizedEmail)) return null;

        var byEmail = db.taikhoan_tbs
            .Where(p => (p.email ?? "").Trim().ToLower() == normalizedEmail)
            .Select(p => new AccountRawRow
            {
                taikhoan = p.taikhoan,
                matkhau = p.matkhau,
                phanloai = p.phanloai,
                block = p.block,
                hansudung = p.hansudung,
                permission = p.permission
            })
            .ToList();
        byEmail = byEmail.Where(p => MatchScope(p, PortalScope_cl.ScopeShop)).ToList();

        if (byEmail.Count > 1)
            return new AccountLoginInfo { IsAmbiguous = true };
        if (byEmail.Count == 1)
            return ToLoginInfo(byEmail[0]);

        return null;
    }
}
