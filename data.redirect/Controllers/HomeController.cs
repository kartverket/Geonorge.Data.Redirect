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
        
        [Route("{register}/so/{objekttype}.{lokalid}")]
        [Route("{register}/so/{objekttype}/{lokalid}")]
        [Route("{register}/so/{objekttype}/{lokalid}/{versjon}")]
        [Route("{register}/so/{*lokalid}")]
        public ActionResult ShowRedirect(string register, string objekttype, string lokalid, string versjon)
        {
            //http://data.geonorge.no/matrikkel/so/1b28ec00-03ca-11e2-a21f-0800200c9a66/4.0
            //http://data.geonorge.no/matrikkel/so/123456
            //http://data.geonorge.no/matrikkel/so/adresse.123456
            //http://data.geonorge.no/matrikkel/adresse/so/123456   (IKKE LOV???)
            //http://data.geonorge.no/kartverket/matrikkel/so/adresse.123456 (IKKE LOV)
            //http://data.geonorge.no/nilu/so/1b28ec00-03ca-11e2-a21f-0800200c9a66/4.0
            //http://data.geonorge.no/sportsfiskeregister/so/innlandsfisk.123456789
            //http://data.geonorge.no/so/akvakulturlokaliteter/lokalitet.35553312 (IKKE LOV)
            //http://data.geonorge.no/akvakulturlokaliteter/so/lokalitet.35553312 = http://wfs.fiskeridirektoratet.no/arcgis/services/akvakultur/mapserver/WfSServer?&Service=wfs&version=1.1.1&request=getfeature&gml_ID=lokalitet.35553312
            //http://data.geonorge.no/so/naturbase/naturvernområder/62883310 (IKKE LOV)
            //http://data.geonorge.no/naturbase/so/naturvernområder/62883310 = http://wfs.miljodirektoratet.no/arcgis/services/vern/mapserver/WfSServer?&Service=wfs&version=1.1.1&request=getfeature&gml_ID=62883310

            //Slå opp i register
            //Løse redirect url
            //gml_ID=lokalitet.35553312 = {register}/so/{objekttype}.{lokalid}
            //gml_ID=35553312 = {register}/so/{lokalid} og {register}/so/{objekttype}/{lokalid}
            return Redirect("http://wfs.fiskeridirektoratet.no/arcgis/services/akvakultur/mapserver/WfSServer?&Service=wfs&version=1.1.1&request=getfeature&gml_ID=" + lokalid);
           
        }
    }
}
