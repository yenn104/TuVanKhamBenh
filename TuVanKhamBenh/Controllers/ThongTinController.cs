using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TuVanKhamBenh.Models;

namespace TuVanKhamBenh.Controllers
{
  [Authorize]
  public class ThongTinController : Controller
  {
    // GET: ThongTin
    public ActionResult Index(string searchString)
    {
      HYKDataContext context = new HYKDataContext();
      var links = from b in context.ThongTins
                  select b;

      if (!String.IsNullOrEmpty(searchString))
      {
        links = links.Where(s => s.CanNang.Contains(searchString) || s.Giong.Contains(searchString));

      }

      return View(links);
    }


    public ActionResult Refesh(string searchString)
    {
      HYKDataContext context = new HYKDataContext();
      var links = from b in context.ThongTins
                  select b;

      return View(links);
    }


    public ActionResult Details(int id)
    {
      HYKDataContext context = new HYKDataContext();
      ThongTin tt = context.ThongTins.FirstOrDefault(x => x.ID == id);
      return View(tt);
    }

    public ActionResult Create()
    {
      if (Request.Form.Count > 0)
      {
        string giong = Request.Form["Giong"];
        string cannang = Request.Form["CanNang"];
        int tuoi = int.Parse(Request.Form["Tuoi"]);

        HYKDataContext context = new HYKDataContext();
        ThongTin tt = new ThongTin();
        tt.CanNang = cannang;
        tt.Tuoi = tuoi;
        tt.Giong = giong;
        context.ThongTins.InsertOnSubmit(tt);
        context.SubmitChanges();
        return RedirectToAction("Index");
      }
      return View();
    }

    public ActionResult Edit(int id)
    {
      HYKDataContext context = new HYKDataContext();
      ThongTin tt = context.ThongTins.FirstOrDefault(x => x.ID == id);

      if (Request.Form.Count > 0)
      {
        string giong = Request.Form["Giong"];
        string cannang = Request.Form["CanNang"];
        int tuoi = int.Parse(Request.Form["Tuoi"]);
        tt.CanNang = cannang;
        tt.Tuoi = tuoi;
        tt.Giong = giong;
        context.SubmitChanges();
        return RedirectToAction("Index");
      }
      return View(tt);
    }

    public ActionResult Delete(int id)
    {
      HYKDataContext context = new HYKDataContext();
      ThongTin tt = context.ThongTins.FirstOrDefault(x => x.ID == id);
      if (tt != null)
      {
        context.ThongTins.DeleteOnSubmit(tt);
        context.SubmitChanges();
      }
      return RedirectToAction("Index");
    }



  }
}