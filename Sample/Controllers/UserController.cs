using Base.Utility;
using Sample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sample.Controllers
{
    /// <summary>
    /// EF code first
    /// Ioc
    /// log4net
    /// </summary>
    public class UserController : Controller
    {
        IUserSvc dal = CastleContainer.Instance.Resolve<IUserSvc>();

        // GET: EFDemo/User
        public ActionResult Index()
        {
            //Log.Info("执行User 打印日志");
            //Add();
            //Update();
            //GetUsers();
            //GetUser(); 
            //Delete();
            return View();
        }

        public void GetUsers()
        {
            var users = dal.GetUsers();
        }
        public void GetUser()
        {
            var users = dal.GetUser(1);
        }
        public void Add()
        {
            UserInfo user = new UserInfo();
            user.LoginName = "test_log";
            user.NiceName = "测试";
            user.Password = "aaaaaa";
            user.Name = "测试";
            user.Gender = 1;
            user.Mobile = "13900";
            user.IdentityCard = "43041098234";
            user.Birthday = "1989-10-20";
            user.Address = "南山新屋村";
            user.Status = 0;
            dal.AddUser(user);
        }

        public void Update()
        {
            UserInfo user = new UserInfo();
            user.Id = 2;
            user.LoginName = "test_log2";
            user.NiceName = "测试2";
            user.Password = "aaaaaa2";
            user.Name = "测试2";
            user.Gender = 1;
            user.Mobile = "139002";
            user.IdentityCard = "430410982342";
            user.Birthday = "1989-10-202";
            user.Address = "南山新屋村2";
            dal.UpdateUser(user);
        }

        public void Delete()
        {
            dal.deleteUser(2);
        }
    }
}