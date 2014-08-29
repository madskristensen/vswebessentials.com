<%@ WebHandler Language="C#" Class="GoTo" %>

using System.Web;

public class GoTo : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        string requestedFeature = string.Empty;

        if (context.Request.QueryString.Count > 0 && context.Request.QueryString["id"] != null)
        {
            requestedFeature = context.Request.QueryString["id"];
        }

        switch (requestedFeature)
        {
            case "weignore":
                context.Response.Redirect("/features/general#weignore");
                break;

            default:
                // Not found... return to homepage
                context.Response.Redirect("/");
                break;
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}