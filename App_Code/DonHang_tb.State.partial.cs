using System.Data.Linq.Mapping;

public partial class DonHang_tb
{
    private string _order_status;
    private string _exchange_status;

    [Column(Name = "order_status", Storage = "_order_status", DbType = "NVarChar(50)")]
    public string order_status
    {
        get
        {
            return this._order_status;
        }
        set
        {
            if (this._order_status != value)
            {
                this.SendPropertyChanging();
                this._order_status = value;
                this.SendPropertyChanged("order_status");
            }
        }
    }

    [Column(Name = "exchange_status", Storage = "_exchange_status", DbType = "NVarChar(50)")]
    public string exchange_status
    {
        get
        {
            return this._exchange_status;
        }
        set
        {
            if (this._exchange_status != value)
            {
                this.SendPropertyChanging();
                this._exchange_status = value;
                this.SendPropertyChanged("exchange_status");
            }
        }
    }

    // Legacy compatibility: some old call sites still reference `ngaytao`.
    public System.Nullable<System.DateTime> ngaytao
    {
        get
        {
            return this.ngaydat;
        }
        set
        {
            this.ngaydat = value;
        }
    }
}
