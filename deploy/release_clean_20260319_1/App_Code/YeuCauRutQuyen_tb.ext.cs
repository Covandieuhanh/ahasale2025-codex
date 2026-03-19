using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

public partial class YeuCauRutQuyen_tb
{
    private int _LoaiVi;
    private int _KyHieu9ViCon_1_9;

    [Column(Storage = "_LoaiVi", DbType = "Int NOT NULL", CanBeNull = false)]
    public int LoaiVi
    {
        get { return this._LoaiVi; }
        set
        {
            if (this._LoaiVi != value)
            {
                this.SendPropertyChanging();
                this._LoaiVi = value;
                this.SendPropertyChanged("LoaiVi");
            }
        }
    }

    [Column(Storage = "_KyHieu9ViCon_1_9", DbType = "Int NOT NULL", CanBeNull = false)]
    public int KyHieu9ViCon_1_9
    {
        get { return this._KyHieu9ViCon_1_9; }
        set
        {
            if (this._KyHieu9ViCon_1_9 != value)
            {
                this.SendPropertyChanging();
                this._KyHieu9ViCon_1_9 = value;
                this.SendPropertyChanged("KyHieu9ViCon_1_9");
            }
        }
    }
}
