using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sample.Models;
using Sample.DataEntity;
using Base.Utility;

namespace Sample
{
    public class UserSvc : IUserSvc
    {

        DataContext db = new DataContext();

        public int AddUser(UserInfo user)
        {
            int result = 0;
            try
            {
                db.Users.Add(user);
                result = db.SaveChanges();

            }
            catch (Exception ex)
            {
                Base.Utility.Log.Error(ex.Message);
                result = -1;
            }
            return result;

        }

        public int UpdateUser(UserInfo user)
        {
            int result = 0;
            try
            {
                UserInfo obj = db.Users.Find(user.Id);
                obj.LoginName = user.LoginName;
                obj.NiceName = user.NiceName;
                obj.Password = user.Password;
                obj.Name = user.Name;
                obj.Gender = user.Gender;
                obj.Mobile = user.Mobile;
                obj.IdentityCard = user.IdentityCard;
                obj.Birthday = user.Birthday;
                obj.Address = user.Address;
                result = db.SaveChanges();

            }
            catch (Exception ex)
            {
                Base.Utility.Log.Error(ex.Message);
                result = -1;
            }
            return result;
        }

        public int deleteUser(int userID)
        {
            var us1 = db.Users.Find(userID);

            if (us1 != null)
            {
                db.Users.Remove(us1);
                return db.SaveChanges();
            }
            return 0;
        }

        public UserInfo GetUser(int userID)
        {

            var us1 = db.Users.Find(userID);

            return us1;
        }

        public List<UserInfo> GetUsers()
        {
            return db.Users.ToList();
        }

    }
}