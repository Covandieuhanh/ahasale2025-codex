using System.Data.Linq.Mapping;

public partial class bspa_thuchi_table
{
    private bool? _tudong_tu_hethong;
    private string _nguon_tudong;
    private string _url_admin_lienket;
    private bool? _tudong_tu_hoadon;
    private string _id_hoadon_lienket;
    private string _id_lichsu_thanhtoan_lienket;

    [Column(Storage = "_tudong_tu_hethong", DbType = "Bit")]
    public bool? tudong_tu_hethong
    {
        get
        {
            return _tudong_tu_hethong;
        }
        set
        {
            if (_tudong_tu_hethong != value)
            {
                SendPropertyChanging();
                _tudong_tu_hethong = value;
                SendPropertyChanged("tudong_tu_hethong");
            }
        }
    }

    [Column(Storage = "_nguon_tudong", DbType = "NVarChar(MAX)")]
    public string nguon_tudong
    {
        get
        {
            return _nguon_tudong;
        }
        set
        {
            if (_nguon_tudong != value)
            {
                SendPropertyChanging();
                _nguon_tudong = value;
                SendPropertyChanged("nguon_tudong");
            }
        }
    }

    [Column(Storage = "_url_admin_lienket", DbType = "NVarChar(MAX)")]
    public string url_admin_lienket
    {
        get
        {
            return _url_admin_lienket;
        }
        set
        {
            if (_url_admin_lienket != value)
            {
                SendPropertyChanging();
                _url_admin_lienket = value;
                SendPropertyChanged("url_admin_lienket");
            }
        }
    }

    [Column(Storage = "_tudong_tu_hoadon", DbType = "Bit")]
    public bool? tudong_tu_hoadon
    {
        get
        {
            return _tudong_tu_hoadon;
        }
        set
        {
            if (_tudong_tu_hoadon != value)
            {
                SendPropertyChanging();
                _tudong_tu_hoadon = value;
                SendPropertyChanged("tudong_tu_hoadon");
            }
        }
    }

    [Column(Storage = "_id_hoadon_lienket", DbType = "NVarChar(MAX)")]
    public string id_hoadon_lienket
    {
        get
        {
            return _id_hoadon_lienket;
        }
        set
        {
            if (_id_hoadon_lienket != value)
            {
                SendPropertyChanging();
                _id_hoadon_lienket = value;
                SendPropertyChanged("id_hoadon_lienket");
            }
        }
    }

    [Column(Storage = "_id_lichsu_thanhtoan_lienket", DbType = "NVarChar(MAX)")]
    public string id_lichsu_thanhtoan_lienket
    {
        get
        {
            return _id_lichsu_thanhtoan_lienket;
        }
        set
        {
            if (_id_lichsu_thanhtoan_lienket != value)
            {
                SendPropertyChanging();
                _id_lichsu_thanhtoan_lienket = value;
                SendPropertyChanged("id_lichsu_thanhtoan_lienket");
            }
        }
    }
}
