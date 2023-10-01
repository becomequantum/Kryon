//無限次元:https://space.bilibili.com/2139404925
//本代码来自:https://github.com/becomequantum/Kryon
global using System.Drawing.Imaging;
using System.Numerics;

namespace 图像处理;

public partial class 图像 {
    public const int 透 = 3, 红 = 2, 绿 = 1, 蓝 = 0; //Bitmap位图数据中,蓝色Blue的值存在低地址上.0为完全透明,255为不透明
   
    static uint 随机种子 = 7;
    static public uint 总点数 = 0; //用于统计图像中大于某个值的点数

    public delegate void RefAction<T1, T2, T3>(ref T1 arg1, ref T2 arg2, ref T3 arg3);//用于像素点RGB值处理的方法委托
    static uint 随机xorshift32() {
        uint x = 随机种子;
        x ^= x << 13;
        x ^= x >> 17;
        x ^= x << 5;
        随机种子 = x;
        return x;
    }//这个不知道啥随机数算法比自带的快,用于刷随机雪花点图
    static public bool 点在框内(Point 点坐标, Bitmap 位图, int X缩减 = 0, int Y缩减 = 0) {
        return 点在框内(点坐标, new Rectangle(0, 0, 位图.Width, 位图.Height), X缩减, Y缩减);
    }
    static public bool 点在框内(Point 点坐标, Rectangle 框, int X缩减 = 0, int Y缩减 = 0) {
        return 点在框内(点坐标.X, 点坐标.Y, new Rectangle(框.X + X缩减, 框.Y + Y缩减, 框.Width - X缩减 * 2, 框.Height - Y缩减 * 2));
    }
    static public bool 点在框内(Vector2 点, Rectangle 框) {
        return 点在框内(点.X, 点.Y, 框);
    }
    static public bool 点在框内(float x, float y, Rectangle 框) {
        return x >= 框.X && x < 框.X + 框.Width && y >= 框.Y && y < 框.Y + 框.Height;
    }
    internal static void 获取HSI_RGB谱 (Bitmap 位图, int[] R谱, int[] G谱, int[] B谱, int[] H谱, int[] S谱, int[] L谱, int[,] HS谱, bool HSL = true) {
        Rectangle 选图框 = new Rectangle(0, 0, 位图.Width, 位图.Height);
        BitmapData 位图数据 = 位图.LockBits(选图框, ImageLockMode.ReadWrite, 位图.PixelFormat);
        int H = 0, S = 0, L = 0;
        int 字节数 = 3;
        if (位图.PixelFormat == PixelFormat.Format32bppArgb) 字节数 = 4;
        unsafe {
            byte* 位针 = (byte*)(位图数据.Scan0);
            for (int y = 0; y < 位图数据.Height; y++, 位针 += 位图数据.Stride - 位图数据.Width * 字节数) //这里必须用"位图数据.x",不小心写成"位图.x"就会导致程序莫名变慢很多
                for (int x = 0; x < 位图数据.Width; x++, 位针 += 字节数) {
                    R谱[位针[红]]++; G谱[位针[绿]]++; B谱[位针[蓝]]++;
                    if (HSL) {
                        RGB转HSL(位针[红], 位针[绿], 位针[蓝], ref H, ref S, ref L);
                        H谱[H]++; S谱[S]++; L谱[L]++;HS谱[H, S]++;
                    }
                }
        }      
        位图.UnlockBits(位图数据);
    }

