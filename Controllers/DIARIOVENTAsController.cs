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
    public class DIARIOVENTAsController : Controller
    {
        private INVYBALEntities db = new INVYBALEntities();

        // GET: DIARIOVENTAs
        public ActionResult Index()
        {
            var dIARIOVENTAS = db.DIARIOVENTAS.Include(d => d.PRODUCTO);
            return View(dIARIOVENTAS.ToList());
        }

        // GET: DIARIOVENTAs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DIARIOVENTA dIARIOVENTA = db.DIARIOVENTAS.Find(id);
            if (dIARIOVENTA == null)
            {
                return HttpNotFound();
            }
            return View(dIARIOVENTA);
        }

        // GET: DIARIOVENTAs/Create
        public ActionResult Create()
        {
            ViewBag.codigo = new SelectList(db.PRODUCTO, "cod_producto", "descripcion");
            return View();
        }

        // POST: DIARIOVENTAs/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,dia,mes,anio,codigo,cantidad,pre_uni,pret_tot")] DIARIOVENTA dIARIOVENTA)
        {
            if (ModelState.IsValid)
            {
                db.DIARIOVENTAS.Add(dIARIOVENTA);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.codigo = new SelectList(db.PRODUCTO, "cod_producto", "descripcion", dIARIOVENTA.codigo);
            return View(dIARIOVENTA);
        }

        // GET: DIARIOVENTAs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DIARIOVENTA dIARIOVENTA = db.DIARIOVENTAS.Find(id);
            if (dIARIOVENTA == null)
            {
                return HttpNotFound();
            }
            ViewBag.codigo = new SelectList(db.PRODUCTO, "cod_producto", "descripcion", dIARIOVENTA.codigo);
            return View(dIARIOVENTA);
        }

        // POST: DIARIOVENTAs/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,dia,mes,anio,codigo,cantidad,pre_uni,pret_tot")] DIARIOVENTA dIARIOVENTA)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dIARIOVENTA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.codigo = new SelectList(db.PRODUCTO, "cod_producto", "descripcion", dIARIOVENTA.codigo);
            return View(dIARIOVENTA);
        }

        // GET: DIARIOVENTAs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DIARIOVENTA dIARIOVENTA = db.DIARIOVENTAS.Find(id);
            if (dIARIOVENTA == null)
            {
                return HttpNotFound();
            }
            return View(dIARIOVENTA);
        }

        // POST: DIARIOVENTAs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DIARIOVENTA dIARIOVENTA = db.DIARIOVENTAS.Find(id);
            db.DIARIOVENTAS.Remove(dIARIOVENTA);
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
