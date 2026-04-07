using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

public static class BatDongSanListingFilterHelper_cl
{
    public static void PopulateSaleFilters(
        string selectedProvince,
        string selectedDistrict,
        ListControl ddlProvince,
        ListControl ddlDistrict,
        ListControl ddlPropertyType,
        ListControl ddlBedrooms,
        ListControl ddlLegal,
        ListControl ddlFurnishing,
        ListControl ddlProject,
        ListControl ddlPrice,
        ListControl ddlArea,
        ListControl ddlSort)
    {
        BatDongSanFilterOptions_cl.PopulateFilterOptions(ddlProvince, "Tất cả khu vực", BatDongSanSearch_cl.GetUnifiedProvinceOptions("sale"));
        BatDongSanFilterOptions_cl.PopulateFilterOptions(ddlDistrict, "Mọi quận/huyện", BatDongSanSearch_cl.GetUnifiedDistrictOptions("sale", selectedProvince));
        BatDongSanFilterOptions_cl.PopulateFilterOptions(ddlPropertyType, "Tất cả loại hình", BatDongSanSearch_cl.GetUnifiedPropertyTypeOptions("sale", selectedProvince, selectedDistrict));
        BatDongSanFilterOptions_cl.PopulateBedroomOptions(ddlBedrooms, 4);
        BatDongSanFilterOptions_cl.PopulateFilterOptions(ddlLegal, "Mọi pháp lý", BatDongSanService_cl.GetLegalStatusOptions());
        BatDongSanFilterOptions_cl.PopulateFilterOptions(ddlFurnishing, "Mọi nội thất", BatDongSanService_cl.GetFurnishingOptions());
        BatDongSanFilterOptions_cl.PopulateFilterOptions(ddlProject, "Mọi dự án", BatDongSanService_cl.GetProjectOptions());
        BatDongSanFilterOptions_cl.PopulateFilterOptions(ddlPrice, "Mọi mức giá", BatDongSanFilterOptions_cl.GetSalePriceOptions());
        BatDongSanFilterOptions_cl.PopulateFilterOptions(ddlArea, "Mọi diện tích", BatDongSanFilterOptions_cl.GetAreaOptions());
        BatDongSanFilterOptions_cl.PopulateSortOptions(ddlSort);
    }

    public static void PopulateRentFilters(
        string selectedProvince,
        string selectedDistrict,
        ListControl ddlProvince,
        ListControl ddlDistrict,
        ListControl ddlPropertyType,
        ListControl ddlBedrooms,
        ListControl ddlFurnishing,
        ListControl ddlProject,
        ListControl ddlPrice,
        ListControl ddlArea,
        ListControl ddlSort)
    {
        BatDongSanFilterOptions_cl.PopulateFilterOptions(ddlProvince, "Tất cả khu vực", BatDongSanSearch_cl.GetUnifiedProvinceOptions("rent"));
        BatDongSanFilterOptions_cl.PopulateFilterOptions(ddlDistrict, "Mọi quận/huyện", BatDongSanSearch_cl.GetUnifiedDistrictOptions("rent", selectedProvince));
        BatDongSanFilterOptions_cl.PopulateFilterOptions(ddlPropertyType, "Tất cả loại hình", BatDongSanSearch_cl.GetUnifiedPropertyTypeOptions("rent", selectedProvince, selectedDistrict));
        BatDongSanFilterOptions_cl.PopulateBedroomOptions(ddlBedrooms, 3);
        BatDongSanFilterOptions_cl.PopulateFilterOptions(ddlFurnishing, "Mọi nội thất", BatDongSanService_cl.GetFurnishingOptions());
        BatDongSanFilterOptions_cl.PopulateFilterOptions(ddlProject, "Mọi dự án", BatDongSanService_cl.GetProjectOptions());
        BatDongSanFilterOptions_cl.PopulateFilterOptions(ddlPrice, "Mọi mức giá", BatDongSanFilterOptions_cl.GetRentPriceOptions());
        BatDongSanFilterOptions_cl.PopulateFilterOptions(ddlArea, "Mọi diện tích", BatDongSanFilterOptions_cl.GetAreaOptions());
        BatDongSanFilterOptions_cl.PopulateSortOptions(ddlSort);
    }

