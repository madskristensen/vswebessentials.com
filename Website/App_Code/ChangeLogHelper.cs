using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Octokit;

/// <summary>
/// Summary description for ChangeLogHelper
/// </summary>
public class ChangeLogHelper
{
    public static string ProduceChangeLog(string owner, string name)
    {
        try
        {
            var github = new GitHubClient(new ProductHeaderValue("vswebessentials.com"));
            //github.Repository.Get("madskristensen", "WebEssentials2015")

            System.Text.StringBuilder s = new System.Text.StringBuilder();
            IReadOnlyList<Release> lst = github.Release.GetAll(owner, name).Result;

            //IReadOnlyList<User> lst2 = await github.Repository.GetAllContributors("madskristensen", "WebEssentiazzzzls2015");

            for (var i = 0; i < lst.Count; i++)
            {
                Release r = lst[i];
                s.Append("<div class='panel panel-default'>");
                s.Append("<div class='panel-heading'>");
                s.Append("<h4 class='panel-title'>");
                s.AppendFormat("<a data-toggle='collapse' data-parent='#accordion{0}' href='#id{1}'>", name, r.Id);
                string publishDate = "[Unknow published date]";
                if (r.PublishedAt.HasValue)
                {
                    publishDate = r.PublishedAt.Value.Date.ToLongDateString();
                }
                s.AppendFormat("{0} - {1}", r.Name, publishDate);
                s.AppendFormat("<a class='btn btn-success btn-nightly-download' href='{0}'><i class='fa fa-download'></i>Download</a>", r.HtmlUrl);
                s.Append("</a></h4></div>");
                var collapseClass = i == 0 ? "in" : "collapse";
                s.AppendFormat("<div id='id{0}' class='panel-collapse {1}'>", r.Id, collapseClass);
                s.Append("<div class='panel-body'>body");
                // Todo : Markdown :
                //s.Append(r.Body);
                s.Append("</div></div></div>");
            }




            return s.ToString();
        }
        catch
        {
            return "<div class='alert alert-error'>Access to GitHub Releases information</div>";
        }





    }
}