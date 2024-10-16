using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

/// <summary>
/// B站无限次元: https://space.bilibili.com/2139404925
/// Github: https://github.com/becomequantum/Kryon
/// </summary>

namespace 加水印 {
    class 水印 {
        static public string[] 日期格式 = new string[] { "加日期", "D", "y", "d", "yyyy-MM-dd", "yyyyMMdd" };
        static public string[] 时间格式 = new string[] { "加时间", "hh:mm", "T" };
       
       
        public static void 加水印(Bitmap 原图, Bitmap 水印图, List<文印参数> 文印参数表, List<图印参数> 图印参数表) {
            Graphics g = Graphics.FromImage(水印图);
            if (原图.PixelFormat == PixelFormat.Format24bppRgb)
                图像.原图刷新图24(原图, 水印图);
            else if(原图.PixelFormat == PixelFormat.Format32bppArgb || 原图.PixelFormat == PixelFormat.Format32bppRgb || 原图.PixelFormat == PixelFormat.Format32bppPArgb)
                图像.原图刷新图32(原图, 水印图);
            else {
                g.DrawImage(原图, 0, 0);
            }
            
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            foreach (var 参数 in 图印参数表)
                加图印(g, 参数, 水印图.Width, 水印图.Height);
            foreach (var 参数 in 文印参数表) 
                加文印(g, 参数, 水印图.Width, 水印图.Height);
            
            g.Dispose();
        }

        private static void 加图印(Graphics g水印图, 图印参数 参数, int 图宽, int 图高) {
            if (参数.画啥 == 图形类别.图 && 参数.图印页 == null) return;
            Graphics g, g空 = g水印图;
            Bitmap 空背景 = null;
            if (参数.透明度波动 > 0) {
                空背景 = new Bitmap(图宽, 图高, PixelFormat.Format32bppArgb);
                g空 = Graphics.FromImage(空背景);
                g = g空;
            }
            else
                g = g水印图;
            Bitmap 水印图 = null;
            PointF 图印中心点= 参数.中心点 = new PointF(图宽 * 参数.X百分比, 图高 * 参数.Y百分比);
            int 短边长度 = 图宽 >= 图高 ? 图高 : 图宽;
            float 横间距 = 参数.横向百分比间距 * 图宽;
            横间距 = 横间距 > 33 ? 横间距 : 33;
            float 纵间距 = 参数.纵向百分比间距 * 图高;
            纵间距 = 纵间距 > 33 ? 纵间距 : 33;
            float 绘图矩形高度 = 短边长度 * 参数.大小百分比;
            绘图矩形高度 = 绘图矩形高度 < 33 ? 33 : 绘图矩形高度;
            float 绘图矩形宽度;
            float 字粗细 = 短边长度 * 参数.粗细 * 0.5f;
            Color 框颜色 = Color.FromArgb(参数.透明度, 参数.颜色);
            Pen 画框笔 = new Pen(框颜色, 字粗细);

            if (参数.画啥 == 图形类别.图) {
                水印图 = (Bitmap)参数.图印页.水印图;
                if (参数.图印页 == null || 水印图 == null) return;
                绘图矩形宽度 = 绘图矩形高度 * 水印图.Width / 水印图.Height;
                参数.大小 = new SizeF(绘图矩形宽度, 绘图矩形高度);
                图像.调透明度(参数.图印页.原图, 水印图, 参数.透明度);
            }
            else {
                绘图矩形宽度 = 绘图矩形高度 * 参数.宽高比;
                参数.大小 = new SizeF(绘图矩形宽度, 绘图矩形高度);
            }



            if (参数.横向复制个数 > 0 && 参数.纵向复制个数 > 0) {
                for (int i = 0; i <= 参数.纵向复制个数; i++)
                    for (int j = 0; j <= 参数.横向复制个数; j++) {
                        PointF 新图片中心点 = new PointF(图印中心点.X + 横间距 * j, 图印中心点.Y + 纵间距 * i);
                        画图片(g, 新图片中心点, 参数.旋转角度, 水印图, 绘图矩形宽度, 绘图矩形高度, 参数.画啥, 画框笔);
                    }
            }
            else {
                画图片(g, 图印中心点, 参数.旋转角度, 水印图, 绘图矩形宽度, 绘图矩形高度, 参数.画啥, 画框笔);
                if (参数.横向复制个数 > 0) {
                    for (int i = 1; i <= 参数.横向复制个数; i++) {
                        图印中心点 = new PointF(图印中心点.X + 横间距, 图印中心点.Y);
                        画图片(g, 图印中心点, 参数.旋转角度, 水印图, 绘图矩形宽度, 绘图矩形高度, 参数.画啥, 画框笔);
                    }
                }
                else if (参数.纵向复制个数 > 0) {
                    for (int i = 1; i <= 参数.纵向复制个数; i++) {
                        图印中心点 = new PointF(图印中心点.X, 图印中心点.Y + 纵间距);
                        画图片(g, 图印中心点, 参数.旋转角度, 水印图, 绘图矩形宽度, 绘图矩形高度, 参数.画啥, 画框笔);
                    }
                }
            }
            if (参数.透明度波动 > 0) {
                图像.颜色随机化(空背景, 参数.透明度波动);
                g水印图.DrawImage(空背景, 0, 0);
                g空.Dispose();
                空背景.Dispose();
            }
        }

