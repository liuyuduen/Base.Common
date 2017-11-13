using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Web.UI.WebControls;
using System.IO;
using System.Web;
using Base.Utility;

namespace Base.Utility
{

    /// <summary>
    /// 图片处理类
    /// 图片上传的方法在FileTool中
    /// </summary>
    public class ImageHelper : System.Web.UI.Page
    {
        #region 生成验证码图片

        /// <summary>
        /// 生成验证码所需4位随机数
        /// </summary>
        /// <returns></returns>
        public string GenerateCheckCode()
        {
            int number;
            string strCode = "";
            Random random = new Random();
            int i = 0;
            for (i = 0; i <= 1; i++)
            {
                number = random.Next();
                number = number % 36;
                if (number < 10)
                { number += 48; }
                else
                { number += 55; }

                strCode = strCode + number.ToString();
            }

            return strCode;

        }

        #endregion

        #region 生成缩略图/增加文字水印/生成图片水印

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>    
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）                
                    break;
                case "W"://指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(System.Drawing.Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
                new System.Drawing.Rectangle(x, y, ow, oh),
                System.Drawing.GraphicsUnit.Pixel);
            string temp = "";
            try
            {
                //以jpg格式保存缩略图

                if (originalImagePath == thumbnailPath)
                {

                    temp = GetTheSameNameRand(thumbnailPath);
                    bitmap.Save(temp, System.Drawing.Imaging.ImageFormat.Jpeg);


                }
                else
                {
                    bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
                if (temp != "")
                {
                    File.Delete(thumbnailPath);
                    File.Copy(temp, thumbnailPath);
                    File.Delete(temp);
                }
            }
        }


        /**/
        /// <summary>
        /// 在图片上增加文字水印
        /// </summary>
        /// <param name="Path">原服务器图片路径</param>
        /// <param name="Path_sy">生成的带文字水印的图片路径</param>
        /// <param name="text">文字内容</param>
        public static void AddWater(string Path, string Path_sy, string text)
        {
            string addText = text;
            System.Drawing.Image image = System.Drawing.Image.FromFile(Path);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
            g.DrawImage(image, 0, 0, image.Width, image.Height);
            System.Drawing.Font f = new System.Drawing.Font("Verdana", 60);
            System.Drawing.Brush b = new System.Drawing.SolidBrush(System.Drawing.Color.Green);

            g.DrawString(addText, f, b, 35, 35);
            g.Dispose();

            image.Save(Path_sy);
            image.Dispose();
        }

        /**/
        /// <summary>
        /// 在图片上生成图片水印
        /// </summary>
        /// <param name="Path">原服务器图片路径</param>
        /// <param name="Path_syp">生成的带图片水印的图片路径</param>
        /// <param name="Path_sypf">水印图片路径</param>
        public static void AddWaterPic(string Path, string Path_syp, string Path_sypf)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile(Path);
            System.Drawing.Image copyImage = System.Drawing.Image.FromFile(Path_sypf);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
            g.DrawImage(copyImage, new System.Drawing.Rectangle((image.Width - copyImage.Width) / 2, (image.Height - copyImage.Height) / 2, copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height, System.Drawing.GraphicsUnit.Pixel);
            g.Dispose();

            if (Path != Path_syp)
            {
                image.Save(Path_syp);
                image.Dispose();
            }
            else
            {
                string temp = GetTheSameNameRand(Path_syp);
                image.Save(temp);
                image.Dispose();

                File.Delete(Path);
                File.Copy(temp, Path);
                File.Delete(temp);

            }

        }
        /// <summary>
        /// 获取某一个文件，返回与此文件名类似的临时文件名
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <returns></returns>
        private static string GetTheSameNameRand(string filepath)
        {
            string dir = System.IO.Path.GetDirectoryName(filepath);
            string name = System.IO.Path.GetFileNameWithoutExtension(filepath);
            string extname = System.IO.Path.GetExtension(filepath);

            name = name + Guid.NewGuid().ToString();

            return dir + name + extname;
        }

        #region 内存流方式处理

