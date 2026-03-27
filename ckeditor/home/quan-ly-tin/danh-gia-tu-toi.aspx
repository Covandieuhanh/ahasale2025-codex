<%@ Page Title="Đánh giá từ tôi"
    Language="C#"
    MasterPageFile="~/MasterPage/Tabler/TablerHome.master"
    AutoEventWireup="true"
    CodeFile="danh-gia-tu-toi.aspx.cs"
    Inherits="home_danh_gia_tu_toi" %>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="server">

    <asp:UpdatePanel ID="upMain" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="container py-4" style="max-width: 992px">

                <h3 class="mb-4 text-primary">Đánh giá từ tôi</h3>

                <!-- ===== LIST ĐÁNH GIÁ ===== -->
                <asp:Repeater ID="rptDanhGiaCuaToi" runat="server">
                    <ItemTemplate>

                        <div class="card mb-4 shadow-sm">
                            <div class="card-body">

                                <div class="row">

                                    <!-- ===== CỘT TRÁI: SẢN PHẨM ===== -->
                                    <div class="col-lg-5 border-end">

                                        <img src="<%# Eval("AnhChinh") %>"
                                             class="img-fluid rounded mb-2"
                                             style="max-height:220px;object-fit:cover;width:100%" />

                                        <div class="fw-bold mt-2">
                                            <%# Eval("TenSanPham") %>
                                        </div>

                                        <div class="text-danger fw-bold mt-1">
                                            <%# string.Format("{0:N0}", Eval("Gia")) %> đ
                                        </div>

                                    </div>

                                    <!-- ===== CỘT PHẢI: ĐÁNH GIÁ ===== -->
                                    <div class="col-lg-7 ps-lg-4 mt-3 mt-lg-0">

                                        <div class="mb-1" style="color:#ffc107;font-size:18px">
                                            <%# new string('★', Convert.ToInt32(Eval("Diem"))) %>
                                        </div>

                                        <div class="text-muted small mb-2">
                                            <%# Eval("NgayDang","{0:dd/MM/yyyy HH:mm}") %>
                                        </div>

                                        <div class="mb-3">
                                            <%# Eval("NoiDung") %>
                                        </div>

                                        <asp:Image runat="server"
                                            ImageUrl='<%# Eval("UrlAnh") %>'
                                            Visible='<%# !string.IsNullOrEmpty(Eval("UrlAnh") as string) %>'
                                            CssClass="img-thumbnail"
                                            Width="140" />

                                    </div>

                                </div>

                            </div>
                        </div>

                    </ItemTemplate>
                </asp:Repeater>

                <!-- ===== KHÔNG CÓ DỮ LIỆU ===== -->
                <asp:PlaceHolder ID="phEmpty" runat="server" Visible="false">
                    <div class="text-center text-muted mt-5">
                        Bạn chưa đánh giá sản phẩm nào.
                    </div>
                </asp:PlaceHolder>

            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- ===== LOADING ===== -->
    <asp:UpdateProgress ID="UpdateProgress1"
        runat="server"
        AssociatedUpdatePanelID="upMain">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100"
                 style="opacity:0.85;z-index:99999">
                <div class="d-flex justify-content-center align-items-center h-100">
                    <div class="spinner-border text-light"></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>