    static public void 直方图均衡化(Bitmap 位图, int[] R谱, int[] G谱, int[] B谱) {
        int[] R累计分布 = new int[256], G累计分布 = new int[256], B累计分布 = new int[256];
        byte[] R映射表 = new byte[256], G映射表 = new byte[256], B映射表 = new byte[256];
        int 总像素点数 = 位图.Width * 位图.Height;
        R累计分布[0] = R谱[0]; G累计分布[0] = G谱[0]; B累计分布[0] = B谱[0];
        for (int i = 1; i < R谱.Length; i++) {
            R累计分布[i] = R累计分布[i - 1] + R谱[i];
            G累计分布[i] = G累计分布[i - 1] + G谱[i];
            B累计分布[i] = B累计分布[i - 1] + B谱[i];
            R映射表[i] = (byte)(255.0 * R累计分布[i] / (总像素点数) + 0.5);
            G映射表[i] = (byte)(255.0 * G累计分布[i] / (总像素点数) + 0.5);
            B映射表[i] = (byte)(255.0 * B累计分布[i] / (总像素点数) + 0.5);
        }
        BitmapData 位图数据 = 位图.LockBits(new Rectangle(0, 0, 位图.Width, 位图.Height), ImageLockMode.ReadWrite, 位图.PixelFormat);
        int 字节数 = 3;
        if (位图.PixelFormat == PixelFormat.Format32bppArgb) 字节数 = 4;
        unsafe {
            byte* 位图指针 = (byte*)(位图数据.Scan0);
            for (int 列 = 0; 列 < 位图数据.Height; 列++, 位图指针 += 位图数据.Stride - 位图数据.Width * 字节数)  //这里必须用"位图数据.Height",不小心写成"位图.Height"就会导致程序莫名变慢很多.
                for (int 行 = 0; 行 < 位图数据.Width; 行++, 位图指针 += 字节数)
                    (位图指针[红],位图指针[绿], 位图指针[蓝]) = (R映射表[位图指针[红]], G映射表[位图指针[绿]], B映射表[位图指针[蓝]]);

        }
        位图.UnlockBits(位图数据);
    }

    static public void RGB转HSL(int color_R, int color_G, int color_B, ref int HH, ref int SS, ref int LL) {
        double R = color_R / 255.00;
        double G = color_G / 255.00;
        double B = color_B / 255.00;
        double max, min, diff, r_dist, g_dist, b_dist;
        double h, s, l;
        int[] hsl = new int[3];
        max = Math.Max(Math.Max(R, G), B);
        min = Math.Min(Math.Min(R, G), B);
        diff = max - min;
        l = (max + min) / 2.00 * 100.00;
        if (diff == 0) {
            h = 0;
            s = 0;
        }
        else {
            if (l < 50) {
                s = diff / (max + min) * 100.00;
            }
            else {
                s = diff / (2 - max - min) * 100.00;
            }
            r_dist = (max - R) / diff;
            g_dist = (max - G) / diff;
            b_dist = (max - B) / diff;
            h = b_dist - g_dist;
            if (R == max) {
                h = b_dist - g_dist;
            }
            else if (G == max) {
                h = 2 + r_dist - b_dist;
            }
            else if (B == max) {
                h = 4 + g_dist - r_dist;
            }
            h *= 60;
            if (h < 0) {
                h += 360;
            }
            if (h >= 360) {
                h -= 360;
            }

        }
        HH = hsl[0] = (int)(h * 2.0 / 3.0 + 0.5);
        SS = hsl[1] = (int)(s * 2.4 + 0.5);
        LL = hsl[2] = (int)(l * 2.4 + 0.5);

    }//网上抄来的代码,转换效果和画图里一样

    static public void HSL转RGB(int HH, int SS, int II, ref int RR, ref int GG, ref int BB) {
        double v;
        double r, g, b;
        double h = HH / 240.0, s = SS / 240.0, l = II / 240.0;
        r = l; g = l; b = l;
        v = (l <= 0.5) ? (l * (1.0 + s)) : (l + s - l * s);
        if (v > 0) {
            double m;
            double sv;
            int sextant;
            double fract, vsf, mid1, mid2;
            m = l + l - v;
            sv = (v - m) / v;
            h *= 6.0;
            sextant = (int)h;
            fract = h - sextant;
            vsf = v * sv * fract;
            mid1 = m + vsf;
            mid2 = v - vsf;
            switch (sextant) {
                case 0:
                    r = v;
                    g = mid1;
                    b = m;
                    break;
                case 1:
                    r = mid2;
                    g = v;
                    b = m;
                    break;
                case 2:
                    r = m;
                    g = v;
                    b = mid1;
                    break;
                case 3:
                    r = m;
                    g = mid2;
                    b = v;
                    break;
                case 4:
                    r = mid1;
                    g = m;
                    b = v;
                    break;
                case 5:
                    r = v;
                    g = m;
                    b = mid2;
                    break;
            }
        }
        //Color rgb = new Color();

        //rgb = Color.FromArgb((int)(r * 255.0f + 0.5), (int)(g * 255.0f + 0.5), (int)(b * 255.0f + 0.5));
        RR = (int)(r * 255.0f + 0.5);
        GG = (int)(g * 255.0f + 0.5);
        BB = (int)(b * 255.0f + 0.5);
        //return rgb;
    }
    #region 下面"多功能处理"中像素点RGB值处理方法, 是用反射+委托调用的
    static public void 随机刷(ref byte Red, ref byte Green, ref byte Blue) {
        uint random = 随机xorshift32();
        Red = (byte)random;
        Green = (byte)(random >> 8);
        Blue = (byte)(random >> 16);
    }
        
