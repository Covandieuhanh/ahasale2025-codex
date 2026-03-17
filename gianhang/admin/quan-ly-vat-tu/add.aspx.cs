using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_temp : System.Web.UI.Page
{
    public string notifi, url_back, user, user_parent;
    dbDataContext db = new dbDataContext();

    protected void Page_Load(object sender, EventArgs e)
    {
        #region Check_Login
        string _quyen = "q13_9";
        string _cookie_user = "", _cookie_pass = "";
        if (Request.Cookies["save_user_admin_aka_1"] != null) _cookie_user = Request.Cookies["save_user_admin_aka_1"].Value;
        if (Request.Cookies["save_pass_admin_aka_1"] != null) _cookie_pass = Request.Cookies["save_pass_admin_aka_1"].Value;
        if (Session["user"] == null) Session["user"] = ""; if (Session["notifi"] == null) Session["notifi"] = ""; if (Session["user"].ToString() == "") Response.Redirect("/gianhang/admin/f5_ss_admin.aspx");
        string _url = Request.Url.GetLeftPart(UriPartial.Authority).ToLower();
        string _kq = bcorn_class.check_login(Session["user"].ToString(), _cookie_user, _cookie_pass, _url, _quyen);
        if (_kq != "")//nếu có thông báo --> có lỗi --> reset --> bắt login lại
        {
            if (_kq == "baotri") Response.Redirect("/baotri.aspx");
            else
            {
                if (_kq == "1") Response.Redirect("/gianhang/admin/login.aspx");//hết Session, hết Cookie
                else
                {
                    if (_kq == "2")//k đủ quyền
                    {
                        Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
                        Response.Redirect("/gianhang/admin");
                    }
                    else
                    {
                        Session["notifi"] = _kq; Session["user"] = "";
                        Response.Cookies["save_user_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["save_pass_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["save_url_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Redirect("/gianhang/admin/login.aspx");
                    }
                }
            }
        }
        #endregion
        user = Session["user"].ToString();
        user_parent = "admin";

        if (!IsPostBack)
        {
            load_nhom();
        }

        if (Request.Cookies["save_url_admin_aka_1"] != null)
            url_back = Request.Cookies["save_url_admin_aka_1"].Value;
        else
            url_back = "/gianhang/admin/quan-ly-vat-tu/Default.aspx";
    }
    public void load_nhom()
    {

        var list_nhom = (from ob1 in db.nhomvattu_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                         select new { id = ob1.id, tennhom = ob1.tennhom, }
                                );
        ddl_nhom.DataSource = list_nhom.OrderBy(p => p.tennhom);
        ddl_nhom.DataTextField = "tennhom";
        ddl_nhom.DataValueField = "id";
        ddl_nhom.DataBind();
        ddl_nhom.Items.Insert(0, new ListItem("Chọn", "0"));

        var list_nhacungcap = (from ob1 in db.nhacungcap_nhaphang_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                               select new { id = ob1.id, ten = ob1.ten, }
                     );
        DropDownList3.DataSource = list_nhacungcap;
        DropDownList3.DataTextField = "ten";
        DropDownList3.DataValueField = "id";
        DropDownList3.DataBind();
        DropDownList3.Items.Insert(0, new ListItem("Chọn", ""));

        var list_phongban = (from ob1 in db.phongban_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                               select new { id = ob1.id, ten = ob1.ten, }
                     );
        DropDownList2.DataSource = list_phongban;
        DropDownList2.DataTextField = "ten";
        DropDownList2.DataValueField = "id";
        DropDownList2.DataBind();
        DropDownList2.Items.Insert(0, new ListItem("Chọn", ""));

    }

    #region button click    
    protected void button1_Click(object sender, EventArgs e)
    {
        if (!Directory.Exists(Server.MapPath("~/uploads/images/vat-tu/")))
            Directory.CreateDirectory(Server.MapPath("~/uploads/images/vat-tu/"));
        bool _checkloi = false;
        string _name = txt_name.Text;
        string _ghichu = txt_ghichu.Text.Trim();
        string _id_nhom = ddl_nhom.SelectedValue.ToString();

        #region sản phẩm
        string _gia_sp_text = txt_giasanpham.Text.Trim().Replace(".", "").Replace(",", "");
        Int64 _gia_sp = 0; Int64.TryParse(_gia_sp_text, out _gia_sp); if (_gia_sp <= 0) _gia_sp = 0;

        string _giavon_sp_text = txt_giavonsanpham.Text.Trim().Replace(".", "").Replace(",", "");
        int _giavon_sp = 0; int.TryParse(_giavon_sp_text, out _giavon_sp); if (_giavon_sp <= 0) _giavon_sp = 0;

        #endregion

        if (_name == "")
        {
            notifi = thongbao_class.metro_dialog_onload("Thông báo", "Vui lòng nhập tên vật tư.", "false", "false", "OK", "alert", "");
        }
        else
        {

            danhsach_vattu_table _ob = new danhsach_vattu_table();
            _ob.ngaytao = DateTime.Now;
            _ob.nguoitao = user;
            _ob.tenvattu = _name;
            _ob.id_nhom = _id_nhom;
            _ob.ghichu = _ghichu;
            _ob.image = "/uploads/images/icon-img.png";
            _ob.giaban = _gia_sp;
            _ob.gianhap = _giavon_sp;
            _ob.donvitinh_sp = txt_dvt_sp.Text.Trim();
            _ob.tinhtrang = DropDownList1.SelectedValue.ToString();
            _ob.vitriphongban = DropDownList2.SelectedValue.ToString();
            _ob.ncc = DropDownList3.SelectedValue.ToString();
            if (FileUpload2.HasFile)
            {
                string _ext = System.IO.Path.GetExtension(FileUpload2.FileName).ToLower();
                if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
                {
                    //byte - kb - mb  ContentLength trra về byte của file
                    long _filesize = (FileUpload2.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                    if (_filesize <= 1) //>1MB
                    {
                        string _filename = "/uploads/images/vat-tu/" + Guid.NewGuid() + _ext;
                        FileUpload2.SaveAs(Server.MapPath("~" + _filename));
                        _ob.image = _filename;
                    }
                    else
                    {
                        notifi = thongbao_class.metro_dialog_onload("Thông báo", "Kích thước ảnh vật tư quá lớn. Vui lòng chọn ảnh có dung lượng <1Mb.", "false", "false", "OK", "alert", "");
                        _checkloi = true;
                    }
                }
                else
                {
                    notifi = thongbao_class.metro_dialog_onload("Thông báo", "Hình ảnh vật tư không đúng định dạng.", "false", "false", "OK", "alert", "");
                    _checkloi = true;
                }
            }

            if (_checkloi == false)
            {
                _ob.id_chinhanh = Session["chinhanh"].ToString();
                db.danhsach_vattu_tables.InsertOnSubmit(_ob);
                db.SubmitChanges();

                //reset hết để bên trang quản lý nó hiển thị, chứ k là có lúc nó k hiện ra bài mới thêm, 
                Session["index_trangthai_qlvt"] = null;
                Session["index_sapxep_qlvt"] = null;
                Session["current_page_qlvt"] = null;
                Session["search_qlvt"] = null;
                Session["show_qlvt"] = null;
                Session["tungay_qlvt"] = null;
                Session["denngay_qlvt"] = null;

                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Thêm vật tư thành công.", "4000", "warning");
                Response.Redirect("/gianhang/admin/quan-ly-vat-tu/add.aspx");
            }
        }
    }
    #endregion


    protected void but_form_themnhomthuchi_Click(object sender, EventArgs e)
    {
        string _tennhom = txt_tennhom.Text.Trim();

        if (_tennhom == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập tên nhà cung cấp.", "4000", "warning"), true);
        else
        {
            nhacungcap_nhaphang_table _ob = new nhacungcap_nhaphang_table();
            _ob.ten = _tennhom;
            _ob.id_chinhanh = Session["chinhanh"].ToString();
            db.nhacungcap_nhaphang_tables.InsertOnSubmit(_ob);
            db.SubmitChanges();
            txt_tennhom.Text = "";
            load_nhom();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Thêm nhà cung cấp thành công.", "4000", "warning"), true);
        }
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        string _ten = txt_tenphongban.Text.Trim();

        if (_ten == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập tên phòng ban.", "4000", "warning"), true);
        else
        {
            phongban_table _ob = new phongban_table();
            _ob.ten = _ten;
            _ob.id_chinhanh = Session["chinhanh"].ToString();
            db.phongban_tables.InsertOnSubmit(_ob);
            db.SubmitChanges();
            txt_tenphongban.Text = "";
            load_nhom();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Thêm phòng ban thành công.", "4000", "warning"), true);
        }
    }
}