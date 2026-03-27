using System;
using System.Collections.Generic;
using System.Linq;

public static class GianHangMenu_cl
{
    public static List<web_menu_table> LoadAll(dbDataContext db, string chiNhanhId)
    {
        string normalizedChiNhanhId = (chiNhanhId ?? string.Empty).Trim();
        if (db == null || normalizedChiNhanhId == string.Empty)
            return new List<web_menu_table>();

        return db.web_menu_tables
            .Where(p => p.id_chinhanh == normalizedChiNhanhId)
            .OrderBy(p => p.rank)
            .ThenBy(p => p.id)
            .ToList();
    }

    public static List<web_menu_table> LoadChildren(dbDataContext db, string chiNhanhId, string parentId)
    {
        string normalizedChiNhanhId = (chiNhanhId ?? string.Empty).Trim();
        string normalizedParentId = (parentId ?? string.Empty).Trim();
        if (db == null || normalizedChiNhanhId == string.Empty)
            return new List<web_menu_table>();

        return db.web_menu_tables
            .Where(p => p.id_chinhanh == normalizedChiNhanhId && p.id_parent == normalizedParentId)
            .OrderBy(p => p.rank)
            .ThenBy(p => p.id)
            .ToList();
    }

    public static web_menu_table FindById(dbDataContext db, string chiNhanhId, string rawMenuId)
    {
        string normalizedChiNhanhId = (chiNhanhId ?? string.Empty).Trim();
        string menuId = (rawMenuId ?? string.Empty).Trim();
        if (db == null || normalizedChiNhanhId == string.Empty || menuId == string.Empty)
            return null;

        return db.web_menu_tables.FirstOrDefault(p =>
            p.id.ToString() == menuId
            && p.id_chinhanh == normalizedChiNhanhId);
    }

    public static bool Exists(dbDataContext db, string chiNhanhId, string rawMenuId)
    {
        return FindById(db, chiNhanhId, rawMenuId) != null;
    }

    public static bool HasChildren(dbDataContext db, string chiNhanhId, string parentId, bool onlyVisible)
    {
        string normalizedChiNhanhId = (chiNhanhId ?? string.Empty).Trim();
        string normalizedParentId = (parentId ?? string.Empty).Trim();
        if (db == null || normalizedChiNhanhId == string.Empty)
            return false;

        IQueryable<web_menu_table> query = db.web_menu_tables.Where(p =>
            p.id_chinhanh == normalizedChiNhanhId
            && p.id_parent == normalizedParentId);
        if (onlyVisible)
            query = query.Where(p => p.bin == false);

        return query.Any();
    }

    public static web_menu_table ResolveByType(dbDataContext db, string chiNhanhId, string rawMenuId, string expectedType)
    {
        web_menu_table menu = FindById(db, chiNhanhId, rawMenuId);
        if (menu == null)
            return null;

        string menuType = (menu.phanloai ?? string.Empty).Trim().ToLowerInvariant();
        string normalizedExpectedType = (expectedType ?? string.Empty).Trim().ToLowerInvariant();
        if (menuType != normalizedExpectedType)
            return null;

        return menu;
    }
}
