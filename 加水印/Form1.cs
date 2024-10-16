using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Windows.Forms;

/// <summary>
/// B站无限次元: https://space.bilibili.com/2139404925
/// Github: https://github.com/becomequantum/Kryon
/// </summary>
namespace 加水印 {
    public partial class 加水印 : Form {
        Bitmap 当前位图;
        图片Tabpage 当前图片Tab;
        PictureBox 当前图片框;
        string 字体 = "";
        Color 字颜色 = Color.Black;
        int[] 自定义颜色;
        FontStyle 粗 = FontStyle.Regular;
        FontStyle 斜 = FontStyle.Regular;
        FontStyle 下划线 = FontStyle.Regular;
        FontStyle 删除线 = FontStyle.Regular;
        文印参数 默认参数 = new 文印参数();
        文印参数 文印主页参数 = new 文印参数();
        文印参数 当前文印参数;
        图印参数 当前图印参数;
        图印参数 图印主页参数 = new 图印参数();
        Control[] 文印控件组;
        Control[] 图印控件组;
        bool 暂停绘制 = false;
        List<文印参数> 文印参数表 = new List<文印参数>();
        List<图印参数> 图印参数表 = new List<图印参数>();
        bool 左键按下 = false;
        Point 原鼠标坐标;
        readonly float 滑动条最大值 = 10000;
        图形类别 画啥 = 图形类别.图;
        Point 颜色按钮初始位置;
        string 保存路径 = @".\水印设置\";
        string 后缀名 = @".wartermark";
        int 变色计数 = 0;
        TabPage 说明;

        public 加水印() {
            InitializeComponent();
            获取系统字体();
            日期时间设置初始化();
            参数初始化();
            说明 = 说明tabPage;
            欢迎使用本软件ToolStripMenuItem.Text = "欢迎使用!点‘说明’可查看使用说明";
            图片tab.Controls.Remove(说明);
        }

        private void 参数初始化() {
            获取文印参数(默认参数);
            获取文印参数(文印主页参数);
            获取文图印控件组();
            默认参数.所属设置页 = 文印主页参数.所属设置页 = 文字水印tabPage;
            文印参数表.Add(文印主页参数);
            图印参数表.Add(图印主页参数);
            当前文印参数 = 文印主页参数;
            图印主页参数初始化();
            查找已保存的设置();
        }

        private void 图印主页参数初始化() {
            图印主页参数.X百分比 = 0.5f;
            图印主页参数.Y百分比 = 0.5f;
            图印主页参数.旋转角度 = 360;
            图印主页参数.大小百分比 = 0.3333f;
            图印主页参数.透明度 = 96;
            图印主页参数.所属设置页 = 图片水印tabPage;
            图印主页参数.纵向复制个数 = 0;
            图印主页参数.横向复制个数 = 0;
            图印主页参数.横向百分比间距 = 0.2222f;
            图印主页参数.纵向百分比间距 = 0.2222f;
            图印主页参数.图印页 = null;
            图印主页参数.名称 = "尚未选择要加为水印的图片";
            图印主页参数.宽高比 = 1.11f;
            图印主页参数.粗细 = 0.0111f;
            图印主页参数.画啥 = 图形类别.图;
            图印主页参数.颜色 = Color.Black;
            图印主页参数.透明度波动 = 0;
        }

        private void 获取文图印控件组() {
            文印控件组 = new Control[文字水印tabPage.Controls.Count];
            for (int i = 0; i < 文印控件组.Length; i++) {
                文印控件组[i] = 文字水印tabPage.Controls[i];
            }
            图印控件组 = new Control[8];
            图印控件组[0] = 位置及大小groupBox;
            图印控件组[1] = 透明度groupBox;
            图印控件组[2] = 旋转角度groupBox;
            图印控件组[3] = 复制groupBox;
            图印控件组[4] = 暂存按钮;
            图印控件组[5] = 水印图标签;
            图印控件组[6] = 选水印图按钮;
            图印控件组[7] = 画啥选择groupBox;
            颜色按钮初始位置 = 颜色button.Location;
        }

        private void 日期时间设置初始化() {
            foreach (var 格式 in 水印.日期格式)
                日期选择comboBox.Items.Add(DateTime.Now.ToString(格式));
            日期选择comboBox.SelectedItem = 水印.日期格式[0];
            foreach (var 格式 in 水印.时间格式)
                时间选择comboBox.Items.Add(DateTime.Now.ToString(格式));
            时间选择comboBox.SelectedItem = 水印.时间格式[0];
        }

        private void 获取系统字体() {
            InstalledFontCollection 字体表 = new InstalledFontCollection();
            for (int i = 字体表.Families.Length - 1; i >= 0; i--) {
                字体comboBox.Items.Add(字体表.Families[i].Name);
            }
            if (字体comboBox.Items.IndexOf("微软雅黑") >= 0) {
                字体comboBox.Items.Insert(0, "微软雅黑");
                字体comboBox.SelectedItem = "微软雅黑";
            }

        }

        private void 打开图片可多选ToolStripMenuItem_Click(object sender, EventArgs e) {
            string[] 文件名数组;
            文件名数组 = 获取多个打开文件名("图片文件|*.bmp;*.jpg;*.png;*.jpeg;*.jpe");
            if (文件名数组 != null && 文件名数组.Length > 0)
                foreach (var 文件 in 文件名数组)
                    打开图片文件(文件);
        }

        private string[] 获取多个打开文件名(string 后缀名过滤) {
            OpenFileDialog 打开文件对话框 = new OpenFileDialog();
            打开文件对话框.Filter = 后缀名过滤;
            打开文件对话框.Multiselect = true;
            打开文件对话框.Title = "打开多个图片文件";
            打开文件对话框.RestoreDirectory = true;
            if (打开文件对话框.ShowDialog(this) == DialogResult.OK)
                return 打开文件对话框.FileNames;
            else return null;
        }//获取多个打开文件名("图片文件|*.bmp;*.jpg;*.png;*.jpeg;*.jpe")

        private string 获取打开文件名(string 后缀名过滤) {
            OpenFileDialog 打开文件对话框 = new OpenFileDialog();
            打开文件对话框.Filter = 后缀名过滤;
            打开文件对话框.Title = "打开多个图片文件";
            打开文件对话框.RestoreDirectory = true;
            if (打开文件对话框.ShowDialog(this) == DialogResult.OK)
                return 打开文件对话框.FileName;
            else return null;
        }

