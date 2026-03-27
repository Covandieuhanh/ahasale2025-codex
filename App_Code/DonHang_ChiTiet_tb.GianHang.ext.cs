using System.Data.Linq.Mapping;

public partial class DonHang_ChiTiet_tb
{
    private int? _gh_product_id;
    private string _gh_name;
    private string _gh_image;
    private string _gh_type;

    [Column(Name = "gh_product_id", Storage = "_gh_product_id", DbType = "Int", CanBeNull = true)]
    public int? gh_product_id
    {
        get { return _gh_product_id; }
        set
        {
            if (_gh_product_id != value)
            {
                this.SendPropertyChanging();
                _gh_product_id = value;
                this.SendPropertyChanged("gh_product_id");
            }
        }
    }

    [Column(Name = "gh_name", Storage = "_gh_name", DbType = "NVarChar(MAX)", CanBeNull = true)]
    public string gh_name
    {
        get { return _gh_name; }
        set
        {
            if (_gh_name != value)
            {
                this.SendPropertyChanging();
                _gh_name = value;
                this.SendPropertyChanged("gh_name");
            }
        }
    }

    [Column(Name = "gh_image", Storage = "_gh_image", DbType = "NVarChar(MAX)", CanBeNull = true)]
    public string gh_image
    {
        get { return _gh_image; }
        set
        {
            if (_gh_image != value)
            {
                this.SendPropertyChanging();
                _gh_image = value;
                this.SendPropertyChanged("gh_image");
            }
        }
    }

    [Column(Name = "gh_type", Storage = "_gh_type", DbType = "NVarChar(30)", CanBeNull = true)]
    public string gh_type
    {
        get { return _gh_type; }
        set
        {
            if (_gh_type != value)
            {
                this.SendPropertyChanging();
                _gh_type = value;
                this.SendPropertyChanged("gh_type");
            }
        }
    }
}
