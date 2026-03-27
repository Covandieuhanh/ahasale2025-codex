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
    menu_class mn_cl = new menu_class();
    string_class str_cl = new string_class();
    dbDataContext db = new dbDataContext();

    private void EnsureLegacyAdminSession()
    {
        if (Session == null)
            return;

        string legacyUser = (Session["user"] ?? "").ToString().Trim();
        if (legacyUser != "")
            return;

        string homeAccount;
        string deniedMessage;
        GianHangAdminBridge_cl.EnsureLegacyAdminSessionFromCurrentHome(db, out homeAccount, out deniedMessage);
    }
    protected void FileBrowser1_Load(object sender, EventArgs e)
    {
        FileBrowser1 = new CKFinder.FileBrowser();
        FileBrowser1.BasePath = "/ckfinder/";
        FileBrowser1.SetupCKEditor(txt_content);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        EnsureLegacyAdminSession();

        #region Check_Login
        string _quyen = "q4_2";
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
        user_parent = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();

        if (!IsPostBack)
        {
            xuly_phanloai();
            show_menu_dropdownbox();
            load_nganh();
            txt_thoiluong_dichvu.Text = datlich_class.thoiluong_macdinh_dichvu_phut.ToString("#,##0");
        }
        else
        {
            xuly_phanloai();
        }

        if (Request.Cookies["save_url_admin_aka_1"] != null)
            url_back = Request.Cookies["save_url_admin_aka_1"].Value;
        else
            url_back = "/gianhang/admin/quan-ly-bai-viet/Default.aspx";
    }
    public void xuly_phanloai()
    {
        if (Session["index_loc_phanloai_baiviet"] != null)
        {
            if (Session["index_loc_phanloai_baiviet"].ToString() == "2")//sản phẩm
                DropDownList1.SelectedIndex = 1;
            else
            {
                if (Session["index_loc_phanloai_baiviet"].ToString() == "3")//dịch vụ
                    DropDownList1.SelectedIndex = 2;
            }
        }


        if (DropDownList1.SelectedValue == "ctbv")
        {
            Panel_dichvu.Visible = false;
            Panel_sanpham.Visible = false;
        }
        else
        {
            if (DropDownList1.SelectedValue == "ctsp")
            {
                Panel_dichvu.Visible = false;
                Panel_sanpham.Visible = true;
            }
            else
            {
                Panel_dichvu.Visible = true;
                Panel_sanpham.Visible = false;
            }
        }
    }
    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        xuly_phanloai();
    }

    #region show_category_view
    public string result_listview, idmenu, selected;
    public void show_menu_dropdownbox()
    {
        foreach (var t in mn_cl.return_list().Where(p => p.id_level == 1 && p.bin == false).OrderBy(p => p.rank))//duyệt hết menu cấp 1 k nằm trong thùng rác
        {
            get_data_menu(t.id.ToString());
        }
    }
    public void get_data_menu(string _id_category)//đưa 1 id vào
    {
        if (idmenu == _id_category)
            selected = "selected";
        else
            selected = "";
        //gán vào select dropbox        
        result_listview = result_listview + "<option " + selected + " class='pl-" + 4 * (mn_cl.return_object(_id_category).id_level - 1) + "' value='" + _id_category + "'>" + mn_cl.return_object(_id_category).name + "</option>";
        if (mn_cl.exist_sub(_id_category)) //nếu có menucon
        {
            foreach (var t in mn_cl.return_list(_id_category).Where(p => p.bin == false).OrderBy(p => p.rank))//thì duyệt hết menu con, chỉ những đứa k nằm trong thùng rác
            {
                get_data_menu(t.id.ToString()); //thì gọi lại hàm, nếu có id con thì cứ gọi lại
            }
        }
    }
    #endregion

    #region button click    
    protected void button1_Click(object sender, EventArgs e)
    {
        if (!Directory.Exists(Server.MapPath("~/uploads/images/bai-viet/")))
            Directory.CreateDirectory(Server.MapPath("~/uploads/images/bai-viet/"));
        bool _checkloi = false;
        idmenu = Request.Form["listmenu"];
        string _name = str_cl.remove_blank(txt_name.Text.Trim());
        string _name_en = str_cl.replace_name_to_url(_name);
        string _description = txt_description.Text.Trim();

        #region dịch vụ
        string _gia_dv_text = txt_giadichvu.Text.Trim().Replace(".", "").Replace(",", "");
        Int64 _gia_dv = 0; Int64.TryParse(_gia_dv_text, out _gia_dv); if (_gia_dv <= 0) _gia_dv = 0;

        string _chotsale_dv = txt_chotsale_dichvu.Text.Trim().Replace(".", "");
        int _r5 = 0; int.TryParse(_chotsale_dv, out _r5); if (_r5 <= 0 || _r5 > 100) _r5 = 0;

        string _lam_dv = txt_lam_dichvu.Text.Trim().Replace(".", "");
        int _r4 = 0; int.TryParse(_lam_dv, out _r4); if (_r4 <= 0 || _r4 > 100) _r4 = 0;

        string _thoiluong_dv_text = txt_thoiluong_dichvu.Text.Trim().Replace(".", "").Replace(",", "");
        int _thoiluong_dv = 0; int.TryParse(_thoiluong_dv_text, out _thoiluong_dv); _thoiluong_dv = datlich_class.chuanhoa_thoiluong_dichvu_phut(_thoiluong_dv);
        #endregion

        #region sản phẩm
        string _gia_sp_text = txt_giasanpham.Text.Trim().Replace(".", "").Replace(",", "");
        Int64 _gia_sp = 0; Int64.TryParse(_gia_sp_text, out _gia_sp); if (_gia_sp <= 0) _gia_sp = 0;

        string _giavon_sp_text = txt_giavonsanpham.Text.Trim().Replace(".", "").Replace(",", "");
        int _giavon_sp = 0; int.TryParse(_giavon_sp_text, out _giavon_sp); if (_giavon_sp <= 0) _giavon_sp = 0;

        string _chotsale_sp = txt_chotsale_sanpham.Text.Trim().Replace(".", "");
        int _r2 = 0; int.TryParse(_chotsale_sp, out _r2); if (_r2 <= 0 || _r2 > 100) _r2 = 0;

        //string _soluong_ton_sp = txt_soluong_ton_sanpham.Text.Trim().Replace(".", "");
        //int _r3 = 0; int.TryParse(_soluong_ton_sp, out _r3); if (_r3 <= 0) _r3 = 0;

        #endregion

        if (_name == "")
        {
            notifi = thongbao_class.metro_dialog_onload("Thông báo", "Vui lòng nhập tên bài viết.", "false", "false", "OK", "alert", "");
            show_menu_dropdownbox();//gặp lỗi là gọi lại hàm này để k bị mất select ở dropdownbox
        }
        else
        {

            web_post_table _ob = new web_post_table();
            _ob.name = _name;
            _ob.name_en = _name_en;
            _ob.id_category = idmenu;
            _ob.description = _description;
            _ob.ngaytao = DateTime.Now;
            _ob.bin = false;
            _ob.content_post = txt_content.Text;
            _ob.noibat = Request.Form["check_noibat"] == "on" ? true : false;
            _ob.hienthi = Request.Form["check_hienthi"] == "on" ? true : false;
            _ob.image = "/uploads/images/icon-img.png";
            _ob.phanloai = DropDownList1.SelectedValue.ToString();
            _ob.id_chinhanh = Session["chinhanh"].ToString();

            if (DropDownList1.SelectedValue == "ctbv")
            {
                _ob.giaban_sanpham = 0; _ob.phantram_chotsale_sanpham = 0; _ob.soluong_ton_sanpham = 0;
                _ob.giaban_dichvu = 0; _ob.phantram_lamdichvu = 0; _ob.phantram_chotsale_dichvu = 0;
                _ob.thoiluong_dichvu_phut = null;
                _ob.id_nganh = "";
            }
            else
            {

                if (DropDownList1.SelectedValue == "ctsp")
                {
                    if (DropDownList6.SelectedValue.ToString() != "")
                    {
                        _ob.id_nganh = DropDownList6.SelectedValue.ToString();
                        _ob.giaban_sanpham = _gia_sp; _ob.phantram_chotsale_sanpham = _r2; //lưu sản phẩm
                        _ob.giaban_dichvu = 0; _ob.phantram_lamdichvu = 0; _ob.phantram_chotsale_dichvu = 0;
                        _ob.thoiluong_dichvu_phut = null;
                        _ob.giavon_sanpham = _giavon_sp;
                        _ob.donvitinh_sp = txt_dvt_sp.Text.Trim();
                        //if (bcorn_class.check_quyen(user, "q4_7") == "")//nếu có quyền thay đổi số lượng của sản phẩm thì
                        //    _ob.soluong_ton_sanpham = _r3;
                        //else
                        _ob.soluong_ton_sanpham = 0;
                    }
                    else
                    {
                        notifi = thongbao_class.metro_dialog_onload("Thông báo", "Bạn chưa chọn ngành.", "false", "false", "OK", "alert", "");
                        _checkloi = true;
                    }
                }
                else
                {
                    if (DropDownList5.SelectedValue.ToString() != "")
                    {
                        _ob.id_nganh = DropDownList5.SelectedValue.ToString();
                        _ob.giaban_sanpham = 0; _ob.phantram_chotsale_sanpham = 0; _ob.soluong_ton_sanpham = 0;
                        _ob.giaban_dichvu = _gia_dv; _ob.phantram_chotsale_dichvu = _r5; _ob.phantram_lamdichvu = _r4;//lưu dịch vụ
                        _ob.thoiluong_dichvu_phut = _thoiluong_dv;
                    }
                    else
                    {
                        notifi = thongbao_class.metro_dialog_onload("Thông báo", "Bạn chưa chọn ngành.", "false", "false", "OK", "alert", "");
                        _checkloi = true;
                    }
                }
            }

            if (FileUpload2.HasFile)
            {
                string _ext = System.IO.Path.GetExtension(FileUpload2.FileName).ToLower();
                if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
                {
                    //byte - kb - mb  ContentLength trra về byte của file
                    long _filesize = (FileUpload2.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                    if (_filesize <= 1) //>1MB
                    {
                        string _filename = "/uploads/images/bai-viet/" + Guid.NewGuid() + _ext;
                        FileUpload2.SaveAs(Server.MapPath("~" + _filename));
                        _ob.image = _filename;
                    }
                    else
                    {
                        notifi = thongbao_class.metro_dialog_onload("Thông báo", "Kích thước ảnh thu nhỏ quá lớn. Vui lòng chọn ảnh có dung lượng <1Mb.", "false", "false", "OK", "alert", "");
                        _checkloi = true;
                    }
                }
                else
                {
                    notifi = thongbao_class.metro_dialog_onload("Thông báo", "Ảnh thu nhỏ không đúng định dạng.", "false", "false", "OK", "alert", "");
                    _checkloi = true;
                }
            }


            if (_checkloi == false)
            {
                db.web_post_tables.InsertOnSubmit(_ob);
                db.SubmitChanges();

                //reset hết để bên trang quản lý nó hiển thị, chứ k là có lúc nó k hiện ra bài mới thêm, 
                Session["index_trangthai_baiviet"] = null;
                Session["index_sapxep_baiviet"] = null;
                Session["current_page_baiviet"] = null;
                Session["search_baiviet"] = null;
                Session["show_baiviet"] = null;
                Session["tungay_baiviet"] = null;
                Session["denngay_baiviet"] = null;

                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Thêm bài viết thành công.", "4000", "warning");
                Response.Redirect("/gianhang/admin/quan-ly-bai-viet/Default.aspx");
            }


        }


    }

    #endregion

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

        DropDownList6.DataSource = list_nganh;
        DropDownList6.DataTextField = "ten";
        DropDownList6.DataValueField = "id";
        DropDownList6.DataBind();
        DropDownList6.Items.Insert(0, new ListItem("Chọn", ""));

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