        static void 画图片(Graphics g, PointF 图印中心点, int 旋转角度, Bitmap 水印图, float 绘图矩形宽度, float 绘图矩形高度, 图形类别 画啥, Pen 笔) {
            g.TranslateTransform(图印中心点.X, 图印中心点.Y); //要先把中心设为原点再设置旋转
            g.RotateTransform(旋转角度);
            if (画啥 == 图形类别.图)
                g.DrawImage(水印图, new RectangleF(-绘图矩形宽度 / 2, -绘图矩形高度 / 2, 绘图矩形宽度, 绘图矩形高度));
            else if (画啥 == 图形类别.矩形)
                g.DrawRectangle(笔, -绘图矩形宽度 / 2, -绘图矩形高度 / 2, 绘图矩形宽度, 绘图矩形高度);
            else if (画啥 == 图形类别.圆)
                g.DrawEllipse(笔, -绘图矩形宽度 / 2, -绘图矩形高度 / 2, 绘图矩形宽度, 绘图矩形高度);
            g.ResetTransform();
        }

        static string 日期时间替换(string 文本) {
            for (int i = 1; i < 日期格式.Length; i++) {
                if (文本.IndexOf(@"+d") < 0) break;
                文本 = 文本.Replace(@"+d" + i.ToString(), DateTime.Now.ToString(日期格式[i]));
            }
            for (int i = 1; i < 时间格式.Length; i++) {
                if (文本.IndexOf(@"+t") < 0) break;
                文本 = 文本.Replace(@"+t" + i.ToString(), DateTime.Now.ToString(时间格式[i]));
            }
            return 文本;
        }

        private static void 加文印(Graphics g水印图, 文印参数 参数, int 图宽, int 图高) {
            if (参数.文本.Length == 0) return;
            Graphics g, g空 = g水印图;
            Bitmap 空背景 = null;
            if (参数.透明度波动 > 0) {
                空背景 = new Bitmap(图宽, 图高, PixelFormat.Format32bppArgb);
                g空 = Graphics.FromImage(空背景);
                g = g空;
            }
            else
                g = g水印图;

            PointF 文本中心点 = 参数.中心点 = new PointF(图宽 * 参数.X百分比, 图高 * 参数.Y百分比);
            string 待绘文本 = 日期时间替换(参数.文本);
            int 短边长度 = 图宽 >= 图高 ? 图高 : 图宽;
            float 字体大小 = 短边长度 * 参数.大小百分比 * 0.8f;
            if (字体大小 < 11) 字体大小 = 11;
            Font 字体 = new Font(参数.字体, 字体大小,参数.粗体 | 参数.斜体 | 参数.下划线 | 参数.删除线);
            SizeF 文本大小 = 参数.大小 = g.MeasureString(待绘文本, 字体);
            PointF 文本位置 = new PointF( - 文本大小.Width / 2,  - 文本大小.Height / 2);
            Brush 画刷 = new SolidBrush(Color.FromArgb(参数.透明度,参数.颜色));
            float 横间距 = 参数.横向百分比间距 * 图宽;
            横间距 = 横间距 > 7 ? 横间距 : 7;
            float 纵间距 = 参数.纵向百分比间距 * 图高;
            纵间距 = 纵间距 > 7 ? 纵间距 : 7;



            if (参数.横向复制个数 > 0 && 参数.纵向复制个数 > 0) {
                for (int i = 0; i <= 参数.纵向复制个数; i++)
                    for (int j = 0; j <= 参数.横向复制个数; j++) {
                        PointF 新文本中心点 = new PointF(文本中心点.X + 横间距 * j, 文本中心点.Y + 纵间距 * i);
                        画文本(g, 新文本中心点, 参数.旋转角度, 待绘文本, 字体, 画刷, 文本位置);
                    }
            }
            else {
                画文本(g, 文本中心点, 参数.旋转角度, 待绘文本, 字体, 画刷, 文本位置);
                if (参数.横向复制个数 > 0) {
                    for (int i = 1; i <= 参数.横向复制个数; i++) {
                        文本中心点 = new PointF(文本中心点.X + 横间距, 文本中心点.Y);
                        画文本(g, 文本中心点, 参数.旋转角度, 待绘文本, 字体, 画刷, 文本位置);
                    }
                }
                else if (参数.纵向复制个数 > 0) {
                    for (int i = 1; i <= 参数.纵向复制个数; i++) {
                        文本中心点 = new PointF(文本中心点.X, 文本中心点.Y + 纵间距);
                        画文本(g, 文本中心点, 参数.旋转角度, 待绘文本, 字体, 画刷, 文本位置);
                    }
                }
            }

            if (参数.透明度波动 > 0) {
                图像.颜色随机化(空背景, 参数.透明度波动);
                g水印图.DrawImage(空背景, 0, 0);
                g空.Dispose();
                空背景.Dispose();
            }
            
        }

