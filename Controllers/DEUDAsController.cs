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
    public class DEUDAsController : Controller
    {
        private INVYBALEntities db = new INVYBALEntities();

		// GET: DEUDAs
		[AuthorizeUser(idOperacion: 1)]
		public ActionResult Index()
        {
            return View(db.DEUDAs.ToList());
        }

        // GET: DEUDAs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEUDA dEUDA = db.DEUDAs.Find(id);
            if (dEUDA == null)
            {
                return HttpNotFound();
            }
            return View(dEUDA);
        }

		// GET: DEUDAs/Create
		[AuthorizeUser(idOperacion: 1)]
		public ActionResult Create()
        {
            return View();
        }

        // POST: DEUDAs/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,dia,mes,anio,concepto1,concepto2,concepto3,deuda1,m_pago")] DEUDA dEUDA)
        {
            if (ModelState.IsValid)
            {
                db.DEUDAs.Add(dEUDA);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dEUDA);
        }

		// GET: DEUDAs/Edit/5
		[AuthorizeUser(idOperacion: 1)]
		public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEUDA dEUDA = db.DEUDAs.Find(id);
            if (dEUDA == null)
            {
                return HttpNotFound();
            }
            return View(dEUDA);
        }

        // POST: DEUDAs/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,dia,mes,anio,concepto1,concepto2,concepto3,deuda1,m_pago,abono")] DEUDA dEUDA)
        {
            if (ModelState.IsValid)
            {

				dEUDA.deuda1 = Convert.ToDecimal(dEUDA.deuda1 - dEUDA.abono);
				db.Entry(dEUDA).State = EntityState.Modified;				
				db.SaveChanges();
				BALANCE balfac = new BALANCE();

				balfac.dia = DateTime.Now.Day;
				balfac.mes = DateTime.Now.Month;
				balfac.anio = DateTime.Now.Year;
				balfac.concepto1 = dEUDA.concepto1 + dEUDA.dia + dEUDA.mes + dEUDA.anio;
				balfac.concepto2 = dEUDA.concepto2;
				balfac.concepto3 = "ABONO";
				balfac.ingreso = dEUDA.abono;
				balfac.gasto = Convert.ToDecimal(0);
				balfac.m_pago = dEUDA.m_pago;
				db.BALANCES.Add(balfac);
				db.SaveChanges();
				
				//DEUDA dEUDA = db.DEUDAs.Find(id);
				//db.DEUDAs.Remove(dEUDA);
				//db.SaveChanges();
				return RedirectToAction("Index");
            }
            return View(dEUDA);
        }

        // GET: DEUDAs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEUDA dEUDA = db.DEUDAs.Find(id);
            if (dEUDA == null)
            {
                return HttpNotFound();
            }
            return View(dEUDA);
        }

        // POST: DEUDAs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DEUDA dEUDA = db.DEUDAs.Find(id);
            db.DEUDAs.Remove(dEUDA);
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
