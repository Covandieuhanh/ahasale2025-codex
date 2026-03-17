<%@ Control Language="C#" AutoEventWireup="true" CodeFile="footer_webcon_uc.ascx.cs" Inherits="webcon_uc_footer_webcon_uc" %>
<style>
    .ft-cl-1 {
        color: #dddddd !important
    }

    .ft-a-1 {
        color: #dddddd !important
    }

        .ft-a-1:hover {
            color: #008a00 !important;
        }

    .ft-cl-2 {
        color: #b9b9b9
    }

    .ft-a-2 {
        color: #b9b9b9 !important
    }

        .ft-a-2:hover {
            color: #008a00 !important;
        }
</style>
<div class="container-fluid pt-20 pb-12" style="background-color: #363636">
    <div class="container" style="font-size: 15px">
        <div class="row text-center">
            <div class="cell-lg-12 p-3">

                <div>
                    <a href="/">
                        <img src="<%=logo %>" width="110" />
                    </a>

                    <div class="ft-cl-1 text-bold text-upper mt-2"><%=tencongty %></div>
                    <div class="ft-cl-2"><i><%=slogan %></i></div>
                </div>
                <div class="ft-cl-2">
                    <div class="mt-2">
                        Địa chỉ: <%=diachi %>
                  
                    </div>
                    <div class="mt-1">
                        Email: <%=email %>
                    </div>
                    <div class="mt-3">
                        <a href="tel:<%=hotline %>">
                            <div class="button fg-white text-bold mr-1" style="border-radius: 20px; padding: 8px 20px;background-color:#ce352c">
                                <span class="mif mif-phone ani-spanner pr-1"></span><%=hotline %>
                            </div>
                        </a>
                    </div>
                </div>
          
            </div>
          
            
        </div>
    </div>
</div>
