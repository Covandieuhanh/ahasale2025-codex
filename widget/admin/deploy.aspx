<%@ Page Language="C#" ValidateRequest="false" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Text" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%
Response.ContentType = "application/json; charset=utf-8";
Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
Response.Cache.SetNoStore();

JavaScriptSerializer serializer = new JavaScriptSerializer();
serializer.MaxJsonLength = Int32.MaxValue;

Action<int, object> writeJson = delegate(int statusCode, object payload) {
  Response.StatusCode = statusCode;
  Response.Write(serializer.Serialize(payload));
  Context.ApplicationInstance.CompleteRequest();
};

try {
  if (!String.Equals(Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase)) {
    writeJson(405, new {
      ok = false,
      message = "Chỉ hỗ trợ POST cho endpoint deploy."
    });
    return;
  }

  string body = "";
  using (StreamReader reader = new StreamReader(Request.InputStream, Encoding.UTF8)) {
    body = reader.ReadToEnd();
  }
  if (String.IsNullOrWhiteSpace(body)) {
    writeJson(400, new {
      ok = false,
      message = "Body rỗng. Vui lòng gửi dữ liệu JSON."
    });
    return;
  }

  var root = serializer.DeserializeObject(body) as System.Collections.Generic.Dictionary<string, object>;
  if (root == null) {
    writeJson(400, new {
      ok = false,
      message = "Dữ liệu JSON không hợp lệ."
    });
    return;
  }

  string action = root.ContainsKey("action") ? Convert.ToString(root["action"]) : "";
  string deployKey = root.ContainsKey("deployKey") ? Convert.ToString(root["deployKey"]) : "";

  // TODO: Đổi khóa này trên server production để tăng bảo mật.
  const string ExpectedDeployKey = "AhaDeploy@2026";
  if (String.IsNullOrWhiteSpace(deployKey) || !String.Equals(deployKey, ExpectedDeployKey, StringComparison.Ordinal)) {
    writeJson(401, new {
      ok = false,
      message = "Sai khóa deploy. Vui lòng kiểm tra lại."
    });
    return;
  }

  if (String.Equals(action, "ping", StringComparison.OrdinalIgnoreCase)) {
    writeJson(200, new {
      ok = true,
      message = "Kết nối deploy thành công. Endpoint đã sẵn sàng."
    });
    return;
  }

  if (!String.Equals(action, "deploy", StringComparison.OrdinalIgnoreCase)) {
    writeJson(400, new {
      ok = false,
      message = "Action không hợp lệ. Chỉ chấp nhận ping hoặc deploy."
    });
    return;
  }

  if (!root.ContainsKey("bundle")) {
    writeJson(400, new {
      ok = false,
      message = "Thiếu dữ liệu bundle để deploy."
    });
    return;
  }

  object bundleObject = root["bundle"];
  string bundleJson = serializer.Serialize(bundleObject);
  if (String.IsNullOrWhiteSpace(bundleJson) || String.Equals(bundleJson, "null", StringComparison.OrdinalIgnoreCase)) {
    writeJson(400, new {
      ok = false,
      message = "Bundle rỗng. Không thể deploy."
    });
    return;
  }

  string endpointPath = Server.MapPath(Request.AppRelativeCurrentExecutionFilePath);
  string adminDirectory = Path.GetDirectoryName(endpointPath);
  string targetPath = Path.GetFullPath(Path.Combine(adminDirectory, "..", "widget-data", "widget-data.json"));
  string targetDirectory = Path.GetDirectoryName(targetPath);

  if (!Directory.Exists(targetDirectory)) {
    writeJson(500, new {
      ok = false,
      message = "Không tìm thấy thư mục widget-data trên server."
    });
    return;
  }

  File.WriteAllText(targetPath, bundleJson, new UTF8Encoding(false));

  writeJson(200, new {
    ok = true,
    message = "Đã lưu trực tiếp dữ liệu chatbot lên host.",
    bytes = bundleJson.Length,
    updatedAt = DateTime.UtcNow.ToString("o")
  });
} catch (Exception ex) {
  writeJson(500, new {
    ok = false,
    message = "Lỗi deploy trực tiếp: " + ex.Message
  });
}
%>
