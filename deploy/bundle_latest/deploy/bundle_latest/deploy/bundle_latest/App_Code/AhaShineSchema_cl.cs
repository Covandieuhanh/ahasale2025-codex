using System;

public static class AhaShineSchema_cl
{
    private static bool _schemaReady;
    private static readonly object _schemaLock = new object();

    public static void ResetSchemaCache()
    {
        lock (_schemaLock)
        {
            _schemaReady = false;
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
                int originalTimeout = db.CommandTimeout;
                if (originalTimeout <= 0 || originalTimeout < 600)
                    db.CommandTimeout = 600;

                db.ExecuteCommand(@"
IF OBJECT_ID('dbo.bspa_bophan_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.bspa_bophan_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [tenbophan] NVarChar(MAX) NULL,
        [user_parent] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.bspa_bophan_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_bophan_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.bspa_bophan_table', 'tenbophan') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_bophan_table ADD [tenbophan] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_bophan_table', 'user_parent') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_bophan_table ADD [user_parent] NVarChar(MAX) NULL;
END

IF OBJECT_ID('dbo.web_post_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.web_post_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [id_category] NVarChar(MAX) NULL,
        [name] NVarChar(MAX) NULL,
        [name_en] VarChar(MAX) NULL,
        [content_post] NVarChar(MAX) NULL,
        [description] NVarChar(MAX) NULL,
        [image] VarChar(MAX) NULL,
        [ngaytao] DateTime NULL,
        [bin] Bit NULL,
        [noibat] Bit NULL,
        [phanloai] NVarChar(MAX) NULL,
        [giaban_sanpham] BigInt NULL,
        [phantram_chotsale_sanpham] Int NULL,
        [soluong_ton_sanpham] Int NULL,
        [hsd_sanpham] Date NULL,
        [giavon_sanpham] Int NULL,
        [giaban_dichvu] BigInt NULL,
        [phantram_lamdichvu] Int NULL,
        [phantram_chotsale_dichvu] Int NULL,
        [donvitinh_sp] NVarChar(MAX) NULL,
        [hienthi] Bit NULL,
        [id_nganh] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.web_post_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.web_post_table', 'id_category') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [id_category] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'name') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [name] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'name_en') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [name_en] VarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'content_post') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [content_post] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'description') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [description] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'image') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [image] VarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'ngaytao') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [ngaytao] DateTime NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'nguoitao') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [nguoitao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'bin') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [bin] Bit NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'noibat') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [noibat] Bit NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'phanloai') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [phanloai] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'giaban_sanpham') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [giaban_sanpham] BigInt NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'phantram_chotsale_sanpham') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [phantram_chotsale_sanpham] Int NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'soluong_ton_sanpham') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [soluong_ton_sanpham] Int NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'hsd_sanpham') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [hsd_sanpham] Date NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'giavon_sanpham') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [giavon_sanpham] Int NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'giaban_dichvu') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [giaban_dichvu] BigInt NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'phantram_lamdichvu') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [phantram_lamdichvu] Int NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'phantram_chotsale_dichvu') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [phantram_chotsale_dichvu] Int NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'donvitinh_sp') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [donvitinh_sp] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'hienthi') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [hienthi] Bit NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'id_nganh') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [id_nganh] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'thoiluong_dichvu_phut') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [thoiluong_dichvu_phut] Int NULL;
END

IF COL_LENGTH('dbo.web_post_table', 'id_baiviet') IS NULL
BEGIN
    ALTER TABLE dbo.web_post_table ADD [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.bspa_caidatchung_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.bspa_caidatchung_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [chitieu_doanhso_dichvu] BigInt NULL,
        [chitieu_doanhso_mypham] BigInt NULL,
        [user_parent] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.bspa_caidatchung_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_caidatchung_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.bspa_caidatchung_table', 'chitieu_doanhso_dichvu') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_caidatchung_table ADD [chitieu_doanhso_dichvu] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_caidatchung_table', 'chitieu_doanhso_mypham') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_caidatchung_table ADD [chitieu_doanhso_mypham] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_caidatchung_table', 'user_parent') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_caidatchung_table ADD [user_parent] NVarChar(MAX) NULL;
END

IF OBJECT_ID('dbo.bspa_chamcong_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.bspa_chamcong_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [user_parent] NVarChar(MAX) NULL,
        [username] NVarChar(MAX) NULL,
        [ngay] Date NULL,
        [chamcong] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.bspa_chamcong_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_chamcong_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.bspa_chamcong_table', 'user_parent') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_chamcong_table ADD [user_parent] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_chamcong_table', 'username') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_chamcong_table ADD [username] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_chamcong_table', 'ngay') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_chamcong_table ADD [ngay] Date NULL;
END

IF COL_LENGTH('dbo.bspa_chamcong_table', 'chamcong') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_chamcong_table ADD [chamcong] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_chamcong_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_chamcong_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.bspa_chiphi_codinh_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.bspa_chiphi_codinh_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [user_parent] NVarChar(MAX) NULL,
        [tenchiphi] NVarChar(MAX) NULL,
        [sotien] BigInt NULL,
        [apdung] DateTime NULL
    );
END

IF COL_LENGTH('dbo.bspa_chiphi_codinh_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_chiphi_codinh_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.bspa_chiphi_codinh_table', 'user_parent') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_chiphi_codinh_table ADD [user_parent] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_chiphi_codinh_table', 'tenchiphi') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_chiphi_codinh_table ADD [tenchiphi] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_chiphi_codinh_table', 'sotien') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_chiphi_codinh_table ADD [sotien] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_chiphi_codinh_table', 'apdung') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_chiphi_codinh_table ADD [apdung] DateTime NULL;
END

IF OBJECT_ID('dbo.bspa_data_khachhang_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.bspa_data_khachhang_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [sdt] NVarChar(MAX) NULL,
        [matkhau] NVarChar(MAX) NULL,
        [tenkhachhang] NVarChar(MAX) NULL,
        [ngaytao] DateTime NULL,
        [nguoitao] NVarChar(MAX) NULL,
        [ngaysinh] Date NULL,
        [user_parent] NVarChar(MAX) NULL,
        [anhdaidien] NVarChar(MAX) NULL,
        [diachi] NVarChar(MAX) NULL,
        [magioithieu] NVarChar(MAX) NULL,
        [nguoichamsoc] NVarChar(MAX) NULL,
        [nhomkhachhang] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL,
        [sodiem_e_aha] Float NULL,
        [vnd_tu_e_aha] Float NULL,
        [capbac] NVarChar(MAX) NULL,
        [solan_lencap] Int NULL,
        [email] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.bspa_data_khachhang_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_data_khachhang_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.bspa_data_khachhang_table', 'sdt') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_data_khachhang_table ADD [sdt] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_data_khachhang_table', 'matkhau') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_data_khachhang_table ADD [matkhau] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_data_khachhang_table', 'tenkhachhang') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_data_khachhang_table ADD [tenkhachhang] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_data_khachhang_table', 'ngaytao') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_data_khachhang_table ADD [ngaytao] DateTime NULL;
END

IF COL_LENGTH('dbo.bspa_data_khachhang_table', 'nguoitao') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_data_khachhang_table ADD [nguoitao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_data_khachhang_table', 'ngaysinh') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_data_khachhang_table ADD [ngaysinh] Date NULL;
END

IF COL_LENGTH('dbo.bspa_data_khachhang_table', 'user_parent') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_data_khachhang_table ADD [user_parent] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_data_khachhang_table', 'anhdaidien') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_data_khachhang_table ADD [anhdaidien] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_data_khachhang_table', 'diachi') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_data_khachhang_table ADD [diachi] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_data_khachhang_table', 'magioithieu') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_data_khachhang_table ADD [magioithieu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_data_khachhang_table', 'nguoichamsoc') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_data_khachhang_table ADD [nguoichamsoc] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_data_khachhang_table', 'nhomkhachhang') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_data_khachhang_table ADD [nhomkhachhang] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_data_khachhang_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_data_khachhang_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF COL_LENGTH('dbo.bspa_data_khachhang_table', 'sodiem_e_aha') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_data_khachhang_table ADD [sodiem_e_aha] Float NULL;
END

IF COL_LENGTH('dbo.bspa_data_khachhang_table', 'vnd_tu_e_aha') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_data_khachhang_table ADD [vnd_tu_e_aha] Float NULL;
END

IF COL_LENGTH('dbo.bspa_data_khachhang_table', 'capbac') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_data_khachhang_table ADD [capbac] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_data_khachhang_table', 'solan_lencap') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_data_khachhang_table ADD [solan_lencap] Int NULL;
END

IF COL_LENGTH('dbo.bspa_data_khachhang_table', 'email') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_data_khachhang_table ADD [email] NVarChar(MAX) NULL;
END

IF OBJECT_ID('dbo.bspa_datlich_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.bspa_datlich_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [tenkhachhang] NVarChar(MAX) NULL,
        [sdt] NVarChar(MAX) NULL,
        [ngaydat] DateTime NULL,
        [nguoitao] NVarChar(MAX) NULL,
        [dichvu] NVarChar(MAX) NULL,
        [tendichvu_taithoidiemnay] NVarChar(MAX) NULL,
        [nhanvien_thuchien] NVarChar(MAX) NULL,
        [ghichu] NVarChar(MAX) NULL,
        [trangthai] NVarChar(MAX) NULL,
        [ngaytao] DateTime NULL,
        [nguongoc] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.bspa_datlich_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_datlich_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.bspa_datlich_table', 'tenkhachhang') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_datlich_table ADD [tenkhachhang] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_datlich_table', 'sdt') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_datlich_table ADD [sdt] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_datlich_table', 'ngaydat') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_datlich_table ADD [ngaydat] DateTime NULL;
END

IF COL_LENGTH('dbo.bspa_datlich_table', 'nguoitao') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_datlich_table ADD [nguoitao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_datlich_table', 'dichvu') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_datlich_table ADD [dichvu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_datlich_table', 'tendichvu_taithoidiemnay') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_datlich_table ADD [tendichvu_taithoidiemnay] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_datlich_table', 'nhanvien_thuchien') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_datlich_table ADD [nhanvien_thuchien] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_datlich_table', 'ghichu') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_datlich_table ADD [ghichu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_datlich_table', 'trangthai') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_datlich_table ADD [trangthai] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_datlich_table', 'ngaytao') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_datlich_table ADD [ngaytao] DateTime NULL;
END

