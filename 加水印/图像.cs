using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 加水印 {
    class 图像 {
        private const int 透 = 3, 红 = 2, 绿 = 1, 蓝 = 0; //Bitmap位图数据中,蓝色Blue的值存在低地址上
        static uint 种子 = 1111;
        
        public static void 原图刷新图24(Bitmap 原图, Bitmap 新图) {
            if (原图.Width != 新图.Width || 原图.Height != 新图.Height) return;
            Rectangle 选图框 = new Rectangle(0, 0, 原图.Width, 原图.Height);
            BitmapData 原图数据 = 原图.LockBits(选图框, ImageLockMode.ReadWrite, 原图.PixelFormat);
            BitmapData 新图数据 = 新图.LockBits(选图框, ImageLockMode.ReadWrite, 新图.PixelFormat);
            unsafe {
                byte* 原图指针 = (byte*)(原图数据.Scan0);
                byte* 新图指针 = (byte*)(新图数据.Scan0);
                for (int 列 = 0; 列 < 原图数据.Height; 列++) { //这里必须用"位图数据.Height",不小心写成"位图.Height"就会导致程序莫名变慢很多.
                    for (int 行 = 0; 行 < 原图数据.Width; 行++, 原图指针 += 3, 新图指针 += 3) {
                        新图指针[红] = 原图指针[红];
                        新图指针[绿] = 原图指针[绿];
                        新图指针[蓝] = 原图指针[蓝];

                    }
                    原图指针 += 原图数据.Stride - 原图数据.Width * 3;
                    新图指针 += 新图数据.Stride - 新图数据.Width * 3;
                }
            }
            原图.UnlockBits(原图数据);
            新图.UnlockBits(新图数据);
        }
        public static void 原图24刷新图32(Bitmap 原图, Bitmap 新图) {
            if (原图.Width != 新图.Width || 原图.Height != 新图.Height) return;
            Rectangle 选图框 = new Rectangle(0, 0, 原图.Width, 原图.Height);
            BitmapData 原图数据 = 原图.LockBits(选图框, ImageLockMode.ReadWrite, 原图.PixelFormat);
            BitmapData 新图数据 = 新图.LockBits(选图框, ImageLockMode.ReadWrite, 新图.PixelFormat);
            unsafe {
                byte* 原图指针 = (byte*)(原图数据.Scan0);
                byte* 新图指针 = (byte*)(新图数据.Scan0);
                for (int 列 = 0; 列 < 原图数据.Height; 列++) { //这里必须用"位图数据.Height",不小心写成"位图.Height"就会导致程序莫名变慢很多.
                    for (int 行 = 0; 行 < 原图数据.Width; 行++) {
                        新图指针[红] = 原图指针[红];
                        新图指针[绿] = 原图指针[绿];
                        新图指针[蓝] = 原图指针[蓝];
                        新图指针[透] = 255;
                        原图指针 += 3;
                        新图指针 += 4;
                    }
                    原图指针 += 原图数据.Stride - 原图数据.Width * 3;
                    //新图指针 += 新图数据.Stride - 新图数据.Width * 3;
                }
            }
            原图.UnlockBits(原图数据);
            新图.UnlockBits(新图数据);
        }

        public static void 原图刷新图32(Bitmap 原图, Bitmap 新图) {
            if (原图.Width != 新图.Width || 原图.Height != 新图.Height) return;
            Rectangle 选图框 = new Rectangle(0, 0, 原图.Width, 原图.Height);
            BitmapData 原图数据 = 原图.LockBits(选图框, ImageLockMode.ReadWrite, 原图.PixelFormat);
            BitmapData 新图数据 = 新图.LockBits(选图框, ImageLockMode.ReadWrite, 新图.PixelFormat);
            unsafe {
                byte* 原图指针 = (byte*)(原图数据.Scan0);
                byte* 新图指针 = (byte*)(新图数据.Scan0);
                for (int 列 = 0; 列 < 原图数据.Height; 列++) { //这里必须用"位图数据.Height",不小心写成"位图.Height"就会导致程序莫名变慢很多.
                    for (int 行 = 0; 行 < 原图数据.Width; 行++, 原图指针 += 4, 新图指针 += 4) {
                        新图指针[红] = 原图指针[红];
                        新图指针[绿] = 原图指针[绿];
                        新图指针[蓝] = 原图指针[蓝];
                        新图指针[透] = 原图指针[透];

                    }
                }
            }
            原图.UnlockBits(原图数据);
            新图.UnlockBits(新图数据);
        }

        public static void 调透明度(Bitmap 原图, Bitmap 新图, int 透明度) {
            if (原图.Width != 新图.Width || 原图.Height != 新图.Height) return;
            Rectangle 选图框 = new Rectangle(0, 0, 原图.Width, 原图.Height);
            BitmapData 原图数据 = 原图.LockBits(选图框, ImageLockMode.ReadWrite, 原图.PixelFormat);
            BitmapData 新图数据 = 新图.LockBits(选图框, ImageLockMode.ReadWrite, 新图.PixelFormat);
            unsafe {
                byte* 原图指针 = (byte*)(原图数据.Scan0);
                byte* 新图指针 = (byte*)(新图数据.Scan0);
                for (int 列 = 0; 列 < 原图数据.Height; 列++) { //这里必须用"位图数据.Height",不小心写成"位图.Height"就会导致程序莫名变慢很多.
                    for (int 行 = 0; 行 < 原图数据.Width; 行++, 原图指针 += 4, 新图指针 += 4) {
                        int 新透明度 = 原图指针[透] * 透明度 / 255;
                        新图指针[透] = (byte)新透明度;
                    }//根据原图透明度和透明度参数调整新图透明度.

                }
            }
            原图.UnlockBits(原图数据);
            新图.UnlockBits(新图数据);

        }//0为完全透明,255为不透明

        public static void 画框调透明度(Bitmap 位图, Rectangle 框, byte 透明度) {
            Rectangle 选图框 = new Rectangle(0, 0, 位图.Width, 位图.Height);
            BitmapData 位图数据 = 位图.LockBits(选图框, ImageLockMode.ReadWrite, 位图.PixelFormat);
            框.X = 框.X < 0 ? 0 : 框.X;
            框.Y = 框.Y < 0 ? 0 : 框.Y;
            unsafe {
                byte* 位图指针 = (byte*)(位图数据.Scan0);
                for (int 列 = 框.Y; 列 < 位图数据.Height && 列 <= 框.Y + 框.Height; 列++) {
                    byte* 指针 = 位图指针 + 列 * 位图数据.Stride + 框.X * 4;
                    for (int 行 = 框.X; 行 < 位图数据.Width && 行 <= 框.X + 框.Width; 行++, 指针 += 4) 
                        指针[透] = 透明度;
                }
            }
            位图.UnlockBits(位图数据);

        }//0为完全透明,255为不透明

        public static void 颜色随机化(Bitmap 位图, byte 波动幅度) {
            Rectangle 选图框 = new Rectangle(0, 0, 位图.Width, 位图.Height);
            BitmapData 位图数据 = 位图.LockBits(选图框, ImageLockMode.ReadWrite, 位图.PixelFormat);
            Random 随机 = new Random();
            种子 = (uint)随机.Next(int.MaxValue);
            uint 倍数 = uint.MaxValue / (uint)(波动幅度 * 2);
            unsafe {
                byte* 位图指针 = (byte*)(位图数据.Scan0);
                for (int 列 = 0; 列 < 位图数据.Height; 列++) {
                    for (int 行 = 0; 行 < 位图数据.Width; 行++, 位图指针 += 4) {
                        if (位图指针[蓝] == 0 && 位图指针[绿] == 0 && 位图指针[红] == 0 && 位图指针[透] == 0)
                            continue;
                        if (随机xorshift32() >> 31 != 0) continue;
                        int T = 位图指针[透] + (int)(随机xorshift32() / 倍数) - 波动幅度;
                        T = T > 255 ? 255 : (T < 0 ? 0 : T);
                        位图指针[透] = (byte)T;
                    }
                }
            }
            位图.UnlockBits(位图数据);
        }
        static uint 随机xorshift32() {
            uint x = 种子;
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            种子 = x;
            return x;
        }
        public static void 着透明色32(int[,] 坐标数组, int x, int y, Bitmap 位图, Bitmap HSI位图, 颜色范围 范围) {
            byte H, S, I;
            Rectangle 选图框 = new Rectangle(0, 0, 位图.Width, 位图.Height);
            BitmapData 位图数据 = 位图.LockBits(选图框, ImageLockMode.ReadWrite, 位图.PixelFormat);
            BitmapData HSI数据 = HSI位图.LockBits(选图框, ImageLockMode.ReadWrite, HSI位图.PixelFormat);
            unsafe {
                byte* 位图指针 = (byte*)(位图数据.Scan0);
                byte* HSI指针 = (byte*)(HSI数据.Scan0);
                I = HSI指针[y * HSI数据.Stride + x * 4 + 蓝];
                S = HSI指针[y * HSI数据.Stride + x * 4 + 绿];
                H = HSI指针[y * HSI数据.Stride + x * 4 + 红];

                for (int 列 = 0; 列 < 位图数据.Height; 列++) { 
                    for (int 行 = 0; 行 < 位图数据.Width; 行++, 位图指针 += 4, HSI指针 += 4) {
                        if (位图指针[透] == 0) continue;
                        if (范围.颜色相似(H, S, I, HSI指针[红], HSI指针[绿], HSI指针[蓝]))
                            位图指针[透] = 0;
                    }
                }
                               
            }
            位图.UnlockBits(位图数据);
            HSI位图.UnlockBits(HSI数据);

        }

        static public void ArgbToHsi(int color_R, int color_G, int color_B, ref int HH, ref int SS, ref int II) {
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
            SS = hsl[1] = (int)(s * 2.55 + 0.5);
            II = hsl[2] = (int)(l * 2.55 + 0.5);

        }

        static public void ArgbToHsi(byte color_R, byte color_G, byte color_B, ref byte HH, ref int SS, ref byte II) {
            int Rint = 0, Gint = 0, Bint = 0, Hint = 0, Sint = 0, Iint = 0;
            Rint = color_R;
            Gint = color_G;
            Bint = color_B;
            ArgbToHsi(Rint, Gint, Bint, ref Hint, ref Sint, ref Iint);
            HH = (byte)Hint;
            SS = (byte)Sint;
            II = (byte)Iint;
        }

        static public Bitmap 生成HSI位图32(Bitmap 位图) {
            Bitmap HSI位图 = new Bitmap(位图.Width, 位图.Height, PixelFormat.Format32bppArgb);
            Rectangle 选图框 = new Rectangle(0, 0, 位图.Width, 位图.Height);
            BitmapData 位图数据 = 位图.LockBits(选图框, ImageLockMode.ReadWrite, 位图.PixelFormat);
            BitmapData HSI位图数据 = HSI位图.LockBits(选图框, ImageLockMode.ReadWrite, HSI位图.PixelFormat);
            int[] hsi = new int[3];
            unsafe {
                byte* 位图指针 = (byte*)(位图数据.Scan0);
                byte* HSI位图指针 = (byte*)(HSI位图数据.Scan0);
                for (int 列 = 0; 列 < 位图数据.Height; 列++) {
                    for (int 行 = 0; 行 < 位图数据.Width; 行++) {
                        ArgbToHsi(位图指针[红], 位图指针[绿], 位图指针[蓝], ref hsi[0], ref hsi[1], ref hsi[2]);
                        HSI位图指针[蓝] = (byte)hsi[2];
                        HSI位图指针[绿] = (byte)hsi[1];
                        HSI位图指针[红] = (byte)hsi[0];
                        HSI位图指针[透] = 位图指针[透];
                        位图指针 += 4;
                        HSI位图指针 += 4;
                    }
                    
                }
            }
            位图.UnlockBits(位图数据);
            HSI位图.UnlockBits(HSI位图数据);
            return HSI位图;
        }//0为完全透明,255为不透明
    }

    class 颜色范围 {
        int Hup = 0;
        int Sup = 0;
        int Iup = 0;
        int Hlow = 0;
        int Slow = 0;
        int Ilow = 0;
        int dH = 1;
        int dS = 3;
        int dI = 2;
        int N = 0;

        public void 范围加() {
            if (N % 2 == 0) {
                Hup = dH;
                Sup = dS;
                Iup = dI;
            }
            else {
                Hlow = dH;
                Slow = dS;
                Ilow = dI;
                dH += 1;//1;
                dS += 3;//3;
                dI += 2;//2;
            }
            N++;
        }

        public void 范围减() {
            N++;
            if (N % 2 == 0) {
                Hup -= 1;
                Sup -= 3;
                Iup -= 2;
                Hup = Hup < 0 ? -1 : Hup;
                Sup = Sup < 0 ? 0 : Sup;
                Iup = Iup < 0 ? 0 : Iup;
            }
            else {
                Hlow -= 1;
                Slow -= 3;
                Ilow -= 2;
                Hlow = Hlow < 0 ? 0 : Hlow;
                Slow = Slow < 0 ? 0 : Slow;
                Ilow = Ilow < 0 ? 0 : Ilow;
            }
            
        }

        public bool 颜色相似(byte H0, byte S0, byte I0, byte H1, byte S1, byte I1) {
            return H1 <= H0 + Hup && H1 >= H0 - Hlow &&
                   S1 <= S0 + Sup && S1 >= S0 - Slow &&
                   I1 <= I0 + Iup && I1 >= I0 - Ilow;

        }//0是原颜色,1要在0的范围内
    }
}
