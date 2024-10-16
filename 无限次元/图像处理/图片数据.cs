using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 图像处理;
public class 图片数据 {
    public Bitmap 位图;
    int[] R谱, G谱, B谱;
    int[] H谱, S谱, L谱;
    int[,] HS谱;
   
    public 图片数据(Bitmap 位图) {
        this.位图 = 位图;
    }

    private void 统计谱数据(bool HSL = true) {
        R谱 = new int[256]; G谱 = new int[256]; B谱 = new int[256];
        H谱 = new int[241]; S谱 = new int[241]; L谱 = new int[241];
        HS谱 = new int[241, 241];
        图像.获取HSI_RGB谱(位图, R谱, G谱, B谱, H谱, S谱, L谱, HS谱, HSL);
    }

    public Bitmap 画直方图() {
        Bitmap 直方图 = new Bitmap(1900, 844, PixelFormat.Format24bppRgb);
        Size 尺寸 = new Size(1024, 380); //单个直方图的尺寸
        Point 起点 = new Point(11, 11); //左上角起始点
        Rectangle RGB图框 = new Rectangle(起点, 尺寸);
        Rectangle HSL图框 = new Rectangle(new Point(起点.X, 起点.Y + 尺寸.Height + 起点.Y * 3), 尺寸); //HSL直方图放在RGB的下面
        int 条高 = 20;
        统计谱数据();
        Graphics g = Graphics.FromImage(直方图);
        g.Clear(Color.FromArgb(22, 44, 55));
        绘图.画色调或亮度条(g, new Rectangle(HSL图框.Left, RGB图框.Bottom + 3, RGB图框.Width, 条高), false);
        绘图.画整型数据(R谱, g, RGB图框, Color.Red);
        绘图.画整型数据(G谱, g, RGB图框, Color.Green);
        绘图.画整型数据(B谱, g, RGB图框, Color.Blue);  //RGB直方图画在一起,如果是灰度图就只会看到一条蓝色谱线

        绘图.画色调或亮度条(g, new Rectangle(HSL图框.Left, HSL图框.Bottom + 3, HSL图框.Width, 条高), true);
        绘图.画整型数据(H谱, g, HSL图框, Color.Purple);
        绘图.画整型数据(S谱, g, HSL图框, Color.Gray);
        绘图.画整型数据(L谱, g, HSL图框, Color.White);

        float 放大倍数 = 3.366f;
        绘图.画HS色谱图(g, HS谱, new Rectangle(RGB图框.Right + 起点.X * 2, 起点.Y, (int)(HS谱.GetLength(1) * 放大倍数) + 1,(int)(HS谱.GetLength(0) * 放大倍数) + 1));

        g.Dispose();
        return 直方图;
    }

    public void 直方图均衡化() {
        统计谱数据(false);
        图像.直方图均衡化(位图, R谱, G谱, B谱);
    }

   

}