IF COL_LENGTH('dbo.bspa_datlich_table', 'nguongoc') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_datlich_table ADD [nguongoc] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_datlich_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_datlich_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF COL_LENGTH('dbo.bspa_datlich_table', 'thoiluong_dichvu_phut') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_datlich_table ADD [thoiluong_dichvu_phut] Int NULL;
END

IF COL_LENGTH('dbo.bspa_datlich_table', 'ngayketthucdukien') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_datlich_table ADD [ngayketthucdukien] DateTime NULL;
END

IF OBJECT_ID('dbo.bspa_hinhanhthuchi_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.bspa_hinhanhthuchi_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [user_parent] NVarChar(MAX) NULL,
        [id_thuchi] NVarChar(MAX) NULL,
        [hinhanh] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL,
        [id_nganh] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.bspa_hinhanhthuchi_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hinhanhthuchi_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.bspa_hinhanhthuchi_table', 'user_parent') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hinhanhthuchi_table ADD [user_parent] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hinhanhthuchi_table', 'id_thuchi') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hinhanhthuchi_table ADD [id_thuchi] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hinhanhthuchi_table', 'hinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hinhanhthuchi_table ADD [hinhanh] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hinhanhthuchi_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hinhanhthuchi_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF COL_LENGTH('dbo.bspa_hinhanhthuchi_table', 'id_nganh') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hinhanhthuchi_table ADD [id_nganh] NVarChar(MAX) NULL;
END

IF OBJECT_ID('dbo.bspa_hoadon_chitiet_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.bspa_hoadon_chitiet_table
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
        [danhgia_5sao_dv] NVarChar(MAX) NULL,
        [ngaytao] DateTime NULL,
        [nguoitao] NVarChar(MAX) NULL,
        [solo] NVarChar(MAX) NULL,
        [id_thedichvu] NVarChar(MAX) NULL,
        [gia_hienthi_khidung_thedv] BigInt NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL,
        [id_nganh] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'user_parent') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [user_parent] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'id_hoadon') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [id_hoadon] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'id_dvsp') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [id_dvsp] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'ten_dvsp_taithoidiemnay') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [ten_dvsp_taithoidiemnay] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'gia_dvsp_taithoidiemnay') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [gia_dvsp_taithoidiemnay] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'soluong') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [soluong] Int NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'thanhtien') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [thanhtien] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'chietkhau') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [chietkhau] Int NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'tongtien_ck_dvsp') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [tongtien_ck_dvsp] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'tongsauchietkhau') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [tongsauchietkhau] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'nguoichot_dvsp') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [nguoichot_dvsp] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'phantram_chotsale_dvsp') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [phantram_chotsale_dvsp] Int NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'tongtien_chotsale_dvsp') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [tongtien_chotsale_dvsp] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'nguoilam_dichvu') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [nguoilam_dichvu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'phantram_lamdichvu') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [phantram_lamdichvu] Int NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'tongtien_lamdichvu') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [tongtien_lamdichvu] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'kyhieu') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [kyhieu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'tennguoichot_hientai') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [tennguoichot_hientai] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'tennguoilam_hientai') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [tennguoilam_hientai] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'hinhanh_hientai') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [hinhanh_hientai] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'danhgia_nhanvien_lamdichvu') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [danhgia_nhanvien_lamdichvu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'danhgia_5sao_dv') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [danhgia_5sao_dv] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'ngaytao') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [ngaytao] DateTime NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'nguoitao') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [nguoitao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'solo') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [solo] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'id_thedichvu') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [id_thedichvu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'gia_hienthi_khidung_thedv') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [gia_hienthi_khidung_thedv] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_chitiet_table', 'id_nganh') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_chitiet_table ADD [id_nganh] NVarChar(MAX) NULL;
END

IF OBJECT_ID('dbo.bspa_hoadon_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.bspa_hoadon_table
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
        [album] NVarChar(MAX) NULL,
        [dichvu_hay_sanpham] NVarChar(MAX) NULL,
        [sl_dichvu] Int NULL,
        [sl_sanpham] Int NULL,
        [ds_dichvu] BigInt NULL,
        [ds_sanpham] BigInt NULL,
        [sauck_dichvu] BigInt NULL,
        [sauck_sanpham] BigInt NULL,
        [km1_ghichu] NVarChar(MAX) NULL,
        [id_guide] UniqueIdentifier NULL,
        [nguongoc] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL,
        [id_nganh] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'user_parent') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [user_parent] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'ngaytao') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [ngaytao] DateTime NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'nguoitao') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [nguoitao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'tongtien') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [tongtien] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'chietkhau') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [chietkhau] Int NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'tongtien_ck_hoadon') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [tongtien_ck_hoadon] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'tongsauchietkhau') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [tongsauchietkhau] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'tenkhachhang') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [tenkhachhang] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'sdt') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [sdt] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'diachi') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [diachi] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'ghichu') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [ghichu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'sotien_dathanhtoan') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [sotien_dathanhtoan] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'thanhtoan_tienmat') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [thanhtoan_tienmat] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'thanhtoan_chuyenkhoan') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [thanhtoan_chuyenkhoan] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'thanhtoan_quetthe') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [thanhtoan_quetthe] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'sotien_conlai') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [sotien_conlai] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'album') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [album] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'dichvu_hay_sanpham') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [dichvu_hay_sanpham] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'sl_dichvu') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [sl_dichvu] Int NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'sl_sanpham') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [sl_sanpham] Int NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'ds_dichvu') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [ds_dichvu] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'ds_sanpham') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [ds_sanpham] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'sauck_dichvu') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [sauck_dichvu] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'sauck_sanpham') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [sauck_sanpham] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'km1_ghichu') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [km1_ghichu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'id_guide') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [id_guide] UniqueIdentifier NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'nguongoc') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [nguongoc] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF COL_LENGTH('dbo.bspa_hoadon_table', 'id_nganh') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_hoadon_table ADD [id_nganh] NVarChar(MAX) NULL;
END

IF OBJECT_ID('dbo.bspa_lichsu_thanhtoan_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.bspa_lichsu_thanhtoan_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [user_parent] NVarChar(MAX) NULL,
        [id_hoadon] NVarChar(MAX) NULL,
        [hinhthuc_thanhtoan] NVarChar(MAX) NULL,
        [sotienthanhtoan] BigInt NULL,
        [thoigian] DateTime NULL,
        [nguoithanhtoan] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL,
        [id_nganh] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.bspa_lichsu_thanhtoan_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_lichsu_thanhtoan_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.bspa_lichsu_thanhtoan_table', 'user_parent') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_lichsu_thanhtoan_table ADD [user_parent] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_lichsu_thanhtoan_table', 'id_hoadon') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_lichsu_thanhtoan_table ADD [id_hoadon] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_lichsu_thanhtoan_table', 'hinhthuc_thanhtoan') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_lichsu_thanhtoan_table ADD [hinhthuc_thanhtoan] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_lichsu_thanhtoan_table', 'sotienthanhtoan') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_lichsu_thanhtoan_table ADD [sotienthanhtoan] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_lichsu_thanhtoan_table', 'thoigian') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_lichsu_thanhtoan_table ADD [thoigian] DateTime NULL;
END

IF COL_LENGTH('dbo.bspa_lichsu_thanhtoan_table', 'nguoithanhtoan') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_lichsu_thanhtoan_table ADD [nguoithanhtoan] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_lichsu_thanhtoan_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_lichsu_thanhtoan_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF COL_LENGTH('dbo.bspa_lichsu_thanhtoan_table', 'id_nganh') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_lichsu_thanhtoan_table ADD [id_nganh] NVarChar(MAX) NULL;
END

IF OBJECT_ID('dbo.bspa_nhomthuchi_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.bspa_nhomthuchi_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [user_parent] NVarChar(MAX) NULL,
        [tennhom] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL,
        [id_nganh] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.bspa_nhomthuchi_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_nhomthuchi_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.bspa_nhomthuchi_table', 'user_parent') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_nhomthuchi_table ADD [user_parent] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_nhomthuchi_table', 'tennhom') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_nhomthuchi_table ADD [tennhom] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_nhomthuchi_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_nhomthuchi_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF COL_LENGTH('dbo.bspa_nhomthuchi_table', 'id_nganh') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_nhomthuchi_table ADD [id_nganh] NVarChar(MAX) NULL;
END

