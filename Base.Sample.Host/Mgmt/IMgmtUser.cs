using Base.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Sample.Host
{
    public interface IMgmtUser
    {
        int InsertUser(T_User user);

        int InsertUsers(List<T_User> users);

        int UpdateUser(T_User user); 

        int DeleteUser(int userId);

    }
}
