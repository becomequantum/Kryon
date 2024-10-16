using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace 加水印 {
    class 图片Tabpage : TabPage{
        public PictureBox 图片框;
        public Bitmap 原图;
        public Bitmap 水印图;
        float 比例 = 1;
        public bool 是文件夹 = false;
        public string 路径;
        public List<string> 所有图片列表 = new List<string>();
        public List<string> 子目录列表 = new List<string>();
        int 当前图片序号 = 0;

        public 图片Tabpage(Bitmap 打开的图片, string 文件名, MouseEventHandler 图片框_MouseMove, MouseEventHandler 图片框_MouseDown, MouseEventHandler 图片框_MouseUp) {
            图片框 = new PictureBox();
            图片框.SizeMode = PictureBoxSizeMode.AutoSize;
            图片框.MouseMove += 图片框_MouseMove;
            图片框.MouseDown += 图片框_MouseDown;
            图片框.MouseUp += 图片框_MouseUp;
            Controls.Add(图片框);
            原图 = 打开的图片;
            图片框.Image = 原图;
            AutoScroll = true;
            Text = Path.GetFileName(文件名);
        }

        public 图片Tabpage(string 文件夹, MouseEventHandler 图片框_MouseMove, MouseEventHandler 图片框_MouseDown, MouseEventHandler 图片框_MouseUp) {
            图片框 = new PictureBox();
            图片框.SizeMode = PictureBoxSizeMode.AutoSize;
            图片框.MouseMove += 图片框_MouseMove;
            图片框.MouseDown += 图片框_MouseDown;
            图片框.MouseUp += 图片框_MouseUp;
            Controls.Add(图片框);
            AutoScroll = true;
            路径 = 文件夹;
            DirectoryInfo 文件夹信息 = new DirectoryInfo(路径);
            获取目录中所有图片文件(文件夹信息, 所有图片列表, 子目录列表);
            if (所有图片列表.Count == 0) return;
            是文件夹 = true;
            图片框.Image = 原图 = 加水印.加载图片文件(所有图片列表[当前图片序号]);
            Text = "文件夹:\"" + 所有图片列表[当前图片序号].Insert(路径.Length,"\"");
        }

        public void 上下翻图片(int d) {
            if (所有图片列表.Count <= 1) return;
            当前图片序号 += d;
            if (当前图片序号 < 0) 当前图片序号 += 所有图片列表.Count;
            if (当前图片序号 >= 所有图片列表.Count) 当前图片序号 -= 所有图片列表.Count;
            Bitmap 图片 = 加水印.加载图片文件(所有图片列表[当前图片序号]);
            if (图片 != null) {
                图片框.SizeMode = PictureBoxSizeMode.AutoSize;
                图片框.Image = 原图 = 图片;
                水印图 = null;
                Text = "文件夹:\"" + 所有图片列表[当前图片序号].Insert(路径.Length, "\"");
            }
        }

        private void 获取目录中所有图片文件(DirectoryInfo 文件夹信息, List<string> 文件列表, List<string> 目录列表) {
            foreach (var 文件 in 文件夹信息.GetFiles()) {
                string 后缀名 = 文件.Extension.ToLower().Substring(1);
                if (后缀名 == "bmp" || 后缀名 == "jpeg" || 后缀名 == "png" || 后缀名 == "jpg")
                    文件列表.Add(文件.FullName);
            }
            foreach (var 文件夹 in 文件夹信息.GetDirectories()) {
                目录列表.Add(文件夹.FullName);
                获取目录中所有图片文件(文件夹, 文件列表, 目录列表);
            }
        }

        public int 保存所有图片到文件夹(string 保存路径, List<文印参数> 文印参数表, List<图印参数> 图印参数表, ProgressBar 进度条) {
            int N = 0;
            if (!Directory.Exists(保存路径)) return N;
            foreach (string 文件夹 in 子目录列表) {
                string 新路径 = 文件夹.Replace(路径, 保存路径);
                if (!Directory.Exists(新路径))
                    Directory.CreateDirectory(新路径);
            }
            进度条.Maximum = 所有图片列表.Count;
            进度条.Visible = true;
            foreach (var 图片文件 in 所有图片列表) {
                Bitmap 图片 = 加水印.加载图片文件(图片文件);
                if (图片 == null) continue; 
                Bitmap 加水印图 = new Bitmap(图片.Width, 图片.Height, 图片.PixelFormat);
                水印.加水印(图片, 加水印图, 文印参数表, 图印参数表);
                string 文件名 = 图片文件.Replace(路径, 保存路径);
                ImageFormat 图片格式 = 加水印.获取图片格式(Path.GetExtension(文件名).Substring(1));
                try {
                    加水印图.Save(文件名, 图片格式);
                }
                catch {
                    using (var 位图 = new Bitmap(加水印图))
                        位图.Save(文件名, 图片格式);

                }
                N++;
                进度条.Value = N;
            }
            进度条.Visible = false;
            return N;
        }



        public void 图片加水印(List<文印参数> 文印参数表, List<图印参数> 图印参数表) {
            if (原图 == null) return;
            if(水印图 == null) 水印图 = new Bitmap(原图.Width, 原图.Height, 原图.PixelFormat);
            水印.加水印(原图, 水印图, 文印参数表, 图印参数表);
            图片框.Image = 水印图;
            图片框.Refresh();
        }

        public void 图片调到合适大小() {
            if (图片框.Image.Width <= Size.Width && 图片框.Image.Height <= Size.Height) {
                图片框.SizeMode = PictureBoxSizeMode.AutoSize;
                比例 = 1;
            }
            else {
                图片框.SizeMode = PictureBoxSizeMode.Zoom;
                AutoScrollPosition = new Point(0, 0); //这一句要写在下一句的前面，这两句是为了解决1:1模式下滚动条有滚动后再会到合适模式会产生的图片位置跑偏问题。
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

        public Point 转图片坐标(Point 点) {
            return new Point((int)(点.X * 比例), (int)(点.Y * 比例));
        }

        public void 关闭() {
            ((TabControl)Parent).TabPages.Remove(this);
            原图.Dispose();
            图片框.Dispose();
            水印图.Dispose();
            Dispose();
        }
    }
}
