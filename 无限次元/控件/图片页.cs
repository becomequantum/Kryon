global using 图像处理;
using System.Drawing.Imaging;
using System.Numerics;

namespace 控件;

public interface I标签页 {
    void 关闭();
}
public class 图片页 : TabPage, I标签页 {
    public PictureBox 图片框;
    PictureBox 缩略图框;          //用于放大模式下显示和设置放大区域
    public 图片数据 图数据;
    public string 文件名, 无路径文件名, 文件信息;
    public ImageFormat 图片格式;
    static public 局部放大图 局部放大控件;
    static public 功能标签页 功能页;
    public Rectangle 选中区域 = Rectangle.Empty;
    private Bitmap 位图副本, 线段索引图;
    Graphics g位图副本;
    Bitmap 放大图;
    public float 缩放比例 = 1;
    public bool 显示网格 = false, 拖动中 = false;
    double 缩略比 = 0;

    static public string 画啥 = "不画";
    Rectangle 虚线框 = Rectangle.Empty, 上个虚线框 = Rectangle.Empty, 矩形框 = Rectangle.Empty, 原图局部框;
    Point 按下起始点, 上个点, 起点, 终点, 原图坐标, 按下点, 上个原图坐标, 局部中心, 右键按下点;
    int 彩线计数;
    Graphics g原图;
    线段信息 [] 线段组;
    public 图片页(Bitmap 位图, string 图片文件名, ContextMenuStrip 右键菜单, PictureBox 缩略图框, ImageFormat 格式 = null) {
        this.缩略图框 = 缩略图框;
        图数据 = new(位图);
        if (格式 == null) 格式 = ImageFormat.Bmp;
        获取文件信息(图片文件名, 格式);

        //图片框初始化
        图片框 = new();
        图片框.SizeMode = PictureBoxSizeMode.AutoSize;
        图片框.Image = 位图;
        图片框.Refresh();
        图片框.MouseMove += 图片框_MouseMove;
        图片框.MouseDown += 图片框_MouseDown;
        图片框.MouseUp += 图片框_MouseUp;
        图片框.MouseClick += 图片框_MouseClick;
        图片框.MouseWheel += 图片框_MouseDown;
        图片框.MouseLeave += 图片框_MouseLeave;

        //TabPage初始化
        AutoScroll = true;
        Controls.Add(图片框);
        Resize += 图片页_Resize;
        ContextMenuStrip = 右键菜单;

        缩略图框.MouseMove += 缩略图框_MouseMove;
        缩略图框.MouseClick += 缩略图框_MouseMove;
        g原图 = Graphics.FromImage(位图);
        位图副本 = new Bitmap(位图.Width, 位图.Height, PixelFormat.Format24bppRgb);
        g位图副本 = Graphics.FromImage(位图副本);
    }


    public void 回到原图() {
        if (!File.Exists(文件名)) { MessageBox.Show("没保存过的图片回不去!"); return; }
        图数据.位图 = 文件.加载图片文件(文件名, out ImageFormat 图片格式);
        if (缩放比例 == 1) {
            图片框.Image = 图数据.位图;
            图片框.Refresh();
        }
        else if (缩放比例 < 1) {
            图片框.Image = 图数据.位图;
            调到合适大小();
        }
        else {
            刷新局部放大图();
            图片框.Refresh();
            显示缩略图();
        }


    }

    public void 获取文件信息(string 图片文件名, ImageFormat 格式) {
        string 文件大小 = "";
        if (File.Exists(图片文件名)) {//如果是打开的图片
            文件名 = 图片文件名;
            无路径文件名 = Path.GetFileName(文件名);
            文件大小 = ((new FileInfo(图片文件名)).Length / 1024.0).ToString(".##") + "KB";
        }
        else
            文件名 = 无路径文件名 = 图片文件名;
        Text = 无路径文件名;
        文件信息 = 图数据.位图.Width + " x " + 图数据.位图.Height + "  " + 文件大小 + "  像素格式: " + 图数据.位图.PixelFormat.ToString()[6..] + "   " + 文件名;
        图片格式 = 格式;
    }

    public void 更新信息() {
        获取文件信息(文件名, 图片格式);
    }

