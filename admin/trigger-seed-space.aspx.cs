using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

public partial class admin_trigger_seed_space : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AdminRolePolicy_cl.RequireSuperAdmin();

        if (!IsPostBack)
        {
            RunSeed();
        }
    }

    protected void btnRun_Click(object sender, EventArgs e)
    {
        RunSeed();
    }

    private void RunSeed()
    {
        int homeSeeded = 0;
        int gianhangSeeded = 0;
        int shopSeeded = 0;
        int skipped = 0;

        using (var db = new dbDataContext())
        {
            CoreSchemaMigration_cl.EnsureSchemaSafe(db);

            List<taikhoan_tb> accounts = db.taikhoan_tbs.ToList();
            DateTime now = AhaTime_cl.Now;

            foreach (var acc in accounts)
            {
                if (acc == null || string.IsNullOrWhiteSpace(acc.taikhoan))
                {
                    skipped++;
                    continue;
                }

                if (acc.block == true)
                {
                    skipped++;
                    continue;
                }

                string permission = (acc.permission ?? "").ToLowerInvariant();
                bool hasHome = permission.Contains("portal_home");
                bool hasShop = permission.Contains("portal_shop");

                string key = acc.taikhoan.Trim();

                if (hasHome)
                {
                    SpaceAccess_cl.UpsertSpaceAccess(
                        db,
                        key,
                        ModuleSpace_cl.Home,
                        SpaceAccess_cl.StatusActive,
                        "manual_seed",
                        true,
                        "system",
                        now,
                        "",
                        null);
                    homeSeeded++;

                    // Ensure feed visibility for home accounts
                    SpaceAccess_cl.UpsertSpaceAccess(
                        db,
                        key,
                        ModuleSpace_cl.GianHang,
                        SpaceAccess_cl.StatusActive,
                        "manual_seed",
                        false,
                        "system",
                        now,
                        "",
                        null);
                    gianhangSeeded++;
                }

                if (hasShop)
                {
                    SpaceAccess_cl.UpsertSpaceAccess(
                        db,
                        key,
                        ModuleSpace_cl.Shop,
                        SpaceAccess_cl.StatusActive,
                        "manual_seed",
                        false,
                        "system",
                        now,
                        "",
                        null);
                    shopSeeded++;
                }
            }
        }

        lblStatus.Text = "<div class='ok'>Hoàn tất đồng bộ.</div>"
            + "<div class='row'>Home: " + homeSeeded + "</div>"
            + "<div class='row'>Gianhang (feed): " + gianhangSeeded + "</div>"
            + "<div class='row'>Shop: " + shopSeeded + "</div>"
            + "<div class='row muted'>Skipped (blocked/invalid): " + skipped + "</div>";
    }
}
