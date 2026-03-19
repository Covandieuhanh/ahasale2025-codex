using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class cauhinhchung_class
{
    public static void update_lienket_chiase(string _title, string _description, string _image)
    {
        dbDataContext db = new dbDataContext();
        var q = db.config_lienket_chiase_tables;
        config_lienket_chiase_table _ob = q.FirstOrDefault();
        if (_ob == null)
        {
            _ob = new config_lienket_chiase_table
            {
                title = "",
                description = "",
                image = ""
            };
            db.config_lienket_chiase_tables.InsertOnSubmit(_ob);
        }
        _ob.title = _title;
        _ob.description = _description;
        _ob.image = _image;
        db.SubmitChanges();
    }
    public static void update_nhung_ma(string _code_head, string _code_body1, string _code_body2, string _code_page, string _code_map)
    {
        dbDataContext db = new dbDataContext();
        var q = db.config_nhungma_tables;
        config_nhungma_table _ob = q.FirstOrDefault();
        if (_ob == null)
        {
            _ob = new config_nhungma_table
            {
                nhungma_head = "",
                nhungma_body1 = "",
                nhungma_body2 = "",
                nhungma_fanpage = "",
                nhungma_googlemaps = ""
            };
            db.config_nhungma_tables.InsertOnSubmit(_ob);
        }
        _ob.nhungma_head = _code_head;
        _ob.nhungma_body1 = _code_body1;
        _ob.nhungma_body2 = _code_body2;
        _ob.nhungma_fanpage = _code_page;
        _ob.nhungma_googlemaps = _code_map;
        db.SubmitChanges();
    }
    public static void update_thongtin( string _favicon, string _icon_mobile, string _logo, string _logo1, string _tencongty, string _slogan, string _diachi,
       string _googlemap, string _hotline, string _email, string _masothue, string _zalo, string _logo_hoadon)
    {
        dbDataContext db = new dbDataContext();
        var q = db.config_thongtin_tables;
        config_thongtin_table _ob = q.FirstOrDefault();
        if (_ob == null)
        {
            _ob = new config_thongtin_table
            {
                icon = "/uploads/images/favicon.png",
                apple_touch_icon = "/uploads/images/icon-mobile.jpg",
                logo = "/uploads/images/logo.png",
                logo1 = "/uploads/images/logo.png",
                tencongty = "Gian hàng đối tác",
                slogan = "",
                diachi = "",
                link_googlemap = "",
                hotline = "",
                email = "",
                masothue = "",
                zalo = "",
                logo_in_hoadon = ""
            };
            db.config_thongtin_tables.InsertOnSubmit(_ob);
        }
        _ob.icon = _favicon;
        _ob.apple_touch_icon = _icon_mobile;
        _ob.tencongty = _tencongty;
        _ob.slogan = _slogan;
        _ob.diachi = _diachi;
        _ob.link_googlemap = _googlemap;
        _ob.hotline = _hotline;
        _ob.zalo = _zalo;
        _ob.email = _email;
        _ob.logo = _logo; _ob.logo1 = _logo1;
        _ob.masothue = _masothue; _ob.logo_in_hoadon = _logo_hoadon;
        db.SubmitChanges();
    }
    public static void update_thoigian_baotri(bool _trangthai, string _batdau, string _ketthuc)
    {
        dbDataContext db = new dbDataContext();
        var q = db.config_baotri_tables;
        config_baotri_table _ob = q.FirstOrDefault();
        if (_ob == null)
        {
            _ob = new config_baotri_table
            {
                baotri_trangthai = false,
                baotri_thoigian_batdau = null,
                baotri_thoigian_ketthuc = null,
                ghichu = ""
            };
            db.config_baotri_tables.InsertOnSubmit(_ob);
        }
        _ob.baotri_trangthai = _trangthai;
        if (_batdau == "0")
            _ob.baotri_thoigian_batdau = null;
        else
            _ob.baotri_thoigian_batdau = DateTime.Parse(_batdau);
        if (_ketthuc == "0")
            _ob.baotri_thoigian_ketthuc = null;
        else
            _ob.baotri_thoigian_ketthuc = DateTime.Parse(_ketthuc);
        db.SubmitChanges();
    }

}
