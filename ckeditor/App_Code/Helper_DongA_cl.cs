using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Helper xử lý Quyền tiêu dùng theo kiểu "blockchain-like"
/// - Giao dịch atomic (transaction)
/// - Ledger 2 chiều
/// - Validate tổng cung
/// - Anti spam
/// - Hỗ trợ: admin->tài khoản tổng, tài khoản tổng->user
/// </summary>
public class Helper_DongA_cl
{
    // ======================================================
    // ✅ CONFIG DÙNG CHUNG
    // ======================================================
    public const string GENESIS_WALLET = "admin";                  // ✅ ví gốc
    public const decimal TOTAL_SUPPLY = 1000000000m;               // ✅ tổng cung
    public const string WITHDRAW_RECEIVE_WALLET = "vitonggianhangdoitac";     // ✅ tài khoản tổng nhận khi rút
    public const int ANTI_DOUBLE_CLICK_SECONDS = 2;                // ✅ chống click nhanh

    // ✅ whitelist tài khoản tổng (dùng trong C#)
    public static readonly HashSet<string> TREASURY_WALLETS = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "vitongkhachhang",
        "vitonggianhangdoitac",
        "vitongdonghanhhesinhthai",
        "vitongcongtacphattrien"
    };

    // ✅ dạng array để query LINQ to SQL (IN)
    public static readonly string[] TREASURY_WALLETS_SQL = new string[]
    {
        "vitongkhachhang",
        "vitonggianhangdoitac",
        "vitongdonghanhhesinhthai",
        "vitongcongtacphattrien"
    };

    // ======================================================
    // ✅ MAP: tài khoản tổng -> nhóm user được phép chuyển
    // ======================================================
    public static string GetAllowedUserGroupByTreasuryWallet(string treasuryWallet)
    {
        if (treasuryWallet.Equals("vitongkhachhang", StringComparison.OrdinalIgnoreCase)) return "Khách hàng";
        if (treasuryWallet.Equals("vitonggianhangdoitac", StringComparison.OrdinalIgnoreCase)) return "Gian hàng đối tác";
        if (treasuryWallet.Equals("vitongdonghanhhesinhthai", StringComparison.OrdinalIgnoreCase)) return "Đồng hành hệ sinh thái";
        if (treasuryWallet.Equals("vitongcongtacphattrien", StringComparison.OrdinalIgnoreCase)) return "Cộng tác phát triển";
        return "";
    }

    // ======================================================
    // ✅ FORMAT
    // ======================================================
    public static string FormatMoney(decimal money)
    {
        return money.ToString("#,##0.##");
    }

    // ======================================================
    // ✅ CHỐNG DOUBLE CLICK / SPAM
    // ======================================================
    public static bool IsSpamTransfer(dbDataContext db, string from, string to, decimal amount)
    {
        DateTime now = AhaTime_cl.Now;

        return db.LichSuChuyenDiem_tbs.Any(p =>
            p.taikhoan_chuyen == from &&
            p.taikhoan_nhan == to &&
            p.dongA == amount &&
            p.nap_rut == true &&
            p.ngay >= now.AddSeconds(-ANTI_DOUBLE_CLICK_SECONDS)
        );
    }

    // ======================================================
    // ✅ LEDGER (ghi lịch sử cộng/trừ)
    // ======================================================
    public static void AddLedger(
        dbDataContext db,
        string wallet,
        decimal amount,
        bool congTru,
        string note,
        string refId,
        int loaiHoSo,
        int? kyHieuHanhVi = null)
    {
        LichSu_DongA_tb ls = new LichSu_DongA_tb();
        ls.taikhoan = wallet;
        ls.dongA = amount;
        ls.ngay = AhaTime_cl.Now;
        ls.CongTru = congTru;     // true=cộng, false=trừ
        ls.id_donhang = "";
        ls.ghichu = note;
        ls.id_rutdiem = refId;    // dùng làm TransactionRefId chung
        ls.LoaiHoSo_Vi = loaiHoSo;
        ls.KyHieu9HanhVi_1_9 = kyHieuHanhVi;
        db.LichSu_DongA_tbs.InsertOnSubmit(ls);
    }

    // ======================================================
    // ✅ THÔNG BÁO
    // ======================================================
    public static void AddNotify(dbDataContext db, string nguoithongbao, string nguoinhan, string noidung, string link)
    {
        ThongBao_tb tb = new ThongBao_tb();
        tb.id = Guid.NewGuid();
        tb.daxem = false;
        tb.nguoithongbao = nguoithongbao;
        tb.nguoinhan = nguoinhan;
        tb.link = link;
        tb.noidung = noidung;
        tb.thoigian = AhaTime_cl.Now;
        tb.bin = false;
        db.ThongBao_tbs.InsertOnSubmit(tb);
    }

    // ======================================================
    // ✅ VALIDATE TỔNG CUNG (chi tiết + top ví lớn nhất)
    // ======================================================
    public static bool ValidateTotalSupply(dbDataContext db, decimal expectedTotal, out string detail)
    {
        decimal actualTotal = db.taikhoan_tbs.Sum(p => (decimal)(p.DongA ?? 0));
        decimal diff = actualTotal - expectedTotal;

        if (actualTotal == expectedTotal)
        {
            detail = "OK";
            return true;
        }

        string status = diff > 0 ? "THỪA" : "THIẾU";

        var top = db.taikhoan_tbs
            .OrderByDescending(x => x.DongA ?? 0)
            .Take(5)
            .Select(x => new { x.taikhoan, DongA = (decimal)(x.DongA ?? 0) })
            .ToList();

        string topMsg = "";
        int i = 1;
        foreach (var item in top)
        {
            topMsg += i + ") " + item.taikhoan + ": " + item.DongA.ToString("#,##0.##") + "\n";
            i++;
        }

        detail =
            "❌ Tổng cung Quyền tiêu dùng không hợp lệ!\n\n" +
            "• Tổng cung kỳ vọng: " + expectedTotal.ToString("#,##0.##") + "\n" +
            "• Tổng cung hiện tại: " + actualTotal.ToString("#,##0.##") + "\n" +
            "• Lệch: " + diff.ToString("#,##0.##") + " (" + status + ")\n\n" +
            "🔎 Top 5 ví đang giữ nhiều nhất:\n" + topMsg + "\n" +
            "✅ Gợi ý xử lý:\n" +
            "1) Reset demo dữ liệu (xóa lịch sử + reset số dư).\n" +
            "2) Cân ví admin: admin = Tổng cung - SUM(các ví khác)\n\n" +
            "⚠️ Giao dịch đã bị hủy để bảo toàn dữ liệu.";

        return false;
    }

    // ======================================================
    // ✅ CORE 1: CHUYỂN admin -> tài khoản tổng
    // ======================================================
    public static bool TransferGenesisToTreasury(dbDataContext db, string fromWallet, string toWallet, decimal amount, out string message)
    {
        message = "";

        if (USDTBridgeConfig_cl.StrictWalletOnlyMinting)
        {
            message = "Hệ thống đang bật chế độ 'Ví blockchain là nguồn tạo điểm duy nhất'. " +
                      "Không thể chuyển điểm từ ví gốc ADMIN.";
            return false;
        }

        if (!fromWallet.Equals(GENESIS_WALLET, StringComparison.OrdinalIgnoreCase))
        {
            message = "Chỉ ADMIN được phép chuyển từ ví gốc.";
            return false;
        }

        if (!TREASURY_WALLETS.Contains(toWallet))
        {
            message = "Tài khoản nhận không hợp lệ (chỉ nhận 4 tài khoản tổng).";
            return false;
        }

        var wFrom = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == fromWallet);
        var wTo = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == toWallet);

        if (wFrom == null) { message = "Không tìm thấy ví gốc ADMIN."; return false; }
        if (wTo == null) { message = "Không tìm thấy tài khoản tổng nhận."; return false; }

        if (!AccountType_cl.IsTreasury(wTo.phanloai))
        {
            message = "Tài khoản nhận không phải tài khoản tổng.";
            return false;
        }

        decimal balanceFrom = wFrom.DongA ?? 0;
        if (balanceFrom < amount)
        {
            message = "Ví gốc không đủ Quyền tiêu dùng để chuyển.";
            return false;
        }

        wFrom.DongA = balanceFrom - amount;
        wTo.DongA = (wTo.DongA ?? 0) + amount;

        LichSuChuyenDiem_tb tx = new LichSuChuyenDiem_tb();
        tx.taikhoan_chuyen = fromWallet;
        tx.taikhoan_nhan = toWallet;
        tx.dongA = amount;
        tx.ngay = AhaTime_cl.Now;
        tx.nap_rut = true;
        tx.trangtrai_rut = "";
        db.LichSuChuyenDiem_tbs.InsertOnSubmit(tx);

        AddLedger(db, fromWallet, amount, false,
            string.Format("Trừ {0:#,##0.##} Quyền tiêu dùng (Chuyển xuống {1})", amount, wTo.hoten),
            tx.id.ToString(),1);

        AddLedger(db, toWallet, amount, true,
            string.Format("Cộng {0:#,##0.##} Quyền tiêu dùng (Nhận từ ví gốc ADMIN)", amount),
            tx.id.ToString(),1);

        AddNotify(db, fromWallet, toWallet,
            string.Format("ADMIN vừa chuyển {0:#,##0.##} Quyền tiêu dùng cho bạn.", amount),
            "/admin/lich-su-chuyen-diem/default.aspx");

        message = "OK";
        return true;
    }

    // ======================================================
    // ✅ CORE 2: CHUYỂN tài khoản tổng -> user (đúng nhóm)
    // ======================================================
    public static bool TransferTreasuryToUser(dbDataContext db, string fromWallet, string toWallet, decimal amount, out string message)
    {
        message = "";

        var wFrom = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == fromWallet);
        if (wFrom == null) { message = "Không tìm thấy ví gửi."; return false; }

        if (!AccountType_cl.IsTreasury(wFrom.phanloai))
        {
            message = "Chỉ tài khoản tổng mới được phép chuyển xuống cấp dưới.";
            return false;
        }

        // ✅ xác định nhóm user được phép nhận
        string allowedGroup = GetAllowedUserGroupByTreasuryWallet(fromWallet);
        if (allowedGroup == "")
        {
            message = "Tài khoản tổng này không hợp lệ hoặc chưa map nhóm user.";
            return false;
        }

        var wTo = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == toWallet);
        if (wTo == null) { message = "Không tìm thấy tài khoản nhận."; return false; }

        if (AccountType_cl.IsTreasury(wTo.phanloai))
        {
            message = "Không được chuyển cho tài khoản tổng khác.";
            return false;
        }

        // ✅ bắt buộc user thuộc đúng nhóm
        if (wTo.phanloai != allowedGroup)
        {
            message = "Tài khoản tổng này chỉ được chuyển cho user loại: " + allowedGroup;
            return false;
        }

        decimal balanceFrom = wFrom.DongA ?? 0;
        if (balanceFrom < amount)
        {
            message = "Tài khoản tổng không đủ Quyền tiêu dùng để chuyển.";
            return false;
        }

        wFrom.DongA = balanceFrom - amount;
        wTo.DongA = (wTo.DongA ?? 0) + amount;

        LichSuChuyenDiem_tb tx = new LichSuChuyenDiem_tb();
        tx.taikhoan_chuyen = fromWallet;
        tx.taikhoan_nhan = toWallet;
        tx.dongA = amount;
        tx.ngay = AhaTime_cl.Now;
        tx.nap_rut = true;
        tx.trangtrai_rut = "";
        db.LichSuChuyenDiem_tbs.InsertOnSubmit(tx);

        AddLedger(db, fromWallet, amount, false,
            string.Format("Trừ {0:#,##0.##} Quyền tiêu dùng (Chuyển cho {1})", amount, wTo.hoten),
            tx.id.ToString(), 1);

        AddLedger(db, toWallet, amount, true,
            string.Format("Cộng {0:#,##0.##} Quyền tiêu dùng (Nhận từ {1})", amount, wFrom.hoten),
            tx.id.ToString(), 1);

        AddNotify(db, fromWallet, toWallet,
            string.Format("{0} vừa chuyển {1:#,##0.##} Quyền tiêu dùng cho bạn.", wFrom.hoten, amount),
            "/home/lich-su-giao-dich.aspx");

        message = "OK";
        return true;
    }

    // ======================================================
    // ✅ CORE 3: XÁC NHẬN RÚT ĐIỂM
    // ======================================================
    public static bool ConfirmWithdraw(dbDataContext db, string txId, out string message)
    {
        message = "";

        var tx = db.LichSuChuyenDiem_tbs.FirstOrDefault(p => p.id.ToString() == txId);
        if (tx == null)
        {
            message = "Không tìm thấy lệnh rút.";
            return false;
        }

        if (tx.nap_rut == true)
        {
            message = "Giao dịch này không phải lệnh rút.";
            return false;
        }

        if (tx.trangtrai_rut != "Chờ xác nhận")
        {
            message = "Lệnh rút không ở trạng thái chờ xác nhận.";
            return false;
        }

        var ledUserMinus = db.LichSu_DongA_tbs.FirstOrDefault(p => p.id_rutdiem == txId && p.CongTru == false);
        if (ledUserMinus == null)
        {
            message = "Không tìm thấy lịch sử trừ điểm của lệnh rút. (Có thể lệnh rút chưa trừ điểm)";
            return false;
        }

        decimal amount = ledUserMinus.dongA ?? 0;
        if (amount <= 0)
        {
            message = "Số tiền rút không hợp lệ.";
            return false;
        }

        var wReceive = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == WITHDRAW_RECEIVE_WALLET);
        if (wReceive == null)
        {
            message = "Không tìm thấy tài khoản tổng nhận tiền rút.";
            return false;
        }

        wReceive.DongA = (wReceive.DongA ?? 0) + amount;

        AddLedger(db, WITHDRAW_RECEIVE_WALLET, amount, true,
            string.Format("Cộng {0:#,##0.##} Quyền tiêu dùng (Nhận từ lệnh rút {1})", amount, txId),
            txId,1);

        tx.trangtrai_rut = "Hoàn thành";
        ledUserMinus.ghichu = string.Format("Rút điểm thành công. ID rút: {0}", txId);

        AddNotify(db, GENESIS_WALLET, tx.taikhoan_chuyen,
            string.Format("ADMIN đã xác nhận lệnh rút điểm của bạn. ID rút: {0}", txId),
            "/home/lich-su-giao-dich.aspx");

        message = "OK";
        return true;
    }
    public static bool CancelWithdraw(dbDataContext db, string txId, out string message)
    {
        message = "";

        // 1) Tìm giao dịch rút
        var tx = db.LichSuChuyenDiem_tbs.FirstOrDefault(p => p.id.ToString() == txId);
        if (tx == null)
        {
            message = "Không tìm thấy lệnh rút.";
            return false;
        }

        // 2) Chỉ xử lý giao dịch rút
        if (tx.nap_rut == true)
        {
            message = "Giao dịch này không phải lệnh rút.";
            return false;
        }

        // 3) Chỉ cho hủy khi đang chờ xác nhận
        if (tx.trangtrai_rut != "Chờ xác nhận")
        {
            message = "Chỉ được hủy lệnh rút khi đang ở trạng thái 'Chờ xác nhận'.";
            return false;
        }

        // 4) Tìm ledger trừ của user (đã trừ trước đó lúc tạo yêu cầu rút)
        var ledUserMinus = db.LichSu_DongA_tbs.FirstOrDefault(p => p.id_rutdiem == txId && p.CongTru == false);
        if (ledUserMinus == null)
        {
            message = "Không tìm thấy lịch sử trừ điểm của lệnh rút.";
            return false;
        }

        decimal amount = ledUserMinus.dongA ?? 0m;
        if (amount <= 0)
        {
            message = "Số tiền rút không hợp lệ.";
            return false;
        }

        // 5) Lấy tài khoản user cần hoàn tiền
        string user = tx.taikhoan_chuyen;
        var wUser = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == user);
        if (wUser == null)
        {
            message = "Không tìm thấy tài khoản người rút để hoàn tiền.";
            return false;
        }

        // 6) Hoàn tiền về user (cộng lại ví)
        wUser.DongA = (wUser.DongA ?? 0m) + amount;

        // 7) Ghi ledger cộng lại (Option A)
        AddLedger(db, user, amount, true,
            string.Format("Hoàn tiền {0:#,##0.##} Quyền tiêu dùng do lệnh rút bị hủy. ID rút: {1}", amount, txId),
            txId,1);

        // 8) Update trạng thái giao dịch
        tx.trangtrai_rut = "Bị hủy";

        // 9) Update ghi chú ledger trừ ban đầu
        ledUserMinus.ghichu = string.Format("Lệnh rút bị hủy. ID rút: {0}", txId);

        // 10) Notify user
        AddNotify(db, GENESIS_WALLET, user,
            string.Format("ADMIN đã hủy lệnh rút điểm của bạn. ID rút: {0}. Số điểm đã được hoàn lại.", txId),
            "/home/lich-su-giao-dich.aspx");

        message = "OK";
        return true;
    }

}
