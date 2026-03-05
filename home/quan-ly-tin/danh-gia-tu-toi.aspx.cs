using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_danh_gia_tu_toi : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string _tk = PortalRequest_cl.GetCurrentAccountEncrypted();
            if (!string.IsNullOrEmpty(_tk))
                ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk);
            else
                ViewState["taikhoan"] = "";
            string taiKhoanHienTai = ViewState["taikhoan"]?.ToString() ?? "";
            using (dbDataContext db = new dbDataContext())
            {
                var data =
                    from dg in db.DanhGiaBaiViets
                        .Where(x => x.TaiKhoanDanhGia == taiKhoanHienTai)
                    join bv in db.BaiViet_tbs
                        .Where(x => x.phanloai == "sanpham")
                        on dg.idBaiViet equals bv.id.ToString() into BaiVietGroup
                    from bv in BaiVietGroup.DefaultIfEmpty()
                    orderby dg.NgayDang descending
                    select new
                    {
                        BaiVietId = bv != null ? bv.id : 0,
                        TenSanPham = bv != null ? bv.name : "",
                        Gia = bv != null ? bv.giaban : 0,
                        AnhChinh = bv != null ? bv.image : "",
                        DanhGiaId = dg.id,
                        dg.Diem,
                        dg.NoiDung,
                        dg.NgayDang,
                        dg.UrlAnh
                    };

                rptDanhGiaCuaToi.DataSource = data;
                rptDanhGiaCuaToi.DataBind();

                phEmpty.Visible = data.Count() == 0;
            }
        }
    }
}
public class DanhGiaViewModel
{
    public int BaiVietId { get; set; }
    public string TieuDe { get; set; }
    public decimal Gia { get; set; }
    public string AnhChinh { get; set; }

    public int DanhGiaId { get; set; }
    public int Diem { get; set; }
    public string NoiDung { get; set; }
    public DateTime NgayDang { get; set; }
    public string UrlAnh { get; set; }
}
