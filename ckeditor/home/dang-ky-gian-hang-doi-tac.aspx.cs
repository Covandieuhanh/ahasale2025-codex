using System;
using System.Linq;

public partial class home_dang_ky_gian_hang_doi_tac : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Redirect("/shop/dang-ky.aspx", false);
        Context.ApplicationInstance.CompleteRequest();
    }

    void LoadAll()
    {
        string tk = mahoa_cl.giaima_Bcorn(Session["taikhoan_home"] + "");
        if (string.IsNullOrEmpty(tk)) return;

        using (dbDataContext db = new dbDataContext())
        {
            // Lấy trạng thái mới nhất (nếu có)
            var last = db.DangKy_GianHangDoiTac_tbs
                .Where(x => x.taikhoan == tk)
                .OrderByDescending(x => x.NgayTao)
                .FirstOrDefault();

            RenderTrangThaiUI(last);

            // Lịch sử
            var ls = db.DangKy_GianHangDoiTac_tbs
                .Where(x => x.taikhoan == tk)
                .OrderByDescending(x => x.NgayTao)
                .Select(x => new
                {
                    x.NgayTao,
                    x.TrangThai,
                    x.GhiChuAdmin,
                    TrangThaiText =
                        x.TrangThai == 0 ? "Chờ duyệt" :
                        x.TrangThai == 1 ? "Đã duyệt" : "Từ chối"
                })
                .ToList();

            rp_lichsu.DataSource = ls;
            rp_lichsu.DataBind();
        }
    }

    void RenderTrangThaiUI(dynamic last)
    {
        // default: chưa có đăng ký
        string html = "<div class='alert alert-info mb-0'>Bạn chưa có đăng ký nào.</div>";
        bool enableButton = true;

        if (last != null)
        {
            int st = (int)last.TrangThai;

            if (st == 0)
            {
                html = "<div class='alert alert-warning mb-0'>Đăng ký của bạn đang chờ admin duyệt.</div>";
                enableButton = false; // đang chờ -> không cho bấm
            }
            else if (st == 1)
            {
                html = "<div class='alert alert-success mb-0'>Bạn đã là gian hàng đối tác (đã duyệt). Không thể đăng ký lại.</div>";
                enableButton = false; // đã duyệt -> chặn vĩnh viễn
            }
            else // st == 2
            {
                html = "<div class='alert alert-danger mb-0'>Đăng ký trước đó đã bị từ chối. Bạn có thể đăng ký lại.</div>";
                enableButton = true; // bị từ chối -> cho đăng ký lại
            }
        }

        ltr_trangthai.Text = html;
        but_dangky.Enabled = enableButton;

        // Option: nếu disabled thì thêm class cho đúng style (không bắt buộc)
        // if (!enableButton) but_dangky.CssClass = "btn btn-success disabled";
    }

    protected void but_dangky_Click(object sender, EventArgs e)
    {
        // bắt buộc đồng ý điều khoản
        if (!chk_dongy.Checked)
        {
            Helper_Tabler_cl.ShowModal(this,
                "Bạn phải đồng ý với điều khoản và chính sách trước khi đăng ký.",
                "Chưa xác nhận", true, "warning");
            return;
        }

        string tk = Session["taikhoan_home"] as string;
        if (!string.IsNullOrEmpty(tk)) tk = mahoa_cl.giaima_Bcorn(tk);
        else return;

        using (dbDataContext db = new dbDataContext())
        {
            // chặn nếu đã duyệt (1 người chỉ được đăng ký 1 lần nếu đã được duyệt)
            bool daDuyet = db.DangKy_GianHangDoiTac_tbs
                .Any(x => x.taikhoan == tk && x.TrangThai == 1);
            if (daDuyet)
            {
                Helper_Tabler_cl.ShowModal(this,
                    "Bạn đã là gian hàng đối tác (đã duyệt), không thể đăng ký lại.",
                    "Không thể đăng ký", true, "info");
                LoadAll();
                return;
            }

            // chặn nếu đang có yêu cầu chờ
            bool dangCho = db.DangKy_GianHangDoiTac_tbs
                .Any(x => x.taikhoan == tk && x.TrangThai == 0);
            if (dangCho)
            {
                Helper_Tabler_cl.ShowModal(this,
                    "Bạn đang có một đăng ký chờ admin duyệt. Không thể gửi thêm đăng ký mới.",
                    "Đã tồn tại đăng ký", true, "warning");
                LoadAll();
                return;
            }

            // insert đăng ký mới
            var dk = new DangKy_GianHangDoiTac_tb
            {
                taikhoan = tk,
                TrangThai = 0,
                NgayTao = AhaTime_cl.Now,
                GhiChuAdmin = null,
                NgayDuyet = null,
                AdminDuyet = null
            };

            db.DangKy_GianHangDoiTac_tbs.InsertOnSubmit(dk);
            db.SubmitChanges();
        }

        Helper_Tabler_cl.ShowToast(this,
            "Đã gửi đăng ký, vui lòng chờ admin duyệt.",
            "success", true, 2500, "Thành công");

        LoadAll();
    }
}