    public void 加载线段数据(string 文件名) {
        using var 流 = File.OpenRead(文件名);
        if (流.Length % 16 != 0) { MessageBox.Show("读线段数据出错!"); return; }
        线段组 = new 线段信息[流.Length / 16];
        using var 读 = new BinaryReader(流);
        for (int i = 0; i < 线段组.Length; i++) {
            线段组[i].起.X = 读.ReadSingle();
            线段组[i].起.Y = 读.ReadSingle();
            线段组[i].终.X = 读.ReadSingle();
            线段组[i].终.Y = 读.ReadSingle();
        }
        文件信息 += "  已加载线段数据" + 线段组.Length + "条";
    }

    #region 鼠标事件
    private void 图片框_MouseLeave(object sender, EventArgs e) {
        局部放大控件.画青蛙();
    }
    private void 图片框_MouseClick(object sender, MouseEventArgs e) {
        原图坐标 = new((int)(e.Location.X / 缩放比例), (int)(e.Location.Y / 缩放比例));   //这个e.Location是相对于图片框的坐标
        PointF 左上 = new PointF(原图坐标.X * 缩放比例, 原图坐标.Y * 缩放比例);
        if (缩放比例 > 1) { 原图坐标.X += 原图局部框.X; 原图坐标.Y += 原图局部框.Y; }
    }
    public void 缩略图框_MouseMove(object sender, MouseEventArgs e) {
        if (缩略图框.Visible == false || ((TabControl)Parent).SelectedTab != this) return;
        if (e.Button == MouseButtons.Left) {
            if (图像.点在框内(e.Location, new Rectangle(new Point(0, 0), 缩略图框.Size), 5, 5)) {
                设置局部中心((int)(e.Location.X / 缩略比), (int)(e.Location.Y / 缩略比));
                刷新局部放大图();
                图片框.Refresh();
            }
        }
    }//在缩略图上可拖动局部放大框的位置
    private void 图片框_MouseMove(object sender, MouseEventArgs e) {
        原图坐标 = new((int)(e.Location.X / 缩放比例), (int)(e.Location.Y / 缩放比例));   //这个e.Location是相对于图片框的坐标
        if (缩放比例 > 1) { 原图坐标.X += 原图局部框.X; 原图坐标.Y += 原图局部框.Y; }
        右键按下点 = 原图坐标;
        using Graphics g图片框 = 图片框.CreateGraphics();

        if (!(缩放比例 > 1 && e.Button == MouseButtons.Left))
            局部放大控件.显示像素信息(图数据.位图, 原图坐标, e.Button == MouseButtons.Left);

        if (缩放比例 > 1 && 参数.显示.ALSD && 线段索引图 != null && 线段组 != null) {
            int i = 线段索引图.GetPixel(e.Location.X, e.Location.Y).ToArgb() & 0x00ffffff;
            if (i > 0) {
                线段信息 线 = 线段组[i];
                PointF 位置 = new(700, 10);
                图片框.Refresh();
                g图片框.DrawString("起点: " + 线.起 + " 终点: " + 线.终, 绘图.TNR字体(20), new SolidBrush(Color.YellowGreen), 位置);
            }
        }//有线段数据时,鼠标移动到线段上显示线段起终点数据.

        if (e.Button == MouseButtons.Right) {
            虚线框 = new Rectangle(Math.Min(按下起始点.X, Cursor.Position.X), Math.Min(按下起始点.Y, Cursor.Position.Y), Math.Abs(按下起始点.X - Cursor.Position.X), Math.Abs(按下起始点.Y - Cursor.Position.Y));
            ControlPaint.DrawReversibleFrame(上个虚线框, Color.Pink, FrameStyle.Dashed);
            ControlPaint.DrawReversibleFrame(虚线框, Color.Pink, FrameStyle.Dashed);
            上个虚线框 = 虚线框;
        }//按下右键拖动画虚线框

        if (e.Button == MouseButtons.Left && 画啥 == "不画") {
            if (缩放比例 <= 1) {

                g图片框.DrawLine(new Pen(图像.色谱颜色24[彩线计数++ % 图像.色谱颜色24.Length]), 上个点, e.Location);
                上个点 = e.Location;
                功能页.更新HSL上下限(局部放大控件.HSL上限, 局部放大控件.HSL下限);
            }//按下左键拖动时画彩线
            else {
                int dx = -(int)Math.Round((e.Location.X - 上个点.X) / 缩放比例);
                int dy = -(int)Math.Round((e.Location.Y - 上个点.Y) / 缩放比例);
                if (dx != 0 || dy != 0) { //非零时再刷
                    拖动中 = true;
                    刷新(dx, dy);
                    拖动中 = false;
                    上个点.X -= dx * (int)缩放比例;
                    上个点.Y -= dy * (int)缩放比例;  //这样赋值'上个点'画面才能跟紧鼠标
                }
            }//放大模式下按住左键可拖动画面
        }
        if (e.Button == MouseButtons.Left && 画啥 != "不画") {
            终点 = 原图坐标;
            Rectangle 略大框 = new Rectangle(矩形框.X - 参数.笔粗细, 矩形框.Y - 参数.笔粗细, 矩形框.Width + 1 + 参数.笔粗细 * 2, 矩形框.Height + 1 + 参数.笔粗细 * 2);//擦的框要比上次画的大一圈
            矩形框 = new Rectangle(Math.Min(按下点.X, 原图坐标.X), Math.Min(按下点.Y, 原图坐标.Y), Math.Abs(按下点.X - 原图坐标.X), Math.Abs(按下点.Y - 原图坐标.Y));

            Color 颜色 = 画啥 == "擦" ? Color.Black : Color.White;
            颜色 = 画啥 == "画框擦" ? Color.Gray : 颜色;
            Pen 笔 = new(颜色, 参数.笔粗细);
            Pen 笔擦框 = new(颜色, 1);

            if (画啥 == "点" || 画啥 == "擦") {
                g原图.DrawLine(笔, 起点, 终点);
                g原图.FillEllipse(new SolidBrush(颜色), 终点.X - 参数.笔粗细 / 2f, 终点.Y - 参数.笔粗细 / 2f, 参数.笔粗细, 参数.笔粗细);
            } else {
                g原图.DrawImage(位图副本, 略大框, 略大框, GraphicsUnit.Pixel); //把上次画的框擦掉,这个步骤是关键.
                if (画啥 == "线") {
                    g原图.DrawLine(笔, 按下点, 原图坐标);
                } else if (画啥 == "矩形") {
                    if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) //按下shfit画正方形
                        矩形框 = new Rectangle(矩形框.X, 矩形框.Y, Math.Max(矩形框.Width, 矩形框.Height), Math.Max(矩形框.Width, 矩形框.Height));
                    g原图.DrawRectangle(笔, 矩形框);
                    图数据.位图.SetPixel(矩形框.X + 矩形框.Width / 2, 矩形框.Y + 矩形框.Height / 2, Color.Red);
                } else if (画啥 == "圆") {
                    if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                        矩形框 = new Rectangle(矩形框.X, 矩形框.Y, Math.Max(矩形框.Width, 矩形框.Height), Math.Max(矩形框.Width, 矩形框.Height));
                    g原图.DrawEllipse(笔, 矩形框);
                    图数据.位图.SetPixel(矩形框.X + 矩形框.Width / 2, 矩形框.Y + 矩形框.Height / 2, Color.Red);
                } else if (画啥 == "画框擦")
                    g原图.DrawRectangle(笔擦框, 矩形框);
            }
            刷新();
            起点 = 终点;
        }//画东西

