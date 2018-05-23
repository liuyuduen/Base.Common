using Base.Entity;
using Base.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Sample.Host
{
    public class UserInterface
    {
        IMgmtUser userMgmt = CastleContainer.Instance.Resolve<IMgmtUser>();
        IQueryUser userQuery = CastleContainer.Instance.Resolve<IQueryUser>();


        public T_User GetUserById(int userId)
        {
            return userQuery.GetUserById(userId);
        }

        public T_User GetUserByLoginName(string loginName)
        {
            return userQuery.GetUserByLoginName(loginName);
        }

        public int InsertUser(T_User user)
        {
            return userMgmt.InsertUser(user);
        }

        public int InsertUsers(List<T_User> users)
        {
            return userMgmt.InsertUsers(users);
        }

        public int UpdateUser(T_User user)
        {
            return userMgmt.UpdateUser(user);
        }
        public int DeleteUser(int userId)
        {
            return userMgmt.DeleteUser(userId);
        }
    }
}
