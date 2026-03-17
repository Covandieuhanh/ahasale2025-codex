using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mp_home : System.Web.UI.MasterPage
{
    public string notifi, meta, nhungma_head, nhungma_body1, nhungma_body2, zalo, hotline;
    dbDataContext db = new dbDataContext();
    protected void Timer1_Tick(object sender, EventArgs e)
    {

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!host_compat_policy_class.is_allowed_request_host(HttpContext.Current.Request))
            Response.Redirect("/");

        AhaShineContext_cl.EnsureContext();
        Session["ten_tk_chinhanh"] = AhaShineContext_cl.UserParent;
        Session["id_chinhanh_webcon"] = AhaShineContext_cl.ResolveChiNhanhId();
        if (Session["notifi_home"] == null)
            Session["notifi_home"] = "";
        if (Session["user_home"] == null)
            Session["user_home"] = "";
        if (Session["user"] == null)
            Session["user"] = "";

        #region lưu url
        HttpContext context = HttpContext.Current;
        string _url = context.Request.RawUrl;
        string _path = context.Request.Path;
        //Response.Write(_url);
        if (AhaShineHomeRoutes_cl.ShouldSkipReturnUrl(_path))
        {
            //gặp mấy thằng này thì k lưu url back 
        }
        else
        {
            if (!IsPostBack)
            {
                app_cookie_policy_class.persist_cookie(
                    context,
                    app_cookie_policy_class.home_return_url_cookie,
                    _url,
                    365
                );

                //chỉnh trạng thái thông báo thành đã xem
                if (!string.IsNullOrWhiteSpace(Request.QueryString["idtb"]))
                {
                    string _idtb = Request.QueryString["idtb"].ToString().Trim();
                    string currentUser = (Session["user"] ?? "").ToString();
                    var q_tb = db.thongbao_tables.Where(p => p.id.ToString() == _idtb && p.nguoinhan == currentUser);
                    if (q_tb.Count() != 0)
                    {
                        thongbao_table _ob = q_tb.First();
                        _ob.daxem = true;//đã đọc
                        db.SubmitChanges();
                    }
                }
            }
        }

        #endregion

        #region meta
        var q = db.config_thongtin_tables;
        if (q.Count() != 0)
        {
            hotline = q.First().hotline;
            zalo = q.First().zalo;
            string _icon = "<link rel=\"shortcut icon\" type=\"image/x-icon\" href=\"" + q.First().icon + "\" />";
            string _appletouch = "<link rel=\"apple-touch-icon\" href=\"" + q.First().apple_touch_icon + "\" />";
            meta = _icon + _appletouch;
        }
        #endregion
        #region nhúng mã
        var q1 = db.config_nhungma_tables;
        if (q1.Count() != 0)
        {
            nhungma_head = q1.First().nhungma_head;
            nhungma_body1 = q1.First().nhungma_body1;
            nhungma_body2 = q1.First().nhungma_body2;
        }
        #endregion

        if (!IsPostBack)
        {
            #region lưu nội dung thông báo
            notifi = (Session["notifi_home"] ?? "").ToString();
            Session["notifi_home"] = "";
            #endregion
        }
    }
}
