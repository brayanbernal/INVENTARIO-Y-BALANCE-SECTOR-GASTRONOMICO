using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using INVYBAL.helper;
using INVYBAL.Models;
using INVYBAL.Models.ViewModels;

namespace INVYBAL.Controllers
{
	public class FACTURAsController : Controller
	{
		private INVYBALEntities db = new INVYBALEntities();

		// GET: FACTURAs

		[HttpPost]

		public JsonResult getcliente(int documento)
		{


			CLIENTE clientes = db.CLIENTEs.Find(documento);
			var response = clientes.nombre;
			return Json(response);
		}
		[HttpPost]
		public JsonResult getpreciosproductos(int id)
		{
			PRODUCTO productos = db.PRODUCTO.Find(id);
			var response = productos.precio;
			return Json(response);
		}
		[HttpPost]
		public JsonResult getpreciosopciones(int id)
		{
			OPCIONE opciones = db.OPCIONES.Find(id);
			var response = opciones.precioOp;
			return Json(response);
		}
		[HttpPost]
		public JsonResult getnomopcion(int c_opc)
		{
			var response = "";
			OPCIONE opcione = db.OPCIONES.Find(c_opc);
			if (opcione.insumoOp != null)
			{
				INVENTARIO inventario = db.INVENTARIO.Find(opcione.insumoOp);
				response = inventario.descripcion;

			}
			else
			{
				response = "";
			}
			return Json(response);

		}
		public JsonResult getopciones(int idopciones)
		{
			HomeHelper homehelper = new HomeHelper();

			var listopciones = homehelper.getopciones(idopciones);

			return Json(listopciones, JsonRequestBehavior.AllowGet);

		}
		public ActionResult Index()
		{
			var fACTURAs = db.FACTURAs.Include(f => f.CLIENTE1).Include(f => f.EMPLEADO1).Include(f => f.ITEMVENTAs);

			return View(fACTURAs.ToList());
		}

		// GET: FACTURAs/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			FACTURA fACTURA = db.FACTURAs.Find(id);
			if (fACTURA == null)
			{
				return HttpNotFound();

			}


			return View(fACTURA);
		}

		// GET: FACTURAs/Create
		public ActionResult Create()
		{

			ViewBag.producto = new SelectList(db.PRODUCTO, "cod_producto", "FullName");
			ViewBag.codigo = new SelectList(db.PRODUCTO, "cod_producto", "cod_producto");
			ViewBag.cliente = new SelectList(db.CLIENTEs, "documento", "documento");
			ViewBag.empleado = new SelectList(db.EMPLEADOes, "documento", "nombre");
			ViewBag.opcion = new SelectList(string.Empty, "Value", "Text");
			return View();
		}

