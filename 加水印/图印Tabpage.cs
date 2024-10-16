
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace 加水印 {
    class 图印Tabpage : TabPage {
        public PictureBox 图片框;
        public Bitmap 原图;
        public Bitmap 原图副本;
        public Bitmap 水印图;
        Bitmap HSI位图;
        public TabPage 图印设置页;
        bool 鼠标按下 = false;
        bool 右键按下 = false;
        Point 坐标;
        颜色范围 范围;
        int[,] 坐标数组;
        float 比例 = 1;
        private Rectangle 虚线框 = Rectangle.Empty;
        private Rectangle 选中区域 = Rectangle.Empty;
        bool Ctrl按下 = false;

        public 图印Tabpage(string 文件名, TabPage 图印页, ContextMenuStrip 右键菜单) {
            AutoScroll = true;
            图片框 = new PictureBox();
            图片框.BorderStyle = BorderStyle.FixedSingle;
            图片框.SizeMode = PictureBoxSizeMode.AutoSize;
            Controls.Add(图片框);
            初始化原图(加载图片文件(文件名));
            图印设置页 = 图印页;
            Text = "图印:" + Path.GetFileName(文件名);
            图片框.MouseDown += 图片框_MouseDown;
            图片框.MouseUp += 图片框_MouseUp;
            图片框.MouseMove += 图片框_MouseMove;
            ContextMenuStrip = 右键菜单;
            右键菜单.Items["变透明ToolStripMenuItem"].Click += 右键菜单变透明_Click;
            右键菜单.Items["取消透明ToolStripMenuItem"].Click += 右键菜单变透明_Click;
        }



        private void 右键菜单变透明_Click(object sender, EventArgs e) {
            byte 透明度 = 255;
            if (((ToolStripMenuItem)sender).Name == "变透明ToolStripMenuItem") 透明度 = 0;
            if (选中区域 != Rectangle.Empty) {
                if (选中区域.Width < 0) {
                    选中区域.X += 选中区域.Width;
                    选中区域.Width = -选中区域.Width;
                }
                if (选中区域.Height < 0) {
                    选中区域.Y += 选中区域.Height;
                    选中区域.Height = -选中区域.Height;
                }
                选中区域.Width = (int)(选中区域.Width * 比例 + 1) + 1;
                选中区域.Height = (int)(选中区域.Height * 比例 + 1) + 1;
                选中区域.X = (int)(选中区域.X * 比例);
                选中区域.Y = (int)(选中区域.Y * 比例);
                图像.画框调透明度(原图, 选中区域, 透明度);
            }
            选中区域 = Rectangle.Empty;
            图片框.Refresh();
        }

        private void 图片框_MouseMove(object sender, MouseEventArgs e) {

            if (右键按下) ResizeToRectangle(new Point(e.X, e.Y));
        }

        public void 回退变透明() {
            if (HSI位图 == null || 范围 == null || 原图副本 == null) return;
            范围.范围减();
            图像.原图刷新图32(原图副本, 原图);
            图像.着透明色32(坐标数组, (int)(坐标.X * 比例), (int)(坐标.Y * 比例), 原图, HSI位图, 范围);
            图片框.Refresh();
        }

        private void 图片框_MouseUp(object sender, MouseEventArgs e) {
            鼠标按下 = false;
            if (右键按下) {
                Cursor.Clip = Rectangle.Empty;//释放对鼠标移动区域的限制
                选中区域 = 虚线框;
            }
            右键按下 = false;
            
        }

        private void 图片框_MouseDown(object sender, MouseEventArgs e) {
            鼠标按下 = true;
            if (e.Button == MouseButtons.Right) 右键按下 = true;
            坐标 = 图片框.PointToClient(Cursor.Position);
            if (e.Button == MouseButtons.Left && Ctrl按下) {
                if (坐标数组 == null)
                    坐标数组 = new int[原图.Width * 原图.Height, 2];
                if (原图副本 == null)
                    原图副本 = new Bitmap(原图.Width, 原图.Height, 原图.PixelFormat);
                if (HSI位图 == null)
                    HSI位图 = 图像.生成HSI位图32(原图);
                try {
                    图像.原图刷新图32(原图, 原图副本);
                    范围 = new 颜色范围();
                    ThreadStart 变透明色 = new ThreadStart(变透明);
                    Thread 变透明线程 = new Thread(变透明色);
                    变透明线程.Start();
                }
                catch (Exception) {
                }
            }
            if (!Ctrl按下 && e.Button == MouseButtons.Left) {//单击清空虚线框
                图片框.Refresh();
                选中区域 = Rectangle.Empty;
            }
            if (右键按下)
                DrawStart(new Point(e.X, e.Y));
        }

        private void 变透明() {
            while (鼠标按下) {
                try {
                    图像.原图刷新图32(原图副本, 原图);
                    图像.着透明色32(坐标数组, (int)(坐标.X * 比例), (int)(坐标.Y * 比例), 原图, HSI位图, 范围);
                    范围.范围加();
                    lock (图片框) {
                        图片框.Refresh();
                    }
                }
                catch (Exception) {
                }
            }
        }

        public void 重新加载图片(string 文件名) {
            初始化原图(加载图片文件(文件名));
            Text = "图印:" + Path.GetFileName(文件名);
            图片框.Refresh();
            原图副本 = null;
            HSI位图 = null;
            鼠标按下 = false;
            坐标数组 = null;
        }

        private Bitmap 加载图片文件(string 图片文件名) { //这样加载图片后，原来的图片文件不会被占用锁定
            try {
                FileStream 文件流 = new FileStream(图片文件名, FileMode.Open, FileAccess.Read);
                Bitmap tmpImg = (Bitmap)Image.FromStream(文件流);
                文件流.Close();
                文件流.Dispose();
                return tmpImg;
            }
            catch (Exception exp) {
                MessageBox.Show(exp.Message);
                return null;
            }
        }

        private void 初始化原图(Bitmap 图片) {
            原图 = new Bitmap(图片.Width, 图片.Height, PixelFormat.Format32bppArgb);//要用这个Argb而不是PArgb.
            水印图 = new Bitmap(图片.Width, 图片.Height, PixelFormat.Format32bppArgb);

            if (图片.PixelFormat == PixelFormat.Format24bppRgb)
                图像.原图24刷新图32(图片, 原图);
            else if (图片.PixelFormat == PixelFormat.Format32bppPArgb || 图片.PixelFormat == PixelFormat.Format32bppArgb || 图片.PixelFormat == PixelFormat.Format32bppRgb)
                图像.原图刷新图32(图片, 原图);
            else {
                Graphics g = Graphics.FromImage(原图);
                g.DrawImage(图片, 0, 0);
                g.Dispose();
            }
            图片框.Image = 原图;
            图像.原图刷新图32(原图, 水印图);

        }
        public void 图片调到合适大小() {
            if (图片框.Image.Width <= Size.Width && 图片框.Image.Height <= Size.Height) {
                图片框.SizeMode = PictureBoxSizeMode.AutoSize;
                比例 = 1;
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
                比例 = (float)图片框.Image.Width / 图片框.Width;
                AutoScroll = false;
            }

        }

        public void 图片调到原始大小() {
            图片框.SizeMode = PictureBoxSizeMode.AutoSize;
            AutoScroll = true;
            比例 = 1;
        }


        public void 关闭() {
            ((TabControl)Parent).TabPages.Remove(this);
            Dispose();
        }

        private void ResizeToRectangle(Point p_Point) {
            DrawRectangle();
            虚线框.Width = p_Point.X - 虚线框.Left;
            虚线框.Height = p_Point.Y - 虚线框.Top;
            DrawRectangle();
        }

        private void DrawRectangle() {
            Rectangle _Rect = 图片框.RectangleToScreen(虚线框);
            ControlPaint.DrawReversibleFrame(_Rect, Color.White, FrameStyle.Dashed);
        }
        private void DrawStart(Point p_Point) {
            Cursor.Clip = 图片框.RectangleToScreen(new Rectangle(0, 0, 图片框.Width, 图片框.Height));
            虚线框 = new Rectangle(p_Point.X, p_Point.Y, 0, 0);
        }

        public void Ctrl状态(bool 按下){
            Ctrl按下 = 按下;
            Console.WriteLine(Ctrl按下.ToString());
        }
    }

    
}
