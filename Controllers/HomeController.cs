using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using _01_THE_WALL.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace _01_THE_WALL.Controllers
{
  public class HomeController : Controller
  {
		private MyContext dbContext;
    public HomeController(MyContext context) => dbContext = context;
    private int? _uid 
    { 
      get{ return HttpContext.Session.GetInt32("uid"); }
      set{ HttpContext.Session.SetInt32("uid", (int)value); }
    }
		private string _tempMsg
    {
      get{ return HttpContext.Session.GetString("TempMsg"); }
      set{ HttpContext.Session.SetString("TempMsg", value); }
    }
		
    #region ***** HttpGets *****
    [HttpGet("")]
    public IActionResult Index()
    {
      if(_uid == null)
        ViewBag.LogoutBtn = "disable";
      ViewBag.TempMsg = _tempMsg;
      return View();
    }

    public IActionResult Privacy() => View();

    [HttpGet("messages")]
    public IActionResult Messages()
    {
      if(_uid == null)
      {
        _tempMsg = "Please login or register";
        return RedirectToAction("Index");
      }
      List<MessageModel> AllMessages = dbContext.Messages
        .Include(message => message.User)
        .Include(message => message.Comments)
        .ThenInclude(comment => comment.User)
        .ToList();
      foreach(var message in AllMessages)
      {
        if(message.UserId == _uid && message.CreatedAt > DateTime.Now.Add(new TimeSpan(0, -30, 0)))
        {
          message.isDeleteable = true;
        }
      }
      MsgMsgsModel MsgMsgs = new MsgMsgsModel();
      MsgMsgs.Msgs = AllMessages;
      return View(MsgMsgs);
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
      HttpContext.Session.Clear();
      _tempMsg = "Goodbye!";
      return RedirectToAction("Index");
    }

    [HttpGet("message/delete/{id} ")]
    public IActionResult DeleteMessage(int id)
    {
      MessageModel MessageToDelete = dbContext.Messages
        .Include(m => m.Comments)
        .FirstOrDefault(m => m.MessageId == id);
      foreach(CommentModel Cmt in MessageToDelete.Comments)
      {
        dbContext.Comments.Remove(Cmt);
      }
      dbContext.Messages.Remove(MessageToDelete);
      dbContext.SaveChanges();
      return RedirectToAction("Messages");
    }
    #endregion

    #region ***** HttpPosts *****
    [HttpPost("user/create")]
    public IActionResult CreateUser(LogRegModel newUser)
    {
      if (ModelState.IsValid)
      {
        UserModel emailCheck = dbContext.Users.FirstOrDefault(user => user.Email == newUser.User.Password);
        if (emailCheck == null)
        {
          PasswordHasher<UserModel> Hasher = new PasswordHasher<UserModel>();
          newUser.User.Password = Hasher.HashPassword(newUser.User, newUser.User.Password);
          var nu = dbContext.Users.Add(newUser.User);
          dbContext.SaveChanges();
          _uid = nu.Entity.UserId;
          HttpContext.Session.SetString("UserName", newUser.User.FirstName);
          return RedirectToAction("Messages");
        }
        ModelState.AddModelError("User.Email", "Email already exists");
        return View("Index");
      }
      else
      {
        return View("Index");
      }
    }
    
    [HttpPost("user/login")]
    public IActionResult LoginUser(LogRegModel loginUser)
    {
      if (ModelState.IsValid)
      {
        UserModel emailCheck = dbContext.Users.FirstOrDefault(user => user.Email == loginUser.Login.Email);
        if (emailCheck != null)
        {
          PasswordHasher<LoginModel> Hasher = new PasswordHasher<LoginModel>();
          var result = Hasher.VerifyHashedPassword(loginUser.Login, emailCheck.Password, loginUser.Login.Password);
          if (result != 0)
          {
            _uid = emailCheck.UserId;
            HttpContext.Session.SetString("UserName", emailCheck.FirstName);
            return RedirectToAction("Messages");
          }
          ModelState.AddModelError("Login.Password", "Incorrect password");
          return View("Index");
        }
        else
        {
          ModelState.AddModelError("Login.Email", "Email does not exist");
          return View("Index");
        }
      }
      return View("Index");
    }
		
    [HttpPost("message/post")]
    public IActionResult PostMessage(MsgMsgsModel newMessage)
    {
      if(ModelState.IsValid)
      {
        newMessage.Msg.UserId = (int)_uid;
        dbContext.Messages.Add(newMessage.Msg);
        dbContext.SaveChanges();
        return RedirectToAction("Messages");
      }
      return View("Messages");
    }

    [HttpPost("comment/post")]
    public IActionResult PostComment(CommentModel newComment)
    {
      if(ModelState.IsValid)
      {
        newComment.UserId = (int)_uid;
        dbContext.Comments.Add(newComment);
        dbContext.SaveChanges();
        return RedirectToAction("Messages");
      }
      return View("Messages");
    }
    #endregion

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
  }

  public static class SessionExtensions
  {
    public static void SetObjectAsJson(this ISession session, string key, object value)
    {
      session.SetString(key, JsonConvert.SerializeObject(value));
    }
    public static T GetObjectFromJson<T>(this ISession session, string key)
    {
      string value = session.GetString(key);
      return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
    }
  }
}
