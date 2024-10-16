using System.Drawing.Imaging;

namespace 控件;
public class Mnist查看标签页 : TabPage, I标签页 {
    public static string Mnist路径 = @"C:\"; //改为Mnist数据所在文件夹
    public static string 训练图片名 = @"train-images.idx3-ubyte";//文件名可能也要改
    public static string 训练标签名 = @"train-labels.idx1-ubyte";
    public static string 测试图片名 = @"t10k-images.idx3-ubyte";
    public static string 测试标签名 = @"t10k-labels.idx1-ubyte";
    private const int 透 = 3, 红 = 2, 绿 = 1, 蓝 = 0; //Bitmap位图数据中,蓝色Blue的值存在低地址上
    static int 窗体宽 = 1600;
    static int 窗体高 = 900;
    int W;  //一行查看多少个手写数字
    int H;  //多少行
    int N;       //右边大图能有多少张图片
    static int MnistSize = 28; //Mnist字符宽度
    int 放大倍数 = 2;
    static int 边框宽度 = 1;
    int 图尺寸;
    public static int 训练图片数 = 60000;
    public static int 测试图片数 = 10000;
    static byte[] 训练数据;
    static byte[] 测试数据;
    public PictureBox 查看图片框;
    PictureBox 放大图片框;
    PictureBox 移动显示图片框;
    public Bitmap 查看图片;
    public Bitmap 放大图片 = new Bitmap(MnistSize * 11, MnistSize * 11, PixelFormat.Format24bppRgb);
    Bitmap 移显图片 = new Bitmap(MnistSize * 11, MnistSize * 11, PixelFormat.Format24bppRgb);
    public Bitmap 原数字图 = new Bitmap(MnistSize, MnistSize, PixelFormat.Format24bppRgb);
    查看设置 设置;
    static List<int>[] 训练数字标签组;
    static List<int>[] 测试数字标签组;
    static byte[] 训练标签;
    static byte[] 测试标签;
    int[] 序号;             //用来存放要读图片的序号
    List<int> 错误序号;
    byte[] 错误标签;
    bool 当前图片是训练集 = true;
    int 当前选中序号 = 0;
    int 当前划过序号 = -1;
    Rectangle 当前选中框 = Rectangle.Empty;


    public Mnist查看标签页() {
        Text = "Mnist查看";
        移动显示图片框 = new PictureBox() {
            SizeMode = PictureBoxSizeMode.AutoSize,
            Location = new Point(0, 0)
        };
        Controls.Add(移动显示图片框);
        查看图片框 = new PictureBox() {
            SizeMode = PictureBoxSizeMode.AutoSize,
            Location = new Point(310, 0)
        };
        Controls.Add(查看图片框);
        放大图片框 = new PictureBox() {
            SizeMode = PictureBoxSizeMode.AutoSize,
            Location = new Point(0, 310)
        };
        Controls.Add(放大图片框);
        设置 = new 查看设置() {
            Location = new Point(0, 620)
        };
        Controls.Add(设置);
        设置.读之按钮.Click += 读之按钮_Click;
        设置.radioButton测试集.CheckedChanged += RadioButton测试集_CheckedChanged;
        查看图片框.MouseMove += 查看图片框_MouseMove;
        查看图片框.MouseClick += 查看图片框_MouseClick;
        放大图片框.MouseMove += 放大图片框_MouseMove;
        设置.comboBox读哪个数.SelectedIndexChanged += ComboBox读哪个数_SelectedIndexChanged;
        设置.上一页.Click += 上一页_Click;
        更新每个数字图片的数目();
        读入Mnist数据();

    }
    public Mnist查看标签页(string 文件名) : this() {
        string 头两字 = Path.GetFileName(文件名).Substring(0, 2);
        if (头两字 == "训练") {
            当前图片是训练集 = true;
            Text = "训练集推理错误";
            设置.radioButton训练集.Checked = true;
        }
        else if (头两字 == "测试") {
            当前图片是训练集 = false;
            Text = "测试集推理错误";
            设置.radioButton测试集.Checked = true;
        }
        else {
            MessageBox.Show("请在文件名最前面标注是'训练集'还是‘测试集’");
        }
        if (获取错误图片信息(文件名)) {
            读之(true);
        }
        设置.comboBox放大倍数.Enabled = false;
        设置.radioButton训练集.Enabled = false;
        设置.radioButton测试集.Enabled = false;
    }