        static public Bitmap 加载图片文件(string 图片文件名) { //这样加载图片后，原来的图片文件不会被占用锁定
            try {
                FileStream 文件流 = new FileStream(图片文件名, FileMode.Open, FileAccess.Read);
                Bitmap tmpBmp = (Bitmap)Image.FromStream(文件流);
                文件流.Close();
                文件流.Dispose();
                return tmpBmp;
            }
            catch (Exception exp) {
                MessageBox.Show(exp.Message);
                return null;
            }
        }

        private void 打开图片文件(string 当前图片文件名) {

            if (当前图片文件名 == null) { return; }
            Bitmap 打开的位图; //打开的图片存在这个变量里
            打开的位图 = 加载图片文件(当前图片文件名);
            当前图片Tab = new 图片Tabpage(打开的位图, 当前图片文件名, 图片框_MouseMove, 图片框_MouseDown, 图片框_MouseUp);
            图片tab.TabPages.Add(当前图片Tab);
            图片tab.SelectedTab = 当前图片Tab;
            调整图片尺寸();
            当前位图 = 打开的位图;
            当前图片框 = 当前图片Tab.图片框;
            水印参数_Changed(this, new EventArgs());
        }

        private void 图片tab_SelectedIndexChanged(object sender, EventArgs e) {
            if (图片tab.SelectedTab != null) {
                if (图片tab.SelectedTab is 图印Tabpage) {
                    foreach (var 控件 in 文印控件组)
                        控件.Enabled = false;
                    水印设置tab.SelectedTab = ((图印Tabpage)图片tab.SelectedTab).图印设置页;
                }
                else
                    foreach (var 控件 in 文印控件组)
                        控件.Enabled = true;
                
                当前图片Tab = 图片tab.SelectedTab as 图片Tabpage;
                调整图片尺寸();
                if (当前图片Tab != null) {
                    当前位图 = (Bitmap)当前图片Tab.图片框.Image;
                    当前图片框 = 当前图片Tab.图片框;
                    当前图片Tab.图片加水印(文印参数表, 图印参数表);
                }
                
            }
        }

        private void 加水印_Resize(object sender, EventArgs e) {
            if (图片tab.SelectedTab != null) {
                调整图片尺寸();
            }
        }

        private void 调整图片尺寸() {
            if (图片tab.SelectedTab != null && 图片tab.SelectedTab is 图印Tabpage) {
                if (适合radioButton.Checked)
                    ((图印Tabpage)图片tab.SelectedTab).图片调到合适大小();
                else if (原始radioButton.Checked)
                    ((图印Tabpage)图片tab.SelectedTab).图片调到原始大小();
            }
            if (当前图片Tab == null) return;
            if (适合radioButton.Checked)
                当前图片Tab.图片调到合适大小();
            else if (原始radioButton.Checked)
                当前图片Tab.图片调到原始大小();
            
        }

        private void 适合radioButton_CheckedChanged(object sender, EventArgs e) {
            调整图片尺寸();
        }

        private void 透明度滑动条_Scroll(object sender, EventArgs e) {
            toolTip1.SetToolTip(透明度滑动条, ((int)((255 - 透明度滑动条.Value + 1) / 256.0 * 100.0 + 0.5)).ToString() + "%");
            水印参数_Changed(sender, e);
        }

        private void 字体comboBox_SelectedIndexChanged(object sender, EventArgs e) {
            try {
                字体 = 字体comboBox.SelectedItem.ToString();
                水印文本框.Font = new Font(字体, 水印文本框.Font.Size);
            }
            catch (Exception) {
                return;
            }

            水印参数_Changed(sender, e);
        }

        private void 颜色button_Click(object sender, EventArgs e) {
            ColorDialog 选色窗体 = new ColorDialog();
            选色窗体.FullOpen = true;
            if (自定义颜色 != null)
                选色窗体.CustomColors = 自定义颜色;

            if (选色窗体.ShowDialog() == DialogResult.OK) {
                自定义颜色 = 选色窗体.CustomColors; //保存用户设置的自定义颜色
                字颜色 = 选色窗体.Color;
                颜色button.ForeColor = 水印文本框.ForeColor = 字颜色;
                if (字颜色.GetBrightness() * 240 >= 180)
                    水印文本框.BackColor = Color.Gray;
                else
                    水印文本框.BackColor = SystemColors.Window;
                水印参数_Changed(sender, e);

            }



        }

        private void 角度滑动条_Scroll(object sender, EventArgs e) {
            toolTip1.SetToolTip(角度滑动条, 角度滑动条.Value.ToString());
            水印参数_Changed(sender, e);
        }

        private void 大小滑动条_Scroll(object sender, EventArgs e) {
            int Value = ((TrackBar)sender).Value;
            string 提示 = Value.ToString();
            if (Value < 100)
                提示 = "0." + 提示 + "%";
            else
                提示 = 提示.Insert(提示.Length - 2, ".") + "%";
            toolTip1.SetToolTip((TrackBar)sender, 提示);
            水印参数_Changed(sender, e);
        }

        private void 粗体按钮_Click(object sender, EventArgs e) {
            if (粗 == FontStyle.Regular) {
                粗体按钮.BackColor = Color.LightGray;
                粗 = FontStyle.Bold;
            }
            else {
                粗体按钮.BackColor = Color.Transparent;
                粗 = FontStyle.Regular;
            }
            水印参数_Changed(sender, e);
        }

        private void 斜体按钮_Click(object sender, EventArgs e) {
            if (斜 == FontStyle.Regular) {
                斜体按钮.BackColor = Color.LightGray;
                斜 = FontStyle.Italic;
            }
            else {
                斜体按钮.BackColor = Color.Transparent;
                斜 = FontStyle.Regular;
            }
            水印参数_Changed(sender, e);
        }

        private void 下划线按钮_Click(object sender, EventArgs e) {
            if (下划线 == FontStyle.Regular) {
                下划线按钮.BackColor = Color.LightGray;
                下划线 = FontStyle.Underline;
            }
            else {
                下划线按钮.BackColor = Color.Transparent;
                下划线 = FontStyle.Regular;
            }
            水印参数_Changed(sender, e);
        }

        private void 删除线按钮_Click(object sender, EventArgs e) {
            if (删除线 == FontStyle.Regular) {
                删除线按钮.BackColor = Color.LightGray;
                删除线 = FontStyle.Strikeout;
            }
            else {
                删除线按钮.BackColor = Color.Transparent;
                删除线 = FontStyle.Regular;
            }
            水印参数_Changed(sender, e);
        }

