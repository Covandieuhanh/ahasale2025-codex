<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DanhMuc_icon_Home_UC.ascx.cs" Inherits="Uc_Home_DanhMuc_icon_Home_UC" %>
<div class="container-xl  mb-4 dm-section " style="margin-top:60px">

  <div class="card">
    <div class="card-body p-3">
      <div class="dm-grid">
        <asp:Repeater ID="Repeater1" runat="server">
          <ItemTemplate>
            <a class="dm-item" href='/<%# Eval("name_en") %>-<%# Eval("id") %>'>
              <span class="dm-img">
                <img src='<%# string.IsNullOrWhiteSpace((Eval("image") + "").Trim()) ? "/uploads/images/macdinh.jpg" : (Eval("image") + "").Trim() %>' alt="<%# Eval("name") %>" loading="lazy" />
              </span>
              <span class="dm-name"><%# Eval("name") %></span>
            </a>
          </ItemTemplate>
        </asp:Repeater>
      </div>
    </div>
  </div>
</div>
<style>
/* =========================
   DANH MỤC - Tabler style
   Desktop: vừa khít (8 cột x 2 hàng) - không dư, không overflow
   Tablet: giảm cột cho vẫn khít
   Mobile: 2 hàng + kéo ngang
========================= */

/* DESKTOP (mặc định) */
.dm-section{
  transform:translateY(-20px);
}


.dm-grid{
  display:grid;
  grid-template-columns:repeat(8, minmax(0, 1fr)); /* vừa khít như màn hình bạn */
  grid-auto-rows:auto;
  column-gap:14px;
  row-gap:14px;

  overflow:hidden;
  padding:6px 8px 10px;
}

/* ITEM */
.dm-item{
  display:flex;
  flex-direction:column;
  align-items:center;
  gap:6px;

  padding:6px 4px;
  border-radius:var(--tblr-border-radius-lg,.5rem);

  color:inherit;
  text-decoration:none !important;
  transition:background-color .15s ease, transform .15s ease;
}
.dm-item:hover{
  background:var(--tblr-bg-surface-secondary, rgba(0,0,0,.03));
}
.dm-item:active{
  transform:translateY(1px);
}

/* ICON */
.dm-img{
  width:60px;
  height:60px;
  border-radius:999px;
  background:var(--tblr-bg-surface-secondary,rgba(0,0,0,.04));
  display:flex;
  align-items:center;
  justify-content:center;
  overflow:hidden;
}
.dm-img img{
  width:100%;
  height:100%;
  object-fit:cover;
}

/* TITLE: 2 dòng + ... */
.dm-name{
  max-width:110px;
  text-align:center;
  font-size:.8rem;
  line-height:1.25;
  color:var(--tblr-muted,#667085);

  display:-webkit-box;
  -webkit-line-clamp:2;
  -webkit-box-orient:vertical;
  overflow:hidden;
}

/* TABLET: giảm cột để vẫn khít */
@media (max-width: 1200px){
  .dm-grid{ grid-template-columns:repeat(7, minmax(0, 1fr)); }
}
@media (max-width: 992px){
  .dm-grid{ grid-template-columns:repeat(6, minmax(0, 1fr)); }
}

/* MOBILE: chuyển sang 2 hàng + kéo ngang */
@media (max-width: 576px){
  .dm-grid{
    display:grid;
    grid-auto-flow:column;
    grid-template-columns:none;
    grid-template-rows:repeat(2, auto);

    grid-auto-columns:96px;
    column-gap:14px;
    row-gap:14px;

    overflow-x:auto;
    overflow-y:hidden;
    -webkit-overflow-scrolling:touch;
    scroll-snap-type:x mandatory;

    padding:6px 6px 10px;
  }

  .dm-item{ scroll-snap-align:start; }
  .dm-img{ width:44px;height:44px; }
  .dm-name{ max-width:90px;font-size:.78rem; }
}/* ===== Hover effect: zoom icon + đổi màu chữ ===== */
.dm-img img{
  transition: transform .2s ease;
}

.dm-item:hover .dm-img img{
  transform: scale(1.06);
}

.dm-item:hover .dm-name{
  color: var(--tblr-primary, #206bc4); /* xanh Tabler */
}

html[data-bs-theme="dark"] .dm-item:hover{
  background: rgba(148,163,184,.12);
}

html[data-bs-theme="dark"] .dm-img{
  background: rgba(148,163,184,.12);
}

html[data-bs-theme="dark"] .dm-name{
  color: #94a3b8;
}

html[data-bs-theme="dark"] .dm-item:hover .dm-name{
  color: #60a5fa;
}

</style>
