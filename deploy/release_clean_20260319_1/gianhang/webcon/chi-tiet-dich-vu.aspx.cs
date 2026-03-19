using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class danh_sach_bai_viet : System.Web.UI.Page
{
    public string notifi = "", idbv = "", noidung = "", name_mn, name_mn_en, phanloai_menu, url_menu;
    public string title_web = "", des = "", image = "", gia = "";

    menu_webcon_class mn_cl = new menu_webcon_class();
    post_webcon_class po_cl = new post_webcon_class();
    dbDataContext db = new dbDataContext();
    public string tkchinhanh;
    taikhoan_class tk_cl = new taikhoan_class();
    #region opengraph
    public string meta = "";
    public void opengraph(string _idbv)
    {
        var q = po_cl.return_object(_idbv);
        title_web = q.name; des = q.description; this.Title = title_web; image = q.image;
        string _title_op = "<meta property=\"og:title\" content=\"" + title_web + "\" />";
        string _image = "<meta property=\"og:image\" content=\"" + image + "\" />";
        string _description = "<meta name=\"description\" content=\"" + des + "\" />";
        string _description_op = "<meta property=\"og:description\" content=\"" + des + "\" />";
        meta = _title_op + _image + _description + _description_op;
    }
    #endregion  
    public string _idmenu;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(Request.QueryString["tkchinhanh"]))
        {
            tkchinhanh = Request.QueryString["tkchinhanh"].ToString().Trim();
            var q_cn = db.chinhanh_tables.Where(p => p.taikhoan_quantri == tkchinhanh);
            if (tk_cl.exist_user(tkchinhanh) && q_cn.Count() != 0)
            {
                Session["ten_tk_chinhanh"] = tkchinhanh;
                Session["id_chinhanh_webcon"] = q_cn.First().id;
                if (!IsPostBack)
                {

                }
            }
            else
            {
                Response.Redirect("/");
            }
        }
        else
        {

            Response.Redirect("/");
        }
        if (string.IsNullOrWhiteSpace(Request.QueryString["idbv"]))
        {
            Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
            Response.Redirect("/");
        }
        else
        {
            idbv = Request.QueryString["idbv"].ToString().Trim();
            if (!po_cl.exist_id(idbv) || po_cl.check_id_in_bin(idbv))
            {
                Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                Response.Redirect("/");
            }
            else
            {
                if (po_cl.return_object(idbv).hienthi != true)
                {
                    Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                    Response.Redirect("/");
                }
                else
                {
                    _idmenu = po_cl.return_object(idbv).id_category;
                    if (_idmenu != "0")
                    {
                        name_mn = mn_cl.return_object(_idmenu).name;
                        name_mn_en = mn_cl.return_object(_idmenu).name_en;
                        phanloai_menu = mn_cl.return_object(_idmenu).phanloai;
                        url_menu = "/gianhang/webcon/danh-sach-bai-viet.aspx?tkchinhanh=" + tkchinhanh + "&idmn=" + _idmenu;
                        if (phanloai_menu == "dsdv")
                        {
                            url_menu = "/gianhang/webcon/danh-sach-dich-vu.aspx?tkchinhanh=" + tkchinhanh + "&idmn=" + _idmenu;
                        }
                        else if (phanloai_menu == "dssp")
                        {
                            url_menu = "/gianhang/webcon/danh-sach-san-pham.aspx?tkchinhanh=" + tkchinhanh + "&idmn=" + _idmenu;
                        }
                    }

                    if (po_cl.return_object(idbv).phanloai != "ctdv")
                    {
                        Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                        Response.Redirect("/");
                    }

                    opengraph(idbv);

                    noidung = po_cl.return_object(idbv).content_post;
                    gia = (po_cl.return_object(idbv).giaban_dichvu ?? 0).ToString("#,##0");


                    var q2 = db.web_post_tables.Where(p => p.id_category == _idmenu && p.bin == false && p.id.ToString() != idbv && p.id_chinhanh == AhaShineContext_cl.ResolveChiNhanhId()).OrderByDescending(p => p.ngaytao);
                    Repeater2.DataSource = q2.Take(9);
                    Repeater2.DataBind();
                    if (q2.Count() == 0)
                    {
                        Panel1.Visible = false;
                    }
                }
            }
        }
    }



    protected void but_dathang_Click(object sender, EventArgs e)
    {
        Response.Redirect("/gianhang/webcon/datlich.aspx?tkchinhanh=" + tkchinhanh + "&id=" + idbv);
    }
}
