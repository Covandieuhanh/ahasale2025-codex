using System;
using System.Linq;

public static class TierHome_cl
{
    public const int Tier0 = 0;
    public const int Tier1 = 1;
    public const int Tier2 = 2;
    public const int Tier3 = 3;

    public static int GetTierFromHanhVi(int? kyHieu9HanhVi)
    {
        int hanhVi = kyHieu9HanhVi ?? 0;
        if (hanhVi >= 7 && hanhVi <= 9) return Tier3;
        if (hanhVi >= 4 && hanhVi <= 6) return Tier2;
        if (hanhVi >= 1 && hanhVi <= 3) return Tier1;
        return Tier0;
    }

    public static int? GetCurrentHanhViFromAccount(taikhoan_tb acc)
    {
        if (acc == null) return null;

        int cap = acc.HeThongSanPham_Cap123 ?? 0;
        int giaTri = 0;
        if (cap == 1) giaTri = acc.HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6 ?? 0;
        else if (cap == 2) giaTri = acc.HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10 ?? 0;
        else if (cap == 3) giaTri = acc.HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4 ?? 0;
        else return null;

        return HanhVi9Cap_cl.GetLoaiHanhViByCapGiaTri(cap, giaTri);
    }

    public static int TinhTierHome(dbDataContext db, string taiKhoan)
    {
        if (db == null) return Tier0;
        string tk = (taiKhoan ?? "").Trim();
        if (tk == "") return Tier0;

        var acc = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == tk);
        int tierFromAccount = GetTierFromHanhVi(GetCurrentHanhViFromAccount(acc));
        if (tierFromAccount > Tier0)
            return tierFromAccount;

        int tier = Tier0;

        var approved = db.YeuCau_HeThongSanPham_tbs
            .Where(x => x.taikhoan == tk && x.TrangThai == 1)
            .Select(x => new { x.CapYeuCau, x.GiaTriYeuCau })
            .ToList();

        foreach (var item in approved)
        {
            int? hanhVi = HanhVi9Cap_cl.GetLoaiHanhViByCapGiaTri(item.CapYeuCau, item.GiaTriYeuCau);
            tier = Math.Max(tier, GetTierFromHanhVi(hanhVi));
        }

        if (tier < Tier0) tier = Tier0;
        if (tier > Tier3) tier = Tier3;
        return tier;
    }

    public static bool CanViewHoSo(int tierHome, int loaiHoSoVi)
    {
        // LoaiHoSo: 1=TieuDung, 2=UuDai, 3=LaoDong, 4=GanKet
        if (loaiHoSoVi == 4) return tierHome >= Tier3;
        if (loaiHoSoVi == 3) return tierHome >= Tier2;
        return true; // tier 0/1 luôn thấy tiêu dùng + ưu đãi
    }

    public static string GetTenTangHome(int tierHome)
    {
        if (tierHome >= Tier3) return "Đồng hành hệ sinh thái";
        if (tierHome >= Tier2) return "Cộng tác phát triển";
        return "Khách hàng";
    }
}
