using System;

public static class HanhVi9Cap_cl
{
    // 9 hành vi theo đúng map % = tên
    public const int HanhVi_15_KetNoi = 1;
    public const int HanhVi_9_ChiaSe = 2;
    public const int HanhVi_6_Marketing = 3;
    public const int HanhVi_10_BanHang = 4;
    public const int HanhVi_15_PhatTrien = 5;
    public const int HanhVi_25_DieuPhoi = 6;
    public const int HanhVi_10_PhucLoi = 7;
    public const int HanhVi_6_GhiNhan = 8;
    public const int HanhVi_4_ChamSoc = 9;

    public static string GetTenHanhViTheoLoai(int? loaiHanhVi)
    {
        if (!loaiHanhVi.HasValue)
            return "-";

        switch (loaiHanhVi.Value)
        {
            case HanhVi_15_KetNoi: return "15% = Hành vi kết nối";
            case HanhVi_9_ChiaSe: return "9% = Hành vi chia sẻ";
            case HanhVi_6_Marketing: return "6% = Hành vi marketing";
            case HanhVi_10_BanHang: return "10% = Hành vi bán hàng";
            case HanhVi_15_PhatTrien: return "15% = Hành vi phát triển";
            case HanhVi_25_DieuPhoi: return "25% = Hành vi điều phối";
            case HanhVi_10_PhucLoi: return "10% = Hành vi phúc lợi";
            case HanhVi_6_GhiNhan: return "6% = Hành vi ghi nhận";
            case HanhVi_4_ChamSoc: return "4% = Hành vi chăm sóc";
            default: return "Hành vi #" + loaiHanhVi.Value;
        }
    }

    public static int? GetLoaiHanhViByCapGiaTri(int cap, int giaTri)
    {
        if (cap == 1 && giaTri == 15) return HanhVi_15_KetNoi;
        if (cap == 1 && giaTri == 9) return HanhVi_9_ChiaSe;
        if (cap == 1 && giaTri == 6) return HanhVi_6_Marketing;

        if (cap == 2 && giaTri == 10) return HanhVi_10_BanHang;
        if (cap == 2 && giaTri == 15) return HanhVi_15_PhatTrien;
        if (cap == 2 && giaTri == 25) return HanhVi_25_DieuPhoi;

        if (cap == 3 && giaTri == 10) return HanhVi_10_PhucLoi;
        if (cap == 3 && giaTri == 6) return HanhVi_6_GhiNhan;
        if (cap == 3 && giaTri == 4) return HanhVi_4_ChamSoc;

        return null;
    }

    public static string GetTenCapDoTheoCapGiaTri(int cap, int giaTri)
    {
        int? loaiHanhVi = GetLoaiHanhViByCapGiaTri(cap, giaTri);
        if (loaiHanhVi.HasValue)
            return GetTenHanhViTheoLoai(loaiHanhVi);

        return "Cấp " + cap + " - " + giaTri;
    }

    public static int? GetLoaiHoSoTongTheoHanhVi(int? loaiHanhVi)
    {
        if (!loaiHanhVi.HasValue) return null;

        int loai = loaiHanhVi.Value;
        if (loai >= 1 && loai <= 3) return 2; // Hồ sơ quyền ưu đãi
        if (loai >= 4 && loai <= 6) return 3; // Hồ sơ hành vi lao động
        if (loai >= 7 && loai <= 9) return 4; // Hồ sơ chỉ số gắn kết

        return null;
    }
}
