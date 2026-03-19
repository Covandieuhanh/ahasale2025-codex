using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class danh_sach_bai_viet : System.Web.UI.Page
{
    public string notifi = "", idmn = "", tenmn, mota;
    public string title_web = "", des = "";
    public string key = "";
    dbDataContext db = new dbDataContext();
    menu_webcon_class mn_cl = new menu_webcon_class();
    post_webcon_class po_cl = new post_webcon_class();
    public string tkchinhanh;
    taikhoan_class tk_cl = new taikhoan_class();

    List<web_post_table> list_all;
    #region phân trang
    public int stt = 1, current_page = 1, total_page = 1, show = 21;
    List<string> list_id_split;
    #endregion

    #region opengraph
    public string meta = "";
    public void opengraph(string _idmn)
    {
        var q = mn_cl.return_object(_idmn);
        title_web = q.name; des = q.description; this.Title = title_web;
        string _title_op = "<meta property=\"og:title\" content=\"" + title_web + "\" />";
        string _image = "<meta property=\"og:image\" content=\"" + q.image + "\" />";
        string _description = "<meta name=\"description\" content=\"" + des + "\" />";
        string _description_op = "<meta property=\"og:description\" content=\"" + des + "\" />";
        meta = _title_op + _image + _description + _description_op;
    }
    #endregion

    //#region lấy danh sách
    //public void show_listpost(string _idmn)
    //{
    //    list_all = po_cl.return_list_of_idmn(_idmn).Where(p => p.bin == false).ToList();
    //    if (mn_cl.exist_sub_category(_idmn)) //nếu có menucon
    //    {
    //        foreach (var t in mn_cl.return_list_sub(_idmn).Where(p => p.bin == false).ToList())//thì duyệt hết menu con
    //        {
    //            get_listpost(t.id.ToString()); //thì gọi lại hàm, nếu có id con thì cứ gọi lại                
    //        }
    //    }
    //}

    //public void get_listpost(string _idmn)//đưa 1 id vào
    //{
    //    var q = po_cl.return_list_of_idmn(_idmn).Where(p => p.bin == false).ToList();
    //    list_all = list_all.Union(q).ToList();
    //    if (mn_cl.exist_sub_category(_idmn)) //nếu có menucon
    //    {
    //        foreach (var t in mn_cl.return_list_sub(_idmn).Where(p => p.bin == false).ToList())//thì duyệt hết menu con
    //        {
    //            get_listpost(t.id.ToString()); //thì gọi lại hàm, nếu có id con thì cứ gọi lại
    //        }
    //    }
    //}
    //#endregion
    public string return_name_category(string _idmn)
    {
        var q = mn_cl.return_object(_idmn);
        return q.name.ToString();
    }
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
            }
            else
            {
                Response.Redirect("/");
                return;
            }
        }
        else
        {
            Response.Redirect("/");
            return;
        }

        if (string.IsNullOrWhiteSpace(Request.QueryString["idmn"]))
        {
            Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/webcon/Default.aspx?tkchinhanh=" + tkchinhanh);
        }
        else
        {
            idmn = Request.QueryString["idmn"].ToString();
            if (!mn_cl.exist_id(idmn))
            {
                Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                Response.Redirect("/gianhang/webcon/Default.aspx?tkchinhanh=" + tkchinhanh);
            }
            else
            {
                if (mn_cl.return_object(idmn).phanloai != "dsdv")//chỉ dành riêng để hiển thị bài viết, sản phẩm và dịch vụ đi trang khác để hiển thị giao diện khác
                {//nếu chung 1 giao diện thì bỏ câu if này
                    Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                    Response.Redirect("/gianhang/webcon/Default.aspx?tkchinhanh=" + tkchinhanh);
                }
                else
                {
                    tenmn = mn_cl.return_object(idmn).name; mota = mn_cl.return_object(idmn).description;
                    if (!IsPostBack)
                    {
                        Session["current_page_home_dichvu"] = "1";
                    }

                    //string _idmn_cap1 = get_idmn_cap1(idmn);

                    main();
                }
            }
        }
    }

    public void main()
    {
        //Intersect: lấy ra các phần tử mà cả 2 bên đều có (phần chung)
        #region lấy dữ liệu
        opengraph(idmn);
        //show_listpost(idmn);
        string id_chinhanh = AhaShineContext_cl.ResolveChiNhanhId();
        var list_all = (from bv in db.web_post_tables.Where(p => p.id_category == idmn && p.bin == false && p.hienthi == true && p.id_chinhanh == id_chinhanh).ToList()
                        join mn in db.web_menu_tables.Where(p => p.id_chinhanh == id_chinhanh).ToList() on bv.id_category equals mn.id.ToString()
                        select new
                        {
                            id = bv.id,
                            name = bv.name,
                            name_en = bv.name_en,
                            tenmn = mn.name,
                            noibat = bv.noibat,
                            ngaytao = bv.ngaytao,
                            giaban = bv.giaban_dichvu,
                            image = bv.image,
                            description = bv.description,
                            phanloai=bv.phanloai,
                        }).ToList();

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.name.ToLower().Contains(_key) || p.id.ToString() == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }
        //xử lý trạng thái

        //sắp xếp
        list_all = list_all.OrderByDescending(p=>p.ngaytao).ToList();
        #endregion

        //xử lý số lượng hiển thị
        total_page = number_of_page_class.return_total_page(list_all.Count(), show);

        //xử lý số trang        
        current_page = int.Parse(Session["current_page_home_dichvu"].ToString());
        if (current_page > total_page)
            current_page = total_page;
        if (current_page >= total_page)
            but_xemtiep.Visible = false;
        else
            but_xemtiep.Visible = true;
        if (current_page == 1)
            but_quaylai.Visible = false;
        else
            but_quaylai.Visible = true;

        //main
        stt = (show * current_page) - show + 1;
        var list_split = list_all.Skip(current_page * show - show).Take(show).ToList();
        list_id_split = new List<string>();
        foreach (var t in list_split)
        {
            list_id_split.Add("check_" + t.id);
        }
        int _s1 = stt + list_split.Count - 1;
        //if (list_all.Count() != 0)
        //    lb_show.Text = "Hiển thị " + stt + "-" + _s1 + " trong số " + list_all.Count().ToString("#,##0") + " mục";
        //else
        //    lb_show.Text = "Hiển thị 0-0 trong số 0";
        Repeater1.DataSource = list_split;
        Repeater1.DataBind();
    }

    #region autopostback
    protected void txt_search_TextChanged(object sender, EventArgs e)
    {
        Session["search_baiviet_home"] = txt_search.Text.Trim();
        Session["current_page_home_dichvu"] = "1";
        main();
    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_home_dichvu"] = int.Parse(Session["current_page_home_dichvu"].ToString()) - 1;
        if (int.Parse(Session["current_page_home_dichvu"].ToString()) < 1)
            Session["current_page_home_dichvu"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_home_dichvu"] = int.Parse(Session["current_page_home_dichvu"].ToString()) + 1;
        if (int.Parse(Session["current_page_home_dichvu"].ToString()) > total_page)
            Session["current_page_home_dichvu"] = total_page;
        main();
    }
    #endregion
}
