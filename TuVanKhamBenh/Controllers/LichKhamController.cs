using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using TuVanKhamBenh.Models;


namespace TuVanKhamBenh.Controllers
{
  [Authorize(Users = "yen,huyen,kiet")]
  public class LichKhamController : Controller
  {
    // GET: LichKham
    public ActionResult Index(string searchString)
    {
      HYKDataContext context = new HYKDataContext();
      var links = from b in context.LichKhams
                  select b;
      if (!String.IsNullOrEmpty(searchString))
      {
        links = links.Where(s => s.Ten.Contains(searchString) || s.Username.Contains(searchString));
      }

      return View(links);
    }


    public ActionResult Refesh(string searchString)
    {
      HYKDataContext context = new HYKDataContext();
      var links = from b in context.LichKhams
                  select b;

      return View(links);
    }



    [AllowAnonymous]
    public ActionResult Create()
    {
      HYKDataContext context = new HYKDataContext();
      List<ThongTin> list1 = context.ThongTins.GroupBy(tt => tt.Tuoi).Select(group => group.First()).ToList();
      List<ThongTin> list2 = context.ThongTins.GroupBy(tt => tt.CanNang).Select(group => group.First()).ToList();
      List<ThongTin> list3 = context.ThongTins.GroupBy(tt => tt.Giong).Select(group => group.First()).ToList();
      ViewBag.tuoi = list1;
      ViewBag.cannang = list2;
      ViewBag.giong = list3;

      if (Request.Form.Count > 0)
      {
        int tuoi = int.Parse(Request.Form["Tuoi"]);       
        string cannang = Request.Form["CanNang"];
        string giong = Request.Form["Giong"];
        
        string username = Request.Form["Username"];
        string ten = Request.Form["Ten"];

        SP_XSINFOResult info = context.SP_XSINFO(tuoi, cannang, giong).FirstOrDefault();
        DateTime ngayhen = DateTime.ParseExact(Request.Form["NgayHen"], "dd/MM/yyyy", null);

        LichKham lk = new LichKham();
        lk.Info = info.ID;
        lk.Ten = ten;
        lk.NgayDat = DateTime.Now;
        lk.NgayHen = ngayhen;

        if (username != null && username != "")
        {
           lk.Username = username;       
        }
        else
        {
          //lk.Username = "";         
        }
        
        context.LichKhams.InsertOnSubmit(lk);      
        context.SubmitChanges();
        List<LichKham>  abc = context.LichKhams.ToList();
        int a = context.LichKhams.Count();
        LichKham dd = abc[a-1];
        //string actionname = "Details/" + dd.ID;
        return RedirectToAction("Details", "LichKham", new { id = dd.ID});
      }
      return View();
    }

    [AllowAnonymous]
    public ActionResult Details(int id)
    {
      HYKDataContext context = new HYKDataContext();
      ////////////////////////////////////HIỆN CỤM THÔNG TIN MÈO THAY VÌ HIỆN ID THÔNG TIN MÈO///////////////////////// SỬA THÊM TRONG VIEW
      LichKham lk = context.LichKhams.FirstOrDefault(x => x.ID == id);
      return View(lk);
    }


    public ActionResult Edit(int id)
    {
      HYKDataContext context = new HYKDataContext();
      LichKham lk = context.LichKhams.FirstOrDefault(x => x.ID == id);
      int idTT = (int)lk.Info;
      ThongTin ttt = context.ThongTins.FirstOrDefault(x => x.ID == idTT);
      ViewBag.t = ttt.Tuoi;
      ViewBag.c = ttt.CanNang;
      ViewBag.g = ttt.Giong;

      List<ThongTin> list1 = context.ThongTins.GroupBy(tt => tt.Tuoi).Select(group => group.First()).ToList();
      List<ThongTin> list2 = context.ThongTins.GroupBy(tt => tt.CanNang).Select(group => group.First()).ToList();
      List<ThongTin> list3 = context.ThongTins.GroupBy(tt => tt.Giong).Select(group => group.First()).ToList();
      ViewBag.tuoi = list1;
      ViewBag.cannang = list2;
      ViewBag.giong = list3;
      ViewBag.NgayHen = lk.NgayHen;
      
      if (Request.Form.Count > 0)
      {
        int tuoi = int.Parse(Request.Form["Tuoi"]);
        string cannang = Request.Form["CanNang"];
        string giong = Request.Form["Giong"];
        SP_XSINFOResult info = context.SP_XSINFO(tuoi, cannang, giong).FirstOrDefault();

        //string username = Request.Form["Username"];
        string ten = Request.Form["Ten"];
        //DateTime ngaydat = DateTime.ParseExact(Request.Form["NgayDat"], "dd/MM/yyyy", null);
        DateTime ngayhen = DateTime.ParseExact(Request.Form["NgayHen"], "dd/MM/yyyy", null);
        lk.Info = info.ID;
        lk.Username = lk.Username;
        lk.Ten = ten;
        //lk.NgayDat = ngaydat;
        lk.NgayDat = lk.NgayDat;
        lk.NgayHen = ngayhen;
        context.SubmitChanges();
        return RedirectToAction("Index");
      }
      return View(lk);
    }

    public ActionResult Delete(int id)
    {
      HYKDataContext context = new HYKDataContext();
      LichKham lk = context.LichKhams.FirstOrDefault(x => x.ID == id);
      if (lk != null)
      {
        context.LichKhams.DeleteOnSubmit(lk);
        context.SubmitChanges();
      }
      return RedirectToAction("Index");
    }

  }

}