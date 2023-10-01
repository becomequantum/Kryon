//VS2022,如果把自定义控件放在另外一个程序集里,每新添加一个用户控件到主窗体上时可能会报错,
//是引用问题,添加新的类代码后要把对这个程序集的引用取消掉,然后再添加上,就不会报错了!
//如果遇到自定义控件界面修改后,主窗体设计器上添加的自定义控件就是不反应最新修改时,可以删除该控件再撤销,该控件界面就会更新.
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace 控件;
public partial class 功能标签页 : UserControl {
    Control[] 图像处理控件;
    Control[] 神经网络控件;
    Dictionary<ToolStripButton, Control[]> 控件切换表 = new Dictionary<ToolStripButton, Control[]>();
    ColorDialog 选色对话框 = new();

    [Description("按钮点击事件"), Category("自定义事件")]
    public event EventHandler 图像处理按钮点击;

    [Description("按钮点击事件"), Category("自定义事件")]
    public event EventHandler 神经网络按钮点击;

    [Description("按钮点击事件"), Category("自定义事件")]
    public event EventHandler 画啥按钮点击;

    [Description("按钮点击事件"), Category("自定义事件")]
    public event EventHandler 显示RGB点击;
    public 功能标签页() {
        InitializeComponent();
        初始化();
    }


    private void 初始化() {
        图像处理控件 = new Control[panel1.Controls.Count];
        panel1.Controls.CopyTo(图像处理控件, 0);
        神经网络控件 = new Control[神经网络panel.Controls.Count];
        神经网络panel.Controls.CopyTo(神经网络控件, 0);
        控件切换表.Add(图像处理按钮, 图像处理控件);
        控件切换表.Add(神经网络按钮, 神经网络控件);
        设置上下限标签颜色();
        画啥按钮点击?.Invoke(radioButton不画, new EventArgs());
    }

    public void 取消画图() {
        radioButton不画.Checked = true;
    }
    public void 更新HSL上下限(Color HSL上限, Color HSL下限) {
        (H上.Text, S上.Text, L上.Text) = (HSL上限.R.ToString(), HSL上限.G.ToString(), HSL上限.B.ToString());
        (H下.Text, S下.Text, L下.Text) = (HSL下限.R.ToString(), HSL下限.G.ToString(), HSL下限.B.ToString());
    }

    private void 设置上下限标签颜色() {
        int R = 0, G = 0, B = 0;
        int H = 0, S = 0, L = 0;
        try {
            H = Convert.ToByte(H上.Text);
            S = 240; L = 100;
            图像.HSL转RGB(H, S, L, ref R, ref G, ref B);
            上限标签.ForeColor = Color.FromArgb(R, G, B);
            H = Convert.ToByte(H下.Text);
            图像.HSL转RGB(H, S, L, ref R, ref G, ref B);
            下限标签.ForeColor = Color.FromArgb(R, G, B);
        } catch (Exception) {
        }

    }

    private void 读参数() {
        try {
            (参数.宽, 参数.高) = (Convert.ToInt32(图宽.Text), Convert.ToInt32(图高.Text));
            参数.颜色 = Color.FromArgb(Convert.ToByte(R值.Text), Convert.ToByte(G值.Text), Convert.ToByte(B值.Text));
            (参数.红比例, 参数.绿比例, 参数.蓝比例) = (Convert.ToSingle(R比例.Text), Convert.ToSingle(G比例.Text), Convert.ToSingle(B比例.Text));
            (参数.W, 参数.b) = (Convert.ToSingle(W值.Text), Convert.ToSingle(Bias值.Text));
            参数.HSL上限 = Color.FromArgb(Convert.ToByte(H上.Text), Convert.ToByte(S上.Text), Convert.ToByte(L上.Text));
            参数.HSL下限 = Color.FromArgb(Convert.ToByte(H下.Text), Convert.ToByte(S下.Text), Convert.ToByte(L下.Text));
            参数.显示连通域标记过程 = 是否显示过程.Checked;
            参数.算子直径 = Convert.ToInt32(算子直径.Text);
            参数.胀蚀阈值 = Convert.ToInt32(胀蚀阈值.Text);
            if (参数.胀蚀阈值 == 0) { 参数.胀蚀阈值 = 1; 胀蚀阈值.Text = "1"; }
            if (参数.胀蚀阈值 > 参数.算子直径 * 参数.算子直径) { 参数.胀蚀阈值 = 参数.算子直径 * 参数.算子直径; 胀蚀阈值.Text = 参数.胀蚀阈值.ToString(); }

        } catch (Exception exp) {
            MessageBox.Show(exp.Message + "\n刚才的值设的有问题!");
        }
    }

    public void 设置颜色(Color 颜色) {
        (R值.Text, G值.Text, B值.Text) = (颜色.R.ToString(), 颜色.G.ToString(), 颜色.B.ToString());
    }

