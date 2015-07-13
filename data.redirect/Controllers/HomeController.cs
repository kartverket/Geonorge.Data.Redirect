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
        [Route("{register}/so/{id}")]
        [Route("{register}/so/{objecttype}.{id}")]
        [Route("{register}/so/{objecttype}/{id}")]
        public ActionResult ShowRedirect(string register, string objecttype, string id)
        {
            //Slå opp i register
            //Løse redirect url
           
            return Redirect("http://wfs.fiskeridirektoratet.no/arcgis/services/akvakultur/mapserver/WfSServer?&Service=wfs&version=1.1.1&request=getfeature&gml_ID=lokalitet.35553312");
           
        }
    }
}
