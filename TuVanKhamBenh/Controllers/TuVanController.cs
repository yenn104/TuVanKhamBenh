using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TuVanKhamBenh.Models;

namespace TuVanKhamBenh.Controllers
{
  
  public class TuVanController : Controller
  {

    [Authorize(Users = "yen,huyen,kiet")]
    public ActionResult Index(string searchString)
    {
      HYKDataContext context = new HYKDataContext();
      var links = from b in context.SP_DSChuanDoan()
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


    public ActionResult TuVan()
    {
      HYKDataContext context = new HYKDataContext();
      List<TrieuChung> list = context.TrieuChungs.ToList();
      List<ThongTin> list1 = context.ThongTins.GroupBy(tt => tt.Tuoi).Select(group => group.First()).ToList();
      List<ThongTin> list2 = context.ThongTins.GroupBy(tt => tt.CanNang).Select(group => group.First()).ToList();
      List<ThongTin> list3 = context.ThongTins.GroupBy(tt => tt.Giong).Select(group => group.First()).ToList();
      ViewBag.tuoi = list1;
      ViewBag.cannang = list2;
      ViewBag.giong = list3;
      ViewBag.ListTC = list;
      return View();
    }

    public ActionResult ChuanDoan()
    {
      if (Request.Form.Count > 0)
      {
        HYKDataContext context = new HYKDataContext();
        int tuoi = int.Parse(Request.Form["Tuoi"]);
        string cannang = Request.Form["CanNang"];
        string giong = Request.Form["Giong"];
        string username ="";
        UserAccount acc = (UserAccount)Session["TaiKhoan"];
        if (acc != null)
        {
          username = acc.Username;
        }
        
        string t_key = "";
        string[] list = Request.Form.GetValues("TC");

        for (int i = 0; i < list.Count(); i++)
        {
          string y = list[i];
          if (t_key == "")
          {
            t_key = "'," + y;
          }
          else
          {
            t_key = t_key + "," + y;
          }
        }
        string key = t_key + ",'";

        SP_XSINFOResult info = context.SP_XSINFO(tuoi, cannang, giong).FirstOrDefault();
        SP_ChuanDoanResult chuandoan = context.SP_ChuanDoan(key, info.ID).FirstOrDefault();

        if (username != null && username != "")
        {
          context.SP_ThemCD(chuandoan.ID, chuandoan.ChiPhi, username);
        }
        else
        {
          context.SP_ThemCD1(chuandoan.ID, chuandoan.ChiPhi);
        }
        context.SubmitChanges();
        return View(chuandoan);
      }
      return RedirectToAction("Index");
    }

    [Authorize]
    public ActionResult Delete(int id)
    {
      HYKDataContext context = new HYKDataContext();
      ChuanDoan lk = context.ChuanDoans.FirstOrDefault(x => x.ID == id);
      if (lk != null)
      {
        context.ChuanDoans.DeleteOnSubmit(lk);
        context.SubmitChanges();
      }
      return RedirectToAction("Index");
    }

    [Authorize]
    public ActionResult LSCD()
    {
      HYKDataContext context = new HYKDataContext();
      UserAccount acc = (UserAccount)Session["TaiKhoan"];
      List<ChuanDoan> lscd = context.ChuanDoans.Where(x => x.UserName == acc.Username).ToList();
      return View(lscd);
    }

    [Authorize]
    public ActionResult LSDL()
    {
      HYKDataContext context = new HYKDataContext();
      UserAccount acc = (UserAccount)Session["TaiKhoan"];
      List<LichKham> lscd = context.LichKhams.Where(x => x.Username == acc.Username).ToList();
      return View(lscd);
    }


  }
}