using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.XPath;

namespace data.redirect.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        public ActionResult Index()
        {
            ViewBag.Title = "Forside";

            return View();
        }

        /// <summary>
        /// TODO dokumentere alle mulige varianter og forskjellen i mappingen på disse til WFS kall
        /// </summary>
        /// <param name="register"></param>
        /// <param name="objecttype"></param>
        /// <param name="id"></param>
        /// <returns></returns>


        [Route("{namepace}/so/{localId}")]
        [Route("so/{namepace}/so/{localId}/{versionId}")]
        //[Route("{namepace}/{theme}/{class}/]
        [Route("{namepace}/so/{theme}/{class}/{localId}/{versionId}")]
        [Route("{namepace}/so/{theme}/{class}/{*localId}")]
        [Route("{namepace}/so/{localId}/{*versionId}")]
        [Route("{namepace}/so/{*localId}")]
        [Route("{*path}")]
        public ActionResult ShowRedirect(string @namepace, string @class, string localId, string versionId, string theme, string path)
        {
            //http://data.geonorge.no/naturbase/so/naturvernområder/62883310 Naturvernområde lokalId?
            //http://data.geonorge.no/matrikkel/so/1b28ec00-03ca-11e2-a21f-0800200c9a66/4.0
            //http://data.geonorge.no/matrikkel/so/123456
            //http://data.geonorge.no/matrikkel/so/adresse.123456
            //http://data.geonorge.no/nilu/so/1b28ec00-03ca-11e2-a21f-0800200c9a66/4.0 Lik som den første...
            //http://data.geonorge.no/sportsfiskeregister/so/innlandsfisk.123456789
            //http://data.geonorge.no/akvakulturlokaliteter/so/lokalitet.35553312 
            //http://data.geonorge.no/matrikkel/adresse/so/123456   (IKKE LOV???)
            //http://data.geonorge.no/kartverket/matrikkel/so/adresse.123456 (IKKE LOV)
            //http://data.geonorge.no/akvakulturlokaliteter/so/lokalitet.35553312 (IKKE LOV)= http://wfs.fiskeridirektoratet.no/arcgis/services/akvakultur/mapserver/WfSServer?&Service=wfs&version=1.1.1&request=getfeature&gml_ID=lokalitet.35553312
            //http://data.geonorge.no/naturbase/so/naturvernområder/62883310 (IKKE LOV)= http://wfs.miljodirektoratet.no/arcgis/services/vern/mapserver/WfSServer?&Service=wfs&version=1.1.1&request=getfeature&gml_ID=62883310

            //Slå opp i namespace register med domene + /so/ + register for å finne tjeneste som skal redirectes til
            string redirectServiceUrl = GetServiceUrlFromNamespaceRegister(@namepace, @class, @theme, localId, path);
            
            if (redirectServiceUrl != null)
            {
                //redirectServiceUrl = FixServiceUrl(redirectServiceUrl, localId, versionId);
                return Redirect(redirectServiceUrl);
            }
            return HttpNotFound();
            //Løse redirect url
            //gml_ID=lokalitet.35553312 = {register}/so/{objekttype}.{lokalid}
            //gml_ID=35553312 = {register}/so/{lokalid} og {register}/so/{objekttype}/{lokalid}
            //return Redirect("http://wfs.fiskeridirektoratet.no/arcgis/services/akvakultur/mapserver/WfSServer?&Service=wfs&version=1.1.1&request=getfeature&gml_ID=" + localId);
        }

        private string FixServiceUrl(string redirectServiceUrl, string localId, string versionId)
        {
            XNamespace WFS = "http://www.opengis.net/wfs";
            XDocument xmlDocument = XDocument.Load(redirectServiceUrl);
            WFS = xmlDocument.Root.Name.Namespace;
            XElement root = xmlDocument.Element(WFS + "WFS_Capabilities");

            string version = root.Attribute("version").Value;

            if (version == "2.0.0")
            {
                redirectServiceUrl = redirectServiceUrl.Replace("request=GetCapabilities", "REQUEST=GetFeature")
                                                       .Replace("REQUEST=GetCapabilities", "REQUEST=GetFeature")
                                                       .Replace("request=getcapabilities", "REQUEST=GetFeature")
                                                       .Replace("service=", "SERVICE=")
                                                       + "&VERSION=" + version
                                                       + "&OUTPUTFORMAT=application%2Fgml%2Bxml%3B+version%3D3.2&STOREDQUERY_ID=urn:ogc:def:query:OGC-WFS::GetFeatureById&ID=" + localId;
            }
            else
            {
                redirectServiceUrl = redirectServiceUrl.Replace("request=GetCapabilities", "request=GetFeature")
                                                       .Replace("REQUEST=GetCapabilities", "request=GetFeature")
                                                       .Replace("request=getcapabilities", "request=GetFeature")
                                                        + "&gml_ID=" + localId;
            }
            return redirectServiceUrl;
        }

        public string GetServiceUrlFromNamespaceRegister(string @namespace, string @class, string theme, string localId, string path)
        {
            string ns = @namespace;

            if (!string.IsNullOrEmpty(path))
                ns = path;
            else 
            { 
                if (!string.IsNullOrWhiteSpace(theme))
                {
                    ns += "/" + theme;
                }
                if (!string.IsNullOrWhiteSpace(@class))
                {
                    ns += "/" + @class;
                }
            }

            System.Net.WebClient c = new System.Net.WebClient();
            c.Encoding = System.Text.Encoding.UTF8;
            var environment = WebConfigurationManager.AppSettings["EnvironmentName"];
            if (!string.IsNullOrEmpty(environment))
                environment = "." + environment;
            var data = c.DownloadString("http://register" + environment + ".geonorge.no/api/register/navnerom/");
            var response = Newtonsoft.Json.Linq.JObject.Parse(data);
            var namespases = response["containeditems"];

            var urlToNamespacRegister = "";

            foreach (var item in namespases)
            {
                var datasetId = ns.Split('/').Last();
                var nameSpace = ns.Replace("/" + datasetId, "");

                if (item["label"].ToString().Contains(nameSpace))
                {
                    urlToNamespacRegister = item["id"].ToString();

                    foreach (var dataset in item["NameSpaceDatasetUrls"])
                    {
                        if (dataset["DatasettId"].ToString() == datasetId)
                            if (dataset["RedirectUrl"].ToString() != "")
                                return dataset["RedirectUrl"].ToString();
                    }
                }
            }

            if (!string.IsNullOrEmpty(urlToNamespacRegister))
                return urlToNamespacRegister;


            return null;
        }
    }
}
