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
	public class FACTABAsController : Controller
	{
		private INVYBALEntities db = new INVYBALEntities();



		[HttpPost]

		public JsonResult getinsumo(int id_insumo)
		{


			INVENTARIO inventario = db.INVENTARIO.Find(id_insumo);
			int? medida = inventario.medida;
			MEDICION medicion = db.MEDICION.Find(medida);
			var response = medicion.medida;
			return Json(response);
		}
		[AuthorizeUser(idOperacion: 1)]
		// GET: FACTABAs
		public ActionResult Index()
		{
			return View(db.FACTABAS.ToList());
		}

		// GET: FACTABAs/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			FACTABA fACTABA = db.FACTABAS.Find(id);
			if (fACTABA == null)
			{
				return HttpNotFound();
			}

			var abastecimientos = (from a in db.INSUMOABAS
								   where a.idabas == id
								   select a).ToList();

			return View(fACTABA);
		}

		// GET: FACTABAs/Create
		public ActionResult Create()
		{
			ViewBag.insumo = new SelectList(db.INVENTARIO, "id", "descripcion");
			return View();
		}

		// POST: FACTABAs/Create
		// Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
		// más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(AbastViewModel model)
		{
			try
			{
				using (INVYBALEntities db = new INVYBALEntities())
				{


					FACTABA fACTABA = new FACTABA();

					fACTABA.dia = DateTime.Now.Day;
					fACTABA.mes = DateTime.Now.Month;
					fACTABA.anio = DateTime.Now.Year;
					fACTABA.factura = model.factura.ToUpper();
					fACTABA.efec = model.efec;
					fACTABA.transf = model.transf;
					fACTABA.tot_fact = 0;
					db.FACTABAS.Add(fACTABA);
					db.SaveChanges();
					decimal totl_factura = 0;

					foreach (var insum in model.insumos)
					{
						//------update inventario-----------
						var inventario = (from a in db.INVENTARIO
										  where a.id == insum.insumo
										  select a).FirstOrDefault();
						inventario.existencias = inventario.existencias + insum.cantidad;
						db.SaveChanges();
						//-------------------------------


						INSUMOABA it = new INSUMOABA();

						it.idabas = fACTABA.id;
						it.insumo = insum.insumo;
						it.cant = insum.cantidad;
						it.precio = insum.precio;
						db.INSUMOABAS.Add(it);
						totl_factura = totl_factura + Convert.ToDecimal(it.precio);
					}

					db.SaveChanges();

					fACTABA.tot_fact = totl_factura;
					db.SaveChanges();


					///////////*******UPDATE BALANCE************///////////
					BALANCE balfac = new BALANCE();

					if (fACTABA.efec == 0&& fACTABA.transf != 0)
					{
						balfac.dia = DateTime.Now.Day;
						balfac.mes = DateTime.Now.Month;
						balfac.anio = DateTime.Now.Year;
						balfac.concepto1 = "ABASTECIMIENTO";
						balfac.concepto2 = "FACTURA#:" + fACTABA.factura;
						balfac.concepto3 = "";
						balfac.ingreso = Convert.ToDecimal(0); ;
						balfac.m_pago = "TRANSFERENCIA";
						balfac.gasto = fACTABA.tot_fact;
						db.BALANCES.Add(balfac);
						db.SaveChanges();
					}
					if (fACTABA.transf == 0 && fACTABA.efec != 0)
					{
						balfac.dia = DateTime.Now.Day;
						balfac.mes = DateTime.Now.Month;
						balfac.anio = DateTime.Now.Year;
						balfac.concepto1 = "ABASTECIMIENTO";
						balfac.concepto2 = "FACTURA#:" + fACTABA.factura;
						balfac.concepto3 = "";
						balfac.ingreso = Convert.ToDecimal(0); ;
						balfac.m_pago = "EFECTIVO";
						balfac.gasto = fACTABA.tot_fact;
						db.BALANCES.Add(balfac);
						db.SaveChanges();
					}
					if (fACTABA.transf != 0 && fACTABA.efec != 0)
					{
						balfac.dia = DateTime.Now.Day;
						balfac.mes = DateTime.Now.Month;
						balfac.anio = DateTime.Now.Year;
						balfac.concepto1 = "ABASTECIMIENTO";
						balfac.concepto2 = "FACTURA#:" + fACTABA.factura;
						balfac.concepto3 = "";
						balfac.ingreso = Convert.ToDecimal(0); ;
						balfac.m_pago = "EFECTIVO";
						balfac.gasto = fACTABA.efec;
						db.BALANCES.Add(balfac);
						db.SaveChanges();




						balfac.dia = DateTime.Now.Day;
						balfac.mes = DateTime.Now.Month;
						balfac.anio = DateTime.Now.Year;
						balfac.concepto1 = "ABASTECIMIENTO";
						balfac.concepto2 = "FACTURA#:" + fACTABA.factura;
						balfac.concepto3 = "";
						balfac.ingreso = Convert.ToDecimal(0); ;
						balfac.m_pago = "TRANSFERENCIA";
						balfac.gasto = fACTABA.transf;
						db.BALANCES.Add(balfac);
						db.SaveChanges();
					}
				



					return RedirectToAction("Index");
				}


			}
			catch (Exception ex)
			{
				ViewBag.insumo = new SelectList(db.INVENTARIO, "id", "descripcion");
				return View();
			}



		}

		// GET: FACTABAs/Edit/5
		[AuthorizeUser(idOperacion: 1)]
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			FACTABA fACTABA = db.FACTABAS.Find(id);
			if (fACTABA == null)
			{
				return HttpNotFound();
			}
			return View(fACTABA);
		}

		// POST: FACTABAs/Edit/5
		// Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
		// más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "id,dia,mes,anio,factura,tot_fact")] FACTABA fACTABA)
		{
			if (ModelState.IsValid)
			{
				db.Entry(fACTABA).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(fACTABA);
		}

		// GET: FACTABAs/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			FACTABA fACTABA = db.FACTABAS.Find(id);
			if (fACTABA == null)
			{
				return HttpNotFound();
			}
			return View(fACTABA);
		}

		// POST: FACTABAs/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			FACTABA fACTABA = db.FACTABAS.Find(id);
			db.FACTABAS.Remove(fACTABA);
			db.SaveChanges();

			///////////*******UPDATE BALANCE***********//////////
			/*	var balance = (from a in db.BALANCES
							   where a.dia == fACTABA.dia &&
							   a.mes == fACTABA.mes &&
							   a.anio == fACTABA.anio &&
							   a.concepto1 == "ABASTECIMIENTO" &&
							   a.concepto2 == "FACTURA#: " + fACTABA.factura
							   select a).FirstOrDefault();

				BALANCE Obalance = db.BALANCES.Find(balance.id);
				db.BALANCES.Remove(Obalance);
				db.SaveChanges();*/




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