    public static void ApplyQueryToControls(
        HttpRequest request,
        ListControl ddlProvince,
        ListControl ddlDistrict,
        ListControl ddlPropertyType,
        ListControl ddlBedrooms,
        ListControl ddlLegal,
        ListControl ddlFurnishing,
        ListControl ddlProject,
        ListControl ddlPrice,
        ListControl ddlArea,
        ListControl ddlSort,
        TextBox txtKeyword)
    {
        BatDongSanUiHelper_cl.SetSelectedValue(ddlProvince, request.QueryString["province"]);
        BatDongSanUiHelper_cl.SetSelectedValue(ddlDistrict, request.QueryString["district"]);
        BatDongSanUiHelper_cl.SetSelectedValue(ddlPropertyType, request.QueryString["propertyType"]);
        BatDongSanUiHelper_cl.SetSelectedValue(ddlBedrooms, request.QueryString["bedrooms"]);
        BatDongSanUiHelper_cl.SetSelectedValue(ddlLegal, request.QueryString["legal"]);
        BatDongSanUiHelper_cl.SetSelectedValue(ddlFurnishing, request.QueryString["furnishing"]);
        BatDongSanUiHelper_cl.SetSelectedValue(ddlProject, request.QueryString["project"]);
        BatDongSanUiHelper_cl.SetSelectedValue(ddlPrice, request.QueryString["price"]);
        BatDongSanUiHelper_cl.SetSelectedValue(ddlArea, request.QueryString["area"]);
        BatDongSanUiHelper_cl.SetSelectedValue(ddlSort, request.QueryString["sort"]);

        if (txtKeyword != null)
            txtKeyword.Text = (request.QueryString["keyword"] ?? "").Trim();
    }

    public static BatDongSanService_cl.ListingQuery BuildListingQuery(
        string purpose,
        ListControl ddlProvince,
        ListControl ddlDistrict,
        ListControl ddlPropertyType,
        ListControl ddlBedrooms,
        ListControl ddlLegal,
        ListControl ddlFurnishing,
        ListControl ddlProject,
        ListControl ddlPrice,
        ListControl ddlArea,
        ListControl ddlSort,
        TextBox txtKeyword)
    {
        decimal? priceMin, priceMax, areaMin, areaMax;
        BatDongSanUiHelper_cl.ParseRange(ddlPrice == null ? "" : ddlPrice.SelectedValue, out priceMin, out priceMax);
        BatDongSanUiHelper_cl.ParseRange(ddlArea == null ? "" : ddlArea.SelectedValue, out areaMin, out areaMax);

        return new BatDongSanService_cl.ListingQuery
        {
            Purpose = purpose,
            Province = ddlProvince == null ? "" : ddlProvince.SelectedValue,
            District = ddlDistrict == null ? "" : ddlDistrict.SelectedValue,
            PropertyType = ddlPropertyType == null ? "" : ddlPropertyType.SelectedValue,
            Bedrooms = ddlBedrooms == null ? "" : ddlBedrooms.SelectedValue,
            LegalStatus = ddlLegal == null ? "" : ddlLegal.SelectedValue,
            FurnishingStatus = ddlFurnishing == null ? "" : ddlFurnishing.SelectedValue,
            Project = ddlProject == null ? "" : ddlProject.SelectedValue,
            PriceMin = priceMin,
            PriceMax = priceMax,
            AreaMin = areaMin,
            AreaMax = areaMax,
            Sort = ddlSort == null ? "" : ddlSort.SelectedValue,
            Keyword = txtKeyword == null ? "" : txtKeyword.Text.Trim()
        };
    }