		// POST: FACTURAs/Create
		// Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
		// más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(ventasViewModel model)
		{

			try
			{
				using (INVYBALEntities db = new INVYBALEntities())
				{

					FACTURA fACTURA = new FACTURA();

					fACTURA.dia = DateTime.Now.Day;
					fACTURA.mes = DateTime.Now.Month;
					fACTURA.anio = DateTime.Now.Year;
					fACTURA.cliente = model.cliente;
					fACTURA.total_fac = Convert.ToDecimal(model.total_fac);
					fACTURA.efectivo = Convert.ToDecimal(model.efectivo);
					fACTURA.transaccion = Convert.ToDecimal(model.transaccion);
					fACTURA.tarjeta = Convert.ToDecimal(model.tarjeta);
					fACTURA.empleado = model.empleado;
					fACTURA.descuento = Convert.ToDecimal(model.descuento);
					fACTURA.extra = Convert.ToDecimal(model.extra);

					N_FACTURA n_FACTURA = db.N_FACTURA.Find(1);


					var m_pago = fACTURA.efectivo + fACTURA.transaccion + fACTURA.tarjeta;
					if (m_pago < fACTURA.total_fac)
					{

						fACTURA.cartera = 1;
						fACTURA.consecutivo_fact = 0;

					}
					else
					{
						fACTURA.consecutivo_fact = n_FACTURA.con_act;
						var nfactura = (from a in db.N_FACTURA
										where a.id == 1
										select a).FirstOrDefault();

						nfactura.con_act = nfactura.con_act + 1;
						db.SaveChanges();
					}
					db.FACTURAs.Add(fACTURA);
					db.SaveChanges();

					//-----------------------------------------------------------------------

					CLIENTE cliente = db.CLIENTEs.Find(fACTURA.cliente);
					var nomcliente = cliente.nombre;
					if (m_pago < fACTURA.total_fac)
					{
						var deu = fACTURA.total_fac - m_pago;
						//------update inventario-----------
						DEUDA deuda = new DEUDA();

						deuda.dia = DateTime.Now.Day;
						deuda.mes = DateTime.Now.Month;
						deuda.anio = DateTime.Now.Year;
						deuda.concepto1 = Convert.ToString(fACTURA.cliente);
						deuda.concepto2 = nomcliente;
						deuda.concepto3 = "";
						deuda.deuda1 = deu;

						db.DEUDAs.Add(deuda);
						db.SaveChanges();
					}

					if (fACTURA.efectivo != 0 || fACTURA.efectivo != null)
					{
						BALANCE balfac = new BALANCE();

						balfac.dia = DateTime.Now.Day;
						balfac.mes = DateTime.Now.Month;
						balfac.anio = DateTime.Now.Year;
						balfac.concepto1 = "FACTURA#" + Convert.ToString(fACTURA.consecutivo_fact);
						balfac.concepto2 = Convert.ToString(fACTURA.cliente) + " " + nomcliente;
						balfac.concepto3 = "VENTA";
						balfac.ingreso = Convert.ToDecimal(fACTURA.total_fac);
						balfac.gasto = Convert.ToDecimal(0);
						balfac.m_pago = "EFECTIVO";
						db.BALANCES.Add(balfac);
						db.SaveChanges();
					}
					if (fACTURA.transaccion != 0 || fACTURA.transaccion != null)
					{
						BALANCE balfac = new BALANCE();

						balfac.dia = DateTime.Now.Day;
						balfac.mes = DateTime.Now.Month;
						balfac.anio = DateTime.Now.Year;
						balfac.concepto1 = "FACTURA#" + Convert.ToString(fACTURA.consecutivo_fact);
						balfac.concepto2 = Convert.ToString(fACTURA.cliente) + " " + nomcliente;
						balfac.concepto3 = "VENTA";
						balfac.ingreso = Convert.ToDecimal(fACTURA.transaccion);
						balfac.gasto = Convert.ToDecimal(0);
						balfac.m_pago = "TRANSACCIÓN";
						db.BALANCES.Add(balfac);
						db.SaveChanges();
					}
					if (fACTURA.tarjeta != 0 || fACTURA.tarjeta != null)
					{
						BALANCE balfac = new BALANCE();

						balfac.dia = DateTime.Now.Day;
						balfac.mes = DateTime.Now.Month;
						balfac.anio = DateTime.Now.Year;
						balfac.concepto1 = "FACTURA#" + Convert.ToString(fACTURA.id);
						balfac.concepto2 = Convert.ToString(fACTURA.cliente) + " " + nomcliente;
						balfac.concepto3 = "VENTA";
						balfac.ingreso = Convert.ToDecimal(fACTURA.tarjeta);
						balfac.gasto = Convert.ToDecimal(0);
						balfac.m_pago = "TARJETA";
						db.BALANCES.Add(balfac);
						db.SaveChanges();
					}

					foreach (var Op in model.items)
					{
						ITEMVENTA it = new ITEMVENTA();

						it.idventa = fACTURA.id;
						it.cantidad = Op.cantidad;
						it.producto = Op.producto;
						it.opciones = Op.opciones;
						it.precio_uni = Op.precio_uni;
						it.precio_ext = Op.precio_ext;
						it.mixt = Op.mixt;
						it.precio_tot = (it.precio_uni + it.precio_ext) * it.cantidad;
						db.ITEMVENTAs.Add(it);
						db.SaveChanges();

						//(((((((((((((((((( update diarioventas))))))))))))))))))))))9

						var venta_diaria = (from a in db.DIARIOVENTAS
											where a.anio == DateTime.Now.Year && a.mes == DateTime.Now.Month && a.dia == DateTime.Now.Day && a.codigo == it.producto
											select a).FirstOrDefault();

						if (venta_diaria != null)
						{
							venta_diaria.cantidad = venta_diaria.cantidad + Op.cantidad;
							db.SaveChanges();
							venta_diaria.pret_tot = venta_diaria.pret_tot + it.precio_tot;
							db.SaveChanges();
						}
						else
						{
							DIARIOVENTA dve = new DIARIOVENTA();

							dve.anio = fACTURA.anio;
							dve.mes = fACTURA.mes;
							dve.dia = fACTURA.dia;
							dve.codigo = it.producto;
							dve.cantidad = it.cantidad;
							dve.pre_uni = it.precio_uni;
							dve.pret_tot = it.precio_tot;
							db.DIARIOVENTAS.Add(dve);
							db.SaveChanges();

						}


						//------update inventario-----------

						PRODUCTO producto = db.PRODUCTO.Find(it.producto);
						if (producto != null)
						{
							var tipo1 = producto.tipo1; decimal? cant1 = producto.cantidad1;
							var tipo2 = producto.tipo2; decimal? cant2 = producto.cantidad2;
							var tipo3 = producto.tipo3; decimal? cant3 = producto.cantidad3;
							var tipo4 = producto.tipo4; decimal? cant4 = producto.cantidad4;
							var tipo5 = producto.tipo5; decimal? cant5 = producto.cantidad5;
							var tipo6 = producto.tipo6; decimal? cant6 = producto.cantidad6;
							var tipo7 = producto.tipo7; decimal? cant7 = producto.cantidad7;

							if (Convert.ToInt16(it.mixt) == 1)
							{


								if (tipo1 != null)
								{
									var inventario = (from a in db.INVENTARIO
													  where a.id == tipo1
													  select a).FirstOrDefault();

									inventario.existencias = inventario.existencias - (cant1 / 2);
									db.SaveChanges();
								}
								if (tipo2 != null)
								{
									var inventario = (from a in db.INVENTARIO
													  where a.id == tipo2
													  select a).FirstOrDefault();

									inventario.existencias = inventario.existencias - (cant2 / 2);
									db.SaveChanges();
								}
								if (tipo3 != null)
								{
									var inventario = (from a in db.INVENTARIO
													  where a.id == tipo3
													  select a).FirstOrDefault();

									inventario.existencias = inventario.existencias - (cant3 / 2);
									db.SaveChanges();
								}
								if (tipo4 != null)
								{
									var inventario = (from a in db.INVENTARIO
													  where a.id == tipo4
													  select a).FirstOrDefault();

									inventario.existencias = inventario.existencias - (cant4 / 2);
									db.SaveChanges();
								}
								if (tipo5 != null)
								{
									var inventario = (from a in db.INVENTARIO
													  where a.id == tipo5
													  select a).FirstOrDefault();

									inventario.existencias = inventario.existencias - (cant5 / 2);
									db.SaveChanges();
								}
								if (tipo6 != null)
								{
									var inventario = (from a in db.INVENTARIO
													  where a.id == tipo6
													  select a).FirstOrDefault();

									inventario.existencias = inventario.existencias - (cant6 / 2);
									db.SaveChanges();
								}
								if (tipo7 != null)
								{
									var inventario = (from a in db.INVENTARIO
													  where a.id == tipo7
													  select a).FirstOrDefault();

									inventario.existencias = inventario.existencias - (cant7 / 2);
									db.SaveChanges();
								}
							}
							else
							{


								if (tipo1 != null)
								{
									var inventario = (from a in db.INVENTARIO
													  where a.id == tipo1
													  select a).FirstOrDefault();

									inventario.existencias = inventario.existencias - cant1;
									db.SaveChanges();
								}
								if (tipo2 != null)
								{
									var inventario = (from a in db.INVENTARIO
													  where a.id == tipo2
													  select a).FirstOrDefault();

									inventario.existencias = inventario.existencias - cant2;
									db.SaveChanges();
								}
								if (tipo3 != null)
								{
									var inventario = (from a in db.INVENTARIO
													  where a.id == tipo3
													  select a).FirstOrDefault();

									inventario.existencias = inventario.existencias - cant3;
									db.SaveChanges();
								}
								if (tipo4 != null)
								{
									var inventario = (from a in db.INVENTARIO
													  where a.id == tipo4
													  select a).FirstOrDefault();

									inventario.existencias = inventario.existencias - cant4;
									db.SaveChanges();
								}
								if (tipo5 != null)
								{
									var inventario = (from a in db.INVENTARIO
													  where a.id == tipo5
													  select a).FirstOrDefault();

									inventario.existencias = inventario.existencias - cant5;
									db.SaveChanges();
								}
								if (tipo6 != null)
								{
									var inventario = (from a in db.INVENTARIO
													  where a.id == tipo6
													  select a).FirstOrDefault();

									inventario.existencias = inventario.existencias - cant6;
									db.SaveChanges();
								}
								if (tipo7 != null)
								{
									var inventario = (from a in db.INVENTARIO
													  where a.id == tipo7
													  select a).FirstOrDefault();

									inventario.existencias = inventario.existencias - cant7;
									db.SaveChanges();
								}
							}
						}


						OPCIONE opcioncan = db.OPCIONES.Find(it.opciones);

						if (opcioncan != null)
						{

							PRODUCTO productoopc = db.PRODUCTO.Find(opcioncan.insumoOp);
							if (producto != null)
							{
								var tipo1 = productoopc.tipo1; decimal? cant1 = productoopc.cantidad1;
								var tipo2 = productoopc.tipo2; decimal? cant2 = productoopc.cantidad2;
								var tipo3 = productoopc.tipo3; decimal? cant3 = productoopc.cantidad3;
								var tipo4 = productoopc.tipo4; decimal? cant4 = productoopc.cantidad4;
								var tipo5 = productoopc.tipo5; decimal? cant5 = productoopc.cantidad5;
								var tipo6 = productoopc.tipo6; decimal? cant6 = productoopc.cantidad6;
								var tipo7 = productoopc.tipo7; decimal? cant7 = productoopc.cantidad7;

								if (Convert.ToInt16(it.mixt) == 1)
								{


									if (tipo1 != null)
									{
										var inventario = (from a in db.INVENTARIO
														  where a.id == tipo1
														  select a).FirstOrDefault();

										inventario.existencias = (inventario.existencias - ((cant1 / 2) * opcioncan.cantidadOp));
										db.SaveChanges();
									}
									if (tipo2 != null)
									{
										var inventario = (from a in db.INVENTARIO
														  where a.id == tipo2
														  select a).FirstOrDefault();

										inventario.existencias = (inventario.existencias - ((cant2 / 2) * opcioncan.cantidadOp));
										db.SaveChanges();
									}
									if (tipo3 != null)
									{
										var inventario = (from a in db.INVENTARIO
														  where a.id == tipo3
														  select a).FirstOrDefault();

										inventario.existencias = (inventario.existencias - ((cant3 / 2) * opcioncan.cantidadOp));
										db.SaveChanges();
									}
									if (tipo4 != null)
									{
										var inventario = (from a in db.INVENTARIO
														  where a.id == tipo4
														  select a).FirstOrDefault();

										inventario.existencias = (inventario.existencias - ((cant4 / 2) * opcioncan.cantidadOp));
										db.SaveChanges();
									}
									if (tipo5 != null)
									{
										var inventario = (from a in db.INVENTARIO
														  where a.id == tipo5
														  select a).FirstOrDefault();

										inventario.existencias = (inventario.existencias - ((cant5 / 2) * opcioncan.cantidadOp));
										db.SaveChanges();
									}
									if (tipo6 != null)
									{
										var inventario = (from a in db.INVENTARIO
														  where a.id == tipo6
														  select a).FirstOrDefault();

										inventario.existencias = (inventario.existencias - ((cant6 / 2) * opcioncan.cantidadOp));
										db.SaveChanges();
									}
									if (tipo7 != null)
									{
										var inventario = (from a in db.INVENTARIO
														  where a.id == tipo7
														  select a).FirstOrDefault();

										inventario.existencias = (inventario.existencias - ((cant7 / 2) * opcioncan.cantidadOp));
										db.SaveChanges();
									}
								}
								else
								{


									if (tipo1 != null)
									{
										var inventario = (from a in db.INVENTARIO
														  where a.id == tipo1
														  select a).FirstOrDefault();

										inventario.existencias = (inventario.existencias - (cant1 * opcioncan.cantidadOp));
										db.SaveChanges();
									}
									if (tipo2 != null)
									{
										var inventario = (from a in db.INVENTARIO
														  where a.id == tipo2
														  select a).FirstOrDefault();

										inventario.existencias = (inventario.existencias - (cant2 * opcioncan.cantidadOp));
										db.SaveChanges();
									}
									if (tipo3 != null)
									{
										var inventario = (from a in db.INVENTARIO
														  where a.id == tipo3
														  select a).FirstOrDefault();

										inventario.existencias = (inventario.existencias - (cant3 * opcioncan.cantidadOp));
										db.SaveChanges();
									}
									if (tipo4 != null)
									{
										var inventario = (from a in db.INVENTARIO
														  where a.id == tipo4
														  select a).FirstOrDefault();

										inventario.existencias = (inventario.existencias - (cant4 * opcioncan.cantidadOp));
										db.SaveChanges();
									}
									if (tipo5 != null)
									{
										var inventario = (from a in db.INVENTARIO
														  where a.id == tipo5
														  select a).FirstOrDefault();

										inventario.existencias = (inventario.existencias - (cant5 * opcioncan.cantidadOp));
										db.SaveChanges();
									}
									if (tipo6 != null)
									{
										var inventario = (from a in db.INVENTARIO
														  where a.id == tipo6
														  select a).FirstOrDefault();

										inventario.existencias = (inventario.existencias - (cant6 * opcioncan.cantidadOp));
										db.SaveChanges();
									}
									if (tipo7 != null)
									{
										var inventario = (from a in db.INVENTARIO
														  where a.id == tipo7
														  select a).FirstOrDefault();

										inventario.existencias = (inventario.existencias - (cant7 * opcioncan.cantidadOp));
										db.SaveChanges();
									}
								}
							}

						}

					}



					//	return View("Details",fACTURA.id);
					return RedirectToAction("Details", new { id = fACTURA.id });
					//	/FACTURAs/Details%3fid%3d2

				}

			}
			catch (Exception ex)
			{
				ViewBag.producto = new SelectList(db.PRODUCTO, "cod_producto", "descripcion");
				ViewBag.codigo = new SelectList(db.PRODUCTO, "cod_producto", "cod_producto");
				ViewBag.cliente = new SelectList(db.CLIENTEs, "documento", "documento");
				ViewBag.empleado = new SelectList(db.EMPLEADOes, "documento", "nombre");
				ViewBag.opcion = new SelectList(string.Empty, "Value", "Text");
				return View();
			}




		}

		// GET: FACTURAs/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			FACTURA fACTURA = db.FACTURAs.Find(id);
			if (fACTURA == null)
			{
				return HttpNotFound();
			}
			return View(fACTURA);
		}

		// POST: FACTURAs/Edit/5
		// Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
		// más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "id,dia,mes,anio,cliente,total_fac,cartera")] FACTURA fACTURA)
		{
			if (ModelState.IsValid)
			{
				db.Entry(fACTURA).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(fACTURA);
		}

		// GET: FACTURAs/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			FACTURA fACTURA = db.FACTURAs.Find(id);
			if (fACTURA == null)
			{
				return HttpNotFound();
			}
			return View(fACTURA);
		}

		// POST: FACTURAs/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			FACTURA fACTURA = db.FACTURAs.Find(id);
			db.FACTURAs.Remove(fACTURA);
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