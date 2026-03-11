using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_edit_info : System.Web.UI.Page
{
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

    // ✅ LOCK nếu: phanloai = Gian hàng đối tác OR đăng ký (latest) đang chờ/đã duyệt
    private bool IsShopLocked(dbDataContext db, string tk)
    {
        if (string.IsNullOrEmpty(tk)) return false;

        var user = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == tk);
        if (user == null) return true;

        // Không thuộc scope shop thì không được chỉnh dữ liệu cửa hàng.
        if (!PortalScope_cl.CanLoginShop(user.taikhoan, user.phanloai, user.permission))
            return true;

        if (user != null && user.phanloai == "Gian hàng đối tác")
            return true;

        var last = db.DangKy_GianHangDoiTac_tbs
            .Where(x => x.taikhoan == tk)
            .OrderByDescending(x => x.NgayTao)
            .FirstOrDefault();

        if (last == null) return false;

        // 0: chờ, 1: duyệt, 2: từ chối
        return last.TrangThai == 0 || last.TrangThai == 1;
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
        bool lockShop = IsShopLocked(db, tk);
        ViewState["lock_shop"] = lockShop;

        // ✅ UI: ẩn tab + ẩn form cửa hàng nếu lock
        phTabCuaHang.Visible = !lockShop;
        phShopEdit.Visible = !lockShop;
        phShopLockedNote.Visible = lockShop;

        // Bind link mạng xã hội
        var link = db.MangXaHoi_tbs.Where(x => x.TaiKhoan.Equals(q_tk.taikhoan)).ToList();
        rptMangXaHoiCN.DataSource = link.Where(x => x.Kieu == "Cá nhân");
        rptMangXaHoiCH.DataSource = link.Where(x => x.Kieu == "Cửa hàng");
        rptMangXaHoiCN.DataBind();
        rptMangXaHoiCH.DataBind();

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
        txt_diachi.Text = q_tk.diachi;
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
            TextBox4.Text = q_tk.diachi_shop;
        }
        else
        {
            // nếu lock: ẩn luôn nút xóa ảnh shop
            Button1.Visible = false;
            Button3.Visible = false;
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

        txtTen.Text = "";
        txtLink.Text = "";
        TxtIcon.Text = "";
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
                    hfIdLink.Value = item.id.ToString();
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
                    File_Folder_cl.del_file(item.Icon);
                    db.MangXaHoi_tbs.DeleteOnSubmit(item);
                    db.SubmitChanges();
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

            m.Ten = txtTen.Text;
            m.Link = SocialLinkIcon_cl.NormalizeExternalLink(txtLink.Text);
            if (string.IsNullOrEmpty(m.Link))
            {
                lblLinkError.Text = "Link chưa hợp lệ. Vui lòng nhập đúng định dạng http/https.";
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

            show_main(db);
            up_main.Update();
        }

        hfIdLink.Value = "";
        txtTen.Text = "";
        txtLink.Text = "";
        TxtIcon.Text = "";
        txtKieu.Text = "";

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
        string _b6 = txt_diachi.Text.Trim();
        string _b7 = txt_gioithieu.Text.Trim();

        // ===== cửa hàng =====
        string _c1 = txt_link_fileupload1.Text.Trim();
        string _c2 = txt_link_fileupload2.Text.Trim();
        string _c3 = TextBox2.Text.Trim();
        string _c4 = TextBox3.Text.Trim();
        string _c5 = TextBox10.Text.Trim();
        string _c7 = TextBox5.Text.Trim();
        string _c8 = TextBox4.Text.Trim();

        bool lockShop = (ViewState["lock_shop"] as bool?) ?? false;

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

            // ✅ chỉ cập nhật shop khi KHÔNG khóa
            if (!lockShop)
            {
                q.logo_shop = _c1;
                q.anhbia_shop = _c2;
                q.ten_shop = _c3;
                q.sdt_shop = _c4;
                q.email_shop = _c5;
                q.motangan_shop = _c7;
                q.diachi_shop = _c8;
            }

            bool isShopAccount = ShopSlug_cl.IsShopAccount(db, q);
            var settings = new HomeProfileSetting_cl.ProfileSettings
            {
                TemplateKey = ddl_profile_template.SelectedValue,
                AccentColor = hf_profile_accent.Value,
                ShowContact = chk_profile_contact.Checked,
                ShowSocial = chk_profile_social.Checked,
                ShowReviews = chk_profile_reviews.Checked,
                ShowShop = (chk_profile_shop != null && chk_profile_shop.Checked),
                ShowProducts = (chk_profile_products != null && chk_profile_products.Checked)
            };
            HomeProfileSetting_cl.Upsert(db, q.taikhoan, settings, q.taikhoan, isShopAccount);

            db.SubmitChanges();
            show_main(db);
            up_main.Update();

            if (lockShop)
                Helper_Tabler_cl.ShowToast(this.Page, "Đã cập nhật thông tin cá nhân. Thông tin cửa hàng đang bị khóa chỉnh sửa.", "warning", true, 2500, "Thông báo");
            else
                Helper_Tabler_cl.ShowToast(this.Page, "Xử lý thành công.", null, true, 2000, "Thông báo");
        }
    }
}
