using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using System.Text.RegularExpressions;

namespace Base.Utility
{

    public class ADHelper
    {
        static string AdDefaultPath =ConfigHelper.AppSettings("AdPath").ToString();
        //static string AdPathForSupporter = TemplateConfig.getInstance().GetConfigByKey("ADCofig", "AdPathForSupport").ToString();
        static string AdPathForSupporter = ConfigHelper.AppSettings("AdPath").ToString();//备用路径
        static string AdUserName = ConfigHelper.AppSettings("ADUser").ToString();
        static string AdPassword = ConfigHelper.AppSettings("ADPassword").ToString();

        //1. Create a connection to Active Directory
        /// <summary>
        /// Method used to create an entry to the AD.
        /// Replace the path, username, and password.
        /// </summary>
        /// <returns>DirectoryEntry</returns>
        public static DirectoryEntry GetDirectoryEntry()
        {
            DirectoryEntry de = new DirectoryEntry();

            de.Path = AdDefaultPath;
            de.Username = AdUserName;
            de.Password = AdPassword;
            return de;
        }

        public static DirectoryEntry GetSupporterDirectoryEntry()
        {
            DirectoryEntry de = new DirectoryEntry();

            de.Path = AdPathForSupporter;
            de.Username = AdUserName;
            de.Password = AdPassword;
            return de;
        }

        //Validate if a user exists
        /// <summary>
        /// Method to validate if a user exists in the AD.
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static bool UserExists(string UserName)
        {
            DirectoryEntry de = GetDirectoryEntry();
            DirectorySearcher deSearch = new DirectorySearcher();
            deSearch.SearchRoot = de;
            deSearch.Filter = "(&(objectClass=user) (cn=" + UserName + "))";
            SearchResultCollection results = deSearch.FindAll();

            if (results.Count == 0)
            {
                de.Dispose();
                return false;
            }
            else
            {
                de.Dispose();
                return true;
            }
        }


        // Set user's properties
        /// <summary>
        /// Helper method that sets properties for AD users.
        /// </summary>
        /// <param name="de"></param>
        /// <param name="PropertyName"></param>
        /// <param name="PropertyValue"></param>
        public static void SetProperty(DirectoryEntry de, string PropertyName, string PropertyValue)
        {

            if (PropertyValue != null)
            {

                if (PropertyValue != "")
                {
                    if (de.Properties.Contains(PropertyName))
                    {
                        de.Properties[PropertyName][0] = PropertyValue;
                    }
                    else
                    {
                        de.Properties[PropertyName].Add(PropertyValue);
                    }
                }
                else
                {
                    if (de.Properties.Contains(PropertyName))
                    {
                        PropertyValue = de.Properties[PropertyName][0].ToString();
                        de.Properties[PropertyName].Remove(PropertyValue);
                    }

                }
            }
        }

         
        /// <summary>
        /// Method that validates if a string has an email pattern.
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public bool IsEmail(string mail)
        {
            Regex mailPattern = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return mailPattern.IsMatch(mail);
        }



        //15. Format dates to AD date format (AAAAMMDDMMSSSS.0Z)
        /// <summary>
        /// Method that formats a date in the required format
        /// needed (AAAAMMDDMMSSSS.0Z) to compare dates in AD.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Date in valid format for AD</returns>
        public string ToADDateString(DateTime date)
        {
            string year = date.Year.ToString();
            int month = date.Month;
            int day = date.Day;

            StringBuilder sb = new StringBuilder();
            sb.Append(year);
            if (month < 10)
            {
                sb.Append("0");
            }
            sb.Append(month.ToString());
            if (day < 10)
            {
                sb.Append("0");
            }
            sb.Append(day.ToString());
            sb.Append("000000.0Z");
            return sb.ToString();
        }


        /// <summary>
        /// 根据用户Id来获取用户信息,返回结果集
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static SearchResultCollection GetUserInfo(string UserId)
        {
            DirectorySearcher deSearch = new DirectorySearcher();

            DirectoryEntry de = GetDirectoryEntry();
            deSearch.SearchRoot = de;
            deSearch.Filter = "(&(objectClass=user)(sAMAccountName=" + UserId + "))";
            SearchResultCollection results = deSearch.FindAll();
            if (results.Count == 0)//如Employee没有在support中找
            {
                DirectoryEntry supporterDE = GetSupporterDirectoryEntry();
                deSearch.SearchRoot = supporterDE;
                deSearch.Filter = "(&(objectClass=user)(sAMAccountName=" + UserId + "))";
                results = deSearch.FindAll();

                supporterDE.Dispose();
            }

            de.Dispose();

            deSearch.Dispose();
            return results;
        }

