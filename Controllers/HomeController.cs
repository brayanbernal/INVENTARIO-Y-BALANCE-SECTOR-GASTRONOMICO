using INVYBAL.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using INVYBAL.Models;
namespace INVYBAL.Controllers
{
	public class HomeController : Controller
	{
		private INVYBALEntities db = new INVYBALEntities();

		public ActionResult Index()
		{

			try
			{

				System.Nullable<Decimal> totalingreso =
		(from ingerso in db.BALANCES
		 select (decimal)ingerso.ingreso)
		.Sum();
				if (totalingreso == null)
				{
					totalingreso = 0;
				}
				ViewBag.ingresos = totalingreso;

				System.Nullable<Decimal> totalegreso =
					(from gasto in db.BALANCES
					 select (decimal)gasto.gasto)
		.Sum();
				if (totalegreso == null)
				{
					totalegreso = 0;
				}

				ViewBag.egreso = totalegreso;


				decimal? saldo = totalingreso - totalegreso;
				ViewBag.saldo = saldo;

				return View();
			}
			
			catch (Exception ex)
			{
				ViewBag.ingresos = 0;
				ViewBag.egreso = 0;
				ViewBag.saldo = 0;
				return View();
	}
}
		[AuthorizeUser(idOperacion:1)]
		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}