        private void 日期选择comboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (日期选择comboBox.SelectedIndex > 0)
                水印文本框.Text += @"+d" + 日期选择comboBox.SelectedIndex;
        }

        private void 时间选择comboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (时间选择comboBox.SelectedIndex > 0)
                水印文本框.Text += @"+t" + 时间选择comboBox.SelectedIndex;
        }

        private void 个数textBox_TextChanged(object sender, EventArgs e) {
            ((TextBox)sender).Text = Regex.Replace(((TextBox)sender).Text, @"[^\d]*", "");//去掉非数字
            int 值;
            if (int.TryParse(((TextBox)sender).Text, out 值)) {
                if (值 > 0) {
                    if (((TextBox)sender).Name == "横向个数textBox")
                        横向间距滑动条.Enabled = true;
                    else
                        纵向间距滑动条.Enabled = true;
                }
                else {
                    if (((TextBox)sender).Name == "横向个数textBox")
                        横向间距滑动条.Enabled = false;
                    else
                        纵向间距滑动条.Enabled = false;
                }
                水印参数_Changed(sender, e);
            }
        }

        private void 添加新文印设置页(文印参数 参数) {
            文印设置Tabpage 新文印设置页 = new 文印设置Tabpage(参数);
            水印设置tab.Controls.Add(新文印设置页);
            水印设置tab.SelectedTab = 新文印设置页;
            参数.所属设置页 = 新文印设置页;
            文印参数表.Add(新文印设置页.本页参数);
            当前文印参数 = 新文印设置页.本页参数;
            当前图印参数 = null;

        }

        private 图印设置Tabpage 添加新图印设置页(图印参数 参数) {
            图印设置Tabpage 新图印设置页 = new 图印设置Tabpage(参数);
            参数.所属设置页 = 新图印设置页;
            水印设置tab.Controls.Add(新图印设置页);
            水印设置tab.SelectedTab = 新图印设置页;
            图印参数表.Add(新图印设置页.本页参数);
            当前图印参数 = 新图印设置页.本页参数;
            当前文印参数 = null;
            return 新图印设置页;
        }

        private void 暂存按钮_Click(object sender, EventArgs e) {
            if (暂存按钮.Text == "暂存") {
                if (水印设置tab.SelectedTab.Name == "文字水印tabPage") {
                    if (水印文本框.Text.Trim().Length == 0) {
                        return;
                    }
                    文印参数 参数 = new 文印参数();
                    if (!获取文印参数(参数)) return;
                    添加新文印设置页(参数);
                    参数.大小 = 文印主页参数.大小;
                    参数.中心点 = 文印主页参数.中心点;
                    文印参数返回默认(文印主页参数);
                }
                else if (水印设置tab.SelectedTab.Name == "图片水印tabPage") {
                    if (图印主页参数.图印页 == null && 图印主页参数.画啥 == 图形类别.图) return;
                    图印参数 参数 = new 图印参数();
                    if (!获取图印参数(参数)) return;
                    图印设置Tabpage 新图印设置页 = 添加新图印设置页(参数);
                    if (图印主页参数.画啥 == 图形类别.图) {
                        参数.图印页 = 图印主页参数.图印页;
                        新图印设置页.Text = 参数.图印页.Text;
                        水印图标签.Text = 参数.名称 = 参数.图印页.Text;
                        新图印设置页.本页参数.图印页.图印设置页 = 新图印设置页;
                    }
                    else {
                        新图印设置页.Text = 参数.画啥.ToString() + 参数.宽高比.ToString();
                    }
                    新图印设置页.本页参数.大小 = 图印主页参数.大小;
                    新图印设置页.本页参数.中心点 = 图印主页参数.中心点;
                    图印主页参数初始化();
                }
            }
            else {
                if (水印设置tab.SelectedTab is 文印设置Tabpage) {
                    文印参数表.Remove(((文印设置Tabpage)水印设置tab.SelectedTab).本页参数);
                    int n = 水印设置tab.SelectedIndex;
                    水印设置tab.Controls.Remove(水印设置tab.SelectedTab);
                    水印设置tab.SelectedIndex = n - 1;
                    水印参数_Changed(sender, e);
                }
                else if (水印设置tab.SelectedTab is 图印设置Tabpage) {
                    图片tab.Controls.Remove(((图印设置Tabpage)水印设置tab.SelectedTab).本页参数.图印页);
                    图印参数表.Remove(((图印设置Tabpage)水印设置tab.SelectedTab).本页参数);
                    int n = 水印设置tab.SelectedIndex;
                    水印设置tab.Controls.Remove(水印设置tab.SelectedTab);
                    水印设置tab.SelectedIndex = n - 1;
                    水印参数_Changed(sender, e);

                }
            }
        }

        private void 水印设置tab_SelectedIndexChanged(object sender, EventArgs e) {
            if (水印设置tab.SelectedTab is 文印设置Tabpage) {
                文印设置Tabpage 文印Tabpage = (文印设置Tabpage)水印设置tab.SelectedTab;
                文印Tabpage.Controls.AddRange(文印控件组);
                文印参数反标控件(文印Tabpage.本页参数);
                当前文印参数 = 文印Tabpage.本页参数;
                当前图印参数 = null;
                if (暂存按钮.Text == "暂存") {
                    暂存按钮.Text = "删除";
                    暂存按钮.ForeColor = Color.Red;
                    暂存按钮.Location = new Point(暂存按钮.Location.X + 50, 暂存按钮.Location.Y);
                }
                颜色button.Location = 颜色按钮初始位置;
            }
            else if (水印设置tab.SelectedTab.Name == "文字水印tabPage") {
                文字水印tabPage.Controls.AddRange(文印控件组);
                文印参数反标控件(文印主页参数);
                当前文印参数 = 文印主页参数;
                当前图印参数 = null;
                if (暂存按钮.Text == "删除") {
                    暂存按钮.Text = "暂存";
                    暂存按钮.ForeColor = Color.Black;
                    暂存按钮.Location = new Point(暂存按钮.Location.X - 50, 暂存按钮.Location.Y);
                }
                颜色button.Location = 颜色按钮初始位置;
            }
            else if (水印设置tab.SelectedTab.Name == "图片水印tabPage") {
                图片水印tabPage.Controls.AddRange(图印控件组);
                图印参数反标控件(图印主页参数);
                当前图印参数 = 图印主页参数;
                当前文印参数 = null;
                if (暂存按钮.Text == "删除") {
                    暂存按钮.Text = "暂存";
                    暂存按钮.ForeColor = Color.Black;
                    暂存按钮.Location = new Point(暂存按钮.Location.X - 50, 暂存按钮.Location.Y);
                }
                画图radioButton.Enabled = true;
                矩形radioButton.Enabled = true;
                圆radioButton.Enabled = true;
                画啥选择groupBox.Controls.Add(颜色button);
                颜色button.Location = new Point(圆radioButton.Location.X + 45, 圆radioButton.Location.Y - 5);
            }
            else if (水印设置tab.SelectedTab is 图印设置Tabpage) {
                图印设置Tabpage 图印Tabpage = (图印设置Tabpage)水印设置tab.SelectedTab;
                图印Tabpage.Controls.AddRange(图印控件组);
                图印参数反标控件(图印Tabpage.本页参数);
                当前图印参数 = 图印Tabpage.本页参数;
                当前文印参数 = null;
                if (暂存按钮.Text == "暂存") {
                    暂存按钮.Text = "删除";
                    暂存按钮.ForeColor = Color.Red;
                    暂存按钮.Location = new Point(暂存按钮.Location.X + 50, 暂存按钮.Location.Y);
                }
                画图radioButton.Enabled = false;
                矩形radioButton.Enabled = false;
                圆radioButton.Enabled = false;
                画啥选择groupBox.Controls.Add(颜色button);
                颜色button.Location = new Point(圆radioButton.Location.X + 45, 圆radioButton.Location.Y - 5);
            }
        }

        private void 水印文本框_TextChanged(object sender, EventArgs e) {
            if (水印设置tab.SelectedTab is 文印设置Tabpage)
                ((文印设置Tabpage)水印设置tab.SelectedTab).Text = 水印文本框.Text.Length <= 4 ? 水印文本框.Text : 水印文本框.Text.Substring(0, 4);
            水印参数_Changed(sender, e);
        }

        private void 关闭当前页ToolStripMenuItem_Click(object sender, EventArgs e) {
            if (图片tab.SelectedTab is 图片Tabpage)
                ((图片Tabpage)图片tab.SelectedTab).关闭();
            if (图片tab.SelectedTab is 图片Tabpage)
                当前图片Tab = (图片Tabpage)图片tab.SelectedTab;
            else
                当前图片Tab = null;
        }

        private void 关闭所有页ToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (var 页 in 图片tab.TabPages) {
                if (页 is 图片Tabpage)
                    ((图片Tabpage)页).关闭();
            }

        }

        private void 图片框_MouseMove(object sender, MouseEventArgs e) {
            if (左键按下) {
                Point 当前鼠标坐标 = 当前图片框.PointToClient(Cursor.Position);

                int dx = 当前鼠标坐标.X - 原鼠标坐标.X;
                int dy = 当前鼠标坐标.Y - 原鼠标坐标.Y;
                int X = X滑动条.Value + (int)(dx / (float)当前图片框.Width * 滑动条最大值);
                int Y = Y滑动条.Value + (int)(dy / (float)当前图片框.Height * 滑动条最大值);
                X滑动条.Value = X >= X滑动条.Minimum && X <= X滑动条.Maximum ? X : X滑动条.Value;
                Y滑动条.Value = Y >= Y滑动条.Minimum && Y <= Y滑动条.Maximum ? Y : Y滑动条.Value;
                水印参数_Changed(sender, e);
                原鼠标坐标 = 当前鼠标坐标;
            }
        }

        private void 图片框_MouseDown(object sender, MouseEventArgs e) {
            原鼠标坐标 = 当前图片框.PointToClient(Cursor.Position);
            if (e.Button == MouseButtons.Left) {
                if (当前文印参数 != null && 当前文印参数.点在文印内(当前图片Tab.转图片坐标(原鼠标坐标)))
                    左键按下 = true;
                else if (当前图印参数 != null && 当前图印参数.图印页 != null && 当前图印参数.点在图印内(当前图片Tab.转图片坐标(原鼠标坐标)))
                    左键按下 = true;
                else {
                    foreach (var 参数 in 文印参数表)
                        if (参数.点在文印内(当前图片Tab.转图片坐标(原鼠标坐标))) {
                            水印设置tab.SelectedTab = 参数.所属设置页;
                            左键按下 = true;
                            break;
                        }
                    foreach (var 参数 in 图印参数表)
                        if (参数.点在图印内(当前图片Tab.转图片坐标(原鼠标坐标))) {
                            水印设置tab.SelectedTab = 参数.所属设置页;
                            左键按下 = true;
                            break;
                        }
                }
            }
        }

        private void 图片框_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left)
                左键按下 = false;

        }

        private void 个数textBox_Leave(object sender, EventArgs e) {
            int 值;
            TextBox 文本框 = (TextBox)sender;
            if (!int.TryParse(文本框.Text, out 值))
                文本框.Text = "0";
        }

        private void 选水印图按钮_Click(object sender, EventArgs e) {
            string 文件名 = 获取打开文件名("图片文件|*.bmp;*.jpg;*.png;*.jpeg;*.jpe");
            if (文件名 == null) return;
            if (水印设置tab.SelectedTab.Name == "图片水印tabPage") {
                if (图印主页参数.图印页 == null) {
                    图印Tabpage 新图印页 = new 图印Tabpage(文件名, 图片水印tabPage, 图印右键菜单);
                    图片tab.Controls.Add(新图印页);
                    图印主页参数.图印页 = 新图印页;
                    水印图标签.Text = 图印主页参数.名称 = 图印主页参数.图印页.Text;
                    if (适合radioButton.Checked)
                        新图印页.图片调到合适大小();
                    else if (原始radioButton.Checked)
                        新图印页.图片调到原始大小();
                }
                else {
                    图印主页参数.图印页.重新加载图片(文件名);
                    水印图标签.Text = 图印主页参数.名称 = 图印主页参数.图印页.Text;
                    if (适合radioButton.Checked)
                        图印主页参数.图印页.图片调到合适大小();
                    else if (原始radioButton.Checked)
                        图印主页参数.图印页.图片调到原始大小();
                }
            }
            else {
                图印设置Tabpage 图印保存页 = (图印设置Tabpage)水印设置tab.SelectedTab;
                图印保存页.本页参数.图印页.重新加载图片(文件名);
                水印图标签.Text = 图印保存页.本页参数.名称 = 图印保存页.Text = 图印保存页.本页参数.图印页.Text;
                if (适合radioButton.Checked)
                    图印保存页.本页参数.图印页.图片调到合适大小();
                else if (原始radioButton.Checked)
                    图印保存页.本页参数.图印页.图片调到原始大小();
            }
            
            水印参数_Changed(sender, e);
        }

        private void 水印图标签_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (水印设置tab.SelectedTab.Name == "图片水印tabPage") {
                if (图印主页参数.图印页 != null)
                    图片tab.SelectedTab = 图印主页参数.图印页;
            }
            else if (水印设置tab.SelectedTab is 图印设置Tabpage) {
                if (((图印设置Tabpage)水印设置tab.SelectedTab).本页参数.图印页 != null)
                    图片tab.SelectedTab = ((图印设置Tabpage)水印设置tab.SelectedTab).本页参数.图印页;
            }

        }

        private void 画啥radioButton_CheckedChanged(object sender, EventArgs e) {
            if (画图radioButton.Checked) {
                画啥 = 图形类别.图;
                宽高比textBox.Enabled = false;
                粗细滑动条.Enabled = false;
                选水印图按钮.Enabled = true;
            }
            else {
                宽高比textBox.Enabled = true;
                粗细滑动条.Enabled = true;
                选水印图按钮.Enabled = false;
            }
            if (矩形radioButton.Checked)
                画啥 = 图形类别.矩形;
            if (圆radioButton.Checked)
                画啥 = 图形类别.圆;
            水印参数_Changed(sender, e);
        }

        private void 粗细textBox_TextChanged(object sender, EventArgs e) {

        }

        private void 宽高比textBox_TextChanged(object sender, EventArgs e) {
            ((TextBox)sender).Text = Regex.Replace(((TextBox)sender).Text, @"[^\d.]*", "");
            if (画啥 != 图形类别.图)
                水印参数_Changed(sender, e);
            if (水印设置tab.SelectedTab is 图印设置Tabpage) {
                图印设置Tabpage 图印设置页 = (图印设置Tabpage)水印设置tab.SelectedTab;
                if (图印设置页.本页参数.画啥 != 图形类别.图) {
                    float 值;
                    if (float.TryParse(((TextBox)sender).Text, out 值))
                        图印设置页.Text = 图印设置页.本页参数.画啥.ToString() + 值.ToString();
                }
            }
        }

        private void 粗细textBox_Leave(object sender, EventArgs e) {

        }

        private void 宽高比textBox_Leave(object sender, EventArgs e) {
            float 值;
            TextBox 文本框 = (TextBox)sender;
            if (!float.TryParse(文本框.Text, out 值))
                文本框.Text = "1.11";
        }

        private void 文印参数反标控件(文印参数 参数) {
            暂停绘制 = true;
            基本参数反标控件(参数);
            水印文本框.Text = 参数.文本;
            if (字颜色.GetBrightness() * 240 >= 180)
                水印文本框.BackColor = Color.Gray;
            else
                水印文本框.BackColor = SystemColors.Window;
            if (参数.粗体 != 粗)
                粗体按钮_Click(this, new EventArgs());
            if (参数.斜体 != 斜)
                斜体按钮_Click(this, new EventArgs());
            if (参数.下划线 != 下划线)
                下划线按钮_Click(this, new EventArgs());
            if (参数.删除线 != 删除线)
                删除线按钮_Click(this, new EventArgs());
            字体comboBox.SelectedItem = 参数.字体;
            暂停绘制 = false;

        }

        private void 图印参数反标控件(图印参数 参数) {
            暂停绘制 = true;
            基本参数反标控件(参数);
            水印图标签.Text = 参数.名称;
            粗细滑动条.Value = (int)(滑动条最大值 * 参数.粗细);
            宽高比textBox.Text = 参数.宽高比.ToString();
            if (参数.画啥 == 图形类别.图)
                画图radioButton.Checked = true;
            if (参数.画啥 == 图形类别.圆)
                圆radioButton.Checked = true;
            if (参数.画啥 == 图形类别.矩形)
                矩形radioButton.Checked = true;
            暂停绘制 = false;
        }

        private void 基本参数反标控件(基本参数 参数) {
            字颜色 = 参数.颜色;
            颜色button.ForeColor = 水印文本框.ForeColor = 字颜色;
            X滑动条.Value = (int)(滑动条最大值 * 参数.X百分比);
            Y滑动条.Value = (int)(滑动条最大值 * 参数.Y百分比);
            透明度滑动条.Value = 参数.透明度;
            角度滑动条.Value = 360 - 参数.旋转角度;
            文字大小滑动条.Value = (int)(滑动条最大值 * 参数.大小百分比);
            横向个数textBox.Text = 参数.横向复制个数.ToString();
            纵向个数textBox.Text = 参数.纵向复制个数.ToString();
            横向间距滑动条.Value = (int)(参数.横向百分比间距 * 滑动条最大值);
            纵向间距滑动条.Value = (int)(参数.纵向百分比间距 * 滑动条最大值);
            波动幅度滑动条.Value = 参数.透明度波动;
        }

        private void 文印参数返回默认(文印参数 参数) {
            参数.拷贝(默认参数);
            参数.文本 = "";
        }
        private bool 获取基本参数(基本参数 参数) {
            try {
                参数.X百分比 = X滑动条.Value / 滑动条最大值;
                参数.Y百分比 = Y滑动条.Value / 滑动条最大值;
                参数.旋转角度 = 360 - 角度滑动条.Value;
                参数.透明度 = 透明度滑动条.Value;
                参数.大小百分比 = 文字大小滑动条.Value / 滑动条最大值;
                参数.横向复制个数 = Convert.ToInt32(横向个数textBox.Text);
                参数.纵向复制个数 = Convert.ToInt32(纵向个数textBox.Text);
                参数.横向百分比间距 = 横向间距滑动条.Value / 滑动条最大值;
                参数.纵向百分比间距 = 纵向间距滑动条.Value / 滑动条最大值;
                参数.颜色 = 字颜色;
                参数.透明度波动 = (byte)波动幅度滑动条.Value;
                return true;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                return false;
            }


        }

        private bool 获取文印参数(文印参数 参数) {
            try {
                if (!获取基本参数(参数)) return false;
                参数.文本 = 水印文本框.Text;
                参数.字体 = 字体;
                参数.旋转角度 = 360 - 角度滑动条.Value;
                参数.粗体 = 粗;
                参数.斜体 = 斜;
                参数.下划线 = 下划线;
                参数.删除线 = 删除线;
                return true;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private bool 获取图印参数(图印参数 参数) {
            try {
                if (!获取基本参数(参数)) return false;
                参数.画啥 = 画啥;
                参数.粗细 = 粗细滑动条.Value / 滑动条最大值;
                参数.宽高比 = (float)Convert.ToDouble(宽高比textBox.Text);
                return true;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                return false;
            }

        }

        private void 水印参数_Changed(object sender, EventArgs e) {
            if (暂停绘制) return;
            if (水印设置tab.SelectedTab is 文印设置Tabpage) {
                if (当前图片Tab != null && 获取文印参数(((文印设置Tabpage)水印设置tab.SelectedTab).本页参数))
                    当前图片Tab.图片加水印(文印参数表, 图印参数表);
            }
            else if (水印设置tab.SelectedTab is 图印设置Tabpage) {
                if (当前图片Tab != null && 获取图印参数(((图印设置Tabpage)水印设置tab.SelectedTab).本页参数))
                    当前图片Tab.图片加水印(文印参数表, 图印参数表);
            }
            else if (水印设置tab.SelectedTab.Name == "文字水印tabPage") {
                if (当前图片Tab != null && 获取文印参数(文印主页参数))
                    当前图片Tab.图片加水印(文印参数表, 图印参数表);
            }
            else if (水印设置tab.SelectedTab.Name == "图片水印tabPage") {
                if (当前图片Tab != null && 获取图印参数(图印主页参数))
                    当前图片Tab.图片加水印(文印参数表, 图印参数表);
            }
        }

        private void 保存设置按钮_Click(object sender, EventArgs e) {
            if (设置名textBox.Text.Length == 0) {
                MessageBox.Show("请在下框中输入文件名!");
                return;
            }
            string 文件名 = 设置名textBox.Text;
            int n = 文件名.IndexOfAny(Path.GetInvalidFileNameChars());
            if (n >= 0) {
                MessageBox.Show("文件名中含有非法字符: " + 文件名.Substring(n, 1) + " 请修改!");
                return;
            }
            try {
                if (Directory.Exists(保存路径) == false)
                    Directory.CreateDirectory(保存路径);
                文件名 = 保存路径 + 文件名 + 后缀名;

                DialogResult 覆盖询问 = DialogResult.Yes;
                if (File.Exists(文件名))
                    覆盖询问 = MessageBox.Show(文件名 + "已存在,是否覆盖?", "是否覆盖设置文件?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (覆盖询问 == DialogResult.No) {
                    MessageBox.Show("那就换个文件名再存吧!");
                    return;
                }

                FileStream 文件流 = new FileStream(文件名, FileMode.Create, FileAccess.Write);
                StreamWriter 写 = new StreamWriter(文件流);
                写.WriteLine("文印");
                foreach (var 参数 in 文印参数表)
                    if (参数.文本.Length > 0)
                        写.WriteLine(参数.转字符串());
                写.WriteLine("图印");
                foreach (var 参数 in 图印参数表) {
                    if (!(参数.画啥 == 图形类别.图))
                        写.WriteLine(参数.转字符串());
                    else if (参数.画啥 == 图形类别.图 && 参数.图印页 != null) {
                        string 图印文件名 = 保存图印文件(参数.图印页);
                        if (图印文件名 != null) {
                            参数.图印文件名 = 图印文件名;
                            写.WriteLine(参数.转字符串());
                        }
                    }
                }
                写.Close();
                文件流.Close();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
            欢迎使用本软件ToolStripMenuItem.Text = "已成功保存设置" + 文件名.Replace(保存路径, "");
            查找已保存的设置();
        }

        private string 保存图印文件(图印Tabpage 图印页) {
            if (图印页.原图 == null) return null;
            try {
                string 文件名 = 图印页.Text.Substring(3);
                string 无后缀名 = Path.GetFileNameWithoutExtension(文件名);
                string 后缀名 = ".png";

                文件名 = 保存路径 + 无后缀名 + 后缀名;
                string 最终文件名 = 文件名;
                if (File.Exists(文件名)) {
                    int n = 1;
                    while (File.Exists(最终文件名 = 保存路径 + 无后缀名 + n.ToString() + 后缀名))
                        n++;
                }
                try {
                    图印页.原图.Save(最终文件名, ImageFormat.Png);
                }
                catch {
                    using (var 位图 = new Bitmap(图印页.原图))
                        位图.Save(最终文件名, ImageFormat.Png);
                }

                return Path.GetFileName(最终文件名);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private void 查找已保存的设置() {
            if (Directory.Exists(保存路径) == false) return;
            DirectoryInfo 文件夹信息 = new DirectoryInfo(保存路径);
            foreach (var 文件 in 文件夹信息.GetFiles())
                if (文件.Name.Length > 后缀名.Length && 文件.Name.Substring(文件.Name.Length - 后缀名.Length, 后缀名.Length) == 后缀名) {
                    string 设置名 = 文件.Name.Substring(0, 文件.Name.Length - 后缀名.Length);
                    bool 有重名 = false;
                    foreach (ToolStripMenuItem item in 已保存的水印设置ToolStripMenuItem.DropDownItems)
                        if (设置名 == item.Text) {
                            有重名 = true;
                            break;
                        }
                    if (有重名) continue;
                    ToolStripMenuItem 保存的设置 = new ToolStripMenuItem(设置名);
                    已保存的水印设置ToolStripMenuItem.DropDownItems.Add(保存的设置);
                    保存的设置.Click += 保存的设置_Click;
                }
        }

        private void 保存的设置_Click(object sender, EventArgs e) {
            string 文件名 = 保存路径 + ((ToolStripMenuItem)sender).Text + 后缀名;
            if (!File.Exists(文件名)) {
                MessageBox.Show(((ToolStripMenuItem)sender).Text + "这个设置文件已不存在! 刚被你删了吧?");
                已保存的水印设置ToolStripMenuItem.DropDownItems.Remove((ToolStripMenuItem)sender);
                return;
            }
            加载已保存的设置(文件名);

        }

        private void 加载已保存的设置(string 文件名) {
            FileStream 文件流 = new FileStream(文件名, FileMode.Open, FileAccess.Read);
            StreamReader 读 = new StreamReader(文件流);
            int 成功数 = 0;
            int 失败数 = 0;

            if (读.ReadLine() != "文印") {
                欢迎使用本软件ToolStripMenuItem.Text = "已成功读取设置 " + 成功数.ToString() + " 个 失败 " + 失败数.ToString() + " 个";
                return;
            }
            string 设置行 = 读.ReadLine();
            while (设置行 != null && 设置行 != "图印") {
                文印参数 参数 = new 文印参数();
                if (参数.字符转参数(设置行)) {
                    添加新文印设置页(参数);
                    成功数++;
                }
                else {
                    参数 = null;
                    失败数++;
                }
                设置行 = 读.ReadLine();
            }
            设置行 = 读.ReadLine();
            while (设置行 != null && 设置行.Length > 10) {
                图印参数 参数 = new 图印参数();
                if (参数.字符转参数(设置行)) {
                    if (参数.画啥 == 图形类别.图) {
                        if (File.Exists(保存路径 + 参数.图印文件名)) {
                            图印设置Tabpage 新图印设置页 = 添加新图印设置页(参数);
                            图印Tabpage 新图印页 = new 图印Tabpage(保存路径 + 参数.图印文件名, 图片水印tabPage, 图印右键菜单);
                            图片tab.Controls.Add(新图印页);
                            参数.图印页 = 新图印页;
                            新图印设置页.Text = 参数.图印页.Text;
                            水印图标签.Text = 参数.名称 = 参数.图印页.Text;
                            新图印设置页.本页参数.图印页.图印设置页 = 新图印设置页;
                            成功数++;
                        }
                        else {
                            MessageBox.Show(保存路径 + 参数.图印文件名 + "这个保存的水印图片不见了,被你删掉了吧!");
                            失败数++;
                        }
                    }
                    else {
                        添加新图印设置页(参数).Text = 参数.画啥.ToString() + 参数.宽高比.ToString();
                        成功数++;
                    }
                }
                else {
                    参数 = null;
                    失败数++;
                }


                设置行 = 读.ReadLine();
            }
            欢迎使用本软件ToolStripMenuItem.Text = "已成功读取设置 " + 成功数.ToString() + " 个 失败 " + 失败数.ToString() + " 个";
            if (成功数 > 0)
                水印参数_Changed(this, new EventArgs());
            读.Close();
            文件流.Close();
        }
        private string 获取保存文件名(string 后缀名过滤) {
            SaveFileDialog 保存文件对话框 = new SaveFileDialog();
            保存文件对话框.Filter = 后缀名过滤;
            保存文件对话框.Title = "另存图片文件";
            保存文件对话框.RestoreDirectory = true;
            保存文件对话框.OverwritePrompt = true;
            if (保存文件对话框.ShowDialog(this) == DialogResult.OK)
                return 保存文件对话框.FileName;
            else return null;
        }

        private string 获取文件夹名(string 描述) {
            string 文件夹名;
            FolderBrowserDialog 文件夹对话框 = new FolderBrowserDialog();
            文件夹对话框.Description = 描述;
            if (文件夹对话框.ShowDialog() == DialogResult.OK) {

                文件夹名 = 文件夹对话框.SelectedPath;
                return 文件夹名;
            }
            else
                return null;
        }
        private void 另存当前图片ToolStripMenuItem_Click(object sender, EventArgs e) {
            Bitmap 待存图片 = null;
            if (图片tab.SelectedTab is 图片Tabpage)
                待存图片 = ((图片Tabpage)图片tab.SelectedTab).水印图;
            if (图片tab.SelectedTab is 图印Tabpage)
                待存图片 = ((图印Tabpage)图片tab.SelectedTab).原图;
            if (待存图片 == null) {
                欢迎使用本软件ToolStripMenuItem.Text = "啥也没打开,存个啥!";
                return;
            }
            string 文件名 = 获取保存文件名("PNG文件(*.png) | *.png|BMP文件(*.bmp) | *.bmp|JPG文件(*.jpg) | *.jpg| ICO文件(*.ico) | *.ico");
            if (文件名 == null) return;
            ImageFormat 图片格式 = 获取图片格式(文件名.Substring(文件名.Length - 3, 3));
            try {
                待存图片.Save(文件名, 图片格式);
            }
            catch {
                using (var 位图 = new Bitmap(待存图片))
                    位图.Save(文件名, 图片格式);

            }
            欢迎使用本软件ToolStripMenuItem.Text = "已成功保存文件:" + 文件名;
        }

        static public ImageFormat 获取图片格式(string 扩展名) {
            switch (扩展名.ToLower()) {
                case "bmp":
                    return ImageFormat.Bmp;
                case "jpg":
                case "jpeg":
                    return ImageFormat.Jpeg;
                case "png":
                    return ImageFormat.Png;
                case "ico":
                    return ImageFormat.Icon;
                default:
                    MessageBox.Show("只能存bmp,jpg,png,ico四种文件类型!");
                    return null;
            }
        }

        private void 另存所有打开的图片ToolStripMenuItem_Click(object sender, EventArgs e) {
            int 图片总数 = 0;
            foreach (TabPage 页 in 图片tab.TabPages)
                if (页 is 图片Tabpage)
                    图片总数++;
            if (图片总数 == 0) {
                欢迎使用本软件ToolStripMenuItem.Text = "一个图片都没打开,存个啥?此功能不存被当作水印的图,只存被加水印的图!";
                return;
            }
            string 保存路径 = 获取文件夹名("请选择一个文件夹,用于保存所有已打开的图片") + @"\";
            int n = 0;

            if (Directory.Exists(保存路径)) {
                foreach (TabPage 页 in 图片tab.TabPages)
                    if (页 is 图片Tabpage) {
                        图片Tabpage 图片页 = (图片Tabpage)页;
                        if (图片页.是文件夹) return;
                        ImageFormat 图片格式 = 获取图片格式(Path.GetExtension(图片页.Text).Substring(1));
                        图片页.图片加水印(文印参数表, 图印参数表);
                        string 文件名 = 保存路径 + 图片页.Text;
                        DialogResult 覆盖询问 = DialogResult.Yes;
                        if (File.Exists(文件名))
                            覆盖询问 = MessageBox.Show(文件名 + "已存在,是否覆盖?", "是否覆盖图片文件?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (覆盖询问 == DialogResult.No) {
                            欢迎使用本软件ToolStripMenuItem.Text = 图片页.Text + "这个文件跳过了!";
                            continue;
                        }

                        try {
                            图片页.水印图.Save(文件名, 图片格式);
                        }
                        catch {
                            using (var 位图 = new Bitmap(图片页.水印图))
                                位图.Save(文件名, 图片格式);

                        }
                        n++;
                    }
                欢迎使用本软件ToolStripMenuItem.Text = "已成功保存" + n.ToString() + "个图片到 " + 保存路径 + " 路径下";
            }


        }

        private void 欢迎使用本软件ToolStripMenuItem_Click(object sender, EventArgs e) {
            欢迎使用本软件ToolStripMenuItem.Text = "欢迎使用本软件!";
        }

        private void timer1_Tick(object sender, EventArgs e) {
            switch (变色计数) {
                case 0:
                    欢迎使用本软件ToolStripMenuItem.ForeColor = Color.Orange;
                    变色计数++;
                    break;
                case 1:
                    欢迎使用本软件ToolStripMenuItem.ForeColor = Color.Yellow;
                    变色计数++;
                    break;
                case 2:
                    欢迎使用本软件ToolStripMenuItem.ForeColor = Color.Green;
                    变色计数 = 0;
                    timer1.Stop();
                    break;

                default:
                    break;
            }


        }

        private void 欢迎使用本软件ToolStripMenuItem_TextChanged(object sender, EventArgs e) {
            欢迎使用本软件ToolStripMenuItem.ForeColor = Color.Red;
            timer1.Start();
        }

        private void 打开图片ToolStripMenuItem_Click(object sender, EventArgs e) {
            string 文件夹 = 获取文件夹名("请选择要打开的文件夹:");
            if (文件夹 != null && Directory.Exists(文件夹)) {
                图片Tabpage 图片页 = new 图片Tabpage(文件夹, 图片框_MouseMove, 图片框_MouseDown, 图片框_MouseUp);
                if (!图片页.是文件夹) {
                    MessageBox.Show(文件夹 + " 这个文件夹里一个图片文件也没找到!啥也没打开!");
                    return;
                }
                当前图片Tab = 图片页;
                图片tab.TabPages.Add(当前图片Tab);
                图片tab.SelectedTab = 当前图片Tab;
                调整图片尺寸();
                当前位图 = (Bitmap)当前图片Tab.图片框.Image;
                当前图片框 = 当前图片Tab.图片框;
                欢迎使用本软件ToolStripMenuItem.Text = "已在文件夹 " + 文件夹 + " 中找到" + 当前图片Tab.所有图片列表.Count.ToString() + "个图片文件";
                水印参数_Changed(sender, e);
            }
        }

        private void 加水印_KeyDown(object sender, KeyEventArgs e) {
            图片Tabpage 图片页 = 图片tab.SelectedTab as 图片Tabpage;
            if (图片页 != null && 图片页.是文件夹) {
                int d = 0;
                if (e.KeyCode == Keys.PageUp) d = -1;
                if (e.KeyCode == Keys.PageDown) d = 1;
                if (d != 0) {
                    图片页.上下翻图片(d);
                    调整图片尺寸();
                    水印参数_Changed(sender, e);
                }
            }
            图印Tabpage 图印页 = 图片tab.SelectedTab as 图印Tabpage;
            if (图印页 != null) {
                if (e.KeyCode == Keys.Back)
                    图印页.回退变透明();
                if (e.KeyCode == Keys.ControlKey)
                    图印页.Ctrl状态(true);
            }
        }

        private void 另存文件夹ToolStripMenuItem_Click(object sender, EventArgs e) {
            if (图片tab.SelectedTab == null || !(图片tab.SelectedTab is 图片Tabpage) || !((图片Tabpage)图片tab.SelectedTab).是文件夹) {
                MessageBox.Show("请先选择一个打开的文件夹页面!");
                return;
            }
            欢迎使用本软件ToolStripMenuItem.Text = "请注意:保存文件夹时不会提示覆盖已有文件,请最好选择空文件夹";
            图片Tabpage 图片页 = (图片Tabpage)图片tab.SelectedTab;
            string 文件夹 = 获取文件夹名("请选择要保存到哪个文件夹:");
            保存到文件夹(文件夹, 图片页);
        }

        private void 保存到文件夹(string 文件夹, 图片Tabpage 图片页) {
            if (文件夹 == null) return;
            if (文件夹 == 图片页.路径) {
                DialogResult 覆盖询问 = DialogResult.Yes;
                覆盖询问 = MessageBox.Show("这个目录就是你打开的目录,这样保存会导致原文件都被覆盖,要继续吗???", "是否继续保存?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (覆盖询问 == DialogResult.No) {
                    欢迎使用本软件ToolStripMenuItem.Text = "啥也没保存,请再选个文件夹吧!";
                    return;
                }
                else {
                    覆盖询问 = MessageBox.Show("真的要覆盖原来的目录?", "是否继续保存?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (覆盖询问 == DialogResult.No) {
                        欢迎使用本软件ToolStripMenuItem.Text = "啥也没保存,请再选个文件夹吧!";
                        return;
                    }
                }
            }
            int N = 图片页.保存所有图片到文件夹(文件夹, 文印参数表, 图印参数表, 进度条);
            欢迎使用本软件ToolStripMenuItem.Text = "已在文件夹 " + 文件夹 + " 中保存了" + N.ToString() + "个图片文件";

        }

        private void linkLabel关闭说明_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            图片tab.Controls.Remove(说明);
        }

        private void 使用说明ToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!图片tab.Controls.Contains(说明)) {
                图片tab.Controls.Add(说明);
                图片tab.SelectedTab = 说明;
            }
            else
                欢迎使用本软件ToolStripMenuItem.Text = "说明页已经显示了";
        }

        private void 波动幅度滑动条_Scroll(object sender, EventArgs e) {
            toolTip1.SetToolTip(波动幅度滑动条, 波动幅度滑动条.Value.ToString());
            水印参数_Changed(sender, e);
        }

        private void 加水印_Load(object sender, EventArgs e) {
            CheckForIllegalCrossThreadCalls = false;
        }

        private void 加水印_KeyUp(object sender, KeyEventArgs e) {
            图印Tabpage 图印页 = 图片tab.SelectedTab as 图印Tabpage;
            if (图印页 != null) {
                if (e.KeyCode == Keys.ControlKey)
                    图印页.Ctrl状态(false);
            }
        }

        private void 無限次元首页链接_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer.exe", @"https://space.bilibili.com/2139404925");
        }
    }
}