        static void 画文本(Graphics g, PointF 文本中心点, int 旋转角度, string 文本, Font 字体, Brush 画刷, PointF 文本位置) {
            g.TranslateTransform(文本中心点.X, 文本中心点.Y); //要先把中心设为原点再设置旋转
            g.RotateTransform(旋转角度);
            g.DrawString(文本, 字体, 画刷, 文本位置);
            g.ResetTransform();
        }

        
       

        
    }

    class 基本参数 {
        public float X百分比;
        public float Y百分比;
        public float 大小百分比;
        public Color 颜色;
        public int 透明度;      //0为完全透明,255为不透明
        public int 旋转角度;
        public int 横向复制个数;
        public int 纵向复制个数;
        public float 横向百分比间距;
        public float 纵向百分比间距;
        public PointF 中心点;
        public SizeF 大小;
        public byte 透明度波动;
        public TabPage 所属设置页;
        public char 隔 = 'ṡ';
        public int 基本参数个数 = 13;

        static public bool 点在印内(Point 点, PointF 中心点, SizeF 大小, int 旋转角度) {
            int 原角度 = (int)(Math.Atan2(点.Y - 中心点.Y, 点.X - 中心点.X) / Math.PI * 180 + 0.5);
            double 长度 = Math.Sqrt(Math.Pow(点.Y - 中心点.Y, 2) + Math.Pow(点.X - 中心点.X, 2));
            double 新弧度 = (原角度 - 旋转角度) / 180.0 * Math.PI;
            int X = (int)Math.Abs(长度 * Math.Cos(新弧度));
            int Y = (int)Math.Abs(长度 * Math.Sin(新弧度));
            if (X < 大小.Width / 2 && Y < 大小.Height / 2)
                return true;
            else
                return false;
        }

        public string 基本参数转字符串() {
            return X百分比.ToString() + 隔 + Y百分比.ToString() + 隔 + 大小百分比.ToString() + 隔 +  颜色.R.ToString() +","+ 颜色.G.ToString() + ","+ 颜色.B.ToString() + 隔 + 透明度.ToString() + 隔+ 旋转角度.ToString() + 隔 + 横向复制个数.ToString() + 隔 + 纵向复制个数.ToString() + 隔 + 横向百分比间距.ToString() + 隔 + 纵向百分比间距.ToString() + 隔 + 中心点.X.ToString() + "," + 中心点.Y.ToString() + 隔 + 大小.Width.ToString() + "," + 大小.Height.ToString() + 隔 + 透明度波动.ToString() + 隔;
        }//13个