    static public void 点运算(ref byte Red, ref byte Green, ref byte Blue) {
        (int R, int G, int B) = ((int)(Red * 参数.W + 参数.b + 0.5), (int)(Green * 参数.W + 参数.b + 0.5), (int)(Blue * 参数.W + 参数.b + 0.5));
        (Red, Green, Blue) = ((byte)((R > 255) ? 255 : ((R < 0) ? 0 : R)), (byte)((G > 255) ? 255 : ((G < 0) ? 0 : G)), (byte)((B > 255) ? 255 : ((B < 0) ? 0 : B)));
    }//每个点的R,G,B值分别乘以W,再加上b
        
    static public void 转灰度(ref byte Red, ref byte Green, ref byte Blue) {
        (int R, int G,int B) = ((int)(Red * 参数.红比例 + 0.5), (int)(Green * 参数.绿比例 + 0.5), (int)(Blue * 参数.蓝比例 + 0.5));
        int Gray = R + G + B;
        Red = Green = Blue = (byte)(Gray > 255 ? 255 : Gray);
    }

    static public void 增益(ref byte Red, ref byte Green, ref byte Blue) {
        (int R, int G, int B) = ((int)(Red * 参数.红比例 + 0.5), (int)(Green * 参数.绿比例 + 0.5), (int)(Blue * 参数.蓝比例 + 0.5));
        (Red, Green, Blue) = ((byte)(R > 255 ? 255 : R), (byte)(G > 255 ? 255 : G), (byte)(B > 255 ? 255 : B));
    }

    static public void R转H(ref byte Red, ref byte Green, ref byte Blue) {
        int R = 0, G = 0, B = 0;
        RGB转HSL(Red, Green, Blue, ref R, ref G, ref B);
        (Red, Green, Blue) = ((byte)R, (byte)G, (byte)B);
    }

    static public void H转R(ref byte Red, ref byte Green, ref byte Blue) {
        int R = 0, G = 0, B = 0;
        HSL转RGB(Red, Green, Blue, ref R, ref G, ref B);
        (Red, Green, Blue) = ((byte)R, (byte)G, (byte)B);
    }

    static public void 计数(ref byte Red, ref byte Green, ref byte Blue) {
        if (Green > 参数.HSL下限.B) 总点数++;
    }
    #endregion

