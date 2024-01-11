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
using INVYBAL.Models.ViewModels;

namespace INVYBAL.Controllers
{
    public class PRODUCTOesController : Controller
    {
        private INVYBALEntities db = new INVYBALEntities();

		// GET: PRODUCTOes

		[AuthorizeUser(idOperacion: 1)]
		public ActionResult Index()
        {
			var pRODUCTO = db.PRODUCTO.Include(p => p.INVENTARIO).Include(p => p.INVENTARIO1).Include(p => p.INVENTARIO2).Include(p => p.INVENTARIO3).Include(p => p.INVENTARIO4).Include(p => p.INVENTARIO5).Include(p => p.INVENTARIO6);
			return View(pRODUCTO.ToList());
        }
		[AuthorizeUser(idOperacion: 1)]
		// GET: PRODUCTOes/Details/5
		public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRODUCTO pRODUCTO = db.PRODUCTO.Find(id);
            if (pRODUCTO == null)
            {
                return HttpNotFound();
            }
            return View(pRODUCTO);
        }

		// GET: PRODUCTOes/Create
		[AuthorizeUser(idOperacion: 1)]
		public ActionResult Create()
        {
            ViewBag.tipo1 = new SelectList(db.INVENTARIO, "id", "descripcion");
            ViewBag.tipo2 = new SelectList(db.INVENTARIO, "id", "descripcion");
            ViewBag.tipo3 = new SelectList(db.INVENTARIO, "id", "descripcion");
            ViewBag.tipo4 = new SelectList(db.INVENTARIO, "id", "descripcion");
            ViewBag.tipo5 = new SelectList(db.INVENTARIO, "id", "descripcion");
            ViewBag.tipo6 = new SelectList(db.INVENTARIO, "id", "descripcion");
            ViewBag.tipo7 = new SelectList(db.INVENTARIO, "id", "descripcion");
            ViewBag.insumo = new SelectList(db.PRODUCTO, "cod_producto", "descripcion");
            ViewBag.categoria = new SelectList(db.CATEGORIAS, "id", "categoria");
            return View();
        }

        // POST: PRODUCTOes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public ActionResult Create(productosViewModel model)
        {
           
			try
			{
				using (INVYBALEntities db =new INVYBALEntities())
				{

					PRODUCTO Oproducto = new PRODUCTO();
					if (model.cantidad1 == null)
					{
						model.cantidad1 = 0;
					}
					if (model.cantidad2 == null)
					{
						model.cantidad2 = 0;
					}
					if (model.cantidad3 == null)
					{
						model.cantidad3 = 0;
					}
					if (model.cantidad4 == null)
					{
						model.cantidad4 = 0;
					}
					if (model.cantidad5 == null)
					{
						model.cantidad5 = 0;
					}
					if (model.cantidad6 == null)
					{
						model.cantidad6 = 0;
					}
					if (model.cantidad7 == null)
					{
						model.cantidad7 = 0;
					}
					//FIN CANTIDAD INGREDIENTES--------------------

				

					Oproducto.cod_producto = model.cod_producto;
					Oproducto.descripcion = model.descripcion.ToUpper();
					Oproducto.precio = model.precio;	
					Oproducto.tipo1 = model.tipo1;
					Oproducto.cantidad1 = model.cantidad1;
					Oproducto.tipo2 = model.tipo2;
					Oproducto.cantidad2 = model.cantidad2;
					Oproducto.tipo3 = model.tipo3;
					Oproducto.cantidad3 = model.cantidad3;
					Oproducto.tipo4 = model.tipo4;
					Oproducto.cantidad4 = model.cantidad4;
					Oproducto.tipo5 = model.tipo5;
					Oproducto.cantidad5 = model.cantidad5;
					Oproducto.tipo6 = model.tipo6;
					Oproducto.cantidad6 = model.cantidad6;
					Oproducto.tipo7 = model.tipo7;
					Oproducto.cantidad7 = model.cantidad7;
					Oproducto.categoria = model.categoria;

					if (db.PRODUCTO.Any(a => a.cod_producto == model.cod_producto))
					{

						ModelState.AddModelError("cod_producto", "Producto Registrado Anteriormente ");
						ViewBag.alert = "registrado";
					}
					
					db.PRODUCTO.Add(Oproducto);
					db.SaveChanges();

					foreach (var Op in model.opciones)
					{
						OPCIONE Opc = new OPCIONE();
						Opc.insumoOp = Op.insumoOp;
						Opc.producto = Oproducto.cod_producto;
						Opc.cantidadOp = 1;
						Opc.precioOp = Op.precioOp;
						if (Opc.insumoOp != null) { 
						db.OPCIONES.Add(Opc);
						db.SaveChanges();
						}
						else
						{

							return RedirectToAction("Index");
						}

					}
					
				}


					return RedirectToAction("Index");
			}
			catch(Exception ex)
			{
				ViewBag.tipo1 = new SelectList(db.INVENTARIO, "id", "descripcion", model.tipo1);
				ViewBag.tipo2 = new SelectList(db.INVENTARIO, "id", "descripcion", model.tipo2);
				ViewBag.tipo3 = new SelectList(db.INVENTARIO, "id", "descripcion", model.tipo3);
				ViewBag.tipo4 = new SelectList(db.INVENTARIO, "id", "descripcion", model.tipo4);
				ViewBag.tipo5 = new SelectList(db.INVENTARIO, "id", "descripcion", model.tipo5);
				ViewBag.tipo6 = new SelectList(db.INVENTARIO, "id", "descripcion", model.tipo6);
				ViewBag.tipo7 = new SelectList(db.INVENTARIO, "id", "descripcion", model.tipo7);
				ViewBag.insumo = new SelectList(db.PRODUCTO, "cod_producto", "descripcion");
				ViewBag.categoria = new SelectList(db.CATEGORIAS, "id", "categoria");


				return View();
			}


	


		}

