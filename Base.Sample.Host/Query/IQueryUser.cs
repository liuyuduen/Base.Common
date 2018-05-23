using Base.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Sample.Host
{
    public interface IQueryUser
    {
        T_User GetUserById(int userId);
        T_User GetUserByLoginName(string loginName);
        List<T_User> GetUsers(); 
    }
}
