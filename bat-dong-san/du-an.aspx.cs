using System;
using System.Collections.Generic;
using System.Web.UI;

public partial class bat_dong_san_du_an : BatDongSanPageBase
{
    protected List<BatDongSanService_cl.ProjectItem> Projects = new List<BatDongSanService_cl.ProjectItem>();

    protected void Page_Load(object sender, EventArgs e)
    {
        Projects = BatDongSanService_cl.GetFeaturedProjects();
        rptProjects.DataSource = Projects;
        rptProjects.DataBind();
    }

}
