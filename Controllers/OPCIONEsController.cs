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
    public class OPCIONEsController : Controller
    {
        private INVYBALEntities db = new INVYBALEntities();

        // GET: OPCIONEs
        public ActionResult Index()
        {
            var oPCIONES = db.OPCIONES.Include(o => o.PRODUCTO1).Include(o => o.PRODUCTO2);
            return View(oPCIONES.ToList());
        }

        // GET: OPCIONEs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OPCIONE oPCIONE = db.OPCIONES.Find(id);
            if (oPCIONE == null)
            {
                return HttpNotFound();
            }
            return View(oPCIONE);
        }

        // GET: OPCIONEs/Create
        public ActionResult Create()
        {
            ViewBag.insumoOp = new SelectList(db.PRODUCTO, "cod_producto", "descripcion");
            ViewBag.producto = new SelectList(db.PRODUCTO, "cod_producto", "descripcion");
            return View();
        }

        // POST: OPCIONEs/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,insumoOp,producto,cantidadOp,precioOp")] OPCIONE oPCIONE)
        {
            if (ModelState.IsValid)
            {
				if(oPCIONE.precioOp==0 || oPCIONE.precioOp==null)
				{
					oPCIONE.precioOp = Convert.ToDecimal(0);
				}
				oPCIONE.cantidadOp = Convert.ToDecimal(1);

                db.OPCIONES.Add(oPCIONE);
                db.SaveChanges();
				return Redirect("~/PRODUCTOes/Index");
			}

            ViewBag.insumoOp = new SelectList(db.PRODUCTO, "cod_producto", "descripcion", oPCIONE.insumoOp);
            ViewBag.producto = new SelectList(db.PRODUCTO, "cod_producto", "descripcion", oPCIONE.producto);
            return View(oPCIONE);
        }

        // GET: OPCIONEs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OPCIONE oPCIONE = db.OPCIONES.Find(id);
            if (oPCIONE == null)
            {
                return HttpNotFound();
            }
            ViewBag.insumoOp = new SelectList(db.PRODUCTO, "cod_producto", "descripcion", oPCIONE.insumoOp);
            ViewBag.producto = new SelectList(db.PRODUCTO, "cod_producto", "descripcion", oPCIONE.producto);
            return View(oPCIONE);
        }

        // POST: OPCIONEs/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,insumoOp,producto,cantidadOp,precioOp")] OPCIONE oPCIONE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oPCIONE).State = EntityState.Modified;
                db.SaveChanges();
				return Redirect("~/PRODUCTOes/Index");
			}
            ViewBag.insumoOp = new SelectList(db.PRODUCTO, "cod_producto", "descripcion", oPCIONE.insumoOp);
            ViewBag.producto = new SelectList(db.PRODUCTO, "cod_producto", "descripcion", oPCIONE.producto);
            return View(oPCIONE);
        }

        // GET: OPCIONEs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OPCIONE oPCIONE = db.OPCIONES.Find(id);
            if (oPCIONE == null)
            {
                return HttpNotFound();
            }
            return View(oPCIONE);
        }

        // POST: OPCIONEs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OPCIONE oPCIONE = db.OPCIONES.Find(id);
            db.OPCIONES.Remove(oPCIONE);
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
