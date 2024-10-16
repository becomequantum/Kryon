using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 图像处理;
public class 验证码{
    static string 码字符 = "abcdefghijkmnpqrstyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";//没有大写的I,小写的l,x,o,w,u,v
    static readonly Random 随机 = new();
    static string[] 字体名 = new string[] { "Times New Roman", "华文楷体","微软雅黑"};
    const int 最大字号 = 130, 最小字号 = 50, 最大偏角 = 60;//随机左右偏角度限制,偏太大人识别也会更费劲一些

    static public Bitmap 画空旋噪验证码(string 码) {
        int X = 20, Y = 10;
        Bitmap 码图 = new ((int)(最大字号 * 码.Length * 1.1), (int)(最大字号 * 1.5 + Y * 2), PixelFormat.Format24bppRgb);
        int[] 字号 = 随字号(码.Length);
        using Graphics g = Graphics.FromImage(码图);
        for (int i = 0; i < 码.Length; i++) {
            int 偏角 = 随机.Next(-最大偏角, 最大偏角 + 1);
            double 偏弧 = Math.Abs(偏角) / Math.PI / 2;
            string 待画字符 = 码.Substring(i, 1);
            Font 字体 = 随个字体(字号[i]);
            SizeF 字尺寸 = g.MeasureString(待画字符, 字体);
            绘图.画旋字(g, 待画字符, 偏角, 字体, Color.White, X, Y);    
            X += (int)字尺寸.Width;//字宽;
        }
        图像.灰度二值化(码图);
        #region 获取掩码，用于加干扰线条，不加这三行代码就不需要
        Bitmap 掩图 = 码图.Clone(new Rectangle(0, 0, 码图.Width, 码图.Height), PixelFormat.Format24bppRgb);
        参数.算子直径 = 3; 参数.胀蚀阈值 = 1;
        byte[,] 掩码 = 图像.阈胀蚀(掩图, false);//做的事情就是膨胀，如果调用Opencv会更快些
        #endregion
        图像.取边缘(码图);  //把字变空心，此后验证码是黑底白色空心字
        加噪点(码图, 参数.内噪比, 参数.外噪比); //内外噪点百分比都设为0时没加任何噪点. 没有外部背景噪声的情况下,感觉内噪到了70%还能比较轻松的认出来,再高就难了. 外噪10%就感觉有很多噪点,此时内噪必需降到10%才能凸显出来   
        //在功能标签页.cs里有处理这两个比值参数,处理后的结果是图片变成了白底黑字,白底黑字似乎比黑底白字更容易识别一些
        加随机线条(g, Color.Black, 参数.随机线条数, 码图.Width, 码图.Height, 掩码);//随机线条不加我看也能抗住神经网络
        return 码图;
    }

    static void 加随机线条(Graphics g, Color 颜色, int N, int 宽, int 高, byte[,] 掩码) {
        int n = 0;
        while (n < N){
            Point 起点 = new(随机.Next(0, 宽), 随机.Next(0, 高));//终点 = new(随机.Next(0, 宽), 随机.Next(0, 高));
            int d = 高 / 12;
            Point 终点 = new(起点.X + 随机.Next(-d, d), 起点.Y + 随机.Next(-d, d));
            if (终点.X < 0 || 终点.X >= 宽 || 终点.Y < 0 || 终点.Y >= 高) continue;
            if (掩码[起点.Y, 起点.X] == 1 || 掩码[终点.Y, 终点.X] == 1) continue;//为了不让随机线条碰到字的边缘影响人的识别。
            g.DrawLine(new Pen(颜色), 起点, 终点);
            n++;
        }
    }
    static void 加噪点(Bitmap 位图,int 内部噪点百分比, int 外部噪点百分比) {
        BitmapData 位图数据 = 位图.LockBits(new Rectangle(0, 0, 位图.Width, 位图.Height), ImageLockMode.ReadWrite, 位图.PixelFormat);
        unsafe {
            byte* 位针 = (byte*)(位图数据.Scan0);
            for (int 列 = 0; 列 < 位图数据.Height; 列++, 位针 += 位图数据.Stride - 位图数据.Width * 3)
                for (int 行 = 0; 行 < 位图数据.Width; 行++, 位针 += 3)
                    位针[图像.红] = 位针[图像.绿] = 位针[图像.蓝] = (位针[图像.绿] == 0) ? ((随机.Next(0, 100) < 外部噪点百分比) ? (byte)255 : (byte)0) : ((随机.Next(0, 100) < 内部噪点百分比) ? (byte)0 : (byte)255);
        }
        位图.UnlockBits(位图数据);
    }

    

    static public int[] 随字号(int N) {
        int[] 字号 = new int[N];
        for (int i = 0; i < N; i++) 字号[i] = 随机.Next(最小字号, 最大字号);
        if (N > 2) {
            int 范围 = (最大字号 - 最小字号) / 4;
            int[] I = 随N(2, N);
            字号[I[0]] = 随机.Next(最大字号 - 范围, 最大字号);
            字号[I[1]] = 随机.Next(最小字号, 最小字号 + 范围);
        }//随机抓两个出来,一个设为较大字号,一个设为较小字号,避免随机的都太大或太小
        return 字号;
    }
    static public string 随N个字符(int N) {
        if (N >= 码字符.Length) return null;
        int[] 查重 = new int[码字符.Length];
        StringBuilder 字符 = new(N);
        int[] I = 随N(N, 码字符.Length);
        for (int i = 0; i < N; i++) 字符.Append(码字符[I[i]]);
        return 字符.ToString();
    }

    static public int[] 随N(int N, int Length) {
        if(N > Length) N = Length;
        int[] 索引 = new int[N], 查重 = new int[Length];
        int n = 0;
        while (n < N) {
            int i = 随机.Next(Length);
            if (查重[i] == 0) {
                索引[n] = i;
                查重[i] = 1;
                n++;
            }
        }
        return 索引;
    }//从0到Length(不含)个索引中随机选N个不重复的

    static Font 随个字体(int 字号) {
        return new Font(new FontFamily(字体名[随机.Next(字体名.Length)]), 字号);
    }
}

