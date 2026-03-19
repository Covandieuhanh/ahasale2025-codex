using System;

public static class AhaShineSupplySchema_cl
{
    private static bool _schemaReady;
    private static readonly object _schemaLock = new object();

    public static void EnsureSafe(dbDataContext db)
    {
        if (db == null)
            return;
        if (_schemaReady)
            return;

        lock (_schemaLock)
        {
            if (_schemaReady)
                return;

            int originalTimeout = db.CommandTimeout;
            try
            {
                if (originalTimeout <= 0 || originalTimeout < 180)
                    db.CommandTimeout = 180;

                db.ExecuteCommand(@"
IF OBJECT_ID('dbo.donnhap_vattu_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.donnhap_vattu_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [user_parent] NVarChar(MAX) NULL,
        [ngaytao] DateTime NULL,
        [nguoitao] NVarChar(MAX) NULL,
        [tongtien] BigInt NULL,
        [chietkhau] Int NULL,
        [tongtien_ck_hoadon] BigInt NULL,
        [tongsauchietkhau] BigInt NULL,
        [tenkhachhang] NVarChar(MAX) NULL,
        [sdt] NVarChar(MAX) NULL,
        [diachi] NVarChar(MAX) NULL,
        [ghichu] NVarChar(MAX) NULL,
        [sotien_dathanhtoan] BigInt NULL,
        [thanhtoan_tienmat] BigInt NULL,
        [thanhtoan_chuyenkhoan] BigInt NULL,
        [thanhtoan_quetthe] BigInt NULL,
        [sotien_conlai] BigInt NULL,
        [sl_dichvu] Int NULL,
        [sl_sanpham] Int NULL,
        [album] NVarChar(MAX) NULL,
        [dichvu_hay_sanpham] NVarChar(MAX) NULL,
        [ds_dichvu] BigInt NULL,
        [ds_sanpham] BigInt NULL,
        [sauck_dichvu] BigInt NULL,
        [sauck_sanpham] BigInt NULL,
        [km1_ghichu] NVarChar(MAX) NULL,
        [id_guide] UniqueIdentifier NULL,
        [nguongoc] NVarChar(MAX) NULL,
        [nhacungcap] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'user_parent') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [user_parent] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'ngaytao') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [ngaytao] DateTime NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'nguoitao') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [nguoitao] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'tongtien') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [tongtien] BigInt NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'chietkhau') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [chietkhau] Int NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'tongtien_ck_hoadon') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [tongtien_ck_hoadon] BigInt NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'tongsauchietkhau') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [tongsauchietkhau] BigInt NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'tenkhachhang') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [tenkhachhang] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'sdt') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [sdt] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'diachi') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [diachi] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'ghichu') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [ghichu] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'sotien_dathanhtoan') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [sotien_dathanhtoan] BigInt NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'thanhtoan_tienmat') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [thanhtoan_tienmat] BigInt NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'thanhtoan_chuyenkhoan') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [thanhtoan_chuyenkhoan] BigInt NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'thanhtoan_quetthe') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [thanhtoan_quetthe] BigInt NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'sotien_conlai') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [sotien_conlai] BigInt NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'sl_dichvu') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [sl_dichvu] Int NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'sl_sanpham') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [sl_sanpham] Int NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'album') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [album] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'dichvu_hay_sanpham') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [dichvu_hay_sanpham] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'ds_dichvu') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [ds_dichvu] BigInt NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'ds_sanpham') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [ds_sanpham] BigInt NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'sauck_dichvu') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [sauck_dichvu] BigInt NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'sauck_sanpham') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [sauck_sanpham] BigInt NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'km1_ghichu') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [km1_ghichu] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'id_guide') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [id_guide] UniqueIdentifier NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'nguongoc') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [nguongoc] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'nhacungcap') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [nhacungcap] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_table', 'id_chinhanh') IS NULL ALTER TABLE dbo.donnhap_vattu_table ADD [id_chinhanh] NVarChar(MAX) NULL;