        public bool 字符转基本参数(string[] s) {
            try {
                X百分比 = (float)Convert.ToDouble(s[0]);
                Y百分比 = (float)Convert.ToDouble(s[1]);
                大小百分比 = (float)Convert.ToDouble(s[2]);
                string[] s颜色 = s[3].Split(',');
                颜色 = Color.FromArgb(Convert.ToInt32(s颜色[0]), Convert.ToInt32(s颜色[1]), Convert.ToInt32(s颜色[2]));
                透明度 = Convert.ToInt32(s[4]);
                旋转角度 = Convert.ToInt32(s[5]);
                横向复制个数 = Convert.ToInt32(s[6]);
                纵向复制个数 = Convert.ToInt32(s[7]);
                横向百分比间距 = (float)Convert.ToDouble(s[8]);
                纵向百分比间距 = (float)Convert.ToDouble(s[9]);
                string[] s点 = s[10].Split(',');
                中心点.X = (float)Convert.ToDouble(s点[0]);
                中心点.Y = (float)Convert.ToDouble(s点[1]);
                string[] s大小 = s[11].Split(',');
                大小.Width = (float)Convert.ToDouble(s大小[0]);
                大小.Height = (float)Convert.ToDouble(s大小[1]);
                透明度波动 = Convert.ToByte(s[12]);
                return true;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

    }

    class 文印参数 : 基本参数 {
        public string 文本 = "";
        public string 字体 = "";
        public FontStyle 粗体;
        public FontStyle 斜体;
        public FontStyle 下划线;
        public FontStyle 删除线;
        int 文印参数个数 = 6;

        public void 拷贝(文印参数 参数) {
            文本 = 参数.文本;
            X百分比 = 参数.X百分比;
            Y百分比 = 参数.Y百分比;
            大小百分比 = 参数.大小百分比;
            透明度 = 参数.透明度;
            字体 = 参数.字体;
            颜色 = 参数.颜色;
            旋转角度 = 参数.旋转角度;
            粗体 = 参数.粗体;
            斜体 = 参数.斜体;
            下划线 = 参数.下划线;
            删除线 = 参数.删除线;
            横向复制个数 = 参数.横向复制个数;
            纵向复制个数 = 参数.纵向复制个数;
            横向百分比间距 = 参数.横向百分比间距;
            纵向百分比间距 = 参数.纵向百分比间距;
            所属设置页 = 参数.所属设置页;
        }

        public bool 点在文印内(Point 点) {
            if (文本.Length == 0) return false;
            return 点在印内(点, 中心点, 大小, 旋转角度);
        }

        public string 转字符串() {
            return 基本参数转字符串() + 文本.ToString() + 隔 + 字体.ToString() + 隔 + ((int)粗体).ToString() + 隔 + ((int)斜体).ToString() + 隔 + ((int)下划线).ToString() + 隔 + ((int)删除线).ToString();
        }

        public bool 字符转参数(string S设置) {
            string[] s = S设置.Split(隔);
            if (s.Length != 文印参数个数 + 基本参数个数) return false;
            if(!字符转基本参数(s)) return false;
            try {
                文本 = s[基本参数个数];
                字体 = s[基本参数个数 + 1];
                粗体 = (FontStyle)Convert.ToInt32(s[基本参数个数 + 2]);
                斜体 = (FontStyle)Convert.ToInt32(s[基本参数个数 + 3]);
                下划线 = (FontStyle)Convert.ToInt32(s[基本参数个数 + 4]);
                删除线 = (FontStyle)Convert.ToInt32(s[基本参数个数 + 5]);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

    }

    class 图印参数 : 基本参数 {
        public 图印Tabpage 图印页;
        public string 名称 = "";
        public 图形类别 画啥 = 图形类别.图;
        public float 宽高比 = 1.11f;
        public float 粗细 = 0.0111f;
        public string 图印文件名 = "空";
        int 图印参数个数 = 5;

        public bool 点在图印内(Point 点) {
            if (画啥 == 图形类别.图 && 图印页 == null) return false;
            return 点在印内(点, 中心点, 大小, 旋转角度);
        }

        public string 转字符串() {
            return 基本参数转字符串() + 名称.ToString() + 隔 + ((int)画啥).ToString() + 隔 + 宽高比.ToString() + 隔 + 粗细.ToString() + 隔 + 图印文件名.ToString(); 
        }

        public bool 字符转参数(string S设置) {
            string[] s = S设置.Split(隔);
            if (s.Length != 图印参数个数 + 基本参数个数) return false;
            if (!字符转基本参数(s)) return false;
            try {
                名称 = s[基本参数个数];
                画啥 = (图形类别)Convert.ToInt32(s[基本参数个数 + 1]);
                宽高比 = (float)Convert.ToDouble(s[基本参数个数 + 2]);
                粗细 = (float)Convert.ToDouble(s[基本参数个数 + 3]);
                图印文件名 = s[基本参数个数 + 4];
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }
    }

    enum 图形类别 {图, 矩形, 圆}
}
