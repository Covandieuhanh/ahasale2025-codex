using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class bat_dong_san_mua_ban : BatDongSanPageBase
{
    private const int PageSize = 18;
    protected List<BatDongSanService_cl.ListingItem> Listings = new List<BatDongSanService_cl.ListingItem>();
    protected BatDongSanService_cl.ListingQuery CurrentQuery = new BatDongSanService_cl.ListingQuery();
    protected int TotalCount = 0;
    protected int CurrentPage = 1;
    protected int TotalPages = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindFilters();
            ApplyQueryToControls();
        }

        BindList();
    }

    private void BindFilters()
    {
        string selectedProvince = (Request.QueryString["province"] ?? "").Trim();
        string selectedDistrict = (Request.QueryString["district"] ?? "").Trim();
        BatDongSanListingFilterHelper_cl.PopulateSaleFilters(selectedProvince, selectedDistrict, ddlProvince, ddlDistrict, ddlPropertyType, ddlBedrooms, ddlLegal, ddlFurnishing, ddlProject, ddlPrice, ddlArea, ddlSort);
    }

    private void ApplyQueryToControls()
    {
        BatDongSanListingFilterHelper_cl.ApplyQueryToControls(Request, ddlProvince, ddlDistrict, ddlPropertyType, ddlBedrooms, ddlLegal, ddlFurnishing, ddlProject, ddlPrice, ddlArea, ddlSort, txtKeyword);
    }

    private void BindList()
    {
        BatDongSanService_cl.ListingQuery query = BatDongSanListingFilterHelper_cl.BuildListingQuery("sale", ddlProvince, ddlDistrict, ddlPropertyType, ddlBedrooms, ddlLegal, ddlFurnishing, ddlProject, ddlPrice, ddlArea, ddlSort, txtKeyword);
        BatDongSanListingFilterHelper_cl.ApplyRequestOverrides(Request, query);
        CurrentQuery = BatDongSanSearch_cl.NormalizeQuery(query);
        List<BatDongSanService_cl.ListingItem> allMatches = BatDongSanSearch_cl.QueryUnifiedListings(CurrentQuery);

        TotalCount = allMatches.Count;
        TotalPages = number_of_page_class.return_total_page(TotalCount, PageSize);
        if (TotalPages < 1) TotalPages = 1;

        CurrentPage = HomePager_cl.ResolvePage(Request);
        if (CurrentPage < 1) CurrentPage = 1;
        if (CurrentPage > TotalPages) CurrentPage = TotalPages;

        Listings = allMatches
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        litPager.Text = HomePager_cl.RenderPager(Request, CurrentPage, TotalPages);

        int from = TotalCount == 0 ? 0 : ((CurrentPage - 1) * PageSize + 1);
        int to = TotalCount == 0 ? 0 : (from + Listings.Count - 1);
        lbPageSummary.Text = from.ToString("#,##0") + "-" + to.ToString("#,##0") + " / " + TotalCount.ToString("#,##0");

        rptListings.DataSource = Listings;
        rptListings.DataBind();
        phList.Visible = Listings.Count > 0;
        phEmpty.Visible = Listings.Count == 0;
    }

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        string url = BatDongSanListingFilterHelper_cl.BuildFilterUrl("/bat-dong-san/mua-ban.aspx", ddlProvince, ddlDistrict, ddlPropertyType, ddlBedrooms, ddlLegal, ddlFurnishing, ddlProject, ddlPrice, ddlArea, ddlSort, txtKeyword);

        Response.Redirect(url, false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected string FormatPricePerSquareMeter(object itemObj)
    {
        return BatDongSanService_cl.FormatPricePerSquareMeter(itemObj as BatDongSanService_cl.ListingItem);
    }

    protected string RenderProjectFact(object itemObj)
    {
        BatDongSanService_cl.ListingItem item = itemObj as BatDongSanService_cl.ListingItem;
        if (item == null || string.IsNullOrWhiteSpace(item.ProjectName))
            return "";
        return "<span class='bds-list-fact'>" + Server.HtmlEncode(item.ProjectName) + "</span>";
    }

    protected string RenderIntentHighlights(object itemObj)
    {
        BatDongSanService_cl.ListingItem item = itemObj as BatDongSanService_cl.ListingItem;
        if (item == null)
            return "";

        var parts = new List<string>();
        string propertyType = (CurrentQuery == null ? "" : CurrentQuery.PropertyType ?? "").Trim().ToLowerInvariant();

        if (item.AreaValue > 0 && (propertyType == "land" || propertyType == "business-premises" || propertyType == "warehouse"))
            parts.Add("<span class='bds-intent-pill'>Diện tích " + Server.HtmlEncode(item.AreaValue.ToString("0.##")) + " m2</span>");

        if (item.BedroomCount > 0 && (propertyType == "house" || propertyType == "apartment" || propertyType == "boarding-house" || propertyType == ""))
            parts.Add("<span class='bds-intent-pill'>" + Server.HtmlEncode(item.BedroomCount.ToString()) + " PN</span>");

        if (!string.IsNullOrWhiteSpace(item.District))
            parts.Add("<span class='bds-intent-pill'>" + Server.HtmlEncode(item.District) + "</span>");

        if (!string.IsNullOrWhiteSpace(item.Province))
            parts.Add("<span class='bds-intent-pill bds-intent-pill-soft'>" + Server.HtmlEncode(item.Province) + "</span>");

        if (parts.Count == 0 && !string.IsNullOrWhiteSpace(item.PropertyTypeLabel))
            parts.Add("<span class='bds-intent-pill'>" + Server.HtmlEncode(item.PropertyTypeLabel) + "</span>");

        return string.Join("", parts.ToArray());
    }

    protected string RenderSourceChip(object itemObj)
    {
        BatDongSanService_cl.ListingItem item = itemObj as BatDongSanService_cl.ListingItem;
        if (item == null)
            return "";

        if (item.IsLinked)
        {
            string label = BatDongSanService_cl.ResolveLinkedSourceLabel(item.LinkedSource);
            return "<span class='bds-card-chip bds-card-chip-linked'>" + Server.HtmlEncode(label) + "</span>";
        }

        return "<span class='bds-card-chip bds-card-chip-native'>AhaLand</span>";
    }

    protected string RenderSupportLine(object itemObj)
    {
        BatDongSanService_cl.ListingItem item = itemObj as BatDongSanService_cl.ListingItem;
        if (item == null)
            return "";

        var parts = new List<string>();
        if (item.AreaValue > 0)
            parts.Add(item.AreaValue.ToString("0.##") + " m2");
        if (item.BathroomCount > 0)
            parts.Add(item.BathroomCount.ToString() + " WC");
        if (!string.IsNullOrWhiteSpace(item.ProjectName))
            parts.Add(item.ProjectName.Trim());

        return Server.HtmlEncode(string.Join(" • ", parts.ToArray()));
    }

    protected string RenderListingSummary(object itemObj)
    {
        BatDongSanService_cl.ListingItem item = itemObj as BatDongSanService_cl.ListingItem;
        if (item == null)
            return "";

        string summary = (item.Description ?? "").Trim();
        if (summary == "")
            summary = item.IsLinked ? "Xem tin gốc để đọc đầy đủ mô tả và thông tin liên hệ." : "Xem chi tiết để đọc thêm thông tin về bất động sản này.";

        return Server.HtmlEncode(summary);
    }

    protected string RenderActiveFilterChips()
    {
        var chips = new List<string>();
        AddStaticChip(chips, "Mua bán");
        AddRemovableChip(chips, BatDongSanListingFilterHelper_cl.GetSelectedTextOrRaw(ddlProvince, Request.QueryString["province"]), "province");
        AddRemovableChip(chips, BatDongSanListingFilterHelper_cl.GetSelectedTextOrRaw(ddlDistrict, Request.QueryString["district"]), "district");
        AddRemovableChip(chips, BatDongSanListingFilterHelper_cl.GetSelectedTextOrRaw(ddlPropertyType, Request.QueryString["propertyType"]), "propertyType");
        AddRemovableChip(chips, BatDongSanListingFilterHelper_cl.GetSelectedTextOrRaw(ddlBedrooms, Request.QueryString["bedrooms"]), "bedrooms");
        AddRemovableChip(chips, BatDongSanListingFilterHelper_cl.GetSelectedTextOrRaw(ddlLegal, Request.QueryString["legal"]), "legal");
        AddRemovableChip(chips, BatDongSanListingFilterHelper_cl.GetSelectedTextOrRaw(ddlFurnishing, Request.QueryString["furnishing"]), "furnishing");
        AddRemovableChip(chips, BatDongSanListingFilterHelper_cl.GetSelectedTextOrRaw(ddlProject, Request.QueryString["project"]), "project");
        AddRemovableChip(chips, ddlPrice == null ? "" : (ddlPrice.SelectedItem == null || string.IsNullOrWhiteSpace(ddlPrice.SelectedValue) ? BatDongSanListingFilterHelper_cl.FormatPriceFilterLabel(Request.QueryString["price"]) : ddlPrice.SelectedItem.Text), "price");
        AddRemovableChip(chips, ddlArea == null ? "" : (ddlArea.SelectedItem == null || string.IsNullOrWhiteSpace(ddlArea.SelectedValue) ? BatDongSanListingFilterHelper_cl.FormatAreaFilterLabel(Request.QueryString["area"]) : ddlArea.SelectedItem.Text), "area");
        AddRemovableChip(chips, (txtKeyword == null ? "" : txtKeyword.Text), "keyword");

        if (chips.Count <= 1)
            return "<span class='bds-filter-chip bds-filter-chip-muted'>Chưa áp bộ lọc nâng cao</span>";

        return string.Join("", chips.ToArray());
    }

    private void AddStaticChip(List<string> chips, string text)
    {
        string normalizedText = (text ?? "").Trim();
        if (normalizedText == "")
            return;
        chips.Add("<span class='bds-filter-chip'>" + Server.HtmlEncode(normalizedText) + "</span>");
    }

    private void AddRemovableChip(List<string> chips, string text, string key)
    {
        string normalizedText = (text ?? "").Trim();
        string normalizedKey = (key ?? "").Trim();
        if (normalizedText == "" || normalizedKey == "" || string.IsNullOrWhiteSpace(Request.QueryString[normalizedKey]))
            return;

        string url = BatDongSanListingFilterHelper_cl.BuildFilterRemovalUrl("/bat-dong-san/mua-ban.aspx", Request, normalizedKey);
        chips.Add("<a class='bds-filter-chip bds-filter-chip-removable' href='" + ResolveUrl(url) + "'>" + Server.HtmlEncode(normalizedText) + "<span class='bds-filter-chip-close' aria-hidden='true'>&times;</span></a>");
    }

}