    #region 事件响应
    private void 图像处理按钮们_Click(object sender, EventArgs e) {
        读参数();
        图像处理按钮点击?.Invoke(sender, e);
    }
    private void 笔粗细_TextChanged(object sender, EventArgs e) {
        if (笔粗细.Text.Length == 0) return;
        try {
            参数.笔粗细 = Convert.ToInt32(笔粗细.Text);
        } catch (Exception exp) {
            MessageBox.Show(exp.Message + "\n刚才的值设的有问题!");
        }
    }
    private void 神经网络按钮们_Click(object sender, EventArgs e) {
        神经网络按钮点击?.Invoke(sender, e);
    }

    private void radioButton_Click(object sender, EventArgs e) {
        画啥按钮点击?.Invoke(sender, e);
    }
    private void 显示RGB_CheckedChanged(object sender, EventArgs e) {
        CheckBox checkBox = (CheckBox)sender;
        if (checkBox.Name == "显示边线") 参数.显示.ALSD = checkBox.Checked;
        else if (checkBox.Name == "黑掉背景") 参数.显示.黑 = checkBox.Checked;
        读参数();
        显示RGB点击?.Invoke(sender, e);
    }

    private void 切换按钮_Click(object sender, EventArgs e) {
        foreach (var item in 切换工具栏.Items) {
            ToolStripButton button = (ToolStripButton)item;
            button.Font = new Font(button.Font.FontFamily, button.Font.Size, FontStyle.Regular);
        }//先把所有的变为没加粗
        ToolStripButton 按钮 = (ToolStripButton)sender;
        按钮.Font = new Font(按钮.Font.FontFamily, 按钮.Font.Size, FontStyle.Bold);//再把被点击的变为粗体
        panel1.Controls.Clear();
        panel1.Controls.AddRange(控件切换表[按钮]);

    }

    private void 位宽选择_Click(object sender, EventArgs e) {
        位宽选择.Text = ((ToolStripMenuItem)sender).Text;
        if (位宽选择.Text == "32位")
            参数.像素格式 = PixelFormat.Format32bppArgb;
        else
            参数.像素格式 = PixelFormat.Format24bppRgb;
    }//选择24位还是32位图

    private void 选色按钮_Click(object sender, EventArgs e) {
        if (选色对话框.ShowDialog() == DialogResult.OK) {
            选色按钮.ForeColor = 选色对话框.Color;
            R值.Text = 选色对话框.Color.R.ToString();
            G值.Text = 选色对话框.Color.G.ToString();
            B值.Text = 选色对话框.Color.B.ToString();
        }
    }//打开选色对话框,选择一个颜色

    private void Wb选值改变(object sender, EventArgs e) {
        if (string.IsNullOrEmpty(Wb选值.Text)) return;
        string[] Wb值 = Wb选值.Text.Split(" ");
        W值.Text = Wb值[0];
        Bias值.Text = Wb值[1];
    }//ComboBox选择常用W,b值

    private void HSL上下限_TextChanged(object sender, EventArgs e) {
        ToolStripTextBox 文本框 = (ToolStripTextBox)sender;
        文本框.Text = 限制字符为正整数(文本框.Text, 240);
        if (文本框 == H上 || 文本框 == H下 && 文本框.Text.Length > 0) 设置上下限标签颜色();
    }
    private void RGB_TextChanged(object sender, EventArgs e) {
        ToolStripTextBox 文本框 = (ToolStripTextBox)sender;
        文本框.Text = 限制字符为正整数(文本框.Text, 255);
        if (文本框.Text.Length > 0)
            RGB标签.ForeColor = Color.FromArgb(Convert.ToInt32(R值.Text), Convert.ToInt32(G值.Text), Convert.ToInt32(B值.Text));
    }

    private void 胀蚀阈值_TextChanged(object sender, EventArgs e) {
        胀蚀阈值.Text = 限制字符为正整数(胀蚀阈值.Text, 参数.算子直径 * 参数.算子直径); //阈值的最大值是直径的平方
    }

    private void 算子直径_Validated(object sender, EventArgs e) {
        算子直径.Text = 限制字符为正整数(算子直径.Text, 99);
        int 直径;
        if (int.TryParse(算子直径.Text, out 直径)) {
            if (直径 < 3) 直径 = 3;
            if (直径 % 2 == 0) 直径++;
            算子直径.Text = 直径.ToString();
            参数.算子直径 = 直径;
        }
    }
    private void H使能_CheckedChanged(object sender, EventArgs e) {
        参数.H使能 = H使能.Checked;
        参数.S使能 = S使能.Checked;
        参数.L使能 = L使能.Checked;
    }



    //無限次元:https://space.bilibili.com/2139404925
    //本代码来自:https://github.com/becomequantum/Kryon
    #endregion



    private string 限制字符为正整数(string S, int 上限) {
        S = Regex.Replace(S, @"[^\d]*", "");
        if (!string.IsNullOrEmpty(S) && S.Length >= 上限.ToString().Length) {
            int 值 = Convert.ToInt32(S);
            值 = 值 > 上限 ? 上限 : 值;
            S = 值.ToString();
        }
        return S;
    }

    private void B比例_MouseLeave(object sender, EventArgs e) {
        读参数();
    }


}



