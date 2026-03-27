<%@ Page Title="Trung tâm quản lý đấu giá" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="portal.aspx.cs" Inherits="daugia_admin_portal" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .daugia-admin-portal-page {
            padding-bottom: 16px;
        }

        .daugia-portal-title {
            min-height: 0;
        }

        .daugia-portal-card {
            overflow: hidden;
            transition: transform .18s ease, box-shadow .18s ease, border-color .18s ease;
        }

        .daugia-portal-card .card-header {
            background: linear-gradient(180deg, rgba(22,163,74,.06) 0%, rgba(255,255,255,0) 100%);
        }

        .daugia-portal-card .card-body {
            display: flex;
            flex-direction: column;
            gap: 14px;
        }

        .daugia-mobile-btns {
            width: 100%;
            display: flex;
            flex-direction: column;
            gap: 8px;
        }

        .daugia-mobile-btns .btn {
            width: 100%;
            justify-content: center;
        }

        @media (max-width: 767.98px) {
            .page-header .page-title {
                font-size: 1.35rem;
            }
        }

        @media (min-width: 768px) {
            .daugia-admin-portal-page {
                padding-bottom: 24px;
            }

            .daugia-mobile-btns {
                width: auto;
                flex-direction: row;
                flex-wrap: wrap;
            }

            .daugia-mobile-btns .btn {
                width: auto;
            }

            .daugia-portal-card:hover {
                transform: translateY(-3px);
                border-color: rgba(21,128,61,.2);
                box-shadow: 0 18px 34px rgba(16,42,67,.12);
            }
        }

        @media (min-width: 992px) {
            .daugia-portal-title {
                min-height: 30px;
            }
        }

        @media (min-width: 1400px) {
            .daugia-admin-portal-header .container-xl,
            .daugia-admin-portal-page .container-xl {
                max-width: 1440px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="page-header d-print-none daugia-admin-portal-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">AhaSale / Đấu giá / Admin Space</div>
                    <h2 class="page-title">Trung tâm quản lý đấu giá</h2>
                    <div class="text-muted">Toàn bộ thao tác quản lý của module đấu giá được tách trong không gian `/daugia/admin/*`.</div>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body daugia-admin-portal-page">
        <div class="container-xl">
            <div class="row row-cards">
                <asp:PlaceHolder ID="phSeller" runat="server" Visible="false">
                    <div class="col-lg-6">
                        <section class="card h-100 daugia-portal-card">
                            <div class="card-header">
                                <h3 class="card-title mb-0 daugia-portal-title">Quản lý phiên của tôi</h3>
                            </div>
                            <div class="card-body">
                                <div class="text-muted mb-3">Tài khoản: <strong><%=SellerAccount %></strong></div>
                                <div class="btn-list daugia-mobile-btns">
                                    <a href="/daugia/admin/quan-ly" class="btn btn-primary btn-sm">Danh sách phiên</a>
                                    <a href="/daugia/admin/tao" class="btn btn-success btn-sm">Tạo phiên mới</a>
                                    <a href="/daugia" class="btn btn-outline-dark btn-sm">Xem khu public</a>
                                </div>
                            </div>
                        </section>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="phSystemAdmin" runat="server" Visible="false">
                    <div class="col-lg-6">
                        <section class="card h-100 daugia-portal-card">
                            <div class="card-header">
                                <h3 class="card-title mb-0 daugia-portal-title">Quản trị hệ thống</h3>
                            </div>
                            <div class="card-body">
                                <div class="text-muted mb-3">Admin: <strong><%=AdminAccount %></strong></div>
                                <div class="btn-list daugia-mobile-btns">
                                    <a href="/admin/quan-ly-dau-gia" class="btn btn-outline-success btn-sm">Quản trị nghiệp vụ</a>
                                </div>
                            </div>
                        </section>
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>
    </div>
</asp:Content>
