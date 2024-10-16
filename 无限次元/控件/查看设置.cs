using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;

namespace 控件{
    public partial class 查看设置 : UserControl {
        public 查看设置() {
            InitializeComponent();
            comboBox放大倍数.SelectedIndex = 1;
            comboBox读哪个数.SelectedIndex = 10;
        }

        private void textBox起始位置_TextChanged(object sender, EventArgs e) {
            ((TextBox)sender).Text = Regex.Replace(((TextBox)sender).Text, @"[^\d]*", "");//去掉非数字
        }

        private void textBox起始位置_Leave(object sender, EventArgs e) {
            int 值;
            TextBox 文本框 = (TextBox)sender;
            if (!int.TryParse(文本框.Text, out 值))
                文本框.Text = "0";
        }

        private void button关闭_Click(object sender, EventArgs e) {
            ((Mnist查看标签页)Parent).关闭();
        }

        private void button保存右图_Click(object sender, EventArgs e) {
            Bitmap 位图 = ((Mnist查看标签页)Parent).查看图片;
            if (位图 == null) return;
            保存图片(位图);
        }

        private void button保存上图_Click(object sender, EventArgs e) {
            Bitmap 位图 = ((Mnist查看标签页)Parent).原数字图;
            if (位图 == null || ((Mnist查看标签页)Parent).原数字图 == null) return;
            保存图片(位图);
        }

        private void 保存图片(Bitmap 位图) {
            string 文件名 = 获取保存文件名("PNG文件(*.png) | *.png|BMP文件(*.bmp) | *.bmp");
            if (文件名 == null) return;
            ImageFormat 图片格式;
            switch (文件名.Substring(文件名.Length - 3, 3).ToLower()) {
                case "bmp":
                    图片格式 = ImageFormat.Bmp;
                    break;
                case "png":
                    图片格式 = ImageFormat.Png;
                    break;
                default:
                    MessageBox.Show("只能存bmp,png两种文件类型!");
                    return;
            }
            try {
                位图.Save(文件名, 图片格式);
            }
            catch {
                using (var 位图1 = new Bitmap(位图))
                    位图1.Save(文件名, 图片格式);

            }
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
    }
}
