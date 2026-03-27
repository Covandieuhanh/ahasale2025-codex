using System;
using System.Collections.Generic;

public static class CoreAccessBootstrap_cl
{
    public static void EnsureCurrentAccountSeeded()
    {
        string accountKey = RootAccount_cl.GetCurrentAccountKey();
        if (accountKey == "")
            return;

        CoreDb_cl.Use(delegate (dbDataContext db)
        {
            taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, accountKey);
            EnsureAccountSeeded(db, account);
        });
    }

    public static void EnsureAccountSeeded(dbDataContext db, taikhoan_tb account)
    {
        if (db == null || account == null)
            return;

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);
        CompanyShopBootstrap_cl.EnsureSystemCatalogMirrored(db);

        string accountKey = (account.taikhoan ?? "").Trim();
        if (accountKey == "")
            return;

        IList<string> legacySpaces = SpaceAccess_cl.GetLegacyAccessSpaces(account);
        bool isPrimaryAssigned = false;

        for (int i = 0; i < legacySpaces.Count; i++)
        {
            string spaceCode = legacySpaces[i];
            if (spaceCode == "")
                continue;

            if (SpaceAccess_cl.GetAccessRow(db, accountKey, spaceCode) != null)
                continue;

            bool shouldBePrimary = !isPrimaryAssigned && spaceCode == ModuleSpace_cl.Home;
            if (!isPrimaryAssigned && shouldBePrimary)
                isPrimaryAssigned = true;

            SpaceAccess_cl.UpsertSpaceAccess(
                db,
                accountKey,
                spaceCode,
                SpaceAccess_cl.StatusActive,
                "legacy_seed",
                shouldBePrimary,
                "",
                null,
                "",
                null);
        }

        if (!isPrimaryAssigned && legacySpaces.Count > 0)
        {
            SpaceAccess_cl.SpaceAccessRow row = SpaceAccess_cl.GetAccessRow(db, accountKey, legacySpaces[0]);
            if (row != null && !row.IsPrimary)
            {
                SpaceAccess_cl.UpsertSpaceAccess(
                    db,
                    accountKey,
                    legacySpaces[0],
                    row.AccessStatus,
                    row.AccessSource,
                    true,
                    row.ApprovedBy,
                    row.ApprovedAt,
                    row.LockedReason,
                    row.ApprovedRequestId);
            }
        }
    }
}