    static public void 多功能处理(Bitmap 位图, RefAction<byte, byte, byte> 处理方法) {
        BitmapData 位图数据 = 位图.LockBits(new Rectangle(0, 0, 位图.Width, 位图.Height), ImageLockMode.ReadWrite, 位图.PixelFormat);
        byte 字节数 = 3;
        if (位图.PixelFormat == PixelFormat.Format32bppArgb) 字节数 = 4;//为了适应24位图和32位图这两种情况
        Random 随机 = new();
        随机种子 = (uint)随机.Next(int.MaxValue);
        unsafe {
            byte* 位针 = (byte*)(位图数据.Scan0);
            for (int y = 0; y < 位图数据.Height; y++, 位针 += 位图数据.Stride - 位图数据.Width * 字节数) 
                for (int x = 0; x < 位图数据.Width; x++, 位针 += 字节数)
                    处理方法(ref 位针[红], ref 位针[绿], ref 位针[蓝]); //把代码展开写在这里速度最快,调用函数会慢一点,用委托调用又会更慢一点.这样写代码简洁了,但速度慢了些
        }
        位图.UnlockBits(位图数据);
    }
    static public void 位图相减(Bitmap 位图1, Bitmap 位图2, Bitmap 结果位图) {
        Rectangle 选图框 = new Rectangle(0, 0, 位图1.Width, 位图1.Height);
        BitmapData 位图1数据 = 位图1.LockBits(选图框, ImageLockMode.ReadWrite, 位图1.PixelFormat);
        BitmapData 位图2数据 = 位图2.LockBits(选图框, ImageLockMode.ReadWrite, 位图2.PixelFormat);
        BitmapData 结果数据 = 结果位图.LockBits(选图框, ImageLockMode.ReadWrite, 结果位图.PixelFormat);
        byte 位图1字节数 = 3, 位图2字节数 = 3;
        if (位图1.PixelFormat == PixelFormat.Format32bppArgb) 位图1字节数 = 4;
        if (位图2.PixelFormat == PixelFormat.Format32bppArgb) 位图2字节数 = 4;
        unsafe {
            byte* 位图1指针 = (byte*)(位图1数据.Scan0);
            byte* 位图2指针 = (byte*)(位图2数据.Scan0);
            byte* 结果指针 = (byte*)(结果数据.Scan0);
            for (int y = 0; y < 位图1数据.Height; y++) { 
                for (int x = 0; x < 位图1数据.Width; x++) {
                    结果指针[红] = (byte)Math.Abs(位图1指针[红] - 位图2指针[红]);
                    结果指针[绿] = (byte)Math.Abs(位图1指针[绿] - 位图2指针[绿]);
                    结果指针[蓝] = (byte)Math.Abs(位图1指针[蓝] - 位图2指针[蓝]);
                    位图1指针 += 位图1字节数;
                    位图2指针 += 位图2字节数;
                    结果指针 += 3;
                }
                位图1指针 += 位图1数据.Stride - 位图1数据.Width * 位图1字节数;
                位图2指针 += 位图2数据.Stride - 位图2数据.Width * 位图2字节数;
                结果指针 += 结果数据.Stride - 结果数据.Width * 3;
            }
        }
        位图1.UnlockBits(位图1数据);
        位图2.UnlockBits(位图2数据);
        结果位图.UnlockBits(结果数据);
    }//用于比较两幅图之间的差异, 相减之后再放大就能看出差异在哪.

    static public Bitmap Nx位图(Bitmap 原图, int N, bool 画格子否 = false, Bitmap Nx位图 = null) {
        if (原图 != null && N > 1) {
            if (Nx位图 == null)
                Nx位图 = new Bitmap(原图.Width * N, 原图.Height * N, PixelFormat.Format24bppRgb);
            BitmapData 原图数据 = 原图.LockBits(new Rectangle(0, 0, 原图.Width, 原图.Height), ImageLockMode.ReadWrite, 原图.PixelFormat);
            BitmapData Nx位图数据 = Nx位图.LockBits(new Rectangle(0, 0, Nx位图.Width, Nx位图.Height), ImageLockMode.ReadWrite, Nx位图.PixelFormat);

            unsafe {
                byte* 原图指针 = (byte*)(原图数据.Scan0);
                byte* Nx位图指针 = (byte*)(Nx位图数据.Scan0);
                byte R = 0, G = 0, B = 0;
                for (int 列 = 0; 列 < Nx位图数据.Height; 列++, Nx位图指针 += Nx位图数据.Stride - Nx位图数据.Width * 3) {
                    for (int 行 = 0; 行 < Nx位图数据.Width; 行++, Nx位图指针 += 3) {
                        if (行 % N == 0) {
                            int index = (行 / N) * 3;
                            R = 原图指针[index + 红]; G = 原图指针[index + 绿]; B = 原图指针[index + 蓝];
                        }
                        Nx位图指针[红] = R;
                        Nx位图指针[绿] = G;
                        Nx位图指针[蓝] = B;

                        if (画格子否 && (列 % N == N - 1 || 行 % N == N - 1)) {
                            Nx位图指针[红] = Nx位图指针[蓝] = 100;
                            Nx位图指针[绿] = 122;
                        }
                    }
                    if (列 % N == N - 1)
                        原图指针 += 原图数据.Stride;
                }
            }
            原图.UnlockBits(原图数据);
            Nx位图.UnlockBits(Nx位图数据);
            return Nx位图;
        }
        else {
            return 原图;
        }
    }
    static public void 变RAW(Bitmap 位图) {
        BitmapData 位图数据 = 位图.LockBits(new Rectangle(0, 0, 位图.Width, 位图.Height), ImageLockMode.ReadWrite, 位图.PixelFormat);
        byte 字节数 = 3;
        if (位图.PixelFormat == PixelFormat.Format32bppArgb) 字节数 = 4;//为了适应24位图和32位图这两种情况
        unsafe {
            byte* 位针 = (byte*)(位图数据.Scan0);
            for (int 列 = 0; 列 < 位图数据.Height; 列++, 位针 += 位图数据.Stride - 位图数据.Width * 字节数)
                for (int 行 = 0; 行 < 位图数据.Width; 行++, 位针 += 字节数)
                    if (列 % 2 == 0) {               //GRGR
                        if (行 % 2 == 0)             //BGBG
                            位针[红] = 位针[蓝] = 0;
                        else
                            位针[绿] = 位针[蓝] = 0;
                    }
                    else {
                        if (行 % 2 == 0)
                            位针[红] = 位针[绿] = 0;
                        else
                            位针[红] = 位针[蓝] = 0;
                    }
        }
        位图.UnlockBits(位图数据);
    }//为了展示插值前的Raw数据看起来是啥样的

