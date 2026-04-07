using System.Collections.Generic;
using System.Web.UI.WebControls;

public static class BatDongSanFilterOptions_cl
{
    public static void PopulateFilterOptions(ListControl control, string defaultLabel, IEnumerable<BatDongSanService_cl.FilterOption> options)
    {
        if (control == null)
            return;

        control.Items.Clear();
        control.Items.Add(new ListItem(defaultLabel, ""));

        if (options == null)
            return;

        foreach (BatDongSanService_cl.FilterOption option in options)
            control.Items.Add(new ListItem(option.Label, option.Value));
    }

    public static void PopulateStaticOptions(ListControl control, string defaultLabel, params ListItem[] items)
    {
        if (control == null)
            return;

        control.Items.Clear();
        control.Items.Add(new ListItem(defaultLabel, ""));

        if (items == null)
            return;

        foreach (ListItem item in items)
        {
            if (item == null)
                continue;

            control.Items.Add(item);
        }
    }

    public static void PopulateBedroomOptions(ListControl control, int maxBedrooms)
    {
        if (control == null)
            return;

        control.Items.Clear();
        control.Items.Add(new ListItem("Không giới hạn", ""));

        for (int i = 1; i <= maxBedrooms; i++)
            control.Items.Add(new ListItem("Từ " + i + " PN", i.ToString()));
    }

    public static void PopulateSortOptions(ListControl control)
    {
        PopulateStaticOptions(
            control,
            "Tin mới nhất",
            new ListItem("Giá tăng dần", "price_asc"),
            new ListItem("Giá giảm dần", "price_desc"),
            new ListItem("Diện tích lớn nhất", "area_desc")
        );

        if (control != null && control.Items.Count > 0)
            control.Items[0].Value = "newest";
    }

    public static IEnumerable<BatDongSanService_cl.FilterOption> GetSalePriceOptions()
    {
        return new[]
        {
            new BatDongSanService_cl.FilterOption { Label = "Dưới 2 tỷ", Value = "0-2000000000" },
            new BatDongSanService_cl.FilterOption { Label = "2 - 4 tỷ", Value = "2000000000-4000000000" },
            new BatDongSanService_cl.FilterOption { Label = "4 - 7 tỷ", Value = "4000000000-7000000000" },
            new BatDongSanService_cl.FilterOption { Label = "7 - 15 tỷ", Value = "7000000000-15000000000" },
            new BatDongSanService_cl.FilterOption { Label = "Trên 15 tỷ", Value = "15000000000-0" }
        };
    }

    public static IEnumerable<BatDongSanService_cl.FilterOption> GetRentPriceOptions()
    {
        return new[]
        {
            new BatDongSanService_cl.FilterOption { Label = "Dưới 5 triệu", Value = "0-5000000" },
            new BatDongSanService_cl.FilterOption { Label = "5 - 10 triệu", Value = "5000000-10000000" },
            new BatDongSanService_cl.FilterOption { Label = "10 - 20 triệu", Value = "10000000-20000000" },
            new BatDongSanService_cl.FilterOption { Label = "20 - 40 triệu", Value = "20000000-40000000" },
            new BatDongSanService_cl.FilterOption { Label = "Trên 40 triệu", Value = "40000000-0" }
        };
    }

    public static IEnumerable<BatDongSanService_cl.FilterOption> GetAreaOptions()
    {
        return new[]
        {
            new BatDongSanService_cl.FilterOption { Label = "Dưới 50 m²", Value = "0-50" },
            new BatDongSanService_cl.FilterOption { Label = "50 - 80 m²", Value = "50-80" },
            new BatDongSanService_cl.FilterOption { Label = "80 - 120 m²", Value = "80-120" },
            new BatDongSanService_cl.FilterOption { Label = "120 - 200 m²", Value = "120-200" },
            new BatDongSanService_cl.FilterOption { Label = "Trên 200 m²", Value = "200-0" }
        };
    }
}
