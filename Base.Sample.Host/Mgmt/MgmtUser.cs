using Base.DataAccess.Repository;
using Base.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Sample.Host
{
    public class MgmtUser : IMgmtUser
    {
        RepositoryFactory<T_User> db = new RepositoryFactory<T_User>();

        public int InsertUser(T_User user)
        {
            return db.Repository().Insert(user);
        }
        public int InsertUsers(List<T_User> users)
        {
            return db.Repository().Insert(users);
        }
        public int UpdateUser(T_User user)
        {
            return db.Repository().Update(user);
        }

        public int DeleteUser(int userId)
        {
            return db.Repository().Delete(userId);
        }
    }
}