IF OBJECT_ID('dbo.bspa_thuchi_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.bspa_thuchi_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [user_parent] NVarChar(MAX) NULL,
        [id_nhomthuchi] NVarChar(MAX) NULL,
        [nguoilapphieu] NVarChar(MAX) NULL,
        [thuchi] NVarChar(MAX) NULL,
        [ngay] DateTime NULL,
        [noidung] NVarChar(MAX) NULL,
        [sotien] BigInt NULL,
        [nguoinhantien] NVarChar(MAX) NULL,
        [duyet_phieuchi] NVarChar(MAX) NULL,
        [chophep_duyetvahuy_phieuchi] Bit NULL,
        [nguoihuy_duyet_phieuchi] NVarChar(MAX) NULL,
        [thoigian_huyduyet_phieuchi] DateTime NULL,
        [tudong_tu_hethong] Bit NULL,
        [nguon_tudong] NVarChar(MAX) NULL,
        [url_admin_lienket] NVarChar(MAX) NULL,
        [tudong_tu_hoadon] Bit NULL,
        [id_hoadon_lienket] NVarChar(MAX) NULL,
        [id_lichsu_thanhtoan_lienket] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL,
        [id_nganh] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'user_parent') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [user_parent] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'id_nhomthuchi') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [id_nhomthuchi] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'nguoilapphieu') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [nguoilapphieu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'thuchi') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [thuchi] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'ngay') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [ngay] DateTime NULL;
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'noidung') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [noidung] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'sotien') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [sotien] BigInt NULL;
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'nguoinhantien') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [nguoinhantien] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'duyet_phieuchi') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [duyet_phieuchi] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'chophep_duyetvahuy_phieuchi') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [chophep_duyetvahuy_phieuchi] Bit NULL;
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'nguoihuy_duyet_phieuchi') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [nguoihuy_duyet_phieuchi] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'thoigian_huyduyet_phieuchi') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [thoigian_huyduyet_phieuchi] DateTime NULL;
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'tudong_tu_hethong') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [tudong_tu_hethong] Bit NULL;
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'nguon_tudong') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [nguon_tudong] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'url_admin_lienket') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [url_admin_lienket] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'tudong_tu_hoadon') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [tudong_tu_hoadon] Bit NULL;
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'id_hoadon_lienket') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [id_hoadon_lienket] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'id_lichsu_thanhtoan_lienket') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [id_lichsu_thanhtoan_lienket] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF COL_LENGTH('dbo.bspa_thuchi_table', 'id_nganh') IS NULL
BEGIN
    ALTER TABLE dbo.bspa_thuchi_table ADD [id_nganh] NVarChar(MAX) NULL;
END

