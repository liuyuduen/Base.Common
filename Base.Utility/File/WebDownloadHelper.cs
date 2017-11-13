using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace Base.Utility
{

    public static class WebDownloadHelper
    {
        static Encoding DefaultEncoding = Encoding.Default;
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileFullName">要下载文件的完整路径</param>
        public static void DownloadFile(string fileFullName)
        {
            DownloadFile(fileFullName, Path.GetFileName(fileFullName));
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileFullName">要下载文件的完整路径</param>
        /// <param name="clientFileName">要保存到客户端的文件名</param>
        public static void DownloadFile(string fileFullName, string clientFileName)
        {
            HttpResponse Response = HttpContext.Current.Response;
            HttpRequest Request = HttpContext.Current.Request;

            System.IO.Stream iStream = null;
            byte[] buffer = new Byte[10240];

            int length;

            long dataToRead;

            try
            {
                iStream = new System.IO.FileStream(fileFullName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                Response.Clear();

                dataToRead = iStream.Length;

                long p = 0;
                if (Request.Headers["Range"] != null)
                {
                    Response.StatusCode = 206;
                    p = long.Parse(Request.Headers["Range"].Replace("bytes=", "").Replace("-", ""));
                }
                if (p != 0)
                {
                    Response.AddHeader("Content-Range", "bytes " + p.ToString() + "-" + ((long)(dataToRead - 1)).ToString() + "/" + dataToRead.ToString());
                }
                Response.AddHeader("Content-Length", ((long)(dataToRead - p)).ToString());
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(System.Text.Encoding.GetEncoding(65001).GetBytes(Path.GetFileName(clientFileName))));

                iStream.Position = p;
                dataToRead = dataToRead - p;

                while (dataToRead > 0)
                {
                    if (Response.IsClientConnected)
                    {
                        length = iStream.Read(buffer, 0, 10240);

                        Response.OutputStream.Write(buffer, 0, length);
                        Response.Flush();

                        buffer = new Byte[10240];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("Error : " + ex.Message);
            }
            finally
            {
                if (iStream != null)
                {
                    iStream.Close();
                }
                Response.End();
            }

        }

        /// <summary>
        /// 下载流到文件
        /// </summary>
        /// <param name="stream">要下载的流</param>
        /// <param name="clientFileName">要保存到客户端的文件名</param>
        public static void Download(Stream stream, string clientFileName)
        {
            HttpResponse Response = HttpContext.Current.Response;
            HttpRequest Request = HttpContext.Current.Request;



            byte[] buffer = new Byte[10240];

            int length;

            long dataToRead;
            Response.Clear();
            dataToRead = stream.Length;
            Response.AddHeader("Content-Length", dataToRead.ToString());
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(System.Text.Encoding.GetEncoding(65001).GetBytes(Path.GetFileName(clientFileName))));


            try
            {
                stream.Position = 0;
                while (dataToRead > 0)
                {
                    if (Response.IsClientConnected)
                    {
                        length = stream.Read(buffer, 0, 10240);

                        Response.OutputStream.Write(buffer, 0, length);
                        Response.Flush();

                        buffer = new Byte[10240];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("Error : " + ex.Message);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
                Response.End();
            }
        }

        /// <summary>
        /// 下载文本内容
        /// </summary>
        /// <param name="content">要下载的文本内容</param>
        /// <param name="clientFileName">要保存到客户端的文件名</param>
        public static void Download(string content, string clientFileName)
        {
            HttpResponse Response = HttpContext.Current.Response;
            HttpRequest Request = HttpContext.Current.Request;

            byte[] buffer = DefaultEncoding.GetBytes(content);

            long dataToRead;
            Response.Clear();
            dataToRead = buffer.Length;
            Response.AddHeader("Content-Length", dataToRead.ToString());
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(System.Text.Encoding.GetEncoding(65001).GetBytes(Path.GetFileName(clientFileName))));


            try
            {
                Response.OutputStream.Write(buffer, 0, (int)dataToRead);
            }
            catch (Exception ex)
            {
                Response.Write("Error : " + ex.Message);
            }
            finally
            {
                Response.End();
            }
        }
    }
}
