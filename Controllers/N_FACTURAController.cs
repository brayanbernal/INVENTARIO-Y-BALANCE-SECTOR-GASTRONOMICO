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
    public class N_FACTURAController : Controller
    {
        private INVYBALEntities db = new INVYBALEntities();

		// GET: N_FACTURA
		[AuthorizeUser(idOperacion: 1)]
		public ActionResult Index()
        {
            return View(db.N_FACTURA.ToList());
        }

		// GET: N_FACTURA/Details/5
		[AuthorizeUser(idOperacion: 1)]
		public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            N_FACTURA n_FACTURA = db.N_FACTURA.Find(id);
            if (n_FACTURA == null)
            {
                return HttpNotFound();
            }
            return View(n_FACTURA);
        }
		[AuthorizeUser(idOperacion: 1)]
		// GET: N_FACTURA/Create
		public ActionResult Create()
        {
            return View();
        }

        // POST: N_FACTURA/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,fact_ini,fact_fin,con_act")] N_FACTURA n_FACTURA)
        {
            if (ModelState.IsValid)
            {
                db.N_FACTURA.Add(n_FACTURA);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(n_FACTURA);
        }
		[AuthorizeUser(idOperacion: 1)]
		// GET: N_FACTURA/Edit/5
		public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            N_FACTURA n_FACTURA = db.N_FACTURA.Find(id);
            if (n_FACTURA == null)
            {
                return HttpNotFound();
            }
            return View(n_FACTURA);
        }

        // POST: N_FACTURA/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,fact_ini,fact_fin,con_act")] N_FACTURA n_FACTURA)
        {
            if (ModelState.IsValid)
            {
                db.Entry(n_FACTURA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(n_FACTURA);
        }

		// GET: N_FACTURA/Delete/5
		[AuthorizeUser(idOperacion: 1)]
		public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            N_FACTURA n_FACTURA = db.N_FACTURA.Find(id);
            if (n_FACTURA == null)
            {
                return HttpNotFound();
            }
            return View(n_FACTURA);
        }

        // POST: N_FACTURA/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            N_FACTURA n_FACTURA = db.N_FACTURA.Find(id);
            db.N_FACTURA.Remove(n_FACTURA);
            db.SaveChanges();
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
