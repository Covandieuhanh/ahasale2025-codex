<%@ Page Title="Vay mua nhà" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerBatDongSan.master" AutoEventWireup="true" CodeFile="vay-mua-nha.aspx.cs" Inherits="bat_dong_san_vay_mua_nha" %>
<%@ Register Src="~/bat-dong-san/uc-bds-nav.ascx" TagPrefix="uc" TagName="BdsNav" %>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="page-header d-print-none">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">AhaSale / Bất động sản / Vay mua nhà</div>
                    <h2 class="page-title">Vay mua nhà</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="mb-3">
                <uc:BdsNav ID="BdsNavMain" runat="server" Current="loan" ShowTopbar="false" IncludeAssets="false" ShowBanner="false" ShowNav="false" />
            </div>

            <div class="card">
                <div class="card-body">
                    <div class="row g-3">
                        <div class="col-md-3">
                            <label class="form-label">Giá trị BĐS (VNĐ)</label>
                            <asp:TextBox ID="txtValue" runat="server" CssClass="form-control" Text="3000000000" oninput="format_sotien_new(this)"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">Vốn tự có (%)</label>
                            <asp:TextBox ID="txtDownPercent" runat="server" CssClass="form-control" Text="30"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">Lãi suất (%/năm)</label>
                            <asp:TextBox ID="txtRate" runat="server" CssClass="form-control" Text="9"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">Thời hạn (năm)</label>
                            <asp:TextBox ID="txtYears" runat="server" CssClass="form-control" Text="20"></asp:TextBox>
                        </div>
                        <div class="col-12">
                            <asp:LinkButton ID="btnCalc" runat="server" CssClass="btn btn-success" OnClick="btnCalc_Click">Tính khoản vay</asp:LinkButton>
                            <a class="btn btn-outline-success" href="/bat-dong-san/mua-ban.aspx">Chọn tin mua bán</a>
                            <a class="btn btn-outline-secondary" href="/bat-dong-san/tham-khao-gia.aspx">Tham khảo giá</a>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row row-cards">
                <div class="col-md-4">
                    <div class="card">
                        <div class="card-body">
                            <div class="text-muted small">Số tiền vay</div>
                            <div class="h2 fw-bold mb-0"><asp:Label ID="lbLoanAmount" runat="server" Text="-"></asp:Label></div>
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="card">
                        <div class="card-body">
                            <div class="text-muted small">Trả hàng tháng</div>
                            <div class="h2 fw-bold mb-0"><asp:Label ID="lbMonthly" runat="server" Text="-"></asp:Label></div>
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="card">
                        <div class="card-body">
                            <div class="text-muted small">Tổng lãi dự kiến</div>
                            <div class="h2 fw-bold mb-0"><asp:Label ID="lbTotalInterest" runat="server" Text="-"></asp:Label></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
