using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using LinqToExcel;
using System.Data.SqlClient;
using INVYBAL.Models;

namespace INVYBAL.Controllers
{
    public class UsersController : Controller
    {
        private INVYBALEntities db = new INVYBALEntities();

        // GET: Users
        public ActionResult Index()
        {
            return View();
        }

		public FileResult DownloadExcel()
		{
			string path = "/Doc/Users.xls";
			return File(path, "application/vnd.ms-excel", "Users.xls");
		}


		[HttpPost]
		public JsonResult UploadExcel(User users, HttpPostedFileBase FileUpload)
		{

			List<string> data = new List<string>();
			if (FileUpload != null)
			{
				// tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
				if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
				{


					string filename = FileUpload.FileName;
					string targetpath = Server.MapPath("~/Doc/");
					FileUpload.SaveAs(targetpath + filename);
					string pathToExcelFile = targetpath + filename;
					var connectionString = "";
					if (filename.EndsWith(".xls"))
					{
						connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
					}
					else if (filename.EndsWith(".xls"))
					{
						connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
					}

					var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
					var ds = new DataSet();

					adapter.Fill(ds, "ExcelTable");

					DataTable dtable = ds.Tables["ExcelTable"];

					string sheetName = "Sheet1";

					var excelFile = new ExcelQueryFactory(pathToExcelFile);
					var artistAlbums = from a in excelFile.Worksheet<User>(sheetName) select a;

					foreach (var a in artistAlbums)
					{
						try
						{
							if (a.Name != "" && a.Adress != "" && a.ContactNo != "")
							{
								User TU = new User();
								TU.Name = a.Name;
								TU.Adress = a.Adress;
								TU.ContactNo = a.ContactNo;
								db.Users.Add(TU);

								db.SaveChanges();



							}
							else
							{
								data.Add("<ul>");
								if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
								if (a.Adress == "" || a.Adress == null) data.Add("<li> Address is required</li>");
								if (a.ContactNo == "" || a.ContactNo == null) data.Add("<li>ContactNo is required</li>");

								data.Add("</ul>");
								data.ToArray();
								return Json(data, JsonRequestBehavior.AllowGet);
							}
						}

						catch (DbEntityValidationException ex)
						{
							foreach (var entityValidationErrors in ex.EntityValidationErrors)
							{

								foreach (var validationError in entityValidationErrors.ValidationErrors)
								{

									Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

								}

							}
						}
					}
					//deleting excel file from folder  
					if ((System.IO.File.Exists(pathToExcelFile)))
					{
						System.IO.File.Delete(pathToExcelFile);
					}
					return Json("success", JsonRequestBehavior.AllowGet);
				}
				else
				{
					//alert message for invalid file format  
					data.Add("<ul>");
					data.Add("<li>Only Excel file format is allowed</li>");
					data.Add("</ul>");
					data.ToArray();
					return Json(data, JsonRequestBehavior.AllowGet);
				}
			}
			else
			{
				data.Add("<ul>");
				if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
				data.Add("</ul>");
				data.ToArray();
				return Json(data, JsonRequestBehavior.AllowGet);
			}
		}

		// GET: Users/Details/5
		public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,Name,Adress,ContactNo")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,Name,Adress,ContactNo")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