IF OBJECT_ID('dbo.chinhanh_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.chinhanh_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [ten] NVarChar(MAX) NULL,
        [taikhoan_quantri] NVarChar(MAX) NULL,
        [logo_footer] NVarChar(MAX) NULL,
        [slogan] NVarChar(MAX) NULL,
        [diachi] NVarChar(MAX) NULL,
        [email] NVarChar(MAX) NULL,
        [sdt] NVarChar(MAX) NULL,
        [logo_hoadon] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.chinhanh_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.chinhanh_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.chinhanh_table', 'ten') IS NULL
BEGIN
    ALTER TABLE dbo.chinhanh_table ADD [ten] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.chinhanh_table', 'taikhoan_quantri') IS NULL
BEGIN
    ALTER TABLE dbo.chinhanh_table ADD [taikhoan_quantri] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.chinhanh_table', 'logo_footer') IS NULL
BEGIN
    ALTER TABLE dbo.chinhanh_table ADD [logo_footer] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.chinhanh_table', 'slogan') IS NULL
BEGIN
    ALTER TABLE dbo.chinhanh_table ADD [slogan] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.chinhanh_table', 'diachi') IS NULL
BEGIN
    ALTER TABLE dbo.chinhanh_table ADD [diachi] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.chinhanh_table', 'email') IS NULL
BEGIN
    ALTER TABLE dbo.chinhanh_table ADD [email] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.chinhanh_table', 'sdt') IS NULL
BEGIN
    ALTER TABLE dbo.chinhanh_table ADD [sdt] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.chinhanh_table', 'logo_hoadon') IS NULL
BEGIN
    ALTER TABLE dbo.chinhanh_table ADD [logo_hoadon] NVarChar(MAX) NULL;
END

IF OBJECT_ID('dbo.config_baotri_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.config_baotri_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [baotri_trangthai] Bit NULL,
        [baotri_thoigian_batdau] DateTime NULL,
        [baotri_thoigian_ketthuc] DateTime NULL,
        [ghichu] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.config_baotri_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.config_baotri_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.config_baotri_table', 'baotri_trangthai') IS NULL
BEGIN
    ALTER TABLE dbo.config_baotri_table ADD [baotri_trangthai] Bit NULL;
END

IF COL_LENGTH('dbo.config_baotri_table', 'baotri_thoigian_batdau') IS NULL
BEGIN
    ALTER TABLE dbo.config_baotri_table ADD [baotri_thoigian_batdau] DateTime NULL;
END

IF COL_LENGTH('dbo.config_baotri_table', 'baotri_thoigian_ketthuc') IS NULL
BEGIN
    ALTER TABLE dbo.config_baotri_table ADD [baotri_thoigian_ketthuc] DateTime NULL;
END

IF COL_LENGTH('dbo.config_baotri_table', 'ghichu') IS NULL
BEGIN
    ALTER TABLE dbo.config_baotri_table ADD [ghichu] NVarChar(MAX) NULL;
END

IF OBJECT_ID('dbo.config_lienket_chiase_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.config_lienket_chiase_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [title] NVarChar(MAX) NULL,
        [description] NVarChar(MAX) NULL,
        [image] VarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.config_lienket_chiase_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.config_lienket_chiase_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.config_lienket_chiase_table', 'title') IS NULL
BEGIN
    ALTER TABLE dbo.config_lienket_chiase_table ADD [title] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_lienket_chiase_table', 'description') IS NULL
BEGIN
    ALTER TABLE dbo.config_lienket_chiase_table ADD [description] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_lienket_chiase_table', 'image') IS NULL
BEGIN
    ALTER TABLE dbo.config_lienket_chiase_table ADD [image] VarChar(MAX) NULL;
END

IF OBJECT_ID('dbo.config_nhungma_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.config_nhungma_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [nhungma_head] NVarChar(MAX) NULL,
        [nhungma_body1] NVarChar(MAX) NULL,
        [nhungma_body2] NVarChar(MAX) NULL,
        [ghichu] NVarChar(MAX) NULL,
        [nhungma_fanpage] NVarChar(MAX) NULL,
        [nhungma_googlemaps] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.config_nhungma_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.config_nhungma_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.config_nhungma_table', 'nhungma_head') IS NULL
BEGIN
    ALTER TABLE dbo.config_nhungma_table ADD [nhungma_head] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_nhungma_table', 'nhungma_body1') IS NULL
BEGIN
    ALTER TABLE dbo.config_nhungma_table ADD [nhungma_body1] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_nhungma_table', 'nhungma_body2') IS NULL
BEGIN
    ALTER TABLE dbo.config_nhungma_table ADD [nhungma_body2] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_nhungma_table', 'ghichu') IS NULL
BEGIN
    ALTER TABLE dbo.config_nhungma_table ADD [ghichu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_nhungma_table', 'nhungma_fanpage') IS NULL
BEGIN
    ALTER TABLE dbo.config_nhungma_table ADD [nhungma_fanpage] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_nhungma_table', 'nhungma_googlemaps') IS NULL
BEGIN
    ALTER TABLE dbo.config_nhungma_table ADD [nhungma_googlemaps] NVarChar(MAX) NULL;
END

IF OBJECT_ID('dbo.config_social_media_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.config_social_media_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [facebook] NVarChar(MAX) NULL,
        [zalo] NVarChar(MAX) NULL,
        [youtube] NVarChar(MAX) NULL,
        [instagram] NVarChar(MAX) NULL,
        [twitter] NVarChar(MAX) NULL,
        [tiktok] NVarChar(MAX) NULL,
        [wechat] NVarChar(MAX) NULL,
        [linkedin] NVarChar(MAX) NULL,
        [whatsapp] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.config_social_media_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.config_social_media_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.config_social_media_table', 'facebook') IS NULL
BEGIN
    ALTER TABLE dbo.config_social_media_table ADD [facebook] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_social_media_table', 'zalo') IS NULL
BEGIN
    ALTER TABLE dbo.config_social_media_table ADD [zalo] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_social_media_table', 'youtube') IS NULL
BEGIN
    ALTER TABLE dbo.config_social_media_table ADD [youtube] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_social_media_table', 'instagram') IS NULL
BEGIN
    ALTER TABLE dbo.config_social_media_table ADD [instagram] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_social_media_table', 'twitter') IS NULL
BEGIN
    ALTER TABLE dbo.config_social_media_table ADD [twitter] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_social_media_table', 'tiktok') IS NULL
BEGIN
    ALTER TABLE dbo.config_social_media_table ADD [tiktok] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_social_media_table', 'wechat') IS NULL
BEGIN
    ALTER TABLE dbo.config_social_media_table ADD [wechat] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_social_media_table', 'linkedin') IS NULL
BEGIN
    ALTER TABLE dbo.config_social_media_table ADD [linkedin] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_social_media_table', 'whatsapp') IS NULL
BEGIN
    ALTER TABLE dbo.config_social_media_table ADD [whatsapp] NVarChar(MAX) NULL;
END

IF OBJECT_ID('dbo.config_thongtin_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.config_thongtin_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [icon] NVarChar(MAX) NULL,
        [apple_touch_icon] NVarChar(MAX) NULL,
        [logo] NVarChar(MAX) NULL,
        [logo1] NVarChar(MAX) NULL,
        [tencongty] NVarChar(MAX) NULL,
        [slogan] NVarChar(MAX) NULL,
        [diachi] NVarChar(MAX) NULL,
        [link_googlemap] NVarChar(MAX) NULL,
        [hotline] NVarChar(MAX) NULL,
        [email] NVarChar(MAX) NULL,
        [masothue] NVarChar(MAX) NULL,
        [zalo] NVarChar(MAX) NULL,
        [logo_in_hoadon] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.config_thongtin_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.config_thongtin_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.config_thongtin_table', 'icon') IS NULL
BEGIN
    ALTER TABLE dbo.config_thongtin_table ADD [icon] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_thongtin_table', 'apple_touch_icon') IS NULL
BEGIN
    ALTER TABLE dbo.config_thongtin_table ADD [apple_touch_icon] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_thongtin_table', 'logo') IS NULL
BEGIN
    ALTER TABLE dbo.config_thongtin_table ADD [logo] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_thongtin_table', 'logo1') IS NULL
BEGIN
    ALTER TABLE dbo.config_thongtin_table ADD [logo1] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_thongtin_table', 'tencongty') IS NULL
BEGIN
    ALTER TABLE dbo.config_thongtin_table ADD [tencongty] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_thongtin_table', 'slogan') IS NULL
BEGIN
    ALTER TABLE dbo.config_thongtin_table ADD [slogan] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_thongtin_table', 'diachi') IS NULL
BEGIN
    ALTER TABLE dbo.config_thongtin_table ADD [diachi] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_thongtin_table', 'link_googlemap') IS NULL
BEGIN
    ALTER TABLE dbo.config_thongtin_table ADD [link_googlemap] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_thongtin_table', 'hotline') IS NULL
BEGIN
    ALTER TABLE dbo.config_thongtin_table ADD [hotline] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_thongtin_table', 'email') IS NULL
BEGIN
    ALTER TABLE dbo.config_thongtin_table ADD [email] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_thongtin_table', 'masothue') IS NULL
BEGIN
    ALTER TABLE dbo.config_thongtin_table ADD [masothue] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_thongtin_table', 'zalo') IS NULL
BEGIN
    ALTER TABLE dbo.config_thongtin_table ADD [zalo] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.config_thongtin_table', 'logo_in_hoadon') IS NULL
BEGIN
    ALTER TABLE dbo.config_thongtin_table ADD [logo_in_hoadon] NVarChar(MAX) NULL;
END

IF OBJECT_ID('dbo.danhsach_vattu_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.danhsach_vattu_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [tenvattu] NVarChar(MAX) NULL,
        [image] NVarChar(MAX) NULL,
        [ngaytao] DateTime NULL,
        [nguoitao] NVarChar(MAX) NULL,
        [id_nhom] NVarChar(MAX) NULL,
        [ghichu] NVarChar(MAX) NULL,
        [donvitinh_sp] NVarChar(MAX) NULL,
        [giaban] BigInt NULL,
        [gianhap] BigInt NULL,
        [tinhtrang] NVarChar(MAX) NULL,
        [vitriphongban] NVarChar(MAX) NULL,
        [ncc] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.danhsach_vattu_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.danhsach_vattu_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.danhsach_vattu_table', 'tenvattu') IS NULL
BEGIN
    ALTER TABLE dbo.danhsach_vattu_table ADD [tenvattu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.danhsach_vattu_table', 'image') IS NULL
BEGIN
    ALTER TABLE dbo.danhsach_vattu_table ADD [image] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.danhsach_vattu_table', 'ngaytao') IS NULL
BEGIN
    ALTER TABLE dbo.danhsach_vattu_table ADD [ngaytao] DateTime NULL;
END

IF COL_LENGTH('dbo.danhsach_vattu_table', 'nguoitao') IS NULL
BEGIN
    ALTER TABLE dbo.danhsach_vattu_table ADD [nguoitao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.danhsach_vattu_table', 'id_nhom') IS NULL
BEGIN
    ALTER TABLE dbo.danhsach_vattu_table ADD [id_nhom] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.danhsach_vattu_table', 'ghichu') IS NULL
BEGIN
    ALTER TABLE dbo.danhsach_vattu_table ADD [ghichu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.danhsach_vattu_table', 'donvitinh_sp') IS NULL
BEGIN
    ALTER TABLE dbo.danhsach_vattu_table ADD [donvitinh_sp] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.danhsach_vattu_table', 'giaban') IS NULL
BEGIN
    ALTER TABLE dbo.danhsach_vattu_table ADD [giaban] BigInt NULL;
END

IF COL_LENGTH('dbo.danhsach_vattu_table', 'gianhap') IS NULL
BEGIN
    ALTER TABLE dbo.danhsach_vattu_table ADD [gianhap] BigInt NULL;
END

IF COL_LENGTH('dbo.danhsach_vattu_table', 'tinhtrang') IS NULL
BEGIN
    ALTER TABLE dbo.danhsach_vattu_table ADD [tinhtrang] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.danhsach_vattu_table', 'vitriphongban') IS NULL
BEGIN
    ALTER TABLE dbo.danhsach_vattu_table ADD [vitriphongban] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.danhsach_vattu_table', 'ncc') IS NULL
BEGIN
    ALTER TABLE dbo.danhsach_vattu_table ADD [ncc] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.danhsach_vattu_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.danhsach_vattu_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF COL_LENGTH('dbo.data_yeucau_tuvan_table', 'noidung1') IS NULL
BEGIN
    ALTER TABLE dbo.data_yeucau_tuvan_table ADD noidung1 NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.data_yeucau_tuvan_table', 'noidung2') IS NULL
BEGIN
    ALTER TABLE dbo.data_yeucau_tuvan_table ADD noidung2 NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.data_yeucau_tuvan_table', 'noidung3') IS NULL
BEGIN
    ALTER TABLE dbo.data_yeucau_tuvan_table ADD noidung3 NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.data_yeucau_tuvan_table', 'noidung4') IS NULL
BEGIN
    ALTER TABLE dbo.data_yeucau_tuvan_table ADD noidung4 NVarChar(MAX) NULL;
END

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
        [danhgia_5sao_dv] NVarChar(MAX) NULL,
        [ngaytao] DateTime NULL,
        [nguoitao] NVarChar(MAX) NULL,
        [nsx] Date NULL,
        [hsd] Date NULL,
        [solo] NVarChar(MAX) NULL,
        [sl_daban] Int NULL,
        [sl_conlai] Int NULL,
        [dvt] NVarChar(MAX) NULL,
        [image] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'user_parent') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [user_parent] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'id_hoadon') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [id_hoadon] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'id_dvsp') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [id_dvsp] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'ten_dvsp_taithoidiemnay') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [ten_dvsp_taithoidiemnay] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'gia_dvsp_taithoidiemnay') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [gia_dvsp_taithoidiemnay] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'soluong') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [soluong] Int NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'thanhtien') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [thanhtien] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'chietkhau') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [chietkhau] Int NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'tongtien_ck_dvsp') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [tongtien_ck_dvsp] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'tongsauchietkhau') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [tongsauchietkhau] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'nguoichot_dvsp') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [nguoichot_dvsp] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'phantram_chotsale_dvsp') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [phantram_chotsale_dvsp] Int NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'tongtien_chotsale_dvsp') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [tongtien_chotsale_dvsp] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'nguoilam_dichvu') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [nguoilam_dichvu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'phantram_lamdichvu') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [phantram_lamdichvu] Int NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'tongtien_lamdichvu') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [tongtien_lamdichvu] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'kyhieu') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [kyhieu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'tennguoichot_hientai') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [tennguoichot_hientai] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'tennguoilam_hientai') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [tennguoilam_hientai] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'hinhanh_hientai') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [hinhanh_hientai] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'danhgia_nhanvien_lamdichvu') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [danhgia_nhanvien_lamdichvu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'danhgia_5sao_dv') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [danhgia_5sao_dv] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'ngaytao') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [ngaytao] DateTime NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'nguoitao') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [nguoitao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'nsx') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [nsx] Date NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'hsd') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [hsd] Date NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'solo') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [solo] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'sl_daban') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [sl_daban] Int NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'sl_conlai') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [sl_conlai] Int NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'dvt') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [dvt] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'image') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [image] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_chitiet_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

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
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.donnhap_vattu_lichsu_thanhtoan_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_lichsu_thanhtoan_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.donnhap_vattu_lichsu_thanhtoan_table', 'user_parent') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_lichsu_thanhtoan_table ADD [user_parent] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_lichsu_thanhtoan_table', 'id_hoadon') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_lichsu_thanhtoan_table ADD [id_hoadon] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_lichsu_thanhtoan_table', 'hinhthuc_thanhtoan') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_lichsu_thanhtoan_table ADD [hinhthuc_thanhtoan] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_lichsu_thanhtoan_table', 'sotienthanhtoan') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_lichsu_thanhtoan_table ADD [sotienthanhtoan] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_lichsu_thanhtoan_table', 'thoigian') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_lichsu_thanhtoan_table ADD [thoigian] DateTime NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_lichsu_thanhtoan_table', 'nguoithanhtoan') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_lichsu_thanhtoan_table ADD [nguoithanhtoan] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_lichsu_thanhtoan_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_lichsu_thanhtoan_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

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
        [album] NVarChar(MAX) NULL,
        [dichvu_hay_sanpham] NVarChar(MAX) NULL,
        [sl_dichvu] Int NULL,
        [sl_sanpham] Int NULL,
        [ds_dichvu] BigInt NULL,
        [ds_sanpham] BigInt NULL,
        [sauck_dichvu] BigInt NULL,
        [sauck_sanpham] BigInt NULL,
        [km1_ghichu] NVarChar(MAX) NULL,
        [id_guide] UniqueIdentifier NULL,
        [nguongoc] NVarChar(MAX) NULL,
        [nhacungcap] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'user_parent') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [user_parent] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'ngaytao') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [ngaytao] DateTime NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'nguoitao') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [nguoitao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'tongtien') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [tongtien] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'chietkhau') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [chietkhau] Int NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'tongtien_ck_hoadon') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [tongtien_ck_hoadon] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'tongsauchietkhau') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [tongsauchietkhau] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'tenkhachhang') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [tenkhachhang] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'sdt') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [sdt] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'diachi') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [diachi] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'ghichu') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [ghichu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'sotien_dathanhtoan') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [sotien_dathanhtoan] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'thanhtoan_tienmat') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [thanhtoan_tienmat] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'thanhtoan_chuyenkhoan') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [thanhtoan_chuyenkhoan] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'thanhtoan_quetthe') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [thanhtoan_quetthe] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'sotien_conlai') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [sotien_conlai] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'album') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [album] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'dichvu_hay_sanpham') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [dichvu_hay_sanpham] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'sl_dichvu') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [sl_dichvu] Int NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'sl_sanpham') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [sl_sanpham] Int NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'ds_dichvu') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [ds_dichvu] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'ds_sanpham') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [ds_sanpham] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'sauck_dichvu') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [sauck_dichvu] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'sauck_sanpham') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [sauck_sanpham] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'km1_ghichu') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [km1_ghichu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'id_guide') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [id_guide] UniqueIdentifier NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'nguongoc') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [nguongoc] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'nhacungcap') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [nhacungcap] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhap_vattu_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.donnhap_vattu_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.donnhaphang_chitiet_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.donnhaphang_chitiet_table
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
        [danhgia_5sao_dv] NVarChar(MAX) NULL,
        [ngaytao] DateTime NULL,
        [nguoitao] NVarChar(MAX) NULL,
        [nsx] Date NULL,
        [hsd] Date NULL,
        [solo] NVarChar(MAX) NULL,
        [sl_daban] Int NULL,
        [sl_conlai] Int NULL,
        [dvt] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'user_parent') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [user_parent] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'id_hoadon') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [id_hoadon] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'id_dvsp') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [id_dvsp] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'ten_dvsp_taithoidiemnay') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [ten_dvsp_taithoidiemnay] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'gia_dvsp_taithoidiemnay') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [gia_dvsp_taithoidiemnay] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'soluong') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [soluong] Int NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'thanhtien') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [thanhtien] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'chietkhau') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [chietkhau] Int NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'tongtien_ck_dvsp') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [tongtien_ck_dvsp] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'tongsauchietkhau') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [tongsauchietkhau] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'nguoichot_dvsp') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [nguoichot_dvsp] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'phantram_chotsale_dvsp') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [phantram_chotsale_dvsp] Int NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'tongtien_chotsale_dvsp') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [tongtien_chotsale_dvsp] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'nguoilam_dichvu') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [nguoilam_dichvu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'phantram_lamdichvu') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [phantram_lamdichvu] Int NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'tongtien_lamdichvu') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [tongtien_lamdichvu] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'kyhieu') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [kyhieu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'tennguoichot_hientai') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [tennguoichot_hientai] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'tennguoilam_hientai') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [tennguoilam_hientai] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'hinhanh_hientai') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [hinhanh_hientai] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'danhgia_nhanvien_lamdichvu') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [danhgia_nhanvien_lamdichvu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'danhgia_5sao_dv') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [danhgia_5sao_dv] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'ngaytao') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [ngaytao] DateTime NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'nguoitao') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [nguoitao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'nsx') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [nsx] Date NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'hsd') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [hsd] Date NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'solo') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [solo] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'sl_daban') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [sl_daban] Int NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'sl_conlai') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [sl_conlai] Int NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'dvt') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [dvt] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_chitiet_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_chitiet_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.donnhaphang_lichsu_thanhtoan_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.donnhaphang_lichsu_thanhtoan_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [user_parent] NVarChar(MAX) NULL,
        [id_hoadon] NVarChar(MAX) NULL,
        [hinhthuc_thanhtoan] NVarChar(MAX) NULL,
        [sotienthanhtoan] BigInt NULL,
        [thoigian] DateTime NULL,
        [nguoithanhtoan] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.donnhaphang_lichsu_thanhtoan_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_lichsu_thanhtoan_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.donnhaphang_lichsu_thanhtoan_table', 'user_parent') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_lichsu_thanhtoan_table ADD [user_parent] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_lichsu_thanhtoan_table', 'id_hoadon') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_lichsu_thanhtoan_table ADD [id_hoadon] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_lichsu_thanhtoan_table', 'hinhthuc_thanhtoan') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_lichsu_thanhtoan_table ADD [hinhthuc_thanhtoan] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_lichsu_thanhtoan_table', 'sotienthanhtoan') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_lichsu_thanhtoan_table ADD [sotienthanhtoan] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhaphang_lichsu_thanhtoan_table', 'thoigian') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_lichsu_thanhtoan_table ADD [thoigian] DateTime NULL;
