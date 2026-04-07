using System;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class bat_dong_san_tham_khao_gia : BatDongSanPageBase
{
    private const int MaxListings = 6;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack)
            return;

        ddlProvince.Items.Add(new ListItem("Chọn khu vực", ""));
        foreach (BatDongSanService_cl.FilterOption option in BatDongSanService_cl.GetFeaturedRegions())
            ddlProvince.Items.Add(new ListItem(option.Label, option.Value));

        ddlPropertyType.Items.Add(new ListItem("Chọn loại hình", ""));
        foreach (BatDongSanService_cl.FilterOption option in BatDongSanService_cl.GetFeaturedPropertyTypes())
            ddlPropertyType.Items.Add(new ListItem(option.Label, option.Value));

        BindStatsAndList();
    }

    protected void btnView_Click(object sender, EventArgs e)
    {
        BindStatsAndList();
    }

    private void BindStatsAndList()
    {
        var listings = BatDongSanService_cl.QueryListings(new BatDongSanService_cl.ListingQuery
        {
            Purpose = "sale",
            Province = ddlProvince.SelectedValue,
            PropertyType = ddlPropertyType.SelectedValue,
            Sort = "newest"
        });

        int count = listings.Count;
        lbCount.Text = count.ToString("#,##0");

        if (count > 0)
        {
            decimal avgPrice = 0;
            decimal avgPpsm = 0;
            decimal minPrice = decimal.MaxValue;
            decimal maxPrice = decimal.MinValue;

            foreach (var item in listings)
            {
                avgPrice += item.PriceValue;
                if (item.AreaValue > 0) avgPpsm += (item.PriceValue / item.AreaValue);
                if (item.PriceValue < minPrice) minPrice = item.PriceValue;
                if (item.PriceValue > maxPrice) maxPrice = item.PriceValue;
            }

            avgPrice = avgPrice / count;
            avgPpsm = avgPpsm / count;

            lbAvgPrice.Text = BatDongSanUiHelper_cl.FormatMoney(avgPrice);
            lbAvgPpsm.Text = avgPpsm > 0 ? avgPpsm.ToString("#,##0") + " đ/m²" : "-";
            lbMinMax.Text = BatDongSanUiHelper_cl.FormatMoney(minPrice) + " / " + BatDongSanUiHelper_cl.FormatMoney(maxPrice);
        }
        else
        {
            lbAvgPrice.Text = "-";
            lbAvgPpsm.Text = "-";
            lbMinMax.Text = "-";
        }

        rptListings.DataSource = listings.Count > 0 ? listings.GetRange(0, Math.Min(MaxListings, listings.Count)) : listings;
        rptListings.DataBind();

        string target = "/bat-dong-san/mua-ban.aspx";
        var query = new System.Collections.Generic.List<string>();
        if (!string.IsNullOrWhiteSpace(ddlProvince.SelectedValue))
            query.Add("province=" + Server.UrlEncode(ddlProvince.SelectedValue));
        if (!string.IsNullOrWhiteSpace(ddlPropertyType.SelectedValue))
            query.Add("propertyType=" + Server.UrlEncode(ddlPropertyType.SelectedValue));
        if (query.Count > 0)
            target += "?" + string.Join("&", query.ToArray());
        lnkViewAll.NavigateUrl = target;
    }

}
