using System;
using System.Data.Linq.Mapping;

public partial class web_post_table
{
    private int? _thoiluong_dichvu_phut;

    [Column(Storage = "_thoiluong_dichvu_phut", DbType = "Int")]
    public int? thoiluong_dichvu_phut
    {
        get
        {
            return _thoiluong_dichvu_phut;
        }
        set
        {
            if (_thoiluong_dichvu_phut != value)
            {
                SendPropertyChanging();
                _thoiluong_dichvu_phut = value;
                SendPropertyChanged("thoiluong_dichvu_phut");
            }
        }
    }
}

public partial class bspa_datlich_table
{
    private int? _thoiluong_dichvu_phut;
    private DateTime? _ngayketthucdukien;

    [Column(Storage = "_thoiluong_dichvu_phut", DbType = "Int")]
    public int? thoiluong_dichvu_phut
    {
        get
        {
            return _thoiluong_dichvu_phut;
        }
        set
        {
            if (_thoiluong_dichvu_phut != value)
            {
                SendPropertyChanging();
                _thoiluong_dichvu_phut = value;
                SendPropertyChanged("thoiluong_dichvu_phut");
            }
        }
    }

    [Column(Storage = "_ngayketthucdukien", DbType = "DateTime")]
    public DateTime? ngayketthucdukien
    {
        get
        {
            return _ngayketthucdukien;
        }
        set
        {
            if (_ngayketthucdukien != value)
            {
                SendPropertyChanging();
                _ngayketthucdukien = value;
                SendPropertyChanged("ngayketthucdukien");
            }
        }
    }
}