END

IF COL_LENGTH('dbo.donnhaphang_lichsu_thanhtoan_table', 'nguoithanhtoan') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_lichsu_thanhtoan_table ADD [nguoithanhtoan] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_lichsu_thanhtoan_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_lichsu_thanhtoan_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.donnhaphang_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.donnhaphang_table
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
        [album] NVarChar(MAX) NULL,
        [dichvu_hay_sanpham] NVarChar(MAX) NULL,
        [sl_dichvu] Int NULL,
        [sl_sanpham] Int NULL,
        [ds_dichvu] BigInt NULL,
        [ds_sanpham] BigInt NULL,
        [sauck_dichvu] BigInt NULL,
        [sauck_sanpham] BigInt NULL,
        [km1_ghichu] NVarChar(MAX) NULL,
        [id_guide] UniqueIdentifier NULL,
        [nguongoc] NVarChar(MAX) NULL,
        [nhacungcap] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.donnhaphang_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'user_parent') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [user_parent] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'ngaytao') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [ngaytao] DateTime NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'nguoitao') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [nguoitao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'tongtien') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [tongtien] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'chietkhau') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [chietkhau] Int NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'tongtien_ck_hoadon') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [tongtien_ck_hoadon] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'tongsauchietkhau') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [tongsauchietkhau] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'tenkhachhang') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [tenkhachhang] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'sdt') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [sdt] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'diachi') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [diachi] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'ghichu') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [ghichu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'sotien_dathanhtoan') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [sotien_dathanhtoan] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'thanhtoan_tienmat') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [thanhtoan_tienmat] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'thanhtoan_chuyenkhoan') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [thanhtoan_chuyenkhoan] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'thanhtoan_quetthe') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [thanhtoan_quetthe] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'sotien_conlai') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [sotien_conlai] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'album') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [album] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'dichvu_hay_sanpham') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [dichvu_hay_sanpham] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'sl_dichvu') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [sl_dichvu] Int NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'sl_sanpham') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [sl_sanpham] Int NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'ds_dichvu') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [ds_dichvu] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'ds_sanpham') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [ds_sanpham] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'sauck_dichvu') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [sauck_dichvu] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'sauck_sanpham') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [sauck_sanpham] BigInt NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'km1_ghichu') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [km1_ghichu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'id_guide') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [id_guide] UniqueIdentifier NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'nguongoc') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [nguongoc] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'nhacungcap') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [nhacungcap] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donnhaphang_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.donnhaphang_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.donthuoc_khachhang_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.donthuoc_khachhang_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [ghichu] NVarChar(MAX) NULL,
        [sdt] NVarChar(MAX) NULL,
        [nguoitao] NVarChar(MAX) NULL,
        [ngaytao] DateTime NULL,
        [ngaytaikham] DateTime NULL,
        [noitaikham] NVarChar(MAX) NULL,
        [loidanbacsi] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.donthuoc_khachhang_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.donthuoc_khachhang_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.donthuoc_khachhang_table', 'ghichu') IS NULL
