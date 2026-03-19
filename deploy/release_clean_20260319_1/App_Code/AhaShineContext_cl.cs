using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

public static class AhaShineContext_cl
{
    public static string UserParent
    {
        get { return ResolveUserParent(); }
    }

    public static string ResolveShopAccount()
    {
        try
        {
            return (GianHangAuth_cl.ResolveShopAccount() ?? "").Trim().ToLowerInvariant();
        }
        catch
        {
            return "";
        }
    }

    public static string ResolveUserParent()
    {
        string fromSession = HttpContext.Current != null ? (HttpContext.Current.Session["user_parent"] as string) : null;
        if (!string.IsNullOrWhiteSpace(fromSession))
            return fromSession.Trim().ToLowerInvariant();

        string shop = ResolveShopAccount();
        if (!string.IsNullOrWhiteSpace(shop))
            return shop;

        return "admin";
    }

    public static string ResolveChiNhanhId()
    {
        var ss = HttpContext.Current != null ? HttpContext.Current.Session : null;
        if (ss == null)
            return "1";
        return (ss["id_chinhanh_webcon"] ?? ss["chinhanh"] ?? "1").ToString();
    }

    public static string ResolveChiNhanhIdForShopAccount(dbDataContext db, string taiKhoan, string fallback = null)
    {
        string explicitFallback = (fallback ?? "").Trim();
        if (!string.IsNullOrWhiteSpace(explicitFallback))
            return explicitFallback;

        string shopAccount = (taiKhoan ?? "").Trim().ToLowerInvariant();
        if (db != null && !string.IsNullOrWhiteSpace(shopAccount))
        {
            taikhoan_table_2023 directAccount = db.taikhoan_table_2023s.FirstOrDefault(p => p.taikhoan == shopAccount);
            if (directAccount != null && !string.IsNullOrWhiteSpace(directAccount.id_chinhanh))
                return directAccount.id_chinhanh.Trim();

            taikhoan_table_2023 childAccount = db.taikhoan_table_2023s
                .FirstOrDefault(p => p.user_parent == shopAccount && p.id_chinhanh != null && p.id_chinhanh != "");
            if (childAccount != null && !string.IsNullOrWhiteSpace(childAccount.id_chinhanh))
                return childAccount.id_chinhanh.Trim();

            chinhanh_table chiNhanh = db.chinhanh_tables.FirstOrDefault(p => p.taikhoan_quantri == shopAccount);
            if (chiNhanh != null)
                return chiNhanh.id.ToString();
        }

        string sessionFallback = ResolveChiNhanhId();
        return string.IsNullOrWhiteSpace(sessionFallback) ? "1" : sessionFallback.Trim();
    }

    public static taikhoan_table_2023 EnsureAdvancedAdminBootstrapForShop(dbDataContext db, string shopAccount)
    {
        string userParent = (shopAccount ?? "").Trim().ToLowerInvariant();
        if (db == null || string.IsNullOrWhiteSpace(userParent))
            return null;

        chinhanh_table chinhanh = EnsureChiNhanh(db, userParent);
        nganh_table nganh = EnsureNganh(db, chinhanh);
        EnsureDefaultMenus(db, chinhanh.id.ToString());
        EnsureDefaultGiangVien(db, userParent, chinhanh, nganh);
        EnsureCaiDatChung(db, userParent);
        return EnsureAdminAccount(db, userParent, chinhanh, nganh);
    }

    public static void EnsureContext()
    {
        dbDataContext db = new dbDataContext();
        AhaShineSchema_cl.EnsureSchemaSafe(db);
        GianHangStorefrontSchema_cl.EnsureSafe(db);
        EnsureConfigDefaults(db);

        string manual = "";
        try
        {
            HttpContext ctx = HttpContext.Current;
            if (ctx != null && ctx.Request != null && ctx.Request.QueryString != null)
                manual = ctx.Request.QueryString["manual"] ?? "";
        }
        catch
        {
            manual = "";
        }
        if (string.Equals(manual, "1", StringComparison.OrdinalIgnoreCase))
            return;

        EnsureSession(db);
    }

    public static bool TryEnsureContext(out string warningMessage)
    {
        warningMessage = "";
        try
        {
            SqlTransientGuard_cl.Execute(() =>
            {
                EnsureDatabaseAvailabilityProbe();
                EnsureContext();
            }, 3, 250);
            return true;
        }
        catch (Exception ex)
        {
            if (!SqlTransientGuard_cl.IsTransient(ex))
                throw;

            warningMessage = BuildTransientWarningMessage();
            return false;
        }
    }

