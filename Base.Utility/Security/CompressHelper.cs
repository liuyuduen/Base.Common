using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.BZip2;

namespace Base.Utility
{
    /// <summary>
    /// 压缩辅助类
    /// </summary>
    public class CompressHelper
    {
        /**/
        /// <summary>
        /// 设定压缩比率，压缩比率越高性消耗也将增大
        /// </summary>
        private static Int32 ZipLevel = ICSharpCode.SharpZipLib.Zip.Compression.Deflater.BEST_COMPRESSION;
        /**/
        /// <summary>
        /// 对字符串进行压缩
        /// </summary>
        /// <param name="Str">字符串</param>
        /// <returns>返回流的字节数组</returns>
        public static string Compress(String str)
        {
            //将存储状态的Base64字串转换为字节数组
            Byte[] pBytes = System.Convert.FromBase64String(str);

            //创建支持内存存储的流
            MemoryStream mMemory = new MemoryStream();

            Deflater mDeflater = new Deflater(ZipLevel);
            ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream mStream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(mMemory, mDeflater, 131072);

            mStream.Write(pBytes, 0, pBytes.Length);
            mStream.Close();

            return ByteToString(mMemory.ToArray());
        }

        /**/
        /// <summary>
        /// 解压缩字符串
        /// </summary>
        /// <param name="str">被压缩的字符串</param>
        /// <returns>返回流的字节数组</returns>
        public static string DeCompress(String str)
        {
            //将Base64字符串转换为字节数组
            Byte[] pBytes = System.Convert.FromBase64String(str);

            ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream mStream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(new MemoryStream(pBytes));

            //创建支持内存存储的流
            MemoryStream mMemory = new MemoryStream();
            Int32 mSize;

            Byte[] mWriteData = new Byte[4096];

            while (true)
            {
                mSize = mStream.Read(mWriteData, 0, mWriteData.Length);
                if (mSize > 0)
                {
                    mMemory.Write(mWriteData, 0, mSize);
                }
                else
                {
                    break;
                }
            }

            mStream.Close();
            return ByteToString(mMemory.ToArray());
        }


        private static string ByteToString(Byte[] pBytes)
        {
            return System.Convert.ToBase64String(pBytes);
        }


        //使用GZIP压缩文件的方法
        static bool GZipFile(string sourcefilename, string zipfilename)
        {
            bool blResult;//表示压缩是否成功的返回结果
            //为源文件创建读取文件的流实例
            FileStream srcFile = File.OpenRead(sourcefilename);
            //为压缩文件创建写入文件的流实例，
            GZipOutputStream zipFile = new GZipOutputStream(File.Open(zipfilename, FileMode.Create));
            try
            {
                byte[] FileData = new byte[srcFile.Length];//创建缓冲数据
                srcFile.Read(FileData, 0, (int)srcFile.Length);//读取源文件
                zipFile.Write(FileData, 0, FileData.Length);//写入压缩文件
                blResult = true;
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
                blResult = false;
            }
            srcFile.Close();//关闭源文件
            zipFile.Close();//关闭压缩文件
            return blResult;
        }
        //使用GZIP解压文件的方法
        static bool UnGzipFile(string zipfilename, string unzipfilename)
        {
            bool blResult;//表示解压是否成功的返回结果
            //创建压缩文件的输入流实例
            GZipInputStream zipFile = new GZipInputStream(File.OpenRead(zipfilename));
            //创建目标文件的流
            FileStream destFile = File.Open(unzipfilename, FileMode.Create);
            try
            {
                int buffersize = 2048;//缓冲区的尺寸，一般是2048的倍数
                byte[] FileData = new byte[buffersize];//创建缓冲数据
                while (buffersize > 0)//一直读取到文件末尾
                {
                    buffersize = zipFile.Read(FileData, 0, buffersize);//读取压缩文件数据
                    destFile.Write(FileData, 0, buffersize);//写入目标文件
                }
                blResult = true;
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
                blResult = false;
            }
            destFile.Close();//关闭目标文件
            zipFile.Close();//关闭压缩文件
            return blResult;
        }
    }
}
