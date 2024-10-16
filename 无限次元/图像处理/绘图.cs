using System.Drawing.Drawing2D;

namespace 图像处理;
public class 绘图 {
    private const int 红 = 2, 绿 = 1, 蓝 = 0; //Bitmap位图数据中,蓝色Blue的值存在低地址上

    static public Font TNR字体(int 字号) {
        return new Font(new FontFamily("Times New Roman"), 字号);
    }

    static public Color 反色(Color 色) {
        return Color.FromArgb(255 - 色.R, 255 - 色.G, 255 - 色.B);
    }

    static void G画点(Graphics g, PointF 点, Color 颜色, float 大小 = 1) {
        G画点(g, 点.X, 点.Y, 颜色, 大小);
    }

    static public void G画点(Graphics g, float x, float y, Color 颜色, float 大小 = 1) {
        if (大小 <= 1)
            g.DrawLine(new Pen(new SolidBrush(颜色)), (int)(x + 0.5), (int)(y + 0.5), (int)(x + 0.5), (float)((int)(y + 0.5) - 0.2));
        else
            g.FillEllipse(new SolidBrush(颜色), new RectangleF(x - 大小 / 2, y - 大小 / 2, 大小, 大小));
    }
   
    static public void 画整型数据(short[] 数据, Graphics g, Rectangle 边框, Color 线条颜色, bool 画中线 = false, float 线粗 = 1) {
        Pen 粗笔 = new Pen(线条颜色, 线粗);
        float X单位 = (float)边框.Width / 数据.Length;
        float Y单位 = 1;//(float)边框.Height / 数据.Max();
        for (int i = 0; i < 数据.Length - 1; i++)
            g.DrawLine(粗笔, i * X单位 + 边框.X, 边框.Height + 边框.Y - 数据[i] * Y单位, (i + 1) * X单位 + 边框.X, 边框.Height + 边框.Y - 数据[i + 1] * Y单位);
        for (int i = 0; i < 数据.Length; i++)
            G画点(g, i * X单位 + 边框.X, 边框.Height + 边框.Y - 数据[i] * Y单位, 数据[i] > 0 ? 反色(线条颜色) : Color.Red, 2);//画数据点
        if (!画中线) return;
        g.DrawLine(new Pen(new SolidBrush(Color.AliceBlue)), ((数据.Length) / 2 ) * X单位 + 边框.X, 边框.Height + 边框.Y, ((数据.Length) / 2) * X单位 + 边框.X, 边框.Height + 边框.Y - 数据[((数据.Length) / 2 )] * Y单位);
    }

    static public void 画浮点数据(float[] 数据, Graphics g, Rectangle 边框, Color 线条颜色, bool 画中线 = false, float 线粗 = 1) {
        Pen 粗笔 = new Pen(线条颜色, 线粗);
        float X单位 = (float)边框.Width / 数据.Length;
        float Y单位 = 1;//(float)边框.Height / 数据.Max();
        for (int i = 0; i < 数据.Length - 1; i++)
            g.DrawLine(粗笔, i * X单位 + 边框.X, 边框.Height + 边框.Y - 数据[i] * Y单位, (i + 1) * X单位 + 边框.X, 边框.Height + 边框.Y - 数据[i + 1] * Y单位);
        //for (int i = 0; i < 数据.Length; i++)
        //    G画点(g, i * X单位 + 边框.X, 边框.Height + 边框.Y - 数据[i] * Y单位, Color.LightGreen, 2);//画数据点
        //if (!画中线) return;
        //g.DrawLine(new Pen(new SolidBrush(Color.AliceBlue)), ((数据.Length) / 2 + 1) * X单位 + 边框.X, 边框.Height + 边框.Y, ((数据.Length) / 2 + 1) * X单位 + 边框.X, 边框.Height + 边框.Y - 数据[((数据.Length) / 2 + 1)] * Y单位);
    }

