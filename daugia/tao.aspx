<%@ Page Title="Tạo phiên đấu giá" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="tao.aspx.cs" Inherits="daugia_tao" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .daugia-create-shell {
            padding: 20px 0 40px;
        }

        .daugia-create-card {
            background: #fff;
            border: 1px solid rgba(15, 23, 42, .08);
            border-radius: 18px;
            padding: 16px;
        }

        .daugia-form-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 10px;
        }

        .daugia-form-grid .full {
            grid-column: 1 / -1;
        }

        @media (max-width: 900px) {
            .daugia-form-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="container-xl daugia-create-shell">
        <div class="daugia-create-card">
            <div class="text-muted small">AhaSale / Đấu giá</div>
            <h1 class="h5 mb-2">Tạo Phiên Đấu Giá Mới</h1>
            <p class="text-muted mb-3">Phiên sẽ bị giữ cọc theo cấu hình module và chuyển trạng thái chờ admin duyệt.</p>

            <div class="daugia-form-grid">
                <div class="full">
                    <label class="form-label">Tiêu đề phiên</label>
                    <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" />
                </div>

                <div class="full">
                    <label class="form-label">Mô tả</label>
                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" />
                </div>

                <div class="full">
                    <label class="form-label">Ảnh đại diện (URL)</label>
                    <asp:TextBox ID="txtImage" runat="server" CssClass="form-control" />
                </div>

                <div>
                    <label class="form-label">Loại tài sản nguồn</label>
                    <asp:DropDownList ID="ddlSourceType" runat="server" CssClass="form-select">
                        <asp:ListItem Value="shop_post">shop_post</asp:ListItem>
                        <asp:ListItem Value="home_quyen_uu_dai">home_quyen_uu_dai</asp:ListItem>
                        <asp:ListItem Value="home_quyen_tieu_dung">home_quyen_tieu_dung</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div>
                    <label class="form-label">ID tài sản nguồn</label>
                    <asp:TextBox ID="txtSourceID" runat="server" CssClass="form-control" />
                </div>

                <div>
                    <label class="form-label">Giá niêm yết (E-AHA)</label>
                    <asp:TextBox ID="txtGiaNiemYet" runat="server" CssClass="form-control" />
                </div>

                <div>
                    <label class="form-label">Phí mỗi lượt (E-AHA)</label>
                    <asp:TextBox ID="txtPhiLuot" runat="server" CssClass="form-control" />
                </div>

                <div>
                    <label class="form-label">Bắt đầu</label>
                    <asp:TextBox ID="txtStartAt" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
                </div>

                <div>
                    <label class="form-label">Kết thúc</label>
                    <asp:TextBox ID="txtEndAt" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
                </div>

                <div>
                    <label class="form-label">Settlement mode</label>
                    <asp:DropDownList ID="ddlSettlementMode" runat="server" CssClass="form-select">
                        <asp:ListItem Value="manual_fulfillment">manual_fulfillment</asp:ListItem>
                        <asp:ListItem Value="system_transfer">system_transfer</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div>
                    <label class="form-label">Scope người bán</label>
                    <asp:DropDownList ID="ddlSellerScope" runat="server" CssClass="form-select">
                        <asp:ListItem Value="shop">shop</asp:ListItem>
                        <asp:ListItem Value="home">home</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="full d-flex gap-2">
                    <asp:LinkButton ID="butCreate" runat="server" CssClass="btn btn-success" OnClick="butCreate_Click">Tạo phiên đấu giá</asp:LinkButton>
                    <a class="btn btn-outline-secondary" href="/daugia">Hủy</a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
