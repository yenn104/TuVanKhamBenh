using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TuVanKhamBenh.Models;

namespace TuVanKhamBenh.Controllers
{
  [Authorize]
  public class TrieuChungController : Controller
  {
    // GET: TrieuChung
    public ActionResult Index(string searchString)
    {
      HYKDataContext context = new HYKDataContext();
      var links = from b in context.TrieuChungs
                  select b;

      if (!String.IsNullOrEmpty(searchString))
      {
        links = links.Where(s => s.Ten.Contains(searchString));
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
      TrieuChung tc = context.TrieuChungs.FirstOrDefault(x => x.ID == id);
      return View(tc);
    }

    public ActionResult Create()
    {
      if (Request.Form.Count > 0)
      {
        string ten = Request.Form["Ten"];
        HYKDataContext context = new HYKDataContext();
        TrieuChung tc = new TrieuChung();
        tc.Ten = ten;
        context.TrieuChungs.InsertOnSubmit(tc);
        context.SubmitChanges();
        return RedirectToAction("Index");
      }
      return View();
    }

    public ActionResult Edit(int id)
    {
      HYKDataContext context = new HYKDataContext();
      TrieuChung tc = context.TrieuChungs.FirstOrDefault(x => x.ID == id);

      if (Request.Form.Count > 0)
      {
        string ten = Request.Form["Ten"];
        tc.Ten = ten;
        context.SubmitChanges();
        return RedirectToAction("Index");
      }
      return View(tc);
    }

    public ActionResult Delete(int id)
    {
      HYKDataContext context = new HYKDataContext();
      TrieuChung tc = context.TrieuChungs.FirstOrDefault(x => x.ID == id);
      if (tc != null)
      {
        context.TrieuChungs.DeleteOnSubmit(tc);
        context.SubmitChanges();
      }
      return RedirectToAction("Index");
    }

  }
}