using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TuVanKhamBenh.Models;

namespace TuVanKhamBenh.Controllers
{

  public class HomeController : Controller
  {
    HYKDataContext context = new HYKDataContext();
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult About()
    {
      return View();
    }
    public ActionResult TuVan()
    {
      return View();
    }

    public ActionResult Contact()
    {
      return View();
    }
    public ActionResult Dangky()
    {
      return View();
    }
    
    [HttpPost]
    public ActionResult DangkyTK(UserAccount acc)
    {
      if (ModelState.IsValid)
      {
        var check = context.UserAccounts.FirstOrDefault(x => x.Username == acc.Username);
        if (check == null)
        {
          if (Request.Form.Count > 0)
          {
            acc.Admin = false;
            context.UserAccounts.InsertOnSubmit(acc);
            context.SubmitChanges();
            return RedirectToAction("DangNhap", "Taikhoan");
          }
        }
        else
        {
          ModelState.AddModelError("", "Tài khoản đã tồn tại");
        }
      }
      return View("Dangky");
    }



    public ActionResult ThongTinDD()
    {
      return View();
    }

  }
}