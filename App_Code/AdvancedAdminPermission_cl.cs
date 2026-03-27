using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

public static class AdvancedAdminPermission_cl
{
    public const string ModuleDashboard = "dashboard";
    public const string ModuleStorefront = "storefront";
    public const string ModuleStaff = "staff";
    public const string ModuleSales = "sales";
    public const string ModuleCustomers = "customers";
    public const string ModuleBooking = "booking";
    public const string ModuleCashflow = "cashflow";
    public const string ModuleWarehouse = "warehouse";
    public const string ModuleServiceCards = "service_cards";
    public const string ModuleSupplies = "supplies";
    public const string ModuleTraining = "training";
    public const string ModuleSystem = "system";
    public const string ModulePeopleHub = "people_hub";

    public sealed class PresetInfo
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string[] Permissions { get; set; }
    }

    private sealed class RoutePermissionRule
    {
        public string Prefix { get; set; }
        public string Label { get; set; }
        public string[] Permissions { get; set; }
    }

    private static readonly Dictionary<string, string[]> ModulePermissions = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
    {
        { ModuleDashboard, new[] { "q0_1", "q0_2", "q0_3", "q0_4", "n0_1", "n0_2", "n0_3", "n0_4" } },
        { ModuleStorefront, new[] { "q1_1", "q1_2", "q1_3", "q1_4", "q1_5", "q3_1", "q3_2", "q3_3", "q3_4", "q3_5", "q3_6", "q4_1", "q4_2", "q4_3", "q4_4", "q4_5", "q4_6", "q4_7", "q5_1", "q6_1" } },
        { ModuleStaff, new[] { "q2_1", "q2_2", "q2_3", "q2_4", "q2_5", "q2_6", "q2_7", "q2_8", "q2_9", "q2_10", "n2_1", "n2_2", "n2_3", "n2_4", "n2_5", "n2_6", "n2_7", "n2_8", "n2_9", "n2_10" } },
        { ModuleSales, new[] { "q7_1", "q7_2", "q7_3", "q7_4", "q7_5", "q7_6", "q7_7", "q7_8", "q7_9", "q7_10", "n7_1", "n7_2", "n7_3", "n7_4", "n7_5", "n7_6", "n7_7", "n7_8", "n7_9", "n7_10" } },
        { ModuleCustomers, new[] { "q8_1", "q8_2", "q8_3", "q8_4", "q8_5", "n8_1", "n8_2", "n8_3", "n8_4", "n8_5" } },
        { ModuleBooking, new[] { "q10_1", "q10_2", "q10_3", "q10_4" } },
        { ModuleCashflow, new[] { "q9_1", "q9_2", "q9_3", "q9_4", "q9_5", "q9_6", "n9_1", "n9_2", "n9_3", "n9_4", "n9_5", "n9_6" } },
        { ModuleWarehouse, new[] { "q11_1", "q11_2", "q11_3", "q11_4", "q11_5", "q11_6", "q11_7", "q11_8" } },
        { ModuleServiceCards, new[] { "q12_1", "q12_2", "q12_3", "q12_4", "q12_5", "n12_1", "n12_2", "n12_3", "n12_4", "n12_5" } },
        { ModuleSupplies, new[] { "q13_1", "q13_2", "q13_3", "q13_4", "q13_5", "q13_6", "q13_7", "q13_8", "q13_9" } },
        { ModuleTraining, new[] { "q14_1", "q14_2", "q14_3", "q14_4", "q14_5", "n14_1", "n14_2", "n14_3", "n14_4", "n14_5", "q15_1", "q15_2", "q15_3", "q15_4", "n15_1", "n15_2", "n15_3", "n15_4" } },
        { ModuleSystem, new[] { "q16_0", "q16_1", "q16_2" } },
        { ModulePeopleHub, new[] { "q2_1", "q2_2", "q2_3", "q2_4", "q2_5", "q2_6", "q2_7", "q2_8", "q2_9", "q2_10", "n2_1", "n2_2", "n2_3", "n2_4", "n2_5", "n2_6", "n2_7", "n2_8", "n2_9", "n2_10", "q8_1", "q8_2", "q8_3", "q8_4", "q8_5", "n8_1", "n8_2", "n8_3", "n8_4", "n8_5", "q14_1", "q14_2", "q14_3", "q14_4", "q14_5", "n14_1", "n14_2", "n14_3", "n14_4", "n14_5", "q15_1", "q15_2", "q15_3", "q15_4", "n15_1", "n15_2", "n15_3", "n15_4" } }
    };

    private static readonly List<PresetInfo> Presets = new List<PresetInfo>
    {
        new PresetInfo
        {
            Key = "chu_shop",
            Label = "Chủ shop",
            Description = "Toàn quyền cấu hình hệ thống, nhân sự và vận hành.",
            Permissions = Flatten(ModuleDashboard, ModuleStorefront, ModuleStaff, ModuleSales, ModuleCustomers, ModuleBooking, ModuleCashflow, ModuleWarehouse, ModuleServiceCards, ModuleSupplies, ModuleTraining, ModuleSystem)
        },
        new PresetInfo
        {
            Key = "le_tan",
            Label = "Lễ tân",
            Description = "Nhận lịch, xem khách hàng và xử lý lịch hẹn cơ bản tại quầy.",
            Permissions = Flatten(ModuleDashboard, ModuleCustomers, ModuleBooking)
        },
        new PresetInfo
        {
            Key = "thu_ngan",
            Label = "Thu ngân",
            Description = "Tập trung hóa đơn, thu chi, thẻ dịch vụ và tra cứu khách hàng.",
            Permissions = Flatten(ModuleDashboard, ModuleCustomers, ModuleSales, ModuleCashflow, ModuleServiceCards)
        },
        new PresetInfo
        {
            Key = "quan_ly_kho",
            Label = "Quản lý kho",
            Description = "Phụ trách kho hàng, vật tư và đơn nhập.",
            Permissions = Flatten(ModuleDashboard, ModuleWarehouse, ModuleSupplies)
        },
        new PresetInfo
        {
            Key = "marketing",
            Label = "Marketing",
            Description = "Phụ trách nội dung, menu, block hiển thị và yêu cầu tư vấn trên /shop.",
            Permissions = Flatten(ModuleDashboard, ModuleStorefront)
        },
        new PresetInfo
        {
            Key = "dao_tao",
            Label = "Đào tạo",
            Description = "Quản lý chuyên gia, học viên và dữ liệu liên quan đào tạo.",
            Permissions = Flatten(ModuleDashboard, ModuleTraining, ModuleCustomers)
        },
        new PresetInfo
        {
            Key = "van_hanh",
            Label = "Vận hành",
            Description = "Bao quát khách hàng, lịch hẹn, hóa đơn, thu chi và thẻ dịch vụ.",
            Permissions = Flatten(ModuleDashboard, ModuleCustomers, ModuleBooking, ModuleSales, ModuleCashflow, ModuleServiceCards)
        },
        new PresetInfo
        {
            Key = "quan_ly_tong_hop",
            Label = "Quản lý tổng hợp",
            Description = "Mẫu rộng cho quản lý chi nhánh, trừ quyền hệ thống sâu.",
            Permissions = Flatten(ModuleDashboard, ModuleStorefront, ModuleStaff, ModuleSales, ModuleCustomers, ModuleBooking, ModuleCashflow, ModuleWarehouse, ModuleServiceCards, ModuleSupplies, ModuleTraining)
        }
    };

    private static readonly List<RoutePermissionRule> RouteRules = new List<RoutePermissionRule>
    {
        new RoutePermissionRule { Prefix = "/gianhang/admin/cau-hinh-storefront/", Label = "Cấu hình /shop", Permissions = new[] { "q1_3" } },
        new RoutePermissionRule { Prefix = "/gianhang/admin/cai-dat-chung/", Label = "Cài đặt chung", Permissions = new[] { "q1_1", "q1_2", "q1_3", "q1_4", "q1_5" } },
        new RoutePermissionRule { Prefix = "/gianhang/admin/cau-hinh-chung/", Label = "Cấu hình chung", Permissions = new[] { "q1_1", "q1_2", "q1_3", "q1_4", "q1_5" } },
        new RoutePermissionRule { Prefix = "/gianhang/admin/quan-ly-menu/", Label = "Quản lý menu", Permissions = new[] { "q3_1", "q3_2", "q3_3", "q3_4", "q3_5", "q3_6" } },
        new RoutePermissionRule { Prefix = "/gianhang/admin/quan-ly-bai-viet/", Label = "Quản lý bài viết / dịch vụ / sản phẩm", Permissions = new[] { "q4_1", "q4_2", "q4_3", "q4_4", "q4_5", "q4_6" } },
        new RoutePermissionRule { Prefix = "/gianhang/admin/quan-ly-module/", Label = "Quản lý module", Permissions = new[] { "q5_1" } },
        new RoutePermissionRule { Prefix = "/gianhang/admin/yeu-cau-tu-van/", Label = "Yêu cầu tư vấn", Permissions = new[] { "q6_1" } },
        new RoutePermissionRule { Prefix = "/gianhang/admin/quan-ly-tai-khoan/", Label = "Tài khoản nhân viên", Permissions = new[] { "q2_1", "q2_2", "q2_3", "q2_4", "q2_5", "q2_6", "q2_7", "q2_8", "q2_9", "q2_10", "n2_1", "n2_2", "n2_3", "n2_4", "n2_5", "n2_6", "n2_7", "n2_8", "n2_9", "n2_10" } },
        new RoutePermissionRule { Prefix = "/gianhang/admin/quan-ly-hoa-don/", Label = "Quản lý hóa đơn", Permissions = new[] { "q7_1", "q7_2", "q7_3", "q7_4", "q7_5", "q7_6", "q7_7", "q7_8", "q7_9", "q7_10", "n7_1", "n7_2", "n7_3", "n7_4", "n7_5", "n7_6", "n7_7", "n7_8", "n7_9", "n7_10" } },
        new RoutePermissionRule { Prefix = "/gianhang/admin/quan-ly-thu-chi/", Label = "Quản lý thu chi", Permissions = new[] { "q9_1", "q9_2", "q9_3", "q9_4", "q9_5", "q9_6", "n9_1", "n9_2", "n9_3", "n9_4", "n9_5", "n9_6" } },
        new RoutePermissionRule { Prefix = "/gianhang/admin/quan-ly-kho-hang/", Label = "Kho hàng", Permissions = new[] { "q11_1", "q11_2", "q11_3", "q11_4", "q11_5", "q11_6", "q11_7", "q11_8" } },
        new RoutePermissionRule { Prefix = "/gianhang/admin/quan-ly-vat-tu/", Label = "Kho vật tư", Permissions = new[] { "q13_1", "q13_2", "q13_3", "q13_4", "q13_5", "q13_6", "q13_7", "q13_8", "q13_9" } },
        new RoutePermissionRule { Prefix = "/gianhang/admin/quan-ly-the-dich-vu/", Label = "Thẻ dịch vụ", Permissions = new[] { "q12_1", "q12_2", "q12_3", "q12_4", "q12_5", "n12_1", "n12_2", "n12_3", "n12_4", "n12_5" } },
        new RoutePermissionRule { Prefix = "/gianhang/admin/quan-ly-hoc-vien/", Label = "Quản lý học viên", Permissions = new[] { "q14_1", "q14_2", "q14_3", "q14_4", "q14_5", "n14_1", "n14_2", "n14_3", "n14_4", "n14_5" } },
        new RoutePermissionRule { Prefix = "/gianhang/admin/quan-ly-giang-vien/", Label = "Quản lý chuyên gia", Permissions = new[] { "q15_1", "q15_2", "q15_3", "q15_4", "n15_1", "n15_2", "n15_3", "n15_4" } },
        new RoutePermissionRule { Prefix = "/gianhang/admin/quan-ly-he-thong/", Label = "Quản lý hệ thống", Permissions = new[] { "q16_0", "q16_1", "q16_2" } }
    };

    public static IList<PresetInfo> GetPresets()
    {
        return Presets.AsReadOnly();
    }

    public static PresetInfo GetPresetByKey(string key)
    {
        return Presets.FirstOrDefault(p => string.Equals(p.Key, key ?? "", StringComparison.OrdinalIgnoreCase));
    }

    public static string BuildPermissionCsvForPreset(string key)
    {
        PresetInfo preset = GetPresetByKey(key);
        if (preset == null || preset.Permissions == null || preset.Permissions.Length == 0)
            return string.Empty;
        return string.Join(",", preset.Permissions);
    }

    public static string[] GetAllPermissionCodes()
    {
        return ModulePermissions.SelectMany(p => p.Value).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(p => p).ToArray();
    }

    public static bool CanAccessModule(string user, string moduleKey)
    {
        if (string.IsNullOrWhiteSpace(user))
            return false;

        if ((user ?? "").Trim().ToLowerInvariant() == "admin")
            return true;

        string[] permissions;
        if (!ModulePermissions.TryGetValue(moduleKey ?? "", out permissions))
            return false;

        return HasAnyPermission(user, permissions);
    }

    public static bool CanAccessPeopleHub(string user)
    {
        return CanAccessModule(user, ModulePeopleHub);
    }

    public static bool HasAnyPermission(string user, IEnumerable<string> permissions)
    {
        if (string.IsNullOrWhiteSpace(user))
            return false;

        foreach (string permission in permissions ?? Enumerable.Empty<string>())
        {
            if (!string.IsNullOrWhiteSpace(permission) && bcorn_class.check_quyen(user, permission) == "")
                return true;
        }
        return false;
    }

    public static string BuildPresetJson()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{");
        bool isFirst = true;
        foreach (PresetInfo preset in Presets)
        {
            if (!isFirst)
                sb.Append(",");

            sb.Append("\"").Append(HttpUtility.JavaScriptStringEncode(preset.Key)).Append("\":");
            sb.Append("{");
            sb.Append("\"label\":\"").Append(HttpUtility.JavaScriptStringEncode(preset.Label)).Append("\",");
            sb.Append("\"description\":\"").Append(HttpUtility.JavaScriptStringEncode(preset.Description)).Append("\",");
            sb.Append("\"permissions\":").Append(ToJsonArray(preset.Permissions));
            sb.Append("}");
            isFirst = false;
        }
        sb.Append("}");
        return sb.ToString();
    }

    public static string BuildAllPermissionCodesJson()
    {
        return ToJsonArray(GetAllPermissionCodes());
    }

    public static bool CanAccessRoute(string user, string absolutePath, out string deniedLabel)
    {
        deniedLabel = string.Empty;
        string path = (absolutePath ?? string.Empty).Trim().ToLowerInvariant();
        if (path == string.Empty)
            return true;

        if (path == "/gianhang/admin" || path == "/gianhang/admin/" || path == "/gianhang/admin/default.aspx")
        {
            deniedLabel = "Bảng điều khiển";
            return CanAccessModule(user, ModuleDashboard);
        }

        if (path == "/gianhang/admin/quan-ly-thong-bao/default.aspx")
        {
            deniedLabel = "Thông báo";
            return CanAccessModule(user, ModuleDashboard);
        }

        if (path.StartsWith("/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx") || path.StartsWith("/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx"))
        {
            deniedLabel = "Lịch hẹn";
            return HasAnyPermission(user, ModulePermissions[ModuleBooking]);
        }

        if (path.StartsWith("/gianhang/admin/quan-ly-khach-hang/"))
        {
            deniedLabel = "Khách hàng";
            return HasAnyPermission(user, ModulePermissions[ModuleCustomers]);
        }

        if (path.StartsWith("/gianhang/admin/quan-ly-con-nguoi/"))
        {
            deniedLabel = "Hồ sơ người";
            return CanAccessPeopleHub(user);
        }

        foreach (RoutePermissionRule rule in RouteRules)
        {
            if (path.StartsWith(rule.Prefix))
            {
                deniedLabel = rule.Label;
                return HasAnyPermission(user, rule.Permissions);
            }
        }

        return true;
    }

    private static string[] Flatten(params string[] moduleKeys)
    {
        return moduleKeys
            .Where(key => !string.IsNullOrWhiteSpace(key) && ModulePermissions.ContainsKey(key))
            .SelectMany(key => ModulePermissions[key])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(key => key)
            .ToArray();
    }

    private static string ToJsonArray(IEnumerable<string> values)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        bool isFirst = true;
        foreach (string value in values ?? Enumerable.Empty<string>())
        {
            if (!isFirst)
                sb.Append(",");
            sb.Append("\"").Append(HttpUtility.JavaScriptStringEncode(value ?? "")).Append("\"");
            isFirst = false;
        }
        sb.Append("]");
        return sb.ToString();
    }
}