    static public void Sobel(Bitmap 位图, double[,] 梯度数组 = null, double[,] 角度数组 = null) {
        BitmapData 位图数据 = 位图.LockBits(new Rectangle(0, 0, 位图.Width, 位图.Height), ImageLockMode.ReadWrite, 位图.PixelFormat);
        byte 字节数 = 3;
        if (位图.PixelFormat == PixelFormat.Format32bppArgb) 字节数 = 4;
        bool 标回原图 = true;
        if (梯度数组 == null || 角度数组 == null) {
            梯度数组 = new double[位图.Height, 位图.Width];
            角度数组 = new double[位图.Height, 位图.Width];
        }
        else
            标回原图 = false;
        double Max = 0;
        unsafe {
            byte* 位图指针 = (byte*)(位图数据.Scan0);
            for (int y = 1; y < 位图数据.Height - 1; y++)
                for (int x = 1; x < 位图数据.Width - 1; x++) {
                    byte 左上 = 位图指针[(y - 1) * 位图数据.Stride + (x - 1) * 字节数];
                    byte 上 = 位图指针[(y - 1) * 位图数据.Stride + x * 字节数];
                    byte 右上 = 位图指针[(y - 1) * 位图数据.Stride + (x + 1) * 字节数];
                    byte 左 = 位图指针[y * 位图数据.Stride + (x - 1) * 字节数];
                    byte 右 = 位图指针[y * 位图数据.Stride + (x + 1) * 字节数];
                    byte 左下 = 位图指针[(y + 1) * 位图数据.Stride + (x - 1) * 字节数];
                    byte 下 = 位图指针[(y + 1) * 位图数据.Stride + x * 字节数];
                    byte 右下 = 位图指针[(y + 1) * 位图数据.Stride + (x + 1) * 字节数];

                    int Gx = (右上 - 左上) + (右 - 左) * 2 + (右下 - 左下);
                    int Gy = (右下 - 右上) + (下 - 上) * 2 + (左下 - 左上);
                    double G = Math.Sqrt(Gx * Gx + Gy * Gy);
                    if (G > Max) Max = G;                     //     -90
                    梯度数组[y, x] = G;                       // 180<-  ->0度 
                    角度数组[y, x] = Math.Atan2(Gy, Gx);      //      90
                }
            if (标回原图)//被'Canny边缘检测'调用时不会标回原图
                for (int y = 0; y < 位图数据.Height; y++, 位图指针 += 位图数据.Stride - 位图数据.Width * 字节数)
                    for (int x = 0; x < 位图数据.Width; x++, 位图指针 += 字节数)
                        位图指针[红] = 位图指针[绿] = 位图指针[蓝] = (byte)(Math.Pow(梯度数组[y, x] / Max, 0.75) * 255);

            位图.UnlockBits(位图数据);
        }
        
    }
    static public void Canny(Bitmap 位图) {
        int Width = 位图.Width;
        int Height = 位图.Height;
        double[,] 梯度数组 = new double[Height, Width];
        double[,] 角度数组 = new double[Height, Width];
        double[,] 抑制后梯度数组 = new double[Height, Width];
        Sobel(位图, 梯度数组, 角度数组);
        double Max = 0;
        for (int y = 1; y < Height - 1; y++)//非极大值抑制
            for (int x = 1; x < Width - 1; x++) {
                #region 沿梯度方向留最大值抑制
                //double 角度 = 角度数组[y, x] * 180 / Math.PI;
                //if ((角度 >= -22.5 && 角度 <= 22.5) || (角度 >= 157.5 && 角度 <= 180) || (角度 <= -157.5 && 角度 >= -180)) {//左右
                //    if (梯度数组[y, x] >= 梯度数组[y, x + 1] && 梯度数组[y, x] >= 梯度数组[y, x - 1]) {
                //        抑制后梯度数组[y, x] = 梯度数组[y, x];
                //        if (抑制后梯度数组[y, x] > Max) Max = 抑制后梯度数组[y, x];
                //    }
                //}
                //else if ((角度 >= 22.5) && (角度 < 67.5) || (角度 <= -112.5) && (角度 > -157.5)) {//左上右下
                //    if (梯度数组[y, x] >= 梯度数组[y - 1, x - 1] && 梯度数组[y, x] >= 梯度数组[y + 1, x + 1]) {
                //        抑制后梯度数组[y, x] = 梯度数组[y, x];
                //        if (抑制后梯度数组[y, x] > Max) Max = 抑制后梯度数组[y, x];
                //    }
                //}
                //else if ((角度 >= 67.5) && (角度 < 112.5) || (角度 <= -67.5) && (角度 > -112.5)) {//上下
                //    if (梯度数组[y, x] >= 梯度数组[y + 1, x] && 梯度数组[y, x] >= 梯度数组[y - 1, x]) {
                //        抑制后梯度数组[y, x] = 梯度数组[y, x];
                //        if (抑制后梯度数组[y, x] > Max) Max = 抑制后梯度数组[y, x];
                //    }
                //}
                //else if ((角度 >= 112.5) && (角度 < 157.5) || (角度 <= -22.5) && (角度 > -67.5)) {//右上左下
                //    if (梯度数组[y, x] >= 梯度数组[y - 1, x + 1] && 梯度数组[y, x] >= 梯度数组[y + 1, x - 1]) {
                //        抑制后梯度数组[y, x] = 梯度数组[y, x];
                //        if (抑制后梯度数组[y, x] > Max) Max = 抑制后梯度数组[y, x];
                //    }
                //}
                #endregion

                #region 统计法抑制
                int 计数 = 0;
                foreach (Point 邻域 in 顺八邻域[0]) {//统计该点的值比多少个领域点大
                    if (梯度数组[y, x] >= 梯度数组[y + 邻域.X, x + 邻域.Y])
                        计数++;
                }
                if (计数 >= 6) { //有比周围的六个点都大就是局部极大值
                    抑制后梯度数组[y, x] = 梯度数组[y, x];
                    if (抑制后梯度数组[y, x] > Max) Max = 抑制后梯度数组[y, x];
                }
                #endregion
            }
        BitmapData 位图数据 = 位图.LockBits(new Rectangle(0, 0, 位图.Width, 位图.Height), ImageLockMode.ReadWrite, 位图.PixelFormat);
        byte 字节数 = 3;
        if (位图.PixelFormat == PixelFormat.Format32bppArgb) 字节数 = 4;
        unsafe {
            byte* 位图指针 = (byte*)(位图数据.Scan0);
            for (int y = 0; y < 位图数据.Height; y++, 位图指针 += 位图数据.Stride - 位图数据.Width * 字节数)
                for (int x = 0; x < 位图数据.Width; x++, 位图指针 += 字节数)
                    位图指针[红] = 位图指针[绿] = 位图指针[蓝] = (byte)(Math.Pow(抑制后梯度数组[y, x] / Max, 0.75) * 255);

        }
        位图.UnlockBits(位图数据);
    }

