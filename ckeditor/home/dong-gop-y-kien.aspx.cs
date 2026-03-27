using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_dong_gop_y_kien : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_home("none", "none", true); //check tài khoản, có chuyển hướng. YÊU CẦU ĐĂNG NHẬP.

            string _tk = Session["taikhoan_home"] as string;
            if (!string.IsNullOrEmpty(_tk))
            {
                ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk);
            }
        }
    }
    protected void btnGuiGopY_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {

            string taiKhoan = ViewState["taikhoan"].ToString();

            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == taiKhoan);

            if (string.IsNullOrEmpty(rdLoaiVanDe.SelectedValue)
                || string.IsNullOrEmpty(txtNoiDung.Text.Trim()))
            {
                Helper_Tabler_cl.ShowModal(
                    this.Page,
                    "Vui lòng nhập đầy đủ thông tin bắt buộc.",
                    "Thông báo",
                    true,
                    "warning"
                );
                return;
            }

            // 2. Lấy danh sách media đã upload (AJAX)
            List<string> listMedia = new List<string>();

            if (!string.IsNullOrEmpty(txt_ListMedia.Text))
            {
                listMedia = txt_ListMedia.Text
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            }

            if (listMedia.Count > 5)
            {
                Helper_Tabler_cl.ShowModal(
                    this.Page,
                    "Chỉ được tải tối đa 5 file.",
                    "Thông báo",
                    true,
                    "warning"
                );
                return;
            }

            // 3. Lưu ý kiến
            DongGopYKien_tb ykien = new DongGopYKien_tb
            {
                LoaiVanDe = rdLoaiVanDe.SelectedValue,
                ChiTietYKien = txtNoiDung.Text.Trim(),
                SoDienThoai = txtSoDienThoai.Text.Trim(),
                Media = 0,
                taikhoan = taiKhoan,
                HoTen = q.hoten,
                NgayTao = AhaTime_cl.Now  
            };

            db.DongGopYKien_tbs.InsertOnSubmit(ykien);
            db.SubmitChanges(); // lấy ykien.id

            int idYKien = ykien.id;
            int demMedia = 0;

            // 4. Lưu media + mapping
            foreach (string link in listMedia)
            {
                media_tb media = new media_tb
                {
                    link = link
                };
                db.media_tbs.InsertOnSubmit(media);
                db.SubmitChanges(); // lấy media.id

                MediaYKien_tb map = new MediaYKien_tb
                {
                    idYKien = idYKien,
                    idMedia = media.id
                };
                db.MediaYKien_tbs.InsertOnSubmit(map);
                db.SubmitChanges();

                demMedia++;
            }

            // 5. Update số lượng media
            if (demMedia > 0)
            {
                ykien.Media = demMedia;
                db.SubmitChanges();
            }

            // 6. Clear form
            rdLoaiVanDe.ClearSelection();
            txtNoiDung.Text = "";
            txtSoDienThoai.Text = "";
            txt_ListMedia.Text = "";

            ScriptManager.RegisterStartupScript(
                this.Page,
                this.GetType(),
                Guid.NewGuid().ToString(),
                thongbao_class.metro_notifi("Thông báo", "Gửi ý kiến thành công.", "1000", "success"),
                true
            );
        }
    }
}