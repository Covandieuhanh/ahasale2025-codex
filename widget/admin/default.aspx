<%@ Page Language="C#" %>
<%
var qs = Request.Url != null ? Request.Url.Query : "";
Response.Redirect("index.html" + qs, false);
Context.ApplicationInstance.CompleteRequest();
%>