    static public void 标背景或二值化(Bitmap 位图, bool 二值化) {
        byte 字节数 = 3;
        if (位图.PixelFormat == PixelFormat.Format32bppArgb) 字节数 = 4;
        BitmapData 位图数据 = 位图.LockBits(new Rectangle(0, 0, 位图.Width, 位图.Height), ImageLockMode.ReadWrite, 位图.PixelFormat);
        unsafe {
            byte* 位针 = (byte*)(位图数据.Scan0);
            for (int y = 0; y < 位图数据.Height; y++, 位针 += 位图数据.Stride - 位图数据.Width * 字节数)
                for (int x = 0; x < 位图数据.Width; x++, 位针 += 字节数) {
                    int H = 0, S = 0, L = 0;
                    RGB转HSL(位针[红], 位针[绿], 位针[蓝], ref H, ref S, ref L);
                    if ((H <= 参数.HSL上限.R && H >= 参数.HSL下限.R || !参数.H使能) &&
                        (S <= 参数.HSL上限.G && S >= 参数.HSL下限.G || !参数.S使能) &&
                        (L <= 参数.HSL上限.B && L >= 参数.HSL下限.B || !参数.L使能)) {
                        if (二值化)
                            位针[红] = 位针[绿] = 位针[蓝] = 0; //背景,黑
                        else
                            (位针[红], 位针[绿], 位针[蓝]) = (参数.颜色.R, 参数.颜色.G, 参数.颜色.B);
                    }
                    else {
                        if (二值化)
                            位针[红] = 位针[绿] = 位针[蓝] = 255; //前景,白        
                    }
                }
        }
        位图.UnlockBits(位图数据);
    }