    private void 读入Mnist数据() {
        if (训练数据 != null) return;
        if (!(File.Exists(Mnist路径 + 训练标签名) && File.Exists(Mnist路径 + 测试标签名))) {
            Mnist路径 = 文件.打开文件夹("请指定Mnist数据集文件所在目录:") + @"\";
        }
        加载数据();     //把Mnist图像数据都加载到字节数组里
        初始化标签组();
    }

    private void 上一页_Click(object sender, EventArgs e) {
        int 起始位置 = Convert.ToInt32(设置.textBox起始位置.Text);
        //if (N == null) return;
        起始位置 -= N;
        if (起始位置 >= N)
            起始位置 -= N;
        else
            起始位置 = 0;
        设置.textBox起始位置.Text = 起始位置.ToString();
        读之(错误序号 != null);
    }

    private void ComboBox读哪个数_SelectedIndexChanged(object sender, EventArgs e) {
        设置.textBox起始位置.Text = "0";
    }


    private bool 获取错误图片信息(string 文件名) {
        错误标签 = new byte[训练图片数];
        错误序号 = new List<int>();
        try {
            using (StreamReader 读文件 = new StreamReader(文件名)) {
                string 一行;
                while ((一行 = 读文件.ReadLine()) != null) {
                    string[] 三个数 = 一行.Split(new char[] { ' ' });
                    int i = Convert.ToInt32(三个数[0]);
                    错误序号.Add(i);
                    错误标签[i] = (byte)Convert.ToInt32(三个数[1]);
                }
            }
        }
        catch (Exception e) {
            MessageBox.Show("读文件出错：" + 文件名);
            return false;
        }
        if (错误序号.Count > 0)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 把Mnist数据加载到字节数组中
    /// </summary>
    static void 加载数据() {
        if (!(File.Exists(Mnist路径 + 训练图片名) && File.Exists(Mnist路径 + 测试图片名))) {
            MessageBox.Show("没找到以下Mnist数据文件(可能要修改'Mnist查看标签页.cs'代码里的文件名): " + 训练图片名 + "\n" + 测试图片名);
            return;
        }
        FileStream 文件流 = new (Mnist路径 + 训练图片名, FileMode.Open);
        训练数据 = new byte[文件流.Length];
        文件流.Read(训练数据, 0, 训练数据.Length);
        文件流.Close();
        文件流 = new (Mnist路径 + 测试图片名, FileMode.Open);
        测试数据 = new byte[文件流.Length];
        文件流.Read(测试数据, 0, 测试数据.Length);
        文件流.Close();
    }

    private void 放大图片框_MouseMove(object sender, MouseEventArgs e) {
        if (序号 == null) return;
        Point 坐标 = 放大图片框.PointToClient(Cursor.Position);
        if (坐标.X < 0 || 坐标.Y < 0 || 坐标.X >= 查看图片框.Image.Width || 坐标.Y >= 查看图片框.Image.Height) return;
        设置.label灰度值.Text = 放大图片.GetPixel(坐标.X, 坐标.Y).G.ToString(); //显示鼠标所在点的灰度值
    }

    private void 查看图片框_MouseClick(object sender, MouseEventArgs e) {
        if (查看图片框.Image == null) return;
        Point 坐标 = 查看图片框.PointToClient(Cursor.Position);
        int x序号 = 坐标.X / 图尺寸;
        int y序号 = 坐标.Y / 图尺寸;
        int 图序号 = y序号 * W + x序号;
        if (序号 == null || 图序号 < 0 || 图序号 >= 序号.Length || 查看图片框.Image == null || 图序号 == 当前选中序号) return;
        画选中框(x序号, y序号, 图序号);
        画放大图(图序号, 放大图片);
        放大图片框.Refresh();
    }
    private void 画选中框(int x, int y, int 图序号) {
        Rectangle 选中框 = new Rectangle(x * 图尺寸, y * 图尺寸, 图尺寸, 图尺寸);
        Graphics g = Graphics.FromImage(查看图片框.Image);
        if (当前选中框 != Rectangle.Empty)
            g.DrawRectangle(new Pen(Color.FromArgb(128, 128, 128)), 当前选中框);
        g.DrawRectangle(new Pen(Color.LightGreen), 选中框);
        查看图片框.Invalidate(new Rectangle(选中框.X, 选中框.Y, 选中框.Width + 1, 选中框.Height + 1));
        查看图片框.Invalidate(new Rectangle(当前选中框.X, 当前选中框.Y, 当前选中框.Width + 1, 当前选中框.Height + 1));
        当前选中框 = 选中框;
        当前选中序号 = 图序号;
        g.Dispose();
    }

    private void 查看图片框_MouseMove(object sender, MouseEventArgs e) {
        if (查看图片框.Image == null) return;
        Point 坐标 = 查看图片框.PointToClient(Cursor.Position);
        int x序号 = 坐标.X / 图尺寸;
        int y序号 = 坐标.Y / 图尺寸;
        int 图序号 = y序号 * W + x序号;
        if (序号 == null || 图序号 < 0 || 图序号 >= 序号.Length) return;
        if (!当前图片是训练集) {
            设置.label测试序号.Text = 序号[图序号].ToString();
            设置.label训练序号.Text = "";
        }
        else {
            设置.label训练序号.Text = 序号[图序号].ToString();
            设置.label测试序号.Text = "";
        }
        if (图序号 != 当前划过序号) {
            画放大图(图序号, 移显图片);
            移动显示图片框.Image = 移显图片;
            当前划过序号 = 图序号;
        }

    }

    private void RadioButton测试集_CheckedChanged(object sender, EventArgs e) {
        更新每个数字图片的数目();
    }

    private void 读之(bool 读错误 = false) {
        byte[] 数据 = 训练数据;
        byte[] 标签 = 训练标签;
        int 图片个数 = 训练图片数;
        当前图片是训练集 = true;
        List<int>[] 标签组 = 训练数字标签组;
        if (设置.radioButton测试集.Checked) {
            图片个数 = 测试图片数;
            标签组 = 测试数字标签组;
            数据 = 测试数据;
            标签 = 测试标签;
            当前图片是训练集 = false;
        }


        放大倍数 = 设置.comboBox放大倍数.SelectedIndex + 1;
        图尺寸 = MnistSize * 放大倍数 + 边框宽度;
        W = 窗体宽 / 图尺寸;
        H = 窗体高 / 图尺寸;
        N = W * H;             //算出右边的大图能显示多少个数字
        if (查看图片 == null)
            查看图片 = new Bitmap(W * 图尺寸 + 1, H * 图尺寸 + 1, PixelFormat.Format32bppArgb);

        int 读哪个数 = 设置.comboBox读哪个数.SelectedIndex;
        int 起始位置 = Convert.ToInt32(设置.textBox起始位置.Text);
        if (读错误) {
            if (错误序号.Count <= N && 查看图片框.Image != null)
                return;

            int 读取数目 = N;
            if (错误序号.Count - 起始位置 < N)
                读取数目 = 错误序号.Count - 起始位置;
            if (读取数目 <= 0) return;
            序号 = 错误序号.GetRange(起始位置, 读取数目).ToArray();
            设置.textBox起始位置.Text = (起始位置 + 读取数目).ToString();
        }
        else if (设置.radioButton顺序读.Checked) {
            序号 = new int[N];
            for (int i = 0; i < 序号.Length; i++) {
                序号[i] = i + 起始位置;
                if (读哪个数 < 10) {
                    序号[i] = 序号[i] >= 标签组[读哪个数].Count ? 序号[i] % 标签组[读哪个数].Count : 序号[i];
                    if (i == 序号.Length - 1)
                        设置.textBox起始位置.Text = (序号[i] + 1).ToString();
                    序号[i] = 标签组[读哪个数][序号[i]];
                }
                else {
                    序号[i] = 序号[i] >= 图片个数 ? 序号[i] % 图片个数 : 序号[i];
                    if (i == 序号.Length - 1)
                        设置.textBox起始位置.Text = (序号[i] + 1).ToString();
                }
            }

        }
        else if (设置.radioButton随机读.Checked) {
            序号 = new int[N];
            if (读哪个数 < 10) {
                图片个数 = 标签组[读哪个数].Count;
                产生随机序号(序号, 图片个数);
                for (int i = 0; i < 序号.Length; i++)
                    序号[i] = 标签组[读哪个数][序号[i]];
            }
            else
                产生随机序号(序号, 图片个数);
        }


        读Mnist到大图片(数据, 标签, 序号);
        查看图片框.Image = 查看图片;
        当前选中序号 = 0;
        当前选中框 = Rectangle.Empty;
        画选中框(0, 0, 0);
        画放大图(0, 放大图片);
        放大图片框.Image = 放大图片;
        当前划过序号 = -1;
    }

    private void 读之按钮_Click(object sender, EventArgs e) {
        ((Button)sender).Enabled = false;
        读之(错误序号 != null);
        ((Button)sender).Enabled = true;
    }

    private void 读Mnist到大图片(byte[] 数据, byte[] 标签, int[] 序号) {
        BitmapData 图数据 = 查看图片.LockBits(new Rectangle(0, 0, 查看图片.Width, 查看图片.Height), ImageLockMode.ReadWrite, 查看图片.PixelFormat);
        byte 灰度值 = 0;
        int 数据索引 = 0;
        unsafe {
            byte* 图指针 = (byte*)(图数据.Scan0);
            for (int y = 0; y < 图数据.Height; y++)  //这里必须用"位图数据.Height",不小心写成"位图.Height"就会导致程序莫名变慢很多.
                for (int x = 0; x < 图数据.Width; x++, 图指针 += 4) {

                    if (设置.显示边框.Checked && (x == 图数据.Width - 1 || y == 图数据.Height - 1 || x % 图尺寸 == 0 || y % 图尺寸 == 0)) {
                        图指针[蓝] = 图指针[绿] = 图指针[红] = 128;
                        图指针[透] = 255;
                    }//画边框
                    else {
                        int x序号 = x / 图尺寸;                        //某一行的第几个图
                        int y序号 = y / 图尺寸;                        //第几行
                        int 图序号 = y序号 * W + x序号;                //序号数组中的第几个
                        int 图X = (x - x序号 * 图尺寸 - 边框宽度) / 放大倍数; //数字图片上的X坐标
                        int 图Y = (y - y序号 * 图尺寸 - 边框宽度) / 放大倍数;
                        if ((x - x序号 * 图尺寸 - 边框宽度) % 放大倍数 == 0 && 图序号 < 序号.Length) {
                            if (图X == 0)
                                数据索引 = 4 * 4 + 序号[图序号] * MnistSize * MnistSize + 图Y * MnistSize + 图X;
                            灰度值 = 数据[数据索引];
                            数据索引++;
                        }
                        图指针[蓝] = 图指针[绿] = 图指针[红] = 灰度值;
                        图指针[透] = 255;
                    }
                }
        }

        查看图片.UnlockBits(图数据);
        Graphics g = Graphics.FromImage(查看图片);
        if (设置.显示标签.Checked) {
            for (int i = 0; i < 序号.Length; i++) {
                g.DrawString(标签[序号[i]].ToString(), TimesNR字体(7 * 放大倍数), new SolidBrush(Color.LightGreen), new Point(i % W * 图尺寸 - 1, i / W * 图尺寸 - 1));
                if (错误标签 != null)
                    g.DrawString(错误标签[序号[i]].ToString(), TimesNR字体(7 * 放大倍数), new SolidBrush(Color.Red), new Point(i % W * 图尺寸 - 1, i / W * 图尺寸 + 7 * 放大倍数));
            }//画标签
        }
        g.Dispose();
    }

    /// <summary>
    /// 用于鼠标在右边图片上移动时，在左边显示放大的数字图
    /// </summary>
    /// <param name="图号"></param>
    /// <param name="大图片"></param>
    private void 画放大图(int 图号, Bitmap 大图片) {
        byte[] 数据 = 训练数据;
        byte[] 标签 = 训练标签;
        if (!当前图片是训练集) {
            数据 = 测试数据;
            标签 = 测试标签;
        }
        int j = 4 * 4 + 序号[图号] * MnistSize * MnistSize;
        BitmapData 图数据 = 原数字图.LockBits(new Rectangle(0, 0, 原数字图.Width, 原数字图.Height), ImageLockMode.ReadWrite, 原数字图.PixelFormat);
        unsafe {
            byte* 图指针 = (byte*)(图数据.Scan0);
            for (int y = 0; y < 图数据.Height; y++) {
                for (int x = 0; x < 图数据.Width; x++, 图指针 += 3) {
                    图指针[蓝] = 图指针[绿] = 图指针[红] = 数据[j];
                    j++;
                }
                图指针 += 图数据.Stride - 图数据.Width * 3;
            }
        }
        原数字图.UnlockBits(图数据);
        图像.Nx位图(原数字图, 11, false, 大图片);
        Graphics g = Graphics.FromImage(大图片);
        g.DrawString(标签[序号[图号]].ToString(), TimesNR字体(24), new SolidBrush(Color.LightGreen), new Point(0, 0));
        if (错误标签 != null)
            g.DrawString(错误标签[序号[图号]].ToString(), TimesNR字体(24), new SolidBrush(Color.Red), new Point(0, 24));
        g.Dispose();
    }

    /// <summary>
    /// 把“0”的图片、“1”的图片...的下标都找出来分别存到10个列表中
    /// </summary>
    static void 初始化标签组() {
        if (!(File.Exists(Mnist路径 + 训练标签名) && File.Exists(Mnist路径 + 测试标签名))) {
            MessageBox.Show("没找到以下Mnist数据文件(可能要修改'Mnist查看标签页.cs'代码里的文件名): " + 训练标签名 + "\n" + 测试标签名);
            return;
        }
        if (训练数字标签组 != null) return;
        训练数字标签组 = new List<int>[10];
        测试数字标签组 = new List<int>[10];
        训练标签 = new byte[训练图片数];
        测试标签 = new byte[测试图片数];
        for (int i = 0; i < 训练数字标签组.Length; i++) {
            训练数字标签组[i] = new List<int>(7000);
            测试数字标签组[i] = new List<int>(1200);
        }
        using FileStream 训练标签流 = new FileStream(Mnist路径 + 训练标签名, FileMode.Open);
        using FileStream 测试标签流 = new FileStream(Mnist路径 + 测试标签名, FileMode.Open);
        训练标签流.Seek(8, SeekOrigin.Begin);
        训练标签流.Read(训练标签, 0, 训练图片数);
        测试标签流.Seek(8, SeekOrigin.Begin);
        测试标签流.Read(测试标签, 0, 测试图片数);
        for (int i = 0; i < 训练图片数; i++)
            训练数字标签组[(int)训练标签[i]].Add(i);
        for (int i = 0; i < 测试图片数; i++)
            测试数字标签组[(int)测试标签[i]].Add(i);
    }

    private void 更新每个数字图片的数目() {
        if (训练数字标签组 == null) return;
        List<int>[] 标签组 = 训练数字标签组;
        int 图片数 = 训练图片数;
        if (设置.radioButton测试集.Checked) {
            标签组 = 测试数字标签组;
            图片数 = 测试图片数;
        }
        for (int i = 0; i < 10; i++)
            设置.comboBox读哪个数.Items[i] = "只读" + i.ToString() + " " + 标签组[i].Count.ToString();
        设置.comboBox读哪个数.Items[10] = "全部" + " " + 图片数.ToString();

    }

    static public void 产生随机序号(int[] 序号数组, int 序号最大值, int 种子 = -1) {
        Random 随机 = new Random();
        if (种子 > 0)
            随机 = new Random(种子);
        序号数组[0] = 随机.Next(序号最大值);
        for (int i = 1; i < 序号数组.Length; i++) {
            bool 和之前相等 = true;
            while (和之前相等) {
                序号数组[i] = 随机.Next(序号最大值);
                for (int j = 0; j < i; j++)
                    if (序号数组[i] == 序号数组[j]) {
                        和之前相等 = true;
                        break;
                    }
                    else
                        和之前相等 = false;
            }
        }
        Array.Sort(序号数组);
    }

    public void 关闭() {
        Dispose();
    }
    static public Font TimesNR字体(int 字号) {
        return new Font(new FontFamily("Times New Roman"), 字号);
    }
}

