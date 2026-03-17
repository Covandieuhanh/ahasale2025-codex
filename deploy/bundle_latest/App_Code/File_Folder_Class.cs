using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

public class file_folder_class
{
    public static void del_file(string _link_file)
    {
        if (File.Exists(System.Web.HttpContext.Current.Server.MapPath("~" + _link_file)))
            File.Delete(System.Web.HttpContext.Current.Server.MapPath("~" + _link_file));
    }
}