        /// <summary>
        /// 获取用户姓名（显示名称）
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static string GetUserNameBySAMAccountName(string UserId)
        {
            SearchResultCollection r = GetUserInfo(UserId);
            if (r.Count <= 0)
            {
                return "";
            }
            else
            {
                return r[0].Properties["CN"][0].ToString();
            }

        }

         
        /// <summary>
        /// 获取部门三字码
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public string GetDepartment(string userId)
        {
            string dept = "";
            string path = "";
            SearchResultCollection resultCollection = GetUserInfo(userId);
            if (resultCollection != null && resultCollection.Count > 0)
            {
                path = resultCollection[0].Path;
            }
            if (path != "")
            {
                string departInfo = "";
                string[] arr = path.Split(',');
                if (arr.Length > 0)
                {
                    for (int i = arr.Length - 1; i > 0; i--)
                    {
                        string temp = arr[i];
                        //查询第一个以ou开头的字符串
                        if (temp.ToUpper().StartsWith("OU"))
                        {
                            temp = temp.Substring(temp.IndexOf('=') + 1);
                            if (temp.ToUpper() == "EMPLOYEE")
                            {
                                for (int j = i - 1; j > 0; j--)
                                {
                                    temp = arr[j];
                                    temp = temp.Substring(temp.IndexOf('=') + 1);
                                    departInfo += temp + "=>";
                                }
                                break;
                            }
                        }
                    }

                    dept = departInfo.TrimEnd(new char[] { '=', '>' });

                }
            }

            if (dept == null)
            {
                return null;
            }
            else
            {
                return dept.Substring(dept.LastIndexOf("=>") + 2, 3);
            }

        }

        #region 获取域用户组组员

        /// <summary>
        /// 获取一个域用户组中所有组成员Email地址。
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="groupName"></param>
        /// <returns>string</returns>
        public static string GetGroupMemberEmail(string domain, string groupName)
        {
            try
            {
                string strMails = string.Empty;//返回值

                DirectoryEntry oDe = GetGroupEntryByName(domain, groupName);
                //如果没有此域用户组，则返回空字符串。
                if (oDe == null || oDe.Path == string.Empty)
                {
                    return strMails;
                }

                //遍历查找出该组的所有成员Email
                object[] oArray = (object[])(oDe.Properties["member"].Value);
                foreach (object o in oArray)
                {
                    string strMember = (string)o;

                    string mail = null;
                    string name = null;

                    DirectoryEntry de = new DirectoryEntry("LDAP://" + strMember);
                    name = de.Properties["samaccountname"][0].ToString();
                    if (IsADGroup(domain, name))
                    {
                        strMails += GetGroupMemberEmail(domain, name);
                    }
                    else
                    {
                        mail = de.Properties["mail"].Value != null ? de.Properties["mail"][0].ToString() : "";
                        strMails += mail + ";";
                    }
                }
                return strMails;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 根据域用户组名，查找该域用户组，返回一个DirectoryEntry对象。
        /// </summary>
        /// <param name="domain">域名</param>
        /// <param name="groupName">域用户组名</param>
        /// <returns>DirectoryEntry</returns>
        public static DirectoryEntry GetGroupEntryByName(string domain, string groupName)
        {
            DirectoryEntry de = new DirectoryEntry("LDAP://DC=" + domain + ",DC=com");
            DirectorySearcher ds = new DirectorySearcher(de, "(objectClass=user)");
            ds.Filter = "(&(&(objectCategory=group)(objectClass=group))(cn=" + groupName + "))";
            ds.PropertiesToLoad.Add("givenname");
            ds.PropertiesToLoad.Add("samaccountname");

            try
            {
                DirectoryEntry oE = new DirectoryEntry();
                SearchResult sr = ds.FindOne();
                if (sr != null)
                {
                    oE = sr.GetDirectoryEntry();
                }
                return oE;
            }
            catch
            {
                return null;
            }
            finally
            {
                de.Dispose();
                ds.Dispose();
            }
        }

        /// <summary>
        /// 传入一个组名参数，判断是否存在该组，返回bool值。
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="groupName"></param>
        /// <returns>bool</returns>
        public static bool IsADGroup(string domain, string groupName)
        {
            string sam = null;
            DirectoryEntry oDe = GetGroupEntryByName(domain, groupName);
            if (oDe != null && oDe.Path != string.Empty)
            {
                sam = oDe.Properties["samaccountname"][0].ToString();
            }

            if (sam != null && sam.ToLower() == groupName.ToLower())
            {
                return true;
            }
            return false;

        }


        /// <summary>
        /// 根据多个条件来获取用户信息,返回结果集
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static SearchResultCollection GetMutiUserInfo(string KeyWords)
        {
            DirectorySearcher deSearch = new DirectorySearcher();

            DirectoryEntry de = GetDirectoryEntry();
            deSearch.SearchRoot = de;

            string _filiter = null;

            _filiter += string.Format("(|(sAMAccountName=*{0}*)(displayName=*{0}*)", KeyWords);
            _filiter += String.Format("(EntryAccount=*{0}*)", KeyWords);
            _filiter += string.Format("(mail=*{0}*))", KeyWords);
            // deSearch.Filter = "(&(objectClass=user)(sAMAccountName=" + UserId + "))";
            deSearch.Filter = "(&" + _filiter + ")";
            deSearch.SizeLimit = 200;
            SearchResultCollection results = deSearch.FindAll();
            if (results.Count == 0)//如Employee没有在support中找
            {
                DirectoryEntry supporterDE = GetSupporterDirectoryEntry();
                deSearch.SearchRoot = supporterDE;
                //  deSearch.Filter = "(&(objectClass=user)(sAMAccountName=" + UserId + "))";

                deSearch.Filter = "(&" + _filiter + ")";
                results = deSearch.FindAll();

                supporterDE.Dispose();
            }

            de.Dispose();

            deSearch.Dispose();
            return results;
        }

        #endregion


    }
}
