using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace MvcSiteMap.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ContentResult Index()
        {
            //Scraping SO feed
            var url = "http://stackoverflow.com/feeds";
            XmlReader reader = XmlReader.Create(url);
            SyndicationFeed feed = SyndicationFeed.Load(reader);

            var q = from item in feed.Items
                    select new {
                        Title = item.Title,
                        URL = item.Id,
                        Date = item.PublishDate
                    };

            //Build the SiteMap
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset",
                    from i in q
                    select
                        new XElement(ns + "url",
                          new XElement(ns + "loc", i.URL),
                              new XElement(ns + "lastmod", String.Format("{0:yyyy-MM-dd}", i.Date)),
                          new XElement(ns + "changefreq", "monthly"),
                          new XElement(ns + "priority", "0.5")
                          )
                        )
                      );

            return Content(sitemap.ToString(), "text/xml");
        }
    }
}
