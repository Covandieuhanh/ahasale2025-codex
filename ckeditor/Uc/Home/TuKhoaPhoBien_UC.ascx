<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TuKhoaPhoBien_UC.ascx.cs" Inherits="Uc_Home_TuKhoaPhoBien_UC" %>
<div class="container-xl my-3">
  <div class="card">
    <div class="card-body">

      <div class="fw-semibold text-secondary mb-3">
        Các từ khóa phổ biến
      </div>

      <div class="row g-3">
        <div class="col-6 col-md-3">
          <ul class="list-unstyled dm-keywords">
            <li><a href="#" class="dm-link">iPhone 12</a></li>
            <li><a href="#" class="dm-link">iPhone 14 Pro Max</a></li>
            <li><a href="#" class="dm-link">Điện thoại iPhone cũ</a></li>
            <li><a href="#" class="dm-link">Điện thoại Samsung cũ</a></li>
            <li><a href="#" class="dm-link">Máy quay cũ</a></li>
            <li><a href="#" class="dm-link">Loa cũ</a></li>
            <li><a href="#" class="dm-link">Điện thoại cũ</a></li>
          </ul>
        </div>

        <div class="col-6 col-md-3">
          <ul class="list-unstyled dm-keywords">
            <li><a href="#" class="dm-link">iPhone 12 Mini</a></li>
            <li><a href="#" class="dm-link">iPhone 14 Plus</a></li>
            <li><a href="#" class="dm-link">Dàn karaoke cũ</a></li>
            <li><a href="#" class="dm-link">Máy tính để bàn giá rẻ</a></li>
            <li><a href="#" class="dm-link">Micro cũ</a></li>
            <li><a href="#" class="dm-link">Máy tính để bàn cũ</a></li>
            <li><a href="#" class="dm-link">Macbook</a></li>
          </ul>
        </div>

        <div class="col-6 col-md-3">
          <ul class="list-unstyled dm-keywords">
            <li><a href="#" class="dm-link">iPhone 12 Pro</a></li>
            <li><a href="#" class="dm-link">iPhone 14 Pro</a></li>
            <li><a href="#" class="dm-link">Tivi cũ giá rẻ</a></li>
            <li><a href="#" class="dm-link">Ống kính (lens) cũ</a></li>
            <li><a href="#" class="dm-link">Tai nghe cũ</a></li>
            <li><a href="#" class="dm-link">Máy tính bảng cũ</a></li>
          </ul>
        </div>

        <div class="col-6 col-md-3">
          <ul class="list-unstyled dm-keywords">
            <li><a href="#" class="dm-link">iPhone 12 Pro Max</a></li>
            <li><a href="#" class="dm-link">Samsung S25 Edge</a></li>
            <li><a href="#" class="dm-link">iPhone 16e</a></li>
            <li><a href="#" class="dm-link">Máy ảnh cũ</a></li>
            <li><a href="#" class="dm-link">Amply</a></li>
            <li><a href="#" class="dm-link">Laptop cũ</a></li>
          </ul>
        </div>
      </div>

    </div>
  </div>
</div>
<style>
    /* list keyword */
.dm-keywords{
  margin:0;
  padding:0;
}

/* bullet mờ */
.dm-keywords li{
  position:relative;
  padding-left:14px;
  margin-bottom:.45rem;
}
.dm-keywords li::before{
  content:"";
  width:5px;
  height:5px;
  border-radius:999px;
  background:var(--tblr-secondary, #9ca3af);
  position:absolute;
  left:0;
  top:.55em;
  opacity:.7;
}

/* link text-secondary */
.dm-link{
  color:var(--tblr-secondary, #6b7280);
  font-size:.875rem;
  text-decoration:none !important;
  transition:color .15s ease;
}

/* hover nhẹ đúng Tabler */
.dm-link:hover{
  color:var(--tblr-primary, #206bc4);
  text-decoration:underline;
  text-underline-offset:2px;
}

</style>