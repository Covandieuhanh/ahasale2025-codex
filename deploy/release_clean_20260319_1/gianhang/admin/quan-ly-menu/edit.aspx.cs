using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_temp : System.Web.UI.Page
{
    public string notifi, id;
    menu_class mn_cl = new menu_class();
    string_class str_cl = new string_class();
    dbDataContext db = new dbDataContext();

    public void main()
    {
        if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
        {
            id = Request.QueryString["id"].ToString().Trim();
            if (mn_cl.exist_id(id))
            {
                var q = db.web_menu_tables.Where(p => p.id.ToString() == id).First();
                if (q.image != "")
                {
                    Button2.Visible = true;
                    Label2.Text = "<img src='" + q.image + "' style='max-width: 150px' />";
                }
                else
                {
                    Button2.Visible = false;
                    Label2.Text = "";
                }
                if (!IsPostBack)
                {
                    txt_name.Text = q.name;
                    txt_url.Text = q.url_other;
                    txt_rank.Text = q.rank.ToString();
                    txt_description.Text = q.description;
                    show_menu_dropdownbox();
                    if (q.phanloai == "dsbv")
                        DropDownList1.SelectedIndex = 0;
                    else
                    {
                        if (q.phanloai == "dssp")
                            DropDownList1.SelectedIndex = 1;
                        else
                        {
                            if (q.phanloai == "dsdv")
                                DropDownList1.SelectedIndex = 2;
                        }
                    }
                }

            }
            else
            {
                Session["notifi"] = thongbao_class.metro_dialog("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                Response.Redirect("/gianhang/admin/quan-ly-menu/Default.aspx");
            }
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin/quan-ly-menu/Default.aspx");
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        #region Check_Login
        string _quyen = "q3_3";
        string _cookie_user = "", _cookie_pass = "";
        if (Request.Cookies["save_user_admin_aka_1"] != null) _cookie_user = Request.Cookies["save_user_admin_aka_1"].Value;
        if (Request.Cookies["save_pass_admin_aka_1"] != null) _cookie_pass = Request.Cookies["save_pass_admin_aka_1"].Value;
        if (Session["user"] == null) Session["user"] = ""; if (Session["notifi"] == null) Session["notifi"] = "";if (Session["user"].ToString() == "") Response.Redirect("/gianhang/admin/f5_ss_admin.aspx");
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
        main();
    }

    #region show_category_view
    public string result_listview, idmenu_cha, selected;
    public void show_menu_dropdownbox()
    {
        foreach (var t in mn_cl.return_list().Where(p => p.id_level == 1 && p.bin == false).OrderBy(p => p.rank))//duyệt hết menu cấp 1 k nằm trong thùng rác
        {
            get_data_menu(t.id.ToString());
        }
    }
    public void get_data_menu(string _id_category)//đưa 1 id vào
    {
        if (mn_cl.return_object(id).id_parent.ToString() == _id_category || idmenu_cha == _id_category)
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
        if (!Directory.Exists(Server.MapPath("~/uploads/images/menu/")))
            Directory.CreateDirectory(Server.MapPath("~/uploads/images/menu/"));
        bool _checkloi = false;
        idmenu_cha = Request.Form["listmenu"];
        string _name = str_cl.remove_blank(txt_name.Text.Trim());
        string _name_en = str_cl.replace_name_to_url(_name);
        string _url = txt_url.Text.Trim().ToLower();
        string _rank_text = txt_rank.Text.Trim();//lấy thứ tự từ ô text
        int _rank = 0;
        int.TryParse(_rank_text, out _rank);//nếu là số nguyên dương thì gán cho _rank
        if (_rank <= 0)//nếu k phải là số thì trả về 0 hoặc nhập vô số âm thì trả về 0 cho đẹp ^^, chơi số nguyên dương thôi
            _rank = 1;
        string _description = txt_description.Text.Trim();


        if (_name == "")
        {
            notifi = thongbao_class.metro_dialog_onload("Thông báo", "Vui lòng nhập tên Menu.", "false", "false", "OK", "alert", "");
            show_menu_dropdownbox();//gặp lỗi là gọi lại hàm này để k bị mất select ở dropdownbox
        }
        else
        {

            if ((mn_cl.exist_id(idmenu_cha) == false && idmenu_cha != "0") || idmenu_cha == id)//kiểm tra xem menu cha có hợp lệ hay k
            {
                notifi = thongbao_class.metro_dialog_onload("Thông báo", "Menu cha không hợp lệ.", "false", "false", "OK", "alert", "");
                show_menu_dropdownbox();//gặp lỗi là gọi lại hàm này để k bị mất select ở dropdownbox
            }
            else
            {
                int _id_level = 1;
                if (idmenu_cha != "0") //nếu có chọn  cha
                    _id_level = mn_cl.return_object(idmenu_cha).id_level.Value + 1;//biết đc cấp của cha và +1 là ra của menucon

                if (!mn_cl.check_sub(id, idmenu_cha))//nếu idparent k phải là con hoặc cháu của id cũ     
                {
                    if (mn_cl.exist_sub(id)) //nếu id đang chỉnh sửa có menu con
                        change_id_level_of_sub_category(id, _id_level); //thì thay đổi toàn bộ level của menu con theo cha nó
                }
                else
                {
                    string idmenu_cha_of_id_category_edit = mn_cl.return_object(id).id_parent;//lấy id cha của id cần thay đổi
                    int _id_level_ofidmenu_cha_new = mn_cl.return_object(idmenu_cha).id_level.Value;
                    int _id_level_of_id_category_edit = mn_cl.return_object(id).id_level.Value;
                    _id_level = _id_level_ofidmenu_cha_new;
                    change_id_level_of_sub_category1(id, idmenu_cha, _id_level_of_id_category_edit, idmenu_cha_of_id_category_edit, _id_level_ofidmenu_cha_new);
                }

                var q = db.web_menu_tables.Where(p => p.id.ToString() == id);//câu lệnh này nếu thay bằng mn_cl.return_object sẽ k lưu lại đc
                web_menu_table _ob = q.First();
                _ob.name = _name;
                _ob.name_en = _name_en;
                _ob.url_other = _url;
                _ob.rank = _rank;
                _ob.id_parent = idmenu_cha;
                _ob.id_level = _id_level;
                _ob.description = _description;
                _ob.phanloai = DropDownList1.SelectedValue.ToString();
                //_ob.ngaytao = DateTime.Now;


                if (FileUpload2.HasFile)
                {
                    string _ext = System.IO.Path.GetExtension(FileUpload2.FileName).ToLower();
                    if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
                    {
                        //byte - kb - mb  ContentLength trra về byte của file
                        long _filesize = (FileUpload2.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                        if (_filesize <= 1) //>1MB
                        {
                            string _filename = "/uploads/images/menu/" + Guid.NewGuid() + _ext;
                            FileUpload2.SaveAs(Server.MapPath("~" + _filename));
                            file_folder_class.del_file(_ob.image);//xóa ảnh cũ
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
                    db.SubmitChanges();
                    Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "4000", "warning");
                    Response.Redirect("/gianhang/admin/quan-ly-menu/edit.aspx?id=" + id);
                }
            }

        }


    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q3_3") == "2")
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        else
        {
            var q = db.web_menu_tables.Where(p => p.id.ToString() == id);//câu lệnh này nếu thay bằng mn_cl.return_object sẽ k lưu lại đc        
            if (q.Count() != 0)
            {
                web_menu_table _ob = q.First();
                file_folder_class.del_file(_ob.image);//xóa ảnh cũ
                _ob.image = "";
                db.SubmitChanges();
                //Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xóa ảnh thu nhỏ thành công.", "4000", "alert");
                //Response.Redirect("/gianhang/admin/quan-ly-menu/edit.aspx?id=" + id);
                main();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa ảnh thu nhỏ thành công.", "4000", "warning"), true);
            }
        }
    }
    #endregion

    #region function
    public void change_id_level_of_sub_category(string _id, int _id_level)//đưa 1 id vào
    {
        foreach (var t in mn_cl.return_list(_id)) //duyệt qua hết menu con, vì trước khi chạy hàm này đã chắc chắn id này có menu con
        {
            web_menu_table _mn_tb = db.web_menu_tables.Where(p => p.id.ToString() == t.id.ToString()).First();
            _mn_tb.id_level = _id_level + 1;
            db.SubmitChanges();
            if (mn_cl.exist_sub(t.id.ToString())) //nếu có menucon                       
                change_id_level_of_sub_category(t.id.ToString(), _id_level + 1); //thì gọi lại hàm, nếu có id con thì cứ gọi lại    
        }
    }
    public void change_id_level_of_sub_category1(string _id, string _id_parent_new, int _id_level_of_id_category_edit, string _id_parent_of_id_category_edit, int _id_level_of_id_parent_new)//đưa 1 id vào
    {
        foreach (var t in mn_cl.return_list(_id)) //duyệt qua hết menu con, vì trước khi chạy hàm này đã chắc chắn id này có menu con
        {
            web_menu_table _mn_tb = db.web_menu_tables.Where(p => p.id.ToString() == t.id.ToString()).First();
            if (t.id.ToString() == _id_parent_new)
            {
                _mn_tb.id_level = _id_level_of_id_category_edit;
                _mn_tb.id_parent = _id_parent_of_id_category_edit;
            }
            else
            {
                _mn_tb.id_level = _id_level_of_id_parent_new + 1;
            }
            db.SubmitChanges();

            if (mn_cl.exist_sub(t.id.ToString())) //nếu có menucon                       
                change_id_level_of_sub_category1(t.id.ToString(), _id_parent_new, _id_level_of_id_category_edit, _id_parent_of_id_category_edit, _id_level_of_id_parent_new + 1); //thì gọi lại hàm, nếu có id con thì cứ gọi lại                              
        }
    }
    #endregion
}