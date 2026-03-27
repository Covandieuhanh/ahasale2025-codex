using System;

public static class CoreSchemaMigration_cl
{
    public const string MigrationKeyRootAccessV1 = "core-root-access-v1.2";

    private static bool _schemaReady;
    private static readonly object _schemaLock = new object();

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
IF OBJECT_ID('dbo.CoreMigrationHistory_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CoreMigrationHistory_tb
    (
        Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        MigrationKey NVARCHAR(128) NOT NULL,
        AppliedAt DATETIME NOT NULL CONSTRAINT DF_CoreMigrationHistory_AppliedAt DEFAULT(GETDATE()),
        AppliedBy NVARCHAR(128) NULL,
        Note NVARCHAR(1000) NULL
    );
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_CoreMigrationHistory_MigrationKey'
      AND object_id = OBJECT_ID('dbo.CoreMigrationHistory_tb')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_CoreMigrationHistory_MigrationKey
        ON dbo.CoreMigrationHistory_tb(MigrationKey);
END

IF OBJECT_ID('dbo.CoreSpaceRequest_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CoreSpaceRequest_tb
    (
        Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        AccountKey NVARCHAR(128) NOT NULL,
        SpaceCode NVARCHAR(64) NOT NULL,
        RequestStatus NVARCHAR(32) NOT NULL CONSTRAINT DF_CoreSpaceRequest_RequestStatus DEFAULT('pending'),
        RequestSource NVARCHAR(64) NULL,
        RequestedBy NVARCHAR(128) NULL,
        RequestMetaJson NVARCHAR(MAX) NULL,
        RequestedAt DATETIME NOT NULL CONSTRAINT DF_CoreSpaceRequest_RequestedAt DEFAULT(GETDATE()),
        ReviewNote NVARCHAR(1000) NULL,
        ReviewedBy NVARCHAR(128) NULL,
        ReviewedAt DATETIME NULL,
        UpdatedAt DATETIME NOT NULL CONSTRAINT DF_CoreSpaceRequest_UpdatedAt DEFAULT(GETDATE())
    );
END

IF COL_LENGTH('dbo.CoreSpaceRequest_tb', 'RequestMetaJson') IS NULL
BEGIN
    ALTER TABLE dbo.CoreSpaceRequest_tb ADD RequestMetaJson NVARCHAR(MAX) NULL;
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_CoreSpaceRequest_Account_Space_Status'
      AND object_id = OBJECT_ID('dbo.CoreSpaceRequest_tb')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_CoreSpaceRequest_Account_Space_Status
        ON dbo.CoreSpaceRequest_tb(AccountKey, SpaceCode, RequestStatus, RequestedAt DESC);
END

IF OBJECT_ID('dbo.CoreShopPolicy_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CoreShopPolicy_tb
    (
        Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        AccountKey NVARCHAR(128) NOT NULL,
        CommissionPercent INT NOT NULL CONSTRAINT DF_CoreShopPolicy_CommissionPercent DEFAULT(0),
        PolicyStatus NVARCHAR(32) NOT NULL CONSTRAINT DF_CoreShopPolicy_Status DEFAULT('active'),
        ApprovedRequestId BIGINT NULL,
        ApprovedBy NVARCHAR(128) NULL,
        ApprovedAt DATETIME NULL,
        Note NVARCHAR(1000) NULL,
        CreatedAt DATETIME NOT NULL CONSTRAINT DF_CoreShopPolicy_CreatedAt DEFAULT(GETDATE()),
        UpdatedAt DATETIME NOT NULL CONSTRAINT DF_CoreShopPolicy_UpdatedAt DEFAULT(GETDATE())
    );
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_CoreShopPolicy_AccountKey'
      AND object_id = OBJECT_ID('dbo.CoreShopPolicy_tb')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_CoreShopPolicy_AccountKey
        ON dbo.CoreShopPolicy_tb(AccountKey);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_CoreShopPolicy_Status'
      AND object_id = OBJECT_ID('dbo.CoreShopPolicy_tb')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_CoreShopPolicy_Status
        ON dbo.CoreShopPolicy_tb(PolicyStatus, UpdatedAt DESC);
END

IF OBJECT_ID('dbo.CoreCompanyShopProductMap_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CoreCompanyShopProductMap_tb
    (
        Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        AccountKey NVARCHAR(128) NOT NULL,
        SystemProductId INT NOT NULL,
        ShopPostId INT NOT NULL,
        CreatedAt DATETIME NOT NULL CONSTRAINT DF_CoreCompanyShopProductMap_CreatedAt DEFAULT(GETDATE()),
        UpdatedAt DATETIME NOT NULL CONSTRAINT DF_CoreCompanyShopProductMap_UpdatedAt DEFAULT(GETDATE())
    );
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_CoreCompanyShopProductMap_Account_System'
      AND object_id = OBJECT_ID('dbo.CoreCompanyShopProductMap_tb')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_CoreCompanyShopProductMap_Account_System
        ON dbo.CoreCompanyShopProductMap_tb(AccountKey, SystemProductId);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_CoreCompanyShopProductMap_Account_Post'
      AND object_id = OBJECT_ID('dbo.CoreCompanyShopProductMap_tb')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_CoreCompanyShopProductMap_Account_Post
        ON dbo.CoreCompanyShopProductMap_tb(AccountKey, ShopPostId);
END

IF OBJECT_ID('dbo.CoreShopSpecialProduct_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CoreShopSpecialProduct_tb
    (
        Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        AccountKey NVARCHAR(128) NOT NULL,
        ShopPostId INT NOT NULL,
        SystemProductId INT NULL,
        HandlerCode NVARCHAR(64) NOT NULL,
        HandlerConfig NVARCHAR(MAX) NULL,
        HandlerStatus NVARCHAR(32) NOT NULL CONSTRAINT DF_CoreShopSpecialProduct_Status DEFAULT('active'),
        Note NVARCHAR(1000) NULL,
        CreatedBy NVARCHAR(128) NULL,
        UpdatedBy NVARCHAR(128) NULL,
        CreatedAt DATETIME NOT NULL CONSTRAINT DF_CoreShopSpecialProduct_CreatedAt DEFAULT(GETDATE()),
        UpdatedAt DATETIME NOT NULL CONSTRAINT DF_CoreShopSpecialProduct_UpdatedAt DEFAULT(GETDATE())
    );
END

IF COL_LENGTH('dbo.CoreShopSpecialProduct_tb', 'SystemProductId') IS NULL
BEGIN
    ALTER TABLE dbo.CoreShopSpecialProduct_tb ADD SystemProductId INT NULL;
END

IF COL_LENGTH('dbo.CoreShopSpecialProduct_tb', 'HandlerConfig') IS NULL
BEGIN
    ALTER TABLE dbo.CoreShopSpecialProduct_tb ADD HandlerConfig NVARCHAR(MAX) NULL;
END

IF COL_LENGTH('dbo.CoreShopSpecialProduct_tb', 'HandlerStatus') IS NULL
BEGIN
    ALTER TABLE dbo.CoreShopSpecialProduct_tb ADD HandlerStatus NVARCHAR(32) NOT NULL CONSTRAINT DF_CoreShopSpecialProduct_Status_Compat DEFAULT('active');
END

IF COL_LENGTH('dbo.CoreShopSpecialProduct_tb', 'Note') IS NULL
BEGIN
    ALTER TABLE dbo.CoreShopSpecialProduct_tb ADD Note NVARCHAR(1000) NULL;
END

IF COL_LENGTH('dbo.CoreShopSpecialProduct_tb', 'CreatedBy') IS NULL
BEGIN
    ALTER TABLE dbo.CoreShopSpecialProduct_tb ADD CreatedBy NVARCHAR(128) NULL;
END

IF COL_LENGTH('dbo.CoreShopSpecialProduct_tb', 'UpdatedBy') IS NULL
BEGIN
    ALTER TABLE dbo.CoreShopSpecialProduct_tb ADD UpdatedBy NVARCHAR(128) NULL;
END

IF COL_LENGTH('dbo.CoreShopSpecialProduct_tb', 'CreatedAt') IS NULL
BEGIN
    ALTER TABLE dbo.CoreShopSpecialProduct_tb ADD CreatedAt DATETIME NOT NULL CONSTRAINT DF_CoreShopSpecialProduct_CreatedAt_Compat DEFAULT(GETDATE());
END

IF COL_LENGTH('dbo.CoreShopSpecialProduct_tb', 'UpdatedAt') IS NULL
BEGIN
    ALTER TABLE dbo.CoreShopSpecialProduct_tb ADD UpdatedAt DATETIME NOT NULL CONSTRAINT DF_CoreShopSpecialProduct_UpdatedAt_Compat DEFAULT(GETDATE());
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_CoreShopSpecialProduct_Account_Post'
      AND object_id = OBJECT_ID('dbo.CoreShopSpecialProduct_tb')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_CoreShopSpecialProduct_Account_Post
        ON dbo.CoreShopSpecialProduct_tb(AccountKey, ShopPostId);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_CoreShopSpecialProduct_Account_System'
      AND object_id = OBJECT_ID('dbo.CoreShopSpecialProduct_tb')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_CoreShopSpecialProduct_Account_System
        ON dbo.CoreShopSpecialProduct_tb(AccountKey, SystemProductId)
        WHERE SystemProductId IS NOT NULL;
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_CoreShopSpecialProduct_Status'
      AND object_id = OBJECT_ID('dbo.CoreShopSpecialProduct_tb')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_CoreShopSpecialProduct_Status
        ON dbo.CoreShopSpecialProduct_tb(HandlerStatus, AccountKey, ShopPostId);
END

IF OBJECT_ID('dbo.CoreShopSpecialExecution_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CoreShopSpecialExecution_tb
    (
        Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        SaleHistoryId BIGINT NOT NULL,
        AccountKey NVARCHAR(128) NULL,
        ShopPostId INT NOT NULL,
        HandlerCode NVARCHAR(64) NOT NULL,
        ExecutionStatus NVARCHAR(32) NOT NULL CONSTRAINT DF_CoreShopSpecialExecution_Status DEFAULT('success'),
        ExecutionSummary NVARCHAR(1000) NULL,
        ExecutionData NVARCHAR(MAX) NULL,
        ExecutedAt DATETIME NOT NULL CONSTRAINT DF_CoreShopSpecialExecution_ExecutedAt DEFAULT(GETDATE())
    );
END

IF COL_LENGTH('dbo.CoreShopSpecialExecution_tb', 'AccountKey') IS NULL
BEGIN
    ALTER TABLE dbo.CoreShopSpecialExecution_tb ADD AccountKey NVARCHAR(128) NULL;
END

IF COL_LENGTH('dbo.CoreShopSpecialExecution_tb', 'ExecutionStatus') IS NULL
BEGIN
    ALTER TABLE dbo.CoreShopSpecialExecution_tb ADD ExecutionStatus NVARCHAR(32) NOT NULL CONSTRAINT DF_CoreShopSpecialExecution_Status_Compat DEFAULT('success');
END

IF COL_LENGTH('dbo.CoreShopSpecialExecution_tb', 'ExecutionSummary') IS NULL
BEGIN
    ALTER TABLE dbo.CoreShopSpecialExecution_tb ADD ExecutionSummary NVARCHAR(1000) NULL;
END

IF COL_LENGTH('dbo.CoreShopSpecialExecution_tb', 'ExecutionData') IS NULL
BEGIN
    ALTER TABLE dbo.CoreShopSpecialExecution_tb ADD ExecutionData NVARCHAR(MAX) NULL;
END

IF COL_LENGTH('dbo.CoreShopSpecialExecution_tb', 'ExecutedAt') IS NULL
BEGIN
    ALTER TABLE dbo.CoreShopSpecialExecution_tb ADD ExecutedAt DATETIME NOT NULL CONSTRAINT DF_CoreShopSpecialExecution_ExecutedAt_Compat DEFAULT(GETDATE());
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_CoreShopSpecialExecution_Sale_Handler'
      AND object_id = OBJECT_ID('dbo.CoreShopSpecialExecution_tb')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_CoreShopSpecialExecution_Sale_Handler
        ON dbo.CoreShopSpecialExecution_tb(SaleHistoryId, HandlerCode);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_CoreShopSpecialExecution_Account_ExecutedAt'
      AND object_id = OBJECT_ID('dbo.CoreShopSpecialExecution_tb')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_CoreShopSpecialExecution_Account_ExecutedAt
        ON dbo.CoreShopSpecialExecution_tb(AccountKey, ExecutedAt DESC, SaleHistoryId);
END

IF OBJECT_ID('dbo.CoreSpaceAccess_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CoreSpaceAccess_tb
    (
        Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        AccountKey NVARCHAR(128) NOT NULL,
        SpaceCode NVARCHAR(64) NOT NULL,
        AccessStatus NVARCHAR(32) NOT NULL CONSTRAINT DF_CoreSpaceAccess_AccessStatus DEFAULT('active'),
        AccessSource NVARCHAR(64) NULL,
        IsPrimary BIT NOT NULL CONSTRAINT DF_CoreSpaceAccess_IsPrimary DEFAULT(0),
        ApprovedRequestId BIGINT NULL,
        ApprovedBy NVARCHAR(128) NULL,
        ApprovedAt DATETIME NULL,
        LockedReason NVARCHAR(500) NULL,
        CreatedAt DATETIME NOT NULL CONSTRAINT DF_CoreSpaceAccess_CreatedAt DEFAULT(GETDATE()),
        UpdatedAt DATETIME NOT NULL CONSTRAINT DF_CoreSpaceAccess_UpdatedAt DEFAULT(GETDATE())
    );
END

IF COL_LENGTH('dbo.CoreSpaceAccess_tb', 'ApprovedRequestId') IS NULL
BEGIN
    ALTER TABLE dbo.CoreSpaceAccess_tb ADD ApprovedRequestId BIGINT NULL;
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_CoreSpaceAccess_Account_Space'
      AND object_id = OBJECT_ID('dbo.CoreSpaceAccess_tb')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_CoreSpaceAccess_Account_Space
        ON dbo.CoreSpaceAccess_tb(AccountKey, SpaceCode);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_CoreSpaceAccess_Space_Status'
      AND object_id = OBJECT_ID('dbo.CoreSpaceAccess_tb')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_CoreSpaceAccess_Space_Status
        ON dbo.CoreSpaceAccess_tb(SpaceCode, AccessStatus, AccountKey);
END

IF OBJECT_ID('dbo.CorePermissionGrant_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CorePermissionGrant_tb
    (
        Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        AccountKey NVARCHAR(128) NOT NULL,
        PermissionCode NVARCHAR(128) NOT NULL,
        GrantStatus NVARCHAR(32) NOT NULL CONSTRAINT DF_CorePermissionGrant_GrantStatus DEFAULT('active'),
        GrantSource NVARCHAR(64) NULL,
        GrantedBy NVARCHAR(128) NULL,
        GrantedAt DATETIME NOT NULL CONSTRAINT DF_CorePermissionGrant_GrantedAt DEFAULT(GETDATE()),
        ExpiresAt DATETIME NULL,
        MetaJson NVARCHAR(MAX) NULL,
        UpdatedAt DATETIME NOT NULL CONSTRAINT DF_CorePermissionGrant_UpdatedAt DEFAULT(GETDATE())
    );
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_CorePermissionGrant_Account_Permission'
      AND object_id = OBJECT_ID('dbo.CorePermissionGrant_tb')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_CorePermissionGrant_Account_Permission
        ON dbo.CorePermissionGrant_tb(AccountKey, PermissionCode);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_CorePermissionGrant_Status'
      AND object_id = OBJECT_ID('dbo.CorePermissionGrant_tb')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_CorePermissionGrant_Status
        ON dbo.CorePermissionGrant_tb(GrantStatus, AccountKey);
END

IF OBJECT_ID('dbo.CoreGianHangOnboarding_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CoreGianHangOnboarding_tb
    (
        Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        AccountKey NVARCHAR(128) NOT NULL,
        ShopName NVARCHAR(160) NOT NULL,
        ContactName NVARCHAR(160) NULL,
        ContactPhone NVARCHAR(64) NULL,
        ContactEmail NVARCHAR(255) NULL,
        PickupAddress NVARCHAR(1000) NULL,
        LastRequestId BIGINT NULL,
        CreatedAt DATETIME NOT NULL CONSTRAINT DF_CoreGianHangOnboarding_CreatedAt DEFAULT(GETDATE()),
        UpdatedAt DATETIME NOT NULL CONSTRAINT DF_CoreGianHangOnboarding_UpdatedAt DEFAULT(GETDATE())
    );
END

IF COL_LENGTH('dbo.CoreGianHangOnboarding_tb', 'LastRequestId') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangOnboarding_tb ADD LastRequestId BIGINT NULL;
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_CoreGianHangOnboarding_AccountKey'
      AND object_id = OBJECT_ID('dbo.CoreGianHangOnboarding_tb')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_CoreGianHangOnboarding_AccountKey
        ON dbo.CoreGianHangOnboarding_tb(AccountKey);
END

IF OBJECT_ID('dbo.CoreGianHangAdminMember_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CoreGianHangAdminMember_tb
    (
        Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        OwnerAccountKey NVARCHAR(128) NOT NULL,
        MemberAccountKey NVARCHAR(128) NULL,
        LegacyUser NVARCHAR(128) NOT NULL,
        MembershipStatus NVARCHAR(32) NOT NULL CONSTRAINT DF_CoreGianHangAdminMember_Status DEFAULT('active'),
        RoleLabel NVARCHAR(128) NULL,
        MemberType NVARCHAR(32) NULL,
        PendingPhone NVARCHAR(32) NULL,
        InviteLookupKey NVARCHAR(128) NULL,
        CreatedBy NVARCHAR(128) NULL,
        AcceptedAt DATETIME NULL,
        CreatedAt DATETIME NOT NULL CONSTRAINT DF_CoreGianHangAdminMember_CreatedAt DEFAULT(GETDATE()),
        UpdatedAt DATETIME NOT NULL CONSTRAINT DF_CoreGianHangAdminMember_UpdatedAt DEFAULT(GETDATE())
    );
END

IF COL_LENGTH('dbo.CoreGianHangAdminMember_tb', 'MemberType') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangAdminMember_tb ADD MemberType NVARCHAR(32) NULL;
END

IF COL_LENGTH('dbo.CoreGianHangAdminMember_tb', 'PendingPhone') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangAdminMember_tb ADD PendingPhone NVARCHAR(32) NULL;
END

IF COL_LENGTH('dbo.CoreGianHangAdminMember_tb', 'InviteLookupKey') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangAdminMember_tb ADD InviteLookupKey NVARCHAR(128) NULL;
END

IF COL_LENGTH('dbo.CoreGianHangAdminMember_tb', 'AcceptedAt') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangAdminMember_tb ADD AcceptedAt DATETIME NULL;
END

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.CoreGianHangAdminMember_tb')
      AND name = 'MemberAccountKey'
      AND is_nullable = 0
)
BEGIN
    ALTER TABLE dbo.CoreGianHangAdminMember_tb ALTER COLUMN MemberAccountKey NVARCHAR(128) NULL;
END

IF EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_CoreGianHangAdminMember_Owner_Member'
      AND object_id = OBJECT_ID('dbo.CoreGianHangAdminMember_tb')
)
BEGIN
    DROP INDEX UX_CoreGianHangAdminMember_Owner_Member ON dbo.CoreGianHangAdminMember_tb;
END

CREATE UNIQUE NONCLUSTERED INDEX UX_CoreGianHangAdminMember_Owner_Member
    ON dbo.CoreGianHangAdminMember_tb(OwnerAccountKey, MemberAccountKey)
    WHERE MemberAccountKey IS NOT NULL;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_CoreGianHangAdminMember_Owner_LegacyUser'
      AND object_id = OBJECT_ID('dbo.CoreGianHangAdminMember_tb')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_CoreGianHangAdminMember_Owner_LegacyUser
        ON dbo.CoreGianHangAdminMember_tb(OwnerAccountKey, LegacyUser);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_CoreGianHangAdminMember_Member_Status'
      AND object_id = OBJECT_ID('dbo.CoreGianHangAdminMember_tb')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_CoreGianHangAdminMember_Member_Status
        ON dbo.CoreGianHangAdminMember_tb(MemberAccountKey, MembershipStatus, OwnerAccountKey);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_CoreGianHangAdminMember_PendingPhone_Status'
      AND object_id = OBJECT_ID('dbo.CoreGianHangAdminMember_tb')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_CoreGianHangAdminMember_PendingPhone_Status
        ON dbo.CoreGianHangAdminMember_tb(PendingPhone, MembershipStatus, OwnerAccountKey);
END

IF OBJECT_ID('dbo.CoreGianHangPersonLink_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CoreGianHangPersonLink_tb
    (
        Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        OwnerAccountKey NVARCHAR(128) NOT NULL,
        NormalizedPhone NVARCHAR(32) NOT NULL,
        PrimaryName NVARCHAR(255) NULL,
        HomeAccountKey NVARCHAR(128) NULL,
        LinkStatus NVARCHAR(32) NOT NULL CONSTRAINT DF_CoreGianHangPersonLink_Status DEFAULT('active'),
        PendingPhone NVARCHAR(32) NULL,
        InviteLookupKey NVARCHAR(128) NULL,
        CreatedBy NVARCHAR(128) NULL,
        AcceptedAt DATETIME NULL,
        CreatedAt DATETIME NOT NULL CONSTRAINT DF_CoreGianHangPersonLink_CreatedAt DEFAULT(GETDATE()),
        UpdatedAt DATETIME NOT NULL CONSTRAINT DF_CoreGianHangPersonLink_UpdatedAt DEFAULT(GETDATE())
    );
END

IF COL_LENGTH('dbo.CoreGianHangPersonLink_tb', 'PrimaryName') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangPersonLink_tb ADD PrimaryName NVARCHAR(255) NULL;
END

IF COL_LENGTH('dbo.CoreGianHangPersonLink_tb', 'HomeAccountKey') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangPersonLink_tb ADD HomeAccountKey NVARCHAR(128) NULL;
END

IF COL_LENGTH('dbo.CoreGianHangPersonLink_tb', 'LinkStatus') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangPersonLink_tb ADD LinkStatus NVARCHAR(32) NOT NULL CONSTRAINT DF_CoreGianHangPersonLink_Status_Compat DEFAULT('active');
END

IF COL_LENGTH('dbo.CoreGianHangPersonLink_tb', 'PendingPhone') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangPersonLink_tb ADD PendingPhone NVARCHAR(32) NULL;
END

IF COL_LENGTH('dbo.CoreGianHangPersonLink_tb', 'InviteLookupKey') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangPersonLink_tb ADD InviteLookupKey NVARCHAR(128) NULL;
END

IF COL_LENGTH('dbo.CoreGianHangPersonLink_tb', 'CreatedBy') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangPersonLink_tb ADD CreatedBy NVARCHAR(128) NULL;
END

IF COL_LENGTH('dbo.CoreGianHangPersonLink_tb', 'AcceptedAt') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangPersonLink_tb ADD AcceptedAt DATETIME NULL;
END

IF COL_LENGTH('dbo.CoreGianHangPersonLink_tb', 'CreatedAt') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangPersonLink_tb ADD CreatedAt DATETIME NOT NULL CONSTRAINT DF_CoreGianHangPersonLink_CreatedAt_Compat DEFAULT(GETDATE());
END

IF COL_LENGTH('dbo.CoreGianHangPersonLink_tb', 'UpdatedAt') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangPersonLink_tb ADD UpdatedAt DATETIME NOT NULL CONSTRAINT DF_CoreGianHangPersonLink_UpdatedAt_Compat DEFAULT(GETDATE());
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_CoreGianHangPersonLink_Owner_Phone'
      AND object_id = OBJECT_ID('dbo.CoreGianHangPersonLink_tb')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_CoreGianHangPersonLink_Owner_Phone
        ON dbo.CoreGianHangPersonLink_tb(OwnerAccountKey, NormalizedPhone);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_CoreGianHangPersonLink_Home_Status'
      AND object_id = OBJECT_ID('dbo.CoreGianHangPersonLink_tb')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_CoreGianHangPersonLink_Home_Status
        ON dbo.CoreGianHangPersonLink_tb(HomeAccountKey, LinkStatus, OwnerAccountKey);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_CoreGianHangPersonLink_Pending_Status'
      AND object_id = OBJECT_ID('dbo.CoreGianHangPersonLink_tb')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_CoreGianHangPersonLink_Pending_Status
        ON dbo.CoreGianHangPersonLink_tb(PendingPhone, LinkStatus, OwnerAccountKey);
END

IF OBJECT_ID('dbo.CoreGianHangSourceLifecycle_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CoreGianHangSourceLifecycle_tb
    (
        Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        OwnerAccountKey NVARCHAR(128) NOT NULL,
        SourceType NVARCHAR(64) NOT NULL,
        SourceKey NVARCHAR(128) NOT NULL,
        LifecycleStatus NVARCHAR(32) NOT NULL CONSTRAINT DF_CoreGianHangSourceLifecycle_Status DEFAULT('active'),
        LifecycleLabel NVARCHAR(255) NULL,
        Note NVARCHAR(1000) NULL,
        ChangedBy NVARCHAR(128) NULL,
        CreatedAt DATETIME NOT NULL CONSTRAINT DF_CoreGianHangSourceLifecycle_CreatedAt DEFAULT(GETDATE()),
        UpdatedAt DATETIME NOT NULL CONSTRAINT DF_CoreGianHangSourceLifecycle_UpdatedAt DEFAULT(GETDATE())
    );
END

IF COL_LENGTH('dbo.CoreGianHangSourceLifecycle_tb', 'LifecycleStatus') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangSourceLifecycle_tb ADD LifecycleStatus NVARCHAR(32) NOT NULL CONSTRAINT DF_CoreGianHangSourceLifecycle_Status_Compat DEFAULT('active');
END

IF COL_LENGTH('dbo.CoreGianHangSourceLifecycle_tb', 'LifecycleLabel') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangSourceLifecycle_tb ADD LifecycleLabel NVARCHAR(255) NULL;
END

IF COL_LENGTH('dbo.CoreGianHangSourceLifecycle_tb', 'Note') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangSourceLifecycle_tb ADD Note NVARCHAR(1000) NULL;
END

IF COL_LENGTH('dbo.CoreGianHangSourceLifecycle_tb', 'ChangedBy') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangSourceLifecycle_tb ADD ChangedBy NVARCHAR(128) NULL;
END

IF COL_LENGTH('dbo.CoreGianHangSourceLifecycle_tb', 'CreatedAt') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangSourceLifecycle_tb ADD CreatedAt DATETIME NOT NULL CONSTRAINT DF_CoreGianHangSourceLifecycle_CreatedAt_Compat DEFAULT(GETDATE());
END

IF COL_LENGTH('dbo.CoreGianHangSourceLifecycle_tb', 'UpdatedAt') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangSourceLifecycle_tb ADD UpdatedAt DATETIME NOT NULL CONSTRAINT DF_CoreGianHangSourceLifecycle_UpdatedAt_Compat DEFAULT(GETDATE());
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_CoreGianHangSourceLifecycle_Owner_Source'
      AND object_id = OBJECT_ID('dbo.CoreGianHangSourceLifecycle_tb')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_CoreGianHangSourceLifecycle_Owner_Source
        ON dbo.CoreGianHangSourceLifecycle_tb(OwnerAccountKey, SourceType, SourceKey);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_CoreGianHangSourceLifecycle_Status'
      AND object_id = OBJECT_ID('dbo.CoreGianHangSourceLifecycle_tb')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_CoreGianHangSourceLifecycle_Status
        ON dbo.CoreGianHangSourceLifecycle_tb(OwnerAccountKey, LifecycleStatus, SourceType);
END

IF OBJECT_ID('dbo.CoreGianHangPersonSourceEvent_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CoreGianHangPersonSourceEvent_tb
    (
        Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        OwnerAccountKey NVARCHAR(128) NOT NULL,
        NormalizedPhone NVARCHAR(32) NOT NULL,
        SourceType NVARCHAR(64) NULL,
        SourceLabel NVARCHAR(128) NULL,
        SourceKey NVARCHAR(128) NULL,
        RoleLabel NVARCHAR(128) NULL,
        DisplayName NVARCHAR(255) NULL,
        EventType NVARCHAR(64) NOT NULL CONSTRAINT DF_CoreGianHangPersonSourceEvent_Type DEFAULT('source_removed'),
        EventNote NVARCHAR(512) NULL,
        CreatedBy NVARCHAR(128) NULL,
        CreatedAt DATETIME NOT NULL CONSTRAINT DF_CoreGianHangPersonSourceEvent_CreatedAt DEFAULT(GETDATE())
    );
END

IF COL_LENGTH('dbo.CoreGianHangPersonSourceEvent_tb', 'SourceType') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangPersonSourceEvent_tb ADD SourceType NVARCHAR(64) NULL;
END

IF COL_LENGTH('dbo.CoreGianHangPersonSourceEvent_tb', 'SourceLabel') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangPersonSourceEvent_tb ADD SourceLabel NVARCHAR(128) NULL;
END

IF COL_LENGTH('dbo.CoreGianHangPersonSourceEvent_tb', 'SourceKey') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangPersonSourceEvent_tb ADD SourceKey NVARCHAR(128) NULL;
END

IF COL_LENGTH('dbo.CoreGianHangPersonSourceEvent_tb', 'RoleLabel') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangPersonSourceEvent_tb ADD RoleLabel NVARCHAR(128) NULL;
END

IF COL_LENGTH('dbo.CoreGianHangPersonSourceEvent_tb', 'DisplayName') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangPersonSourceEvent_tb ADD DisplayName NVARCHAR(255) NULL;
END

IF COL_LENGTH('dbo.CoreGianHangPersonSourceEvent_tb', 'EventType') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangPersonSourceEvent_tb ADD EventType NVARCHAR(64) NOT NULL CONSTRAINT DF_CoreGianHangPersonSourceEvent_Type_Compat DEFAULT('source_removed');
END

IF COL_LENGTH('dbo.CoreGianHangPersonSourceEvent_tb', 'EventNote') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangPersonSourceEvent_tb ADD EventNote NVARCHAR(512) NULL;
END

IF COL_LENGTH('dbo.CoreGianHangPersonSourceEvent_tb', 'CreatedBy') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangPersonSourceEvent_tb ADD CreatedBy NVARCHAR(128) NULL;
END

IF COL_LENGTH('dbo.CoreGianHangPersonSourceEvent_tb', 'CreatedAt') IS NULL
BEGIN
    ALTER TABLE dbo.CoreGianHangPersonSourceEvent_tb ADD CreatedAt DATETIME NOT NULL CONSTRAINT DF_CoreGianHangPersonSourceEvent_CreatedAt_Compat DEFAULT(GETDATE());
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_CoreGianHangPersonSourceEvent_OwnerPhone'
      AND object_id = OBJECT_ID('dbo.CoreGianHangPersonSourceEvent_tb')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_CoreGianHangPersonSourceEvent_OwnerPhone
        ON dbo.CoreGianHangPersonSourceEvent_tb(OwnerAccountKey, NormalizedPhone, CreatedAt DESC);
END

IF NOT EXISTS (
    SELECT 1 FROM dbo.CoreMigrationHistory_tb WHERE MigrationKey = '" + MigrationKeyRootAccessV1 + @"'
)
BEGIN
    INSERT INTO dbo.CoreMigrationHistory_tb(MigrationKey, AppliedAt, AppliedBy, Note)
    VALUES ('" + MigrationKeyRootAccessV1 + @"', GETDATE(), 'app', 'Root access host-safe schema bootstrap');
END
");

                _schemaReady = true;
            }
            catch
            {
                // Keep old system running. Retry on a later request.
            }
        }
    }
}
