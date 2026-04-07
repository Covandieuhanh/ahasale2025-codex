using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

public partial class home_quan_ly_bai_Them : System.Web.UI.Page
{
    private readonly DanhMuc_cl dm_cl = new DanhMuc_cl();
    private readonly String_cl str_cl = new String_cl();

    private static string NormalizeProvinceName(string value)
    {
        string name = (value ?? "").Trim();
        if (string.IsNullOrEmpty(name))
            return "";

        if (name.StartsWith("Tỉnh ", StringComparison.OrdinalIgnoreCase))
            name = name.Substring(5);
        else if (name.StartsWith("Thành phố ", StringComparison.OrdinalIgnoreCase))
            name = name.Substring(10);
        else if (name.StartsWith("TP. ", StringComparison.OrdinalIgnoreCase))
            name = name.Substring(4);
        else if (name.StartsWith("TP ", StringComparison.OrdinalIgnoreCase))
            name = name.Substring(3);

        return name.Trim();
    }

    private bool IsCurrentCompanyShopPortal()
    {
        using (dbDataContext db = new dbDataContext())
        {
            return CompanyShop_cl.IsCurrentPortalCompanyShop(db);
        }
    }

    private void BindCompanyShopOptions(bool isCompanyShopPortal)
    {
        ph_company_shop_options.Visible = isCompanyShopPortal;

        if (!isCompanyShopPortal)
            return;

        if (ddl_kenh_hienthi.Items.Count == 0)
        {
            ddl_kenh_hienthi.Items.Add(new ListItem("Công khai (hiển thị ngoài Home)", "public"));
            ddl_kenh_hienthi.Items.Add(new ListItem("Nội bộ (chỉ shop công ty tự bán)", "internal"));
        }

        if (ddl_kenh_hienthi.Items.FindByValue("public") != null)
            ddl_kenh_hienthi.SelectedValue = "public";
        txt_phantram_san.Text = "0";
    }

    private void BindLoaiTinOptions()
    {
        if (ddl_loai_tin.Items.Count > 0)
            return;

        ddl_loai_tin.Items.Add(new ListItem("Sản phẩm", AccountVisibility_cl.PostTypeProduct));
        ddl_loai_tin.Items.Add(new ListItem("Dịch vụ", AccountVisibility_cl.PostTypeService));
        ddl_loai_tin.SelectedValue = AccountVisibility_cl.PostTypeProduct;
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
    }

    private bool IsDuyetGianHangDoiTac()
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

    private string GetListUrl()
    {
        return PortalRequest_cl.IsShopPortalRequest()
            ? "/shop/quan-ly-tin"
            : "/home/quan-ly-tin/default.aspx";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        lnk_back_top.NavigateUrl = GetListUrl();
        lnk_cancel.NavigateUrl = GetListUrl();

        Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
        check_login_cl.check_login_home("none", "none", true);

        bool isCompanyShopPortal = IsCurrentCompanyShopPortal();
        ph_company_shop_options.Visible = isCompanyShopPortal;

        if (!IsDuyetGianHangDoiTac())
        {
            Session["home_modal_msg"] = "Tính năng này chỉ dành cho tài khoản đã đăng ký gian hàng đối tác thành công.";
            Session["home_modal_title"] = "Chưa đủ điều kiện";
            Session["home_modal_type"] = "warning";
            Response.Redirect(PortalRequest_cl.IsShopPortalRequest() ? "/shop/default.aspx" : "~/", true);
            return;
        }

        if (!IsPostBack)
        {
            txt_giaban.Text = "0";
            txt_phantram_uu_dai.Text = "0";
            LoadDanhMuc();
            BindCompanyShopOptions(isCompanyShopPortal);
            BindLoaiTinOptions();
            BindBatDongSanOptions();
            ApplyIncomingDefaults();
        }
    }

    private void LoadDanhMuc()
    {
        dm_cl.Show_DanhMuc(2, 3, ddl_DanhMuc, false, "web", "135");
        using (dbDataContext db = new dbDataContext())
        {
            hf_bds_category_ids.Value = BatDongSanMetadata_cl.GetRealEstateCategoryIdsCsv(db);
        }
    }

