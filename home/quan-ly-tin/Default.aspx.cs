using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_quan_ly_bai_Default : System.Web.UI.Page
{
    DanhMuc_cl dm_cl = new DanhMuc_cl();
    String_cl str_cl = new String_cl();
    DateTime_cl dt_cl = new DateTime_cl();

    bool IsDuyetGianHangDoiTac()
    {
        string tk = (PortalRequest_cl.GetCurrentAccount() ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(tk))
        {
            string tkEnc = PortalRequest_cl.GetCurrentAccountEncrypted();
            if (string.IsNullOrEmpty(tkEnc))
                return false;
            tk = mahoa_cl.giaima_Bcorn(tkEnc);
        }
        if (string.IsNullOrEmpty(tk))
            return false;

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == tk);
            return acc != null && SpaceAccess_cl.CanAccessShop(db, acc);
        }
    }

    private bool IsCurrentCompanyShopPortal()
    {
        using (dbDataContext db = new dbDataContext())
        {
            return CompanyShop_cl.IsCurrentPortalCompanyShop(db);
        }
    }

    private bool IsCompanyShopPortalCached()
    {
        return string.Equals((ViewState["is_company_shop_portal"] ?? "").ToString(), "1", StringComparison.OrdinalIgnoreCase);
    }

    private void BindCompanyShopOptions(bool isCompanyShopPortal, bool resetDefaults)
    {
        ph_company_shop_options.Visible = isCompanyShopPortal;
        if (!isCompanyShopPortal)
            return;

        if (ddl_kenh_hienthi.Items.Count == 0)
        {
            ddl_kenh_hienthi.Items.Add(new ListItem("Công khai (hiển thị ngoài Home)", "public"));
            ddl_kenh_hienthi.Items.Add(new ListItem("Nội bộ (chỉ shop công ty tự bán)", "internal"));
        }

        if (resetDefaults)
        {
            if (ddl_kenh_hienthi.Items.FindByValue("public") != null)
                ddl_kenh_hienthi.SelectedValue = "public";
            txt_phantram_san.Text = "0";
        }
    }

    private void BindBatDongSanOptions()
    {
        if (ddl_bds_purpose.Items.Count == 0)
        {
            ddl_bds_purpose.Items.Add(new ListItem("Mua bán", "sale"));
            ddl_bds_purpose.Items.Add(new ListItem("Cho thuê", "rent"));
        }

        if (ddl_bds_property_type.Items.Count == 0)
        {
            ddl_bds_property_type.Items.Add(new ListItem("Căn hộ", "apartment"));
            ddl_bds_property_type.Items.Add(new ListItem("Nhà phố", "house"));
            ddl_bds_property_type.Items.Add(new ListItem("Đất nền", "land"));
            ddl_bds_property_type.Items.Add(new ListItem("Văn phòng", "office"));
            ddl_bds_property_type.Items.Add(new ListItem("Mặt bằng", "business-premises"));
        }

        if (ddl_bds_direction.Items.Count == 0)
        {
            ddl_bds_direction.Items.Add(new ListItem("Chưa chọn", ""));
            ddl_bds_direction.Items.Add(new ListItem("Đông", "Đông"));
            ddl_bds_direction.Items.Add(new ListItem("Tây", "Tây"));
            ddl_bds_direction.Items.Add(new ListItem("Nam", "Nam"));
            ddl_bds_direction.Items.Add(new ListItem("Bắc", "Bắc"));
            ddl_bds_direction.Items.Add(new ListItem("Đông Nam", "Đông Nam"));
            ddl_bds_direction.Items.Add(new ListItem("Đông Bắc", "Đông Bắc"));
            ddl_bds_direction.Items.Add(new ListItem("Tây Nam", "Tây Nam"));
            ddl_bds_direction.Items.Add(new ListItem("Tây Bắc", "Tây Bắc"));
        }
    }

    private void BindBatDongSanCategoryIds()
    {
        using (dbDataContext db = new dbDataContext())
        {
            hf_bds_category_ids.Value = BatDongSanMetadata_cl.GetRealEstateCategoryIdsCsv(db);
        }
    }

    private string GetCreatePostUrl()
    {
        return PortalRequest_cl.IsShopPortalRequest()
            ? "/shop/quan-ly-tin/them"
            : "/home/quan-ly-tin/them.aspx";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        lnk_add_new.NavigateUrl = GetCreatePostUrl();
        bool isCompanyShopPortal = IsCurrentCompanyShopPortal();
        ViewState["is_company_shop_portal"] = isCompanyShopPortal ? "1" : "0";
        ph_company_shop_options.Visible = isCompanyShopPortal;

        if (!IsPostBack)
        {
            Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_home("none", "none", true); //check tài khoản, có chuyển hướng. YÊU CẦU ĐĂNG NHẬP.

            // ✅ CHỈ CHO VÀO NẾU ĐÃ ĐƯỢC DUYỆT GIAN HÀNG ĐỐI TÁC
            if (!IsDuyetGianHangDoiTac())
            {
                // set modal để trang chủ hiển thị
                Session["home_modal_msg"] = "Tính năng này chỉ dành cho tài khoản đã đăng ký gian hàng đối tác thành công.";
                Session["home_modal_title"] = "Chưa đủ điều kiện";
                Session["home_modal_type"] = "warning"; // success/info/warning/danger

                Response.Redirect(PortalRequest_cl.IsShopPortalRequest() ? "/shop/default.aspx" : "~/", true); // đổi lại đúng trang chủ bạn dùng
                return;
            }

            string _tk = PortalRequest_cl.GetCurrentAccountEncrypted();

            if (!string.IsNullOrEmpty(_tk))//nếu có khách đăng nhập
            {
                ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
            { }

            if (PortalRequest_cl.IsShopPortalRequest())
            {
                using (dbDataContext db = new dbDataContext())
                {
                    ShopToAhaShinePostSync_cl.SyncTradePostsForShopThrottled(db, (ViewState["taikhoan"] ?? "").ToString(), 3);
                }
            }

            set_dulieu_macdinh();
            BindCompanyShopOptions(isCompanyShopPortal, true);
            BindBatDongSanOptions();
            BindBatDongSanCategoryIds();
            show_main();

            string editIdRaw = (Request.QueryString["edit_id"] ?? "").Trim();
            int editIdValue;
            if (!string.IsNullOrEmpty(editIdRaw) && int.TryParse(editIdRaw, out editIdValue))
            {
                LoadEditPost(editIdValue);
            }
        }
    }
    public void set_dulieu_macdinh()
    {
        ViewState["current_page_qltin_home"] = HomePager_cl.ResolvePage(Request).ToString();
    }

    #region main - phân trang - tìm kiếm
    public void show_main()
    {
    
        using (dbDataContext db = new dbDataContext())
        {
            #region lấy dữ liệu
            string taiKhoan = (ViewState["taikhoan"] ?? "").ToString().Trim().ToLowerInvariant();
            if (taiKhoan == "")
                taiKhoan = CompanyShop_cl.ResolveCurrentPortalAccount();
            if (taiKhoan == "")
            {
                Repeater1.DataSource = new object[0];
                Repeater1.DataBind();
                lb_show.Text = "0-0/0";
                lb_show_md.Text = "0-0/0";
                return;
            }

            bool isCompanyShopPortal = IsCompanyShopPortalCached();
            var sourcePosts = db.BaiViet_tbs.Where(p => p.nguoitao == taiKhoan);
            if (isCompanyShopPortal)
                sourcePosts = sourcePosts.Where(p => p.phanloai == CompanyShop_cl.ProductTypePublic
                    || p.phanloai == CompanyShop_cl.ProductTypeInternal
                    || p.phanloai == AccountVisibility_cl.PostTypeService);
            else
                sourcePosts = sourcePosts.Where(p => p.phanloai == CompanyShop_cl.ProductTypePublic
                    || p.phanloai == AccountVisibility_cl.PostTypeService);

            var list_all = (from ob1 in sourcePosts
                            join ob2 in db.DanhMuc_tbs on ob1.id_DanhMuc equals ob2.id.ToString() into danhMucGroup
                            from ob2 in danhMucGroup.DefaultIfEmpty()
                            join ob3 in db.DanhMuc_tbs on ob1.id_DanhMucCap2 equals ob3.id.ToString() into danhMucGroup2
                            from ob3 in danhMucGroup2.DefaultIfEmpty()
                            select new
                            {
                                ob1.id,
                                ob1.image,
                                ob1.name,
                                ob1.name_en,
                                ob1.ngaytao,
                                ob1.giaban,
                                ob1.bin,
                                ob1.description,
                           
                                PhanTram_GiamGia_ThanhToan_BangEvoucher = ob1.PhanTram_GiamGia_ThanhToan_BangEvoucher,
                                PhanTram_ChiTra_ChoSan = ob1.banhang_thuong,
                                KenhRaw = ob1.phanloai,
                                TenMenu = ob2 != null ? ob2.name : "",  // Trả về rỗng nếu không có danh mục
                                TenMenu2 = ob3 != null ? "<span class='pl-3 mif-chevron-right'></span>" + ob3.name : "",  // Trả về rỗng nếu không có danh mục con
                            }).AsQueryable();

            // Kiểm tra xem textbox có dữ liệu tìm kiếm không
            string _key = txt_timkiem.Text.Trim();
            if (!string.IsNullOrEmpty(_key))
                list_all = list_all.Where(p => p.name.Contains(_key) || p.name_en.Contains(_key) || p.id.ToString() == _key);
            else
            {
                string _key1 = txt_timkiem1.Text.Trim();
                if (!string.IsNullOrEmpty(_key1))
                    list_all = list_all.Where(p => p.name.Contains(_key1) || p.name_en.Contains(_key1) || p.id.ToString() == _key1);
            }

            //sắp xếp
            list_all = list_all.OrderByDescending(p => p.ngaytao);
            int _Tong_Record = list_all.Count();
            #endregion

            #region phân trang OK, k sửa
            // Xử lý số record mỗi trang
            int show = 30; if (show <= 0) show = 30;
            //xử lý trang hiện tại. Đảm bảo current_page không nhỏ hơn 1 và không lớn hơn total_page
            int current_page = int.Parse(ViewState["current_page_qltin_home"].ToString()); int total_page = number_of_page_class.return_total_page(_Tong_Record, show);
            if (total_page < 1) total_page = 1; if (current_page < 1) current_page = 1; else if (current_page > total_page) current_page = total_page;
            ViewState["total_page"] = total_page;

            litPager.Text = HomePager_cl.RenderPager(Request, current_page, total_page);
            but_xemtiep.Visible = false;
            but_xemtiep1.Visible = false;
            but_quaylai.Visible = false;
            but_quaylai1.Visible = false;
            //xử lý nút bấm tới lui
            if (current_page >= total_page)
            {
                but_xemtiep.Enabled = false;//máy tính
                but_xemtiep1.Enabled = false;//điện thoại
            }
            else
            {
                but_xemtiep.Enabled = true;
                but_xemtiep1.Enabled = true;
            }
            if (current_page == 1)
            {
                but_quaylai.Enabled = false;
                but_quaylai1.Enabled = false;
            }
            else
            {
                but_quaylai.Enabled = true;
                but_quaylai1.Enabled = true;
            }
            //PHÂN TRANG****PHÂN TRANG
            var list_split = list_all.Skip(current_page * show - show).Take(show);
            //xử lý thanh thông báo phân trang
            int stt = (show * current_page) - show + 1; int _s1 = stt + list_split.Count() - 1;
            if (_Tong_Record != 0) lb_show.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0"); else lb_show.Text = "0-0/0"; lb_show_md.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0");
            #endregion

            Repeater1.DataSource = list_split;
            Repeater1.DataBind();
        }
    }

    private void BindThanhPhoOptions(string selected)
    {
        using (dbDataContext db = new dbDataContext())
        {
            var thamPhos = db.ThanhPhos.ToList();
            DanhSachTP.Items.Clear();
            DanhSachTP.Items.Add(new ListItem("Nhấn để chọn", ""));
            foreach (var tp in thamPhos)
            {
                DanhSachTP.Items.Add(new ListItem(TinhThanhDisplay_cl.Format(tp.Ten), tp.Ten));
            }
        }

        if (!string.IsNullOrEmpty(selected) && DanhSachTP.Items.FindByValue(selected) != null)
            DanhSachTP.SelectedValue = selected;
    }

    protected string GetEditPostUrl(object idObj)
    {
        string id = (idObj ?? "").ToString();
        if (string.IsNullOrEmpty(id))
            return "#";

        string url = PortalRequest_cl.IsShopPortalRequest()
            ? "/shop/quan-ly-tin?edit_id=" + HttpUtility.UrlEncode(id)
            : "/home/quan-ly-tin/default.aspx?edit_id=" + HttpUtility.UrlEncode(id);
        return url;
    }

    private void LoadEditPost(int id)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string tk = PortalRequest_cl.GetCurrentAccount();
            if (string.IsNullOrEmpty(tk))
                tk = CompanyShop_cl.ResolveCurrentPortalAccount();
            BaiViet_tb post = db.BaiViet_tbs.FirstOrDefault(p => p.id == id && p.nguoitao == tk);
            if (post == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Bài viết không tồn tại hoặc bạn không có quyền cập nhật.", "Thông báo", true, "warning");
                return;
            }

            reset_control_add_edit();
            ViewState["add_edit"] = "edit";
            ViewState["edit_id"] = id.ToString();
            Label1.Text = "CẬP NHẬT TIN";
            but_add_edit.Text = "CẬP NHẬT";

            dm_cl.Show_DanhMuc(2, 3, ddl_DanhMuc, false, "web", "135");
            if (!string.IsNullOrEmpty(post.id_DanhMuc) && ddl_DanhMuc.Items.FindByValue(post.id_DanhMuc) != null)
                ddl_DanhMuc.SelectedValue = post.id_DanhMuc;

            BindThanhPhoOptions((post.ThanhPho ?? "").Trim());
            BindCompanyShopOptions(IsCompanyShopPortalCached(), false);
            BindBatDongSanOptions();
            BindBatDongSanCategoryIds();

            txt_name.Text = post.name ?? "";
            txt_description.Text = post.description ?? "";
            txt_noidung.Text = post.content_post ?? "";
            txt_link_fileupload.Text = post.image ?? "";
            txt_giaban.Text = string.Format("{0:#,##0}", (post.giaban ?? 0));
            txt_phantram_uu_dai.Text = (post.PhanTram_GiamGia_ThanhToan_BangEvoucher ?? 0).ToString();
            txt_phantram_san.Text = CompanyShop_cl.GetPlatformSharePercent(post).ToString();
            LinkMap.Text = post.LinkMap ?? "";

            if (!string.IsNullOrEmpty(post.image))
            {
                string safeImg = HttpUtility.HtmlEncode(post.image);
                lit_uploaded_main.Text = "<div class='small text-muted mb-1'>Ảnh hiện tại</div><img class='img-fluid rounded' style='max-width:160px' src='" + safeImg + "' />";
            }

            var extraImages = db.AnhSanPham_tbs.Where(x => x.idsp == post.id).ToList();
            if (extraImages.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var img in extraImages)
                {
                    string url = (img.url ?? "").Trim();
                    if (string.IsNullOrEmpty(url))
                        continue;
                    string safeUrl = HttpUtility.HtmlEncode(url);
                    sb.Append("<div style='display:inline-block;margin:6px'>")
                      .Append("<img class='rounded' style='width:100px;height:100px;object-fit:cover;border:1px solid rgba(98,105,118,.18)' src='")
                      .Append(safeUrl)
                      .Append("' />")
                      .Append("</div>");
                }
                lit_uploaded_list.Text = sb.ToString();
            }

            if (ph_company_shop_options.Visible)
            {
                string kenh = CompanyShop_cl.IsInternalProductType(post.phanloai) ? "internal" : "public";
                if (ddl_kenh_hienthi.Items.FindByValue(kenh) != null)
                    ddl_kenh_hienthi.SelectedValue = kenh;
            }

            BatDongSanMetadata_cl.PostMetadata metadata = BatDongSanMetadata_cl.GetByPostId(db, post.id);
            if (metadata != null)
            {
                string normalizedPurpose = BatDongSanMetadata_cl.NormalizePurpose(metadata.ListingPurpose);
                string normalizedType = BatDongSanMetadata_cl.NormalizePropertyType(metadata.PropertyType);
                if (ddl_bds_purpose.Items.FindByValue(normalizedPurpose) != null)
                    ddl_bds_purpose.SelectedValue = normalizedPurpose;
                if (ddl_bds_property_type.Items.FindByValue(normalizedType) != null)
                    ddl_bds_property_type.SelectedValue = normalizedType;
                if (ddl_bds_legal.Items.FindByValue((metadata.LegalStatus ?? "").Trim()) != null)
                    ddl_bds_legal.SelectedValue = (metadata.LegalStatus ?? "").Trim();
                if (ddl_bds_furnishing.Items.FindByValue((metadata.FurnishingStatus ?? "").Trim()) != null)
                    ddl_bds_furnishing.SelectedValue = (metadata.FurnishingStatus ?? "").Trim();

                txt_bds_area.Text = metadata.AreaValue.ToString("0.##");
                txt_bds_bedrooms.Text = metadata.BedroomCount.ToString();
                txt_bds_bathrooms.Text = metadata.BathroomCount.ToString();
                txt_bds_deposit.Text = metadata.DepositAmount.ToString("0.##");
                txt_bds_rental_term.Text = metadata.RentalTermMonths.ToString();
                txt_bds_floor_count.Text = metadata.FloorCount.ToString();
                txt_bds_land_width.Text = metadata.LandWidth.ToString("0.##");
                txt_bds_land_length.Text = metadata.LandLength.ToString("0.##");
                if (ddl_bds_direction.Items.FindByValue((metadata.HouseDirection ?? "").Trim()) != null)
                    ddl_bds_direction.SelectedValue = (metadata.HouseDirection ?? "").Trim();
                txt_bds_project.Text = metadata.ProjectName ?? "";
                txt_bds_district.Text = metadata.DistrictName ?? "";
                txt_bds_ward.Text = metadata.WardName ?? "";
                txt_bds_address_line.Text = metadata.AddressLine ?? "";
            }

            pn_add.Visible = true;
        }
    }

    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        ViewState["current_page_qltin_home"] = int.Parse(ViewState["current_page_qltin_home"].ToString()) - 1;
        show_main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        ViewState["current_page_qltin_home"] = int.Parse(ViewState["current_page_qltin_home"].ToString()) + 1;
        show_main();
    }
    protected void txt_timkiem_TextChanged(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        ViewState["current_page_qltin_home"] = 1;
        show_main();
    }
    #endregion

    #region ADD - EDIT - CHI TIẾT
    public void reset_control_add_edit()
    {
        Label1.Text = null;
        txt_name.Text = "";
        txt_giaban.Text = "0";
        txt_phantram_uu_dai.Text = "0"; // ✅ NEW default
        txt_phantram_san.Text = "0";
        txt_description.Text = "";
        txt_noidung.Text = "";
        txt_link_fileupload.Text = "";
        hf_anhphu.Value = ""; // ✅ reset ảnh phụ
        lit_uploaded_main.Text = "";
        lit_uploaded_list.Text = "";
        LinkMap.Text = "";
        txt_bds_area.Text = "0";
        txt_bds_bedrooms.Text = "0";
        txt_bds_bathrooms.Text = "0";
        txt_bds_deposit.Text = "0";
        txt_bds_rental_term.Text = "0";
        txt_bds_floor_count.Text = "0";
        txt_bds_land_width.Text = "0";
        txt_bds_land_length.Text = "0";
        txt_bds_project.Text = "";
        txt_bds_district.Text = "";
        txt_bds_ward.Text = "";
        txt_bds_address_line.Text = "";
        if (ddl_bds_direction.Items.Count > 0) ddl_bds_direction.SelectedIndex = 0;
        if (ddl_bds_purpose.Items.FindByValue("sale") != null) ddl_bds_purpose.SelectedValue = "sale";
        if (ddl_bds_property_type.Items.FindByValue("apartment") != null) ddl_bds_property_type.SelectedValue = "apartment";
        if (ddl_bds_legal.Items.FindByValue("Chưa cập nhật") != null) ddl_bds_legal.SelectedValue = "Chưa cập nhật";
        if (ddl_bds_furnishing.Items.FindByValue("Chưa cập nhật") != null) ddl_bds_furnishing.SelectedValue = "Chưa cập nhật";
        ViewState["add_edit"] = null;
        ViewState["edit_id"] = null;
        ddl_DanhMuc.DataSource = null;
        ddl_DanhMuc.DataBind();
    }

    protected void but_show_form_add_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        //reset control
        reset_control_add_edit();

        ViewState["add_edit"] = "add";
        Label1.Text = "ĐĂNG TIN MỚI";
        but_add_edit.Text = "ĐĂNG TIN";

        DanhMuc_cl dm_cl = new DanhMuc_cl();
        dm_cl.Show_DanhMuc(2, 3, ddl_DanhMuc, false, "web", "135");//ngoại trừ 135 là liên hệ

        BindThanhPhoOptions("");
        BindCompanyShopOptions(IsCompanyShopPortalCached(), true);
        BindBatDongSanOptions();
        BindBatDongSanCategoryIds();

        //hiện form add_edit trong updatePanel_add
        pn_add.Visible = !pn_add.Visible;
        up_add.Update();
    }

    protected void but_close_form_add_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        //reset control
        reset_control_add_edit();
        //ẩn form
        pn_add.Visible = !pn_add.Visible;
    }

    protected void but_add_edit_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        #region Chuẩn bị dữ liệu
        //xác định loại bài viết
        string _phanloai_baiviet = "sanpham";
        //đảm bảo luôn có thư mục chứa ảnh
        if (!Directory.Exists(Server.MapPath("~/uploads/img-handler/"))) Directory.CreateDirectory(Server.MapPath("~/uploads/img-handler/"));
        //xử lý dữ liệu đầu vào
        string _name = str_cl.Remove_Blank(txt_name.Text.Trim());
        string _name_en = str_cl.replace_name_to_url(_name);
        string _idmenu = ddl_DanhMuc.SelectedValue.ToString();//giá trị đầu là ""
        string _description = txt_description.Text.Trim();
        string _noidung = txt_noidung.Text.Trim();
        string _image = txt_link_fileupload.Text;
        string _linkMap = LinkMap.Text;
        string _thanhPho = DanhSachTP.SelectedValue.ToString();
        bool _bin = false;
        DateTime _ngaytao = AhaTime_cl.Now;
        string _nguoitao = PortalRequest_cl.GetCurrentAccount();
        if (string.IsNullOrEmpty(_nguoitao))
            _nguoitao = CompanyShop_cl.ResolveCurrentPortalAccount();
        bool _noibat = false;

        Int64 _giaban = 0, _giavon = 0;
        bool _chotsale_phantram_hoac_tien = true, _banhang_phantram_hoac_tien = true;
        Int64 _chotsale_thuong = 0, _banhang_thuong = 0;

        _giaban = Number_cl.Check_Int64(txt_giaban.Text.Trim());
        _giavon = 0;
        _chotsale_thuong = 0;
        _banhang_thuong = 0;
        if (_giaban < 0) _giaban = 0;
        if (_giavon < 0) _giavon = 0;
        if (_chotsale_thuong < 0) _chotsale_thuong = 0;
        if (_banhang_thuong < 0) _banhang_thuong = 0;

        // ✅ NEW: phần trăm ưu đãi (null/rỗng -> 0)
        int _phantram_uu_dai = 0;
        string rawPercent = (txt_phantram_uu_dai.Text ?? "").Trim();
        if (!string.IsNullOrEmpty(rawPercent))
        {
            int.TryParse(rawPercent, out _phantram_uu_dai);
        }
        if (_phantram_uu_dai < 0) _phantram_uu_dai = 0;

        int _phantram_chietkhau_san = 0;
        int.TryParse((txt_phantram_san.Text ?? "").Trim(), out _phantram_chietkhau_san);
        _phantram_chietkhau_san = CompanyShop_cl.ClampPlatformSharePercent(_phantram_chietkhau_san);
        string _kenh_hienthi = (ddl_kenh_hienthi.SelectedValue ?? "").Trim();
        string _bds_purpose = BatDongSanMetadata_cl.NormalizePurpose(ddl_bds_purpose.SelectedValue);
        string _bds_property_type = BatDongSanMetadata_cl.NormalizePropertyType(ddl_bds_property_type.SelectedValue);
        decimal _bds_area = BatDongSanMetadata_cl.ParseArea(txt_bds_area.Text);
        int _bds_bedrooms = Number_cl.Check_Int((txt_bds_bedrooms.Text ?? "").Trim());
        int _bds_bathrooms = Number_cl.Check_Int((txt_bds_bathrooms.Text ?? "").Trim());
        decimal _bds_deposit = BatDongSanMetadata_cl.ParseDecimal(txt_bds_deposit.Text);
        int _bds_rental_term = Number_cl.Check_Int((txt_bds_rental_term.Text ?? "").Trim());
        int _bds_floor_count = Number_cl.Check_Int((txt_bds_floor_count.Text ?? "").Trim());
        decimal _bds_land_width = BatDongSanMetadata_cl.ParseDecimal(txt_bds_land_width.Text);
        decimal _bds_land_length = BatDongSanMetadata_cl.ParseDecimal(txt_bds_land_length.Text);
        string _bds_direction = (ddl_bds_direction.SelectedValue ?? "").Trim();
        string _bds_legal = (ddl_bds_legal.SelectedValue ?? "").Trim();
        string _bds_furnishing = (ddl_bds_furnishing.SelectedValue ?? "").Trim();
        string _bds_project = (txt_bds_project.Text ?? "").Trim();
        string _bds_district = (txt_bds_district.Text ?? "").Trim();
        string _bds_ward = (txt_bds_ward.Text ?? "").Trim();
        string _bds_address_line = (txt_bds_address_line.Text ?? "").Trim();

        #endregion

        using (dbDataContext db = new dbDataContext())
        {
            bool isCompanyShop = CompanyShop_cl.IsCompanyShopAccount(db, _nguoitao);
            _phanloai_baiviet = CompanyShop_cl.NormalizeProductType(_kenh_hienthi, isCompanyShop);
            bool isShopPortal = PortalRequest_cl.IsShopPortalRequest();
            int appliedPlatformSharePercent = isCompanyShop ? _phantram_chietkhau_san : 0;
            if (isShopPortal)
            {
                int policyPercent;
                if (ShopPolicy_cl.TryGetActivePolicyPercent(db, _nguoitao, out policyPercent))
                {
                    appliedPlatformSharePercent = policyPercent;
                }
                else if (!isCompanyShop)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Shop của bạn chưa có chính sách % chiết khấu mặc định. Vui lòng gửi/duyệt yêu cầu mở không gian shop trước khi đăng tin.", "Thông báo", true, "warning");
                    return;
                }
            }

            bool _isBatDongSan = BatDongSanMetadata_cl.IsRealEstateCategory(db, _idmenu);
            if (_isBatDongSan)
            {
                if (_bds_area <= 0)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Tin bất động sản bắt buộc phải có diện tích hợp lệ.", "Thông báo", true, "warning");
                    return;
                }

                if (string.Equals(_bds_property_type, "land", StringComparison.OrdinalIgnoreCase))
                {
                    _bds_bedrooms = 0;
                    _bds_bathrooms = 0;
                    _bds_furnishing = "Chưa cập nhật";
                    if (_bds_land_width <= 0 || _bds_land_length <= 0)
                    {
                        Helper_Tabler_cl.ShowModal(this.Page, "Đất nền cần nhập Ngang và Dài hợp lệ.", "Thông báo", true, "warning");
                        return;
                    }
                }

                if (string.Equals(_bds_property_type, "office", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(_bds_property_type, "business-premises", StringComparison.OrdinalIgnoreCase))
                {
                    _bds_bedrooms = 0;
                }

                if (string.Equals(_bds_property_type, "house", StringComparison.OrdinalIgnoreCase))
                {
                    if (_bds_floor_count <= 0)
                    {
                        Helper_Tabler_cl.ShowModal(this.Page, "Nhà phố cần nhập số tầng.", "Thông báo", true, "warning");
                        return;
                    }
                }
                else
                {
                    _bds_floor_count = 0;
                }

                if (!string.Equals(_bds_property_type, "land", StringComparison.OrdinalIgnoreCase))
                {
                    _bds_land_width = 0;
                    _bds_land_length = 0;
                }

                if (!string.Equals(_bds_purpose, "rent", StringComparison.OrdinalIgnoreCase))
                {
                    _bds_deposit = 0;
                    _bds_rental_term = 0;
                }
                else
                {
                    if (_bds_deposit <= 0 || _bds_rental_term <= 0)
                    {
                        Helper_Tabler_cl.ShowModal(this.Page, "Tin cho thuê cần nhập tiền cọc và kỳ hạn thuê.", "Thông báo", true, "warning");
                        return;
                    }
                }

                if (!CompanyShop_cl.IsInternalProductType(_phanloai_baiviet))
                    _phanloai_baiviet = AccountVisibility_cl.PostTypeProduct;
            }

            #region Kiểm tra ngoại lệ.
            if (_idmenu == "")
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng chọn Danh mục.", "Thông báo", true, "warning");
                return;
            }
            if (_name == "")
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập tên sản phẩm.", "Thông báo", true, "warning");
                return;
            }
            if (_image == "")
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng chọn ảnh sản phẩm.", "Thông báo", true, "warning");
                return;
            }
            if (_noidung == "")
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập nội dung.", "Thông báo", true, "warning");
                return;
            }
            if (_giaban == 0)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập giá bán.", "Thông báo", true, "warning");
                return;
            }

            // ✅ NEW: validate tối đa 50
            if (_phantram_uu_dai > 50)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Phần trăm ưu đãi tối đa là 50%. Vui lòng nhập lại.", "Thông báo", true, "warning");
                return;
            }
            #endregion

            if (ViewState["add_edit"].ToString() == "add")
            {
                #region thêm mới
                BaiViet_tb _ob = new BaiViet_tb();
                _ob.name = _name;
                _ob.name_en = _name_en;

                var q_danhmuc = db.DanhMuc_tbs.FirstOrDefault(p => p.id.ToString() == _idmenu);
                if (q_danhmuc != null)
                {
                    _ob.id_DanhMuc = _idmenu;
                    _ob.id_DanhMucCap2 = "";
                }
                else
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Danh mục không hợp lệ.", "Thông báo", true, "warning");
                    return;
                }

                _ob.content_post = _noidung;
                _ob.description = _description;
                _ob.image = _image;
                _ob.bin = _bin;
                _ob.ngaytao = _ngaytao;
                _ob.nguoitao = _nguoitao;
                _ob.noibat = _noibat;
                _ob.giaban = _giaban;
                _ob.giavon = _giavon;
                _ob.soluong_tonkho = 0;
                _ob.banhang_thuong = _banhang_thuong;
                _ob.chotsale_thuong = _chotsale_thuong;
                _ob.phanloai = _phanloai_baiviet;
                _ob.banhang_phantram_hoac_tien = _banhang_phantram_hoac_tien;
                _ob.chotsale_phantram_hoac_tien = _chotsale_phantram_hoac_tien;
                _ob.soluong_daban = 0;
                _ob.soluong_daban = 0;
                _ob.ThanhPho = _thanhPho;
                _ob.LinkMap = _linkMap;

                // ✅ NEW: lưu phần trăm ưu đãi (null coi như 0)
                // Nếu field của bạn là int? thì vẫn ok (0), còn nếu muốn null khi 0 thì có thể set null tại đây.
                _ob.PhanTram_GiamGia_ThanhToan_BangEvoucher = _phantram_uu_dai;
                CompanyShop_cl.SetPlatformSharePercent(_ob, appliedPlatformSharePercent);

                db.BaiViet_tbs.InsertOnSubmit(_ob);
                db.SubmitChanges();
                if (_isBatDongSan)
                {
                    BatDongSanMetadata_cl.Upsert(db, new BatDongSanMetadata_cl.PostMetadata
                    {
                        PostId = _ob.id,
                        ListingPurpose = _bds_purpose,
                        PropertyType = _bds_property_type,
                        AreaValue = _bds_area,
                        DepositAmount = _bds_deposit,
                        RentalTermMonths = _bds_rental_term,
                        FloorCount = _bds_floor_count,
                        LandWidth = _bds_land_width,
                        LandLength = _bds_land_length,
                        HouseDirection = _bds_direction,
                        LegalStatus = _bds_legal,
                        FurnishingStatus = _bds_furnishing,
                        BedroomCount = _bds_bedrooms,
                        BathroomCount = _bds_bathrooms,
                        ProjectName = _bds_project,
                        ProvinceName = _thanhPho,
                        DistrictName = _bds_district,
                        WardName = _bds_ward,
                        AddressLine = _bds_address_line
                    });
                }
                ShopToAhaShinePostSync_cl.SyncTradePost(db, _ob);
                #endregion

                #region lưu danh sách ảnh phụ vào AnhSanPham_tb
                string ds_anhphu = hf_anhphu.Value;
                if (!string.IsNullOrEmpty(ds_anhphu))
                {
                    string[] arrUrl = ds_anhphu.Split('|');
                    foreach (string url in arrUrl)
                    {
                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            AnhSanPham_tb ob_anh = new AnhSanPham_tb();
                            ob_anh.url = url.Trim();
                            ob_anh.idsp = (int?)_ob.id; // idsp là nullable int
                            db.AnhSanPham_tbs.InsertOnSubmit(ob_anh);
                        }
                    }
                    db.SubmitChanges();
                }
                #endregion

                #region cập nhật dữ liệu và update hiển thị
                //reset 1 vài control để việc tiếp tục nhập (nếu muốn nhập tiếp) nhanh hơn
                txt_name.Text = "";
                txt_giaban.Text = "0";
                txt_phantram_uu_dai.Text = "0"; // ✅ NEW reset
                txt_phantram_san.Text = "0";
                txt_description.Text = "";
                txt_link_fileupload.Text = "";
                txt_noidung.Text = "";
                hf_anhphu.Value = "";
                txt_bds_area.Text = "0";
                txt_bds_bedrooms.Text = "0";
                txt_bds_bathrooms.Text = "0";
                txt_bds_deposit.Text = "0";
                txt_bds_rental_term.Text = "0";
                txt_bds_floor_count.Text = "0";
                txt_bds_land_width.Text = "0";
                txt_bds_land_length.Text = "0";
                txt_bds_project.Text = "";
                txt_bds_district.Text = "";
                txt_bds_ward.Text = "";
                txt_bds_address_line.Text = "";

                DropDownList_cl.Return_Index_By_ID(ddl_DanhMuc, _idmenu);//đảm bảo ddl giữ nguyên khi nạp lại dữ liệu
                show_main();
                up_main.Update();

                Helper_Tabler_cl.ShowToast(this.Page, "Xử lý thành công", null, true, 2000, "Thông báo");
                #endregion
            }
            else//edit
            {
                string editId = (ViewState["edit_id"] ?? "").ToString();
                int editIdValue;
                if (string.IsNullOrEmpty(editId) || !int.TryParse(editId, out editIdValue))
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Không xác định được bài viết cần cập nhật.", "Thông báo", true, "warning");
                    return;
                }

                var q_edit = db.BaiViet_tbs.FirstOrDefault(p => p.id == editIdValue && p.nguoitao == _nguoitao);
                if (q_edit == null)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Bài viết không tồn tại hoặc bạn không có quyền cập nhật.", "Thông báo", true, "warning");
                    return;
                }

                var q_danhmuc = db.DanhMuc_tbs.FirstOrDefault(p => p.id.ToString() == _idmenu);
                if (q_danhmuc == null)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Danh mục không hợp lệ.", "Thông báo", true, "warning");
                    return;
                }

                BaiViet_tb _ob = q_edit;
                _ob.name = _name;
                _ob.name_en = _name_en;
                _ob.id_DanhMuc = _idmenu;
                _ob.id_DanhMucCap2 = "";
                _ob.content_post = _noidung;
                _ob.description = _description;
                _ob.image = _image;
                _ob.giaban = _giaban;
                _ob.ThanhPho = _thanhPho;
                _ob.LinkMap = _linkMap;

                if (!AccountVisibility_cl.IsServicePost(_ob))
                    _ob.phanloai = _phanloai_baiviet;

                _ob.PhanTram_GiamGia_ThanhToan_BangEvoucher = _phantram_uu_dai;
                CompanyShop_cl.SetPlatformSharePercent(_ob, appliedPlatformSharePercent);
                if (_isBatDongSan)
                {
                    BatDongSanMetadata_cl.Upsert(db, new BatDongSanMetadata_cl.PostMetadata
                    {
                        PostId = _ob.id,
                        ListingPurpose = _bds_purpose,
                        PropertyType = _bds_property_type,
                        AreaValue = _bds_area,
                        DepositAmount = _bds_deposit,
                        RentalTermMonths = _bds_rental_term,
                        FloorCount = _bds_floor_count,
                        LandWidth = _bds_land_width,
                        LandLength = _bds_land_length,
                        HouseDirection = _bds_direction,
                        LegalStatus = _bds_legal,
                        FurnishingStatus = _bds_furnishing,
                        BedroomCount = _bds_bedrooms,
                        BathroomCount = _bds_bathrooms,
                        ProjectName = _bds_project,
                        ProvinceName = _thanhPho,
                        DistrictName = _bds_district,
                        WardName = _bds_ward,
                        AddressLine = _bds_address_line
                    });
                }

                string ds_anhphu = (hf_anhphu.Value ?? "").Trim();
                if (!string.IsNullOrEmpty(ds_anhphu))
                {
                    var oldImgs = db.AnhSanPham_tbs.Where(x => x.idsp == _ob.id).ToList();
                    if (oldImgs.Count > 0)
                        db.AnhSanPham_tbs.DeleteAllOnSubmit(oldImgs);

                    string[] arrUrl = ds_anhphu.Split('|');
                    foreach (string url in arrUrl)
                    {
                        if (string.IsNullOrWhiteSpace(url))
                            continue;
                        AnhSanPham_tb ob_anh = new AnhSanPham_tb();
                        ob_anh.url = url.Trim();
                        ob_anh.idsp = (int?)_ob.id;
                        db.AnhSanPham_tbs.InsertOnSubmit(ob_anh);
                    }
                }

                db.SubmitChanges();
                ShopToAhaShinePostSync_cl.SyncTradePost(db, _ob);

                show_main();
                up_main.Update();
                pn_add.Visible = false;
                up_add.Update();
                reset_control_add_edit();
                Helper_Tabler_cl.ShowToast(this.Page, "Cập nhật thành công", null, true, 2000, "Thông báo");
            }
        }
    }
    #endregion

    protected void LinkButton1_Click(object sender, EventArgs e)//chuyển trạng thái thành đã bán
    {
        check_login_cl.check_login_home("none", "none", true);

        var selectedIds = new List<int>(); // Danh sách để lưu trữ ID của các mục đã được chọn

        // Thu thập tất cả ID của các mục đã được chọn trong Repeater1
        foreach (RepeaterItem item in Repeater1.Items)
        {
            CheckBox chkItem = (CheckBox)item.FindControl("checkID");
            Label lblData = (Label)item.FindControl("lbID");

            if (chkItem != null && lblData != null && chkItem.Checked)
            {
                int id = int.Parse(lblData.Text);
                selectedIds.Add(id); // Thêm ID vào danh sách
            }
        }

        if (selectedIds.Count > 0)
        {
            // Sử dụng dbDataContext và thực hiện cập nhật hàng loạt
            using (dbDataContext db = new dbDataContext())
            {
                // Lấy tất cả các mục có ID trong danh sách và cập nhật thuộc tính `bin` của chúng
                var danhMucsToUpdate = db.BaiViet_tbs
                    .Where(d => selectedIds.Contains(d.id))
                    .ToList();

                foreach (var dm in danhMucsToUpdate)
                {
                    dm.bin = false;//đang bán
                    ShopToAhaShinePostSync_cl.SyncTradePost(db, dm);
                }

                // Lưu tất cả các thay đổi trong một lần
                db.SubmitChanges();
            }

            // Hiển thị thông báo thành công
            show_main();
            Helper_Tabler_cl.ShowToast(this.Page, "Xử lý thành công", null, true, 2000, "Thông báo");
        }
        else
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Không có mục nào được chọn.", "Thông báo", true, "warning");
        }
    }

    protected void LinkButton2_Click(object sender, EventArgs e)//chuyển trạng thái thành ngưng bán
    {
        check_login_cl.check_login_home("none", "none", true);

        var selectedIds = new List<int>(); // Danh sách để lưu trữ ID của các mục đã được chọn

        // Thu thập tất cả ID của các mục đã được chọn trong Repeater1
        foreach (RepeaterItem item in Repeater1.Items)
        {
            CheckBox chkItem = (CheckBox)item.FindControl("checkID");
            Label lblData = (Label)item.FindControl("lbID");

            if (chkItem != null && lblData != null && chkItem.Checked)
            {
                int id = int.Parse(lblData.Text);
                selectedIds.Add(id); // Thêm ID vào danh sách
            }
        }

        if (selectedIds.Count > 0)
        {
            // Sử dụng dbDataContext và thực hiện cập nhật hàng loạt
            using (dbDataContext db = new dbDataContext())
            {
                // Lấy tất cả các mục có ID trong danh sách và cập nhật thuộc tính `bin` của chúng
                var danhMucsToUpdate = db.BaiViet_tbs
                    .Where(d => selectedIds.Contains(d.id))
                    .ToList();

                foreach (var dm in danhMucsToUpdate)
                {
                    dm.bin = true;//ngưng bán
                    ShopToAhaShinePostSync_cl.SyncTradePost(db, dm);
                }

                // Lưu tất cả các thay đổi trong một lần
                db.SubmitChanges();
            }

            // Hiển thị thông báo thành công
            show_main();
            Helper_Tabler_cl.ShowToast(this.Page, "Xử lý thành công", null, true, 2000, "Thông báo");
        }
        else
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Không có mục nào được chọn.", "Thông báo", true, "warning");
        }
    }
}