    public static void ApplyRequestOverrides(HttpRequest request, BatDongSanService_cl.ListingQuery query)
    {
        if (request == null || query == null)
            return;

        if (string.IsNullOrWhiteSpace(query.Province))
            query.Province = (request.QueryString["province"] ?? "").Trim();
        if (string.IsNullOrWhiteSpace(query.District))
            query.District = (request.QueryString["district"] ?? "").Trim();
        if (string.IsNullOrWhiteSpace(query.PropertyType))
            query.PropertyType = (request.QueryString["propertyType"] ?? "").Trim();
        if (string.IsNullOrWhiteSpace(query.Bedrooms))
            query.Bedrooms = (request.QueryString["bedrooms"] ?? "").Trim();
        if (string.IsNullOrWhiteSpace(query.LegalStatus))
            query.LegalStatus = (request.QueryString["legal"] ?? "").Trim();
        if (string.IsNullOrWhiteSpace(query.FurnishingStatus))
            query.FurnishingStatus = (request.QueryString["furnishing"] ?? "").Trim();
        if (string.IsNullOrWhiteSpace(query.Project))
            query.Project = (request.QueryString["project"] ?? "").Trim();
        if (string.IsNullOrWhiteSpace(query.Sort))
            query.Sort = (request.QueryString["sort"] ?? "").Trim();
        if (string.IsNullOrWhiteSpace(query.Keyword))
            query.Keyword = (request.QueryString["keyword"] ?? "").Trim();

        if (!query.PriceMin.HasValue && !query.PriceMax.HasValue)
        {
            decimal? min;
            decimal? max;
            BatDongSanUiHelper_cl.ParseRange((request.QueryString["price"] ?? "").Trim(), out min, out max);
            query.PriceMin = min;
            query.PriceMax = max;
        }
        if (!query.AreaMin.HasValue && !query.AreaMax.HasValue)
        {
            decimal? min;
            decimal? max;
            BatDongSanUiHelper_cl.ParseRange((request.QueryString["area"] ?? "").Trim(), out min, out max);
            query.AreaMin = min;
            query.AreaMax = max;
        }
    }

    public static string BuildFilterUrl(
        string baseUrl,
        ListControl ddlProvince,
        ListControl ddlDistrict,
        ListControl ddlPropertyType,
        ListControl ddlBedrooms,
        ListControl ddlLegal,
        ListControl ddlFurnishing,
        ListControl ddlProject,
        ListControl ddlPrice,
        ListControl ddlArea,
        ListControl ddlSort,
        TextBox txtKeyword)
    {
        var query = new List<string>();
        BatDongSanUiHelper_cl.AddQuery(query, "province", ddlProvince == null ? "" : ddlProvince.SelectedValue);
        BatDongSanUiHelper_cl.AddQuery(query, "district", ddlDistrict == null ? "" : ddlDistrict.SelectedValue);
        BatDongSanUiHelper_cl.AddQuery(query, "propertyType", ddlPropertyType == null ? "" : ddlPropertyType.SelectedValue);
        BatDongSanUiHelper_cl.AddQuery(query, "bedrooms", ddlBedrooms == null ? "" : ddlBedrooms.SelectedValue);
        BatDongSanUiHelper_cl.AddQuery(query, "legal", ddlLegal == null ? "" : ddlLegal.SelectedValue);
        BatDongSanUiHelper_cl.AddQuery(query, "furnishing", ddlFurnishing == null ? "" : ddlFurnishing.SelectedValue);
        BatDongSanUiHelper_cl.AddQuery(query, "project", ddlProject == null ? "" : ddlProject.SelectedValue);
        BatDongSanUiHelper_cl.AddQuery(query, "price", ddlPrice == null ? "" : ddlPrice.SelectedValue);
        BatDongSanUiHelper_cl.AddQuery(query, "area", ddlArea == null ? "" : ddlArea.SelectedValue);
        BatDongSanUiHelper_cl.AddQuery(query, "sort", ddlSort == null ? "" : ddlSort.SelectedValue);
        BatDongSanUiHelper_cl.AddQuery(query, "keyword", txtKeyword == null ? "" : txtKeyword.Text.Trim());

        if (query.Count <= 0)
            return baseUrl;

        return baseUrl + "?" + string.Join("&", query.ToArray());
    }

