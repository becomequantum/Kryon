//無限次元:https://space.bilibili.com/2139404925
//本代码来自:https://github.com/becomequantum/Kryon


namespace 图像处理;
public partial class 图像 {
    public const int 前景 = 1, 背景 = 0;
    static public Color[] 色谱颜色24 = new Color[24] { Color.FromArgb(255, 0, 0),Color.FromArgb(255, 64, 0), Color.FromArgb(255, 128, 0),Color.FromArgb(255, 192, 0), Color.FromArgb(255, 255, 0), Color.FromArgb(192, 255, 0),Color.FromArgb(128, 255, 0),Color.FromArgb(64, 255, 0),
            Color.FromArgb(0, 255, 0),Color.FromArgb(0,255,64),Color.FromArgb(0,255,128),Color.FromArgb(0,255,192),Color.FromArgb(0,255,255), Color.FromArgb(0,192,255), Color.FromArgb(0,128,255), Color.FromArgb(0,64,255),
            Color.FromArgb(0,0,255),Color.FromArgb(64,0,255),Color.FromArgb(128,0,255),Color.FromArgb(192,0,255), Color.FromArgb(255,0,255),Color.FromArgb(255,0,192),Color.FromArgb(255,0,128),Color.FromArgb(255,0,64)};
    static Point 右 = new(1, 0), 左 = new (-1, 0), 下 = new (0, 1), 右下 = new (1, 1), 左下 = new (-1, 1), 左上 = new (-1, -1), 上 = new (0, -1), 右上 = new (1, -1);

    public static Point[][] 顺八邻域 = new Point[4][]{ new Point[] { 右, 下, 左, 上, 右下, 左下, 左上, 右上},
                                                       new Point[] { 下, 左, 上, 右, 右下, 左下, 左上, 右上},
                                                       new Point[] { 左, 上, 右, 下, 右下, 左下, 左上, 右上},
                                                       new Point[] { 上, 右, 下, 左, 右下, 左下, 左上, 右上} };


    static bool 位图转二值数组<T>(Bitmap 位图, T[,] 二值数组, T 前景一, T 背景零, int pad = 0) {
        byte 字节数 = 3;
        if (位图.PixelFormat == PixelFormat.Format32bppArgb) 字节数 = 4;
        BitmapData 位图数据 = 位图.LockBits(new Rectangle(0, 0, 位图.Width, 位图.Height), ImageLockMode.ReadWrite, 位图.PixelFormat);
        unsafe {
            byte* 位针 = (byte*)(位图数据.Scan0);
            for (int y = 0; y < 位图数据.Height; y++, 位针 += 位图数据.Stride - 位图数据.Width * 字节数)
                for (int x = 0; x < 位图数据.Width; x++, 位针 += 字节数)
                    if (x == 0 || x == 位图数据.Width - 1 || y == 0 || y == 位图数据.Height - 1)
                        二值数组[y + pad, x + pad] = 背景零;     //边框一律为背景
                    else if (位针[红] == 255 && 位针[绿] == 255 && 位针[蓝] == 255)
                        二值数组[y + pad, x + pad] = 前景一;     //白色为1,前景
                    else if (位针[红] == 0 && 位针[绿] == 0 && 位针[蓝] == 0)
                        二值数组[y + pad, x + pad] = 背景零;     //黑色为0,背景
                    else {
                        位图.UnlockBits(位图数据);
                        return false;           //还有其它颜色说明不是二值图像,直接返回false.
                    }
        }
        位图.UnlockBits(位图数据);
        return true;
    }
    static public void 灰度二值化(Bitmap 位图, int 阈值 = 128) {
        int 字节数 = (位图.PixelFormat == PixelFormat.Format32bppArgb) ? 4 : 3;
        BitmapData 位图数据 = 位图.LockBits(new Rectangle(0, 0, 位图.Width, 位图.Height), ImageLockMode.ReadWrite, 位图.PixelFormat);
        unsafe {
            byte* 位针 = (byte*)(位图数据.Scan0);
            for (int y = 0; y < 位图数据.Height; y++, 位针 += 位图数据.Stride - 位图数据.Width * 字节数)
                for (int x = 0; x < 位图数据.Width; x++, 位针 += 字节数)
                    位针[红] = 位针[绿] = 位针[蓝] = 位针[绿] >= 阈值 ? (byte)255 : (byte)0;
        }
        位图.UnlockBits(位图数据);
    }

