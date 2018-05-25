using Base.Entity;
using Base.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Base.Sample.Host
{
    [DataContract(Namespace = "http://tempuri.org/", Name = "Update")]
    //[DataContract]
    public class UserInterface
    {
        IMgmtUser userMgmt = CastleContainer.Instance.Resolve<IMgmtUser>();
        IQueryUser userQuery = CastleContainer.Instance.Resolve<IQueryUser>();


        [DataMember]
        public string str { get; set; }

        [System.ServiceModel.OperationContract]
        [System.ServiceModel.Web.WebInvoke(Method = "POST")]
        public T_User GetUserById(int userId)
        {
            return userQuery.GetUserById(userId);
        }
        [System.ServiceModel.OperationContract]
        [System.ServiceModel.Web.WebInvoke(Method = "POST")]
        public T_User GetUserByLoginName(string loginName)
        {
            return userQuery.GetUserByLoginName(loginName);
        }
        [System.ServiceModel.OperationContract]
        [System.ServiceModel.Web.WebInvoke(Method = "POST")]
        public int InsertUser(T_User user)
        {
            return userMgmt.InsertUser(user);
        }
        [System.ServiceModel.OperationContract]
        [System.ServiceModel.Web.WebInvoke(Method = "POST")]
        public int InsertUsers(List<T_User> users)
        {
            return userMgmt.InsertUsers(users);
        }
        [System.ServiceModel.OperationContract]
        [System.ServiceModel.Web.WebInvoke(Method = "POST")]
        public int UpdateUser(T_User user)
        {
            return userMgmt.UpdateUser(user);
        }
        [System.ServiceModel.OperationContract]
        [System.ServiceModel.Web.WebInvoke(Method = "POST")]
        public int DeleteUser(int userId)
        {
            return userMgmt.DeleteUser(userId);
        }
    }
}
