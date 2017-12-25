using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace YPCommon.Photo
{
    public class GrayscalePhoto
    {
        private delegate T BitmapDelegate<T>(Bitmap workBitmap, BitmapData bitData, IntPtr ptr, int bytes, byte[] grayValues);

        /// <summary>
        /// 灰度均衡化
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Bitmap GrayBalanced(Bitmap bitmap)
        {
            if (bitmap == null) return null;
            Bitmap balanced = (Bitmap)bitmap.Clone();
            return BitmapDelegateWork(balanced, delegate (Bitmap workBitmap, BitmapData bitData, IntPtr ptr, int bytes, byte[] grayValues)
            {
                int[] tempArray = new int[256];
                int[] countPixel = new int[256];
                byte[] pixelMap = new byte[256];

                for (int i = 0; i < bytes; i++)
                {
                    byte temp = grayValues[i];
                    countPixel[temp]++;
                }

                for (int i = 0; i < 256; i++)
                {
                    if (i != 0) tempArray[i] = tempArray[i - 1] + countPixel[i];
                    else tempArray[0] = countPixel[0];
                    pixelMap[i] = (byte)(255.0 * tempArray[i] / bytes + 0.5);
                }


                for (int i = 0; i < bytes; i++)
                {
                    byte temp = grayValues[i];
                    grayValues[i] = pixelMap[temp];
                }

                Marshal.Copy(grayValues, 0, ptr, bytes);
                workBitmap.UnlockBits(bitData);
                return workBitmap;
            });
        }


        /// <summary>
        /// 灰度拉伸
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Bitmap GrayTensile(Bitmap bitmap)
        {
            if (bitmap == null) return null;
            Bitmap tensile = (Bitmap)bitmap.Clone();
            return BitmapDelegateWork(tensile, delegate (Bitmap workBitmap, BitmapData bitData, IntPtr ptr, int bytes, byte[] grayValues)
            {
                byte a = 255, b = 0;
                for (int i = 0; i < bytes; i++)
                {
                    // 最小灰度值
                    if (a > grayValues[i]) a = grayValues[i];
                    // 最大灰度值
                    if (b < grayValues[i]) b = grayValues[i];
                }

                // 计算出斜率
                double p = 255.0 / (b - a);

                // 灰度拉伸
                for (int i = 0; i < bytes; i++)
                {
                    grayValues[i] = (byte)(p * (grayValues[i] - a) + 0.5);
                }

                Marshal.Copy(grayValues, 0, ptr, bytes);
                workBitmap.UnlockBits(bitData);
                return workBitmap;
            });
        }

        /// <summary>
        /// 线性操作
        /// </summary>
        /// <param name="bitmap">操作图像（灰度图）</param>
        /// <param name="scaling">斜率</param>
        /// <param name="offset">偏移量</param>
        /// <returns></returns>
        public static Bitmap LinearOperation(Bitmap bitmap, double scaling, double offset)
        {
            if (bitmap == null) return null;
            Bitmap linear = (Bitmap)bitmap.Clone();
            return BitmapDelegateWork(linear, delegate (Bitmap workBitmap, BitmapData bitData, IntPtr ptr, int bytes, byte[] grayValues)
            {
                for (int i = 0; i < bytes; i++)
                {
                    int temp = (int)(scaling * grayValues[i] + offset + 0.5);
                    if (temp > 255) grayValues[i] = 255;
                    else if (temp < 0) grayValues[i] = 0;
                    else grayValues[i] = (byte)temp;
                }

                Marshal.Copy(grayValues, 0, ptr, bytes);
                workBitmap.UnlockBits(bitData);
                return workBitmap;
            });
        }


        /// <summary>
        /// 获取灰度直方图数据
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static List<int> GrayBitmapData(Bitmap bitmap)
        {
            if (bitmap == null) return null;
            return BitmapDelegateWork(bitmap, delegate (Bitmap workBitmap, BitmapData bitData, IntPtr ptr, int bytes, byte[] grayValues)
            {
                int maxPixel = 0;
                int[] countPixel = new int[256];
                // 灰度等级数组清零
                Array.Clear(countPixel, 0, 256);
                // 计算各个灰度级的像素个数
                for (int i = 0; i < bytes; i++)
                {
                    // 灰度级
                    byte temp = grayValues[i];

                    countPixel[temp]++;
                    if (countPixel[temp] > maxPixel)
                    {
                        maxPixel = countPixel[temp];
                    }
                }

                Marshal.Copy(grayValues, 0, ptr, bytes);
                bitmap.UnlockBits(bitData);

                List<int> data = new List<int>();
                // 获取直方图像素中灰度值
                for (int i = 0; i < 256; i++)
                {
                    // 纵坐标长度
                    int temp = (int)(100.0 * countPixel[i] / maxPixel);
                    data.Add(temp);
                }
                return data;
            });
        }

        /// <summary>
        /// 获取灰度值位图(内存处理方法)
        /// </summary>
        /// <param name="bitmap">转换位图</param>
        /// <param name="mode">处理模式</param>
        /// <returns>处理结果位图</returns>
        public static Bitmap GrayscaleBitmap(Bitmap bitmap, GrayMode mode = GrayMode.Perceived)
        {
            if (bitmap == null) return null;
            Bitmap srcBitmap = (Bitmap)bitmap.Clone();
            int width = srcBitmap.Width, height = srcBitmap.Height;
            Rectangle rect = new Rectangle(0, 0, width, height);
            //将Bitmap锁定到系统内存中,获得BitmapData
            BitmapData srcBmData = srcBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            //创建Bitmap
            Bitmap dstBitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            BitmapData dstBmData = dstBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            // get palette
            ColorPalette cp = dstBitmap.Palette;
            // init palette
            for (int i = 0; i < 256; i++)
            {
                cp.Entries[i] = Color.FromArgb(i, i, i);
            }
            dstBitmap.Palette = cp;

            //位图中第一个像素数据的地址。它也可以看成是位图中的第一个扫描行
            IntPtr srcPtr = srcBmData.Scan0, dstPtr = dstBmData.Scan0;

            //将Bitmap对象的信息存放到byte数组中
            int src_bytes = srcBmData.Stride * height, dst_bytes = dstBmData.Stride * height;
            byte[] srcValues = new byte[src_bytes], dstValues = new byte[dst_bytes];

            //复制GRB信息到byte数组
            Marshal.Copy(srcPtr, srcValues, 0, src_bytes);
            Marshal.Copy(dstPtr, dstValues, 0, dst_bytes);

            //根据Y=0.299*R+0.114*G+0.587B,Y为亮度
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    //只处理每行中图像像素数据,舍弃未用空间
                    //注意位图结构中RGB按BGR的顺序存储
                    int k = 3 * j;
                    byte r = srcValues[i * srcBmData.Stride + k + 2];
                    byte g = srcValues[i * srcBmData.Stride + k + 1];
                    byte b = srcValues[i * srcBmData.Stride + k];

                    // 灰度值提取
                    byte temp = 0;
                    switch (mode)
                    {
                        case GrayMode.AverageMethod:
                            temp = (byte)((r + g + b) / 3);
                            break;
                        case GrayMode.Perceived:
                            temp = (byte)(r * 0.299 + g * 0.587 + b * 0.114);
                            break;
                        case GrayMode.TakeGreen:
                            temp = g;
                            break;
                    }


                    dstValues[i * dstBmData.Stride + j] = temp;
                }
            }

            Marshal.Copy(dstValues, 0, dstPtr, dst_bytes);

            //解锁位图
            srcBitmap.UnlockBits(srcBmData);
            dstBitmap.UnlockBits(dstBmData);

            return dstBitmap;
        }


        /// <summary>
        /// 共用缓存图片处理委托
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        private static T BitmapDelegateWork<T>(Bitmap bitmap, BitmapDelegate<T> work)
        {
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bitData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr ptr = bitData.Scan0;
            int bytes = bitmap.Width * bitmap.Height;
            byte[] grayValues = new byte[bytes];
            Marshal.Copy(ptr, grayValues, 0, bytes);
            return work(bitmap, bitData, ptr, bytes, grayValues);
        }
    }

    /// <summary>
    /// 灰度值算法类型枚举
    /// </summary>
    public enum GrayMode
    {
        /// <summary>
        /// 平均算法
        /// </summary>
        AverageMethod,
        /// <summary>
        /// 色彩感知算法
        /// </summary>
        Perceived,
        /// <summary>
        /// 取绿色通道
        /// </summary>
        TakeGreen
    }
}
