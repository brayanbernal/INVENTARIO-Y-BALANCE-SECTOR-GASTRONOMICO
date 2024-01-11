using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using INVYBAL.Filters;
using INVYBAL.Models;

namespace INVYBAL.Controllers
{
    public class INVENTARIOsController : Controller
    {
        private INVYBALEntities db = new INVYBALEntities();

		// GET: INVENTARIOs
		[AuthorizeUser(idOperacion: 1)]
		public ActionResult Index()
        {
            var iNVENTARIO = db.INVENTARIO.Include(i => i.MEDICION);
            return View(iNVENTARIO.ToList());
        }

		// GET: INVENTARIOs/Details/5
		[AuthorizeUser(idOperacion: 1)]
		public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            INVENTARIO iNVENTARIO = db.INVENTARIO.Find(id);
            if (iNVENTARIO == null)
            {
                return HttpNotFound();
            }
            return View(iNVENTARIO);
        }

		// GET: INVENTARIOs/Create
		[AuthorizeUser(idOperacion: 1)]
		public ActionResult Create()
        {
            ViewBag.medida = new SelectList(db.MEDICION, "id", "medida");
            return View();
        }

        // POST: INVENTARIOs/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion,medida,existencias")] INVENTARIO iNVENTARIO)
        {


			if(iNVENTARIO.existencias==null)
			{
				iNVENTARIO.existencias = 0;
			}

			iNVENTARIO.descripcion = iNVENTARIO.descripcion.ToUpper();

			if (db.INVENTARIO.Any(a => a.descripcion == iNVENTARIO.descripcion))
			{

				ModelState.AddModelError("descripcion", "Producto Registrado Anteriormente ");
				ViewBag.alert = "errProd";
				ViewData["error"] = "errProd";

			}
			else
			{
				if (iNVENTARIO.existencias < 0)
				{
					ModelState.AddModelError("existencias", "Existencias mayor a 0");
					ViewBag.alert = "Existenciasr mayor a 0";
				}
				else
				{
					if (ModelState.IsValid)
					{
						db.INVENTARIO.Add(iNVENTARIO);
						db.SaveChanges();
						return RedirectToAction("Index");
					}
				}
			}

		
            ViewBag.medida = new SelectList(db.MEDICION, "id", "medida", iNVENTARIO.medida);
            return View(iNVENTARIO);
        }

		// GET: INVENTARIOs/Edit/5
		[AuthorizeUser(idOperacion: 1)]
		public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            INVENTARIO iNVENTARIO = db.INVENTARIO.Find(id);
            if (iNVENTARIO == null)
            {
                return HttpNotFound();
            }
            ViewBag.medida = new SelectList(db.MEDICION, "id", "medida", iNVENTARIO.medida);
            return View(iNVENTARIO);
        }

        // POST: INVENTARIOs/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion,medida,existencias")] INVENTARIO iNVENTARIO)
        {
			if (iNVENTARIO.existencias < 0)
			{
				ModelState.AddModelError("existencias", "Existencias mayores o iguales a 0");
				ViewBag.alert = "Existencias mayores o iguales a 0";
			}
			else
			{
				if (ModelState.IsValid)
				{
					db.Entry(iNVENTARIO).State = EntityState.Modified;
					db.SaveChanges();
					return RedirectToAction("Index");
				}
			}
            ViewBag.medida = new SelectList(db.MEDICION, "id", "medida", iNVENTARIO.medida);
            return View(iNVENTARIO);
        }

		// GET: INVENTARIOs/Delete/5
		[AuthorizeUser(idOperacion: 1)]
		public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            INVENTARIO iNVENTARIO = db.INVENTARIO.Find(id);
            if (iNVENTARIO == null)
            {
                return HttpNotFound();
            }
            return View(iNVENTARIO);
        }

        // POST: INVENTARIOs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            INVENTARIO iNVENTARIO = db.INVENTARIO.Find(id);
            db.INVENTARIO.Remove(iNVENTARIO);

			try
			{
				db.SaveChanges();
			}
			catch (Exception ex)
			{
				if (ex.InnerException != null ||
			  ex.InnerException.InnerException != null ||
			  ex.InnerException.InnerException.Message.Contains("REFERENCE")
			
			  )
				{
					ModelState.AddModelError(string.Empty, "El INSUMO no puede eliminarse. Tiene otros registros relacionados.");
				}
				else
				{
					ModelState.AddModelError(string.Empty, ex.Message);
				}

				
				return View(); 
			}
           


			return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