        /// <summary>
        /// 给内存图片设置水印处理
        /// </summary>
        /// <param name="imageStream">内存图片流对象</param>
        /// <param name="newPicPath">保存新图片的的实际路径</param>
        /// <param name="watermarkPath">将要加载到内存图片中的水印图片地址</param>
        /// <returns>图片保存是否成功</returns>
        public static bool WatermarkPic(System.IO.Stream imageStream, string newPicPath, string watermarkPath)
        {
            bool isSaveOk = false;
            System.Drawing.Image oldImageObj = new System.Drawing.Bitmap(imageStream);
            System.Drawing.Image newImageObj = new System.Drawing.Bitmap(oldImageObj.Width, oldImageObj.Height);
            //新建一个画板 
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newImageObj);
            //设置高质量插值法 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度 
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充 
            g.Clear(System.Drawing.Color.Transparent);
            //在指定位置并且按指定大小绘制原图片的指定部分 
            g.DrawImage(oldImageObj, new System.Drawing.Rectangle(0, 0, newImageObj.Width, newImageObj.Height), new System.Drawing.Rectangle(0, 0, newImageObj.Width, newImageObj.Height), System.Drawing.GraphicsUnit.Pixel);
            //处理水印
            try
            {
                //水印图片对象
                System.Drawing.Image watermarkImageObj = new System.Drawing.Bitmap(watermarkPath);
                //缩放水印图片对象(在newImageObj的底部1/2处绘制水印图片：按其缩放水印图片)
                System.Drawing.Bitmap zoomWatermarkImageObj = new Bitmap(watermarkImageObj, newImageObj.Width / 2, (watermarkImageObj.Height * newImageObj.Height / 2) / watermarkImageObj.Width);
                //设置水印图片为透明色
                zoomWatermarkImageObj.MakeTransparent(Color.White);
                //绘制水印图片
                g.DrawImage(zoomWatermarkImageObj, new System.Drawing.PointF(newImageObj.Width - zoomWatermarkImageObj.Width, newImageObj.Height - zoomWatermarkImageObj.Height));
                watermarkImageObj.Dispose();
                zoomWatermarkImageObj.Dispose();
            }
            catch
            {
            }

