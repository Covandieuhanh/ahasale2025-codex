using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

public partial class home_quan_ly_bai_Them : System.Web.UI.Page
{
    private readonly DanhMuc_cl dm_cl = new DanhMuc_cl();
    private readonly String_cl str_cl = new String_cl();

    private bool IsDuyetGianHangDoiTac()
    {
        string tkEnc = PortalRequest_cl.GetCurrentAccountEncrypted();
        if (string.IsNullOrEmpty(tkEnc))
            return false;

        string tk = mahoa_cl.giaima_Bcorn(tkEnc);
        using (dbDataContext db = new dbDataContext())
        {
            return db.DangKy_GianHangDoiTac_tbs.Any(x => x.taikhoan == tk && x.TrangThai == 1);
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
            LoadThanhPho();
        }
    }

    private void LoadDanhMuc()
    {
        dm_cl.Show_DanhMuc(2, 3, ddl_DanhMuc, false, "web", "135");
    }

    private void LoadThanhPho()
    {
        using (dbDataContext db = new dbDataContext())
        {
            var thanhPhos = db.ThanhPhos.ToList();
            DanhSachTP.Items.Clear();
            DanhSachTP.Items.Add(new ListItem("Nhấn để chọn", ""));
            foreach (var tp in thanhPhos)
            {
                DanhSachTP.Items.Add(new ListItem(tp.Ten, tp.Ten));
            }
        }
    }

    protected void but_submit_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);

        string phanloaiBaiViet = "sanpham";
        if (!Directory.Exists(Server.MapPath("~/uploads/img-handler/")))
            Directory.CreateDirectory(Server.MapPath("~/uploads/img-handler/"));

        string name = str_cl.Remove_Blank((txt_name.Text ?? "").Trim());
        string nameEn = str_cl.replace_name_to_url(name);
        string idMenu = (ddl_DanhMuc.SelectedValue ?? "").Trim();
        string description = (txt_description.Text ?? "").Trim();
        string noiDung = (txt_noidung.Text ?? "").Trim();
        string image = (txt_link_fileupload.Text ?? "").Trim();
        string linkMap = (LinkMap.Text ?? "").Trim();
        string thanhPho = (DanhSachTP.SelectedValue ?? "").Trim();
        long giaBan = Number_cl.Check_Int64((txt_giaban.Text ?? "").Trim());
        if (giaBan < 0) giaBan = 0;

        int phanTramUuDai = 0;
        int.TryParse((txt_phantram_uu_dai.Text ?? "").Trim(), out phanTramUuDai);
        if (phanTramUuDai < 0) phanTramUuDai = 0;

        string nguoiTao = PortalRequest_cl.GetCurrentAccount();
        if (idMenu == "")
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng chọn Danh mục.", "Thông báo", true, "warning");
            return;
        }
        if (name == "")
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập tên sản phẩm.", "Thông báo", true, "warning");
            return;
        }
        if (image == "")
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng chọn ảnh hoặc video sản phẩm.", "Thông báo", true, "warning");
            return;
        }
        if (noiDung == "")
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập nội dung.", "Thông báo", true, "warning");
            return;
        }
        if (giaBan == 0)
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập giá bán.", "Thông báo", true, "warning");
            return;
        }
        if (phanTramUuDai > 50)
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Phần trăm ưu đãi tối đa là 50%. Vui lòng nhập lại.", "Thông báo", true, "warning");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            var qDanhMuc = db.DanhMuc_tbs.FirstOrDefault(p => p.id.ToString() == idMenu);
            if (qDanhMuc == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Danh mục không hợp lệ.", "Thông báo", true, "warning");
                return;
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
            db.BaiViet_tbs.InsertOnSubmit(ob);
            db.SubmitChanges();

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
