using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class taikhoan_add : System.Web.UI.Page
{
    public string notifi, user, user_parent;
    dbDataContext db = new dbDataContext();
    string_class str_cl = new string_class();
    random_class rd_cl = new random_class();
    datetime_class dt_cl = new datetime_class();
    nganh_class ng_cl = new nganh_class();
    private bool HasAnyPermission(params string[] permissionKeys)
    {
        if (string.IsNullOrWhiteSpace(user) || permissionKeys == null)
            return false;

        for (int i = 0; i < permissionKeys.Length; i++)
        {
            string permissionKey = (permissionKeys[i] ?? "").Trim();
            if (permissionKey != "" && bcorn_class.check_quyen(user, permissionKey) == "")
                return true;
        }

        return false;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        #region Check quyen theo nganh
        user = (access.User ?? "").Trim();
        user_parent = access.OwnerAccountKey;
        if (HasAnyPermission("q15_2", "n15_2"))
        {
            if (!IsPostBack)
            {
                load_nganh();
            }
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        #endregion
    }
    public void load_nganh()
    {
        var list_nganh = (from ob1 in db.nganh_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                          select new { id = ob1.id, ten = ob1.ten, }
                               );
        DropDownList5.DataSource = list_nganh;
        DropDownList5.DataTextField = "ten";
        DropDownList5.DataValueField = "id";
        DropDownList5.DataBind();
        DropDownList5.Items.Insert(0, new ListItem("Chọn", ""));
        if (HasAnyPermission("q15_2"))
        {
            
        }
        else
        {
            DropDownList5.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
            DropDownList5.Enabled = false;
        }

    }
    protected void button1_Click1(object sender, EventArgs e)
    {
        if (!HasAnyPermission("q15_2", "n15_2"))
        {
            notifi = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để thêm chuyên gia.", "false", "false", "OK", "alert", "");
            return;
        }

        string _fullname = str_cl.VietHoa_ChuCai_DauTien(str_cl.remove_blank(txt_hoten.Text.Trim().ToLower()));
        string _ngaysinh = txt_ngaysinh.Text;
        string _email = txt_email.Text.ToLower().Trim();
        string _sdt = txt_dienthoai.Text.Trim();
        string _zalo = txt_zalo.Text.Trim();
        string _facebook = txt_facebook.Text.ToLower().Trim();
        string _nganhhoc = DropDownList5.SelectedValue.ToString();

        string _sobuoi_lt = txt_sobuoi_lythuyet.Text.Trim().Replace(".", "").Replace(",", ""); int _r1 = 0; int.TryParse(_sobuoi_lt, out _r1); if (_r1 < 0) _r1 = 0;
        string _sobuoi_th = txt_sobuoi_thuchanh.Text.Trim().Replace(".", "").Replace(",", ""); int _r2 = 0; int.TryParse(_sobuoi_th, out _r2); if (_r2 < 0) _r2 = 0;
        string _sobuoi_tg = txt_sobuoi_trogiang.Text.Trim().Replace(".", "").Replace(",", ""); int _r3 = 0; int.TryParse(_sobuoi_tg, out _r3); if (_r3 < 0) _r3 = 0;

        if (_fullname == "")
            notifi = thongbao_class.metro_dialog_onload("Thông báo", "Vui lòng nhập họ tên.", "false", "false", "OK", "warning", "");
        else
        {
            if (_nganhhoc == "")
                notifi = thongbao_class.metro_dialog_onload("Thông báo", "Vui lòng chọn ngành", "false", "false", "OK", "warning", "");
            else
            {
                if (_ngaysinh != "" && dt_cl.check_date(_ngaysinh) == false)//nếu có nhập mới kiểm tra
                    notifi = thongbao_class.metro_notifi_onload("Thông báo", "Ngày sinh không hợp lệ.", "4000", "warning");
                else
                {
                    if (!regex_class.check_email_invalid(_email) && _email != "")//nếu có nhập mail thì kiểm tra định dạng mail
                        notifi = thongbao_class.metro_notifi_onload("Thông báo", "Email không hợp lệ.", "4000", "warning");
                    else
                    {
                        giangvien_table _ob1 = new giangvien_table();

                        bool _checkloi = false;
                        string _avt = "/uploads/images/macdinh.jpg";

                        if (FileUpload2.HasFile)//nếu có ảnh thu nhỏ đc chọn
                        {
                            string _ext = Path.GetExtension(FileUpload2.FileName).ToLower();
                            if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
                            {
                                //byte - kb - mb  ContentLength trra về byte của file
                                long _filesize = (FileUpload2.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                                if (_filesize <= 1) //>1MB
                                {
                                    _avt = "/uploads/images/avatar/" + Guid.NewGuid() + _ext;
                                    FileUpload2.SaveAs(Server.MapPath("~" + _avt));//lưu ảnh mới
                                }
                                else
                                {
                                    notifi = thongbao_class.metro_dialog_onload("Thông báo", "Kích thước ảnh đại diện quá lớn. Vui lòng chọn ảnh có dung lượng <1Mb.", "false", "false", "OK", "alert", "");
                                    _checkloi = true;
                                }
                            }
                            else
                            {
                                notifi = thongbao_class.metro_dialog_onload("Thông báo", "Ảnh đại diện không đúng định dạng.", "false", "false", "OK", "alert", "");
                                _checkloi = true;
                            }
                        }
                        #region chỉ dành riêng khi tạo
                        _ob1.nguoitao = user;
                        _ob1.ngaytao = DateTime.Now;
                        _ob1.id_chinhanh = Session["chinhanh"].ToString();
                        #endregion

                        _ob1.hoten = _fullname;
                        _ob1.hoten_khongdau = str_cl.remove_vietnamchar(_fullname);
                        _ob1.anhdaidien = _avt;
                        _ob1.trangthai = DropDownList1.SelectedValue.ToString();
                        _ob1.email = _email;
                        _ob1.dienthoai = _sdt;
                        _ob1.zalo = _zalo;
                        _ob1.facebook = _facebook;
                        if (_ngaysinh != "")
                            _ob1.ngaysinh = DateTime.Parse(_ngaysinh);
                        else
                            _ob1.ngaysinh = null;
                        //_ob1.chuyenmon = txt_chuyenmon.Text.Trim();
                        _ob1.chuyenmon = _nganhhoc;
                        _ob1.sobuoi_lythuyet = _r1; _ob1.sobuoi_thuchanh = _r2; _ob1.sobuoi_trogiang = _r2;
                        _ob1.danhgiagiangvien = txt_danhgia.Text.Trim();
                        _ob1.goidaotao = DropDownList2.SelectedValue.ToString();

                        if (_checkloi == false)
                        {
                            db.giangvien_tables.InsertOnSubmit(_ob1);
                            db.SubmitChanges();
                            GianHangAdminPersonHub_cl.SyncSourcePhoneState(db, user_parent, "", _sdt, _fullname, user);
                            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Thêm Chuyên gia thành công.", "4000", "warning");
                            Response.Redirect("/gianhang/admin/quan-ly-giang-vien/Default.aspx");
                        }

                    }
                }
            }
        }
    }

    protected void but_form_themnhomthuchi_Click(object sender, EventArgs e)
    {
        string _tennhom = txt_tennhom.Text.Trim();

        if (_tennhom == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập tên ngành.", "4000", "warning"), true);
        else
        {

            nganh_table _ob = new nganh_table();
            _ob.ten = _tennhom;
            _ob.id_chinhanh = Session["chinhanh"].ToString();
            db.nganh_tables.InsertOnSubmit(_ob);
            db.SubmitChanges();
            txt_tennhom.Text = "";
            load_nganh();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Thêm ngành thành công.", "4000", "warning"), true);
            //Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Thêm nhóm khách hàng thành công.", "2000", "warning");
            //Response.Redirect("/gianhang/admin/quan-ly-thu-chi/nhom-thu-chi.aspx");
        }
    }
}
