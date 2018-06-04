using Base.DataAccess.Repository;
using Base.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Business
{
    public class QueryUser : IQueryUser
    {
        RepositoryFactory<T_User> db = new RepositoryFactory<T_User>();

        public T_User GetUserById(int userId)
        {
            return db.Repository().FindEntity(userId);
        }
        public T_User GetUserByLoginName(string loginName)
        {
            return db.Repository().FindEntity("loginName", loginName);
        } 
        public List<T_User> GetUsers()
        {
            return db.Repository().FindList();
        }

    }
}
