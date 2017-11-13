using Sample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    public interface IUserSvc
    {
        List<UserInfo> GetUsers();

        UserInfo GetUser(int userid);

        int AddUser(UserInfo user);

        int UpdateUser(UserInfo user);

        int deleteUser(int userid);
    }
}
