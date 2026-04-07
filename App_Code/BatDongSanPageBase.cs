using System.Web.UI;

public abstract class BatDongSanPageBase : Page
{
    protected string BuildListingUrl(object itemObj)
    {
        return BatDongSanUiHelper_cl.BuildListingUrl(itemObj);
    }

    protected string BuildProjectUrl(object itemObj)
    {
        return BatDongSanUiHelper_cl.BuildProjectUrl(itemObj);
    }

    protected string BuildPurposeLabel(object purposeObj)
    {
        return BatDongSanUiHelper_cl.BuildPurposeLabel(purposeObj);
    }

    protected string BuildPurposeBadgeCss(object purposeObj)
    {
        return BatDongSanUiHelper_cl.BuildPurposeBadgeCss(purposeObj);
    }
}
