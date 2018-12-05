using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BildeOpplast.Models;
using System.IO;
using System.Drawing;

namespace BildeOpplast.Controllers
{
    public class BildeAdminController : Controller
    {
        [HttpGet]
        public ActionResult LastOppBilde()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LastOppBilde(Bilde bilde, HttpPostedFileBase bildefil)
        {
            try
            {
                //Lagre bildefil
                String bildenavn = Path.GetFileName(bildefil.FileName);
                String bildefilsti = Path.Combine(Server.MapPath("~/Content/Bilder") , bildenavn);

                bildefil.SaveAs(bildefilsti);

                //Image bildefil = 

                //Lagre ny bildeentitet
                using(BilderOrmDataContext bilderOrm = new BilderOrmDataContext())
                {
                    bilde.BildeSrc = bildenavn;

                    bilderOrm.Bildes.InsertOnSubmit(bilde);
                    bilderOrm.SubmitChanges();
                }

                ViewBag.LastetOpp = true;

            }catch(Exception ex)
            {
                ViewBag.LastetOpp = false;
                ViewBag.Feilmelding = ex.Message;
            }

            return View();
        }

        public ActionResult VisOpplastinger()
        {

             Session["Bilde"] = new List<Bilde>{
                 new Bilde { Tittel = "Bil", BildeSrc = "bil.jpg" },
                 new Bilde { Tittel = "Bil2", BildeSrc = "bil.jpg" };
                new Bilde { Tittel = "Bil3", BildeSrc = "bil.jpg" };
             }

            Session["Bilde"] = new Bilde { Tittel = "Bil", BildeSrc = "bil.jpg" };

            Bilde bilde = (Bilde)Session["Brukernavn"];
            String tittel = bilde.Tittel;
            String bildeSrc = bilde.BildeSrc;

            if (Session["Brukernavn"] != null)
            {
                ViewBag.Brukernavn = (string)Session["Brukernavn"];

                using (BilderOrmDataContext bilderOrm = new BilderOrmDataContext())
                {
                    List<Bilde> bildeliste = (from bilder in bilderOrm.Bildes
                                              select bilder).ToList();

                    return View(bildeliste);
                }

            }
            else
            {
                return RedirectToAction("LogIn");
            }
        }

        public ActionResult RedigerBilde(int? id) 
        {
            if (id != null)
            {
                using (BilderOrmDataContext bilderOrm = new BilderOrmDataContext())
                {
                    Bilde valgtBilde = (from bilder in bilderOrm.Bildes
                                        where bilder.Id == id
                                        select bilder).SingleOrDefault();

                    /*
                     * List<Bilde> searchResult = (from bilder in bilderOrm.Bildes
                                        where bilder.Tittel.Contains(soeketerm) 
                                        select bilder).SingleOrDefault();*/

                    return View(valgtBilde);
                }
            }
            return View();            
        }

        [HttpPost]
        public ActionResult RedigerBilde(Bilde bilde)
        {
            using(BilderOrmDataContext bilderOrm = new BilderOrmDataContext())
            {
                Bilde valgtBilde = (from bilder in bilderOrm.Bildes
                                    where bilder.Id == bilde.Id
                                    select bilder).SingleOrDefault();

                valgtBilde.Tittel = bilde.Tittel;

                bilderOrm.SubmitChanges();

                //return View(valgtBilde);

                return RedirectToAction("VisOpplastinger");
            }
        }

        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(Bruker bruker)
        {
            Session.Add("Brukernavn", bruker.Brukernavn);

            return RedirectToAction("VisOpplastinger");
        }

        public ActionResult LogOut()
        {
            Session.Remove("Brukernavn");

            return RedirectToAction("LastOppBilde");
        }

    }
}