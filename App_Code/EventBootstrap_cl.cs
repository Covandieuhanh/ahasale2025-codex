using System;
using System.Linq;

public static class EventBootstrap_cl
{
    private static bool _schemaReady;
    private static readonly object _schemaLock = new object();

    public static void EnsureReady()
    {
        using (dbDataContext db = new dbDataContext())
        {
            EnsureSchemaSafe(db);
        }
    }

    public static void EnsureSchemaSafe(dbDataContext db)
    {
        if (db == null)
            return;
        if (_schemaReady)
            return;

        lock (_schemaLock)
        {
            if (_schemaReady)
                return;

            try
            {
                db.ExecuteCommand(@"
IF OBJECT_ID('dbo.EV_Program_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.EV_Program_tb
    (
        [id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [campaign_code] NVARCHAR(80) NULL,
        [campaign_name] NVARCHAR(300) NOT NULL,
        [campaign_type] NVARCHAR(40) NOT NULL,
        [description] NVARCHAR(MAX) NULL,
        [shop_scope] NVARCHAR(30) NULL,
        [status] NVARCHAR(30) NULL,
        [start_at] DATETIME NULL,
        [end_at] DATETIME NULL,
        [tier_step_percent] FLOAT NULL,
        [tier_max_percent] FLOAT NULL,
        [tier_cap_occurrence] INT NULL,
        [reward_unit] NVARCHAR(40) NULL,
        [definition_json] NVARCHAR(MAX) NULL,
        [version_no] INT NULL CONSTRAINT DF_EV_Program_version_no DEFAULT(1),
        [published_version_no] INT NULL,
        [published_snapshot_json] NVARCHAR(MAX) NULL,
        [approval_required] BIT NULL CONSTRAINT DF_EV_Program_approval_required DEFAULT(1),
        [is_deleted] BIT NULL CONSTRAINT DF_EV_Program_is_deleted DEFAULT(0),
        [created_at] DATETIME NULL,
        [updated_at] DATETIME NULL,
        [created_by] NVARCHAR(120) NULL,
        [updated_by] NVARCHAR(120) NULL,
        [published_by] NVARCHAR(120) NULL,
        [published_at] DATETIME NULL
    );
END

IF COL_LENGTH('dbo.EV_Program_tb', 'definition_json') IS NULL
    ALTER TABLE dbo.EV_Program_tb ADD [definition_json] NVARCHAR(MAX) NULL;

IF COL_LENGTH('dbo.EV_Program_tb', 'version_no') IS NULL
BEGIN
    ALTER TABLE dbo.EV_Program_tb ADD [version_no] INT NULL CONSTRAINT DF_EV_Program_version_no DEFAULT(1);
    UPDATE dbo.EV_Program_tb SET version_no = 1 WHERE version_no IS NULL;
END

IF COL_LENGTH('dbo.EV_Program_tb', 'published_version_no') IS NULL
    ALTER TABLE dbo.EV_Program_tb ADD [published_version_no] INT NULL;

IF COL_LENGTH('dbo.EV_Program_tb', 'published_snapshot_json') IS NULL
    ALTER TABLE dbo.EV_Program_tb ADD [published_snapshot_json] NVARCHAR(MAX) NULL;

IF OBJECT_ID('dbo.EV_ProgramTarget_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.EV_ProgramTarget_tb
    (
        [id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [program_id] BIGINT NOT NULL,
        [shop_account] NVARCHAR(120) NOT NULL,
        [created_at] DATETIME NULL,
        [created_by] NVARCHAR(120) NULL
    );
END

IF OBJECT_ID('dbo.EV_ProgramVersion_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.EV_ProgramVersion_tb
    (
        [id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [program_id] BIGINT NOT NULL,
        [version_no] INT NOT NULL,
        [definition_json] NVARCHAR(MAX) NULL,
        [change_note] NVARCHAR(400) NULL,
        [created_at] DATETIME NULL,
        [created_by] NVARCHAR(120) NULL
    );
END

IF OBJECT_ID('dbo.EV_ProgramPublic_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.EV_ProgramPublic_tb
    (
        [id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [program_id] BIGINT NOT NULL,
        [campaign_code] NVARCHAR(80) NULL,
        [campaign_name] NVARCHAR(300) NOT NULL,
        [campaign_type] NVARCHAR(40) NOT NULL,
        [shop_scope] NVARCHAR(30) NULL,
        [target_shop_count] INT NULL,
        [status] NVARCHAR(30) NULL,
        [start_at] DATETIME NULL,
        [end_at] DATETIME NULL,
        [published_version_no] INT NULL,
        [definition_json] NVARCHAR(MAX) NULL,
        [is_active] BIT NULL CONSTRAINT DF_EV_ProgramPublic_is_active DEFAULT(1),
        [is_deleted] BIT NULL CONSTRAINT DF_EV_ProgramPublic_is_deleted DEFAULT(0),
        [published_at] DATETIME NULL,
        [published_by] NVARCHAR(120) NULL,
        [updated_at] DATETIME NULL
    );
END

IF OBJECT_ID('dbo.EV_ProgramTemplate_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.EV_ProgramTemplate_tb
    (
        [id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [template_code] NVARCHAR(80) NOT NULL,
        [template_name] NVARCHAR(200) NOT NULL,
        [campaign_type] NVARCHAR(40) NOT NULL,
        [definition_schema_json] NVARCHAR(MAX) NULL,
        [is_active] BIT NULL CONSTRAINT DF_EV_ProgramTemplate_is_active DEFAULT(1),
        [created_at] DATETIME NULL,
        [updated_at] DATETIME NULL
    );
END

IF OBJECT_ID('dbo.EV_ProgramAudit_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.EV_ProgramAudit_tb
    (
        [id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [program_id] BIGINT NULL,
        [action_code] NVARCHAR(60) NULL,
        [action_note] NVARCHAR(MAX) NULL,
        [actor_account] NVARCHAR(120) NULL,
        [created_at] DATETIME NULL
    );
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_EV_Program_status_created'
      AND object_id = OBJECT_ID('dbo.EV_Program_tb')
)
BEGIN
    CREATE INDEX IX_EV_Program_status_created
        ON dbo.EV_Program_tb(status, created_at DESC, id DESC);
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_EV_Program_type_status'
      AND object_id = OBJECT_ID('dbo.EV_Program_tb')
)
BEGIN
    CREATE INDEX IX_EV_Program_type_status
        ON dbo.EV_Program_tb(campaign_type, status, id DESC);
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_EV_Program_code'
      AND object_id = OBJECT_ID('dbo.EV_Program_tb')
)
BEGIN
    CREATE INDEX IX_EV_Program_code
        ON dbo.EV_Program_tb(campaign_code);
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_EV_Program_published_version'
      AND object_id = OBJECT_ID('dbo.EV_Program_tb')
)
BEGIN
    CREATE INDEX IX_EV_Program_published_version
        ON dbo.EV_Program_tb(published_version_no, published_at DESC);
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_EV_ProgramTarget_program'
      AND object_id = OBJECT_ID('dbo.EV_ProgramTarget_tb')
)
BEGIN
    CREATE INDEX IX_EV_ProgramTarget_program
        ON dbo.EV_ProgramTarget_tb(program_id, shop_account);
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_EV_ProgramVersion_program_version'
      AND object_id = OBJECT_ID('dbo.EV_ProgramVersion_tb')
)
BEGIN
    CREATE INDEX IX_EV_ProgramVersion_program_version
        ON dbo.EV_ProgramVersion_tb(program_id, version_no DESC, id DESC);
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_EV_ProgramPublic_program_active'
      AND object_id = OBJECT_ID('dbo.EV_ProgramPublic_tb')
)
BEGIN
    CREATE INDEX IX_EV_ProgramPublic_program_active
        ON dbo.EV_ProgramPublic_tb(program_id, is_active, is_deleted, updated_at DESC, id DESC);
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_EV_ProgramPublic_type_status'
      AND object_id = OBJECT_ID('dbo.EV_ProgramPublic_tb')
)
BEGIN
    CREATE INDEX IX_EV_ProgramPublic_type_status
        ON dbo.EV_ProgramPublic_tb(campaign_type, status, is_active, updated_at DESC, id DESC);
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_EV_ProgramAudit_program_time'
      AND object_id = OBJECT_ID('dbo.EV_ProgramAudit_tb')
)
BEGIN
    CREATE INDEX IX_EV_ProgramAudit_program_time
        ON dbo.EV_ProgramAudit_tb(program_id, created_at DESC, id DESC);
END

IF NOT EXISTS (
    SELECT 1
    FROM dbo.EV_ProgramTemplate_tb
    WHERE template_code = 'voucher_tier_default'
)
BEGIN
    INSERT INTO dbo.EV_ProgramTemplate_tb
    (
        template_code, template_name, campaign_type, definition_schema_json, is_active, created_at, updated_at
    )
    VALUES
    (
        'voucher_tier_default',
        N'Mẫu voucher bậc thang mặc định',
        'voucher_tier',
        N'{""engine"":""tier"",""reward"":""voucher"",""params"":{""step_percent"":5,""max_percent"":50,""cap_occurrence"":10}}',
        1,
        GETDATE(),
        GETDATE()
    );
END

IF NOT EXISTS (
    SELECT 1
    FROM dbo.EV_ProgramTemplate_tb
    WHERE template_code = 'salary_bonus_tier_default'
)
BEGIN
    INSERT INTO dbo.EV_ProgramTemplate_tb
    (
        template_code, template_name, campaign_type, definition_schema_json, is_active, created_at, updated_at
    )
    VALUES
    (
        'salary_bonus_tier_default',
        N'Mẫu lương/thưởng bậc thang mặc định',
        'salary_bonus_tier',
        N'{""engine"":""tier"",""reward"":""salary_bonus"",""params"":{""step_percent"":5,""max_percent"":50,""cap_occurrence"":10}}',
        1,
        GETDATE(),
        GETDATE()
    );
END
");
            }
            catch
            {
                // Cố gắng migrate fallback ở dưới.
            }

            if (!IsSchemaReady(db))
                EnsureSchemaFallback(db);

            _schemaReady = IsSchemaReady(db);
        }
    }

    private static bool IsSchemaReady(dbDataContext db)
    {
        if (db == null)
            return false;

        int ready = db.ExecuteQuery<int>(@"
SELECT CASE
    WHEN OBJECT_ID('dbo.EV_Program_tb', 'U') IS NULL THEN 0
    WHEN COL_LENGTH('dbo.EV_Program_tb', 'definition_json') IS NULL THEN 0
    WHEN COL_LENGTH('dbo.EV_Program_tb', 'version_no') IS NULL THEN 0
    WHEN COL_LENGTH('dbo.EV_Program_tb', 'published_version_no') IS NULL THEN 0
    WHEN COL_LENGTH('dbo.EV_Program_tb', 'published_snapshot_json') IS NULL THEN 0
    WHEN OBJECT_ID('dbo.EV_ProgramVersion_tb', 'U') IS NULL THEN 0
    WHEN OBJECT_ID('dbo.EV_ProgramPublic_tb', 'U') IS NULL THEN 0
    WHEN OBJECT_ID('dbo.EV_ProgramTemplate_tb', 'U') IS NULL THEN 0
    WHEN OBJECT_ID('dbo.EV_ProgramAudit_tb', 'U') IS NULL THEN 0
    ELSE 1
END AS IsReady
").FirstOrDefault();

        return ready == 1;
    }

    private static void ExecuteIgnoreErrors(dbDataContext db, string sql)
    {
        if (db == null || string.IsNullOrWhiteSpace(sql))
            return;

        try
        {
            db.ExecuteCommand(sql);
        }
        catch
        {
            // Continue các bước khác để cứu schema.
        }
    }

    private static void EnsureSchemaFallback(dbDataContext db)
    {
        if (db == null)
            return;

        ExecuteIgnoreErrors(db, @"
IF OBJECT_ID('dbo.EV_Program_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.EV_Program_tb
    (
        [id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [campaign_code] NVARCHAR(80) NULL,
        [campaign_name] NVARCHAR(300) NOT NULL,
        [campaign_type] NVARCHAR(40) NOT NULL,
        [description] NVARCHAR(MAX) NULL,
        [shop_scope] NVARCHAR(30) NULL,
        [status] NVARCHAR(30) NULL,
        [start_at] DATETIME NULL,
        [end_at] DATETIME NULL,
        [tier_step_percent] FLOAT NULL,
        [tier_max_percent] FLOAT NULL,
        [tier_cap_occurrence] INT NULL,
        [reward_unit] NVARCHAR(40) NULL,
        [definition_json] NVARCHAR(MAX) NULL,
        [version_no] INT NULL CONSTRAINT DF_EV_Program_version_no DEFAULT(1),
        [published_version_no] INT NULL,
        [published_snapshot_json] NVARCHAR(MAX) NULL,
        [approval_required] BIT NULL CONSTRAINT DF_EV_Program_approval_required DEFAULT(1),
        [is_deleted] BIT NULL CONSTRAINT DF_EV_Program_is_deleted DEFAULT(0),
        [created_at] DATETIME NULL,
        [updated_at] DATETIME NULL,
        [created_by] NVARCHAR(120) NULL,
        [updated_by] NVARCHAR(120) NULL,
        [published_by] NVARCHAR(120) NULL,
        [published_at] DATETIME NULL
    );
END");

        ExecuteIgnoreErrors(db, "IF COL_LENGTH('dbo.EV_Program_tb', 'definition_json') IS NULL ALTER TABLE dbo.EV_Program_tb ADD [definition_json] NVARCHAR(MAX) NULL;");
        ExecuteIgnoreErrors(db, "IF COL_LENGTH('dbo.EV_Program_tb', 'version_no') IS NULL ALTER TABLE dbo.EV_Program_tb ADD [version_no] INT NULL;");
        ExecuteIgnoreErrors(db, "IF COL_LENGTH('dbo.EV_Program_tb', 'version_no') IS NOT NULL UPDATE dbo.EV_Program_tb SET [version_no] = 1 WHERE [version_no] IS NULL;");
        ExecuteIgnoreErrors(db, @"
IF COL_LENGTH('dbo.EV_Program_tb', 'version_no') IS NOT NULL
AND NOT EXISTS (
    SELECT 1
    FROM sys.default_constraints dc
    INNER JOIN sys.columns c ON c.default_object_id = dc.object_id
    WHERE dc.parent_object_id = OBJECT_ID('dbo.EV_Program_tb')
      AND c.name = 'version_no'
)
BEGIN
    ALTER TABLE dbo.EV_Program_tb
    ADD CONSTRAINT DF_EV_Program_version_no DEFAULT(1) FOR [version_no];
END");
        ExecuteIgnoreErrors(db, "IF COL_LENGTH('dbo.EV_Program_tb', 'published_version_no') IS NULL ALTER TABLE dbo.EV_Program_tb ADD [published_version_no] INT NULL;");
        ExecuteIgnoreErrors(db, "IF COL_LENGTH('dbo.EV_Program_tb', 'published_snapshot_json') IS NULL ALTER TABLE dbo.EV_Program_tb ADD [published_snapshot_json] NVARCHAR(MAX) NULL;");

        ExecuteIgnoreErrors(db, @"
IF OBJECT_ID('dbo.EV_ProgramTarget_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.EV_ProgramTarget_tb
    (
        [id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [program_id] BIGINT NOT NULL,
        [shop_account] NVARCHAR(120) NOT NULL,
        [created_at] DATETIME NULL,
        [created_by] NVARCHAR(120) NULL
    );
END");

        ExecuteIgnoreErrors(db, @"
IF OBJECT_ID('dbo.EV_ProgramVersion_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.EV_ProgramVersion_tb
    (
        [id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [program_id] BIGINT NOT NULL,
        [version_no] INT NOT NULL,
        [definition_json] NVARCHAR(MAX) NULL,
        [change_note] NVARCHAR(400) NULL,
        [created_at] DATETIME NULL,
        [created_by] NVARCHAR(120) NULL
    );
END");

        ExecuteIgnoreErrors(db, @"
IF OBJECT_ID('dbo.EV_ProgramPublic_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.EV_ProgramPublic_tb
    (
        [id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [program_id] BIGINT NOT NULL,
        [campaign_code] NVARCHAR(80) NULL,
        [campaign_name] NVARCHAR(300) NOT NULL,
        [campaign_type] NVARCHAR(40) NOT NULL,
        [shop_scope] NVARCHAR(30) NULL,
        [target_shop_count] INT NULL,
        [status] NVARCHAR(30) NULL,
        [start_at] DATETIME NULL,
        [end_at] DATETIME NULL,
        [published_version_no] INT NULL,
        [definition_json] NVARCHAR(MAX) NULL,
        [is_active] BIT NULL CONSTRAINT DF_EV_ProgramPublic_is_active DEFAULT(1),
        [is_deleted] BIT NULL CONSTRAINT DF_EV_ProgramPublic_is_deleted DEFAULT(0),
        [published_at] DATETIME NULL,
        [published_by] NVARCHAR(120) NULL,
        [updated_at] DATETIME NULL
    );
END");

        ExecuteIgnoreErrors(db, @"
IF OBJECT_ID('dbo.EV_ProgramTemplate_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.EV_ProgramTemplate_tb
    (
        [id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [template_code] NVARCHAR(80) NOT NULL,
        [template_name] NVARCHAR(200) NOT NULL,
        [campaign_type] NVARCHAR(40) NOT NULL,
        [definition_schema_json] NVARCHAR(MAX) NULL,
        [is_active] BIT NULL CONSTRAINT DF_EV_ProgramTemplate_is_active DEFAULT(1),
        [created_at] DATETIME NULL,
        [updated_at] DATETIME NULL
    );
END");

        ExecuteIgnoreErrors(db, @"
IF OBJECT_ID('dbo.EV_ProgramAudit_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.EV_ProgramAudit_tb
    (
        [id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [program_id] BIGINT NULL,
        [action_code] NVARCHAR(60) NULL,
        [action_note] NVARCHAR(MAX) NULL,
        [actor_account] NVARCHAR(120) NULL,
        [created_at] DATETIME NULL
    );
END");

        ExecuteIgnoreErrors(db, @"
IF OBJECT_ID('dbo.EV_Program_tb', 'U') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EV_Program_code' AND object_id = OBJECT_ID('dbo.EV_Program_tb'))
BEGIN
    CREATE INDEX IX_EV_Program_code ON dbo.EV_Program_tb(campaign_code);
END");

        ExecuteIgnoreErrors(db, @"
IF OBJECT_ID('dbo.EV_Program_tb', 'U') IS NOT NULL
AND COL_LENGTH('dbo.EV_Program_tb', 'published_version_no') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EV_Program_published_version' AND object_id = OBJECT_ID('dbo.EV_Program_tb'))
BEGIN
    CREATE INDEX IX_EV_Program_published_version ON dbo.EV_Program_tb(published_version_no, published_at DESC);
END");

        ExecuteIgnoreErrors(db, @"
IF OBJECT_ID('dbo.EV_ProgramVersion_tb', 'U') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EV_ProgramVersion_program_version' AND object_id = OBJECT_ID('dbo.EV_ProgramVersion_tb'))
BEGIN
    CREATE INDEX IX_EV_ProgramVersion_program_version ON dbo.EV_ProgramVersion_tb(program_id, version_no DESC, id DESC);
END");

        ExecuteIgnoreErrors(db, @"
IF OBJECT_ID('dbo.EV_ProgramPublic_tb', 'U') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EV_ProgramPublic_program_active' AND object_id = OBJECT_ID('dbo.EV_ProgramPublic_tb'))
BEGIN
    CREATE INDEX IX_EV_ProgramPublic_program_active ON dbo.EV_ProgramPublic_tb(program_id, is_active, is_deleted, updated_at DESC, id DESC);
END");

        ExecuteIgnoreErrors(db, @"
IF OBJECT_ID('dbo.EV_ProgramPublic_tb', 'U') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EV_ProgramPublic_type_status' AND object_id = OBJECT_ID('dbo.EV_ProgramPublic_tb'))
BEGIN
    CREATE INDEX IX_EV_ProgramPublic_type_status ON dbo.EV_ProgramPublic_tb(campaign_type, status, is_active, updated_at DESC, id DESC);
END");

        ExecuteIgnoreErrors(db, @"
IF OBJECT_ID('dbo.EV_ProgramTemplate_tb', 'U') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM dbo.EV_ProgramTemplate_tb WHERE template_code = 'voucher_tier_default')
BEGIN
    INSERT INTO dbo.EV_ProgramTemplate_tb
    (
        template_code, template_name, campaign_type, definition_schema_json, is_active, created_at, updated_at
    )
    VALUES
    (
        'voucher_tier_default',
        N'Mẫu voucher bậc thang mặc định',
        'voucher_tier',
        N'{""engine"":""tier"",""reward"":""voucher"",""params"":{""step_percent"":5,""max_percent"":50,""cap_occurrence"":10}}',
        1,
        GETDATE(),
        GETDATE()
    );
END");

        ExecuteIgnoreErrors(db, @"
IF OBJECT_ID('dbo.EV_ProgramTemplate_tb', 'U') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM dbo.EV_ProgramTemplate_tb WHERE template_code = 'salary_bonus_tier_default')
BEGIN
    INSERT INTO dbo.EV_ProgramTemplate_tb
    (
        template_code, template_name, campaign_type, definition_schema_json, is_active, created_at, updated_at
    )
    VALUES
    (
        'salary_bonus_tier_default',
        N'Mẫu lương/thưởng bậc thang mặc định',
        'salary_bonus_tier',
        N'{""engine"":""tier"",""reward"":""salary_bonus"",""params"":{""step_percent"":5,""max_percent"":50,""cap_occurrence"":10}}',
        1,
        GETDATE(),
        GETDATE()
    );
END");
    }
}