    public static void EnsureSchemaAndDefaults()
    {
        dbDataContext db = new dbDataContext();
        AhaShineSchema_cl.EnsureSchemaSafe(db);
        GianHangStorefrontSchema_cl.EnsureSafe(db);
        EnsureConfigDefaults(db);

        string userParent = ResolveUserParent();
        chinhanh_table chinhanh = EnsureChiNhanh(db, userParent);
        nganh_table nganh = EnsureNganh(db, chinhanh);
        EnsureAdminAccount(db, userParent, chinhanh, nganh);
        EnsureDefaultMenus(db, chinhanh.id.ToString());
        EnsureDefaultGiangVien(db, userParent, chinhanh, nganh);
        EnsureCaiDatChung(db, userParent);
    }

    public static bool TryEnsureSchemaAndDefaults(out string warningMessage)
    {
        warningMessage = "";
        try
        {
            SqlTransientGuard_cl.Execute(() =>
            {
                EnsureDatabaseAvailabilityProbe();
                EnsureSchemaAndDefaults();
            }, 3, 250);
            return true;
        }
        catch (Exception ex)
        {
            if (!SqlTransientGuard_cl.IsTransient(ex))
                throw;

            warningMessage = BuildTransientWarningMessage();
            return false;
        }
    }

    public static bool TryEnsureAuthReady(out string warningMessage)
    {
        warningMessage = "";
        try
        {
            SqlTransientGuard_cl.Execute(() => EnsureDatabaseAvailabilityProbe(), 3, 250);
            return true;
        }
        catch (Exception ex)
        {
            if (!SqlTransientGuard_cl.IsTransient(ex))
                throw;

            warningMessage = BuildTransientWarningMessage();
            return false;
        }
    }

    public static string BuildTransientWarningMessage()
    {
        return "Kết nối dữ liệu đang tạm thời gián đoạn. Vui lòng tải lại trang sau vài giây.";
    }

