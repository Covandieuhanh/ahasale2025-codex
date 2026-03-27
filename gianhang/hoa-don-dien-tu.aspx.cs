using System;
using System.Web.UI;

public partial class gianhang_hoa_don_dien_tu : System.Web.UI.Page
{
    public string id, p, meta;
    public string id_guide, nguoixuat, logo_hoadon, user, user_parent, tencty, diachi, sdt, ngaytao, ten_kh, sdt_kh, diachi_kh, tongtien, ck, sauck, tien_dathanhtoan, tien_conlai, bangchu, km1_ghichu;

    protected void Page_Load(object sender, EventArgs e)
    {
        string rawId = (Request.QueryString["id"] ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(rawId))
        {
            RedirectInvalid();
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            GianHangSchema_cl.EnsureSchemaSafe(db);
            GianHangInvoicePrint_cl.InvoicePrintState state = GianHangInvoicePrint_cl.BuildState(db, rawId);
            if (state != null)
            {
                ApplyState(state);
                return;
            }
        }

        RedirectInvalid();
    }

    private void ApplyState(GianHangInvoicePrint_cl.InvoicePrintState state)
    {
        id = state.PublicId ?? string.Empty;
        id_guide = state.GuideId ?? string.Empty;
        tencty = state.SellerName ?? string.Empty;
        diachi = state.SellerAddress ?? string.Empty;
        sdt = state.SellerPhone ?? string.Empty;
        logo_hoadon = state.SellerLogo ?? string.Empty;
        nguoixuat = state.SellerIssuer ?? string.Empty;
        ngaytao = state.CreatedAtText ?? string.Empty;
        ten_kh = state.CustomerName ?? string.Empty;
        sdt_kh = state.CustomerPhone ?? string.Empty;
        diachi_kh = state.CustomerAddress ?? string.Empty;
        tongtien = state.TotalText ?? "0";
        ck = state.DiscountText ?? "0";
        sauck = state.AfterDiscountText ?? "0";
        tien_dathanhtoan = state.PaidText ?? "0";
        tien_conlai = state.RemainingText ?? "0";
        bangchu = state.AmountInWords ?? "0";
        km1_ghichu = state.NoteHtml ?? string.Empty;
        meta = state.MetaHtml ?? string.Empty;

        Repeater1.DataSource = state.Lines ?? new System.Collections.Generic.List<GianHangInvoicePrint_cl.InvoiceLine>();
        Repeater1.DataBind();
    }

    private void RedirectInvalid()
    {
        Response.Redirect("/gianhang/default.aspx", false);
        if (Context != null && Context.ApplicationInstance != null)
            Context.ApplicationInstance.CompleteRequest();
    }
}