    public static string BuildFilterUrl(string baseUrl, BatDongSanService_cl.ListingQuery query)
    {
        var parts = new List<string>();
        BatDongSanUiHelper_cl.AddQuery(parts, "province", query == null ? "" : query.Province);
        BatDongSanUiHelper_cl.AddQuery(parts, "district", query == null ? "" : query.District);
        BatDongSanUiHelper_cl.AddQuery(parts, "propertyType", query == null ? "" : query.PropertyType);
        BatDongSanUiHelper_cl.AddQuery(parts, "bedrooms", query == null ? "" : query.Bedrooms);
        BatDongSanUiHelper_cl.AddQuery(parts, "legal", query == null ? "" : query.LegalStatus);
        BatDongSanUiHelper_cl.AddQuery(parts, "furnishing", query == null ? "" : query.FurnishingStatus);
        BatDongSanUiHelper_cl.AddQuery(parts, "project", query == null ? "" : query.Project);
        if (query != null && (query.PriceMin.HasValue || query.PriceMax.HasValue))
            BatDongSanUiHelper_cl.AddQuery(parts, "price", (query.PriceMin ?? 0).ToString("0.##") + "-" + (query.PriceMax ?? 0).ToString("0.##"));
        if (query != null && (query.AreaMin.HasValue || query.AreaMax.HasValue))
            BatDongSanUiHelper_cl.AddQuery(parts, "area", (query.AreaMin ?? 0).ToString("0.##") + "-" + (query.AreaMax ?? 0).ToString("0.##"));
        BatDongSanUiHelper_cl.AddQuery(parts, "sort", query == null ? "" : query.Sort);
        BatDongSanUiHelper_cl.AddQuery(parts, "keyword", query == null ? "" : query.Keyword);

        if (parts.Count <= 0)
            return baseUrl;

        return baseUrl + "?" + string.Join("&", parts.ToArray());
    }

    public static string BuildFilterRemovalUrl(string baseUrl, HttpRequest request, params string[] keysToRemove)
    {
        var parts = new List<string>();
        var remove = new HashSet<string>((keysToRemove ?? new string[0]), System.StringComparer.OrdinalIgnoreCase);
        string[] orderedKeys = new[] { "province", "district", "propertyType", "bedrooms", "legal", "furnishing", "project", "price", "area", "sort", "keyword" };

        foreach (string key in orderedKeys)
        {
            if (remove.Contains(key))
                continue;
            AddQueryFromRequest(parts, request, key);
        }

        if (parts.Count <= 0)
            return baseUrl;

        return baseUrl + "?" + string.Join("&", parts.ToArray());
    }

    public static string GetSelectedTextOrRaw(ListControl control, string rawValue)
    {
        string normalized = (rawValue ?? "").Trim();
        if (control != null && control.SelectedItem != null && !string.IsNullOrWhiteSpace(control.SelectedValue))
            return control.SelectedItem.Text;
        return normalized;
    }

    public static string FormatPriceFilterLabel(string rawValue)
    {
        decimal? min;
        decimal? max;
        BatDongSanUiHelper_cl.ParseRange(rawValue, out min, out max);
        if (!min.HasValue && !max.HasValue)
            return (rawValue ?? "").Trim();
        if (min.HasValue && max.HasValue && max.Value > 0)
            return BatDongSanUiHelper_cl.FormatMoney(min.Value) + " - " + BatDongSanUiHelper_cl.FormatMoney(max.Value);
        if (min.HasValue && min.Value > 0)
            return "Từ " + BatDongSanUiHelper_cl.FormatMoney(min.Value);
        if (max.HasValue && max.Value > 0)
            return "Đến " + BatDongSanUiHelper_cl.FormatMoney(max.Value);
        return (rawValue ?? "").Trim();
    }

    public static string FormatAreaFilterLabel(string rawValue)
    {
        decimal? min;
        decimal? max;
        BatDongSanUiHelper_cl.ParseRange(rawValue, out min, out max);
        if (!min.HasValue && !max.HasValue)
            return (rawValue ?? "").Trim();
        if (min.HasValue && max.HasValue && max.Value > 0)
            return min.Value.ToString("0.##") + " - " + max.Value.ToString("0.##") + " m2";
        if (min.HasValue && min.Value > 0)
            return "Từ " + min.Value.ToString("0.##") + " m2";
        if (max.HasValue && max.Value > 0)
            return "Đến " + max.Value.ToString("0.##") + " m2";
        return (rawValue ?? "").Trim();
    }

    private static void AddQueryFromRequest(List<string> query, HttpRequest request, string key)
    {
        if (request == null || query == null || string.IsNullOrWhiteSpace(key))
            return;
        BatDongSanUiHelper_cl.AddQuery(query, key, (request.QueryString[key] ?? "").Trim());
    }
}