    static public void 画整型数据(int[] 数据, Graphics g, Rectangle 边框, Color 线条颜色, bool 画中线 = false, float 线粗 = 1) {
        Pen 粗笔 = new Pen(线条颜色, 线粗);
        float X单位 = (float)边框.Width / 数据.Length;
        float Y单位 = (float)边框.Height / 数据.Max();
        for (int i = 0; i < 数据.Length - 1; i++)
            g.DrawLine(粗笔, i * X单位 + 边框.X, 边框.Height + 边框.Y - 数据[i] * Y单位, (i + 1) * X单位 + 边框.X, 边框.Height + 边框.Y - 数据[i + 1] * Y单位);
        for (int i = 0; i < 数据.Length; i++)
            G画点(g, i * X单位 + 边框.X, 边框.Height + 边框.Y - 数据[i] * Y单位, Color.LightGreen, 2);//画数据点
        if (!画中线) return;
        g.DrawLine(new Pen(new SolidBrush(Color.AliceBlue)), ((数据.Length) / 2 + 1) * X单位 + 边框.X, 边框.Height + 边框.Y, ((数据.Length) / 2 + 1) * X单位 + 边框.X, 边框.Height + 边框.Y - 数据[((数据.Length) / 2 + 1)] * Y单位);
    }
    static public void 画字节数据(byte[] 数据, Graphics g, Rectangle 绘图边框, Color 线条颜色, bool 画竖线 = false) {
        Pen 笔 = new Pen(new SolidBrush(线条颜色));
        float X单位 = (float)绘图边框.Width / 数据.Length;
        float Y单位 = (float)绘图边框.Height / byte.MaxValue;

        for (int i = 0; i < 数据.Length - 1; i++)
            g.DrawLine(笔, i * X单位 + 绘图边框.X, 绘图边框.Height + 绘图边框.Y - 数据[i] * Y单位, (i + 1) * X单位 + 绘图边框.X, 绘图边框.Height + 绘图边框.Y - 数据[i + 1] * Y单位);
        if (画竖线)
            for (int i = 0; i < 数据.Length; i++)
                g.DrawLine(new Pen(new SolidBrush(图像.色谱颜色24[(i * 2 + i / 12) % 24])), i * X单位 + 绘图边框.X, 绘图边框.Height + 绘图边框.Y, i * X单位 + 绘图边框.X, 绘图边框.Height + 绘图边框.Y - 数据[i] * Y单位);
        //for (int i = 0; i < 数据.Length; i++)
        //    G画点(g, i * X单位 + 绘图边框.X, 绘图边框.Height + 绘图边框.Y - 数据[i] * Y单位, Color.LightGreen);//画数据点
    }
    static public void 生成正弦数据(byte[] 字节数据, double 频率, double 幅度, double 相位, bool 形状展开 = false) {//幅度值在[0,1]之间
        for (int k = 0; k < 字节数据.Length; k++) {
            double x = (double)k / 字节数据.Length * Math.PI * 2;
            if (!形状展开)
                字节数据[k] = (byte)((Math.Sin(x * 频率 + 相位) * 幅度 + 1) * byte.MaxValue / 2 + 0.5);
            else
                字节数据[k] = (byte)((Math.Sin(x * 频率 + 相位) * 幅度 + 幅度) * byte.MaxValue / 2 + 0.5);
        }
    }

    static public void 生成正弦数据(double[] 数据, double 频率, double 幅度, double 相位) {//幅度值在[0,1]之间
        for (int k = 0; k < 数据.Length; k++) {
            double x = (double)k / 数据.Length * Math.PI * 2;
            数据[k] = Math.Sin(x * 频率 + 相位) * 幅度;
        }
    }

    static public void 画柱状图(Graphics g, double[] 数据, double 数据最大值, double 数据最小值, Rectangle 边框, String 图名, Color 柱色) {
        int 宽 = 边框.Width / 数据.Length;
        int 柱宽 = 宽 / 2 + 宽 % 2;
        for (int i = 0; i < 数据.Length; i++) {
            int 柱高 = (int)((数据[i] - 数据最小值) / 数据最大值 * 边框.Height + 0.5);
            g.FillRectangle(new SolidBrush(柱色), i * 宽 + 边框.X, 边框.Height - 柱高 + 边框.Y, 柱宽, 柱高);
            if (i % 4 == 0 || i < 10)
                g.DrawString(i.ToString(), TNR字体(8), new SolidBrush(Color.Purple), new Point(边框.X + i * 宽 - 3, 边框.Y + 边框.Height));
        }
        g.DrawString(图名, TNR字体(8), new SolidBrush(Color.Purple), new Point(边框.X - 3, 边框.Y - 15));
    }

