using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace INVYBAL.Controllers
{
	public class ErrorController : Controller
	{
		[HttpGet]
		public ActionResult UnauthorizedOperation(String operacion, String modulo, String msjeErrorExcepcion)
		{
			return View();
		}
	}
}