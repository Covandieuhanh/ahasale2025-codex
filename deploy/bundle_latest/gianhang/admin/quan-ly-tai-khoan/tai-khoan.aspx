<%@ Page Title="Chi tiết tài khoản" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="tai-khoan.aspx.cs" Inherits="gianhang_taikhoan_detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
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


    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>

