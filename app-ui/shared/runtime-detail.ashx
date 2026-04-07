<%@ WebHandler Language="C#" Class="AppUiRuntimeDetailHandler" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

public class AppUiRuntimeDetailHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetNoStore();

        string space = ((context.Request["space"] ?? "home").Trim() ?? "home").ToLowerInvariant();
        string rawId = (context.Request["id"] ?? "").Trim();

        try
        {
            object payload;
            if (space == "batdongsan")
                payload = BuildBatDongSanDetail(rawId);
            else if (space == "choxe")
                payload = BuildVerticalDetail(rawId, "choxe", "Xe", "Danh mục xe");
            else if (space == "dienthoai-maytinh")
                payload = BuildVerticalDetail(rawId, "dienthoai-maytinh", "Công nghệ", "Nhóm thiết bị");
            else if (space == "gianhang")
                payload = BuildGianHangDetail(rawId);
            else
                payload = BuildHomeDetail(rawId);
            context.Response.Write(new JavaScriptSerializer().Serialize(payload));
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "app_ui_runtime_detail", ex.StackTrace);
            context.Response.StatusCode = 500;
            context.Response.Write("{\"ok\":false,\"item\":null}");
        }
    }

    public bool IsReusable { get { return false; } }

    private static object BuildHomeDetail(string rawId)
    {
        int id = ParseRuntimeId(rawId);
        if (id <= 0)
            return new { ok = false, item = (object)null };

        return CoreDb_cl.Use(db =>
        {
            BaiViet_tb row = db.BaiViet_tbs.FirstOrDefault(p => p.id == id);
            if (row == null)
                return new { ok = false, item = (object)null };

            string categoryId = !string.IsNullOrWhiteSpace(row.id_DanhMucCap2) ? row.id_DanhMucCap2 : (row.id_DanhMuc ?? "");
            string categoryName = "";
            if (!string.IsNullOrWhiteSpace(categoryId))
            {
                DanhMuc_tb dm = db.DanhMuc_tbs.FirstOrDefault(p => p.id.ToString() == categoryId);
                categoryName = dm == null ? "" : (dm.name ?? "");
            }

            string locationName = "";
            string locationId = (row.ThanhPho ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(locationId))
            {
                ThanhPho tp = db.ThanhPhos.FirstOrDefault(p => p.id.ToString() == locationId);
                locationName = tp == null ? "" : TinhThanhDisplay_cl.Format(tp.Ten);
            }

            return new
            {
                ok = true,
                item = new
                {
                    id = "runtime-home-" + row.id.ToString(),
                    runtime_id = row.id,
                    runtime_source = "home",
                    title = (row.name ?? "").Trim(),
                    summary = (row.description ?? "").Trim(),
                    image = ResolveImage(row.image),
                    category = string.IsNullOrWhiteSpace(categoryName) ? "Tin đăng" : categoryName,
                    location = locationName,
                    price = row.giaban.HasValue && row.giaban.Value > 0 ? row.giaban.Value.ToString("#,##0") + " đ" : "Liên hệ",
                    meta = row.ngaytao.HasValue ? ("Cập nhật " + row.ngaytao.Value.ToString("dd/MM/yyyy")) : "Tin từ nền tảng hiện tại",
                    badge = "Runtime",
                    detail_url = AhaSearchRoutes_cl.BuildSearchUrl((row.name ?? "").Trim(), categoryId, locationId, locationId, string.Empty, string.Empty, "", "1")
                } as object
            };
        });
    }

    private static object BuildBatDongSanDetail(string rawId)
    {
        int id = ParseRuntimeId(rawId);
        if (id <= 0)
            return new { ok = false, item = (object)null };

        return CoreDb_cl.Use(db =>
        {
            LinkedFeedStore_cl.LinkedPost row = LinkedFeedStore_cl.GetById(db, id);
            if (row == null)
                return new { ok = false, item = (object)null };

            string district = (row.District ?? "").Trim();
            string province = (row.Province ?? "").Trim();
            string location = district != "" && province != "" ? (district + ", " + province) : (district != "" ? district : province);
            string summary = BatDongSanService_cl.SanitizeLinkedText(row.Summary ?? "", 900, true);
            if (string.IsNullOrWhiteSpace(summary))
                summary = (row.Title ?? "").Trim();
            return new
            {
                ok = true,
                item = new
                {
                    id = "runtime-bds-" + row.Id.ToString(),
                    runtime_id = row.Id,
                    runtime_source = "batdongsan",
                    title = row.Title ?? "",
                    summary = summary,
                    image = ResolveImage(row.ThumbnailUrl),
                    category = string.IsNullOrWhiteSpace(row.PropertyType) ? "Bất động sản" : row.PropertyType,
                    location = location,
                    price = string.IsNullOrWhiteSpace(row.PriceText) ? "Liên hệ" : row.PriceText,
                    meta = string.IsNullOrWhiteSpace(row.AreaText) ? "Tin liên kết" : row.AreaText,
                    badge = string.Equals((row.Purpose ?? "").Trim(), "rent", StringComparison.OrdinalIgnoreCase) ? "Cho thuê" : "Mua bán",
                    detail_url = row.SourceUrl ?? "",
                    specs = new object[]
                    {
                        new { label = "Loại hình", value = string.IsNullOrWhiteSpace(row.PropertyType) ? "Bất động sản" : row.PropertyType },
                        new { label = "Diện tích", value = string.IsNullOrWhiteSpace(row.AreaText) ? "--" : row.AreaText },
                        new { label = "Nguồn", value = string.IsNullOrWhiteSpace(row.Source) ? "Liên kết" : row.Source }
                    }
                } as object
            };
        });
    }

    private static object BuildVerticalDetail(string rawId, string spaceCode, string badgeLabel, string categorySpecLabel)
    {
        int id = ParseRuntimeId(rawId);
        if (id <= 0)
            return new { ok = false, item = (object)null };

        return CoreDb_cl.Use(db =>
        {
            BaiViet_tb row = db.BaiViet_tbs.FirstOrDefault(p => p.id == id);
            if (row == null)
                return new { ok = false, item = (object)null };

            string categoryId = !string.IsNullOrWhiteSpace(row.id_DanhMucCap2) ? row.id_DanhMucCap2 : (row.id_DanhMuc ?? "");
            string categoryName = "";
            if (!string.IsNullOrWhiteSpace(categoryId))
            {
                DanhMuc_tb dm = db.DanhMuc_tbs.FirstOrDefault(p => p.id.ToString() == categoryId);
                categoryName = dm == null ? "" : (dm.name ?? "");
            }

            string locationName = "";
            string locationId = (row.ThanhPho ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(locationId))
            {
                ThanhPho tp = db.ThanhPhos.FirstOrDefault(p => p.id.ToString() == locationId);
                locationName = tp == null ? "" : TinhThanhDisplay_cl.Format(tp.Ten);
            }

            return new
            {
                ok = true,
                item = new
                {
                    id = "runtime-" + spaceCode + "-" + row.id.ToString(),
                    runtime_id = row.id,
                    runtime_source = spaceCode,
                    title = (row.name ?? "").Trim(),
                    summary = (row.description ?? "").Trim(),
                    image = ResolveImage(row.image),
                    category = string.IsNullOrWhiteSpace(categoryName) ? badgeLabel : categoryName,
                    location = locationName,
                    price = row.giaban.HasValue && row.giaban.Value > 0 ? row.giaban.Value.ToString("#,##0") + " đ" : "Liên hệ",
                    meta = row.ngaytao.HasValue ? ("Cập nhật " + row.ngaytao.Value.ToString("dd/MM/yyyy")) : "Tin từ nền tảng hiện tại",
                    badge = "Aha " + badgeLabel,
                    detail_url = AhaSearchRoutes_cl.BuildSearchUrl((row.name ?? "").Trim(), categoryId, locationId, locationId, string.Empty, string.Empty, "", "1"),
                    specs = new object[]
                    {
                        new { label = categorySpecLabel, value = string.IsNullOrWhiteSpace(categoryName) ? badgeLabel : categoryName },
                        new { label = "Khu vực", value = string.IsNullOrWhiteSpace(locationName) ? "--" : locationName },
                        new { label = "Nguồn", value = "Nền tảng hiện tại" }
                    }
                } as object
            };
        });
    }

    private static object BuildGianHangDetail(string rawId)
    {
        int id = ParseRuntimeId(rawId);
        if (id <= 0)
            return new { ok = false, item = (object)null };

        RootAccount_cl.RootAccountInfo info = RootAccount_cl.GetCurrentInfo();
        string accountKey = info == null ? "" : (info.AccountKey ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(accountKey))
            return new { ok = false, item = (object)null };

        return CoreDb_cl.Use(db =>
        {
            GH_SanPham_tb post = GianHangProduct_cl.QueryByStorefront(db, accountKey).FirstOrDefault(p => p.id == id);
            if (post == null)
                return new { ok = false, item = (object)null };

            bool isHidden = post.bin == true;
            string status = isHidden ? "Đã ẩn" : "Đang bán";
            string statusTone = isHidden ? "pending" : "live";
            string category = (post.loai ?? "").Trim().ToLowerInvariant() == GianHangProduct_cl.LoaiDichVu ? "Dịch vụ" : "Sản phẩm";
            DateTime? updated = post.ngay_cap_nhat ?? post.ngay_tao;

            List<string> leads = GianHangCustomer_cl.LoadCustomers(db, accountKey, "", 3)
                .Select(c => "Khách " + (string.IsNullOrWhiteSpace(c.DisplayName) ? "mới" : c.DisplayName) + " đang theo dõi tin.")
                .ToList();
            if (leads.Count == 0)
                leads.Add("Chưa có lead mới cho tin này.");

            return new
            {
                ok = true,
                item = new
                {
                    id = "runtime-gh-" + post.id,
                    runtime_id = post.id,
                    runtime_source = "gianhang",
                    title = (post.ten ?? "").Trim(),
                    summary = (post.mo_ta ?? "").Trim(),
                    image = ResolveImage(post.hinh_anh),
                    category = category,
                    location = "",
                    price = post.gia_ban.HasValue && post.gia_ban.Value > 0 ? post.gia_ban.Value.ToString("#,##0") + " đ" : "Liên hệ",
                    meta = updated.HasValue ? ("Cập nhật " + updated.Value.ToString("dd/MM/yyyy HH:mm")) : "Cập nhật --",
                    badge = status,
                    status = status,
                    statusTone = statusTone,
                    stat = "Lượt xem " + (post.luot_truy_cap ?? 0).ToString("#,##0"),
                    updatedAt = updated.HasValue ? ("Cập nhật " + updated.Value.ToString("dd/MM/yyyy HH:mm")) : "Cập nhật --",
                    publishTargets = post.id_baiviet.HasValue ? new[] { "Gian hàng", "Home" } : new[] { "Gian hàng" },
                    reviewNotes = isHidden ? new[] { "Tin đang ở trạng thái ẩn." } : new[] { "Tin đang hoạt động bình thường." },
                    leads = leads,
                    quickActions = new[] { "Sửa nội dung", "Đẩy tin", isHidden ? "Hiển thị lại tin" : "Tạm ẩn tin" },
                    publishHistory = new[] { "Đồng bộ từ gian hàng runtime." },
                    checklist = new[] { "Kiểm tra tiêu đề", "Kiểm tra giá", "Theo dõi lead" }
                } as object
            };
        });
    }

    private static int ParseRuntimeId(string rawId)
    {
        string value = (rawId ?? "").Trim();
        if (value.StartsWith("runtime-home-", StringComparison.OrdinalIgnoreCase))
            value = value.Substring("runtime-home-".Length);
        if (value.StartsWith("runtime-bds-", StringComparison.OrdinalIgnoreCase))
            value = value.Substring("runtime-bds-".Length);
        if (value.StartsWith("runtime-choxe-", StringComparison.OrdinalIgnoreCase))
            value = value.Substring("runtime-choxe-".Length);
        if (value.StartsWith("runtime-dienthoai-maytinh-", StringComparison.OrdinalIgnoreCase))
            value = value.Substring("runtime-dienthoai-maytinh-".Length);
        if (value.StartsWith("runtime-gh-", StringComparison.OrdinalIgnoreCase))
            value = value.Substring("runtime-gh-".Length);
        int id;
        return int.TryParse(value, out id) ? id : 0;
    }

    private static string ResolveImage(string raw)
    {
        string value = (raw ?? "").Trim();
        if (string.IsNullOrWhiteSpace(value))
            return "/app-ui/assets/placeholder-media.svg";
        if (value.StartsWith("~/", StringComparison.Ordinal))
            value = value.Substring(1);
        if (!value.StartsWith("/", StringComparison.Ordinal) && !value.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            value = "/" + value;
        return value;
    }
}
