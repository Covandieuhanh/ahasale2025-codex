using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class taikhoan_add : System.Web.UI.Page
{
    public string id, sdt, notifi, user, user_parent, id_chinhanh_khachhang;
    taikhoan_class tk_cl = new taikhoan_class();
    hoadon_class hd_cl = new hoadon_class();
    dbDataContext db = new dbDataContext();
    string_class str_cl = new string_class();
    datetime_class dt_cl = new datetime_class();
    data_khachhang_class dt_kh_cl = new data_khachhang_class(); thedichvu_class tdv_cl = new thedichvu_class();
    post_class po_cl = new post_class();
    public string hoten;
    public int tong_hoadon = 0, tong_dv, tong_sp;
    public Int64 tong_chitieu = 0, tong_congno = 0;
    public List<string> list_id_split_ghichu, list_id_split_donthuoc, list_id_split_hinhanh, list_id_split_thedv;
    public string act_hinhanh;

    //thẻ dịch vụ
    public Int64 sl_thedv = 0, doanhso_tdv = 0, doanhso_tdv_sauck = 00, tongtien_dathanhtoan_tdv = 0, tong_congno_tdv = 0;
    public int stt_tdv = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user_home_webcon"] == null) Session["user_home_webcon"] = ""; if (Session["notifi_home"] == null) Session["notifi_home"] = "";
        user = Session["user_home_webcon"].ToString();
        user_parent = AhaShineContext_cl.UserParent;

        if (user == "")
            Response.Redirect("/");

        string _id_chinhanh = AhaShineContext_cl.ResolveChiNhanhId();
        var q_1 = db.bspa_data_khachhang_tables.Where(p => p.sdt.ToString() == Session["user_home_webcon"].ToString() && p.id_chinhanh == _id_chinhanh).OrderByDescending(p => p.ngaytao);
        if (q_1.Count() == 0)
        {
            Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Tài khoản khách hàng không còn hợp lệ.", "false", "false", "OK", "alert", "");
            Response.Redirect("/");
            return;
        }

        id = q_1.First().id.ToString();
        id_chinhanh_khachhang = q_1.First().id_chinhanh;

        var q = db.bspa_data_khachhang_tables.Where(p => p.id.ToString() == id);
        if (q.Count() != 0)
        {
            main();
            show_hoadon();
            show_lichhen();
            show_ghichu();
            show_hinhanh();
            show_donthuoc();
            show_thedichvu();

            if (!string.IsNullOrWhiteSpace(Request.QueryString["act"]))
            {
                string _act = Request.QueryString["act"].ToString().Trim();
                switch (_act)
                {
                    case ("hinhanh"): act_hinhanh = "active"; break;
                }
            }
        }
        else
        {
            Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
            Response.Redirect("/default.aspx");
        }

    }
    public string check_hsd(string _hsd)
    {
        if (DateTime.Now.Date > DateTime.Parse(_hsd).Date)
            return "<span class='fg-red'>" + _hsd + "</span>";
        else
            return _hsd;
    }

    public void main()
    {
        bspa_data_khachhang_table _ob = db.bspa_data_khachhang_tables.Where(p => p.id.ToString() == id).First();
        hoten = _ob.tenkhachhang;
        sdt = _ob.sdt;
        id_chinhanh_khachhang = _ob.id_chinhanh;
        if (!IsPostBack)
        {

            txt_hoten.Text = hoten;
            txt_dienthoai.Text = sdt;
            txt_ngaysinh.Text = _ob.ngaysinh != null ? _ob.ngaysinh.Value.ToString("dd/MM/yyyy") : "";
            txt_diachi.Text = _ob.diachi;
            txt_magioithieu.Text = _ob.magioithieu;
            lb_nv_chamsoc.Text = tk_cl.return_hoten(_ob.nguoichamsoc);

            //var list_nhanvien = (from ob1 in db.taikhoan_table_2023s.Where(p => p.trangthai == "Đang hoạt động").ToList()
            //                     select new { username = ob1.taikhoan, tennhanvien = ob1.hoten, }
            //                );
            //ddl_nhanvien_chamsoc.DataSource = list_nhanvien;
            //ddl_nhanvien_chamsoc.DataTextField = "tennhanvien";
            //ddl_nhanvien_chamsoc.DataValueField = "username";
            //ddl_nhanvien_chamsoc.DataBind();
            //ddl_nhanvien_chamsoc.Items.Insert(0, new ListItem("Chọn nhân viên", ""));
            //if (_ob.nguoichamsoc != "")
            //    ddl_nhanvien_chamsoc.SelectedIndex = tk_cl.return_index(_ob.nguoichamsoc);


            //var list_nohmkhachhang = (from ob1 in db.nhomkhachhang_tables.ToList()
            //                          select new { id = ob1.id, tennhom = ob1.tennhom, }
            //          );
            //DropDownList1.DataSource = list_nohmkhachhang;
            //DropDownList1.DataTextField = "tennhom";
            //DropDownList1.DataValueField = "id";
            //DropDownList1.DataBind();
            //DropDownList1.Items.Insert(0, new ListItem("Chọn", ""));
            //if (_ob.nhomkhachhang != "")
            //    DropDownList1.SelectedIndex = dt_kh_cl.return_index(_ob.nhomkhachhang);


            //ngaytao = _ob.ngaytao.Value.ToString("dd/MM/yyyy");
            //nguoitao = _ob.nguoitao;


        }
        if (_ob.anhdaidien != "")
        {
            Button2.Visible = true;
            Label1.Text = "<img src='" + _ob.anhdaidien + "' class='img-cover-vuongtron' width='100' height='100' />";
        }
        else
        {
            Label1.Text = "<img src='/uploads/images/macdinh.jpg' class='img-cover-vuongtron' width='100' height='100' />";
            Button2.Visible = false;
        }

    }


    public void show_thedichvu()
    {
        var q = (from ob1 in db.thedichvu_tables.Where(p => p.sdt == sdt).ToList()
                     //join ob2 in db.web_menu_tables.ToList() on bv.id_category equals mn.id.ToString()
                 select new
                 {
                     id = ob1.id,
                     tenkhachhang = ob1.tenkh,
                     sdt = ob1.sdt,
                     ngaytao = ob1.ngaytao,
                     tenthe = ob1.tenthe,
                     tendv = ob1.ten_taithoidiemnay,
                     hsd = ob1.hsd,
                     sobuoi = ob1.tongsoluong,
                     sl_dalam = ob1.sl_dalam,
                     sl_conlai = ob1.sl_conlai,
                     tongtien = ob1.tongtien,
                     ck_hoadon = ob1.chietkhau,
                     tongtien_ck = ob1.tongtien_ck_hoadon,
                     tongsauchietkhau = ob1.tongsauchietkhau,
                     sotien_dathanhtoan = ob1.sotien_dathanhtoan,
                     sotien_conlai = ob1.sotien_conlai,
                     phantramchot = ob1.phantram_chotsale,
                     tongtien_chot = ob1.tongtien_chotsale_dvsp,
                     tennguoichot = tk_cl.exist_user_of_userparent(ob1.nguoichotsale, user_parent) == false ? ob1.nguoichotsale : "<div><a class='fg-black' href='/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + ob1.nguoichotsale + "'>" + tk_cl.return_object(ob1.nguoichotsale).hoten + "</a></div>",
                 }).ToList();
        q = q.OrderByDescending(p => p.ngaytao.Value).ToList();
        var list_split_thedv = q.ToList();
        list_id_split_thedv = new List<string>();
        foreach (var t in list_split_thedv)
        {
            list_id_split_thedv.Add("check_thedv_" + t.id);
        }
        if (q.Count() != 0)
        {
            sl_thedv = q.Count();
            doanhso_tdv = q.Sum(p => p.tongtien).Value;
            doanhso_tdv_sauck = q.Sum(p => p.tongsauchietkhau).Value;
            tongtien_dathanhtoan_tdv = q.Sum(p => p.sotien_dathanhtoan).Value;
            tong_congno_tdv = q.Sum(p => p.sotien_conlai).Value;


            Repeater7.DataSource = q;
            Repeater7.DataBind();
        }
    }

    public void show_lichhen()
    {
        var q = from ob1 in db.bspa_datlich_tables.Where(p => p.sdt == sdt && p.id_chinhanh == id_chinhanh_khachhang).ToList()
                select new
                {
                    id = ob1.id,
                    ngaydat = ob1.ngaydat,
                    tenkhachhang = ob1.tenkhachhang,
                    sdt = ob1.sdt,
                    nguoitao = ob1.nguoitao,
                    dichvu = ob1.dichvu,
                    tendv = ob1.tendichvu_taithoidiemnay,
                    nhanvien_thuchien = tk_cl.exist_user_of_userparent(ob1.nhanvien_thuchien, user_parent) == false ? ob1.nhanvien_thuchien : "<div><a class='fg-black' href='/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + ob1.nhanvien_thuchien + "'>" + tk_cl.return_object(ob1.nhanvien_thuchien).hoten + "</a></div>",
                    ghichu = ob1.ghichu,
                    trangthai = ob1.trangthai,
                    ngaytao = ob1.ngaytao,
                };
        Repeater3.DataSource = q.OrderByDescending(p => p.ngaydat);
        Repeater3.DataBind();
    }

    public void show_ghichu()
    {
        var list_ghichu_hoadon = (from ob1 in db.bspa_hoadon_tables.Where(p => p.sdt == sdt && p.ghichu != "")
                                      //join ob2 in db.bspa_hoadon_chitiet_tables.ToList() on ob1.id.ToString() equals ob2.id_hoadon
                                  select new
                                  {
                                      id = ob1.id,
                                      ghichu = ob1.ghichu,
                                      ngaytao = ob1.ngaytao,
                                      nguoitao = ob1.nguoitao,
                                      kyhieu = "hoadon",
                                      id_hoadon = ob1.id.ToString(),
                                  });
        var list_ghichu_db = (from ob1 in db.ghichu_khachhang_tables.Where(p => p.sdt == sdt)
                                  //join ob2 in db.bspa_hoadon_chitiet_tables.ToList() on ob1.id.ToString() equals ob2.id_hoadon
                              select new
                              {
                                  id = ob1.id,
                                  ghichu = ob1.ghichu,
                                  ngaytao = ob1.ngaytao,
                                  nguoitao = ob1.nguoitao,
                                  kyhieu = "table",
                                  id_hoadon = "",
                              });
        var q = list_ghichu_hoadon.Union(list_ghichu_db);
        q = q.OrderByDescending(p => p.ngaytao);

        list_id_split_ghichu = new List<string>();
        foreach (var t in q)
        {
            list_id_split_ghichu.Add("ghichu_" + t.id);
        }

        Repeater4.DataSource = q;
        Repeater4.DataBind();

    }

    public void show_donthuoc()
    {
        var q = (from ob1 in db.donthuoc_khachhang_tables.Where(p => p.sdt == sdt)
                     //join ob2 in db.bspa_hoadon_chitiet_tables.ToList() on ob1.id.ToString() equals ob2.id_hoadon
                 select new
                 {
                     id = ob1.id,
                     ghichu = ob1.ghichu,
                     ngaytao = ob1.ngaytao,
                     nguoitao = ob1.nguoitao,
                     noitaikham = ob1.noitaikham,
                     loidan = ob1.loidanbacsi,
                     ngaytaikham = ob1.ngaytaikham,
                 });
        q = q.OrderByDescending(p => p.ngaytao);

        list_id_split_donthuoc = new List<string>();
        foreach (var t in q)
        {
            list_id_split_donthuoc.Add("donthuoc_" + t.id);
        }

        Repeater5.DataSource = q;
        Repeater5.DataBind();

    }
    public void show_hinhanh()
    {
        var q = (from ob1 in db.hinhanh_truocsau_khachhang_tables.Where(p => p.sdt == sdt)
                     //join ob2 in db.bspa_hoadon_chitiet_tables.ToList() on ob1.id.ToString() equals ob2.id_hoadon
                 select new
                 {
                     id = ob1.id,
                     ghichu = ob1.ghichu,
                     ngaytao = ob1.ngaytao,
                     nguoitao = ob1.nguoitao,
                     anhtruoc = ob1.hinhanh_truoc == "" ? "" : "<img src='" + ob1.hinhanh_truoc + "' class='img-cover-vuong w-h-100' />",
                     anhsau = ob1.hinhanh_sau == "" ? "" : "<img src='" + ob1.hinhanh_sau + "' class='img-cover-vuong w-h-100' />",
                 });
        q = q.OrderByDescending(p => p.ngaytao);

        list_id_split_hinhanh = new List<string>();
        foreach (var t in q)
        {
            list_id_split_hinhanh.Add("hinhanh_" + t.id);
        }

        Repeater6.DataSource = q;
        Repeater6.DataBind();

    }
    public void show_hoadon()
    {
        var q = db.bspa_hoadon_tables.Where(p => p.sdt == sdt);
        if (q.Count() != 0)
        {
            tong_hoadon = q.Count();
            tong_dv = q.Sum(p => p.sl_dichvu).Value;
            tong_sp = q.Sum(p => p.sl_sanpham).Value;
            tong_chitieu = q.Sum(p => p.tongsauchietkhau).Value;
            tong_congno = q.Sum(p => p.sotien_conlai).Value;

            q = q.OrderByDescending(p => p.ngaytao);

            Repeater1.DataSource = q;
            Repeater1.DataBind();
        }
    }

    public List<bspa_hoadon_chitiet_table> show_chitiet_hoadon(string _idhd)
    {
        var q = db.bspa_hoadon_chitiet_tables.Where(p => p.id_hoadon == _idhd).OrderByDescending(p => p.ngaytao).ToList();
        if (q.Count() != 0)
            return q;
        return null;
    }

    public List<bspa_hoadon_chitiet_table> show_chitiet_thedv(string _id_tdv)
    {
        var q = db.bspa_hoadon_chitiet_tables.Where(p => p.id_thedichvu == _id_tdv).OrderBy(p => p.ngaytao).ToList();
        if (q.Count() != 0)
            return q;
        return null;
    }

    public string return_ten_nhanvien(string _user)
    {
        return datlich_class.return_ten_nguoitao_hienthi(_user);
    }





    protected void button1_Click(object sender, EventArgs e)
    {
        bspa_data_khachhang_table _ob = db.bspa_data_khachhang_tables.Where(p => p.id.ToString() == id).First();
        string _sdt_old = _ob.sdt;
        string _tenkhachhang = str_cl.VietHoa_ChuCai_DauTien(txt_hoten.Text.Trim().ToLower());
        string _sdt = str_cl.remove_blank(txt_dienthoai.Text.Trim()).Replace(" ", "").Replace(".", "").Replace("-", "").Replace("+", "");
        string _diachi = txt_diachi.Text;
        if (_tenkhachhang == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập tên khách hàng.", "false", "false", "OK", "alert", ""), true);
        else
        {
            if (_sdt == "")
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập số điện thoại khách hàng.", "false", "false", "OK", "alert", ""), true);
            else
            {
                if (bcorn_class.exist_sdt_old_data_kh(_sdt_old, _sdt) && _sdt != _sdt_old)
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Email này đã được đăng ký cho một tài khoản khác.", "false", "false", "OK", "alert", ""), true);
                else
                {
                    var q_data = db.bspa_data_khachhang_tables.Where(p => p.sdt == _sdt);

                    bspa_data_khachhang_table _ob1 = q_data.First();

                    bool _checkloi = false;
                    string _avt = "";
                    _avt = _ob1.anhdaidien;

                    if (FileUpload2.HasFile)//nếu có ảnh thu nhỏ đc chọn
                    {
                        string _ext = Path.GetExtension(FileUpload2.FileName).ToLower();
                        if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
                        {
                            //byte - kb - mb  ContentLength trra về byte của file
                            long _filesize = (FileUpload2.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                            if (_filesize <= 1) //>1MB
                            {
                                if (_ob1.anhdaidien != "/uploads/images/macdinh.jpg")
                                    file_folder_class.del_file(_ob1.anhdaidien);//xóa ảnh cũ
                                _avt = "/uploads/images/khach-hang/" + Guid.NewGuid() + _ext;
                                FileUpload2.SaveAs(Server.MapPath("~" + _avt));//lưu ảnh mới
                            }
                            else
                            {
                                notifi = thongbao_class.metro_dialog_onload("Thông báo", "Kích thước ảnh đại diện quá lớn. Vui lòng chọn ảnh có dung lượng <1Mb.", "false", "false", "OK", "alert", "");
                                _checkloi = true;
                            }
                        }
                        else
                        {
                            notifi = thongbao_class.metro_dialog_onload("Thông báo", "Ảnh đại diện không đúng định dạng.", "false", "false", "OK", "alert", "");
                            _checkloi = true;
                        }
                    }
                    _ob1.anhdaidien = _avt;
                    _ob1.tenkhachhang = _tenkhachhang;
                    _ob1.diachi = _diachi;
                    _ob1.magioithieu = txt_magioithieu.Text.Trim();
                    //_ob1.ngaytao = DateTime.Now;
                    //_ob1.nguoitao = user;
                    //_ob1.user_parent = user_parent;
                    //_ob1.nguoichamsoc = ddl_nhanvien_chamsoc.SelectedValue.ToString();
                    _ob1.sdt = _sdt;
                    //_ob1.nhomkhachhang = DropDownList1.SelectedValue.ToString();


                    if (txt_ngaysinh.Text != "" && dt_cl.check_date(txt_ngaysinh.Text) == true)
                        _ob1.ngaysinh = DateTime.Parse(txt_ngaysinh.Text);

                    if (_checkloi == false)
                    {
                        db.SubmitChanges();
                        Session["notifi_home"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "4000", "warning");
                        Response.Redirect("/gianhang/webcon/ho-so.aspx?tkchinhanh=" + Session["ten_tk_chinhanh"].ToString());
                    }
                }
            }
        }
    }
    protected void Button2_Click(object sender, EventArgs e)
    {

            bspa_data_khachhang_table _ob = db.bspa_data_khachhang_tables.Where(p => p.id.ToString() == id).First();
            if (_ob.anhdaidien != "/uploads/images/macdinh.jpg")
            {
                file_folder_class.del_file(_ob.anhdaidien);//xóa ảnh cũ
                _ob.anhdaidien = "/uploads/images/macdinh.jpg";
            }
            db.SubmitChanges();
            main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa ảnh đại diện thành công.", "4000", "warning"), true);
        
    }

}