BEGIN
    ALTER TABLE dbo.donthuoc_khachhang_table ADD [ghichu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donthuoc_khachhang_table', 'sdt') IS NULL
BEGIN
    ALTER TABLE dbo.donthuoc_khachhang_table ADD [sdt] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donthuoc_khachhang_table', 'nguoitao') IS NULL
BEGIN
    ALTER TABLE dbo.donthuoc_khachhang_table ADD [nguoitao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donthuoc_khachhang_table', 'ngaytao') IS NULL
BEGIN
    ALTER TABLE dbo.donthuoc_khachhang_table ADD [ngaytao] DateTime NULL;
END

IF COL_LENGTH('dbo.donthuoc_khachhang_table', 'ngaytaikham') IS NULL
BEGIN
    ALTER TABLE dbo.donthuoc_khachhang_table ADD [ngaytaikham] DateTime NULL;
END

IF COL_LENGTH('dbo.donthuoc_khachhang_table', 'noitaikham') IS NULL
BEGIN
    ALTER TABLE dbo.donthuoc_khachhang_table ADD [noitaikham] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donthuoc_khachhang_table', 'loidanbacsi') IS NULL
BEGIN
    ALTER TABLE dbo.donthuoc_khachhang_table ADD [loidanbacsi] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.donthuoc_khachhang_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.donthuoc_khachhang_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.ghichu_khachhang_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ghichu_khachhang_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [ghichu] NVarChar(MAX) NULL,
        [sdt] NVarChar(MAX) NULL,
        [nguoitao] NVarChar(MAX) NULL,
        [ngaytao] DateTime NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.ghichu_khachhang_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.ghichu_khachhang_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.ghichu_khachhang_table', 'ghichu') IS NULL
BEGIN
    ALTER TABLE dbo.ghichu_khachhang_table ADD [ghichu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.ghichu_khachhang_table', 'sdt') IS NULL
BEGIN
    ALTER TABLE dbo.ghichu_khachhang_table ADD [sdt] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.ghichu_khachhang_table', 'nguoitao') IS NULL
BEGIN
    ALTER TABLE dbo.ghichu_khachhang_table ADD [nguoitao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.ghichu_khachhang_table', 'ngaytao') IS NULL
BEGIN
    ALTER TABLE dbo.ghichu_khachhang_table ADD [ngaytao] DateTime NULL;
END

IF COL_LENGTH('dbo.ghichu_khachhang_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.ghichu_khachhang_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.giangvien_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.giangvien_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [hoten] NVarChar(MAX) NULL,
        [hoten_khongdau] NVarChar(MAX) NULL,
        [anhdaidien] NVarChar(MAX) NULL,
        [ngaysinh] DateTime NULL,
        [email] NVarChar(MAX) NULL,
        [dienthoai] NVarChar(MAX) NULL,
        [zalo] NVarChar(MAX) NULL,
        [facebook] NVarChar(MAX) NULL,
        [chuyenmon] NVarChar(MAX) NULL,
        [sobuoi_lythuyet] Int NULL,
        [sobuoi_thuchanh] Int NULL,
        [sobuoi_trogiang] Int NULL,
        [goidaotao] NVarChar(MAX) NULL,
        [danhgiagiangvien] NVarChar(MAX) NULL,
        [nguoitao] NVarChar(MAX) NULL,
        [ngaytao] DateTime NULL,
        [trangthai] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.giangvien_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.giangvien_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.giangvien_table', 'hoten') IS NULL
BEGIN
    ALTER TABLE dbo.giangvien_table ADD [hoten] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.giangvien_table', 'hoten_khongdau') IS NULL
BEGIN
    ALTER TABLE dbo.giangvien_table ADD [hoten_khongdau] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.giangvien_table', 'anhdaidien') IS NULL
BEGIN
    ALTER TABLE dbo.giangvien_table ADD [anhdaidien] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.giangvien_table', 'ngaysinh') IS NULL
BEGIN
    ALTER TABLE dbo.giangvien_table ADD [ngaysinh] DateTime NULL;
END

IF COL_LENGTH('dbo.giangvien_table', 'email') IS NULL
BEGIN
    ALTER TABLE dbo.giangvien_table ADD [email] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.giangvien_table', 'dienthoai') IS NULL
BEGIN
    ALTER TABLE dbo.giangvien_table ADD [dienthoai] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.giangvien_table', 'zalo') IS NULL
BEGIN
    ALTER TABLE dbo.giangvien_table ADD [zalo] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.giangvien_table', 'facebook') IS NULL
BEGIN
    ALTER TABLE dbo.giangvien_table ADD [facebook] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.giangvien_table', 'chuyenmon') IS NULL
BEGIN
    ALTER TABLE dbo.giangvien_table ADD [chuyenmon] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.giangvien_table', 'sobuoi_lythuyet') IS NULL
BEGIN
    ALTER TABLE dbo.giangvien_table ADD [sobuoi_lythuyet] Int NULL;
END

IF COL_LENGTH('dbo.giangvien_table', 'sobuoi_thuchanh') IS NULL
BEGIN
    ALTER TABLE dbo.giangvien_table ADD [sobuoi_thuchanh] Int NULL;
END

IF COL_LENGTH('dbo.giangvien_table', 'sobuoi_trogiang') IS NULL
BEGIN
    ALTER TABLE dbo.giangvien_table ADD [sobuoi_trogiang] Int NULL;
END

IF COL_LENGTH('dbo.giangvien_table', 'goidaotao') IS NULL
BEGIN
    ALTER TABLE dbo.giangvien_table ADD [goidaotao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.giangvien_table', 'danhgiagiangvien') IS NULL
BEGIN
    ALTER TABLE dbo.giangvien_table ADD [danhgiagiangvien] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.giangvien_table', 'nguoitao') IS NULL
BEGIN
    ALTER TABLE dbo.giangvien_table ADD [nguoitao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.giangvien_table', 'ngaytao') IS NULL
BEGIN
    ALTER TABLE dbo.giangvien_table ADD [ngaytao] DateTime NULL;
END

IF COL_LENGTH('dbo.giangvien_table', 'trangthai') IS NULL
BEGIN
    ALTER TABLE dbo.giangvien_table ADD [trangthai] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.giangvien_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.giangvien_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.hethong_checkall_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.hethong_checkall_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [ngay] DateTime NULL,
        [user_parent] NVarChar(MAX) NULL,
        [update_chiphi_codinh] Bit NULL
    );
END

IF COL_LENGTH('dbo.hethong_checkall_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.hethong_checkall_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.hethong_checkall_table', 'ngay') IS NULL
BEGIN
    ALTER TABLE dbo.hethong_checkall_table ADD [ngay] DateTime NULL;
END

IF COL_LENGTH('dbo.hethong_checkall_table', 'user_parent') IS NULL
BEGIN
    ALTER TABLE dbo.hethong_checkall_table ADD [user_parent] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hethong_checkall_table', 'update_chiphi_codinh') IS NULL
BEGIN
    ALTER TABLE dbo.hethong_checkall_table ADD [update_chiphi_codinh] Bit NULL;
END

IF OBJECT_ID('dbo.hinhanh_truocsau_khachhang_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.hinhanh_truocsau_khachhang_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [hinhanh_truoc] NVarChar(MAX) NULL,
        [hinhanh_sau] NVarChar(MAX) NULL,
        [sdt] NVarChar(MAX) NULL,
        [nguoitao] NVarChar(MAX) NULL,
        [ngaytao] DateTime NULL,
        [ghichu] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.hinhanh_truocsau_khachhang_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.hinhanh_truocsau_khachhang_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.hinhanh_truocsau_khachhang_table', 'hinhanh_truoc') IS NULL
BEGIN
    ALTER TABLE dbo.hinhanh_truocsau_khachhang_table ADD [hinhanh_truoc] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hinhanh_truocsau_khachhang_table', 'hinhanh_sau') IS NULL
BEGIN
    ALTER TABLE dbo.hinhanh_truocsau_khachhang_table ADD [hinhanh_sau] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hinhanh_truocsau_khachhang_table', 'sdt') IS NULL
BEGIN
    ALTER TABLE dbo.hinhanh_truocsau_khachhang_table ADD [sdt] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hinhanh_truocsau_khachhang_table', 'nguoitao') IS NULL
BEGIN
    ALTER TABLE dbo.hinhanh_truocsau_khachhang_table ADD [nguoitao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hinhanh_truocsau_khachhang_table', 'ngaytao') IS NULL
BEGIN
    ALTER TABLE dbo.hinhanh_truocsau_khachhang_table ADD [ngaytao] DateTime NULL;
END

IF COL_LENGTH('dbo.hinhanh_truocsau_khachhang_table', 'ghichu') IS NULL
BEGIN
    ALTER TABLE dbo.hinhanh_truocsau_khachhang_table ADD [ghichu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hinhanh_truocsau_khachhang_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.hinhanh_truocsau_khachhang_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.hocvien_lichsu_thanhtoan_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.hocvien_lichsu_thanhtoan_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [id_hocvien] NVarChar(MAX) NULL,
        [hinhthuc_thanhtoan] NVarChar(MAX) NULL,
        [sotienthanhtoan] BigInt NULL,
        [thoigian] DateTime NULL,
        [nguoithanhtoan] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.hocvien_lichsu_thanhtoan_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_lichsu_thanhtoan_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.hocvien_lichsu_thanhtoan_table', 'id_hocvien') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_lichsu_thanhtoan_table ADD [id_hocvien] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hocvien_lichsu_thanhtoan_table', 'hinhthuc_thanhtoan') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_lichsu_thanhtoan_table ADD [hinhthuc_thanhtoan] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hocvien_lichsu_thanhtoan_table', 'sotienthanhtoan') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_lichsu_thanhtoan_table ADD [sotienthanhtoan] BigInt NULL;
END

IF COL_LENGTH('dbo.hocvien_lichsu_thanhtoan_table', 'thoigian') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_lichsu_thanhtoan_table ADD [thoigian] DateTime NULL;
END

IF COL_LENGTH('dbo.hocvien_lichsu_thanhtoan_table', 'nguoithanhtoan') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_lichsu_thanhtoan_table ADD [nguoithanhtoan] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hocvien_lichsu_thanhtoan_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_lichsu_thanhtoan_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.hocvien_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.hocvien_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [id_giangvien] NVarChar(MAX) NULL,
        [tengiangvien_hientai] NVarChar(MAX) NULL,
        [nganhhoc] NVarChar(MAX) NULL,
        [sobuoi_lythuyet] Int NULL,
        [sobuoi_thuchanh] Int NULL,
        [sobuoi_trogiang] Int NULL,
        [goidaotao] NVarChar(MAX) NULL,
        [hocphi] BigInt NULL,
        [xeploai] NVarChar(MAX) NULL,
        [ngaycapbang] DateTime NULL,
        [capbang] NVarChar(MAX) NULL,
        [sotien_dathanhtoan] BigInt NULL,
        [sotien_conlai] BigInt NULL,
        [nguoitao] NVarChar(MAX) NULL,
        [ngaytao] DateTime NULL,
        [hoten] NVarChar(MAX) NULL,
        [hoten_khongdau] NVarChar(MAX) NULL,
        [anhdaidien] NVarChar(MAX) NULL,
        [ngaysinh] DateTime NULL,
        [email] NVarChar(MAX) NULL,
        [dienthoai] NVarChar(MAX) NULL,
        [zalo] NVarChar(MAX) NULL,
        [facebook] NVarChar(MAX) NULL,
        [anh_capbang] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.hocvien_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.hocvien_table', 'id_giangvien') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [id_giangvien] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'tengiangvien_hientai') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [tengiangvien_hientai] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'nganhhoc') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [nganhhoc] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'sobuoi_lythuyet') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [sobuoi_lythuyet] Int NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'sobuoi_thuchanh') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [sobuoi_thuchanh] Int NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'sobuoi_trogiang') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [sobuoi_trogiang] Int NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'goidaotao') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [goidaotao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'hocphi') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [hocphi] BigInt NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'xeploai') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [xeploai] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'ngaycapbang') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [ngaycapbang] DateTime NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'capbang') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [capbang] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'sotien_dathanhtoan') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [sotien_dathanhtoan] BigInt NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'sotien_conlai') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [sotien_conlai] BigInt NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'nguoitao') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [nguoitao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'ngaytao') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [ngaytao] DateTime NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'hoten') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [hoten] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'hoten_khongdau') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [hoten_khongdau] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'anhdaidien') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [anhdaidien] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'ngaysinh') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [ngaysinh] DateTime NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'email') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [email] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'dienthoai') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [dienthoai] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'zalo') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [zalo] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'facebook') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [facebook] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'anh_capbang') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [anh_capbang] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.hocvien_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.hocvien_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.khachhang_table_2023', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.khachhang_table_2023
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [taikhoan] NVarChar(MAX) NOT NULL,
        [matkhau] NVarChar(MAX) NULL,
        [sdt] NVarChar(50) NULL,
        [tenkhachhang] NVarChar(MAX) NULL,
        [hoten_khongdau] NVarChar(MAX) NULL,
        [ngaytao] DateTime NULL,
        [nguoitao] NVarChar(MAX) NULL,
        [ngaysinh] Date NULL,
        [anhdaidien] NVarChar(MAX) NULL,
        [diachi] NVarChar(MAX) NULL,
        [magioithieu] NVarChar(MAX) NULL,
        [nguoichamsoc] NVarChar(MAX) NULL,
        [nhomkhachhang] NVarChar(MAX) NULL,
        [trangthai] NVarChar(MAX) NULL,
        [email] NVarChar(MAX) NULL,
        [zalo] NVarChar(MAX) NULL,
        [facebook] NVarChar(MAX) NULL,
        [hansudung] Date NULL,
        [makhoiphuc] NVarChar(MAX) NULL,
        [hsd_makhoiphuc] DateTime NULL,
        [permission] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'taikhoan') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [taikhoan] NVarChar(MAX) NOT NULL;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'matkhau') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [matkhau] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'sdt') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [sdt] NVarChar(50) NULL;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'tenkhachhang') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [tenkhachhang] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'hoten_khongdau') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [hoten_khongdau] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'ngaytao') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [ngaytao] DateTime NULL;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'nguoitao') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [nguoitao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'ngaysinh') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [ngaysinh] Date NULL;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'anhdaidien') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [anhdaidien] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'diachi') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [diachi] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'magioithieu') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [magioithieu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'nguoichamsoc') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [nguoichamsoc] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'nhomkhachhang') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [nhomkhachhang] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'trangthai') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [trangthai] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'email') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [email] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'zalo') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [zalo] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'facebook') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [facebook] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'hansudung') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [hansudung] Date NULL;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'makhoiphuc') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [makhoiphuc] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'hsd_makhoiphuc') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [hsd_makhoiphuc] DateTime NULL;
