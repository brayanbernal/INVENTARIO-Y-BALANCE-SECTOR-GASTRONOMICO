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
	public class BALANCEsController : Controller
	{
		private INVYBALEntities db = new INVYBALEntities();

		// GET: BALANCEs
		public ActionResult EgresoDiario(int dia, int mes, int anio)
		{
			var filtrofecha = db.BALANCES.Where(i => i.dia == dia).Where(i => i.mes == mes).Where(i => i.anio == anio);
			return View(filtrofecha.ToList());
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult reporte(int dia, int mes, int anio)
		{
			//try
			//{
			 
			ViewBag.fech = dia + "/" + mes + "/" + anio;
			System.Nullable<Decimal> VENTA_EFECTIVO = (from ingreso in db.BALANCES
													   where ingreso.mes == mes && ingreso.anio == anio && ingreso.dia == dia && ingreso.concepto3 == "VENTA" && ingreso.m_pago == "EFECTIVO"
													   select ingreso.ingreso).Sum();

			System.Nullable<Decimal> VENTA_TARJETA = (from ingreso in db.BALANCES
													  where ingreso.mes == mes && ingreso.anio == anio && ingreso.dia == dia && ingreso.concepto3 == "VENTA" && ingreso.m_pago == "TARJETA"
													  select ingreso.ingreso).Sum();

			System.Nullable<Decimal> VENTA_TRANSACCION = (from ingreso in db.BALANCES
														  where ingreso.mes == mes && ingreso.anio == anio && ingreso.dia == dia && ingreso.concepto3 == "VENTA" && (ingreso.m_pago == "TRANSACCIÓN" || ingreso.m_pago == "TRANSACCION")
														  select ingreso.ingreso).Sum();

			if (VENTA_EFECTIVO == null)
			{
				VENTA_EFECTIVO = 0;
			}

			if (VENTA_TARJETA == null)
			{
				VENTA_TARJETA = 0;
			}

			if (VENTA_TRANSACCION == null)
			{
				VENTA_TRANSACCION = 0;
			}

			ViewBag.ven_efe = VENTA_EFECTIVO; ViewBag.ven_tar = VENTA_TARJETA; ViewBag.ven_tra = VENTA_TRANSACCION;





			System.Nullable<Decimal> OT_INGR_EFECTIVO = (from ingreso in db.BALANCES
														 where ingreso.mes == mes && ingreso.anio == anio && ingreso.dia == dia && ingreso.concepto3 != "VENTA" && ingreso.m_pago == "EFECTIVO"
														 select ingreso.ingreso).Sum();

			System.Nullable<Decimal> OT_INGR_TARJETA = (from ingreso in db.BALANCES
														where ingreso.mes == mes && ingreso.anio == anio && ingreso.dia == dia && ingreso.concepto3 != "VENTA" && ingreso.m_pago == "TARJETA"
														select ingreso.ingreso).Sum();

			System.Nullable<Decimal> OT_INGR_TRANSACCION = (from ingreso in db.BALANCES
															where ingreso.mes == mes && ingreso.anio == anio && ingreso.dia == dia && ingreso.concepto3 != "VENTA" && (ingreso.m_pago == "TRANSACCIÓN" || ingreso.m_pago == "TRANSACCION")
															select ingreso.ingreso).Sum();

			if (OT_INGR_EFECTIVO == null)
			{
				OT_INGR_EFECTIVO = 0;
			}

			if (OT_INGR_TARJETA == null)
			{
				OT_INGR_TARJETA = 0;
			}

			if (OT_INGR_TRANSACCION == null)
			{
				OT_INGR_TRANSACCION = 0;
			}

			ViewBag.ot_efe = OT_INGR_EFECTIVO; ViewBag.ot_tar = OT_INGR_TARJETA; ViewBag.ot_tra = OT_INGR_TRANSACCION;

			System.Nullable<Decimal> EGRESOS_EFECTIVO = (from egreso in db.BALANCES
														 where egreso.mes == mes && egreso.anio == anio && egreso.dia == dia
														 select egreso.gasto).Sum();

			System.Nullable<Decimal> EGRESOS_TARJETA = (from egreso in db.BALANCES
														where egreso.mes == mes && egreso.anio == anio && egreso.dia == dia
														select egreso.gasto).Sum();

			System.Nullable<Decimal> EGRESOS_TRANSACCION = (from egreso in db.BALANCES
															where egreso.mes == mes && egreso.anio == anio && egreso.dia == dia
															select egreso.gasto).Sum();

			if (EGRESOS_EFECTIVO == null)
			{
				EGRESOS_EFECTIVO = 0;
			}

			if (EGRESOS_TARJETA == null)
			{
				EGRESOS_TARJETA = 0;
			}

			if (EGRESOS_TRANSACCION == null)
			{
				EGRESOS_TRANSACCION = 0;
			}

			ViewBag.egre_efe = EGRESOS_EFECTIVO; ViewBag.egre_tar = EGRESOS_TARJETA; ViewBag.egre_tra = EGRESOS_TRANSACCION;

			Decimal TOT_EFE = Convert.ToDecimal((VENTA_EFECTIVO + OT_INGR_EFECTIVO) - EGRESOS_EFECTIVO);
			Decimal TOT_TAR = Convert.ToDecimal((VENTA_TARJETA + OT_INGR_TARJETA) - EGRESOS_TARJETA);
			Decimal TOT_TRA = Convert.ToDecimal((VENTA_TRANSACCION + OT_INGR_TRANSACCION) - EGRESOS_TRANSACCION);

			ViewBag.tot_efe = TOT_EFE;
			ViewBag.tot_tar = TOT_TAR;
			ViewBag.tot_tra = TOT_TRA;

			var filtrofecha = db.DIARIOVENTAS.Where(i => i.dia == dia).Where(i => i.mes == mes).Where(i => i.anio == anio);
			return View(filtrofecha.ToList());

		}
		[AuthorizeUser(idOperacion: 1)]
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

				return View(db.BALANCES.ToList());
			}

			catch (Exception ex)
			{
				ViewBag.ingresos = 0;
				ViewBag.egreso = 0;
				ViewBag.saldo = 0;
				return View(db.BALANCES.ToList());
			}
		}



		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Index(int dia, int mes, int anio)
		{
			try
			{

				if (dia == 0 && mes != 0 && anio != 0)
				{

					System.Nullable<Decimal> totalingreso = (from ingreso in db.BALANCES
															 where ingreso.mes == mes && ingreso.anio == anio
															 select (decimal)ingreso.ingreso).Sum();
					if (totalingreso == null)
					{
						totalingreso = 0;
					}
					ViewBag.ingresos = totalingreso;


					System.Nullable<Decimal> totalegreso = (from egreso in db.BALANCES
															where egreso.mes == mes && egreso.anio == anio
															select (decimal)egreso.gasto).Sum();
					if (totalegreso == null)
					{
						totalegreso = 0;
					}
					ViewBag.egreso = totalegreso;

					decimal? saldo = totalingreso - totalegreso;
					ViewBag.saldo = saldo;



					var filtrofecha = db.BALANCES.Where(i => i.mes == mes).Where(i => i.anio == anio);
					return View(filtrofecha.ToList());
				}
				else
				{
					if (dia == 0 && mes == 0 && anio != 0)
					{

						System.Nullable<Decimal> totalingreso = (from ingreso in db.BALANCES
																 where ingreso.anio == anio
																 select (decimal)ingreso.ingreso).Sum();
						if (totalingreso == null)
						{
							totalingreso = 0;
						}
						ViewBag.ingresos = totalingreso;


						System.Nullable<Decimal> totalegreso = (from egreso in db.BALANCES
																where egreso.anio == anio
																select (decimal)egreso.gasto).Sum();
						if (totalegreso == null)
						{
							totalegreso = 0;
						}
						ViewBag.egreso = totalegreso;

						decimal? saldo = totalingreso - totalegreso;
						ViewBag.saldo = saldo;



						var filtroanio = db.BALANCES.Where(i => i.anio == anio);
						return View(filtroanio.ToList());
					}
					else
					{
						if (dia != 0 && mes != 0 && anio != 0)
						{
							System.Nullable<Decimal> totalingreso = (from ingreso in db.BALANCES
																	 where ingreso.dia == dia && ingreso.mes == mes && ingreso.anio == anio
																	 select (decimal)ingreso.ingreso).Sum();
							if (totalingreso == null)
							{
								totalingreso = 0;
							}
							ViewBag.ingresos = totalingreso;


							System.Nullable<Decimal> totalegreso = (from egreso in db.BALANCES
																	where egreso.dia == dia && egreso.mes == mes && egreso.anio == anio
																	select (decimal)egreso.gasto).Sum();
							if (totalegreso == null)
							{
								totalegreso = 0;
							}
							ViewBag.egreso = totalegreso;

							decimal? saldo = totalingreso - totalegreso;
							ViewBag.saldo = saldo;



							var filtrofecha = db.BALANCES.Where(i => i.dia == dia).Where(i => i.mes == mes).Where(i => i.anio == anio);
							return View(filtrofecha.ToList());

						}
						else
						{

							return View(db.BALANCES.ToList());
						}
					}
				}
			}
			catch (Exception ex)
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
				ViewBag.MSJ = "REGISTROS NO ENCONTRADOS";
				return View(db.BALANCES.ToList());
			}
		}

		// GET: BALANCEs/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			BALANCE bALANCE = db.BALANCES.Find(id);
			if (bALANCE == null)
			{
				return HttpNotFound();
			}
			return View(bALANCE);
		}

		// GET: BALANCEs/Create
		[AuthorizeUser(idOperacion: 1)]
		public ActionResult Create()
		{
			return View();
		}

		// POST: BALANCEs/Create
		// Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
		// más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "id,dia,mes,anio,concepto1,concepto2,ingreso,gasto,concepto3,m_pago")] BALANCE bALANCE)
		{
			if (bALANCE.ingreso == null) { bALANCE.ingreso = 0; }
			if (bALANCE.gasto == null) { bALANCE.gasto = 0; }




			if (ModelState.IsValid)
			{
				db.BALANCES.Add(bALANCE);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			ViewBag.alert = "erInGa";
			return View(bALANCE);


		}
		[AuthorizeUser(idOperacion: 1)]
		// GET: BALANCEs/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			BALANCE bALANCE = db.BALANCES.Find(id);
			if (bALANCE == null)
			{
				return HttpNotFound();
			}
			return View(bALANCE);
		}

		// POST: BALANCEs/Edit/5
		// Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
		// más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "id,dia,mes,anio,concepto1,concepto2,ingreso,gasto,concepto3,m_pago")] BALANCE bALANCE)
		{
			if (ModelState.IsValid)
			{
				db.Entry(bALANCE).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(bALANCE);
		}

		// GET: BALANCEs/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			BALANCE bALANCE = db.BALANCES.Find(id);
			if (bALANCE == null)
			{
				return HttpNotFound();
			}
			return View(bALANCE);
		}

		// POST: BALANCEs/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			BALANCE bALANCE = db.BALANCES.Find(id);
			db.BALANCES.Remove(bALANCE);
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
