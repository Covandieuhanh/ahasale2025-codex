using System;
using System.Linq;

public static class CardIssuance_cl
{
    public const int CardTypeUuDai = 1;
    public const int CardTypeTieuDung = 2;
    public const int CardTypeShopPartner = 3;
    public const int CardTypeDongHanhHeSinhThai = 4;
    public const int CardTypeLaoDong = 5;
    public const string DefaultInitialPin = "6868";

    public static string GetCardName(int loai)
    {
        switch (loai)
        {
            case CardTypeUuDai: return "Thẻ ưu đãi";
            case CardTypeTieuDung: return "Thẻ tiêu dùng";
            case CardTypeShopPartner: return "Thẻ gian hàng đối tác";
            case CardTypeDongHanhHeSinhThai: return "Thẻ đồng hành hệ sinh thái";
            case CardTypeLaoDong: return "Thẻ lao động";
            default: return "Thẻ #" + loai;
        }
    }

    public static string ResolveAccountScope(taikhoan_tb account)
    {
        if (account == null)
            return "";
        return PortalScope_cl.ResolveScope(account.taikhoan, account.phanloai, account.permission);
    }

    public static bool IsCardTypeAllowedForScope(string scope, int loaiThe)
    {
        if (string.Equals(scope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase))
            return loaiThe == CardTypeShopPartner;

        if (string.Equals(scope, PortalScope_cl.ScopeHome, StringComparison.OrdinalIgnoreCase))
        {
            return loaiThe == CardTypeUuDai
                || loaiThe == CardTypeTieuDung
                || loaiThe == CardTypeLaoDong
                || loaiThe == CardTypeDongHanhHeSinhThai;
        }

        return false;
    }

    public static string BuildInvalidCardTypeMessage(string scope)
    {
        if (string.Equals(scope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase))
            return "Tài khoản gian hàng đối tác chỉ được phát hành thẻ gian hàng đối tác.";

        if (string.Equals(scope, PortalScope_cl.ScopeHome, StringComparison.OrdinalIgnoreCase))
            return "Tài khoản home chỉ được phát hành thẻ ưu đãi, thẻ tiêu dùng, thẻ lao động hoặc thẻ đồng hành hệ sinh thái.";

        return "Tài khoản này không thuộc hệ home/gian hàng đối tác nên không thể phát hành thẻ.";
    }

    public static The_PhatHanh_tb IssueCard(
        dbDataContext db,
        string taiKhoan,
        int loaiThe,
        string actor,
        DateTime now)
    {
        if (db == null)
            throw new ArgumentNullException("db");

        string accountKey = (taiKhoan ?? "").Trim().ToLowerInvariant();
        if (accountKey == "")
            throw new InvalidOperationException("Không xác định được tài khoản để phát hành thẻ.");

        taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == accountKey);
        if (account == null)
            throw new InvalidOperationException("Tài khoản không tồn tại.");

        return IssueCard(db, account, loaiThe, actor, now);
    }

    public static The_PhatHanh_tb IssueCard(
        dbDataContext db,
        taikhoan_tb account,
        int loaiThe,
        string actor,
        DateTime now)
    {
        if (db == null)
            throw new ArgumentNullException("db");
        if (account == null || string.IsNullOrWhiteSpace(account.taikhoan))
            throw new InvalidOperationException("Không xác định được tài khoản để phát hành thẻ.");

        string scope = ResolveAccountScope(account);
        if (!IsCardTypeAllowedForScope(scope, loaiThe))
            throw new InvalidOperationException(BuildInvalidCardTypeMessage(scope));

        string accountKey = (account.taikhoan ?? "").Trim().ToLowerInvariant();
        string actorKey = (actor ?? "").Trim();
        if (actorKey == "")
            actorKey = "system";

        var olds = db.The_PhatHanh_tbs
            .Where(p => p.taikhoan == accountKey && p.LoaiThe == loaiThe && p.TrangThai == true)
            .ToList();

        for (int i = 0; i < olds.Count; i++)
        {
            olds[i].TrangThai = false;
            olds[i].NgayCapNhatTrangThai = now;
            olds[i].NguoiCapNhatTrangThai = actorKey;
        }

        The_PhatHanh_tb card = new The_PhatHanh_tb();
        card.idGuide = Guid.NewGuid();
        card.taikhoan = accountKey;
        card.LoaiThe = loaiThe;
        card.TenThe = GetCardName(loaiThe);
        card.NgayPhatHanh = now;
        card.TrangThai = true;
        card.NgayTao = now;
        card.NguoiTao = actorKey;
        card.NgayCapNhatTrangThai = now;
        card.NguoiCapNhatTrangThai = actorKey;

        account.mapin_thanhtoan = PinSecurity_cl.HashPin(DefaultInitialPin);

        db.The_PhatHanh_tbs.InsertOnSubmit(card);
        return card;
    }
}
