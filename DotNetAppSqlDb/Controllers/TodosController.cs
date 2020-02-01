using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DotNetAppSqlDb.Models;using System.Diagnostics;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;

namespace DotNetAppSqlDb.Controllers
{
    public class TodosController : Controller
    {
        private MyDatabaseContext db = new MyDatabaseContext();

        // GET: Todos
        public ActionResult Index()
        {            
            Trace.WriteLine("GET /Todos/Index");
            return View(db.Todoes.ToList());
        }

        // GET: Todos/Details/5
        public ActionResult Details(int? id)
        {
            Trace.WriteLine("GET /Todos/Details/" + id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Todo todo = db.Todoes.Find(id);
            if (todo == null)
            {
                return HttpNotFound();
            }
            return View(todo);
        }

        // GET: Todos/Create
        public ActionResult Create()
        {
            Trace.WriteLine("GET /Todos/Create");
            return View(new Todo { CreatedDate = DateTime.Now });
        }

        // POST: Todos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Description,CreatedDate,Done")] Todo todo)
        {
            Trace.WriteLine("POST /Todos/Create");
            if (ModelState.IsValid)
            {
                db.Todoes.Add(todo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(todo);
        }

        // GET: Todos/Edit/5
        public ActionResult Edit(int? id)
        {
            Trace.WriteLine("GET /Todos/Edit/" + id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Todo todo = db.Todoes.Find(id);
            if (todo == null)
            {
                return HttpNotFound();
            }
            return View(todo);
        }

        // POST: Todos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Description,CreatedDate,Done")] Todo todo)
        {
            Trace.WriteLine("POST /Todos/Edit/" + todo.ID);
            if (ModelState.IsValid)
            {
                db.Entry(todo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(todo);
        }

        // GET: Todos/Delete/5
        public ActionResult Delete(int? id)
        {
            //Trace.WriteLine("GET /Todos/Delete/" + id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Todo todo = db.Todoes.Find(id);
            if (todo == null)
            {
                return HttpNotFound();
            }
            //return View(todo);

            db.Todoes.Remove(todo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Todos/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Trace.WriteLine("POST /Todos/Delete/" + id);
        //    Todo todo = db.Todoes.Find(id);
        //    db.Todoes.Remove(todo);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult ExportReport()
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "CrystalReport.rpt"));
            rd.SetDataSource(db.Todoes.ToList());
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "Todoes_list.pdf");
            }
            catch
            {
                throw;
            }
        }

        public ActionResult ExportSPReport()
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Report/CrystalReportSP.rpt")));
            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDbConnection"].ToString());
            SqlCommand cmd = new SqlCommand("SelectTodoesList", cn);
            DataTable dt = new DataTable();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            rd.SetDataSource(dt);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream str = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            str.Seek(0, SeekOrigin.Begin);
            return File(str, "application/pdf", "Todoes_list.pdf");
        }
    }
}
