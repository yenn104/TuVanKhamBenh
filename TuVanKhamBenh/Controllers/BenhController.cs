using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TuVanKhamBenh.Models;

namespace TuVanKhamBenh.Controllers
{
  [Authorize(Users = "yen,huyen,kiet")]
  public class BenhController : Controller
  {
    private HYKDataContext context = new HYKDataContext();
    // GET: Benh

    [Authorize]
    public ActionResult Index(string searchString)
    {
      var links = from b in context.Benhs
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
      var links = from b in context.Benhs
                  select b;

      return View(links);
    }

    public ActionResult Details(int id)
    {
      Benh b = context.Benhs.FirstOrDefault(x => x.ID == id);
      List<SP_TCBenhResult> list1 = context.SP_TCBenh(id).ToList();
      ViewBag.ListTCB = list1;
      return View(b);
    }

    public ActionResult spCreate()
    {
      if (Request.Form.Count > 0)
      {
        string ten = Request.Form["Ten"];
        context.SP_BenhCreate(ten);
        context.SubmitChanges();
        return RedirectToAction("Index");
      }
      return View();
    }

    public ActionResult Edit(int id)
    {
      Benh b = context.Benhs.FirstOrDefault(x => x.ID == id);
      List<SP_TCBenhResult> list1 = context.SP_TCBenh(id).ToList();
      List<TrieuChung> list2 = context.TrieuChungs.ToList();
      ViewBag.ListTCB = list1;
      ViewBag.ListTC = list2;

      if (Request.Form.Count > 0)
      {
        string ten = Request.Form["Ten"];
        b.Ten = ten;
        
        List<TrieuChungBenh> list3 = context.TrieuChungBenhs.Where(x => x.IDBenh == id).ToList();
        string[] list = Request.Form.GetValues("TC");
        b.Slg = list.Count();

        context.TrieuChungBenhs.DeleteAllOnSubmit(list3);

        for (int i = 0; i < list.Count(); i++)
        {
          TrieuChungBenh tcb = new TrieuChungBenh();
          int y = int.Parse(list[i]);        
          tcb.IDBenh = id;
          tcb.IDTrieuChung = y;
          context.TrieuChungBenhs.InsertOnSubmit(tcb);
          context.SubmitChanges();
        }

        return RedirectToAction("Index");
      }
      return View(b);
    }

    public ActionResult Delete(int id)
    {
      Benh b = context.Benhs.FirstOrDefault(x => x.ID == id);
      if (b != null)
      {
        context.Benhs.DeleteOnSubmit(b);
        context.SubmitChanges();
      }
      return RedirectToAction("Index");
    }

    [Authorize]
    public ActionResult LogOut()
    {
        return View();      
    }

    }
}
