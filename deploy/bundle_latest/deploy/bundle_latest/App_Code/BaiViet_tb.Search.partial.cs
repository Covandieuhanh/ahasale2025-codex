public partial class BaiViet_tb
{
    partial void OnnameChanged()
    {
        name_khongdau = BaiVietSearchSchema_cl.NormalizeText(name);
    }

    partial void OndescriptionChanged()
    {
        description_khongdau = BaiVietSearchSchema_cl.NormalizeText(description);
    }
}
