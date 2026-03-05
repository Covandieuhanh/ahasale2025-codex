SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;

IF COL_LENGTH('dbo.DonHang_tb', 'order_status') IS NULL
BEGIN
    ALTER TABLE dbo.DonHang_tb
    ADD order_status NVARCHAR(50) NULL;
END;

IF COL_LENGTH('dbo.DonHang_tb', 'exchange_status') IS NULL
BEGIN
    ALTER TABLE dbo.DonHang_tb
    ADD exchange_status NVARCHAR(50) NULL;
END;

EXEC(N'
    UPDATE dbo.DonHang_tb
    SET order_status =
        CASE
            WHEN trangthai = N''Đã giao'' THEN N''Đã giao''
            WHEN trangthai = N''Đã nhận'' THEN N''Đã nhận''
            WHEN trangthai = N''Đã hủy'' THEN N''Đã hủy''
            ELSE N''Đã đặt''
        END
    WHERE order_status IS NULL
       OR LTRIM(RTRIM(order_status)) = N'''';
');

EXEC(N'
    UPDATE dbo.DonHang_tb
    SET exchange_status =
        CASE
            WHEN trangthai = N''Chờ Trao đổi'' THEN N''Chờ Trao đổi''
            WHEN trangthai = N''Đã Trao đổi'' THEN N''Đã Trao đổi''
            WHEN trangthai = N''Đã nhận'' THEN N''Đã Trao đổi''
            ELSE N''Chưa Trao đổi''
        END
    WHERE exchange_status IS NULL
       OR LTRIM(RTRIM(exchange_status)) = N'''';
');

EXEC(N'
    UPDATE dbo.DonHang_tb
    SET order_status =
        CASE
            WHEN order_status IN (N''Đã đặt'', N''Đã giao'', N''Đã nhận'', N''Đã hủy'') THEN order_status
            ELSE N''Đã đặt''
        END;
');

EXEC(N'
    UPDATE dbo.DonHang_tb
    SET exchange_status =
        CASE
            WHEN exchange_status IN (N''Chưa Trao đổi'', N''Chờ Trao đổi'', N''Đã Trao đổi'') THEN exchange_status
            ELSE N''Chưa Trao đổi''
        END;
');

EXEC(N'
    SELECT
        has_order_status = CASE WHEN COL_LENGTH(''dbo.DonHang_tb'', ''order_status'') IS NULL THEN 0 ELSE 1 END,
        has_exchange_status = CASE WHEN COL_LENGTH(''dbo.DonHang_tb'', ''exchange_status'') IS NULL THEN 0 ELSE 1 END,
        total_orders = COUNT(1),
        waiting_exchange_orders = SUM(CASE WHEN exchange_status = N''Chờ Trao đổi'' THEN 1 ELSE 0 END),
        delivered_orders = SUM(CASE WHEN order_status = N''Đã giao'' THEN 1 ELSE 0 END),
        received_orders = SUM(CASE WHEN order_status = N''Đã nhận'' THEN 1 ELSE 0 END),
        cancelled_orders = SUM(CASE WHEN order_status = N''Đã hủy'' THEN 1 ELSE 0 END)
    FROM dbo.DonHang_tb;
');