    static void 二值数组标位图(Bitmap 位图, byte[,] 二值数组) {
        byte 字节数 = 3;
        if (位图.PixelFormat == PixelFormat.Format32bppArgb) 字节数 = 4;
        BitmapData 位图数据 = 位图.LockBits(new Rectangle(0, 0, 位图.Width, 位图.Height), ImageLockMode.ReadWrite, 位图.PixelFormat);
        unsafe {
            byte* 位针 = (byte*)(位图数据.Scan0);
            for (int y = 0; y < 位图数据.Height; y++, 位针 += 位图数据.Stride - 位图数据.Width * 字节数)
                for (int x = 0; x < 位图数据.Width; x++, 位针 += 字节数)
                    if (二值数组[y, x] == 1)
                        位针[红] = 位针[绿] = 位针[蓝] = 255;
                    else
                        位针[红] = 位针[绿] = 位针[蓝] = 0;
        }
        位图.UnlockBits(位图数据);
    }

    public static byte[,] 阈胀蚀(Bitmap 位图, bool 反标位图 = true) {
        int 直径 = 参数.算子直径, 半径 = 参数.算子直径 / 2;
        byte[,] 二值数组 = new byte[位图.Height + 半径 * 2, 位图.Width + 半径 * 2];
        if (!位图转二值数组(位图, 二值数组, (byte)1, (byte)0, 半径 /* 加了padding */)) { MessageBox.Show("这不是二值图像!"); return null; }
        byte[,] 结果数组 = new byte[位图.Height, 位图.Width];
        byte[][] 移位寄存器 = new byte[直径][];                //模仿Verilog实现卷积时的写法.这样写在CPU里执行的并不快。
        for (int i = 0; i < 移位寄存器.Length; i++)
            移位寄存器[i] = new byte[直径];
        byte[] 临时;
        for (int y = 0; y < 二值数组.GetLength(0) - 直径; y++)
            for (int x = 0; x < 二值数组.GetLength(1); x++) {
                临时 = 移位寄存器[0];
                for (int i = 0; i < 移位寄存器.Length - 1; i++)
                    移位寄存器[i] = 移位寄存器[i + 1];
                移位寄存器[移位寄存器.Length - 1] = 临时;    //向左移位. Verilog实现其实不需要这个临时变量.

                for (int i = 0; i < 移位寄存器[移位寄存器.Length - 1].Length; i++)
                    移位寄存器[移位寄存器.Length - 1][i] = 二值数组[y + i, x]; //把数组里高度为算子直径的一列值移入移位寄存器.

                if (x < 直径 - 1) continue;   //已经移入直径列数据后再开始计算

                int 和 = 0;
                foreach (var 列 in 移位寄存器)
                    foreach (var 值 in 列)
                        和 += 值;

                if (和 >= 参数.胀蚀阈值)  //阈值为1时为膨胀效果, 阈值为直径平方时为腐蚀效果. 
                    结果数组[y, x - (直径 - 1)] = 1;
            }
        if (反标位图) 二值数组标位图(位图, 结果数组);
        return 结果数组;
    }

    public static void 取边缘(Bitmap 位图)
    {
        byte[,] 二值数组 = new byte[位图.Height, 位图.Width];
        if (!位图转二值数组(位图, 二值数组, (byte)1, (byte)0)) { MessageBox.Show("这不是二值图像!"); return; }
        byte[,] 结果数组 = new byte[位图.Height, 位图.Width];
        for (int y = 1; y < 二值数组.GetLength(0) - 1; y++)
            for (int x = 1; x < 二值数组.GetLength(1) - 1; x++)
            {
                if (二值数组[y, x] == 0) continue;
                int 和 = 二值数组[y + 1, x] + 二值数组[y - 1, x] + 二值数组[y, x + 1] + 二值数组[y, x - 1];
                if (和 == 4) 结果数组[y, x] = 0;   
                else 结果数组[y, x] = 1;
            }
        二值数组标位图(位图, 结果数组);
    }

    static public void 连通域识别(Bitmap 位图, Action 刷新图片) {
        int[,] 二值数组 = 连通域扫描(位图, 刷新图片);
        if (二值数组 == null) { MessageBox.Show("这不是二值图像!"); return; }
        递归法连通域信息反标位图(位图, 二值数组);
    }

    #region 递归法连通域识别
    static int[,] 连通域扫描(Bitmap 位图, Action 刷新图片) {
        int[,] 二值数组 = new int[位图.Height, 位图.Width];
        if (!位图转二值数组(位图, 二值数组, 前景, 背景)) return null;
        int 标号 = 2;                                           //0:背景, 1: 前景, 所以连通域标号从2开始
        for (int y = 1; y < 二值数组.GetLength(0) - 1; y++)     //GetLength:[0 Height, 1 Width]
            for (int x = 1; x < 二值数组.GetLength(1) - 1; x++)
                if (二值数组[y, x] == 1) {
                    int 计数 = 0, 邻域索引 = 0;
                    递归标邻域(二值数组, x, y, 标号++, ref 计数, ref 邻域索引, 刷新图片, 位图);
                }
        return 二值数组;
    }

