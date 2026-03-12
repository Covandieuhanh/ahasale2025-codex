using System;
using System.Collections.Generic;

public static class AddressFormat_cl
{
    public static string BuildFullAddress(string detail, string ward, string district, string province)
    {
        List<string> parts = new List<string>();

        string detailClean = (detail ?? "").Trim();
        string wardClean = (ward ?? "").Trim();
        string districtClean = (district ?? "").Trim();
        string provinceClean = (province ?? "").Trim();

        if (!string.IsNullOrEmpty(detailClean)) parts.Add(detailClean);
        if (!string.IsNullOrEmpty(wardClean)) parts.Add(wardClean);
        if (!string.IsNullOrEmpty(districtClean)) parts.Add(districtClean);
        if (!string.IsNullOrEmpty(provinceClean)) parts.Add(provinceClean);

        return string.Join(", ", parts);
    }
}