        上个原图坐标 = 原图坐标;
        Focus();                                     //让Tabpage获得焦点好让滚轮能够滚动页面
    }

    private void 图片框_MouseUp(object sender, MouseEventArgs e) {
        图片框.Refresh();
        选中区域 = new Rectangle(图片框.PointToClient(上个虚线框.Location), 上个虚线框.Size);
        选中区域 = new Rectangle((int)(选中区域.X / 缩放比例), (int)(选中区域.Y / 缩放比例), (int)(选中区域.Width / 缩放比例), (int)(选中区域.Height / 缩放比例));
        if (缩放比例 > 1) { 选中区域.X += 原图局部框.X; 选中区域.Y += 原图局部框.Y; }
        上个虚线框 = Rectangle.Empty;

        if (e.Button == MouseButtons.Left && 画啥 == "画框擦") {
            g原图.FillRectangle(Brushes.Black, 矩形框.X, 矩形框.Y, 矩形框.Width + 1, 矩形框.Height + 1);
            刷新();
        }
        Cursor.Clip = Rectangle.Empty;
    }

    private void 图片框_MouseDown(object sender, MouseEventArgs e) {
        按下起始点 = Cursor.Position;
        上个点 = e.Location;
        彩线计数 = 0;
        if (e.Button == MouseButtons.Right) {
            右键按下点 = new((int)(e.Location.X / 缩放比例), (int)(e.Location.Y / 缩放比例));   //这个e.Location是相对于图片框的坐标
            if (缩放比例 > 1) { 右键按下点.X += 原图局部框.X; 右键按下点.Y += 原图局部框.Y; }
        }

        if (e.Button == MouseButtons.Left && 画啥 != "不画") {
            原图坐标 = new((int)(e.Location.X / 缩放比例), (int)(e.Location.Y / 缩放比例));   //这个e.Location是相对于图片框的坐标
            if (缩放比例 > 1) { 原图坐标.X += 原图局部框.X; 原图坐标.Y += 原图局部框.Y; }
            起点 = 原图坐标;
            按下点 = 原图坐标;
            g位图副本.DrawImage(图数据.位图, 0, 0);
            Cursor.Clip = RectangleToScreen(new Rectangle(0, 0, Width, Height));//鼠标限制在图片框内
        }
    }

    #endregion

    #region 图片缩放

    public void 切换到此页() {
        if (缩放比例 <= 1)
            缩略图框.Visible = false;
        else
            显示缩略图();
    }
    public void 图片缩放(int 增量 = 0, bool 右键放大 = false) {
        if (右键放大 && 图像.点在框内(右键按下点, 图数据.位图)) {
            局部中心 = 右键按下点;
            if (缩放比例 < 1) 缩放比例 = 1;  //好直接进入放大状态
        }
        if (缩放比例 == 1) {
            if (增量 < 0) 调到合适大小(); //1比1时按减号就所小到合适
            if (增量 > 0) {
                缩放比例 += 增量;
                if (!右键放大) 获取局部中心点坐标();
                进入放大模式();
                刷新局部放大图();
                图片框.Refresh();
            }
        }
        else if (缩放比例 < 1) {
            if (增量 > 0) 调到1比1();     //缩小到合适窗口大小状态下按加号就回到1比1
        }
        else {                            //处于放大状态下
            缩放比例 += 增量;
            if (缩放比例 == 1) { 调到1比1(); return; }
            刷新局部放大图();
            图片框.Refresh();
        }
    }

    private void 图片页_Resize(object sender, EventArgs e) {
        if (缩放比例 < 1) 调到合适大小();
    }
    private void 调到合适大小() {
        if (图片框.Image.Width <= Size.Width && 图片框.Image.Height <= Size.Height) {
            图片框.SizeMode = PictureBoxSizeMode.AutoSize;
            缩放比例 = 1;
        }
        else {
            图片框.SizeMode = PictureBoxSizeMode.Zoom;
            AutoScrollPosition = new Point(0, 0); //这一句要写在下一句的前面，这两句是为了解决1:1模式下滚动条有滚动后再回到合适模式会产生的图片位置跑偏问题。
            图片框.Location = new Point(0, 0);
            double 图片宽长比 = (double)图片框.Image.Width / 图片框.Image.Height;
            double Tab宽长比 = (double)Size.Width / Size.Height;

            if (图片宽长比 >= Tab宽长比) {  //图片较宽
                int 高 = (int)(Size.Width / 图片宽长比 + 0.5);
                图片框.Size = new Size(Size.Width, 高);    //图片较宽时图片框的宽度会定为和Tab的宽度相同,这样图高不会超过Tab的高度.
            }
            else {
                int 宽 = (int)(Size.Height * 图片宽长比 + 0.5);
                图片框.Size = new Size(宽, Size.Height);
            }
            缩放比例 = (float)图片框.Width / 图片框.Image.Width;
            AutoScroll = false;
        }

    }//图片比显示区域大时,把图片缩小到适合显示区域的大小,此时没有滚动条,能看到完整图片.

    private void 调到1比1() {
        缩放比例 = 1;
        图片框.SizeMode = PictureBoxSizeMode.AutoSize;
        图片框.Image = 图数据.位图;
        AutoScroll = true;
        图片框.Refresh();
        缩略图框.Visible = false;
    }

    private void 进入放大模式() {
        AutoScroll = false;  //放大模式下也没有滚动条,靠按住鼠标左键来拖动画面
        放大图 ??= new(Width, Height, PixelFormat.Format24bppRgb);
        图片框.SizeMode = PictureBoxSizeMode.AutoSize;
        图片框.Image = 放大图;
        显示缩略图();
    }

    private void 显示缩略图() {
        缩略图框.Size = new((int)(缩略图框.Height * (图数据.位图.Width / (double)图数据.位图.Height)), 缩略图框.Height);
        缩略图框.Image = 图数据.位图;
        缩略图框.Visible = true;
        缩略图框.Refresh();
        缩略比 = 缩略图框.Width / (double)图数据.位图.Width;
    }

    private void 刷新局部放大图() {
        设置局部中心(局部中心.X, 局部中心.Y);
        Font 字体 = 绘图.TNR字体(9);
        using Graphics g = Graphics.FromImage(放大图);
        Rectangle 位图框 = new (0, 0, 图数据.位图.Width, 图数据.位图.Height);
        BitmapData 位图数据 = 图数据.位图.LockBits(位图框, ImageLockMode.ReadWrite, 图数据.位图.PixelFormat);
        byte 字节数 = 3;
        if (图数据.位图.PixelFormat == PixelFormat.Format32bppArgb) 字节数 = 4;

        if (参数.显示.黑) g.Clear(Color.Black);
        else {
            BitmapData 放大图数据 = 放大图.LockBits(new Rectangle(0, 0, 放大图.Width, 放大图.Height), ImageLockMode.ReadWrite, 放大图.PixelFormat);
            unsafe {
                byte* 原图指针 = (byte*)(位图数据.Scan0);
                byte* Nx位图指针 = (byte*)(放大图数据.Scan0);
                byte R = 0, G = 0, B = 0, 背景红 = BackColor.R, 背景绿 = BackColor.G, 背景蓝 = BackColor.B;//在下面的代码里直接用BackColor会很卡
                int N = (int)缩放比例;
                原图指针 += 位图数据.Stride * 原图局部框.Top;

                for (int 列 = 0; 列 < 放大图数据.Height; 列++, Nx位图指针 += 放大图数据.Stride - 放大图数据.Width * 3) {
                    for (int 行 = 0; 行 < 放大图数据.Width; 行++, Nx位图指针 += 3) {
                        if (行 >= 原图局部框.Right * N || 列 >= 原图局部框.Bottom * N || 行 / N + 原图局部框.Left >= 位图数据.Width || 列 / N + 原图局部框.Top >= 位图数据.Height) {
                            Nx位图指针[图像.红] = 背景红;
                            Nx位图指针[图像.绿] = 背景绿;
                            Nx位图指针[图像.蓝] = 背景蓝;
                            continue;
                        }
                        if (行 % N == 0) {
                            int index = (行 / N + 原图局部框.Left) * 字节数;
                            R = 原图指针[index + 图像.红]; G = 原图指针[index + 图像.绿]; B = 原图指针[index + 图像.蓝];
                        }
                        Nx位图指针[图像.红] = R;
                        Nx位图指针[图像.绿] = G;
                        Nx位图指针[图像.蓝] = B;
                    }
                    if (列 % N == N - 1)
                        原图指针 += 位图数据.Stride;
                }

            }//填充放大图

            放大图.UnlockBits(放大图数据);
        }
        图数据.位图.UnlockBits(位图数据);
        显示二值边线(g);
        if (显示网格) {
            Pen 网格笔 = new(Color.Gray);
            float[] 虚线样式 = new float[] { 1f, 2f };
            网格笔.DashPattern = 虚线样式;
            for (int y = 原图局部框.Top; y < 原图局部框.Bottom; y++)
                g.DrawLine(网格笔, 0, (y - 原图局部框.Top) * 缩放比例 - 1, 原图局部框.Width * 缩放比例 - 2, (y - 原图局部框.Top) * 缩放比例 - 1);
            for (int x = 原图局部框.Left; x < 原图局部框.Right; x++)
                g.DrawLine(网格笔, (x - 原图局部框.Left) * 缩放比例 - 1, 0, (x - 原图局部框.Left) * 缩放比例 - 1, 原图局部框.Height * 缩放比例 - 2);
        }

        Rectangle 缩略局部框 = new((int)(原图局部框.X * 缩略比), (int)(原图局部框.Y * 缩略比), (int)(原图局部框.Width * 缩略比 + 1), (int)(原图局部框.Height * 缩略比 + 1));
        using Graphics g缩略 = 缩略图框.CreateGraphics();
        缩略图框.Refresh();
        g缩略.DrawRectangle(new Pen(Color.Red), 缩略局部框);
        g缩略.DrawRectangle(new Pen(Color.Cyan), 缩略局部框.X - 1, 缩略局部框.Y - 1, 缩略局部框.Width + 2, 缩略局部框.Height + 2);

    }//只把显示区域能装下的原图局部拿出来放大显示了.

    private void 显示二值边线(Graphics g) {
        if (!参数.显示.ALSD) return;
        if (线段组 == null) { MessageBox.Show("此图还没加载线段数据!"); return; }
        线段索引图 ??= new(Width, Height, PixelFormat.Format32bppArgb);
        using Graphics g索引 = Graphics.FromImage(线段索引图);
        g索引.Clear(Color.Transparent);
        for (int i = 1; i < 线段组.Length; i++) {
            if (线段组[i].终 == Vector2.Zero && 线段组[i].起 == Vector2.Zero) break;
            if (!图像.点在框内(线段组[i].终, 原图局部框) && !图像.点在框内(线段组[i].起, 原图局部框)) continue;
            Color 颜色 = i % 2 == 0 ? Color.Red : Color.Green;
            PointF 起点 = new((线段组[i].起.X - 原图局部框.Left) * 缩放比例 + 缩放比例 / 2f, (线段组[i].起.Y - 原图局部框.Top) * 缩放比例 + 缩放比例 / 2f);
            PointF 终点 = new((线段组[i].终.X - 原图局部框.Left) * 缩放比例 + 缩放比例 / 2f, (线段组[i].终.Y - 原图局部框.Top) * 缩放比例 + 缩放比例 / 2f);
            g.DrawLine(new Pen(颜色, 3), 起点, 终点);
            g索引.DrawLine(new Pen(Color.FromArgb(255, Color.FromArgb(i)), 5), 起点, 终点);
        }


    }
    private void 获取局部中心点坐标() {
        int 右 = Math.Min(Size.Width, 图数据.位图.Width);
        int 下 = Math.Min(Size.Height, 图数据.位图.Height);//图片比显示区域小时,右下坐标取图片的右下点.
        局部中心 = new(右 / 2 + 1 - 图片框.Location.X, 下 / 2 + 1 - 图片框.Location.Y);
    }//没有点右键指定放大哪里时,如果图片小,没有出现滚动条,就把图片中心作为放大区域的中心;如果图片较大,就把显示区域的中心作为放大中心.

    public void 刷新(int dx = 0, int dy = 0) {
        if (缩放比例 > 1) {
            if ((dx != 0 || dy != 0) && !设置局部中心(局部中心.X + dx, 局部中心.Y + dy))
                return;
            刷新局部放大图();
        }
        图片框.Refresh();
    }

    private bool 设置局部中心(int x, int y) {//原图坐标
        if (放大图 == null) 放大图 = new(Width, Height, PixelFormat.Format24bppRgb);
        int 局部宽 = (int)(放大图.Width / 缩放比例), 局部高 = (int)(放大图.Height / 缩放比例); //放大图的大小就是显示区域的大小,就是this.Size,也就是Tab页的大小
        局部宽 = Math.Min(局部宽, 图数据.位图.Width) + 1;
        局部高 = Math.Min(局部高, 图数据.位图.Height) + 1;         //图片放大之后还比显示区域小的情况
        if (x - 局部宽 / 2 - 1 < 0) x = 局部宽 / 2 + 1;
        if (x + 局部宽 / 2 + 1 >= 图数据.位图.Width) x = 图数据.位图.Width - 局部宽 / 2 + 1;
        if (y - 局部高 / 2 - 1 < 0) y = 局部高 / 2 + 1;
        if (y + 局部高 / 2 + 1>= 图数据.位图.Height) y = 图数据.位图.Height - 局部高 / 2 + 1;
        bool 有改动 = 局部中心.X != x || 局部中心.Y != y;
        局部中心 = new Point(x, y);
        int 左上X = 局部中心.X - 局部宽 / 2 - 1, 左上Y = 局部中心.Y - 局部高 / 2 - 1;
        if (左上X < 0) 左上X = 0;
        if (左上Y < 0) 左上Y = 0;
        原图局部框 = new Rectangle(左上X, 左上Y, 局部宽-1, 局部高-1);
        return 有改动;
    }

    #endregion

    public void 关闭() {
        缩略图框.MouseMove -= 缩略图框_MouseMove;
        缩略图框.MouseClick -= 缩略图框_MouseMove;
        g原图.Dispose();
        g位图副本.Dispose();
        Dispose();
        GC.Collect();
    }
}

