<%@ Page Title="Cơ cấu /gianhang" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="tien-ich-co-cau.aspx.cs" Inherits="gianhang_admin_gianhang_tien_ich_co_cau" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-utility-form{display:grid;gap:18px;}
        .gh-utility-form__hero,.gh-utility-form__card{padding:22px;border:1px solid #efe4e5;border-radius:22px;background:#fff;box-shadow:0 16px 28px rgba(15,23,42,.05);}
        .gh-utility-form__hero{display:flex;justify-content:space-between;gap:16px;flex-wrap:wrap;background:radial-gradient(680px 220px at 0% 0%, rgba(255,111,72,.14), transparent 60%),linear-gradient(180deg,#fffdfc 0%,#fff 100%);}
        .gh-utility-form__kicker{display:inline-flex;align-items:center;min-height:30px;padding:0 12px;border-radius:999px;background:#fff3ee;border:1px solid #ffd8c7;color:#bb4d19;font-size:12px;font-weight:800;text-transform:uppercase;}
        .gh-utility-form__title{margin:10px 0 6px;color:#7f1d1d;font-size:30px;font-weight:900;line-height:1.15;}
        .gh-utility-form__sub{margin:0;color:#8d5d5d;font-size:14px;line-height:1.7;max-width:760px;}
        .gh-utility-form__actions{display:flex;gap:10px;flex-wrap:wrap;}
        .gh-utility-form__btn{display:inline-flex;align-items:center;justify-content:center;min-height:42px;padding:0 16px;border-radius:14px;text-decoration:none !important;font-size:13px;font-weight:800;border:1px solid transparent;}
        .gh-utility-form__btn--primary{background:linear-gradient(135deg,#d73a31,#ef6b41);color:#fff !important;box-shadow:0 14px 28px rgba(215,58,49,.18);}
        .gh-utility-form__btn--soft{background:#fff;color:#7f1d1d !important;border-color:#f2c9c9;}
        .gh-utility-form__grid{display:grid;grid-template-columns:minmax(0,2fr) minmax(300px,1fr);gap:16px;}
        .gh-utility-form__field label{display:block;margin-bottom:8px;color:#7f1d1d;font-size:13px;font-weight:800;}
        .gh-utility-form__input{width:100%;min-height:48px;padding:0 16px;border-radius:16px;border:1px solid #f0d3d7;background:#fff;box-shadow:inset 0 1px 2px rgba(15,23,42,.03);font-size:15px;font-weight:700;color:#1f2937;outline:none;}
        .gh-utility-form__hint{margin-top:10px;color:#6b7280;font-size:13px;line-height:1.7;}
        .gh-utility-form__footer{display:flex;gap:10px;flex-wrap:wrap;margin-top:16px;}
        .gh-utility-form__submit,.gh-utility-form__clear{appearance:none;border:0;display:inline-flex;align-items:center;justify-content:center;min-height:44px;padding:0 16px;border-radius:14px;font-size:13px;font-weight:800;cursor:pointer;}
        .gh-utility-form__submit{background:linear-gradient(135deg,#d73a31,#ef6b41);color:#fff;box-shadow:0 14px 28px rgba(215,58,49,.18);}
        .gh-utility-form__clear{background:#fff;border:1px solid #e6d6d6;color:#7f1d1d;}
        .gh-utility-form__status{margin-top:14px;padding:12px 14px;border-radius:14px;background:#fff8f3;border:1px solid #ffd8c7;color:#a8431f;font-weight:700;line-height:1.7;}
        .gh-utility-form__stat{padding:16px;border:1px solid #ebeef4;border-radius:18px;background:#f8fafc;}
        .gh-utility-form__stat small{display:block;color:#8d5d5d;font-size:12px;font-weight:700;text-transform:uppercase;}
        .gh-utility-form__stat strong{display:block;margin-top:8px;color:#111827;font-size:26px;font-weight:900;}
        .gh-utility-form__stat span{display:block;margin-top:6px;color:#6b7280;font-size:13px;line-height:1.7;}
        .gh-utility-form__list{margin:14px 0 0;padding-left:18px;color:#4b5563;line-height:1.7;}
        @media (max-width: 767px){.gh-utility-form__grid{grid-template-columns:1fr;}.gh-utility-form__title{font-size:26px;}}
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-utility-form">
        <section class="gh-utility-form__hero">
            <div>
                <div class="gh-utility-form__kicker">Tiện ích · cơ cấu</div>
                <h1 class="gh-utility-form__title">Cơ cấu kết quả cho <%=WorkspaceDisplayName %></h1>
                <p class="gh-utility-form__sub">
                    Cấu hình số thứ tự sẽ được ưu tiên khi dừng công cụ quay số của workspace này.
                    Nếu để trống hoặc tắt đi, kết quả sẽ quay ngẫu nhiên hoàn toàn.
                </p>
            </div>
            <div class="gh-utility-form__actions">
                <a class="gh-utility-form__btn gh-utility-form__btn--primary" href="<%=DrawUrl %>">Mở quay số</a>
                <a class="gh-utility-form__btn gh-utility-form__btn--soft" href="<%=HubUrl %>">Về tiện ích</a>
                <a class="gh-utility-form__btn gh-utility-form__btn--soft" href="<%=PublicConfigUrl %>" target="_blank">Mở native /gianhang</a>
            </div>
        </section>

        <div class="gh-utility-form__grid">
            <section class="gh-utility-form__card">
                <div class="gh-utility-form__field">
                    <label for="<%=txtCoCau.ClientID %>">Dòng ưu tiên khi dừng quay</label>
                    <asp:TextBox ID="txtCoCau" runat="server" CssClass="gh-utility-form__input" MaxLength="5" placeholder="Ví dụ: 3"></asp:TextBox>
                    <div class="gh-utility-form__hint">
                        Nếu danh sách quay có 10 dòng và mình nhập <strong>3</strong>, khi dừng công cụ sẽ ưu tiên chọn dòng thứ 3.
                        Giá trị phải là số nguyên dương.
                    </div>
                </div>

                <div class="gh-utility-form__footer">
                    <asp:Button ID="btnSave" runat="server" Text="Lưu cơ cấu" CssClass="gh-utility-form__submit" OnClick="btnSave_Click" />
                    <asp:Button ID="btnClear" runat="server" Text="Tắt cơ cấu" CssClass="gh-utility-form__clear" CausesValidation="false" OnClick="btnClear_Click" />
                </div>

                <asp:PlaceHolder ID="phStatus" runat="server" Visible="false">
                    <div class="gh-utility-form__status"><%=StatusMessage %></div>
                </asp:PlaceHolder>
            </section>

            <aside class="gh-utility-form__card">
                <div class="gh-utility-form__stat">
                    <small>Trạng thái hiện tại</small>
                    <strong><%=CurrentCoCauLabel %></strong>
                    <span><%=CurrentCoCauNote %></span>
                </div>
                <ul class="gh-utility-form__list">
                    <li>Cấu hình này chỉ áp dụng cho workspace hiện tại.</li>
                    <li>Công cụ quay số trong admin và native <code>/gianhang</code> sẽ cùng đọc một trạng thái cơ cấu theo owner.</li>
                    <li>Nếu chỉ số cơ cấu vượt quá số dòng thực tế, công cụ sẽ tự rơi về chế độ ngẫu nhiên.</li>
                </ul>
            </aside>
        </div>
    </div>
</asp:Content>
