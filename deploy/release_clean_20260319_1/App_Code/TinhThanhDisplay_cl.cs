using System;
using System.Collections.Generic;
using System.Linq;

public static class TinhThanhDisplay_cl
{
    private static readonly Dictionary<string, string> OldToNew = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // 63 tỉnh/thành cũ -> tỉnh/thành mới (theo Nghị quyết 202/2025/QH15)
        { "Hà Giang", "Tuyên Quang" },
        { "Tuyên Quang", "Tuyên Quang" },
        { "Cao Bằng", "Cao Bằng" },
        { "Bắc Kạn", "Thái Nguyên" },
        { "Thái Nguyên", "Thái Nguyên" },
        { "Lào Cai", "Lào Cai" },
        { "Yên Bái", "Lào Cai" },
        { "Lạng Sơn", "Lạng Sơn" },
        { "Bắc Giang", "Bắc Ninh" },
        { "Bắc Ninh", "Bắc Ninh" },
        { "Phú Thọ", "Phú Thọ" },
        { "Vĩnh Phúc", "Phú Thọ" },
        { "Hòa Bình", "Phú Thọ" },
        { "Quảng Ninh", "Quảng Ninh" },
        { "Hà Nội", "Hà Nội" },
        { "TP. Hà Nội", "TP. Hà Nội" },
        { "TP Hà Nội", "TP Hà Nội" },
        { "Hải Dương", "TP. Hải Phòng" },
        { "Hải Phòng", "TP. Hải Phòng" },
        { "TP. Hải Phòng", "TP. Hải Phòng" },
        { "TP Hải Phòng", "TP. Hải Phòng" },
        { "Hưng Yên", "Hưng Yên" },
        { "Thái Bình", "Hưng Yên" },
        { "Hà Nam", "Ninh Bình" },
        { "Nam Định", "Ninh Bình" },
        { "Ninh Bình", "Ninh Bình" },
        { "Thanh Hóa", "Thanh Hóa" },
        { "Nghệ An", "Nghệ An" },
        { "Hà Tĩnh", "Hà Tĩnh" },
        { "Quảng Bình", "Quảng Trị" },
        { "Quảng Trị", "Quảng Trị" },
        { "Thừa Thiên Huế", "TP. Huế" },
        { "Thừa Thiên - Huế", "TP. Huế" },
        { "TP. Huế", "TP. Huế" },
        { "Huế", "TP. Huế" },
        { "Quảng Nam", "TP. Đà Nẵng" },
        { "Đà Nẵng", "TP. Đà Nẵng" },
        { "TP. Đà Nẵng", "TP. Đà Nẵng" },
        { "Quảng Ngãi", "Quảng Ngãi" },
        { "Kon Tum", "Quảng Ngãi" },
        { "Bình Định", "Gia Lai" },
        { "Gia Lai", "Gia Lai" },
        { "Phú Yên", "Đắk Lắk" },
        { "Đắk Lắk", "Đắk Lắk" },
        { "Đắk Nông", "Lâm Đồng" },
        { "Lâm Đồng", "Lâm Đồng" },
        { "Khánh Hòa", "Khánh Hòa" },
        { "Ninh Thuận", "Khánh Hòa" },
        { "Bình Thuận", "Lâm Đồng" },
        { "Bình Phước", "Đồng Nai" },
        { "Đồng Nai", "Đồng Nai" },
        { "Long An", "Tây Ninh" },
        { "Tây Ninh", "Tây Ninh" },
        { "Bình Dương", "TP. Hồ Chí Minh" },
        { "Bà Rịa - Vũng Tàu", "TP. Hồ Chí Minh" },
        { "TP. Hồ Chí Minh", "TP. Hồ Chí Minh" },
        { "TP.HCM", "TP. Hồ Chí Minh" },
        { "TP HCM", "TP. Hồ Chí Minh" },
        { "Hồ Chí Minh", "TP. Hồ Chí Minh" },
        { "Tiền Giang", "Đồng Tháp" },
        { "Đồng Tháp", "Đồng Tháp" },
        { "Kiên Giang", "An Giang" },
        { "An Giang", "An Giang" },
        { "Bến Tre", "Vĩnh Long" },
        { "Trà Vinh", "Vĩnh Long" },
        { "Vĩnh Long", "Vĩnh Long" },
        { "Sóc Trăng", "TP. Cần Thơ" },
        { "Hậu Giang", "TP. Cần Thơ" },
        { "Cần Thơ", "TP. Cần Thơ" },
        { "TP. Cần Thơ", "TP. Cần Thơ" },
        { "Bạc Liêu", "Cà Mau" },
        { "Cà Mau", "Cà Mau" },
        { "Điện Biên", "Điện Biên" },
        { "Lai Châu", "Lai Châu" },
        { "Sơn La", "Sơn La" }
    };

    public static string Format(string rawName)
    {
        string name = (rawName ?? "").Trim();
        if (string.IsNullOrEmpty(name))
            return "Không có";

        if (name.Contains("("))
            return name;

        string newName;
        if (OldToNew.TryGetValue(name, out newName) && !string.IsNullOrWhiteSpace(newName))
        {
            string cleanedNew = newName.Trim();
            if (string.Equals(cleanedNew, name, StringComparison.OrdinalIgnoreCase))
                return name;

            if (!cleanedNew.EndsWith("mới", StringComparison.OrdinalIgnoreCase))
                cleanedNew = cleanedNew + " mới";

            return name + " (" + cleanedNew + ")";
        }

        return name;
    }

    public static string ResolveNameFromId(string rawOrId, IDictionary<long, string> idMap)
    {
        string value = (rawOrId ?? "").Trim();
        if (string.IsNullOrEmpty(value))
            return "";

        long id;
        if (idMap != null && long.TryParse(value, out id))
        {
            string mapped;
            if (idMap.TryGetValue(id, out mapped))
                return mapped ?? "";
        }

        return value;
    }
}
