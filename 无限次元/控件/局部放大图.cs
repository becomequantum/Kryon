namespace 控件;
public partial class 局部放大图 : UserControl {
    const int 放大图边长 = 128;
    Bitmap 位图 = new Bitmap(放大图边长, 放大图边长, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
    Graphics g位图;
    public Color HSL上限 = Color.FromArgb(0, 0, 0);
    public Color HSL下限 = Color.FromArgb(255, 255, 255);
    public 局部放大图() {
        InitializeComponent();
        初始化控件();
    }

    private void 初始化控件() {
        g位图 = Graphics.FromImage(位图);
        图片框.Image = 位图;
        画青蛙();
    }

    public void 画青蛙() {
        string 青蛙 = "0000111111100000000000011111110000" +
                      "0000100000000000000000000000010000" +
                      "0000101111100001111000011111010000" +
                      "0000101000101110000111010001010000" +
                      "0000101010100000000000010101010000" +
                      "0000101110100000000000010111010000" +
                      "0000100000100000000000010000010000" +
                      "0000111111100000000000011111110000" +
                      "0010000000000000000000000000000100" +
                      "0100000000000000000000000000000010" +
                      "0100000000000000000000000000000010" +
                      "1000000000000001001000000000000001" +
                      "1000000000000000000000000000000001" +
                      "0100000000000000000000000000000010" +
                      "0100000000100000000000010000000010" +
                      "0010000000011000000001100000000100" +
                      "0001000000001110000111000000001000" +
                      "0000110000000011111100000000110000" +
                      "0000001100000000000000000011000000" +
                      "0000000011110000000000111100000000" +
                      "0000000000001111111111000000000000";
        g位图.Clear(SystemColors.Control);
        int r = 3;
        for (int y = 0; y < 21; y++)
            for (int x = 0; x < 34; x++)
                if (青蛙[y * 34 + x] == '1')
                    g位图.FillRectangle(new SolidBrush(Color.Green), x * r + 3 * r, y * r + 20, r, r);
        图片框.Refresh();
    }

    public void 重置HSL上下限() {
        HSL上限 = Color.FromArgb(0, 0, 0);
        HSL下限 = Color.FromArgb(255, 255, 255);
    }//点回到原图时会重置

    public void 显示像素信息(Bitmap 位图, Point 坐标, bool 统计上下限 = false) {
        if (!图像.点在框内(坐标, 位图)) return; 
        try {
            画局部放大(位图, 坐标);
            int X = 坐标.X, Y = 坐标.Y;
            X坐标.Text = X.ToString();
            Y坐标.Text = Y.ToString();
            Color 颜色 = 位图.GetPixel(X, Y);
            红.Text = 颜色.R.ToString();
            绿.Text = 颜色.G.ToString();
            蓝.Text = 颜色.B.ToString();
            int H = 0, S = 0, L = 0;
            图像.RGB转HSL(颜色.R, 颜色.G, 颜色.B, ref H, ref S, ref L);
            int R = 0, G = 0, B = 0;
            图像.HSL转RGB(H, 240, 90, ref R, ref G, ref B);
            色.ForeColor = Color.FromArgb(R, G, B);
            色.Text = H.ToString();
            饱.Text = S.ToString();
            亮.Text = L.ToString();
            if (统计上下限 && !(L == 0)) {
                HSL上限 = Color.FromArgb(H > HSL上限.R ? H : HSL上限.R, S > HSL上限.G ? S : HSL上限.G, L > HSL上限.B ? L : HSL上限.B);
                HSL下限 = Color.FromArgb(H < HSL下限.R ? H : HSL下限.R, S < HSL下限.G ? S : HSL下限.G, L < HSL下限.B ? L : HSL下限.B);
            }//按住鼠标左键移动鼠标时统计鼠标所经过点HSL值的范围.
        }
        catch (Exception) {
        }
    }//当鼠标的图片上移动时,显示鼠标指向点的坐标, RGB值, HSL值,局部放大等信息

    private void 画局部放大(Bitmap 位图, Point 坐标) {
        int 局部直径 = 128 / 放大倍数.Value;
        int 格子直径 = 放大倍数.Value;
        int 方块直径 = 格子直径;
        if (放大倍数.Value > 6 && 放大倍数.Value % 2 == 1) //放大倍数为大于6的奇数时显示网格线
            方块直径 -= 1;
        g位图.Clear(SystemColors.Control);
        for (int y = 0; y < 局部直径; y++) 
            for (int x = 0; x < 局部直径; x++) {
                int X = 坐标.X + x - 局部直径 / 2, Y = 坐标.Y + y - 局部直径 / 2;
                if (X >= 0 && Y >= 0 && X < 位图.Width && Y < 位图.Height)
                    g位图.FillRectangle(new SolidBrush(位图.GetPixel(X, Y)), x * 格子直径, y * 格子直径, 方块直径, 方块直径);
                else
                    g位图.FillRectangle(new SolidBrush(SystemColors.Control), x * 格子直径, y * 格子直径, 格子直径, 格子直径);
            }
        if (局部直径 % 2 == 1) {
            int 中心点位置 = 局部直径 / 2 * 格子直径 - 1;
            Rectangle 中心框 = new Rectangle(中心点位置, 中心点位置, 格子直径, 格子直径);
            g位图.DrawRectangle(new Pen(Color.Red), 中心框);  //在鼠标所在的中心像素点上画个红框
        }
        图片框.Refresh();
    }

    private void 局部放大图_MouseEnter(object sender, EventArgs e) {
        X坐标.Text = "X";
        Y坐标.Text = "Y";
        红.Text = "红";
        绿.Text = "绿";
        蓝.Text = "蓝";
        色.Text = "色度";
        饱.Text = "饱和";
        亮.Text = "亮度";
        画青蛙();
    }

    private void 放大倍数_Scroll(object sender, EventArgs e) {
        倍数.Text = 放大倍数.Value.ToString();
        
    }
}