            //保存
            try
            {
                newImageObj.Save(newPicPath, System.Drawing.Imaging.ImageFormat.Bmp);
                isSaveOk = true;
            }
            catch (System.Exception e)
            {
                isSaveOk = false;
                throw e;
            }
            finally
            {
                try
                {
                    newImageObj.Dispose();
                    newImageObj.Dispose();
                    g.Dispose();
                }
                catch
                {
                }
            }
            return isSaveOk;
        }

        /// <summary>
        /// 按宽度:高成比例缩放图片并加水印处理
        /// </summary>
        /// <param name="stream">内存图片流对象</param>
        /// <param name="newPicPath">保存新图片的的实际路径</param>
        /// <param name="intWidth">按比例缩放到intWidth宽度</param>
        /// <param name="watermarkImage">水印图片实际地址</param>
        /// <returns>是否成功</returns>
        public static bool ZoomImageWidth(System.IO.Stream stream, string newPicPath, int intWidth, string watermarkImage)
        {
            System.Drawing.Image oldImageObj = System.Drawing.Image.FromStream(stream);
            int x = 0;
            int y = 0;
            int ow = oldImageObj.Width;
            int oh = oldImageObj.Height;
            //只缩小不放大
            int toheight = oldImageObj.Height;
            int towidth = oldImageObj.Width;
            if (intWidth < oldImageObj.Width)
            {
                toheight = oldImageObj.Height * intWidth / oldImageObj.Width;
                towidth = intWidth;
            }
            return SaveImage(oldImageObj, towidth, toheight, x, y, ow, oh, newPicPath, stream, watermarkImage);
        }

        /// <summary>
        /// 按高度:宽成比例缩放图片
        /// </summary>
        /// <param name="stream">内存图片流对象</param>
        /// <param name="newPicPath">保存新图片的的实际路径</param>
        /// <param name="intHeight">按比例缩放到intHeight高度</param>
        /// <param name="watermarkImage">水印图片实际地址</param>
        /// <returns>是否成功</returns>
        public static bool ZoomImageHeight(System.IO.Stream stream, string newPicPath, int intHeight, string watermarkImage)
        {
            System.Drawing.Image oldImageObj = System.Drawing.Image.FromStream(stream);
            int x = 0;
            int y = 0;
            int ow = oldImageObj.Width;
            int oh = oldImageObj.Height;
            int toheight = oldImageObj.Height;
            int towidth = oldImageObj.Width;
            //只缩小不放大
            if (intHeight < oldImageObj.Height)
            {
                towidth = oldImageObj.Width * intHeight / oldImageObj.Height;
                toheight = intHeight;
            }
            return SaveImage(oldImageObj, towidth, toheight, x, y, ow, oh, newPicPath, stream, watermarkImage);
        }

        /// <summary>
        /// 指定高度和宽度剪裁图片
        /// </summary>
        /// <param name="stream">内存图片流对象</param>
        /// <param name="newPicPath">保存新图片的的实际路径</param>
        /// <param name="watermarkImage">水印图片实际地址</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>是否成功</returns>
        public static bool CutImage(System.IO.Stream stream, string newPicPath, int width, int height, string watermarkImage)
        {
            System.Drawing.Image oldImageObj = System.Drawing.Image.FromStream(stream);
            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = oldImageObj.Width;
            int oh = oldImageObj.Height;
            if ((double)oldImageObj.Width / (double)oldImageObj.Height > (double)towidth / (double)toheight)
            {
                oh = oldImageObj.Height;
                ow = oldImageObj.Height * towidth / toheight;
                y = 0;
                x = (oldImageObj.Width - ow) / 2;
            }
            else
            {
                ow = oldImageObj.Width;
                oh = oldImageObj.Width * height / towidth;
                x = 0;
                y = (oldImageObj.Height - oh) / 2;
            }
            return SaveImage(oldImageObj, towidth, toheight, x, y, ow, oh, newPicPath, stream, watermarkImage);
        }

        /// <summary>
        /// 不加水印指定高度和宽度剪裁图片 
        /// </summary>
        /// <param name="stream">内存图片流对象</param>
        /// <param name="newPicPath">保存新图片的的实际路径</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>是否成功</returns>
        public static bool CutImage(System.IO.Stream stream, string newPicPath, int width, int height)
        {
            System.Drawing.Image oldImageObj = System.Drawing.Image.FromStream(stream);
            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = oldImageObj.Width;
            int oh = oldImageObj.Height;
            if ((double)oldImageObj.Width / (double)oldImageObj.Height > (double)towidth / (double)toheight)
            {
                oh = oldImageObj.Height;
                ow = oldImageObj.Height * towidth / toheight;
                y = 0;
                x = (oldImageObj.Width - ow) / 2;
            }
            else
            {
                ow = oldImageObj.Width;
                oh = oldImageObj.Width * height / towidth;
                x = 0;
                y = (oldImageObj.Height - oh) / 2;
            }
            return SaveImage(oldImageObj, towidth, toheight, x, y, ow, oh, newPicPath, stream);
        }

        /// <summary>
        /// 不加水印保存图像
        /// </summary>
        /// <param name="oldImageObj"></param>
        /// <param name="towidth"></param>
        /// <param name="toheight"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="ow"></param>
        /// <param name="oh"></param>
        /// <param name="newImagePath"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static bool SaveImage(System.Drawing.Image oldImageObj, int towidth, int toheight, int x, int y, int ow, int oh, string newImagePath, System.IO.Stream stream)
        {
            bool isSaveOk = false;
            System.Drawing.Image newImageObj = new System.Drawing.Bitmap(towidth, toheight);
            //新建一个画板 
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newImageObj);
            //设置高质量插值法 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度 
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充 
            g.Clear(System.Drawing.Color.Transparent);
            //在指定位置并且按指定大小绘制原图片的指定部分 
            g.DrawImage(oldImageObj, new System.Drawing.Rectangle(0, 0, towidth, toheight), new System.Drawing.Rectangle(x, y, ow, oh), System.Drawing.GraphicsUnit.Pixel);

            //保存
            try
            {
                newImageObj.Save(newImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                isSaveOk = true;
            }
            catch (System.Exception e)
            {
                isSaveOk = false;
                throw e;
            }
            finally
            {
                try
                {
                    oldImageObj.Dispose();
                    newImageObj.Dispose();
                    g.Dispose();
                }
                catch
                {
                }
            }
            return isSaveOk;
        }

        private static bool SaveImage(System.Drawing.Image oldImageObj, int towidth, int toheight, int x, int y, int ow, int oh, string newImagePath, System.IO.Stream stream, string watermarkImage)
        {
            bool isSaveOk = false;
            System.Drawing.Image newImageObj = new System.Drawing.Bitmap(towidth, toheight);
            //新建一个画板 
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newImageObj);
            //设置高质量插值法 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度 
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充 
            g.Clear(System.Drawing.Color.Transparent);
            //在指定位置并且按指定大小绘制原图片的指定部分 
            g.DrawImage(oldImageObj, new System.Drawing.Rectangle(0, 0, towidth, toheight), new System.Drawing.Rectangle(x, y, ow, oh), System.Drawing.GraphicsUnit.Pixel);
            //处理水印
            try
            {
                //水印图片对象
                System.Drawing.Image watermarkImageObj = new System.Drawing.Bitmap(watermarkImage);
                //缩放水印图片对象(在newImageObj的底部1/2处绘制水印图片：按其缩放水印图片)
                System.Drawing.Bitmap zoomWatermarkImageObj = new Bitmap(watermarkImageObj, towidth / 3, (watermarkImageObj.Height * toheight / 3) / watermarkImageObj.Width);
                //设置水印图片为透明色
                zoomWatermarkImageObj.MakeTransparent(Color.White);
                //绘制水印图片
                g.DrawImage(zoomWatermarkImageObj, new System.Drawing.PointF(newImageObj.Width - zoomWatermarkImageObj.Width, newImageObj.Height - zoomWatermarkImageObj.Height));
                watermarkImageObj.Dispose();
                zoomWatermarkImageObj.Dispose();
            }
            catch
            {
            }

            //保存
            try
            {
                newImageObj.Save(newImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                isSaveOk = true;
            }
            catch (System.Exception e)
            {
                isSaveOk = false;
                throw e;
            }
            finally
            {
                try
                {
                    oldImageObj.Dispose();
                    newImageObj.Dispose();
                    g.Dispose();
                }
                catch
                {
                }
            }
            return isSaveOk;
        }

        #endregion

        #endregion

        #region 图片旋转函数
        /// <summary>
        /// 以逆时针为方向对图像进行旋转
        /// </summary>
        /// <param name="b">位图流</param>
        /// <param name="angle">旋转角度[0,360]</param>
        /// <returns></returns>
        public static Bitmap Rotate(Bitmap b, int angle)
        {
            angle = angle % 360;

            //弧度转换
            double radian = angle * Math.PI / 180.0;
            double cos = Math.Cos(radian);
            double sin = Math.Sin(radian);

            //原图的宽和高
            int w = b.Width;
            int h = b.Height;
            int W = (int)(Math.Max(Math.Abs(w * cos - h * sin), Math.Abs(w * cos + h * sin)));
            int H = (int)(Math.Max(Math.Abs(w * sin - h * cos), Math.Abs(w * sin + h * cos)));

            //目标位图
            Bitmap dsImage = new Bitmap(W, H);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(dsImage);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //计算偏移量
            Point Offset = new Point((W - w) / 2, (H - h) / 2);

            //构造图像显示区域：让图像的中心与窗口的中心点一致
            Rectangle rect = new Rectangle(Offset.X, Offset.Y, w, h);
            Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(360 - angle);

            //恢复图像在水平和垂直方向的平移
            g.TranslateTransform(-center.X, -center.Y);
            g.DrawImage(b, rect);

            //重至绘图的所有变换
            g.ResetTransform();

            g.Save();
            g.Dispose();
            //dsImage.Save("yuancd.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            return dsImage;
        }
        #endregion

    }
    /// <summary>
    /// 图片验证码
    /// </summary>
    public class VerifyCode
    {
        /// <summary>
        /// 验证码长度
        /// </summary>
        private int _length = 4;

        /// <summary>
        /// 字体最大尺寸
        /// </summary>
        private int _fontSize = 18;

        /// <summary>
        /// 边框，0时没有连框
        /// </summary>
        private int _border = 0;

        /// <summary>
        /// 背景色
        /// </summary>
        private Color _backgroundColor = Color.LightCyan;

        /// <summary>
        /// 验证码色
        /// </summary>
        private Color _fontColor = Color.SeaGreen;

        /// <summary>
        /// 验证码中的数字出现机率  ，越大出现的数字机率越大
        /// </summary>
        private int _rateNumber = 5;

        /// <summary>
        /// 随机生成的验证码
        /// </summary>
        private string _randomChars;

        /// <summary>
        /// 随机码的旋转角度
        /// </summary>
        private int _randomAngle = 0;

        /// <summary>
        /// 字体
        /// </summary>
        private string _fontFamily = "Arial";

        /// <summary>
        /// 噪点数量,0  时不用
        /// </summary>
        private int _chaosNumber = 0;

        /// <summary>
        /// 随机种子，公用
        /// </summary>
        private Random random = new Random();

        ///  <summary>
        ///  噪点
        ///  </summary>
        ///  <param  name="chaosNumber">噪点</param>
        public VerifyCode(int chaosNumber)
        {
            _chaosNumber = chaosNumber;
        }

        ///  <summary>
        /// 长度，噪点
        ///  </summary>
        ///  <param  name="length">长度</param>
        ///  <param  name="chaosNumber">噪点</param>
        public VerifyCode(int length, int chaosNumber)
        {
            _length = length;
            _chaosNumber = chaosNumber;
        }

        ///  <summary>
        ///  长度，噪点，数字机率
        ///  </summary>
        ///  <param  name="length">长度</param>
        ///  <param  name="chaosNumber">噪点</param>
        ///  <param  name="rate">数字机率越大，生成的随机码中数字占的比例越多</param>
        public VerifyCode(int length, int chaosNumber, int rate)
        {
            _length = length;
            _chaosNumber = chaosNumber;
            _rateNumber = rate;
        }

        #region 验证码长度(默认4个)
        /// <summary>
        /// 验证码长度(默认4个)
        /// </summary>
        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }
        #endregion

        #region  字体最大尺寸(默认18)
        /// <summary>
        /// 字体最大尺寸(默认18)
        /// </summary>
        public int FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; }
        }
        #endregion

        #region  边框（默认0  没有连框）
        /// <summary>
        /// 边框（默认0  没有连框）
        /// </summary>
        public int Border
        {
            get { return _border; }
            set { _border = value; }
        }
        #endregion

        #region  自定义背景色(默认Color.AliceBlue)
        /// <summary>
        /// 自定义背景色(默认Color.AliceBlue)
        /// </summary>
        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; }
        }
        #endregion

        #region  验证码色(默认Color.Blue)
        /// <summary>
        /// 验证码色(默认Color.Blue)
        /// </summary>
        public Color FontColor
        {
            get { return _fontColor; }
            set { _fontColor = value; }
        }
        #endregion

        #region  随机生成的验证码
        /// <summary>
        /// 随机生成的验证码
        /// </summary>
        public string RandomCode
        {
            get { return _randomChars; }
            set { _randomChars = value.ToUpper(); }
        }
        #endregion

        #region  验证码中的数字出现机率,越大出现的数字机率越大(默认10)
        /// <summary>
        /// 验证码中的数字出现机率
        /// </summary>
        public int RateNumber
        {
            get { return _rateNumber; }
            set { _rateNumber = value; }
        }
        #endregion

        #region  随机码的旋转角度(默认40度)
        /// <summary>
        /// 随机码的旋转角度(默认40度)
        /// </summary>
        public int RandomAngle
        {
            get { return _randomAngle; }
            set { _randomAngle = value; }
        }
        #endregion

        #region  字体
        /// <summary>
        /// 字体
        /// </summary>
        public string FontFamily
        {
            get { return _fontFamily; }
            set { _fontFamily = value; }
        }
        #endregion

        #region  噪点数量(默认值为2)
        /// <summary>
        /// 噪点数量(默认值为2)
        /// </summary>
        public int ChaosNumber
        {
            get { return _chaosNumber; }
            set { _chaosNumber = value; }
        }
        #endregion

        ///  <summary>
        ///  生成随机验证码
        ///  </summary>
        private void CreateCode()
        {
            //有外部输入验证码时不用产生，否则随机生成
            if (!string.IsNullOrEmpty(_randomChars))
            { return; }

            char code;
            for (int i = 0; i < _length; i++)
            {
                int rand = random.Next();
                if (rand % _rateNumber == 0)
                { code = (char)('A' + (char)(rand % 26)); }
                else
                { code = (char)('0' + (char)(rand % 10)); }
                _randomChars += code.ToString();
            }

            //Response.Cookies.Add(new HttpCookie("ValidCode", _randomChars));
        }

        ///  <summary>
        ///  背景噪点生成
        ///  </summary>
        ///  <param  name="graph"></param>
        private void CreateBackgroundChaos(Bitmap map, Graphics graph)
        {
            Pen blackPen = new Pen(Color.Azure, 0);
            for (int i = 0; i < map.Width * 2; i++)
            {
                int x = random.Next(map.Width);
                int y = random.Next(map.Height);
                graph.DrawRectangle(blackPen, x, y, 1, 1);
            }
        }

        ///  <summary>
        ///  前景色噪点
        ///  </summary>
        ///  <param  name="map"></param>
        private void CreaetForeChaos(Bitmap map)
        {
            for (int i = 0; i < map.Width * _chaosNumber; i++)
            {
                int x = random.Next(map.Width);
                int y = random.Next(map.Height);
                //map.SetPixel(x,  y,  Color.FromArgb(random.Next(300)));
                map.SetPixel(x, y, _fontColor);
            }
        }

        /// <summary>
        /// 创建随机码图片
        /// </summary>
        /// <param name="backColorCode">背景颜色代码 空为默认颜色</param>
        public void CreateImage(string backColorCode)
        {
            CreateCode();          //创建验证码

            Bitmap map = new Bitmap((int)(_randomChars.Length * 15), 24);                            //创建图片背景
            Graphics graph = Graphics.FromImage(map);
            //graph.FillRectangle(new  SolidBrush(Color.Black),  0,  0,  map.Width+1,  map.Height+1);          //填充一个有背景的矩形

            //if  (_border  >  0)  //画一个边框
            //{
            //        graph.DrawRectangle(new  Pen(Color.Black,  0),  0,  0,  map.Width  -  _border,  map.Height  -  _border);
            //}        
            //graph.SmoothingMode  =  System.Drawing.Drawing2D.SmoothingMode.AntiAlias;        //模式
            //清除画面，填充背景
            if (String.IsNullOrEmpty(backColorCode))
            {
                graph.Clear(_backgroundColor);
            }
            else
            {
                WebColorConverter ww = new WebColorConverter();
                graph.Clear((Color)ww.ConvertFromString(backColorCode));

            }

            SolidBrush brush = new SolidBrush(_fontColor);    //画笔
            Point dot = new Point(12, 12);

            //CreateBackgroundChaos(map,graph);              //背景噪点生成

            //CreaetForeChaos(map);              //前景色噪点

            //文字距中
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            //验证码旋转，防止机器识别
            char[] chars = _randomChars.ToCharArray();            //拆散字符串成单字符数组
            for (int i = 0; i < chars.Length; i++)
            {
                Font fontstyle = new Font(_fontFamily, random.Next(_fontSize - 3, _fontSize), FontStyle.Bold);            //字体样式
                float angle = random.Next(-_randomAngle, _randomAngle);            //转动的度数

                graph.TranslateTransform(dot.X, dot.Y);          //移动光标到指定位置
                graph.RotateTransform(angle);
                graph.DrawString(chars[i].ToString(), fontstyle, brush, -2, 2, format);
                graph.RotateTransform(-angle);                    //转回去
                graph.TranslateTransform(2, -dot.Y);          //移动光标到指定位置
            }


            //生成图片
            MemoryStream ms = new MemoryStream();
            map.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ContentType = "image/gif";
            HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            graph.Dispose();
            map.Dispose();
        }
    }



    /// <summary>
    /// 验证码辅助类(仿Google)
    /// </summary>
    public class VerifyCodeHelper
    {
        #region 验证码长度(默认6个验证码的长度)
        private int length = 6;
        public int Length
        {
            get { return length; }
            set { length = value; }
        }
        #endregion

        #region 验证码字体大小(为了显示扭曲效果，默认40像素)
        private int fontSize = 40;
        public int FontSize
        {
            get { return fontSize; }
            set { fontSize = value; }
        }
        #endregion

        #region 边框补(默认1像素)
        private int padding = 2;
        public int Padding
        {
            get { return padding; }
            set { padding = value; }
        }
        #endregion

        #region 是否输出燥点(默认不输出)
        private bool chaos = true;
        public bool Chaos
        {
            get { return chaos; }
            set { chaos = value; }
        }
        #endregion

        #region 输出燥点的颜色(默认灰色)
        private Color chaosColor = Color.LightGray;
        public Color ChaosColor
        {
            get { return chaosColor; }
            set { chaosColor = value; }
        }
        #endregion

        #region 自定义背景色(默认白色)
        private Color backgroundColor = Color.White;
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }
        #endregion

        #region 自定义随机颜色数组
        private Color[] colors = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
        public Color[] Colors
        {
            get { return colors; }
            set { colors = value; }
        }
        #endregion

        #region 自定义字体数组
        private string[] fonts = { "Arial", "Georgia" };
        public string[] Fonts
        {
            get { return fonts; }
            set { fonts = value; }
        }
        #endregion

        #region 自定义随机码字符串序列(使用逗号分隔)
        private string codeSerial = "0,1,2,3,4,5,6,7,8,9";
        public string CodeSerial
        {
            get { return codeSerial; }
            set { codeSerial = value; }
        }
        #endregion

        #region 产生波形滤镜效果

        private const double PI = 3.1415926535897932384626433832795;
        private const double PI2 = 6.283185307179586476925286766559;

        /// <summary>
        /// 正弦曲线Wave扭曲图片（Edit By 51aspx.com）
        /// </summary>
        /// <param name="srcBmp">图片路径</param>
        /// <param name="bXDir">如果扭曲则选择为True</param>
        /// <param name="nMultValue">波形的幅度倍数，越大扭曲的程度越高，一般为3</param>
        /// <param name="dPhase">波形的起始相位，取值区间[0-2*PI)</param>
        /// <returns></returns>
        public System.Drawing.Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            System.Drawing.Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);

            // 将位图背景填充为白色
            System.Drawing.Graphics graph = System.Drawing.Graphics.FromImage(destBmp);
            graph.FillRectangle(new SolidBrush(System.Drawing.Color.White), 0, 0, destBmp.Width, destBmp.Height);
            graph.Dispose();

            double dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;

            for (int i = 0; i < destBmp.Width; i++)
            {
                for (int j = 0; j < destBmp.Height; j++)
                {
                    double dx = 0;
                    dx = bXDir ? (PI2 * (double)j) / dBaseAxisLen : (PI2 * (double)i) / dBaseAxisLen;
                    dx += dPhase;
                    double dy = Math.Sin(dx);

                    // 取得当前点的颜色
                    int nOldX = 0, nOldY = 0;
                    nOldX = bXDir ? i + (int)(dy * dMultValue) : i;
                    nOldY = bXDir ? j : j + (int)(dy * dMultValue);

                    System.Drawing.Color color = srcBmp.GetPixel(i, j);
                    if (nOldX >= 0 && nOldX < destBmp.Width
                     && nOldY >= 0 && nOldY < destBmp.Height)
                    {
                        destBmp.SetPixel(nOldX, nOldY, color);
                    }
                }
            }

            return destBmp;
        }



        #endregion

        #region 生成校验码图片

        /// <summary>
        /// 生成校验码图片
        /// </summary>
        /// <param name="code">效验码</param>
        /// <returns>校验码图片</returns>
        public Bitmap CreateImageCode(string code)
        {
            int fSize = FontSize;
            int fWidth = fSize + Padding;

            int imageWidth = (int)(code.Length * fWidth) + 4 + Padding * 2;
            int imageHeight = fSize * 2 + Padding;

            System.Drawing.Bitmap image = new System.Drawing.Bitmap(imageWidth, imageHeight);

            Graphics g = Graphics.FromImage(image);

            g.Clear(BackgroundColor);

            Random rand = new Random();

            //给背景添加随机生成的燥点
            if (this.Chaos)
            {

                Pen pen = new Pen(ChaosColor, 0);
                int c = Length * 10;

                for (int i = 0; i < c; i++)
                {
                    int x = rand.Next(image.Width);
                    int y = rand.Next(image.Height);

                    g.DrawRectangle(pen, x, y, 1, 1);
                }
            }

            int left = 0, top = 0, top1 = 1, top2 = 1;

            int n1 = (imageHeight - FontSize - Padding * 2);
            int n2 = n1 / 4;
            top1 = n2;
            top2 = n2 * 2;

            Font f;
            Brush b;

            int cindex, findex;

            //随机字体和颜色的验证码字符
            for (int i = 0; i < code.Length; i++)
            {
                cindex = rand.Next(Colors.Length - 1);
                findex = rand.Next(Fonts.Length - 1);

                f = new System.Drawing.Font(Fonts[findex], fSize, System.Drawing.FontStyle.Bold);
                b = new System.Drawing.SolidBrush(Colors[cindex]);

                if (i % 2 == 1)
                {
                    top = top2;
                }
                else
                {
                    top = top1;
                }

                left = i * fWidth;

                g.DrawString(code.Substring(i, 1), f, b, left, top);
            }

            //画一个边框 边框颜色为Color.Gainsboro
            g.DrawRectangle(new Pen(Color.Gainsboro, 0), 0, 0, image.Width - 1, image.Height - 1);
            g.Dispose();

            //产生波形（Add By 51aspx.com）
            image = TwistImage(image, true, 8, 4);

            return image;
        }
        #endregion

        #region 验证码

        private string verifyCode = String.Empty;
        public string VerifyCode
        {
            get { return verifyCode; }
        }

        #endregion

        #region 将创建好的图片输出到页面

        /// <summary>
        /// 将创建好的图片输出到页面
        /// </summary>
        /// <param name="code">验证码</param>
        public void CreateImageOnPage()
        {
            CreateImageOnPage(verifyCode);
        }

        /// <summary>
        /// 将创建好的图片输出到页面
        /// </summary>
        /// <param name="code">验证码</param>
        public void CreateImageOnPage(string code)
        {
            if (String.IsNullOrEmpty(code)) code = verifyCode;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            Bitmap image = this.CreateImageCode(code);

            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

            HttpHelper.CurrentResponse.ClearContent();
            HttpHelper.CurrentResponse.ContentType = "image/Jpeg";
            HttpHelper.CurrentResponse.BinaryWrite(ms.GetBuffer());

            ms.Close();
            ms = null;
            image.Dispose();
            image = null;
        }
        #endregion

        #region 生成随机字符码

        /// <summary>
        /// 生成随机字符码
        /// </summary>
        /// <param name="codeLen">长度</param>
        /// <returns>随机字符码</returns>
        public string CreateVerifyCode(int codeLen)
        {
            verifyCode = "";

            if (codeLen == 0)
            {
                codeLen = Length;
            }

            string[] arr = CodeSerial.Split(',');

            int randValue = -1;

            Random rand = new Random(unchecked((int)DateTime.Now.Ticks));

            for (int i = 0; i < codeLen; i++)
            {
                randValue = rand.Next(0, arr.Length - 1);

                verifyCode += arr[randValue];
            }

            return verifyCode;
        }

        /// <summary>
        /// 生成随机字符码(默认为6位)
        /// </summary>
        /// <returns>随机字符码</returns>
        public string CreateVerifyCode()
        {
            return CreateVerifyCode(Length);
        }
        #endregion
    }


}
