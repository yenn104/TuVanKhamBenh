using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TuVanKhamBenh.Models;
using WebMatrix.WebData;
using System.Security.Cryptography;
using System.Text;
using TuVanKhamBenh.common;

namespace TuVanKhamBenh.Controllers
{

  public class TaikhoanController : Controller
  {
    private HYKDataContext context = new HYKDataContext();

    // GET: Taikhoan
    [Authorize(Users = "yen,huyen,kiet")]
    public ActionResult Index(string searchString)
    {
      var links = from b in context.UserAccounts
                  select b;

      if (!String.IsNullOrEmpty(searchString))
      {
        links = links.Where(s => s.Username.Contains(searchString) || s.Ten.Contains(searchString) || s.Gmail.Contains(searchString));
      }

      return View(links);
    }


    public ActionResult Refesh(string searchString)
    {
      HYKDataContext context = new HYKDataContext();
      var links = from b in context.UserAccounts
                  select b;

      return View(links);
    }

    public ActionResult DangNhap()
    {
      return View();
    }


    [HttpPost]
    public ActionResult Login(UserAccount account, String returnUrl)
    {
        if (ModelState.IsValid)
          {      
          string user = account.Username;
          string pass = account.Password;
          var acc = context.UserAccounts.FirstOrDefault(s => s.Username.Equals(user) && s.Password.Equals(pass));

          if (acc != null)
          {
            if (acc.Admin == true)
            {
              FormsAuthentication.SetAuthCookie(acc.Username, false);
              if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                  && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
              {
                return Redirect(returnUrl);
              }
              else
              {
                FormsAuthentication.SetAuthCookie(account.Username, true);
                Session["TaiKhoan"] = acc;
                return RedirectToAction("TrangchuAdmin", "Admin");
              }
            }
            else
            {
              if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                  && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
              {
                return Redirect(returnUrl);
              }
              else
              {
                FormsAuthentication.SetAuthCookie(account.Username, true);
                Session["TaiKhoan"] = acc;
                return RedirectToAction("Index", "Home");
              }
            }
          }
          else
          {
            ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không chính xác!");
          }
      }
      return View("DangNhap");
    }

    char ch;
    private string Random()
    {
      StringBuilder builder = new StringBuilder();
      Random random = new Random();
      
      for (int i = 0; i < 6; i++)
      {
        ch = Convert.ToChar(Convert.ToInt32(random.Next(65, 87)));
        builder.Append(ch);
      }
        return builder.ToString().ToLower();
    }


    public ActionResult ViewFogot()
    {
      return View();
    }


    [HttpPost]
    public ActionResult ForgotPassword()
    {
      if (ModelState.IsValid)
      {
        if (Request.Form.Count > 0)
        {
          string username = Request.Form["Username"];
          var acc = context.UserAccounts.FirstOrDefault(s => s.Username.Equals(username));
          if (acc != null)
          {
            acc.Password = Random();
            context.SubmitChanges();
            string content = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/MailTemp.html"));

            content = content.Replace("{{CustomerName}}", acc.Username);
            content = content.Replace("{{Ten}}", acc.Ten);
            content = content.Replace("{{Email}}", acc.Gmail);
            content = content.Replace("{{Password}}", acc.Password);

            var toGmail = acc.Gmail;
            new MailHelper().SendMail(toGmail, "Mật khẩu mới", content);

            return RedirectToAction("Index", "Home");
          }
          else
          {
            ModelState.AddModelError("", "Tài khoản không tồn tại!");
          }
        }

      }    
      return View("ViewFogot");
    }

    [Authorize]
    public ActionResult LogOut()
    {
      FormsAuthentication.SignOut();
      Session.Clear();
      return RedirectToAction("Index", "Home");
    }

    [Authorize(Users = "yen,huyen,kiet")]
    public ActionResult Details(string Username)
    {
      UserAccount acc = context.UserAccounts.FirstOrDefault(x => x.Username == Username);
      return View(acc);
    }

    //[HttpPost]

    public ActionResult spCreate(UserAccount acc)
    {
      if (ModelState.IsValid)
      {
        var check = context.UserAccounts.FirstOrDefault(x => x.Username == acc.Username);
        if (check == null)
        {
          if (Request.Form.Count > 0)
          {         
            context.UserAccounts.InsertOnSubmit(acc);
            context.SubmitChanges();
            return RedirectToAction("Index");
          }
        }
        else
        {
          ModelState.AddModelError("", "Tài khoản đã tồn tại");
        }
      }
      return View();
    }




    //public ActionResult spCreate()
    //{
    //  if (Request.Form.Count > 0)
    //  {
    //    string username = Request.Form["Username"];
    //    string password = Request.Form["Password"];
    //    string ten = Request.Form["Ten"];
    //    string gmail = Request.Form["gmail"];

    //    context.SP_AccountCreate(username, password, ten, gmail);
    //    context.SubmitChanges();
    //    return RedirectToAction("Index", "Home");
    //  }
    //  return View();
    //}

    [Authorize]
    public ActionResult Edit(string Username)
    {
      UserAccount acc = context.UserAccounts.FirstOrDefault(x => x.Username == Username);

      if (Request.Form.Count > 0)
      {
        string password = Request.Form["Password"];
        string ten = Request.Form["Ten"];
        string gmail = Request.Form["Gmail"];
        if(password != "" && password != null)
        {
            acc.Password = password;
        } 
        acc.Ten = ten;
        acc.Gmail = gmail;
        context.SubmitChanges();
        return RedirectToAction("Index");
      }
      return View(acc);
    }

    [Authorize]
    public ActionResult ChangePassword()
    {
      return View();
    }



    [Authorize]
    [HttpPost]
    public ActionResult ChangePass()
    {
      if (ModelState.IsValid)
      {
        if (Request.Form.Count > 0)
        {
          string username = Request.Form["Username"];
          string oldPass = Request.Form["OldPassword"];
          string newPass = Request.Form["NewPassword"];
          string confirm = Request.Form["ConfirmPassword"];

          UserAccount acc = context.UserAccounts.FirstOrDefault(x => x.Username == username);
          if (oldPass == acc.Password)
          {
            if (newPass == confirm)
            {
              acc.Password = newPass;
              context.SubmitChanges();
              return RedirectToAction("Index", "Home");
            }
            else
            {
              ModelState.AddModelError("", "New password and confirm password don't match!");
            }
          }
          else
          {
            ModelState.AddModelError("", "Old password don't correct!");
          }
        }
      }
      return View("ChangePassword");
    }



    [Authorize]
    public ActionResult Delete(string Username)
    {
      HYKDataContext context = new HYKDataContext();
      UserAccount acc = context.UserAccounts.FirstOrDefault(x => x.Username == Username);
      if (acc != null)
      {
        context.UserAccounts.DeleteOnSubmit(acc);
        context.SubmitChanges();
      }
      return RedirectToAction("Index");
    }

  }
}
