using System.Data.Linq;

public partial class dbDataContext
{
    public Table<bspa_bophan_table> bspa_bophan_tables { get { return GetTable<bspa_bophan_table>(); } }
    public Table<web_post_table> web_post_tables { get { return GetTable<web_post_table>(); } }
    public Table<bspa_caidatchung_table> bspa_caidatchung_tables { get { return GetTable<bspa_caidatchung_table>(); } }
    public Table<bspa_chamcong_table> bspa_chamcong_tables { get { return GetTable<bspa_chamcong_table>(); } }
    public Table<bspa_chiphi_codinh_table> bspa_chiphi_codinh_tables { get { return GetTable<bspa_chiphi_codinh_table>(); } }
    public Table<bspa_data_khachhang_table> bspa_data_khachhang_tables { get { return GetTable<bspa_data_khachhang_table>(); } }
    public Table<bspa_datlich_table> bspa_datlich_tables { get { return GetTable<bspa_datlich_table>(); } }
    public Table<bspa_hinhanhthuchi_table> bspa_hinhanhthuchi_tables { get { return GetTable<bspa_hinhanhthuchi_table>(); } }
    public Table<bspa_hoadon_chitiet_table> bspa_hoadon_chitiet_tables { get { return GetTable<bspa_hoadon_chitiet_table>(); } }
    public Table<bspa_hoadon_table> bspa_hoadon_tables { get { return GetTable<bspa_hoadon_table>(); } }
    public Table<bspa_lichsu_thanhtoan_table> bspa_lichsu_thanhtoan_tables { get { return GetTable<bspa_lichsu_thanhtoan_table>(); } }
    public Table<bspa_nhomthuchi_table> bspa_nhomthuchi_tables { get { return GetTable<bspa_nhomthuchi_table>(); } }
    public Table<bspa_thuchi_table> bspa_thuchi_tables { get { return GetTable<bspa_thuchi_table>(); } }
    public Table<chinhanh_table> chinhanh_tables { get { return GetTable<chinhanh_table>(); } }
    public Table<config_baotri_table> config_baotri_tables { get { return GetTable<config_baotri_table>(); } }
    public Table<config_lienket_chiase_table> config_lienket_chiase_tables { get { return GetTable<config_lienket_chiase_table>(); } }
    public Table<config_nhungma_table> config_nhungma_tables { get { return GetTable<config_nhungma_table>(); } }
    public Table<config_social_media_table> config_social_media_tables { get { return GetTable<config_social_media_table>(); } }
    public Table<config_thongtin_table> config_thongtin_tables { get { return GetTable<config_thongtin_table>(); } }
    public Table<danhsach_vattu_table> danhsach_vattu_tables { get { return GetTable<danhsach_vattu_table>(); } }
    public Table<donnhap_vattu_chitiet_table> donnhap_vattu_chitiet_tables { get { return GetTable<donnhap_vattu_chitiet_table>(); } }
    public Table<donnhap_vattu_lichsu_thanhtoan_table> donnhap_vattu_lichsu_thanhtoan_tables { get { return GetTable<donnhap_vattu_lichsu_thanhtoan_table>(); } }
    public Table<donnhap_vattu_table> donnhap_vattu_tables { get { return GetTable<donnhap_vattu_table>(); } }
    public Table<donnhaphang_chitiet_table> donnhaphang_chitiet_tables { get { return GetTable<donnhaphang_chitiet_table>(); } }
    public Table<donnhaphang_lichsu_thanhtoan_table> donnhaphang_lichsu_thanhtoan_tables { get { return GetTable<donnhaphang_lichsu_thanhtoan_table>(); } }
    public Table<donnhaphang_table> donnhaphang_tables { get { return GetTable<donnhaphang_table>(); } }
    public Table<donthuoc_khachhang_table> donthuoc_khachhang_tables { get { return GetTable<donthuoc_khachhang_table>(); } }
    public Table<ghichu_khachhang_table> ghichu_khachhang_tables { get { return GetTable<ghichu_khachhang_table>(); } }
    public Table<gianhang_storefront_config_table> gianhang_storefront_config_tables { get { return GetTable<gianhang_storefront_config_table>(); } }
    public Table<gianhang_storefront_section_table> gianhang_storefront_section_tables { get { return GetTable<gianhang_storefront_section_table>(); } }
    public Table<giangvien_table> giangvien_tables { get { return GetTable<giangvien_table>(); } }
    public Table<hethong_checkall_table> hethong_checkall_tables { get { return GetTable<hethong_checkall_table>(); } }
    public Table<hinhanh_truocsau_khachhang_table> hinhanh_truocsau_khachhang_tables { get { return GetTable<hinhanh_truocsau_khachhang_table>(); } }
    public Table<hocvien_lichsu_thanhtoan_table> hocvien_lichsu_thanhtoan_tables { get { return GetTable<hocvien_lichsu_thanhtoan_table>(); } }
    public Table<hocvien_table> hocvien_tables { get { return GetTable<hocvien_table>(); } }
    public Table<khachhang_table_2023> khachhang_table_2023s { get { return GetTable<khachhang_table_2023>(); } }
    public Table<lichsudiem_table> lichsudiem_tables { get { return GetTable<lichsudiem_table>(); } }
    public Table<listbank_table> listbank_tables { get { return GetTable<listbank_table>(); } }
    public Table<mailbox_table> mailbox_tables { get { return GetTable<mailbox_table>(); } }
    public Table<nganh_table> nganh_tables { get { return GetTable<nganh_table>(); } }
    public Table<nhacungcap_nhaphang_table> nhacungcap_nhaphang_tables { get { return GetTable<nhacungcap_nhaphang_table>(); } }
    public Table<nhomkhachhang_table> nhomkhachhang_tables { get { return GetTable<nhomkhachhang_table>(); } }
    public Table<nhomvattu_table> nhomvattu_tables { get { return GetTable<nhomvattu_table>(); } }
    public Table<phongban_table> phongban_tables { get { return GetTable<phongban_table>(); } }
    public Table<taikhoan_table_2023> taikhoan_table_2023s { get { return GetTable<taikhoan_table_2023>(); } }
    public Table<thedichvu_lichsu_thanhtoan_table> thedichvu_lichsu_thanhtoan_tables { get { return GetTable<thedichvu_lichsu_thanhtoan_table>(); } }
    public Table<thedichvu_table> thedichvu_tables { get { return GetTable<thedichvu_table>(); } }
    public Table<thongbao_table> thongbao_tables { get { return GetTable<thongbao_table>(); } }
    public Table<tinhthanh_table> tinhthanh_tables { get { return GetTable<tinhthanh_table>(); } }
    public Table<web_menu_table> web_menu_tables { get { return GetTable<web_menu_table>(); } }
    public Table<web_module_slider_table> web_module_slider_tables { get { return GetTable<web_module_slider_table>(); } }
}
