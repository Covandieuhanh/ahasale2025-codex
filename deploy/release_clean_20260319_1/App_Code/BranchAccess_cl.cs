using System;
using System.Collections.Generic;
using System.Linq;

public static class BranchAccess_cl
{
    public static List<string> ResolveBranchAccounts(dbDataContext db, string ownerShop)
    {
        string owner = (ownerShop ?? "").Trim().ToLowerInvariant();
        if (db == null || string.IsNullOrWhiteSpace(owner))
            return new List<string>();

        List<string> accounts = db.taikhoan_table_2023s
            .Where(p => p.user_parent == owner || p.taikhoan == owner)
            .Select(p => p.taikhoan)
            .ToList();

        if (!accounts.Any(a => string.Equals((a ?? "").Trim(), owner, StringComparison.OrdinalIgnoreCase)))
            accounts.Add(owner);

        return accounts
            .Where(a => !string.IsNullOrWhiteSpace(a))
            .Select(a => a.Trim().ToLowerInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static List<chinhanh_table> ResolveBranchesForOwner(dbDataContext db, string ownerShop)
    {
        List<string> accounts = ResolveBranchAccounts(db, ownerShop);
        if (db == null || accounts.Count == 0)
            return new List<chinhanh_table>();

        return db.chinhanh_tables
            .Where(p => accounts.Contains(p.taikhoan_quantri))
            .OrderBy(p => p.ten)
            .ToList();
    }

    public static bool IsBranchOwnedBy(dbDataContext db, string ownerShop, string branchId)
    {
        string id = (branchId ?? "").Trim();
        if (db == null || string.IsNullOrWhiteSpace(id))
            return false;

        List<string> accounts = ResolveBranchAccounts(db, ownerShop);
        if (accounts.Count == 0)
            return false;

        return db.chinhanh_tables.Any(p => p.id.ToString() == id && accounts.Contains(p.taikhoan_quantri));
    }
}
