using System;
using System.Collections.Generic;
using System.Web.UI;

public partial class bat_dong_san_du_an_chi_tiet : BatDongSanPageBase
{
    protected BatDongSanService_cl.ProjectItem Project;

    protected void Page_Load(object sender, EventArgs e)
    {
        string slug = (Request.QueryString["slug"] ?? "").Trim();
        Project = BatDongSanService_cl.GetProjectBySlug(slug);
        phProject.Visible = Project != null;
        phEmpty.Visible = Project == null;

        if (Project != null)
        {
            rptListings.DataSource = BatDongSanService_cl.GetListingsByProject(Project.Name, 12);
            rptListings.DataBind();
        }
    }

}