END

IF COL_LENGTH('dbo.khachhang_table_2023', 'permission') IS NULL
BEGIN
    ALTER TABLE dbo.khachhang_table_2023 ADD [permission] NVarChar(MAX) NULL;
END

IF OBJECT_ID('dbo.lichsudiem_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.lichsudiem_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [ngay] DateTime NULL,
        [taikhoan_nhan] NVarChar(MAX) NULL,
        [taikhoan_gui] NVarChar(MAX) NULL,
        [loaidiem] NVarChar(MAX) NULL,
        [sodiem] Float NULL,
        [noidung] NVarChar(MAX) NULL,
        [tanggiam] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.lichsudiem_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.lichsudiem_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.lichsudiem_table', 'ngay') IS NULL
BEGIN
    ALTER TABLE dbo.lichsudiem_table ADD [ngay] DateTime NULL;
END

IF COL_LENGTH('dbo.lichsudiem_table', 'taikhoan_nhan') IS NULL
BEGIN
    ALTER TABLE dbo.lichsudiem_table ADD [taikhoan_nhan] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.lichsudiem_table', 'taikhoan_gui') IS NULL
BEGIN
    ALTER TABLE dbo.lichsudiem_table ADD [taikhoan_gui] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.lichsudiem_table', 'loaidiem') IS NULL
BEGIN
    ALTER TABLE dbo.lichsudiem_table ADD [loaidiem] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.lichsudiem_table', 'sodiem') IS NULL
BEGIN
    ALTER TABLE dbo.lichsudiem_table ADD [sodiem] Float NULL;
END

IF COL_LENGTH('dbo.lichsudiem_table', 'noidung') IS NULL
BEGIN
    ALTER TABLE dbo.lichsudiem_table ADD [noidung] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.lichsudiem_table', 'tanggiam') IS NULL
BEGIN
    ALTER TABLE dbo.lichsudiem_table ADD [tanggiam] NVarChar(MAX) NULL;
END

IF OBJECT_ID('dbo.listbank_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.listbank_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [name] NVarChar(MAX) NULL,
        [id_parent] BigInt NULL,
        [id_level] NChar(10) NULL
    );
END

IF COL_LENGTH('dbo.listbank_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.listbank_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.listbank_table', 'name') IS NULL
BEGIN
    ALTER TABLE dbo.listbank_table ADD [name] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.listbank_table', 'id_parent') IS NULL
BEGIN
    ALTER TABLE dbo.listbank_table ADD [id_parent] BigInt NULL;
END

IF COL_LENGTH('dbo.listbank_table', 'id_level') IS NULL
BEGIN
    ALTER TABLE dbo.listbank_table ADD [id_level] NChar(10) NULL;
END

IF OBJECT_ID('dbo.mailbox_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.mailbox_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [tieude] NVarChar(MAX) NULL,
        [name] NVarChar(MAX) NULL,
        [phone] NVarChar(MAX) NULL,
        [email] NVarChar(MAX) NULL,
        [noidung] NVarChar(MAX) NULL,
        [thoigian] DateTime NULL,
        [daxem] Bit NULL,
        [nguongoc] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.mailbox_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.mailbox_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.mailbox_table', 'tieude') IS NULL
BEGIN
    ALTER TABLE dbo.mailbox_table ADD [tieude] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.mailbox_table', 'name') IS NULL
BEGIN
    ALTER TABLE dbo.mailbox_table ADD [name] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.mailbox_table', 'phone') IS NULL
BEGIN
    ALTER TABLE dbo.mailbox_table ADD [phone] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.mailbox_table', 'email') IS NULL
BEGIN
    ALTER TABLE dbo.mailbox_table ADD [email] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.mailbox_table', 'noidung') IS NULL
BEGIN
    ALTER TABLE dbo.mailbox_table ADD [noidung] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.mailbox_table', 'thoigian') IS NULL
BEGIN
    ALTER TABLE dbo.mailbox_table ADD [thoigian] DateTime NULL;
END

IF COL_LENGTH('dbo.mailbox_table', 'daxem') IS NULL
BEGIN
    ALTER TABLE dbo.mailbox_table ADD [daxem] Bit NULL;
END

IF COL_LENGTH('dbo.mailbox_table', 'nguongoc') IS NULL
BEGIN
    ALTER TABLE dbo.mailbox_table ADD [nguongoc] NVarChar(MAX) NULL;
END

IF OBJECT_ID('dbo.nganh_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.nganh_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [ten] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.nganh_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.nganh_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.nganh_table', 'ten') IS NULL
BEGIN
    ALTER TABLE dbo.nganh_table ADD [ten] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.nganh_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.nganh_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.nhacungcap_nhaphang_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.nhacungcap_nhaphang_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [ten] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.nhacungcap_nhaphang_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.nhacungcap_nhaphang_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.nhacungcap_nhaphang_table', 'ten') IS NULL
BEGIN
    ALTER TABLE dbo.nhacungcap_nhaphang_table ADD [ten] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.nhacungcap_nhaphang_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.nhacungcap_nhaphang_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.nhomkhachhang_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.nhomkhachhang_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [tennhom] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.nhomkhachhang_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.nhomkhachhang_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.nhomkhachhang_table', 'tennhom') IS NULL
BEGIN
    ALTER TABLE dbo.nhomkhachhang_table ADD [tennhom] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.nhomkhachhang_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.nhomkhachhang_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.nhomvattu_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.nhomvattu_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [tennhom] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.nhomvattu_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.nhomvattu_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.nhomvattu_table', 'tennhom') IS NULL
BEGIN
    ALTER TABLE dbo.nhomvattu_table ADD [tennhom] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.nhomvattu_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.nhomvattu_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.phongban_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.phongban_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [ten] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.phongban_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.phongban_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.phongban_table', 'ten') IS NULL
BEGIN
    ALTER TABLE dbo.phongban_table ADD [ten] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.phongban_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.phongban_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.taikhoan_table_2023', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.taikhoan_table_2023
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [taikhoan] NVarChar(MAX) NOT NULL,
        [matkhau] NVarChar(MAX) NULL,
        [hoten] NVarChar(MAX) NULL,
        [hoten_khongdau] NVarChar(MAX) NULL,
        [anhdaidien] NVarChar(MAX) NULL,
        [ngaysinh] Date NULL,
        [nguoitao] NVarChar(MAX) NULL,
        [ngaytao] DateTime NULL,
        [trangthai] NVarChar(MAX) NULL,
        [email] NVarChar(MAX) NULL,
        [dienthoai] NVarChar(50) NULL,
        [zalo] NVarChar(MAX) NULL,
        [facebook] NVarChar(MAX) NULL,
        [hansudung] Date NULL,
        [makhoiphuc] NVarChar(MAX) NULL,
        [hsd_makhoiphuc] DateTime NULL,
        [permission] NVarChar(MAX) NULL,
        [luongcoban] Int NULL,
        [diachi] NVarChar(MAX) NULL,
        [songaycong] Int NULL,
        [luongngay] BigInt NULL,
        [id_bophan] NVarChar(MAX) NULL,
        [user_parent] NVarChar(MAX) NULL,
        [id_nganh] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'taikhoan') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [taikhoan] NVarChar(MAX) NOT NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'matkhau') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [matkhau] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'hoten') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [hoten] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'hoten_khongdau') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [hoten_khongdau] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'anhdaidien') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [anhdaidien] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'ngaysinh') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [ngaysinh] Date NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'nguoitao') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [nguoitao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'ngaytao') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [ngaytao] DateTime NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'trangthai') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [trangthai] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'email') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [email] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'dienthoai') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [dienthoai] NVarChar(50) NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'zalo') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [zalo] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'facebook') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [facebook] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'hansudung') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [hansudung] Date NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'makhoiphuc') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [makhoiphuc] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'hsd_makhoiphuc') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [hsd_makhoiphuc] DateTime NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'permission') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [permission] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'luongcoban') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [luongcoban] Int NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'diachi') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [diachi] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'songaycong') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [songaycong] Int NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'luongngay') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [luongngay] BigInt NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'id_bophan') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [id_bophan] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'user_parent') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [user_parent] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'id_nganh') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [id_nganh] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.taikhoan_table_2023', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_table_2023 ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.thedichvu_lichsu_thanhtoan_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.thedichvu_lichsu_thanhtoan_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [user_parent] NVarChar(MAX) NULL,
        [id_hoadon] NVarChar(MAX) NULL,
        [hinhthuc_thanhtoan] NVarChar(MAX) NULL,
        [sotienthanhtoan] BigInt NULL,
        [thoigian] DateTime NULL,
        [nguoithanhtoan] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.thedichvu_lichsu_thanhtoan_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_lichsu_thanhtoan_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.thedichvu_lichsu_thanhtoan_table', 'user_parent') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_lichsu_thanhtoan_table ADD [user_parent] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.thedichvu_lichsu_thanhtoan_table', 'id_hoadon') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_lichsu_thanhtoan_table ADD [id_hoadon] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.thedichvu_lichsu_thanhtoan_table', 'hinhthuc_thanhtoan') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_lichsu_thanhtoan_table ADD [hinhthuc_thanhtoan] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.thedichvu_lichsu_thanhtoan_table', 'sotienthanhtoan') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_lichsu_thanhtoan_table ADD [sotienthanhtoan] BigInt NULL;
