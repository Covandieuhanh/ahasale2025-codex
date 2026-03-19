<%@ Page Title="Phân quyền" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="phan-quyen.aspx.cs" Inherits="admin_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <div id="main-content" class=" mb-10">
        <div class="row border-bottom bd-light">
            <div class="cell-6">
                <ul class="h-menu">
                    <li data-role="hint" data-hint-position="top" data-hint-text="Quay lại">
                        <a class="button" href="<%=url_back %>"><span class="mif mif-arrow-left"></span></a></li>
                    <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                </ul>
            </div>
            <div class="cell-6">
            </div>
        </div>
        <asp:Panel ID="Panel1" runat="server" DefaultButton="Button1">
            <div class="row">
                <div class="cell-lg-12">
                    <div class="mt-3">
                        Phân quyền cho tài khoản:
                        <label class="fw-600"><%=user %></label>
                        <div>Chi nhánh: <span class="text-bold"><%=tenchinhanh %></span></div>
                        <div class="fg-red text-bold">LƯU Ý: Chỉ phân quyền mục 1 hoặc mục 2</div>
                    </div>
                </div>

                <%if (bcorn_class.check_quyen(user, "q2_1") == "")// ="": có quyền; =2: k có quyền
                    { %>
                <div class="cell-lg-6">
                    <div class="mt-3">
                        <ul data-role="treeview">
                            <li>
                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;1. TOÀN QUYỀN FULL NGÀNH" name="q9999">
                                <ul>

                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;TRANG CHỦ ADMIN" name="q0" />
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem thống kê doanh số" name="q0_1" <%=q0_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem dịch vụ nổi bật" name="q0_2" <%=q0_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem sản phẩm bán chạy" name="q0_3" <%=q0_3 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem Top làm dịch vụ" name="q0_4" <%=q0_4 %> />
                                            </li>
                                        </ul>
                                    </li>



                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;TÀI KHOẢN NHÂN VIÊN" name="q2">
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem danh sách" name="q2_4" <%=q2_4 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Tạo tài khoản" name="q2_5" <%=q2_5 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Phân quyền" name="q2_1" <%=q2_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem chi tiết" name="q2_2" <%=q2_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Chỉnh sửa" name="q2_3" <%=q2_3 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Doanh số tổng nhân viên" name="q2_6" <%=q2_6 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Doanh số nhân viên chi tiết" name="q2_10" <%=q2_10 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem bảng chấm công" name="q2_7" <%=q2_7 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Chấm công" name="q2_8" <%=q2_8 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Tính lương nhân viên" name="q2_9" <%=q2_9 %> />
                                            </li>
                                        </ul>
                                    </li>

                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;QUẢN LÝ HÓA ĐƠN" name="q7">
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem hóa đơn" name="q7_1" <%=q7_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Tạo hóa đơn" name="q7_2" <%=q7_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Chỉnh sửa hóa đơn" name="q7_3" <%=q7_3 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xóa hóa đơn" name="q7_4" <%=q7_4 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem lịch sử thanh toán" name="q7_7" <%=q7_7 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xác nhận thanh toán" name="q7_5" <%=q7_5 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xóa thanh toán" name="q7_6" <%=q7_6 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem lịch sử bán hàng" name="q7_8" <%=q7_8 %> />
                                            </li>

                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;XEM THỐNG KÊ" name="q7910">
                                                <ul>
                                                    <li>
                                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem thống kê dịch vụ" name="q7_9" <%=q7_9 %> /></li>
                                                    <li>
                                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem thống kê sản phẩm" name="q7_10" <%=q7_10 %> /></li>
                                                </ul>
                                            </li>
                                        </ul>
                                    </li>

                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;QUẢN LÝ KHÁCH HÀNG" name="q8">
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem data khách hàng" name="q8_1" <%=q8_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Thêm khách hàng" name="q8_2" <%=q8_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Sửa thông tin khách hàng" name="q8_3" <%=q8_3 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xóa khách hàng" name="q8_4" <%=q8_4 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Quản lý nhóm khách hàng" name="q8_5" <%=q8_5 %> />
                                            </li>
                                        </ul>
                                    </li>

                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;QUẢN LÝ THU CHI" name="q9">
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem danh sách thu chi" name="q9_1" <%=q9_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Tạo thu chi" name="q9_2" <%=q9_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Chỉnh sửa thu chi" name="q9_3" <%=q9_3 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xóa thu chi" name="q9_4" <%=q9_4 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Quản lý loại thu chi" name="q9_5" <%=q9_5 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Duyệt phiếu chi" name="q9_6" <%=q9_6 %> />
                                            </li>
                                        </ul>
                                    </li>

                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;QUẢN LÝ LỊCH HẸN" name="q10">
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem lịch hẹn" name="q10_1" <%=q10_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Đặt lịch hẹn" name="q10_2" <%=q10_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Sửa lịch hẹn" name="q10_3" <%=q10_3 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xóa lịch hẹn" name="q10_4" <%=q10_4 %> />
                                            </li>
                                        </ul>
                                    </li>

                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;QUẢN LÝ Kho" name="q11">
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem Kho" name="q11_1" <%=q11_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Nhập mỹ phẩm" name="q11_2" <%=q11_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem đơn nhập mỹ phẩm" name="q11_4" <%=q11_4 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Sửa đơn nhập mỹ phẩm" name="q11_5" <%=q11_5 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xóa đơn nhập mỹ phẩm" name="q11_6" <%=q11_6 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Quản lý nhà cung cấp" name="q11_7" <%=q11_7 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Thanh toán đơn nhập mỹ phẩm" name="q11_8" <%=q11_8 %> />
                                            </li>
                                        </ul>
                                    </li>

                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;QUẢN LÝ VẬT TƯ" name="q13">
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Quản lý vật tư (Thêm, sửa xóa)" name="q13_9" <%=q13_9 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem kho vật tư" name="q13_1" <%=q13_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Nhập vật tư" name="q13_2" <%=q13_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem đơn nhập vật tư" name="q13_4" <%=q13_4 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Sửa đơn nhập vật tư" name="q13_5" <%=q13_5 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xóa đơn nhập vật tư" name="q13_6" <%=q13_6 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Quản lý nhóm vật tư" name="q13_7" <%=q13_7 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Thanh toán đơn nhập vật tư" name="q13_8" <%=q13_8 %> />
                                            </li>
                                        </ul>
                                    </li>

                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;QUẢN LÝ THẺ DỊCH VỤ" name="q12">
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem các thẻ đã bán" name="q12_1" <%=q12_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Bán thẻ dịch vụ" name="q12_2" <%=q12_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Sửa thẻ" name="q12_3" <%=q12_3 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xóa thẻ" name="q12_4" <%=q12_4 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Quản lý thanh toán" name="q12_5" <%=q12_5 %> />
                                            </li>
                                        </ul>
                                    </li>

                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;QUẢN LÝ Chuyên gia" name="q15">
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem Chuyên gia" name="q15_1" <%=q15_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Thêm Chuyên gia" name="q15_2" <%=q15_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Sửa Chuyên gia" name="q15_3" <%=q15_3 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xóa Chuyên gia" name="q15_4" <%=q15_4 %> />
                                            </li>
                                        </ul>
                                    </li>

                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;QUẢN LÝ thành viên" name="q14">
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem thành viên" name="q14_1" <%=q14_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Thêm thành viên" name="q14_2" <%=q14_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Sửa thành viên" name="q14_3" <%=q14_3 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xóa thành viên" name="q14_4" <%=q14_4 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Quản lý thanh toán học phí" name="q14_5" <%=q14_5 %> />
                                            </li>
                                        </ul>
                                    </li>

                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;QUẢN LÝ HỆ THỐNG" name="q16" />
                                        <ul>
                                            <%if (Session["chinhanh"].ToString() == "1")
                                                { %>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Quản lý chi nhánh" name="q16_0" <%=q16_0 %>></li>
                                            <%} %>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Quản lý ngành" name="q16_1" <%=q16_1 %>></li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Quản lý phòng ban" name="q16_2" <%=q16_2 %>></li>
                                        </ul>
                                    </li>

                                    <%if (Session["chinhanh"].ToString() == "1")
                                        { %>
                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;CẤU HÌNH CHUNG" name="q1">
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Tạo liên kết chia sẻ" name="q1_1" <%=q1_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Nhúng mã vào website" name="q1_2" <%=q1_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Cập nhật thông tin" name="q1_3" <%=q1_3 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Link social media" name="q1_4" <%=q1_4 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Cài đặt bảo trì" name="q1_5" <%=q1_5 %> />
                                            </li>
                                        </ul>
                                    </li>
                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;QUẢN LÝ MODULE" name="q5">
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Quản lý slide ảnh" name="q5_1" <%=q5_1 %> />
                                            </li>

                                        </ul>
                                    </li>
                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;QUẢN LÝ DATA YÊU CẦU TƯ VẤN" name="q6_1" <%=q6_1 %> />
                                    </li>
                                    <%} %>

                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;QUẢN LÝ MENU" name="q3">
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem" name="q3_1" <%=q3_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Thêm" name="q3_2" <%=q3_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Sửa" name="q3_3" <%=q3_3 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xóa (vào thùng rác)" name="q3_4" <%=q3_4 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xóa vĩnh viễn" name="q3_5" <%=q3_5 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Khôi phục mục đã xóa" name="q3_6" <%=q3_6 %> />
                                            </li>
                                        </ul>
                                    </li>

                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;QUẢN LÝ BÀI VIẾT" name="q4">
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem" name="q4_1" <%=q4_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Thêm" name="q4_2" <%=q4_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Sửa" name="q4_3" <%=q4_3 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xóa (vào thùng rác)" name="q4_4" <%=q4_4 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xóa vĩnh viễn" name="q4_5" <%=q4_5 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Khôi phục mục đã xóa" name="q4_6" <%=q4_6 %> />
                                            </li>
                                            <%--<li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Sửa số lượng tồn của sản phẩm" name="q4_7" <%=q4_7 %> />
                                            </li>--%>
                                        </ul>
                                    </li>

                                </ul>
                            </li>
                        </ul>
                    </div>
                </div>
                <%} %>

                <div class="cell-lg-6">
                    <div class="mt-3">
                        <ul data-role="treeview">
                            <li>
                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;2. TOÀN QUYỀN NGÀNH CỦA MÌNH" name="q8888">
                                <ul>

                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;TRANG CHỦ ADMIN" name="n0" />
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem thống kê doanh số" name="n0_1" <%=n0_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem dịch vụ nổi bật" name="n0_2" <%=n0_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem sản phẩm bán chạy" name="n0_3" <%=n0_3 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem Top làm dịch vụ" name="n0_4" <%=n0_4 %> />
                                            </li>
                                        </ul>
                                    </li>

                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;TÀI KHOẢN NHÂN VIÊN" name="n2">
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem danh sách" name="n2_4" <%=n2_4 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Tạo tài khoản" name="n2_5" <%=n2_5 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Phân quyền" name="n2_1" <%=n2_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem chi tiết" name="n2_2" <%=n2_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Chỉnh sửa" name="n2_3" <%=n2_3 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Doanh số tổng nhân viên" name="n2_6" <%=n2_6 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Doanh số nhân viên chi tiết" name="n2_10" <%=n2_10 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem bảng chấm công" name="n2_7" <%=n2_7 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Chấm công" name="n2_8" <%=n2_8 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Tính lương nhân viên" name="n2_9" <%=n2_9 %> />
                                            </li>
                                        </ul>
                                    </li>

                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;QUẢN LÝ HÓA ĐƠN" name="n7">
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem hóa đơn" name="n7_1" <%=n7_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Tạo hóa đơn" name="n7_2" <%=n7_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Chỉnh sửa hóa đơn" name="n7_3" <%=n7_3 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xóa hóa đơn" name="n7_4" <%=n7_4 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem lịch sử thanh toán" name="n7_7" <%=n7_7 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xác nhận thanh toán" name="n7_5" <%=n7_5 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xóa thanh toán" name="n7_6" <%=n7_6 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem lịch sử bán hàng" name="n7_8" <%=n7_8 %> />
                                            </li>

                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;XEM THỐNG KÊ" name="n7910">
                                                <ul>
                                                    <li>
                                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem thống kê dịch vụ" name="n7_9" <%=n7_9 %> /></li>
                                                    <li>
                                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem thống kê sản phẩm" name="n7_10" <%=n7_10 %> /></li>
                                                </ul>
                                            </li>
                                        </ul>
                                    </li>

                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;QUẢN LÝ THU CHI" name="n9">
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem danh sách thu chi" name="n9_1" <%=n9_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Tạo thu chi" name="n9_2" <%=n9_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Chỉnh sửa thu chi" name="n9_3" <%=n9_3 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xóa thu chi" name="n9_4" <%=n9_4 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Quản lý loại thu chi" name="n9_5" <%=n9_5 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Duyệt phiếu chi" name="n9_6" <%=n9_6 %> />
                                            </li>
                                        </ul>
                                    </li>

                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;QUẢN LÝ THẺ DỊCH VỤ" name="n12">
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem các thẻ đã bán" name="n12_1" <%=n12_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Bán thẻ dịch vụ" name="n12_2" <%=n12_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Sửa thẻ" name="n12_3" <%=n12_3 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xóa thẻ" name="n12_4" <%=n12_4 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Quản lý thanh toán" name="n12_5" <%=n12_5 %> />
                                            </li>
                                        </ul>
                                    </li>

                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;QUẢN LÝ Chuyên gia" name="n15">
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem Chuyên gia" name="n15_1" <%=n15_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Thêm Chuyên gia" name="n15_2" <%=n15_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Sửa Chuyên gia" name="n15_3" <%=n15_3 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xóa Chuyên gia" name="n15_4" <%=n15_4 %> />
                                            </li>
                                        </ul>
                                    </li>

                                    <li>
                                        <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;QUẢN LÝ thành viên" name="n14">
                                        <ul>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xem thành viên" name="n14_1" <%=n14_1 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Thêm thành viên" name="n14_2" <%=n14_2 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Sửa thành viên" name="n14_3" <%=n14_3 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Xóa thành viên" name="n14_4" <%=n14_4 %> />
                                            </li>
                                            <li>
                                                <input type="checkbox" data-role="checkbox" data-style="2" data-caption="&nbsp;Quản lý thanh toán học phí" name="n14_5" <%=n14_5 %> />
                                            </li>
                                        </ul>
                                    </li>

                                </ul>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
            <div class="mt-3 text-center border-top bd-light">
                <asp:Button ID="Button1" runat="server" Text="CẬP NHẬT" CssClass="button success mt-6" OnClick="Button1_Click" />
            </div>
        </asp:Panel>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>