    static public void 画色调或亮度条(Graphics g, Rectangle 绘图框, bool 画色调) {
        Bitmap 位图 = new Bitmap(绘图框.Width, 绘图框.Height, PixelFormat.Format24bppRgb);
        画色调或亮度条(位图, 画色调);
        g.DrawImage(位图, 绘图框);

    }

    static void 画色调或亮度条(Bitmap 位图, bool 画色调) {
        BitmapData 位图数据 = 位图.LockBits(new Rectangle(0, 0, 位图.Width, 位图.Height), ImageLockMode.ReadWrite, 位图.PixelFormat);
        Color[] 颜色值 = new Color[位图.Width];
        byte[] 亮度值 = new byte[位图.Width];
        if (画色调) {
            for (int i = 0; i < 颜色值.Length; i++) {
                int H = 线性映射a到b(i, 颜色值.Length, 240, 0, 0);
                int R = 0, G = 0, B = 0;
                图像.HSL转RGB(H, 239, 128, ref R, ref G, ref B);
                颜色值[i] = Color.FromArgb(R, G, B);
            }
        }
        else
            for (int i = 0; i < 亮度值.Length; i++)
                亮度值[i] = (byte)线性映射a到b(i, 亮度值.Length, 256);
        unsafe {
            byte* 位针 = (byte*)(位图数据.Scan0);
            for (int 列 = 0; 列 < 位图数据.Height; 列++, 位针 += 位图数据.Stride - 位图数据.Width * 3)
                for (int 行 = 0; 行 < 位图数据.Width; 行++, 位针 += 3) {
                    if (画色调)
                        (位针[红], 位针[绿], 位针[蓝]) = (颜色值[行].R, 颜色值[行].G, 颜色值[行].B);
                    else
                        位针[红] = 位针[绿] = 位针[蓝] = 亮度值[行];
                }
        }
        位图.UnlockBits(位图数据);
    }

    public static void 画HS色谱图(Graphics g, int[,] HS谱, Rectangle 绘图框) {
        int H = 0, S = 0, I = 128;
        int R = 0, G = 0, B = 0;
        int 最大值 = 240;
        float 倍数 = 绘图框.Width / (float)最大值;
        int 谱均值 = 数组平均值(HS谱);
        for (int 列 = 最大值; 列 >= 0; 列--) {
            for (int 行 = 0; 行 < 最大值; 行++) {
                H = 行;
                S = 最大值 - 列;                   //把灰的画在下面

                if (HS谱[H, S] > 0) {
                    I = (int)(HS谱[H, S] / (float)谱均值 * 100);
                    if (I > 120) I = 120;
                    I = 120 - I;                   //颜色深代表点数多
                }
                else
                    I = 128;
                图像.HSL转RGB(H, S, I, ref R, ref G, ref B);
                g.FillRectangle(new SolidBrush(Color.FromArgb(R, G, B)), 绘图框.X + 行 * 倍数, 绘图框.Y + 列 * 倍数, 倍数, 倍数);

            }
        }
    }

    static private int 数组平均值(int[,] 二维数组) {
        ulong 和 = 0;
        ulong N = 0;
        foreach (var 值 in 二维数组)
            if (值 > 0) {
                和 += (ulong)值;
                N++;
            }
        return (int)(和 / N);
    }

    static public int 线性映射a到b(int a, int aLength, int bLength, int aMin = 0, int bMin = 0) {
        return (int)((double)(a - aMin) / aLength * bLength + bMin);
    }//把线段a上的某点坐标线性映射到线段b上,返回的是b上的坐标

    static public int 线性映射a到b(float a, int aLength, int bLength, int aMin = 0, int bMin = 0) {
        return (int)((double)(a - aMin) / aLength * bLength + bMin);
    }

    public static void 画旋字(Graphics g, string S, float 转角, Font 字体, Color 颜色, float x, float y) {
        SizeF 字大小 = g.MeasureString(S, 字体);
        float w = 字大小.Width / 2f, h = 字大小.Height / 2f;
        g.TranslateTransform(x + w, y + h);
        g.RotateTransform(转角);
        g.DrawString(S, 字体, new SolidBrush(颜色), -w, -h);
        g.ResetTransform();
    }

}

//無限次元:https://space.bilibili.com/2139404925
//本代码来自:https://github.com/becomequantum/Kryon

