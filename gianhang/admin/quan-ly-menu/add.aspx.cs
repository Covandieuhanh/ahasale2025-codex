using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_temp : System.Web.UI.Page
{
    public string notifi, url_back;
    public string user;
    menu_class mn_cl = new menu_class();
    string_class str_cl = new string_class();
    dbDataContext db = new dbDataContext();

    private bool HasPermission(string permissionKey)
    {
        string currentUser = (user ?? "").Trim();
        if (string.IsNullOrEmpty(currentUser) || string.IsNullOrEmpty(permissionKey))
            return false;

        return bcorn_class.check_quyen(currentUser, permissionKey) != "2";
    }
    private void RedirectToAdminHome()
    {
        Response.Redirect(GianHangAdminBridge_cl.BuildAdminHomeUrl(HttpContext.Current));
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q3_2");
        if (access == null)
            return;

        user = (access.User ?? "").Trim();

        if (!IsPostBack)
            show_menu_dropdownbox();

        url_back = GianHangAdminBridge_cl.ResolvePreferredAdminRedirectUrl(HttpContext.Current, "", "/gianhang/admin/quan-ly-menu/Default.aspx");
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
        if (idmenu_cha == _id_category)
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
        if (!HasPermission("q3_2"))
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            RedirectToAdminHome();
            return;
        }

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
            if (mn_cl.exist_id(idmenu_cha) == false && idmenu_cha != "0")//kiểm tra xem menu cha có hợp lệ hay k
            {
                notifi = thongbao_class.metro_dialog_onload("Thông báo", "Menu cha hợp lệ.", "false", "false", "OK", "alert", "");
                show_menu_dropdownbox();//gặp lỗi là gọi lại hàm này để k bị mất select ở dropdownbox
            }
            else
            {
                int _id_level = 1;
                if (idmenu_cha != "0") //nếu có chọn menu cha
                    _id_level = mn_cl.return_object(idmenu_cha).id_level.Value + 1;//biết đc cấp của menucha | +1 của menucon 
                web_menu_table _ob = new web_menu_table();
                _ob.name = _name;
                _ob.name_en = _name_en;
                _ob.url_other = _url;
                _ob.rank = _rank;
                _ob.id_parent = idmenu_cha;
                _ob.id_level = _id_level;
                _ob.description = _description;
                _ob.ngaytao = DateTime.Now;
                _ob.image = "";
                _ob.bin = false;
                _ob.phanloai = DropDownList1.SelectedValue.ToString();
                _ob.id_chinhanh = Session["chinhanh"].ToString();

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
                    db.web_menu_tables.InsertOnSubmit(_ob);
                    db.SubmitChanges();

                    //reset hết để bên trang quản lý nó hiển thị, chứ k là có lúc nó k hiện ra bài mới thêm, 
                    Session["index_trangthai_menu"] = null;
                    Session["index_sapxep_menu"] = null;
                    Session["current_page_menu"] = null;
                    Session["search_menu"] = null;
                    Session["show_menu"] = null;
                    Session["tungay_menu"] = null;
                    Session["denngay_menu"] = null;

                    Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Thêm menu thành công.", "4000", "warning");
                    Response.Redirect("/gianhang/admin/quan-ly-menu/add.aspx");
                }
            }

        }


    }
    protected void Button2_Click(object sender, EventArgs e)
    {

    }
    #endregion
}