    private void ApplyIncomingDefaults()
    {
        string category = (Request.QueryString["category"] ?? "").Trim().ToLowerInvariant();
        string purpose = (Request.QueryString["purpose"] ?? "").Trim().ToLowerInvariant();

        if (purpose == "rent" && ddl_bds_purpose.Items.FindByValue("rent") != null)
            ddl_bds_purpose.SelectedValue = "rent";

        if (category != "bat-dong-san")
            return;

        using (dbDataContext db = new dbDataContext())
        {
            string bdsCategoryId = BatDongSanMetadata_cl.FindRealEstateCategoryId(db);
            if (bdsCategoryId != "" && ddl_DanhMuc.Items.FindByValue(bdsCategoryId) != null)
                ddl_DanhMuc.SelectedValue = bdsCategoryId;
        }
    }

    protected void but_submit_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);

        if (!Directory.Exists(Server.MapPath("~/uploads/img-handler/")))
            Directory.CreateDirectory(Server.MapPath("~/uploads/img-handler/"));

        string name = str_cl.Remove_Blank((txt_name.Text ?? "").Trim());
        string nameEn = str_cl.replace_name_to_url(name);
        string idMenu = (ddl_DanhMuc.SelectedValue ?? "").Trim();
        string description = (txt_description.Text ?? "").Trim();
        string noiDung = (txt_noidung.Text ?? "").Trim();
        string image = (txt_link_fileupload.Text ?? "").Trim();
        string linkMap = (LinkMap.Text ?? "").Trim();
        string tinh = (hf_tinh.Value ?? "").Trim();
        string quan = (hf_quan.Value ?? "").Trim();
        string phuong = (hf_phuong.Value ?? "").Trim();
        string chiTiet = (txt_diachi_chitiet.Text ?? "").Trim();
        string fullAddress = AddressFormat_cl.BuildFullAddress(chiTiet, phuong, quan, tinh);
        string thanhPho = NormalizeProvinceName(tinh);
        long giaBan = Number_cl.Check_Int64((txt_giaban.Text ?? "").Trim());
        if (giaBan < 0) giaBan = 0;

        int phanTramUuDai = 0;
        int.TryParse((txt_phantram_uu_dai.Text ?? "").Trim(), out phanTramUuDai);
        if (phanTramUuDai < 0) phanTramUuDai = 0;

        int phanTramChoSan = 0;
        int.TryParse((txt_phantram_san.Text ?? "").Trim(), out phanTramChoSan);
        phanTramChoSan = CompanyShop_cl.ClampPlatformSharePercent(phanTramChoSan);

        string selectedProductType = (ddl_kenh_hienthi.SelectedValue ?? "").Trim();
        string selectedLoaiTin = (ddl_loai_tin.SelectedValue ?? "").Trim();

        string nguoiTao = PortalRequest_cl.GetCurrentAccount();
        if (idMenu == "")
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng chọn Danh mục.", "Thông báo", true, "warning");
            return;
        }
        if (name == "")
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập tên tin.", "Thông báo", true, "warning");
            return;
        }
        if (image == "")
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng chọn ảnh hoặc video tin.", "Thông báo", true, "warning");
            return;
        }
        if (noiDung == "")
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập nội dung.", "Thông báo", true, "warning");
            return;
        }
        if (giaBan == 0)
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập giá.", "Thông báo", true, "warning");
            return;
        }
        if (string.IsNullOrEmpty(tinh) || string.IsNullOrEmpty(quan) || string.IsNullOrEmpty(phuong))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng chọn đầy đủ Tỉnh/Thành, Quận/Huyện và Phường/Xã.", "Thông báo", true, "warning");
            return;
        }
        if (chiTiet.Length < 4)
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập địa chỉ chi tiết.", "Thông báo", true, "warning");
            return;
        }
        if (phanTramUuDai > 50)
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Phần trăm ưu đãi tối đa là 50%. Vui lòng nhập lại.", "Thông báo", true, "warning");
            return;
        }

        if (string.IsNullOrEmpty(linkMap) && !string.IsNullOrEmpty(fullAddress))
        {
            linkMap = "https://www.google.com/maps/search/?api=1&query=" + HttpUtility.UrlEncode(fullAddress);
        }

        using (dbDataContext db = new dbDataContext())
        {
            bool isCompanyShop = CompanyShop_cl.IsCompanyShopAccount(db, nguoiTao);
            string phanloaiBaiViet = CompanyShop_cl.NormalizeProductType(selectedProductType, isCompanyShop);
            bool isShopPortal = PortalRequest_cl.IsShopPortalRequest();
            int appliedPlatformSharePercent = isCompanyShop ? phanTramChoSan : 0;
            if (isShopPortal)
            {
                int policyPercent;
                if (ShopPolicy_cl.TryGetActivePolicyPercent(db, nguoiTao, out policyPercent))
                {
                    appliedPlatformSharePercent = policyPercent;
                }
                else if (!isCompanyShop)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Shop của bạn chưa có chính sách % chiết khấu mặc định. Vui lòng gửi/duyệt yêu cầu mở không gian shop trước khi đăng tin.", "Thông báo", true, "warning");
                    return;
                }
            }

            bool isInternal = CompanyShop_cl.IsInternalProductType(phanloaiBaiViet);
            if (string.Equals(selectedLoaiTin, AccountVisibility_cl.PostTypeService, StringComparison.OrdinalIgnoreCase))
            {
                if (isInternal)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Không gian nội bộ chỉ hỗ trợ sản phẩm. Vui lòng chuyển về Công khai để đăng dịch vụ.", "Thông báo", true, "warning");
                    return;
                }

                phanloaiBaiViet = AccountVisibility_cl.PostTypeService;
            }
            else
            {
                if (!isInternal)
                    phanloaiBaiViet = AccountVisibility_cl.PostTypeProduct;
            }

            var qDanhMuc = db.DanhMuc_tbs.FirstOrDefault(p => p.id.ToString() == idMenu);
            if (qDanhMuc == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Danh mục không hợp lệ.", "Thông báo", true, "warning");
                return;
            }

            bool isBatDongSan = BatDongSanMetadata_cl.IsRealEstateCategory(db, idMenu);
            string bdsPurpose = BatDongSanMetadata_cl.NormalizePurpose(ddl_bds_purpose.SelectedValue);
            string bdsPropertyType = BatDongSanMetadata_cl.NormalizePropertyType(ddl_bds_property_type.SelectedValue);
            decimal bdsAreaValue = BatDongSanMetadata_cl.ParseArea(txt_bds_area.Text);
            int bdsBedrooms = Number_cl.Check_Int32((txt_bds_bedrooms.Text ?? "").Trim());
            int bdsBathrooms = Number_cl.Check_Int32((txt_bds_bathrooms.Text ?? "").Trim());
            string bdsLegal = (ddl_bds_legal.SelectedValue ?? "").Trim();
            string bdsFurnishing = (ddl_bds_furnishing.SelectedValue ?? "").Trim();
            string bdsProject = (txt_bds_project.Text ?? "").Trim();
            decimal bdsDepositAmount = BatDongSanMetadata_cl.ParseDecimal(txt_bds_deposit.Text);
            int bdsRentalTermMonths = Number_cl.Check_Int32((txt_bds_rental_term.Text ?? "").Trim());
            int bdsFloorCount = Number_cl.Check_Int32((txt_bds_floor_count.Text ?? "").Trim());
            decimal bdsLandWidth = BatDongSanMetadata_cl.ParseDecimal(txt_bds_land_width.Text);
            decimal bdsLandLength = BatDongSanMetadata_cl.ParseDecimal(txt_bds_land_length.Text);
            string bdsDirection = (ddl_bds_direction.SelectedValue ?? "").Trim();

            if (isBatDongSan)
            {
                if (bdsAreaValue <= 0)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Tin bất động sản bắt buộc phải có diện tích hợp lệ.", "Thông báo", true, "warning");
                    return;
                }

                if (string.Equals(bdsPropertyType, "land", StringComparison.OrdinalIgnoreCase))
                {
                    bdsBedrooms = 0;
                    bdsBathrooms = 0;
                    bdsFurnishing = "Chưa cập nhật";
                    if (bdsLandWidth <= 0 || bdsLandLength <= 0)
                    {
                        Helper_Tabler_cl.ShowModal(this.Page, "Đất nền cần nhập Ngang và Dài hợp lệ.", "Thông báo", true, "warning");
                        return;
                    }
                }

                if (string.Equals(bdsPropertyType, "office", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(bdsPropertyType, "business-premises", StringComparison.OrdinalIgnoreCase))
                {
                    bdsBedrooms = 0;
                }

                if (string.Equals(bdsPropertyType, "house", StringComparison.OrdinalIgnoreCase))
                {
                    if (bdsFloorCount <= 0)
                    {
                        Helper_Tabler_cl.ShowModal(this.Page, "Nhà phố cần nhập số tầng.", "Thông báo", true, "warning");
                        return;
                    }
                }
                else
                {
                    bdsFloorCount = 0;
                }

                if (!string.Equals(bdsPropertyType, "land", StringComparison.OrdinalIgnoreCase))
                {
                    bdsLandWidth = 0;
                    bdsLandLength = 0;
                }

                if (!string.Equals(bdsPurpose, "rent", StringComparison.OrdinalIgnoreCase))
                {
                    bdsDepositAmount = 0;
                    bdsRentalTermMonths = 0;
                }
                else
                {
                    if (bdsDepositAmount <= 0 || bdsRentalTermMonths <= 0)
                    {
                        Helper_Tabler_cl.ShowModal(this.Page, "Tin cho thuê cần nhập tiền cọc và kỳ hạn thuê.", "Thông báo", true, "warning");
                        return;
                    }
                }

                selectedLoaiTin = AccountVisibility_cl.PostTypeProduct;
                if (!isInternal)
                    phanloaiBaiViet = AccountVisibility_cl.PostTypeProduct;
            }

            BaiViet_tb ob = new BaiViet_tb();
            ob.name = name;
            ob.name_en = nameEn;
            ob.id_DanhMuc = idMenu;
            ob.id_DanhMucCap2 = "";
            ob.content_post = noiDung;
            ob.description = description;
            ob.image = image;
            ob.bin = false;
            ob.ngaytao = AhaTime_cl.Now;
            ob.nguoitao = nguoiTao;
            ob.noibat = false;
            ob.giaban = giaBan;
            ob.giavon = 0;
            ob.soluong_tonkho = 0;
            ob.banhang_thuong = 0;
            ob.chotsale_thuong = 0;
            ob.phanloai = phanloaiBaiViet;
            ob.banhang_phantram_hoac_tien = true;
            ob.chotsale_phantram_hoac_tien = true;
            ob.soluong_daban = 0;
            ob.ThanhPho = thanhPho;
            ob.LinkMap = linkMap;
            ob.PhanTram_GiamGia_ThanhToan_BangEvoucher = phanTramUuDai;
            CompanyShop_cl.SetPlatformSharePercent(ob, appliedPlatformSharePercent);
            db.BaiViet_tbs.InsertOnSubmit(ob);
            db.SubmitChanges();

            if (isBatDongSan)
            {
                BatDongSanMetadata_cl.Upsert(db, new BatDongSanMetadata_cl.PostMetadata
                {
                    PostId = ob.id,
                    ListingPurpose = bdsPurpose,
                    PropertyType = bdsPropertyType,
                    AreaValue = bdsAreaValue,
                    DepositAmount = bdsDepositAmount,
                    RentalTermMonths = bdsRentalTermMonths,
                        FloorCount = bdsFloorCount,
                        LandWidth = bdsLandWidth,
                        LandLength = bdsLandLength,
                        HouseDirection = bdsDirection,
                        LegalStatus = bdsLegal,
                        FurnishingStatus = bdsFurnishing,
                        BedroomCount = bdsBedrooms,
                    BathroomCount = bdsBathrooms,
                    ProjectName = bdsProject,
                    ProvinceName = tinh,
                    DistrictName = quan,
                    WardName = phuong,
                    AddressLine = fullAddress
                });
            }

            ShopToAhaShinePostSync_cl.SyncTradePost(db, ob);

            string dsAnhPhu = (hf_anhphu.Value ?? "").Trim();
            if (!string.IsNullOrEmpty(dsAnhPhu))
            {
                string[] arrUrl = dsAnhPhu.Split('|');
                foreach (string url in arrUrl)
                {
                    string cleanUrl = (url ?? "").Trim();
                    if (cleanUrl == "") continue;
                    AnhSanPham_tb obAnh = new AnhSanPham_tb();
                    obAnh.url = cleanUrl;
                    obAnh.idsp = (int?)ob.id;
                    db.AnhSanPham_tbs.InsertOnSubmit(obAnh);
                }
                db.SubmitChanges();
            }
        }

        Session["thongbao_home"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng tin thành công.", "1000", "success");
        Response.Redirect(GetListUrl(), false);
        Context.ApplicationInstance.CompleteRequest();
    }
}
