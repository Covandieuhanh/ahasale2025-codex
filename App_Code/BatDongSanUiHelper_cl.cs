using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

public static class BatDongSanUiHelper_cl
{
    public static string BuildPurposeLabel(object purposeObj)
    {
        return string.Equals((purposeObj ?? "").ToString(), "rent", StringComparison.OrdinalIgnoreCase)
            ? "Cho thuê"
            : "Mua bán";
    }

    public static string BuildPurposeBadgeCss(object purposeObj)
    {
        return string.Equals((purposeObj ?? "").ToString(), "rent", StringComparison.OrdinalIgnoreCase)
            ? "bg-azure-lt text-azure"
            : "bg-green-lt text-green";
    }

    public static string BuildListingUrl(object itemObj)
    {
        return BatDongSanService_cl.BuildListingUrl(itemObj as BatDongSanService_cl.ListingItem);
    }

    public static string BuildProjectUrl(object itemObj)
    {
        return BatDongSanService_cl.BuildProjectUrl(itemObj as BatDongSanService_cl.ProjectItem);
    }

    public static void AddQuery(List<string> query, string key, string value)
    {
        if (query == null || string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            return;

        query.Add(key + "=" + HttpUtility.UrlEncode(value.Trim()));
    }

    public static void SetSelectedValue(ListControl control, string value)
    {
        if (control == null)
            return;

        string normalized = (value ?? "").Trim();
        ListItem item = control.Items.FindByValue(normalized);
        if (item != null)
            control.SelectedValue = normalized;
    }

    public static void ParseRange(string raw, out decimal? min, out decimal? max)
    {
        min = null;
        max = null;

        string value = (raw ?? "").Trim();
        if (string.IsNullOrWhiteSpace(value) || !value.Contains("-"))
            return;

        string[] parts = value.Split('-');
        if (parts.Length != 2)
            return;

        decimal temp;
        if (decimal.TryParse(parts[0], out temp))
            min = temp;
        if (decimal.TryParse(parts[1], out temp))
            max = temp;
    }

    public static string FormatMoney(decimal value)
    {
        if (value <= 0) return "Liên hệ";
        if (value >= 1000000000m) return (value / 1000000000m).ToString("0.##") + " tỷ";
        if (value >= 1000000m) return (value / 1000000m).ToString("0.##") + " triệu";
        return value.ToString("#,##0") + " đ";
    }
}