    private static void EnsureDatabaseAvailabilityProbe()
    {
        using (dbDataContext probe = new dbDataContext())
        {
            SqlConnection sqlConnection = probe.Connection as SqlConnection;
            if (sqlConnection != null)
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(sqlConnection.ConnectionString ?? "");
                if (builder.ConnectTimeout <= 0 || builder.ConnectTimeout > 2)
                    builder.ConnectTimeout = 2;
                builder.Pooling = false;
                sqlConnection.ConnectionString = builder.ConnectionString;
            }

            if (probe.Connection.State != ConnectionState.Open)
                probe.Connection.Open();

            probe.Connection.Close();
        }
    }

    private static void EnsureSession(dbDataContext db)
    {
        if (db == null)
            return;

        HttpContext ctx = HttpContext.Current;
        if (ctx == null)
            return;

        string sessionUser = (ctx.Session["user"] as string ?? "").Trim().ToLowerInvariant();
        string userParent = ResolveUserParent();

        chinhanh_table chinhanh = EnsureChiNhanh(db, userParent);
        nganh_table nganh = EnsureNganh(db, chinhanh);
        EnsureDefaultMenus(db, chinhanh.id.ToString());
        EnsureDefaultGiangVien(db, userParent, chinhanh, nganh);

        // If session user exists and is valid, align session data
        if (!string.IsNullOrEmpty(sessionUser))
        {
            taikhoan_table_2023 acc = db.taikhoan_table_2023s.FirstOrDefault(p => p.taikhoan == sessionUser);
            if (acc != null)
            {
                if (!string.IsNullOrWhiteSpace(acc.user_parent))
                    userParent = acc.user_parent.Trim().ToLowerInvariant();

                if (string.IsNullOrWhiteSpace(ctx.Session["user_parent"] as string))
                    ctx.Session["user_parent"] = userParent;

                if (string.IsNullOrWhiteSpace(ctx.Session["chinhanh"] as string))
                    ctx.Session["chinhanh"] = string.IsNullOrWhiteSpace(acc.id_chinhanh) ? chinhanh.id.ToString() : acc.id_chinhanh;

                if (string.IsNullOrWhiteSpace(ctx.Session["nganh"] as string))
                    ctx.Session["nganh"] = string.IsNullOrWhiteSpace(acc.id_nganh) ? nganh.id.ToString() : acc.id_nganh;

                return;
            }
        }

        // Auto-bootstrap admin account for shop owner
        taikhoan_table_2023 adminAcc = EnsureAdminAccount(db, userParent, chinhanh, nganh);

        ctx.Session["user"] = adminAcc.taikhoan;
        ctx.Session["user_parent"] = userParent;
        ctx.Session["chinhanh"] = adminAcc.id_chinhanh ?? chinhanh.id.ToString();
        ctx.Session["nganh"] = adminAcc.id_nganh ?? nganh.id.ToString();
    }

    private static chinhanh_table EnsureChiNhanh(dbDataContext db, string userParent)
    {
        string key = (userParent ?? "").Trim().ToLowerInvariant();
        chinhanh_table cn = db.chinhanh_tables.FirstOrDefault(p => (p.taikhoan_quantri ?? "").Trim().ToLower() == key);
        if (cn != null)
            return cn;

        cn = new chinhanh_table
        {
            ten = string.IsNullOrWhiteSpace(userParent) ? "Chi nhánh chính" : ("Chi nhánh " + userParent),
            taikhoan_quantri = userParent,
            diachi = "",
            email = "",
            sdt = "",
            slogan = "",
            logo_footer = "",
            logo_hoadon = ""
        };
        db.chinhanh_tables.InsertOnSubmit(cn);
        db.SubmitChanges();
        return cn;
    }

    private static nganh_table EnsureNganh(dbDataContext db, chinhanh_table chinhanh)
    {
        string chinhanhId = chinhanh.id.ToString();
        nganh_table nganh = db.nganh_tables.FirstOrDefault(p => p.id_chinhanh == chinhanhId);
        if (nganh != null)
            return nganh;

        nganh = new nganh_table
        {
            ten = "Ngành chính",
            id_chinhanh = chinhanhId
        };
        db.nganh_tables.InsertOnSubmit(nganh);
        db.SubmitChanges();
        return nganh;
    }

    private static taikhoan_table_2023 EnsureAdminAccount(dbDataContext db, string userParent, chinhanh_table chinhanh, nganh_table nganh)
    {
        string tk = (userParent ?? "admin").Trim().ToLowerInvariant();
        taikhoan_table_2023 acc = db.taikhoan_table_2023s.FirstOrDefault(p => p.taikhoan == tk);
        if (acc != null)
            return acc;

        acc = new taikhoan_table_2023
        {
            taikhoan = tk,
            matkhau = encode_class.encode_md5(encode_class.encode_sha1("123456")),
            hoten = string.IsNullOrWhiteSpace(userParent) ? "Quản trị" : ("Quản trị " + userParent),
            hoten_khongdau = string.IsNullOrWhiteSpace(userParent) ? "quantri" : ("quantri " + userParent).Replace(" ", ""),
            anhdaidien = "/uploads/images/icon-user.png",
            ngaysinh = DateTime.Now.Date,
            nguoitao = tk,
            ngaytao = DateTime.Now,
            trangthai = "Đang hoạt động",
            email = "",
            dienthoai = "",
            permission = "all",
            user_parent = userParent,
            id_chinhanh = chinhanh.id.ToString(),
            id_nganh = nganh.id.ToString(),
            luongcoban = 0,
            songaycong = 0,
            luongngay = 0
        };
        db.taikhoan_table_2023s.InsertOnSubmit(acc);
        db.SubmitChanges();

        EnsureCaiDatChung(db, userParent);
        return acc;
    }

    private static void EnsureDefaultMenus(dbDataContext db, string chinhanhId)
    {
        EnsureMenu(db, chinhanhId, 550, "Sản phẩm", "san-pham", "dssp");
        EnsureMenu(db, chinhanhId, 551, "Dịch vụ", "dich-vu", "dsdv");
        EnsureMenu(db, chinhanhId, 577, "Bài viết", "bai-viet", "dsbv");
    }

    private static void EnsureDefaultGiangVien(dbDataContext db, string userParent, chinhanh_table chinhanh, nganh_table nganh)
    {
        if (db == null || chinhanh == null || nganh == null)
            return;

        string chinhanhId = chinhanh.id.ToString();
        string nganhId = nganh.id.ToString();
        giangvien_table existing = db.giangvien_tables.FirstOrDefault(
            p => p.id_chinhanh == chinhanhId
                && p.chuyenmon == nganhId
                && (p.trangthai ?? "") == "Đang giảng dạy");
        if (existing != null)
            return;

        giangvien_table teacher = new giangvien_table
        {
            hoten = "Giảng viên mặc định",
            hoten_khongdau = "giangvienmacdinh",
            anhdaidien = "/uploads/images/icon-user.png",
            ngaysinh = DateTime.Now.Date,
            email = "",
            dienthoai = "",
            zalo = "",
            facebook = "",
            chuyenmon = nganhId,
            sobuoi_lythuyet = 0,
            sobuoi_thuchanh = 0,
            sobuoi_trogiang = 0,
            goidaotao = "",
            danhgiagiangvien = "",
            nguoitao = string.IsNullOrWhiteSpace(userParent) ? "admin" : userParent,
            ngaytao = DateTime.Now,
            trangthai = "Đang giảng dạy",
            id_chinhanh = chinhanhId
        };
        db.giangvien_tables.InsertOnSubmit(teacher);
        db.SubmitChanges();
    }

    private static void EnsureMenu(dbDataContext db, string chinhanhId, long menuId, string name, string slug, string phanloai)
    {
        web_menu_table existing = db.web_menu_tables.FirstOrDefault(p => p.id == menuId);
        if (existing != null)
        {
            bool changed = false;
            if (!string.Equals(existing.phanloai ?? "", phanloai ?? "", StringComparison.OrdinalIgnoreCase))
            {
                existing.phanloai = phanloai;
                changed = true;
            }
            if (string.IsNullOrWhiteSpace(existing.id_chinhanh))
            {
                existing.id_chinhanh = chinhanhId;
                changed = true;
            }
            if (changed)
                db.SubmitChanges();
            return;
        }

        db.ExecuteCommand(@"
SET IDENTITY_INSERT dbo.web_menu_table ON;
INSERT INTO dbo.web_menu_table (id, name, id_parent, id_level, name_en, description, image, rank, url_other, ngaytao, bin, phanloai, id_chinhanh)
VALUES ({0}, {1}, '0', 1, {2}, '', '', 1, '', GETDATE(), 0, {3}, {4});
SET IDENTITY_INSERT dbo.web_menu_table OFF;
", menuId, name, slug, phanloai, chinhanhId);
    }

    private static void EnsureCaiDatChung(dbDataContext db, string userParent)
    {
        if (db.bspa_caidatchung_tables.Any(p => p.user_parent == userParent))
            return;

        bspa_caidatchung_table cd = new bspa_caidatchung_table
        {
            user_parent = userParent,
            chitieu_doanhso_dichvu = 0,
            chitieu_doanhso_mypham = 0
        };
        db.bspa_caidatchung_tables.InsertOnSubmit(cd);
        db.SubmitChanges();
    }

    private static void EnsureConfigDefaults(dbDataContext db)
    {
        if (!db.config_baotri_tables.Any())
        {
            db.config_baotri_tables.InsertOnSubmit(new config_baotri_table
            {
                baotri_trangthai = false,
                baotri_thoigian_batdau = null,
                baotri_thoigian_ketthuc = null,
                ghichu = ""
            });
            db.SubmitChanges();
        }

        if (!db.config_lienket_chiase_tables.Any())
        {
            db.config_lienket_chiase_tables.InsertOnSubmit(new config_lienket_chiase_table
            {
                title = "",
                description = "",
                image = ""
            });
            db.SubmitChanges();
        }

        if (!db.config_nhungma_tables.Any())
        {
            db.config_nhungma_tables.InsertOnSubmit(new config_nhungma_table
            {
                nhungma_head = "",
                nhungma_body1 = "",
                nhungma_body2 = "",
                nhungma_fanpage = "",
                nhungma_googlemaps = ""
            });
            db.SubmitChanges();
        }

        if (!db.config_social_media_tables.Any())
        {
            db.config_social_media_tables.InsertOnSubmit(new config_social_media_table
            {
                facebook = "",
                instagram = "",
                tiktok = "",
                youtube = "",
                twitter = ""
            });
            db.SubmitChanges();
        }

        if (!db.config_thongtin_tables.Any())
        {
            db.config_thongtin_tables.InsertOnSubmit(new config_thongtin_table
            {
                icon = "/uploads/images/favicon.png",
                apple_touch_icon = "/uploads/images/icon-mobile.jpg",
                logo = "/uploads/images/logo.png",
                logo1 = "/uploads/images/logo.png",
                tencongty = "Gian hàng đối tác",
                slogan = "",
                diachi = "",
                link_googlemap = "",
                hotline = "",
                email = "",
                masothue = "",
                zalo = "",
                logo_in_hoadon = ""
            });
            db.SubmitChanges();
        }

        GianHangStorefrontConfig_cl.EnsureDefaults(db, ResolveChiNhanhId());
    }
}
