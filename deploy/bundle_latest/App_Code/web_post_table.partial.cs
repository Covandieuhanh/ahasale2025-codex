using System.Data.Linq.Mapping;

public partial class web_post_table
{
    [Column]
    public int? id_baiviet { get; set; }

    [Column]
    public string nguoitao { get; set; }
}
