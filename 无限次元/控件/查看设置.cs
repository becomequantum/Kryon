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

namespace �ؼ�{
    public partial class �鿴���� : UserControl {
        public �鿴����() {
            InitializeComponent();
            comboBox�Ŵ���.SelectedIndex = 1;
            comboBox���ĸ���.SelectedIndex = 10;
        }

        private void textBox��ʼλ��_TextChanged(object sender, EventArgs e) {
            ((TextBox)sender).Text = Regex.Replace(((TextBox)sender).Text, @"[^\d]*", "");//ȥ��������
        }

        private void textBox��ʼλ��_Leave(object sender, EventArgs e) {
            int ֵ;
            TextBox �ı��� = (TextBox)sender;
            if (!int.TryParse(�ı���.Text, out ֵ))
                �ı���.Text = "0";
        }

        private void button�ر�_Click(object sender, EventArgs e) {
            ((Mnist�鿴��ǩҳ)Parent).�ر�();
        }

        private void button������ͼ_Click(object sender, EventArgs e) {
            Bitmap λͼ = ((Mnist�鿴��ǩҳ)Parent).�鿴ͼƬ;
            if (λͼ == null) return;
            ����ͼƬ(λͼ);
        }

        private void button������ͼ_Click(object sender, EventArgs e) {
            Bitmap λͼ = ((Mnist�鿴��ǩҳ)Parent).ԭ����ͼ;
            if (λͼ == null || ((Mnist�鿴��ǩҳ)Parent).ԭ����ͼ == null) return;
            ����ͼƬ(λͼ);
        }

        private void ����ͼƬ(Bitmap λͼ) {
            string �ļ��� = ��ȡ�����ļ���("PNG�ļ�(*.png) | *.png|BMP�ļ�(*.bmp) | *.bmp");
            if (�ļ��� == null) return;
            ImageFormat ͼƬ��ʽ;
            switch (�ļ���.Substring(�ļ���.Length - 3, 3).ToLower()) {
                case "bmp":
                    ͼƬ��ʽ = ImageFormat.Bmp;
                    break;
                case "png":
                    ͼƬ��ʽ = ImageFormat.Png;
                    break;
                default:
                    MessageBox.Show("ֻ�ܴ�bmp,png�����ļ�����!");
                    return;
            }
            try {
                λͼ.Save(�ļ���, ͼƬ��ʽ);
            }
            catch {
                using (var λͼ1 = new Bitmap(λͼ))
                    λͼ1.Save(�ļ���, ͼƬ��ʽ);

            }
        }

        private string ��ȡ�����ļ���(string ��׺������) {
            SaveFileDialog �����ļ��Ի��� = new SaveFileDialog();
            �����ļ��Ի���.Filter = ��׺������;
            �����ļ��Ի���.Title = "���ͼƬ�ļ�";
            �����ļ��Ի���.RestoreDirectory = true;
            �����ļ��Ի���.OverwritePrompt = true;
            if (�����ļ��Ի���.ShowDialog(this) == DialogResult.OK)
                return �����ļ��Ի���.FileName;
            else return null;
        }
    }
}
