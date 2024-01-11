using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace INVYBAL.Controllers
{
    public class ACCESOController : Controller
    {
        // GET: ACCESO
        public ActionResult Login()
        {
			Session["User"] = null;
			return View();
			
        }
		[HttpPost]
		public ActionResult Login(int User, string Pass)
		{

			try
			{
				using (Models.INVYBALEntities db = new Models.INVYBALEntities())// CREAMOS CONEXION 
				{
					var oUser = (from d in db.USUARIOs
								 where d.documento == User && d.password == Pass.Trim()
								 select d).FirstOrDefault();
					if (oUser == null)
					{
						ViewBag.Error = "Usuario o contraseña invalida";
						return View();
					}

					Session["User"] = oUser;

				}

				return RedirectToAction("Index", "Home");
			}
			catch (Exception ex)
			{
				ViewBag.Error = ex.Message;
				return View();
			}

		}


	}
}