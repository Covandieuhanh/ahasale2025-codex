using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for DropDownList_cl
/// </summary>
public class DropDownList_cl
{
    public static void Return_Index_By_ID(DropDownList dropdown, string _idvalue)
    {
        if (dropdown.Items.Count == 0)
        {
            // DropDownList rỗng
            return;
        }

        // Tìm vị trí của mục trong danh sách
        int index = -1;
        for (int i = 0; i < dropdown.Items.Count; i++)
        {
            if (dropdown.Items[i].Value == _idvalue)
            {
                index = i;
                break;
            }
        }

        // Nếu tìm thấy mục, đặt SelectedIndex
        if (index != -1)
        {
            dropdown.SelectedIndex = index;
        }
    }

}