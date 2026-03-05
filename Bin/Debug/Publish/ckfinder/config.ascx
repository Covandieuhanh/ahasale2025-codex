<%@ Control Language="C#" EnableViewState="false" AutoEventWireup="false" Inherits="CKFinder.Settings.ConfigFile" %>
<%@ Import Namespace="CKFinder.Settings" %>
<script runat="server">
    public override bool CheckAuthentication()
    {
        if (Session["taikhoan"].ToString() != "")
            return true;
        else
            return false;
    }
    public override void SetConfig()
    {
        LicenseName = "bcorn";
        LicenseKey = "5FECEPW78PYK2KT52J5H9XBSFG6X2KB7";
        BaseUrl = "/uploads/";
        BaseDir = "";
        Plugins = new string[] {
			// "CKFinder.Plugins.FileEditor, CKFinder_FileEditor",
			// "CKFinder.Plugins.ImageResize, CKFinder_ImageResize",
			// "CKFinder.Plugins.Watermark, CKFinder_Watermark"
		};
        Thumbnails.Enabled = true;
        Thumbnails.DirectAccess = false;
        Thumbnails.MaxWidth = 100;
        Thumbnails.MaxHeight = 100;
        Thumbnails.Quality = 80;

        // Set the maximum size of uploaded images. If an uploaded image is
        // larger, it gets scaled down proportionally. Set to 0 to disable this
        // feature.
        Images.MaxWidth = 800;
        Images.MaxHeight = 800;
        Images.Quality = 100;

        CheckSizeAfterScaling = true;
        DisallowUnsafeCharacters = true;
        CheckDoubleExtension = true;
        ForceSingleExtension = true;
        HtmlExtensions = new string[] { "html", "htm", "xml", "js" };
        HideFolders = new string[] { ".*", "CVS" };
        HideFiles = new string[] { ".*" };
        SecureImageUploads = true;

        EnableCsrfProtection = true;
        RoleSessionVar = "CKFinder_UserRole";
        AccessControl acl = AccessControl.Add();
        acl.Role = "*";
        acl.ResourceType = "*";
        acl.Folder = "/";

        acl.FolderView = true;
        acl.FolderCreate = true;
        acl.FolderRename = true;
        acl.FolderDelete = true;

        acl.FileView = true;
        acl.FileUpload = true;
        acl.FileRename = true;
        acl.FileDelete = true;

        DefaultResourceTypes = "";

        ResourceType type;

        type = ResourceType.Add("Files");/*nếu là file thì đẩy vào thư mục ''/uploads/files/ckeditor'*/
        type.Url = BaseUrl + "Files/ckeditor";
        type.MaxSize = 0;
        type.AllowedExtensions = new string[] { "7z", "aiff", "asf", "avi", "bmp", "csv", "doc", "docx", "fla", "flv", "gif", "gz", "gzip", "jpeg", "jpg", "mid", "mov", "mp3", "mp4", "mpc", "mpeg", "mpg", "ods", "odt", "pdf", "png", "ppt", "pptx", "pxd", "qt", "ram", "rar", "rm", "rmi", "rmvb", "rtf", "sdc", "sitd", "swf", "sxc", "sxw", "tar", "tgz", "tif", "tiff", "txt", "vsd", "wav", "wma", "wmv", "xls", "xlsx", "zip" };
        type.DeniedExtensions = new string[] { };

        type = ResourceType.Add("Images");/*nếu là ảnh thì đẩy vào thư mục ''/uploads/files/ckeditor'*/
        type.Url = BaseUrl + "Images/ckeditor";
        type.MaxSize = 0;
        type.AllowedExtensions = new string[] { "bmp", "gif", "jpeg", "jpg", "png", "ico" };
        type.DeniedExtensions = new string[] { };
    }

</script>
