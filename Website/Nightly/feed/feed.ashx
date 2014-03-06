<%@ WebHandler Language="C#" Class="feed" %>

using System;
using System.Web;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

public class feed : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        string manifest = context.Server.MapPath("extension.vsixmanifest");
        XDocument doc = XDocument.Load(manifest);

        XmlNamespaceManager xnm = new XmlNamespaceManager(new NameTable());
        xnm.AddNamespace("t", "http://schemas.microsoft.com/developer/vsx-schema/2011");
        xnm.AddNamespace("d", "http://schemas.microsoft.com/developer/vsx-schema-design/2011");

        var idNode = doc.XPathSelectElement("//t:Identitya", xnm);

        if (idNode == null)
            throw new HttpException(500, "Could not load the VSIX manifest file");

        string version = idNode.Attribute("Version").Value;
        string id = idNode.Attribute("Id").Value;
        string vsix = context.Server.MapPath("webessentials2013.vsix");
        DateTime updated = File.GetLastAccessTimeUtc(vsix);

        SetHeaders(context, manifest, updated);
        WriteXml(context, version, id, updated);
    }

    private static void SetHeaders(HttpContext context, string manifest, DateTime updated)
    {
        context.Response.ContentType = "text/xml";
        context.Response.Cache.SetLastModified(updated);
        context.Response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
        context.Response.Cache.SetValidUntilExpires(true);
        context.Response.AddFileDependency(manifest);
    }

    private void WriteXml(HttpContext context, string version, string id, DateTime updated)
    {
        using (XmlWriter writer = XmlWriter.Create(context.Response.OutputStream))
        {
            writer.WriteStartElement("feed", "http://www.w3.org/2005/Atom");

            writer.WriteElementString("title", "Web Essentials 2013 Nightly");
            writer.WriteElementString("id", id);
            writer.WriteElementString("updated", updated.ToString("yyyy-MM-ddTHH:mm:ssZ"));

            writer.WriteStartElement("entry");

            writer.WriteElementString("id", id);

            writer.WriteStartElement("title");
            writer.WriteAttributeString("type", "text");
            writer.WriteValue("Web Essentials 2013 Nightly");
            writer.WriteEndElement(); // title

            writer.WriteStartElement("summary");
            writer.WriteAttributeString("type", "text");
            writer.WriteValue("Nightly build of Web Essentials 2013");
            writer.WriteEndElement(); // summary

            writer.WriteElementString("published", "2012-09-01T18:51:00-07:00");
            writer.WriteElementString("updated", updated.ToString("yyyy-MM-ddTHH:mm:ssZ"));

            writer.WriteStartElement("author");
            writer.WriteElementString("name", "Mads Kristensen");
            writer.WriteEndElement(); // author

            writer.WriteStartElement("category");
            writer.WriteAttributeString("term", "Web Essentials Nightly");
            writer.WriteEndElement(); // category

            writer.WriteStartElement("content");
            writer.WriteAttributeString("type", "application/octet-stream");
            writer.WriteAttributeString("src", "http://vswebessentials.com/nightly/feed/WebEssentials2013.vsix");
            writer.WriteEndElement(); // content

            writer.WriteRaw("<Vsix xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://schemas.microsoft.com/developer/vsx-syndication-schema/2010\">");
            writer.WriteElementString("Id", id);
            writer.WriteElementString("Version", version);

            writer.WriteStartElement("References");
            writer.WriteEndElement();

            writer.WriteRaw("</Vsix>");// Vsix
            writer.WriteEndElement(); // entry
            writer.WriteEndElement(); // feed
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