IF OBJECT_ID('dbo.donnhap_vattu_chitiet_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.donnhap_vattu_chitiet_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [user_parent] NVarChar(MAX) NULL,
        [id_hoadon] NVarChar(MAX) NULL,
        [id_dvsp] NVarChar(MAX) NULL,
        [ten_dvsp_taithoidiemnay] NVarChar(MAX) NULL,
        [gia_dvsp_taithoidiemnay] BigInt NULL,
        [soluong] Int NULL,
        [thanhtien] BigInt NULL,
        [chietkhau] Int NULL,
        [tongtien_ck_dvsp] BigInt NULL,
        [tongsauchietkhau] BigInt NULL,
        [nguoichot_dvsp] NVarChar(MAX) NULL,
        [phantram_chotsale_dvsp] Int NULL,
        [tongtien_chotsale_dvsp] BigInt NULL,
        [nguoilam_dichvu] NVarChar(MAX) NULL,
        [phantram_lamdichvu] Int NULL,
        [tongtien_lamdichvu] BigInt NULL,
        [kyhieu] NVarChar(MAX) NULL,
        [tennguoichot_hientai] NVarChar(MAX) NULL,
        [tennguoilam_hientai] NVarChar(MAX) NULL,
        [hinhanh_hientai] NVarChar(MAX) NULL,
        [danhgia_nhanvien_lamdichvu] NVarChar(MAX) NULL,
        [ngaytao] DateTime NULL,
        [nguoitao] NVarChar(MAX) NULL,
        [nsx] Date NULL,
        [hsd] Date NULL,
        [solo] NVarChar(MAX) NULL,
        [sl_daban] Int NULL,
        [sl_conlai] Int NULL,
        [dvt] NVarChar(MAX) NULL,
        [image] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'user_parent') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [user_parent] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'id_hoadon') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [id_hoadon] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'id_dvsp') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [id_dvsp] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'ten_dvsp_taithoidiemnay') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [ten_dvsp_taithoidiemnay] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'gia_dvsp_taithoidiemnay') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [gia_dvsp_taithoidiemnay] BigInt NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'soluong') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [soluong] Int NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'thanhtien') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [thanhtien] BigInt NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'chietkhau') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [chietkhau] Int NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'tongtien_ck_dvsp') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [tongtien_ck_dvsp] BigInt NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'tongsauchietkhau') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [tongsauchietkhau] BigInt NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'nguoichot_dvsp') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [nguoichot_dvsp] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'phantram_chotsale_dvsp') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [phantram_chotsale_dvsp] Int NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'tongtien_chotsale_dvsp') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [tongtien_chotsale_dvsp] BigInt NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'nguoilam_dichvu') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [nguoilam_dichvu] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'phantram_lamdichvu') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [phantram_lamdichvu] Int NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'tongtien_lamdichvu') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [tongtien_lamdichvu] BigInt NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'kyhieu') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [kyhieu] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'tennguoichot_hientai') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [tennguoichot_hientai] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'tennguoilam_hientai') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [tennguoilam_hientai] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'hinhanh_hientai') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [hinhanh_hientai] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'danhgia_nhanvien_lamdichvu') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [danhgia_nhanvien_lamdichvu] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'ngaytao') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [ngaytao] DateTime NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'nguoitao') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [nguoitao] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'nsx') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [nsx] Date NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'hsd') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [hsd] Date NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'solo') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [solo] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'sl_daban') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [sl_daban] Int NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'sl_conlai') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [sl_conlai] Int NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'dvt') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [dvt] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'image') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [image] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'id_chinhanh') IS NULL ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [id_chinhanh] NVarChar(MAX) NULL;

IF OBJECT_ID('dbo.donnhap_vattu_lichsu_thanhtoan_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.donnhap_vattu_lichsu_thanhtoan_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [user_parent] NVarChar(MAX) NULL,
        [id_hoadon] NVarChar(MAX) NULL,
        [hinhthuc_thanhtoan] NVarChar(MAX) NULL,
        [sotienthanhtoan] BigInt NULL,
        [thoigian] DateTime NULL,
        [nguoithanhtoan] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.donnhap_vattu_lichsu_thanhtoan_table', 'user_parent') IS NULL ALTER TABLE dbo.donnhap_vattu_lichsu_thanhtoan_table ADD [user_parent] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_lichsu_thanhtoan_table', 'id_hoadon') IS NULL ALTER TABLE dbo.donnhap_vattu_lichsu_thanhtoan_table ADD [id_hoadon] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_lichsu_thanhtoan_table', 'hinhthuc_thanhtoan') IS NULL ALTER TABLE dbo.donnhap_vattu_lichsu_thanhtoan_table ADD [hinhthuc_thanhtoan] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_lichsu_thanhtoan_table', 'sotienthanhtoan') IS NULL ALTER TABLE dbo.donnhap_vattu_lichsu_thanhtoan_table ADD [sotienthanhtoan] BigInt NULL;
IF COL_LENGTH('dbo.donnhap_vattu_lichsu_thanhtoan_table', 'thoigian') IS NULL ALTER TABLE dbo.donnhap_vattu_lichsu_thanhtoan_table ADD [thoigian] DateTime NULL;
IF COL_LENGTH('dbo.donnhap_vattu_lichsu_thanhtoan_table', 'nguoithanhtoan') IS NULL ALTER TABLE dbo.donnhap_vattu_lichsu_thanhtoan_table ADD [nguoithanhtoan] NVarChar(MAX) NULL;
IF COL_LENGTH('dbo.donnhap_vattu_lichsu_thanhtoan_table', 'id_chinhanh') IS NULL ALTER TABLE dbo.donnhap_vattu_lichsu_thanhtoan_table ADD [id_chinhanh] NVarChar(MAX) NULL;
");

                _schemaReady = true;
            }
            catch
            {
            }
            finally
            {
                db.CommandTimeout = originalTimeout;
            }
        }
    }

    public static void ResetCache()
    {
        lock (_schemaLock)
        {
            _schemaReady = false;
        }
    }
}
