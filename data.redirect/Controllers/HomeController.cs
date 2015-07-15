using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace data.redirect.Controllers
{
    public class HomeController : Controller
    {
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


        [Route("so/{namepace}/{localId}")]
        [Route("so/{namepace}/{localId}/{versionId}")]
        [Route("so/{namepace}/{theme}/{class}/{localId}/{versionId}")]
        [Route("so/{namepace}/{theme}/{class}/{*localId}")]
        [Route("so/{namepace}/{localId}/{*versionId}")]
        [Route("so/{namepace}/{*localId}")]
        public ActionResult ShowRedirect(string @namepace, string @class, string localId, string versionId, string theme)
        {
            //http://data.geonorge.no/so/matrikkel/1b28ec00-03ca-11e2-a21f-0800200c9a66/4.0
            //http://data.geonorge.no/so/matrikkel/123456
            //http://data.geonorge.no/so/matrikkel/adresse.123456
            //http://data.geonorge.no/matrikkel/adresse/so/123456   (IKKE LOV???)
            //http://data.geonorge.no/kartverket/matrikkel/so/adresse.123456 (IKKE LOV)
            //http://data.geonorge.no/so/nilu/1b28ec00-03ca-11e2-a21f-0800200c9a66/4.0
            //http://data.geonorge.no/so/sportsfiskeregister/innlandsfisk.123456789
            //http://data.geonorge.no/so/akvakulturlokaliteter/lokalitet.35553312 
            //http://data.geonorge.no/akvakulturlokaliteter/so/lokalitet.35553312 (IKKE LOV)= http://wfs.fiskeridirektoratet.no/arcgis/services/akvakultur/mapserver/WfSServer?&Service=wfs&version=1.1.1&request=getfeature&gml_ID=lokalitet.35553312
            //http://data.geonorge.no/so/naturbase/naturvernområder/62883310 
            //http://data.geonorge.no/naturbase/so/naturvernområder/62883310 (IKKE LOV)= http://wfs.miljodirektoratet.no/arcgis/services/vern/mapserver/WfSServer?&Service=wfs&version=1.1.1&request=getfeature&gml_ID=62883310

            //Slå opp i namespace register med domene + /so/ + register for å finne tjeneste som skal redirectes til

            string redirectServiceUrl = GetServiceUrlFromNamespaceRegister(@namepace);
            return Redirect(redirectServiceUrl);

            //Løse redirect url
            //gml_ID=lokalitet.35553312 = {register}/so/{objekttype}.{lokalid}
            //gml_ID=35553312 = {register}/so/{lokalid} og {register}/so/{objekttype}/{lokalid}
            //return Redirect("http://wfs.fiskeridirektoratet.no/arcgis/services/akvakultur/mapserver/WfSServer?&Service=wfs&version=1.1.1&request=getfeature&gml_ID=" + localId);
           
        }

        public string GetServiceUrlFromNamespaceRegister(string @namespace) {

            System.Net.WebClient c = new System.Net.WebClient();
            c.Encoding = System.Text.Encoding.UTF8;
            var data = c.DownloadString("http://register.dev.geonorge.no/api/register/navnerom/");
            var response = Newtonsoft.Json.Linq.JObject.Parse(data);
            var namespases = response["containeditems"];

            foreach (var item in namespases)
            {
                if (item["label"].ToString().Contains(@namespace))
                {
                    return item["serviceUrl"].ToString();
                }
            }            
            return null;
        }
    }
}
