using System;
using System.Collections.Generic;
using System.Web;
using Octokit;

public class ChangeLogHelper
{
    private const int _monthsBack = 10;

    public static string GetChangeLog(string name)
    {
        try
        {
            var cacheKey = "ChangeLogList_" + name;
            ChangeLogData changeLogData;
            var cachedData = System.Web.HttpContext.Current.Cache[cacheKey];

            if (cachedData == null)
            {
                changeLogData = ProduceChangeLogList(name);

                CacheChangeLogData(cacheKey, changeLogData);

                return changeLogData.ChangeLogOutput;
            }

            changeLogData = (ChangeLogData)cachedData;

            var isForcedRefreshAsked = HttpContext.Current.Request.Url.AbsoluteUri.Contains("/force");
            if (changeLogData.ExpirationDateTime < DateTime.Now || isForcedRefreshAsked)
            {
                var refreshedChangeLogData = ProduceChangeLogList(name);
                if (!refreshedChangeLogData.IsGitHubApiSuccessed)
                {
                    // "Old" data are better than no data. Will retry latter to not slow down 
                    // the web site nor stress the GitHub API for nothing.
                    refreshedChangeLogData = changeLogData;
                }

                CacheChangeLogData(cacheKey, refreshedChangeLogData);

                return refreshedChangeLogData.ChangeLogOutput;
            }

            return changeLogData.ChangeLogOutput;
        }
        catch
        {
            return GenerateErrorSection(name);
        }
    }

    private static void CacheChangeLogData(string cacheKey, ChangeLogData changeLogData)
    {
        if (System.Web.HttpContext.Current.Cache[cacheKey] != null)
            System.Web.HttpContext.Current.Cache.Remove(cacheKey);

        changeLogData.UpdateExpiration();

        System.Web.HttpContext.Current.Cache.Add(cacheKey,
                                                    changeLogData,
                                                    null,
                                                    DateTime.Now.AddHours(12),
                                                    System.Web.Caching.Cache.NoSlidingExpiration,
                                                    System.Web.Caching.CacheItemPriority.Normal,
                                                    null);
    }

    private static string GenerateErrorSection(string name)
    {
        return "<div class='alert alert-danger'><p>There seems to be a problem accessing the change log data but no worry! <a class='alert-link' href='https://github.com/madskristensen/" + name + "/releases'>This list is always available on GitHub</a>.</p></div>";
    }

    private static ChangeLogData ProduceChangeLogList(string name)
    {
        var result = new ChangeLogData();

        try
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();

            var github = new GitHubClient(new ProductHeaderValue("vswebessentials.com"));
            IReadOnlyList<Release> lst = github.Release.GetAll("madskristensen", name).Result;

            // Only display releases from the last 6 months
            var sixMonthAgoDate = DateTime.Now.AddMonths(-_monthsBack);
            // Display an link for the full list on GitHub if older releases found
            bool isMoreReleasesAvailable = false;

            s.AppendFormat("<div class='panel-group' id='accordion" + name + "'>");

            for (var i = 0; i < lst.Count; i++)
            {
                Release r = lst[i];
                s.Append("<div class='panel panel-default'>");
                s.Append("<div class='panel-heading'>");
                s.Append("<h4 class='panel-title'>");
                s.AppendFormat("<a data-toggle='collapse' data-parent='#accordion{0}' href='#id{1}'>", name, r.Id);
                string publishedAt = "[Unknow published date]";
                if (r.PublishedAt.HasValue)
                {
                    publishedAt = r.PublishedAt.Value.Date.ToString("MMM dd. yyyy");
                }
                s.AppendFormat("{0} <em style=\"float:right;margin-right:6em\">{1}</em>", r.Name, publishedAt);
                s.AppendFormat("<a class='btn btn-success btn-nightly-download' href='{0}'><i class='fa fa-download'></i>Download</a>", r.HtmlUrl);
                s.Append("</a></h4></div>");
                var collapseClass = i == 0 ? "in" : "collapse";
                s.AppendFormat("<div id='id{0}' class='panel-collapse {1}'>", r.Id, collapseClass);
                s.Append("<div class='panel-body'>");
                s.Append(r.BodyHtml);
                s.Append("</div></div></div>");

                if (r.PublishedAt < sixMonthAgoDate)
                {
                    isMoreReleasesAvailable = true;
                    break;
                }
            }

            s.AppendFormat("</div>");

            if (isMoreReleasesAvailable)
            {
                s.Append("<div><p>For the complete list of releases, go to the <a class='' target='_blank' href='https://github.com/madskristensen/" + name + "/releases'>GitHub Release page of the project</a>.</p></div>");
            }

            result.ChangeLogOutput = s.ToString();
            result.IsGitHubApiSuccessed = true;
        }
        catch
        {
            result.ChangeLogOutput = GenerateErrorSection(name);
            result.IsGitHubApiSuccessed = false;
        }

        return result;
    }
}