<%@ Page Title="Chi tiết tài khoản" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="tai-khoan.aspx.cs" Inherits="gianhang_taikhoan_detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .home-account-search-box {
            position: relative;
        }

        .home-account-search-result {
            position: absolute;
            top: calc(100% + 6px);
            left: 0;
            right: 0;
            z-index: 30;
            background: #fff;
            border: 1px solid #d7e3f4;
            border-radius: 14px;
            box-shadow: 0 18px 40px rgba(13, 37, 63, 0.12);
            overflow: hidden;
            display: none;
        }

        .home-account-search-item {
            width: 100%;
            display: block;
            text-align: left;
            padding: 12px 14px;
            border: 0;
            background: #fff;
            cursor: pointer;
        }

        .home-account-search-item + .home-account-search-item {
            border-top: 1px solid #eef3fb;
        }

        .home-account-search-item:hover {
            background: #f7fbff;
        }

        .home-account-search-meta {
            display: block;
            margin-top: 4px;
            color: #6b7b90;
            font-size: 12px;
        }

        .home-account-search-empty {
            padding: 12px 14px;
            color: #6b7b90;
            font-size: 13px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <div id="main-content" class=" mb-10">
        <div class="row border-bottom bd-light">
            <div class="cell-6">
                <ul class="h-menu">
                    <li data-role="hint" data-hint-position="top" data-hint-text="Quay lại">
                        <a class="button" href="/gianhang/admin/quan-ly-tai-khoan/Default.aspx"><span class="mif mif-arrow-left"></span></a></li>
                    <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                </ul>
            </div>
            <div class="cell-6">
            </div>
        </div>
        <div class="row">
            <div class="cell-12">
                <div class="mt-5">
                    Chi tiết tài khoản:
                        <label class="fw-600"><%=user %></label>
                </div>
                <div class="text-center">
                    <asp:Label ID="Label2" runat="server" Text=""></asp:Label>

                    <%if (trangthai == "Đang hoạt động")
                        { %>
                    <div class="text-bold mt-1">
                        <%=hoten %>
                    </div>
                    <div>
                        <span class="data-wrapper"><code class="bg-cyan fg-white">Đang hoạt động</code></span>
                    </div>
                    <%}
                        else
                        { %>
                    <div class="text-bold mt-1">
                        <span class="fg-red"><%=hoten %></span>
                    </div>
                    <div>
                        <span class="data-wrapper"><code class="bg-red fg-white">Đã bị khóa</code></span>
                    </div>
                    <%} %>

                    <div class="mt-2">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <%if (bcorn_class.check_quyen(Session["user"].ToString(), "q2_3") == ""||bcorn_class.check_quyen(Session["user"].ToString(), "n2_3") == "")// ="": có quyền; =2: k có quyền
                                    { %>
                                <a href="/gianhang/admin/quan-ly-tai-khoan/doi-mat-khau.aspx?user=<%=user %>" class="button dark mt-1">Đổi mật khẩu</a>
                                <a href="/gianhang/admin/quan-ly-tai-khoan/edit.aspx?user=<%=user %>" class="button success mt-1">Chỉnh sửa</a>
                                <%} %>
                                <%if (bcorn_class.check_quyen(Session["user"].ToString(), "q2_1") == ""||bcorn_class.check_quyen(Session["user"].ToString(), "n2_1") == "")// ="": có quyền; =2: k có quyền
                                    { %>
                                <a href="/gianhang/admin/quan-ly-tai-khoan/phan-quyen.aspx?user=<%=user %>" class="button warning mt-1">Phân quyền</a>
                                <%} %>
                                <%if (bcorn_class.check_quyen(Session["user"].ToString(), "q2_3") == ""||bcorn_class.check_quyen(Session["user"].ToString(), "n2_3") == "")// ="": có quyền; =2: k có quyền
                                    { %>
                                <asp:Button ID="but_khoa" runat="server" Text="Khóa" CssClass="alert d-inline mt-1" OnClick="but_khoa_Click" />
                                <asp:Button ID="but_mokhoa" runat="server" Text="Mở khóa" CssClass="info d-inline mt-1" OnClick="but_mokhoa_Click" />
                                <%} %>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                            <ProgressTemplate>
                                <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                                    <div style="padding-top: 50vh;">
                                        <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                                    </div>
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </div>
                </div>
                <hr class="mt-3" />

            </div>
        </div>

        <div class="row">
            <div class="cell-lg-4 p-3-lg mt-0-lg mt-5">
                <h5>Thông tin cơ bản</h5>
                <div class="mt-3">
                    <label class="fw-600">Tài khoản</label>
                    <div>
                        <%=user %>
                    </div>
                </div>
                <div class="mt-3">
                    <label class="fw-600">Ngày tạo</label>
                    <div>
                        <%=ngaytao %>
                    </div>
                </div>
                <div class="mt-3">
                    <label class="fw-600">Người tạo</label>
                    <div>
                        <a href="/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=<%=nguoitao %>"><%=nguoitao %></a>
                    </div>
                </div>
                <div class="mt-3">
                    <label class="fw-600">Ngày sinh</label>
                    <div>
                        <%=ngaysinh %>
                    </div>
                </div>

            </div>
            <div class="cell-lg-4 p-3-lg mt-0-lg mt-5">
                <h5>Liên hệ</h5>
                <div class="mt-3">
                    <label class="fw-600">Email</label>
                    <div>
                        <%=email %>
                    </div>
                </div>
                <div class="mt-3">
                    <label class="fw-600">Điện thoại</label>
                    <div>
                        <%=sdt %>
                    </div>
                </div>
                <div class="mt-3">
                    <label class="fw-600">Zalo</label>
                    <div>
                        <%=zalo %>
                    </div>
                </div>
                <div class="mt-3">
                    <label class="fw-600">Facebook</label>
                    <div>
                        <a href="<%=facebook %>">Nhấn vào đây</a>
                    </div>
                </div>
            </div>
            <div class="cell-lg-4 p-3-lg mt-0-lg mt-5">
                <h5>Hoạt động</h5>
                <div class="mt-3">
                    <label class="fw-600">Trạng thái</label>
                    <div>
                        <%=trangthai %>
                    </div>
                </div>
               
                <div class="mt-3">
                    <label class="fw-600">Hạn sử dụng</label>
                    <div>
                        <%=hsd %>
                    </div>
                </div>

                <h5>Lương</h5>
                <div class="mt-3">
                    <label class="fw-600">Lương cơ bản</label>
                    <div>
                        <%=luongcb %>
                    </div>
                </div>
               
                <div class="mt-3">
                    <label class="fw-600">Số ngày công</label>
                    <div>
                        <%=songaycong %>
                    </div>
                </div>
            </div>
        </div>

        <hr class="mt-6" />

        <div class="row">
            <div class="cell-12 p-3-lg mt-0-lg mt-5">
                <h5>Hồ sơ người AhaSale</h5>
                <p class="fg-gray">
                    Việc gắn tài khoản Home cho con người trong `/gianhang/admin` đã được chuyển sang module trung tâm <strong>Hồ sơ người</strong>. Màn chi tiết tài khoản này chỉ còn hiển thị trạng thái và mở nhanh tới hồ sơ người theo số điện thoại.
                </p>
                <div class="mt-3">
                    <label class="fw-600">Trạng thái liên kết AhaSale</label>
                    <div>
                        <asp:Literal ID="lit_home_linked" runat="server" />
                    </div>
                </div>
                <div class="mt-3">
                    <label class="fw-600">Quyền vào /gianhang/admin</label>
                    <div>
                        <span class="data-wrapper"><code class="<%=HttpUtility.HtmlAttributeEncode(personHubAdminAccessCss) %>"><%=HttpUtility.HtmlEncode(personHubAdminAccessLabel) %></code></span>
                    </div>
                    <div class="mt-1 fg-gray"><%=HttpUtility.HtmlEncode(personHubAdminAccessNote) %></div>
                </div>
                <div class="mt-4 p-3 border" style="border-radius: 14px; border-color: #f3d6b3!important; background: #fff9f2;">
                    <div class="fw-700"><%=HttpUtility.HtmlEncode(personHubImpactTitle) %></div>
                    <div class="mt-1 fg-gray"><%=HttpUtility.HtmlEncode(personHubImpactNote) %></div>
                </div>
                <div class="mt-4">
                    <a class="button success" href="<%=personHubUrl %>">Mở hồ sơ người</a>
                </div>
                <div class="mt-3 fg-gray">
                    Khi bạn liên kết tại hồ sơ người, các hồ sơ khác cùng số điện thoại trong không gian này cũng sẽ tự nhận trạng thái liên kết tương ứng.
                </div>
                <% if (!string.IsNullOrWhiteSpace(personHubRelatedRolesHtml)) { %>
                <div class="mt-4 p-3 border bd-default bg-light" style="border-radius: 14px;">
                    <div class="fw-700">Cùng số điện thoại này còn có thêm vai trò khác</div>
                    <div class="mt-1 fg-gray">
                        Đây là các hồ sơ khác trong cùng không gian đang dùng chung số điện thoại với nhân sự này.
                    </div>
                    <div class="mt-2">
                        <%=personHubRelatedRolesHtml %>
                    </div>
                </div>
                <% } %>
            </div>
        </div>


    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>
