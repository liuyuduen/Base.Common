using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Base.Utility
{
    /// <summary>
    /// 后台执行 js 的各种操作
    /// </summary>
    public class JSHelper
    {
        private static Page page
        {
            get
            {
                return (Page)HttpContext.Current.Handler;
            }
        }

        /// <summary>
        /// 页面弹出信息
        /// </summary>
        /// <param name="msg">要弹出的信息</param>
        public static void Alert(string msg)
        {
            msg = GetAlertStr(msg);
            ExecuteScript("alert(\"" + msg + "\");");
        }

        /// <summary>
        /// 弹出信息，并转到指定的url
        /// </summary>
        /// <param name="msg">要弹出的信息</param>
        /// <param name="url">要转到的地址</param>
        public static void AlertAndRedirect(string msg, string url)
        {
            msg = GetAlertStr(msg);
            ExecuteScript("alert(\"" + msg + "\");location=\"" + url + "\";");
        }

        /// <summary>
        /// 弹出信息,用户点击确定后关闭窗口
        /// </summary>
        /// <param name="msg">弹出的信息</param>
        public static void AlertAndClose(string msg)
        {
            msg = GetAlertStr(msg);
            ExecuteScript("alert(\"" + msg + "\");window.opener=null;window.open(\"\",\"_self\");window.close();");
        }

        /// <summary>
        /// 弹出确认信息，并根据用户选择结果执行指定脚本
        /// </summary>
        /// <param name="msg">要弹出的信息</param>
        /// <param name="okScript">点击"确认"按钮后，执行的脚本</param>
        /// <param name="cancelScript">点击"取消"按钮后，执行的脚本</param>
        public static void ConfirmAndExecuteScript(string msg, string okScript, string cancelScript)
        {
            msg = GetAlertStr(msg);
            ExecuteScript("if(confirm(\"" + msg + "\")) {" + okScript + "} else {" + cancelScript + "}");
        }

        /// <summary>
        /// 获得规范格式的 alert信息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private static string GetAlertStr(string msg)
        {
            msg = msg.Replace("\"", " ");
            msg = msg.Replace("\\", "\\\\");
            msg = msg.Replace("\r\n", " ");
            msg = msg.Replace("\n\n", " ");
            msg = msg.Replace("<br>", "\\n");
            return msg;
        }

        /// <summary>
        /// 提示错误信息！操作回滚！
        /// Silver:缺点是会失去css样式，建议直接在页面使用
        /// string script = "<script>alert('请输入商品名称');</script>";
        ///Page.ClientScript.RegisterStartupScript(this.GetType(), "", script);的方式
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="code"></param>
        public static void ShowMsg(string msg)
        {
            HttpContext.Current.Response.Write("<script>alert(\"" + msg + "\");history.back();</script>");
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 提示成功信息！转向！
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="url"></param>
        public static void ShowMsg(string msg, string url)
        {
            HttpContext.Current.Response.Write("<script>alert(\"" + msg + "\");location.href='" + url + "';</script>");
            HttpContext.Current.Response.End();
        }


        #region 窗口操作

        /// <summary>
        /// 打开新窗口
        /// </summary>
        /// <param name="Url">打开的窗口的地址</param>
        public void OpenWindow(string Url)
        {
            string Ojs = @"<script language='JavaScript'>
					    window.open('{0}','','');
                        </script>";
            HttpContext.Current.Response.Write(string.Format(Ojs, Url.ToString()));
        }



        /// <summary>
        /// 打开全屏窗口
        /// </summary>
        /// <param name="full">输入任可数字为全屏窗口</param>
        /// <param name="Url">打开的窗口的地址</param>
        /// <param name="FullWindow">打开全屏的窗口</param>
        public void OpenWindow(int FullWindow, string Url)
        {
            string Ojs = @"<script language='JavaScript'>
						screenH=window.screen.height;
						screenW=window.screen.width;
					    window.open('{0}','','width='+screenW+',height='+screenH+',top=0px,left=0px,scrollbars=yes,location=no,menubar=no,resizable=yes,status=no,toolbar=no');
                        </script>";
            HttpContext.Current.Response.Write(string.Format(Ojs, Url.ToString()));
        }


        /// <summary>
        /// 打开全屏窗口
        /// </summary>
        /// <param name="full">输入任可数字为全屏窗口</param>
        /// <param name="Url">打开的窗口的地址</param>
        /// <param name="OtherJs">打开窗口后执行的脚本，如果不需就为空。如(window.close();)</param>
        /// <param name="FullWindow">打开全屏窗口后，执行Js代码。</param>
        public void OpenWindow(int FullWindow, string Url, string OtherJs)
        {
            string Ojs = @"<script language='JavaScript'>
						screenH=window.screen.height;
						screenW=window.screen.width;
					    window.open('{0}','','width='+screenW+',height='+screenH+',top=0px,left=0px,scrollbars=yes,location=no,menubar=no,resizable=yes,status=no,toolbar=no');
						{1}	
                        </script>";
            HttpContext.Current.Response.Write(string.Format(Ojs, Url.ToString(), OtherJs.ToString()));
        }



        /// <summary>
        /// 打开自定义大小窗口
        /// </summary>
        /// <param name="Url">窗口的地址栏</param>
        /// <param name="Height">窗口的高度</param>
        /// <param name="Width">窗口的宽度</param>
        public void OpenWindow(string Url, string Height, string Width)
        {
            string Cjs = @"<script language='JavaScript'>
					    window.open('{0}','','width={1},height={2},top=0px,left=0px,scrollbars=no,location=no,menubar=no,resizable=no,status=no,toolbar=no');
                        </script>";
            HttpContext.Current.Response.Write(string.Format(Cjs, Url.ToString(), Width, Height));
        }


        /// <summary>
        /// 打开自定义大小窗口，并执行操作。
        /// </summary>
        /// <param name="Url">窗口的地址栏</param>
        /// <param name="Height">窗口的高度</param>
        /// <param name="Width">窗口的宽度</param>
        /// <param name="Js">打开窗口执行的Js语句</param>
        public void OpenWindow(string Url, string Height, string Width, string Js)
        {
            string Cjs = @"<script language='JavaScript'>
					    window.open('{0}','','width={1},height={2},top=0px,left=0px,scrollbars=no,location=no,menubar=no,resizable=no,status=no,toolbar=no');
						{3};
                        </script>";
            HttpContext.Current.Response.Write(string.Format(Cjs, Url.ToString(), Width, Height, Js));
        }

        #endregion

        #region 对话框的操作

        /// <summary>
        /// 弹出WINDOW的对话框
        /// </summary>
        /// <param name="Url">打开的地址栏</param>
        /// <param name="Width">对话框的高度</param>
        /// <param name="Height">对话框的宽度</param>
        public void openDioag(string Url, string Width, string Height)
        {
            string Dijs = @"<script language='JavaScript'>
							window.showModalDialog('{0}', '', 'dialogWidth:{1}px; dialogHeight:{2}px; status:no; directories:no;scrollbars:no;Resizable=no;' );					    
                           </script>";
            HttpContext.Current.Response.Write(string.Format(Dijs, Url, Width, Height));
        }



        /// <summary>
        /// 关闭当前窗口
        /// </summary>
        public void WindowClose()
        {
            HttpContext.Current.Response.Write("<script language='javascript'>window.close();</script>");
        }

        /// <summary>
        /// 关闭当前窗口，刷新母窗口。
        /// </summary>
        /// <param name="Url">新窗口的名称</param>
        public void ReClose(string Url)
        {
            string Os = @"<script language='JavaScript'>
						window.opener.location='{0}';	
						window.close();
                        </script>";
            HttpContext.Current.Response.Write(string.Format(Os, Url.ToString()));
        }


        /// <summary>
        /// 弹出对话框,关闭当前窗口，刷新窗口名称。
        /// </summary>
        /// <param name="Message">对话框内容</param>
        /// <param name="Url">新窗口的名称</param>
        public void ReClose(string Message, string Url)
        {
            string Os = @"<script language='JavaScript'>
						alert('{0}'); 
						window.opener.location='{1}';	
						window.close();
                        </script>";
            HttpContext.Current.Response.Write(string.Format(Os, Message.ToString().Replace("'", "").Replace("\\", "|").ToString(), Url.ToString()));
        }

        /// <summary>
        /// 根据传入的表单与变量名传入值
        /// </summary>
        /// <param name="Formname">表单名</param>
        /// <param name="Fvaluename">变量名</param>
        /// <param name="Url">传入值</param>
        public void ReClose(string Formname, string Fvaluename, string Url)
        {
            string Os = @"<script language='JavaScript'>
						window.opener.document.{0}.{1}.value='{2}';
						window.close();
                        </script>";
            HttpContext.Current.Response.Write(string.Format(Os, Formname, Fvaluename, Url));
        }


     

        #endregion
        /// <summary>
        /// 执行脚本（输出到from开始标签的后面）
        /// </summary>
        /// <param name="script"></param>
        public static void ExecuteScript(string script)
        {
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), Guid.NewGuid().ToString(), script, true);
        }

        /// <summary>
        /// 执行脚本(输出到from结束标签的前面)
        /// </summary>
        public static void ExecuteScriptFrontOfEndForm(string script)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), Guid.NewGuid().ToString(), script, true);
        }

        /// <summary>
        /// 向页面中注册JS脚本
        /// </summary>
        /// <param name="jsFileName">路径(绝对、相对都可以) + 文件名 ( ../js/init.debug.js )</param>
        public static void RegisterJsFile(string jsFileName)
        {
            page.ClientScript.RegisterClientScriptInclude(page.GetType(), Guid.NewGuid().ToString(), jsFileName);
        }

        /// <summary>
        /// 向页面中注册CSS样式
        /// </summary>
        /// <param name="cssFileName">路径(绝对、相对都可以) + 文件名 ( ../css/base.css )</param>
        public static void RegisterCssFile(string cssFileName)
        {
            HtmlLink link1 = new HtmlLink();
            link1.Attributes["type"] = "text/css";
            link1.Attributes["rel"] = "Stylesheet";
            link1.Attributes["href"] = cssFileName; ;

            page.Header.Controls.Add(link1);
        }

        /// <summary>
        /// 向页面注册脚本,用于获取页面元素的JS引用
        /// </summary>
        /// <param name="controls">控件集</param>
        public static void ClientScriptCreate(ControlCollection controls)
        {
            StringBuilder stb = new StringBuilder();
            ClientScriptConnect(controls, stb);
            ExecuteScriptFrontOfEndForm(stb.ToString());
        }

        /// <summary>
        /// 需注册到页面的脚本的拼接
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="stb"></param>
        private static void ClientScriptConnect(ControlCollection controls, StringBuilder stb)
        {
            foreach (Control c in controls)
            {
                if (c.Visible && !string.IsNullOrEmpty(c.ID))
                {
                    stb.AppendFormat("var {0} = document.getElementById('{1}');", c.ID, c.ClientID);
                }
                if (c.Controls.Count > 0)
                {
                    if (c is CheckBoxList)
                    {
                        ClientScriptConnect(c.ID, c.ClientID, ((CheckBoxList)c).Items, stb);
                    }
                    else
                    {
                        if (c is RadioButtonList)
                            ClientScriptConnect(c.ID, c.ClientID, ((RadioButtonList)c).Items, stb);
                        else
                            ClientScriptConnect(c.Controls, stb);
                    }
                }
            }
        }

        /// <summary>
        /// 需注册到页面的脚本的拼接
        /// </summary>
        /// <param name="parentID"></param>
        /// <param name="parentClientID"></param>
        /// <param name="items"></param>
        /// <param name="stb"></param>
        private static void ClientScriptConnect(string parentID, string parentClientID, ListItemCollection items, StringBuilder stb)
        {
            stb.AppendFormat("var {0} = new Array();", parentID + "_items");

            for (int i = 0; i < items.Count; i++)
            {
                stb.AppendFormat("var {0} = document.getElementById('{1}');", parentID + "_" + i, parentClientID + "_" + i);
                stb.AppendFormat("{0}[{1}.length]={2};", parentID + "_items", parentID + "_items", parentID + "_" + i);
            }

        }

    }
}