    static public bool 尺寸相等(Bitmap 位图1, Bitmap 位图2) {
        return 位图1.Width == 位图2.Width && 位图1.Height == 位图2.Height;
    }

    static public void 读Yolo标签(Bitmap 位图, string 标签文件) {
        using StreamReader 读标签 = new(标签文件);
        Graphics g = Graphics.FromImage(位图);
        string 一行;
        while ((一行 = 读标签.ReadLine()) != null) {
            解析Yolo标签(一行, 位图, out Rectangle 边框, out string 标签);
            g.DrawRectangle(new Pen(Color.Red), 边框);
            g.DrawString(标签, 绘图.TNR字体(14), new SolidBrush(Color.Red), new Point(边框.X, 边框.Y - 20));
        }
        g.Dispose();
    }

    

    static private void 解析Yolo标签(string 一行标签, Bitmap 位图,out Rectangle 边框, out string 标签) {
        string[] 信息 = 一行标签.Split(" ");
        标签 = 信息[0];
        边框 = Rectangle.Empty;
        try {
            float 中心X = Convert.ToSingle(信息[1]);
            float 中心Y = Convert.ToSingle(信息[2]);
            float 宽 = Convert.ToSingle(信息[3]);
            float 高 = Convert.ToSingle(信息[4]);
            float 左上X = 中心X - 宽 / 2;
            float 左上Y = 中心Y - 高 / 2;
            边框.X = (int)(左上X * 位图.Width);
            边框.Y = (int)(左上Y * 位图.Height);
            边框.Width = (int)(宽 * 位图.Width + 1.5);
            边框.Height = (int)(高 * 位图.Height + 1.5);
        }
        catch (Exception) {
            MessageBox.Show("获取标签信息失败!");
            return;
        }
    }
}

public class 参数 {
    static public PixelFormat 像素格式 = PixelFormat.Format24bppRgb;
    static public int 宽, 高, 笔粗细 = 1, 算子直径 = 3, 胀蚀阈值;
    static public Color 颜色;
    static public double 红比例, 绿比例, 蓝比例;
    static public double W, b;
    static public Color HSL上限 = Color.FromArgb(160, 240, 240);
    static public Color HSL下限 = Color.FromArgb(140, 0, 0);
    static public bool H使能 = true, S使能 = false, L使能 = false;
    static public bool 显示连通域标记过程 = false;
    static public (bool 黑, bool ALSD) 显示 = (false, false);
    
}

public struct 线段信息 {
    public Vector2 起, 终;
    public 线段信息(Vector2 起点, Vector2 终点) {
        起 = 起点;
        终 = 终点;
    }
}//存放线段识别的结果