		// GET: PRODUCTOes/Edit/5
		[AuthorizeUser(idOperacion: 1)]
		public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRODUCTO pRODUCTO = db.PRODUCTO.Find(id);
            if (pRODUCTO == null)
            {
                return HttpNotFound();
            }
            ViewBag.tipo1 = new SelectList(db.INVENTARIO, "id", "descripcion", pRODUCTO.tipo1);
            ViewBag.tipo2 = new SelectList(db.INVENTARIO, "id", "descripcion", pRODUCTO.tipo2);
            ViewBag.tipo3 = new SelectList(db.INVENTARIO, "id", "descripcion", pRODUCTO.tipo3);
            ViewBag.tipo4 = new SelectList(db.INVENTARIO, "id", "descripcion", pRODUCTO.tipo4);
            ViewBag.tipo5 = new SelectList(db.INVENTARIO, "id", "descripcion", pRODUCTO.tipo5);
            ViewBag.tipo6 = new SelectList(db.INVENTARIO, "id", "descripcion", pRODUCTO.tipo6);
            ViewBag.tipo7 = new SelectList(db.INVENTARIO, "id", "descripcion", pRODUCTO.tipo7);
			ViewBag.cat = new SelectList(db.CATEGORIAS, "id", "categoria");
			return View(pRODUCTO);
        }

        // POST: PRODUCTOes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cod_producto,descripcion,precio,tipo1,cantidad1,tipo2,cantidad2,tipo3,cantidad3,tipo4,cantidad4,tipo5,cantidad5,tipo6,cantidad6,tipo7,cantidad7,categoria")] PRODUCTO pRODUCTO)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pRODUCTO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.tipo1 = new SelectList(db.INVENTARIO, "id", "descripcion", pRODUCTO.tipo1);
            ViewBag.tipo2 = new SelectList(db.INVENTARIO, "id", "descripcion", pRODUCTO.tipo2);
            ViewBag.tipo3 = new SelectList(db.INVENTARIO, "id", "descripcion", pRODUCTO.tipo3);
            ViewBag.tipo4 = new SelectList(db.INVENTARIO, "id", "descripcion", pRODUCTO.tipo4);
            ViewBag.tipo5 = new SelectList(db.INVENTARIO, "id", "descripcion", pRODUCTO.tipo5);
            ViewBag.tipo6 = new SelectList(db.INVENTARIO, "id", "descripcion", pRODUCTO.tipo6);
            ViewBag.tipo7 = new SelectList(db.INVENTARIO, "id", "descripcion", pRODUCTO.tipo7);
			ViewBag.cat = new SelectList(db.CATEGORIAS, "id", "categoria");
			return View(pRODUCTO);
        }

		// GET: PRODUCTOes/Delete/5
		[AuthorizeUser(idOperacion: 1)]
		public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRODUCTO pRODUCTO = db.PRODUCTO.Find(id);
            if (pRODUCTO == null)
            {
                return HttpNotFound();
            }
            return View(pRODUCTO);
        }

        // POST: PRODUCTOes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PRODUCTO pRODUCTO = db.PRODUCTO.Find(id);
            db.PRODUCTO.Remove(pRODUCTO);
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
