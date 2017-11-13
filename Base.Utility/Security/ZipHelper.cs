using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ICSharpCode.SharpZipLib.Zip;

namespace Base.Utility.Security
{
    /// <summary>
    /// 压缩、解压缩
    /// </summary>
    public class ZipHelper
    {
        /// <summary>
        /// 压缩单个文件
        /// </summary>
        /// <param name="FileToZip">被压缩的文件名称(包含文件路径)</param>
        /// <param name="ZipedFile">压缩后的文件名称(包含文件路径)</param>
        /// <param name="CompressionLevel">压缩率0（无压缩）-9（压缩率最高），通常设为6</param>
        /// <param name="BlockSize">缓存大小，通常设为2048</param>
        public static bool PackFile(string FileToZip, string ZipedFile, int CompressionLevel, int BlockSize)
        {
            //如果文件没有找到，则报错 
            if (!System.IO.File.Exists(FileToZip))
            {
                return false;
            }

            FileStream StreamToZip = new System.IO.FileStream(FileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.FileStream ZipFile = System.IO.File.Create(ZipedFile);
            ZipOutputStream ZipStream = new ZipOutputStream(ZipFile);
            ZipEntry ZipEntry = new ZipEntry(new FileInfo(FileToZip).Name);
            ZipStream.PutNextEntry(ZipEntry);
            ZipStream.SetLevel(CompressionLevel);
            byte[] buffer = new byte[BlockSize];
            System.Int32 size = StreamToZip.Read(buffer, 0, buffer.Length);
            ZipStream.Write(buffer, 0, size);
            try
            {
                while (size < StreamToZip.Length)
                {
                    int sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                    ZipStream.Write(buffer, 0, sizeRead);
                    size += sizeRead;
                }
            }
            catch (Exception)
            {
                return false;
            }
            ZipStream.Finish();
            ZipStream.Close();
            StreamToZip.Close();
            return true;
        }


        /// <summary>压缩文件夹</summary>
        /// <remarks> 
        /// </remarks>
        /// <param name="filename">filename生成的文件的名称，如：C\123\123.zip</param>
        /// <param name="directory">directory要压缩的文件夹路径</param>
        /// <returns></returns>
        public static bool PackFiles(string filename, string directory)
        {
            try
            {

                directory = directory.Replace("/", "\\");

                if (!directory.EndsWith("\\"))
                    directory += "\\";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                FastZip fz = new FastZip();
                fz.CreateEmptyDirectories = true;
                fz.CreateZip(filename, directory, true, "");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <remarks> 
        /// </remarks>
        /// <param name="relativePath">压缩文件的名称，如：C:\123\123.zip</param>
        /// <param name="dir">要解压的文件夹路径</param>
        public static bool UnpackFiles(string relativePath, string dir)
        {
            try
            {
                if (!File.Exists(relativePath))
                    return false;

                dir = dir.Replace("/", "\\");
                if (!dir.EndsWith("\\"))
                    dir += "\\";

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                ZipInputStream s = new ZipInputStream(File.OpenRead(relativePath));
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {

                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    if (directoryName != String.Empty)
                        Directory.CreateDirectory(dir + directoryName);

                    if (fileName != String.Empty)
                    {
                        FileStream streamWriter = File.Create(dir + theEntry.Name);

                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }

                        streamWriter.Close();
                    }
                }
                s.Close(); return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