    static void 递归标邻域(int[,] 二值数组, int x, int y,  int 标号, ref int 计数, ref int 邻域索引, Action 刷新图片, Bitmap 位图 = null) {
        二值数组[y, x] = 标号;
        #region 显示算法标记过程
        if (参数.显示连通域标记过程) {
            位图.SetPixel(x , y, 色谱颜色24[(标号 - 2) % 24]);            
            刷新图片();
        }
        #endregion
        #region 延迟爆栈措施
        计数++;
        if (计数 > 64) {
            邻域索引 = (邻域索引 + 1) % (顺八邻域.Length);
            计数 = 0;
        }//计数大于一定值时就改变递归标点的方向,形成顺时针向内转的螺旋,这样区域内点都被标上时递归调用的函数就都会返回,减少栈里的内容.
        #endregion
        foreach (Point 邻域点 in 顺八邻域[邻域索引]) {
            if (二值数组[y + 邻域点.Y, x + 邻域点.X] == 1)
                递归标邻域(二值数组, x + 邻域点.X, y + 邻域点.Y, 标号, ref 计数, ref 邻域索引, 刷新图片, 位图);
            邻域索引 = (邻域索引 + 1) % (顺八邻域.Length); //延迟爆栈措施
        }
        计数 = 0; //延迟爆栈措施
    }//递归法的坏处是连通域点数太多就会栈溢出,所以加了点延迟爆栈措施,连通域太大终究还是会爆.

    private static void 递归法连通域信息反标位图(Bitmap 位图, int[,] 二值数组) {
        List<连通域信息> 连通域表 = new();
        byte 字节数 = 3;
        if (位图.PixelFormat == PixelFormat.Format32bppArgb) 字节数 = 4;
        BitmapData 位图数据 = 位图.LockBits(new Rectangle(0, 0, 位图.Width, 位图.Height), ImageLockMode.ReadWrite, 位图.PixelFormat);
        unsafe {//结果反标在位图上
            byte* 位针 = (byte*)(位图数据.Scan0);
            for (int y = 0; y < 位图数据.Height; y++, 位针 += 位图数据.Stride - 位图数据.Width * 字节数)
                for (int x = 0; x < 位图数据.Width; x++, 位针 += 字节数)
                    if (二值数组[y , x ] > 1) {
                        int 表Index = 二值数组[y , x ] - 2;
                        if (连通域表.Count < 表Index + 1) 连通域表.Add(new 连通域信息());
                        连通域表[表Index].总点数++;          //统计总点数
                        //计算连通域边框坐标
                        if (x < 连通域表[表Index].XMin) 连通域表[表Index].XMin = x;
                        if (x > 连通域表[表Index].XMax) 连通域表[表Index].XMax = x;
                        if (y > 连通域表[表Index].YMax) 连通域表[表Index].YMax = y; 
                        if (y < 连通域表[表Index].YMin) 连通域表[表Index].YMin = y;
                        //不同的连通域标记上不同的颜色
                        Color 颜色 = 色谱颜色24[(二值数组[y, x] - 2) % 24];
                        (位针[红], 位针[绿], 位针[蓝]) = (颜色.R, 颜色.G, 颜色.B);
                    }
        }
        位图.UnlockBits(位图数据);
        连通域信息标记(位图, 连通域表);
    }//连通域信息统计是可以放在连通域扫描的同时进行的, 这里图省事分开写了.

    private static void 连通域信息标记(Bitmap 位图, List<连通域信息> 连通域表) {
        Graphics g = Graphics.FromImage(位图);
        for (int i = 0; i < 连通域表.Count; i++) {
            //显示连通域编号和总点数
            g.DrawString(/*(i + 1).ToString() + ":" +*/ 连通域表[i].总点数.ToString(), 绘图.TNR字体(12), new SolidBrush(Color.LightGreen), new Point(连通域表[i].XMin, 连通域表[i].YMax));
            //画边框，边框是套在连通域外边的，左右上下各扩大一个像素
            g.DrawRectangle(new Pen(Color.LightGreen), new Rectangle(连通域表[i].XMin - 1, 连通域表[i].YMin - 1, 连通域表[i].XMax - 连通域表[i].XMin + 2, 连通域表[i].YMax - 连通域表[i].YMin + 2));
        }
        g.Dispose();
    }
    #endregion


    class 连通域信息 {
        public int 总点数 = 0;           //连通域的总点数
        public int XMin = int.MaxValue;  //连通域外切矩形坐标
        public int XMax = 0;
        public int YMin = int.MaxValue;
        public int YMax = 0;
        public int BottomLineXMax = 0;   //记录连通域最下面一行最右边点的X坐标，用于逐行扫描法中判断一个连通域是否结束
        public int 真实标号 = -1;        //只在逐行连通域标记时用到，用于记录这个连通域被并入到哪个连通域去了
    }
   
}

