using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_temp : System.Web.UI.Page
{
    public string notifi, url_back, user, user_parent, id, _id_nhom;
    dbDataContext db = new dbDataContext();
    nhaphang_class nh_cl = new nhaphang_class();
    vattu_class vt_cl = new vattu_class();
    nhapvattu_class nvt_cl = new nhapvattu_class();
    phongban_class pb_cl = new phongban_class();

    private bool EnsureActionAccess(string requiredPermission)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, requiredPermission);
        if (access == null)
            return false;

        user = (access.User ?? "").Trim();
        user_parent = access.OwnerAccountKey;
        return true;
    }

    public void load_nhom()
    {
        danhsach_vattu_table _ob = vt_cl.return_object(id);
        var list_nhom = (from ob1 in db.nhomvattu_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                         select new { id = ob1.id, tennhom = ob1.tennhom, }
                                        );
        ddl_nhom.DataSource = list_nhom.OrderBy(p => p.tennhom);
        ddl_nhom.DataTextField = "tennhom";
        ddl_nhom.DataValueField = "id";
        ddl_nhom.DataBind();
        ddl_nhom.Items.Insert(0, new ListItem("Chọn", "0"));
        ddl_nhom.SelectedIndex = nvt_cl.return_index_nhomvattu(_id_nhom);


        var list_nhacungcap = (from ob1 in db.nhacungcap_nhaphang_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                               select new { id = ob1.id, ten = ob1.ten, }
                     );
        DropDownList3.DataSource = list_nhacungcap;
        DropDownList3.DataTextField = "ten";
        DropDownList3.DataValueField = "id";
        DropDownList3.DataBind();
        DropDownList3.Items.Insert(0, new ListItem("Chọn", ""));
        DropDownList3.SelectedIndex = nh_cl.return_index_ncc(_ob.ncc);

        var list_phongban = (from ob1 in db.phongban_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                             select new { id = ob1.id, ten = ob1.ten, }
        );
        DropDownList2.DataSource = list_phongban;
        DropDownList2.DataTextField = "ten";
        DropDownList2.DataValueField = "id";
        DropDownList2.DataBind();
        DropDownList2.Items.Insert(0, new ListItem("Chọn", ""));
        DropDownList2.SelectedIndex = pb_cl.return_index(_ob.vitriphongban);

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q13_9");
        if (access == null)
            return;
        user = (access.User ?? "").Trim();
        user_parent = access.OwnerAccountKey;


        if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
        {
            id = Request.QueryString["id"].ToString().Trim();
            if (vt_cl.exist_id(id))
            {
                danhsach_vattu_table _ob = vt_cl.return_object(id);
                _id_nhom = _ob.id_nhom;
                if (!IsPostBack)
                {
                    load_nhom();

                    txt_name.Text = _ob.tenvattu;
                    txt_dvt_sp.Text = _ob.donvitinh_sp;
                    txt_ghichu.Text = _ob.ghichu;
                    txt_giasanpham.Text = _ob.giaban.Value.ToString("#,##0");
                    txt_giavonsanpham.Text = _ob.gianhap.Value.ToString("#,##0");
                    
                    if (_ob.tinhtrang == "Thuê")
                        DropDownList1.SelectedIndex = 0;
                    else
                        DropDownList1.SelectedIndex = 1;
                }

            }
            else
            {
                Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                Response.Redirect("/gianhang/admin/Default.aspx");
            }

            url_back = GianHangAdminBridge_cl.ResolvePreferredAdminRedirectUrl(HttpContext.Current, "", "/gianhang/admin/quan-ly-vat-tu/Default.aspx");
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin/Default.aspx");
        }

    }

    protected void button1_Click(object sender, EventArgs e)
    {
        if (!EnsureActionAccess("q13_9"))
            return;

        if (!Directory.Exists(Server.MapPath("~/uploads/images/vat-tu/")))
            Directory.CreateDirectory(Server.MapPath("~/uploads/images/vat-tu/"));
        bool _checkloi = false;
        string _name = txt_name.Text;
        string _ghichu = txt_ghichu.Text.Trim();
        string _id_nhom = ddl_nhom.SelectedValue.ToString();

        string _gia_sp_text = txt_giasanpham.Text.Trim().Replace(".", "").Replace(",", "");
        Int64 _gia_sp = 0; Int64.TryParse(_gia_sp_text, out _gia_sp); if (_gia_sp <= 0) _gia_sp = 0;

        string _giavon_sp_text = txt_giavonsanpham.Text.Trim().Replace(".", "").Replace(",", "");
        int _giavon_sp = 0; int.TryParse(_giavon_sp_text, out _giavon_sp); if (_giavon_sp <= 0) _giavon_sp = 0;

        if (_name == "")
        {
            notifi = thongbao_class.metro_dialog_onload("Thông báo", "Vui lòng nhập tên vật tư.", "false", "false", "OK", "alert", "");
        }
        else
        {

            danhsach_vattu_table _ob = db.danhsach_vattu_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
            _ob.tenvattu = _name;
            _ob.id_nhom = _id_nhom;
            _ob.ghichu = _ghichu;
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
                        file_folder_class.del_file(_ob.image);//xóa ảnh cũ
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
                db.SubmitChanges();

                //reset hết để bên trang quản lý nó hiển thị, chứ k là có lúc nó k hiện ra bài mới thêm, 
                Session["index_trangthai_qlvt"] = null;
                Session["index_sapxep_qlvt"] = null;
                Session["current_page_qlvt"] = null;
                Session["search_qlvt"] = null;
                Session["show_qlvt"] = null;
                Session["tungay_qlvt"] = null;
                Session["denngay_qlvt"] = null;

                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật vật tư thành công.", "4000", "warning");
                Response.Redirect("/gianhang/admin/quan-ly-vat-tu/Default.aspx");
            }
        }
    }


    protected void Button2_Click(object sender, EventArgs e)
    {
        if (!EnsureActionAccess("q13_9"))
            return;

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