END

IF COL_LENGTH('dbo.thedichvu_lichsu_thanhtoan_table', 'thoigian') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_lichsu_thanhtoan_table ADD [thoigian] DateTime NULL;
END

IF COL_LENGTH('dbo.thedichvu_lichsu_thanhtoan_table', 'nguoithanhtoan') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_lichsu_thanhtoan_table ADD [nguoithanhtoan] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.thedichvu_lichsu_thanhtoan_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_lichsu_thanhtoan_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.thedichvu_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.thedichvu_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [nguoitao] NVarChar(MAX) NULL,
        [ngaytao] DateTime NULL,
        [tenthe] NVarChar(MAX) NULL,
        [tenkh] NVarChar(MAX) NULL,
        [iddv] NVarChar(MAX) NULL,
        [ten_taithoidiemnay] NVarChar(MAX) NULL,
        [hsd] DateTime NULL,
        [tongsoluong] Int NULL,
        [sl_dalam] Int NULL,
        [sl_conlai] Int NULL,
        [sdt] NVarChar(MAX) NULL,
        [ghichu] NVarChar(MAX) NULL,
        [tongtien] BigInt NULL,
        [chietkhau] Int NULL,
        [tongtien_ck_hoadon] BigInt NULL,
        [tongsauchietkhau] BigInt NULL,
        [diachi] NVarChar(MAX) NULL,
        [sotien_dathanhtoan] BigInt NULL,
        [sotien_conlai] BigInt NULL,
        [nguoichotsale] NVarChar(MAX) NULL,
        [phantram_chotsale] Int NULL,
        [tongtien_chotsale_dvsp] BigInt NULL,
        [tennguoichot_hientai] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL,
        [id_nganh] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.thedichvu_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.thedichvu_table', 'nguoitao') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [nguoitao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'ngaytao') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [ngaytao] DateTime NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'tenthe') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [tenthe] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'tenkh') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [tenkh] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'iddv') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [iddv] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'ten_taithoidiemnay') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [ten_taithoidiemnay] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'hsd') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [hsd] DateTime NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'tongsoluong') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [tongsoluong] Int NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'sl_dalam') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [sl_dalam] Int NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'sl_conlai') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [sl_conlai] Int NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'sdt') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [sdt] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'ghichu') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [ghichu] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'tongtien') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [tongtien] BigInt NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'chietkhau') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [chietkhau] Int NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'tongtien_ck_hoadon') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [tongtien_ck_hoadon] BigInt NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'tongsauchietkhau') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [tongsauchietkhau] BigInt NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'diachi') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [diachi] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'sotien_dathanhtoan') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [sotien_dathanhtoan] BigInt NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'sotien_conlai') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [sotien_conlai] BigInt NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'nguoichotsale') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [nguoichotsale] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'phantram_chotsale') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [phantram_chotsale] Int NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'tongtien_chotsale_dvsp') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [tongtien_chotsale_dvsp] BigInt NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'tennguoichot_hientai') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [tennguoichot_hientai] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF COL_LENGTH('dbo.thedichvu_table', 'id_nganh') IS NULL
BEGIN
    ALTER TABLE dbo.thedichvu_table ADD [id_nganh] NVarChar(MAX) NULL;
END

IF OBJECT_ID('dbo.thongbao_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.thongbao_table
    (
        [id] UniqueIdentifier NOT NULL PRIMARY KEY,
        [nguoithongbao] NVarChar(MAX) NULL,
        [nguoinhan] NVarChar(MAX) NULL,
        [link] NVarChar(MAX) NULL,
        [thoigian] DateTime NULL,
        [noidung] NVarChar(MAX) NULL,
        [daxem] Bit NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.thongbao_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.thongbao_table ADD [id] UniqueIdentifier NOT NULL;
END

IF COL_LENGTH('dbo.thongbao_table', 'nguoithongbao') IS NULL
BEGIN
    ALTER TABLE dbo.thongbao_table ADD [nguoithongbao] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.thongbao_table', 'nguoinhan') IS NULL
BEGIN
    ALTER TABLE dbo.thongbao_table ADD [nguoinhan] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.thongbao_table', 'link') IS NULL
BEGIN
    ALTER TABLE dbo.thongbao_table ADD [link] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.thongbao_table', 'thoigian') IS NULL
BEGIN
    ALTER TABLE dbo.thongbao_table ADD [thoigian] DateTime NULL;
END

IF COL_LENGTH('dbo.thongbao_table', 'noidung') IS NULL
BEGIN
    ALTER TABLE dbo.thongbao_table ADD [noidung] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.thongbao_table', 'daxem') IS NULL
BEGIN
    ALTER TABLE dbo.thongbao_table ADD [daxem] Bit NULL;
END

IF COL_LENGTH('dbo.thongbao_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.thongbao_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.tinhthanh_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.tinhthanh_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [tinhthanh] NVarChar(MAX) NULL
    );
END

IF COL_LENGTH('dbo.tinhthanh_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.tinhthanh_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.tinhthanh_table', 'tinhthanh') IS NULL
BEGIN
    ALTER TABLE dbo.tinhthanh_table ADD [tinhthanh] NVarChar(MAX) NULL;
END

IF OBJECT_ID('dbo.web_menu_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.web_menu_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [name] NVarChar(MAX) NULL,
        [id_parent] NVarChar(MAX) NULL,
        [id_level] Int NULL,
        [name_en] NVarChar(MAX) NULL,
        [description] NVarChar(MAX) NULL,
        [image] VarChar(MAX) NULL,
        [rank] Int NULL,
        [url_other] NVarChar(MAX) NULL,
        [ngaytao] DateTime NULL,
        [bin] Bit NULL,
        [phanloai] NVarChar(MAX) NULL,
        [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL
    );
END

IF COL_LENGTH('dbo.web_menu_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.web_menu_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.web_menu_table', 'name') IS NULL
BEGIN
    ALTER TABLE dbo.web_menu_table ADD [name] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.web_menu_table', 'id_parent') IS NULL
BEGIN
    ALTER TABLE dbo.web_menu_table ADD [id_parent] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.web_menu_table', 'id_level') IS NULL
BEGIN
    ALTER TABLE dbo.web_menu_table ADD [id_level] Int NULL;
END

IF COL_LENGTH('dbo.web_menu_table', 'name_en') IS NULL
BEGIN
    ALTER TABLE dbo.web_menu_table ADD [name_en] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.web_menu_table', 'description') IS NULL
BEGIN
    ALTER TABLE dbo.web_menu_table ADD [description] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.web_menu_table', 'image') IS NULL
BEGIN
    ALTER TABLE dbo.web_menu_table ADD [image] VarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.web_menu_table', 'rank') IS NULL
BEGIN
    ALTER TABLE dbo.web_menu_table ADD [rank] Int NULL;
END

IF COL_LENGTH('dbo.web_menu_table', 'url_other') IS NULL
BEGIN
    ALTER TABLE dbo.web_menu_table ADD [url_other] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.web_menu_table', 'ngaytao') IS NULL
BEGIN
    ALTER TABLE dbo.web_menu_table ADD [ngaytao] DateTime NULL;
END

IF COL_LENGTH('dbo.web_menu_table', 'bin') IS NULL
BEGIN
    ALTER TABLE dbo.web_menu_table ADD [bin] Bit NULL;
END

IF COL_LENGTH('dbo.web_menu_table', 'phanloai') IS NULL
BEGIN
    ALTER TABLE dbo.web_menu_table ADD [phanloai] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.web_menu_table', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.web_menu_table ADD [id_chinhanh] NVarChar(MAX) NULL,
        [id_baiviet] Int NULL;
END

IF OBJECT_ID('dbo.web_module_slider_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.web_module_slider_table
    (
        [id] BigInt NOT NULL IDENTITY PRIMARY KEY,
        [img] NVarChar(MAX) NULL,
        [link] NVarChar(MAX) NULL,
        [but_title] NVarChar(MAX) NULL,
        [rank] Int NULL
    );
END

IF COL_LENGTH('dbo.web_module_slider_table', 'id') IS NULL
BEGIN
    ALTER TABLE dbo.web_module_slider_table ADD [id] BigInt NOT NULL IDENTITY;
END

IF COL_LENGTH('dbo.web_module_slider_table', 'img') IS NULL
BEGIN
    ALTER TABLE dbo.web_module_slider_table ADD [img] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.web_module_slider_table', 'link') IS NULL
BEGIN
    ALTER TABLE dbo.web_module_slider_table ADD [link] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.web_module_slider_table', 'but_title') IS NULL
BEGIN
    ALTER TABLE dbo.web_module_slider_table ADD [but_title] NVarChar(MAX) NULL;
END

IF COL_LENGTH('dbo.web_module_slider_table', 'rank') IS NULL
BEGIN
    ALTER TABLE dbo.web_module_slider_table ADD [rank] Int NULL;
END

");

                _schemaReady = true;
                db.CommandTimeout = originalTimeout;
            }
            catch
            {
                // thử lại sau nếu lỗi
            }
        }
    }
}
