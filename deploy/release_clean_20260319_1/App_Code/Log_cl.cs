using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Log_cl
{
    public static void Add_Log(string _log_mes, string _log_user, string _log_line)
    {
        //try
        //{
        //    using (var db = new dbDataContext())
        //    {
        //        string _url = HttpContext.Current.Request.Url.PathAndQuery;
        //        bool logExists = db.Log_tbs.Any(l => l.log_mes == _log_mes && l.log_user == _log_user && l.log_url == _url && l.log_line == _log_line && l.trangthai != "Đã sửa");
        //        //nếu chưa có lỗi này thì thêm mới
        //        if (!logExists)
        //        {
        //            var log = new Log_tb
        //            {
        //                log_mes = _log_mes,
        //                log_date = AhaTime_cl.Now,
        //                log_user = _log_user,
        //                log_url = _url,
        //                trangthai = "Chưa sửa",
        //                log_line = _log_line,
        //                bin = false,
        //                user_fix = "",
        //                mota_loi = "",
        //            };
        //            db.Log_tbs.InsertOnSubmit(log);
        //            db.SubmitChanges();

        //            #region GỬI THÔNG BÁO TRÊN APP
        //            var tb = new ThongBao_tb
        //            {
        //                id = Guid.NewGuid(),
        //                bin = false,
        //                nguoithongbao = "admin",
        //                nguoinhan = "admin",
        //                link = "/admin/quan-ly-he-thong/log.aspx",
        //                thoigian = AhaTime_cl.Now,
        //                noidung = "Có lỗi từ hệ thống của bạn. ID: " + log.id,
        //                daxem = false,
        //            };
        //            db.ThongBao_tbs.InsertOnSubmit(tb);
        //            #endregion
        //            db.SubmitChanges();

        //            #region THÔNG BÁO QUA EMAIL
        //            // Lấy danh sách email từ bảng taikhoan_tb
        //            var emailAddresses = db.taikhoan_tbs
        //                .Where(tk => tk.taikhoan == "admin" /*|| tk.username == "bonbap"*/)
        //                .Select(tk => tk.email)
        //                .ToList();

        //            string _tenmien = HttpContext.Current.Request.Url.Host.ToUpper();
        //            string _tieude = "Có lỗi từ hệ thống của bạn";
        //            string _noidung = "<div>ID lỗi: " + log.id + "</div>";
        //            _noidung = _noidung + "<div>" + _tenmien + _url + "</div>" + "<div>" + _log_mes + "</div>" + "<div>" + _log_line + "</div>";
        //            string _ten_nguoigui = _tenmien;
        //            string _link_dinhkem = "";

        //            //gán mail trực tiếp
        //            //List<string> emailAddresses = new List<string>
        //            //{
        //            //    "ahasale.vn@gmail.com","mail khác",...
        //            //};
        //            foreach (var _email_nhan in emailAddresses)
        //            {
        //                guiEmail_cl.SendEmail(_email_nhan, _tieude, _noidung, _ten_nguoigui, _link_dinhkem);
        //            }
        //            #endregion
        //        }
        //    }
        //}
        //catch (Exception _ex)
        //{
        //    string _tk = HttpContext.Current.Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
        //    if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
        //    {
        //        _tk = mahoa_cl.giaima_Bcorn(_tk);
        //    }
        //    Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        //}
    }


}
