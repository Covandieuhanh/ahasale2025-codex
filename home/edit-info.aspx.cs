using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_edit_info : System.Web.UI.Page
{
    private string ResolveProfileAddress()
    {
        string tinh = (hf_profile_tinh.Value ?? "").Trim();
        string quan = (hf_profile_quan.Value ?? "").Trim();
        string phuong = (hf_profile_phuong.Value ?? "").Trim();
        string chiTiet = (txt_diachi_chitiet.Text ?? "").Trim();
        string raw = (hf_profile_raw.Value ?? "").Trim();

        bool hasInput = !string.IsNullOrEmpty(chiTiet) || !string.IsNullOrEmpty(tinh) || !string.IsNullOrEmpty(quan) || !string.IsNullOrEmpty(phuong);
        if (!hasInput)
            return raw;

        string full = AddressFormat_cl.BuildFullAddress(chiTiet, phuong, quan, tinh);
        txt_diachi.Text = full;
        return full;
    }

    private string ResolveShopAddress()
    {
        string tinh = (hf_shop_tinh.Value ?? "").Trim();
        string quan = (hf_shop_quan.Value ?? "").Trim();
        string phuong = (hf_shop_phuong.Value ?? "").Trim();
        string chiTiet = (txt_diachi_shop_chitiet.Text ?? "").Trim();
        string raw = (hf_shop_raw.Value ?? "").Trim();

        bool hasInput = !string.IsNullOrEmpty(chiTiet) || !string.IsNullOrEmpty(tinh) || !string.IsNullOrEmpty(quan) || !string.IsNullOrEmpty(phuong);
        if (!hasInput)
            return raw;

        string full = AddressFormat_cl.BuildFullAddress(chiTiet, phuong, quan, tinh);
        TextBox4.Text = full;
        return full;
    }

    private static string NormalizeSocialPreset(string value)
    {
        string preset = (value ?? "").Trim().ToLowerInvariant();
        switch (preset)
        {
            case "facebook":
            case "zalo":
            case "tiktok":
            case "youtube":
            case "instagram":
            case "telegram":
            case "shopee":
            case "website":
                return preset;
            default:
                return "";
        }
    }

    private static string StripSocialPrefix(string value, params string[] prefixes)
    {
        string result = (value ?? "").Trim();
        foreach (string prefix in prefixes)
        {
            if (result.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                result = result.Substring(prefix.Length);
            }
        }

        return result.Trim().TrimStart('/');
    }

    private static string BuildPresetSocialLink(string presetValue, string rawValue)
    {
        string raw = (rawValue ?? "").Trim();
        if (raw.Length == 0)
            return "";

        string normalized = SocialLinkIcon_cl.NormalizeExternalLink(raw);
        if (!string.IsNullOrEmpty(normalized))
            return normalized;

        string preset = NormalizeSocialPreset(presetValue);
        string candidate = "";
        string value = raw.Trim().TrimStart('/');

        switch (preset)
        {
            case "facebook":
                value = StripSocialPrefix(value, "www.facebook.com/", "facebook.com/");
                value = value.TrimStart('@');
                candidate = "https://www.facebook.com/" + value;
                break;

            case "zalo":
                value = StripSocialPrefix(value, "zalo.me/", "chat.zalo.me/", "zaloapp.com/qr/p/");
                string digits = new string(value.Where(char.IsDigit).ToArray());
                candidate = "https://zalo.me/" + (string.IsNullOrEmpty(digits) ? value : digits);
                break;

            case "tiktok":
                value = StripSocialPrefix(value, "www.tiktok.com/", "tiktok.com/");
                if (!value.StartsWith("@", StringComparison.OrdinalIgnoreCase) && value.IndexOf('/') < 0)
                    value = "@" + value;
                candidate = "https://www.tiktok.com/" + value;
                break;

            case "youtube":
                value = StripSocialPrefix(value, "www.youtube.com/", "youtube.com/", "youtu.be/");
                if (value.StartsWith("@", StringComparison.OrdinalIgnoreCase)
                    || value.StartsWith("channel/", StringComparison.OrdinalIgnoreCase)
                    || value.StartsWith("c/", StringComparison.OrdinalIgnoreCase)
                    || value.StartsWith("user/", StringComparison.OrdinalIgnoreCase))
                {
                    candidate = "https://www.youtube.com/" + value;
                }
                else
                {
                    candidate = "https://www.youtube.com/@" + value.TrimStart('@');
                }
                break;

            case "instagram":
                value = StripSocialPrefix(value, "www.instagram.com/", "instagram.com/");
                candidate = "https://www.instagram.com/" + value.TrimStart('@');
                break;

            case "telegram":
                value = StripSocialPrefix(value, "t.me/", "telegram.me/");
                candidate = "https://t.me/" + value.TrimStart('@');
                break;

            case "shopee":
                value = StripSocialPrefix(value, "www.shopee.vn/", "shopee.vn/");
                if (value.All(char.IsDigit))
                    value = "shop/" + value;
                candidate = "https://shopee.vn/" + value;
                break;

            case "website":
                candidate = "https://" + raw.TrimStart('/');
                break;

            default:
                if (raw.Contains("."))
                    candidate = "https://" + raw.TrimStart('/');
                break;
        }

        return SocialLinkIcon_cl.NormalizeExternalLink(candidate);
    }

    private static string DetectSocialPreset(string link)
    {
        string normalized = SocialLinkIcon_cl.NormalizeExternalLink(link);
        if (string.IsNullOrEmpty(normalized))
            return "";

        Uri uri;
        if (!Uri.TryCreate(normalized, UriKind.Absolute, out uri))
            return "";

        string host = (uri.Host ?? "").Trim().ToLowerInvariant();
        if (host.Contains("facebook.com") || host == "fb.com" || host.EndsWith(".fb.com"))
            return "facebook";
        if (host.Contains("zalo.me") || host.Contains("zalo.vn") || host.Contains("chat.zalo"))
            return "zalo";
        if (host.Contains("tiktok.com") || host == "vt.tiktok.com")
            return "tiktok";
        if (host.Contains("youtube.com") || host == "youtu.be")
            return "youtube";
        if (host.Contains("instagram.com"))
            return "instagram";
        if (host == "t.me" || host.Contains("telegram.me"))
            return "telegram";
        if (host.Contains("shopee.vn") || host.Contains("shopee.com"))
            return "shopee";

        return "";
    }

    private string ResolveDefaultSocialLinkTitle(taikhoan_tb account)
    {
        bool isShop = string.Equals((txtKieu.Text ?? "").Trim(), "Cửa hàng", StringComparison.OrdinalIgnoreCase);
        string titleFromForm = isShop ? (TextBox2.Text ?? "").Trim() : (txt_hoten.Text ?? "").Trim();
        if (!string.IsNullOrEmpty(titleFromForm))
            return titleFromForm;

        if (account != null)
        {
            string titleFromAccount = isShop ? (account.ten_shop ?? "").Trim() : (account.hoten ?? "").Trim();
            if (!string.IsNullOrEmpty(titleFromAccount))
                return titleFromAccount;

            if (!string.IsNullOrEmpty(account.taikhoan))
                return account.taikhoan;
        }

        return "";
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_home("none", "none", false);

            string _tk = PortalRequest_cl.GetCurrentAccountEncrypted();
            if (string.IsNullOrWhiteSpace(_tk))
            {
                Response.Redirect(PortalRequest_cl.IsShopPortalRequest() ? "/shop/login.aspx" : "/");
                return;
            }

            using (dbDataContext db = new dbDataContext())
            {
                ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk);
                show_main(db);
            }
        }
    }

    // ✅ LOCK nếu: không thuộc scope shop
    private bool IsShopLocked(dbDataContext db, string tk)
    {
        if (string.IsNullOrEmpty(tk)) return true;

        var user = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == tk);
        if (user == null) return true;

        if (!SpaceAccess_cl.CanAccessShop(db, user))
            return true;

        return false;
    }

    private bool ShouldPreferShopTab()
    {
        if (PortalRequest_cl.IsShopPortalRequest())
            return true;

        string raw = (Request.QueryString["shop_portal"] ?? "").Trim();
        if (raw == "1" || raw.Equals("true", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    protected bool PreferShopTab
    {
        get { return (ViewState["prefer_shop_tab"] as bool?) ?? false; }
    }

    private bool ShouldManageStorefrontInGianHangOnly()
    {
        return true;
    }

    protected string GetTabClass(bool isShop)
    {
        bool active = PreferShopTab == isShop;
        return active ? "nav-link active" : "nav-link";
    }

    protected string GetPaneClass(bool isShop)
    {
        bool active = PreferShopTab == isShop;
        return active ? "tab-pane active show" : "tab-pane";
    }

    protected string GetAriaSelected(bool isShop)
    {
        bool active = PreferShopTab == isShop;
        return active ? "true" : "false";
    }

    public void show_main(dbDataContext db)
    {
        string tk = Convert.ToString(ViewState["taikhoan"]) ?? "";
        var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
        if (q_tk == null)
        {
            Response.Redirect(PortalRequest_cl.IsShopPortalRequest() ? "/shop/default.aspx" : "/");
            return;
        }

        // ✅ xác định lock shop
        bool lockShop = IsShopLocked(db, tk) || ShouldManageStorefrontInGianHangOnly();
        ViewState["lock_shop"] = lockShop;

        // Gian hàng được chỉnh riêng trong /gianhang/tai-khoan/default.aspx
        // để không còn đè dữ liệu storefront từ hồ sơ Home.
        phTabCuaHang.Visible = !lockShop;
        phShopEdit.Visible = !lockShop;
        phShopLockedNote.Visible = lockShop;

        bool preferShopTab = ShouldPreferShopTab();
        if (lockShop) preferShopTab = false;
        ViewState["prefer_shop_tab"] = preferShopTab;

        string metaTags = string.Format(@"
            <!-- Title -->
            <title>Chỉnh sửa thông tin {0}</title>
        ", q_tk.hoten);
        literal_meta.Text = metaTags;

        ViewState["avt_query"] = q_tk.anhdaidien;
        ViewState["hoten_query"] = q_tk.hoten;
        ViewState["sdt_query"] = q_tk.dienthoai;
        ViewState["DongA_query"] = (q_tk.DongA ?? 0m).ToString("#,##0");

        if (q_tk.phanloai == "Gian hàng đối tác")
            ViewState["phanloai_query"] = "<span class=\"badge bg-yellow-lt\">Gian hàng đối tác</span>";
        else if (q_tk.phanloai == "Đồng hành hệ sinh thái")
            ViewState["phanloai_query"] = "<span class=\"badge bg-red-lt\">Đồng hành hệ sinh thái</span>";
        else if (q_tk.phanloai == "Khách hàng")
            ViewState["phanloai_query"] = "<span class=\"badge bg-green-lt\">Khách hàng</span>";

        #region cá nhân
        txt_link_fileupload.Text = q_tk.anhdaidien;
        if (!string.IsNullOrEmpty(q_tk.anhdaidien))
        {
            Button2.Visible = true;
            Label2.Text = "<div><small class='text-muted'>Ảnh cũ</small></div><img src='" + q_tk.anhdaidien + "' style='max-width: 100px;border-radius:10px' />";
        }
        else
        {
            Button2.Visible = false;
            Label2.Text = "";
        }

        txt_hoten.Text = q_tk.hoten;
        txt_sdt.Text = q_tk.dienthoai;
        TextBox9.Text = q_tk.email;
        string diaChiCaNhan = q_tk.diachi ?? "";
        txt_diachi.Text = diaChiCaNhan;
        txt_diachi_chitiet.Text = diaChiCaNhan;
        hf_profile_raw.Value = diaChiCaNhan;
        txt_gioithieu.Text = q_tk.gioithieu;

        bool isShopAccount = ShopSlug_cl.IsShopAccount(db, q_tk);
        phProfileShopToggle.Visible = isShopAccount;

        var profileSettings = HomeProfileSetting_cl.GetSettings(db, q_tk.taikhoan, isShopAccount);
        if (ddl_profile_template.Items.FindByValue(profileSettings.TemplateKey) != null)
            ddl_profile_template.SelectedValue = profileSettings.TemplateKey;
        hf_profile_accent.Value = string.IsNullOrWhiteSpace(profileSettings.AccentColor) ? "#22c55e" : profileSettings.AccentColor;
        chk_profile_contact.Checked = profileSettings.ShowContact;
        chk_profile_social.Checked = profileSettings.ShowSocial;
        chk_profile_reviews.Checked = profileSettings.ShowReviews;
        if (chk_profile_shop != null) chk_profile_shop.Checked = profileSettings.ShowShop;
        if (chk_profile_products != null) chk_profile_products.Checked = profileSettings.ShowProducts;

        var link = db.MangXaHoi_tbs.Where(x => x.TaiKhoan.Equals(q_tk.taikhoan)).ToList();
        rptMangXaHoiCN.DataSource = SocialLinkOrder_cl.SortLinks(link.Where(x => x.Kieu == "Cá nhân"), profileSettings.SocialOrderPersonal);
        rptMangXaHoiCH.DataSource = SocialLinkOrder_cl.SortLinks(link.Where(x => x.Kieu == "Cửa hàng"), profileSettings.SocialOrderShop);
        rptMangXaHoiCN.DataBind();
        rptMangXaHoiCH.DataBind();

        // left summary card removed -> no Literal4/Literal5
        #endregion

        #region thông tin cửa hàng (chỉ load nếu KHÔNG khóa - để khỏi “vô tình” hiển thị)
        if (!lockShop)
        {
            txt_link_fileupload1.Text = q_tk.logo_shop;
            if (!string.IsNullOrEmpty(q_tk.logo_shop))
            {
                Button1.Visible = true;
                Label3.Text = "<div><small class='text-muted'>Ảnh cũ</small></div><img src='" + q_tk.logo_shop + "' style='max-width: 100px;border-radius:10px' />";
            }
            else
            {
                Button1.Visible = false;
                Label3.Text = "";
            }

            txt_link_fileupload2.Text = q_tk.anhbia_shop;
            if (!string.IsNullOrEmpty(q_tk.anhbia_shop))
            {
                Button3.Visible = true;
                Label4.Text = "<div><small class='text-muted'>Ảnh cũ</small></div><img src='" + q_tk.anhbia_shop + "' style='max-width: 100px;border-radius:10px' />";
            }
            else
            {
                Button3.Visible = false;
                Label4.Text = "";
            }

            TextBox2.Text = q_tk.ten_shop;
            TextBox3.Text = q_tk.sdt_shop;
            TextBox10.Text = q_tk.email_shop;
            TextBox5.Text = q_tk.motangan_shop;
            string diaChiShop = q_tk.diachi_shop ?? "";
            TextBox4.Text = diaChiShop;
            txt_diachi_shop_chitiet.Text = diaChiShop;
            hf_shop_raw.Value = diaChiShop;

            // ===== tiến độ hoàn thiện hồ sơ shop =====
            int totalFields = 5;
            int doneFields = 0;
            var missing = new List<string>();

            if (!string.IsNullOrWhiteSpace(q_tk.ten_shop)) doneFields++; else missing.Add("Tên gian hàng đối tác");
            if (!string.IsNullOrWhiteSpace(q_tk.logo_shop)) doneFields++; else missing.Add("Logo");
            if (!string.IsNullOrWhiteSpace(q_tk.anhbia_shop)) doneFields++; else missing.Add("Ảnh bìa");
            if (!string.IsNullOrWhiteSpace(q_tk.motangan_shop)) doneFields++; else missing.Add("Mô tả ngắn");
            if (!string.IsNullOrWhiteSpace(q_tk.diachi_shop)) doneFields++; else missing.Add("Địa chỉ");

            int percent = (int)Math.Round(doneFields * 100.0 / totalFields);
            lb_shop_completion.Text = percent.ToString() + "%";
            lb_shop_completion_note.Text = (missing.Count == 0)
                ? "Hồ sơ gian hàng đối tác đã đầy đủ."
                : ("Thiếu: " + string.Join(", ", missing));

            hl_shop_public.NavigateUrl = ShopSlug_cl.GetPublicUrl(db, q_tk);
            hl_shop_public.Visible = true;
        }
        else
        {
            // nếu lock: ẩn luôn nút xóa ảnh shop
            Button1.Visible = false;
            Button3.Visible = false;
            hl_shop_public.Visible = false;
            lb_shop_completion.Text = "0%";
            lb_shop_completion_note.Text = "Không thể chỉnh vì tài khoản không thuộc phạm vi gian hàng đối tác.";
        }
        #endregion
    }

    protected void Button2_Click(object sender, EventArgs e) // xóa ảnh đại diện
    {
        using (dbDataContext db = new dbDataContext())
        {
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["taikhoan"].ToString());
            if (q != null)
            {
                taikhoan_tb _ob = q;
                File_Folder_cl.del_file(_ob.anhdaidien);
                _ob.anhdaidien = "/uploads/images/macdinh.jpg";
                Button2.Visible = false;
                db.SubmitChanges();

                Label2.Text = "";
                txt_link_fileupload.Text = "";

                Helper_Tabler_cl.ShowToast(this.Page, "Xóa ảnh thành công.", null, true, 2000, "Thông báo");
            }
        }
    }

    protected void Button1_Click(object sender, EventArgs e) // xóa logo shop
    {
        // ✅ nếu bị khóa shop thì không cho
        bool lockShop = (ViewState["lock_shop"] as bool?) ?? false;
        if (lockShop)
        {
            Helper_Tabler_cl.ShowToast(this.Page, "Bạn không thể chỉnh sửa cửa hàng lúc này.", "warning", true, 2500, "Thông báo");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["taikhoan"].ToString());
            if (q != null)
            {
                taikhoan_tb _ob = q;
                File_Folder_cl.del_file(_ob.logo_shop);
                _ob.logo_shop = "";
                Button1.Visible = false;
                db.SubmitChanges();

                Label3.Text = "";
                txt_link_fileupload1.Text = "";

                Helper_Tabler_cl.ShowToast(this.Page, "Xóa ảnh thành công.", null, true, 2000, "Thông báo");
            }
        }
    }

    protected void Button3_Click(object sender, EventArgs e) // xóa ảnh bìa shop
    {
        // ✅ nếu bị khóa shop thì không cho
        bool lockShop = (ViewState["lock_shop"] as bool?) ?? false;
        if (lockShop)
        {
            Helper_Tabler_cl.ShowToast(this.Page, "Bạn không thể chỉnh sửa cửa hàng lúc này.", "warning", true, 2500, "Thông báo");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["taikhoan"].ToString());
            if (q != null)
            {
                taikhoan_tb _ob = q;
                File_Folder_cl.del_file(_ob.anhbia_shop);
                _ob.anhbia_shop = "";
                Button3.Visible = false;
                db.SubmitChanges();

                Label4.Text = "";
                txt_link_fileupload2.Text = "";

                Helper_Tabler_cl.ShowToast(this.Page, "Xóa ảnh thành công.", null, true, 2000, "Thông báo");
            }
        }
    }

    protected void but_themlink_Click(object sender, EventArgs e)
    {
        // ✅ nếu đang ở tab Cửa hàng và bị khóa shop thì chặn thêm link kiểu "Cửa hàng"
        bool lockShop = (ViewState["lock_shop"] as bool?) ?? false;
        if (lockShop && txtKieu.Text == "Cửa hàng")
        {
            Helper_Tabler_cl.ShowToast(this.Page, "Bạn không thể chỉnh sửa link cửa hàng lúc này.", "warning", true, 2500, "Thông báo");
            return;
        }

        txtTen.Text = ResolveDefaultSocialLinkTitle(null);
        txtLink.Text = "";
        TxtIcon.Text = "";
        hfIdLink.Value = "";
        hfSocialPreset.Value = "";
        lblLinkError.Text = "";
        lblLinkError.Visible = false;
        ViewState["EditLinkId"] = null;
        uploadedFileContainer.Visible = false;

        pn_themlink.Visible = true;
        up_themlink.Update();
    }

    protected void btnDong_Click(object sender, EventArgs e)
    {
        txtTen.Text = "";
        txtLink.Text = "";
        TxtIcon.Text = "";
        hfIdLink.Value = "";
        hfSocialPreset.Value = "";
        lblLinkError.Text = "";
        lblLinkError.Visible = false;
        ViewState["EditLinkId"] = null;

        pn_themlink.Visible = false;
        up_themlink.Update();
    }

    protected void rptMangXaHoi_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string id = e.CommandArgument.ToString();

        using (var db = new dbDataContext())
        {
            var item = db.MangXaHoi_tbs.FirstOrDefault(x => x.id.ToString() == id);

            if (e.CommandName == "EditItem")
            {
                // ✅ nếu là link "Cửa hàng" mà shop đang khóa thì chặn
                bool lockShop = (ViewState["lock_shop"] as bool?) ?? false;
                if (lockShop && item != null && item.Kieu == "Cửa hàng")
                {
                    Helper_Tabler_cl.ShowToast(this.Page, "Bạn không thể chỉnh sửa link cửa hàng lúc này.", "warning", true, 2500, "Thông báo");
                    return;
                }

                if (item != null)
                {
                    txtTen.Text = item.Ten;
                    txtLink.Text = item.Link;
                    txtKieu.Text = item.Kieu;
                    hfIdLink.Value = item.id.ToString();
                    hfSocialPreset.Value = DetectSocialPreset(item.Link);
                    lblLinkError.Text = "";
                    lblLinkError.Visible = false;
                    pn_themlink.Visible = true;

                    if (!string.IsNullOrEmpty(item.Icon))
                    {
                        previewImage.ImageUrl = item.Icon;
                        uploadedFileContainer.Visible = true;
                        TxtIcon.Text = item.Icon;
                    }
                    else
                    {
                        uploadedFileContainer.Visible = false;
                    }

                    up_themlink.Update();
                }
            }
            else if (e.CommandName == "DeleteItem")
            {
                // ✅ nếu là link "Cửa hàng" mà shop đang khóa thì chặn
                bool lockShop = (ViewState["lock_shop"] as bool?) ?? false;
                if (lockShop && item != null && item.Kieu == "Cửa hàng")
                {
                    Helper_Tabler_cl.ShowToast(this.Page, "Bạn không thể xóa link cửa hàng lúc này.", "warning", true, 2500, "Thông báo");
                    return;
                }

                if (item != null)
                {
                    string deletedKind = item.Kieu ?? "";
                    File_Folder_cl.del_file(item.Icon);
                    db.MangXaHoi_tbs.DeleteOnSubmit(item);
                    db.SubmitChanges();

                    string currentAccountKey = Convert.ToString(ViewState["taikhoan"]) ?? "";
                    var currentAccount = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == currentAccountKey);
                    if (currentAccount != null)
                    {
                        bool isShopAccount = ShopSlug_cl.IsShopAccount(db, currentAccount);
                        var settings = HomeProfileSetting_cl.GetSettings(db, currentAccountKey, isShopAccount);
                        var scopedLinks = db.MangXaHoi_tbs
                            .Where(x => x.TaiKhoan == currentAccountKey && x.Kieu == deletedKind)
                            .ToList();

                        if (string.Equals(deletedKind, "Cửa hàng", StringComparison.OrdinalIgnoreCase))
                            settings.SocialOrderShop = SocialLinkOrder_cl.Normalize(settings.SocialOrderShop, scopedLinks);
                        else
                            settings.SocialOrderPersonal = SocialLinkOrder_cl.Normalize(settings.SocialOrderPersonal, scopedLinks);

                        HomeProfileSetting_cl.Upsert(db, currentAccountKey, settings, currentAccountKey, isShopAccount);
                    }
                }
                show_main(db);
                up_main.Update();
            }
            else if (e.CommandName == "MoveUpItem" || e.CommandName == "MoveDownItem")
            {
                bool lockShop = (ViewState["lock_shop"] as bool?) ?? false;
                if (lockShop && item != null && item.Kieu == "Cửa hàng")
                {
                    Helper_Tabler_cl.ShowToast(this.Page, "Bạn không thể sắp xếp link cửa hàng lúc này.", "warning", true, 2500, "Thông báo");
                    return;
                }

                if (item != null)
                {
                    string currentAccountKey = Convert.ToString(ViewState["taikhoan"]) ?? "";
                    var currentAccount = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == currentAccountKey);
                    if (currentAccount != null)
                    {
                        bool isShopAccount = ShopSlug_cl.IsShopAccount(db, currentAccount);
                        var settings = HomeProfileSetting_cl.GetSettings(db, currentAccountKey, isShopAccount);
                        var scopedLinks = db.MangXaHoi_tbs
                            .Where(x => x.TaiKhoan == currentAccountKey && x.Kieu == item.Kieu)
                            .ToList();

                        int offset = e.CommandName == "MoveUpItem" ? -1 : 1;
                        if (string.Equals(item.Kieu, "Cửa hàng", StringComparison.OrdinalIgnoreCase))
                        {
                            settings.SocialOrderShop = SocialLinkOrder_cl.Move(settings.SocialOrderShop, scopedLinks, item.id, offset);
                        }
                        else
                        {
                            settings.SocialOrderPersonal = SocialLinkOrder_cl.Move(settings.SocialOrderPersonal, scopedLinks, item.id, offset);
                        }

                        HomeProfileSetting_cl.Upsert(db, currentAccountKey, settings, currentAccountKey, isShopAccount);
                    }
                }

                show_main(db);
                up_main.Update();
            }
        }
    }

    protected void removeUploadedImage(object sender, EventArgs e)
    {
        File_Folder_cl.del_file(TxtIcon.Text);
        TxtIcon.Text = "";
        uploadedFileContainer.Visible = false;
    }

    protected void btnLuuLink_Click(object sender, EventArgs e)
    {
        lblLinkError.Visible = false;
        if (string.IsNullOrWhiteSpace(txtLink.Text))
        {
            lblLinkError.Text = "Vui lòng nhập Link";
            lblLinkError.Visible = true;
            return;
        }

        // ✅ nếu đang lưu link kiểu "Cửa hàng" mà shop khóa thì chặn
        bool lockShop = (ViewState["lock_shop"] as bool?) ?? false;
        if (lockShop && txtKieu.Text == "Cửa hàng")
        {
            Helper_Tabler_cl.ShowToast(this.Page, "Bạn không thể chỉnh sửa link cửa hàng lúc này.", "warning", true, 2500, "Thông báo");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["taikhoan"].ToString());
            if (q_tk == null) return;

            MangXaHoi_tb m;
            if (!string.IsNullOrEmpty(hfIdLink.Value))
            {
                int id = int.Parse(hfIdLink.Value);
                m = db.MangXaHoi_tbs.FirstOrDefault(x => x.id == id);
                if (m == null) return;
            }
            else
            {
                m = new MangXaHoi_tb();
                db.MangXaHoi_tbs.InsertOnSubmit(m);
                m.TaiKhoan = q_tk.taikhoan;
            }

            string preparedLink = BuildPresetSocialLink(hfSocialPreset.Value, txtLink.Text);
            m.Ten = string.IsNullOrWhiteSpace(txtTen.Text) ? ResolveDefaultSocialLinkTitle(q_tk) : txtTen.Text.Trim();
            m.Link = SocialLinkIcon_cl.NormalizeExternalLink(preparedLink);
            if (string.IsNullOrEmpty(m.Link))
            {
                lblLinkError.Text = "Link chưa hợp lệ. Bạn có thể dán link đầy đủ hoặc chỉ nhập username/số điện thoại theo ứng dụng đã chọn.";
                lblLinkError.Visible = true;
                return;
            }

            m.Icon = SocialLinkIcon_cl.ResolveIconForSave(
                m.Link,
                TxtIcon.Text,
                m.Icon
            );
            m.Kieu = txtKieu.Text;

            db.SubmitChanges();

            var refreshedSettings = HomeProfileSetting_cl.GetSettings(db, q_tk.taikhoan, ShopSlug_cl.IsShopAccount(db, q_tk));
            var scopedLinks = db.MangXaHoi_tbs
                .Where(x => x.TaiKhoan == q_tk.taikhoan && x.Kieu == m.Kieu)
                .ToList();
            if (string.Equals(m.Kieu, "Cửa hàng", StringComparison.OrdinalIgnoreCase))
                refreshedSettings.SocialOrderShop = SocialLinkOrder_cl.Normalize(refreshedSettings.SocialOrderShop, scopedLinks);
            else
                refreshedSettings.SocialOrderPersonal = SocialLinkOrder_cl.Normalize(refreshedSettings.SocialOrderPersonal, scopedLinks);
            HomeProfileSetting_cl.Upsert(db, q_tk.taikhoan, refreshedSettings, q_tk.taikhoan, ShopSlug_cl.IsShopAccount(db, q_tk));

            show_main(db);
            up_main.Update();
        }

        hfIdLink.Value = "";
        hfSocialPreset.Value = "";
        txtTen.Text = "";
        txtLink.Text = "";
        TxtIcon.Text = "";
        txtKieu.Text = "";
        lblLinkError.Text = "";
        lblLinkError.Visible = false;

        pn_themlink.Visible = false;
        up_themlink.Update();

        Helper_Tabler_cl.ShowToast(this.Page, "Lưu thành công!", null, true, 2000, "Thông báo");
    }

    protected string ResolveSocialIcon(object iconRaw, object linkRaw)
    {
        return SocialLinkIcon_cl.ResolveIconForDisplay(
            Convert.ToString(iconRaw) ?? "",
            Convert.ToString(linkRaw) ?? ""
        );
    }

    protected bool ShouldShowSocialIcon(object iconRaw, object linkRaw)
    {
        return !string.IsNullOrEmpty(ResolveSocialIcon(iconRaw, linkRaw));
    }

    protected string GetSocialIconMarginStyle(object iconRaw, object linkRaw)
    {
        return ShouldShowSocialIcon(iconRaw, linkRaw) ? "" : "margin-left:60px;";
    }

    protected void but_capnhat_Click(object sender, EventArgs e)
    {
        // ===== cá nhân =====
        string _b1 = txt_link_fileupload.Text;
        string _b2 = txt_hoten.Text.Trim();
        string _b3 = txt_sdt.Text.Trim();
        string _b4 = TextBox9.Text.Trim();
        string _b6 = ResolveProfileAddress();
        string _b7 = txt_gioithieu.Text.Trim();

        bool lockShop = true;

        using (dbDataContext db = new dbDataContext())
        {
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["taikhoan"].ToString());
            if (q == null) return;

            // ✅ luôn cho cập nhật cá nhân
            q.anhdaidien = _b1;
            q.hoten = _b2;
            q.dienthoai = _b3;
            q.email = _b4;
            q.diachi = _b6;
            q.gioithieu = _b7;

            bool isShopAccount = ShopSlug_cl.IsShopAccount(db, q);
            var currentProfileSettings = HomeProfileSetting_cl.GetSettings(db, q.taikhoan, isShopAccount);
            var settings = new HomeProfileSetting_cl.ProfileSettings
            {
                TemplateKey = ddl_profile_template.SelectedValue,
                AccentColor = hf_profile_accent.Value,
                ShowContact = chk_profile_contact.Checked,
                ShowSocial = chk_profile_social.Checked,
                ShowReviews = chk_profile_reviews.Checked,
                ShowShop = (chk_profile_shop != null && chk_profile_shop.Checked),
                ShowProducts = (chk_profile_products != null && chk_profile_products.Checked),
                SocialOrderPersonal = currentProfileSettings.SocialOrderPersonal,
                SocialOrderShop = currentProfileSettings.SocialOrderShop
            };
            HomeProfileSetting_cl.Upsert(db, q.taikhoan, settings, q.taikhoan, isShopAccount);

            db.SubmitChanges();
            show_main(db);
            up_main.Update();

            Helper_Tabler_cl.ShowModal(this.Page, "Cập nhật thông tin thành công", "Thông báo", true, "success");
        }
    }
}
