using System.Drawing.Imaging;

namespace �ؼ�;
public class Mnist�鿴��ǩҳ : TabPage, I��ǩҳ {
    public static string Mnist·�� = @"C:\"; //��ΪMnist���������ļ���
    public static string ѵ��ͼƬ�� = @"train-images.idx3-ubyte";//�ļ�������ҲҪ��
    public static string ѵ����ǩ�� = @"train-labels.idx1-ubyte";
    public static string ����ͼƬ�� = @"t10k-images.idx3-ubyte";
    public static string ���Ա�ǩ�� = @"t10k-labels.idx1-ubyte";
    private const int ͸ = 3, �� = 2, �� = 1, �� = 0; //Bitmapλͼ������,��ɫBlue��ֵ���ڵ͵�ַ��
    static int ����� = 1600;
    static int ����� = 900;
    int W;  //һ�в鿴���ٸ���д����
    int H;  //������
    int N;       //�ұߴ�ͼ���ж�����ͼƬ
    static int MnistSize = 28; //Mnist�ַ����
    int �Ŵ��� = 2;
    static int �߿��� = 1;
    int ͼ�ߴ�;
    public static int ѵ��ͼƬ�� = 60000;
    public static int ����ͼƬ�� = 10000;
    static byte[] ѵ������;
    static byte[] ��������;
    public PictureBox �鿴ͼƬ��;
    PictureBox �Ŵ�ͼƬ��;
    PictureBox �ƶ���ʾͼƬ��;
    public Bitmap �鿴ͼƬ;
    public Bitmap �Ŵ�ͼƬ = new Bitmap(MnistSize * 11, MnistSize * 11, PixelFormat.Format24bppRgb);
    Bitmap ����ͼƬ = new Bitmap(MnistSize * 11, MnistSize * 11, PixelFormat.Format24bppRgb);
    public Bitmap ԭ����ͼ = new Bitmap(MnistSize, MnistSize, PixelFormat.Format24bppRgb);
    �鿴���� ����;
    static List<int>[] ѵ�����ֱ�ǩ��;
    static List<int>[] �������ֱ�ǩ��;
    static byte[] ѵ����ǩ;
    static byte[] ���Ա�ǩ;
    int[] ���;             //�������Ҫ��ͼƬ�����
    List<int> �������;
    byte[] �����ǩ;
    bool ��ǰͼƬ��ѵ���� = true;
    int ��ǰѡ����� = 0;
    int ��ǰ������� = -1;
    Rectangle ��ǰѡ�п� = Rectangle.Empty;


    public Mnist�鿴��ǩҳ() {
        Text = "Mnist�鿴";
        �ƶ���ʾͼƬ�� = new PictureBox() {
            SizeMode = PictureBoxSizeMode.AutoSize,
            Location = new Point(0, 0)
        };
        Controls.Add(�ƶ���ʾͼƬ��);
        �鿴ͼƬ�� = new PictureBox() {
            SizeMode = PictureBoxSizeMode.AutoSize,
            Location = new Point(310, 0)
        };
        Controls.Add(�鿴ͼƬ��);
        �Ŵ�ͼƬ�� = new PictureBox() {
            SizeMode = PictureBoxSizeMode.AutoSize,
            Location = new Point(0, 310)
        };
        Controls.Add(�Ŵ�ͼƬ��);
        ���� = new �鿴����() {
            Location = new Point(0, 620)
        };
        Controls.Add(����);
        ����.��֮��ť.Click += ��֮��ť_Click;
        ����.radioButton���Լ�.CheckedChanged += RadioButton���Լ�_CheckedChanged;
        �鿴ͼƬ��.MouseMove += �鿴ͼƬ��_MouseMove;
        �鿴ͼƬ��.MouseClick += �鿴ͼƬ��_MouseClick;
        �Ŵ�ͼƬ��.MouseMove += �Ŵ�ͼƬ��_MouseMove;
        ����.comboBox���ĸ���.SelectedIndexChanged += ComboBox���ĸ���_SelectedIndexChanged;
        ����.��һҳ.Click += ��һҳ_Click;
        ����ÿ������ͼƬ����Ŀ();
        ����Mnist����();

    }
    public Mnist�鿴��ǩҳ(string �ļ���) : this() {
        string ͷ���� = Path.GetFileName(�ļ���).Substring(0, 2);
        if (ͷ���� == "ѵ��") {
            ��ǰͼƬ��ѵ���� = true;
            Text = "ѵ�����������";
            ����.radioButtonѵ����.Checked = true;
        }
        else if (ͷ���� == "����") {
            ��ǰͼƬ��ѵ���� = false;
            Text = "���Լ��������";
            ����.radioButton���Լ�.Checked = true;
        }
        else {
            MessageBox.Show("�����ļ�����ǰ���ע��'ѵ����'���ǡ����Լ���");
        }
        if (��ȡ����ͼƬ��Ϣ(�ļ���)) {
            ��֮(true);
        }
        ����.comboBox�Ŵ���.Enabled = false;
        ����.radioButtonѵ����.Enabled = false;
        ����.radioButton���Լ�.Enabled = false;
    }

    private void ����Mnist����() {
        if (ѵ������ != null) return;
        if (!(File.Exists(Mnist·�� + ѵ����ǩ��) && File.Exists(Mnist·�� + ���Ա�ǩ��))) {
            Mnist·�� = �ļ�.���ļ���("��ָ��Mnist���ݼ��ļ�����Ŀ¼:") + @"\";
        }
        ��������();     //��Mnistͼ�����ݶ����ص��ֽ�������
        ��ʼ����ǩ��();
    }

    private void ��һҳ_Click(object sender, EventArgs e) {
        int ��ʼλ�� = Convert.ToInt32(����.textBox��ʼλ��.Text);
        //if (N == null) return;
        ��ʼλ�� -= N;
        if (��ʼλ�� >= N)
            ��ʼλ�� -= N;
        else
            ��ʼλ�� = 0;
        ����.textBox��ʼλ��.Text = ��ʼλ��.ToString();
        ��֮(������� != null);
    }

    private void ComboBox���ĸ���_SelectedIndexChanged(object sender, EventArgs e) {
        ����.textBox��ʼλ��.Text = "0";
    }


    private bool ��ȡ����ͼƬ��Ϣ(string �ļ���) {
        �����ǩ = new byte[ѵ��ͼƬ��];
        ������� = new List<int>();
        try {
            using (StreamReader ���ļ� = new StreamReader(�ļ���)) {
                string һ��;
                while ((һ�� = ���ļ�.ReadLine()) != null) {
                    string[] ������ = һ��.Split(new char[] { ' ' });
                    int i = Convert.ToInt32(������[0]);
                    �������.Add(i);
                    �����ǩ[i] = (byte)Convert.ToInt32(������[1]);
                }
            }
        }
        catch (Exception e) {
            MessageBox.Show("���ļ�����" + �ļ���);
            return false;
        }
        if (�������.Count > 0)
            return true;
        else
            return false;
    }

    /// <summary>
    /// ��Mnist���ݼ��ص��ֽ�������
    /// </summary>
    static void ��������() {
        if (!(File.Exists(Mnist·�� + ѵ��ͼƬ��) && File.Exists(Mnist·�� + ����ͼƬ��))) {
            MessageBox.Show("û�ҵ�����Mnist�����ļ�(����Ҫ�޸�'Mnist�鿴��ǩҳ.cs'��������ļ���): " + ѵ��ͼƬ�� + "\n" + ����ͼƬ��);
            return;
        }
        FileStream �ļ��� = new (Mnist·�� + ѵ��ͼƬ��, FileMode.Open);
        ѵ������ = new byte[�ļ���.Length];
        �ļ���.Read(ѵ������, 0, ѵ������.Length);
        �ļ���.Close();
        �ļ��� = new (Mnist·�� + ����ͼƬ��, FileMode.Open);
        �������� = new byte[�ļ���.Length];
        �ļ���.Read(��������, 0, ��������.Length);
        �ļ���.Close();
    }

    private void �Ŵ�ͼƬ��_MouseMove(object sender, MouseEventArgs e) {
        if (��� == null) return;
        Point ���� = �Ŵ�ͼƬ��.PointToClient(Cursor.Position);
        if (����.X < 0 || ����.Y < 0 || ����.X >= �鿴ͼƬ��.Image.Width || ����.Y >= �鿴ͼƬ��.Image.Height) return;
        ����.label�Ҷ�ֵ.Text = �Ŵ�ͼƬ.GetPixel(����.X, ����.Y).G.ToString(); //��ʾ������ڵ�ĻҶ�ֵ
    }

    private void �鿴ͼƬ��_MouseClick(object sender, MouseEventArgs e) {
        if (�鿴ͼƬ��.Image == null) return;
        Point ���� = �鿴ͼƬ��.PointToClient(Cursor.Position);
        int x��� = ����.X / ͼ�ߴ�;
        int y��� = ����.Y / ͼ�ߴ�;
        int ͼ��� = y��� * W + x���;
        if (��� == null || ͼ��� < 0 || ͼ��� >= ���.Length || �鿴ͼƬ��.Image == null || ͼ��� == ��ǰѡ�����) return;
        ��ѡ�п�(x���, y���, ͼ���);
        ���Ŵ�ͼ(ͼ���, �Ŵ�ͼƬ);
        �Ŵ�ͼƬ��.Refresh();
    }
    private void ��ѡ�п�(int x, int y, int ͼ���) {
        Rectangle ѡ�п� = new Rectangle(x * ͼ�ߴ�, y * ͼ�ߴ�, ͼ�ߴ�, ͼ�ߴ�);
        Graphics g = Graphics.FromImage(�鿴ͼƬ��.Image);
        if (��ǰѡ�п� != Rectangle.Empty)
            g.DrawRectangle(new Pen(Color.FromArgb(128, 128, 128)), ��ǰѡ�п�);
        g.DrawRectangle(new Pen(Color.LightGreen), ѡ�п�);
        �鿴ͼƬ��.Invalidate(new Rectangle(ѡ�п�.X, ѡ�п�.Y, ѡ�п�.Width + 1, ѡ�п�.Height + 1));
        �鿴ͼƬ��.Invalidate(new Rectangle(��ǰѡ�п�.X, ��ǰѡ�п�.Y, ��ǰѡ�п�.Width + 1, ��ǰѡ�п�.Height + 1));
        ��ǰѡ�п� = ѡ�п�;
        ��ǰѡ����� = ͼ���;
        g.Dispose();
    }

    private void �鿴ͼƬ��_MouseMove(object sender, MouseEventArgs e) {
        if (�鿴ͼƬ��.Image == null) return;
        Point ���� = �鿴ͼƬ��.PointToClient(Cursor.Position);
        int x��� = ����.X / ͼ�ߴ�;
        int y��� = ����.Y / ͼ�ߴ�;
        int ͼ��� = y��� * W + x���;
        if (��� == null || ͼ��� < 0 || ͼ��� >= ���.Length) return;
        if (!��ǰͼƬ��ѵ����) {
            ����.label�������.Text = ���[ͼ���].ToString();
            ����.labelѵ�����.Text = "";
        }
        else {
            ����.labelѵ�����.Text = ���[ͼ���].ToString();
            ����.label�������.Text = "";
        }
        if (ͼ��� != ��ǰ�������) {
            ���Ŵ�ͼ(ͼ���, ����ͼƬ);
            �ƶ���ʾͼƬ��.Image = ����ͼƬ;
            ��ǰ������� = ͼ���;
        }

    }

    private void RadioButton���Լ�_CheckedChanged(object sender, EventArgs e) {
        ����ÿ������ͼƬ����Ŀ();
    }

    private void ��֮(bool ������ = false) {
        byte[] ���� = ѵ������;
        byte[] ��ǩ = ѵ����ǩ;
        int ͼƬ���� = ѵ��ͼƬ��;
        ��ǰͼƬ��ѵ���� = true;
        List<int>[] ��ǩ�� = ѵ�����ֱ�ǩ��;
        if (����.radioButton���Լ�.Checked) {
            ͼƬ���� = ����ͼƬ��;
            ��ǩ�� = �������ֱ�ǩ��;
            ���� = ��������;
            ��ǩ = ���Ա�ǩ;
            ��ǰͼƬ��ѵ���� = false;
        }


        �Ŵ��� = ����.comboBox�Ŵ���.SelectedIndex + 1;
        ͼ�ߴ� = MnistSize * �Ŵ��� + �߿���;
        W = ����� / ͼ�ߴ�;
        H = ����� / ͼ�ߴ�;
        N = W * H;             //����ұߵĴ�ͼ����ʾ���ٸ�����
        if (�鿴ͼƬ == null)
            �鿴ͼƬ = new Bitmap(W * ͼ�ߴ� + 1, H * ͼ�ߴ� + 1, PixelFormat.Format32bppArgb);

        int ���ĸ��� = ����.comboBox���ĸ���.SelectedIndex;
        int ��ʼλ�� = Convert.ToInt32(����.textBox��ʼλ��.Text);
        if (������) {
            if (�������.Count <= N && �鿴ͼƬ��.Image != null)
                return;

            int ��ȡ��Ŀ = N;
            if (�������.Count - ��ʼλ�� < N)
                ��ȡ��Ŀ = �������.Count - ��ʼλ��;
            if (��ȡ��Ŀ <= 0) return;
            ��� = �������.GetRange(��ʼλ��, ��ȡ��Ŀ).ToArray();
            ����.textBox��ʼλ��.Text = (��ʼλ�� + ��ȡ��Ŀ).ToString();
        }
        else if (����.radioButton˳���.Checked) {
            ��� = new int[N];
            for (int i = 0; i < ���.Length; i++) {
                ���[i] = i + ��ʼλ��;
                if (���ĸ��� < 10) {
                    ���[i] = ���[i] >= ��ǩ��[���ĸ���].Count ? ���[i] % ��ǩ��[���ĸ���].Count : ���[i];
                    if (i == ���.Length - 1)
                        ����.textBox��ʼλ��.Text = (���[i] + 1).ToString();
                    ���[i] = ��ǩ��[���ĸ���][���[i]];
                }
                else {
                    ���[i] = ���[i] >= ͼƬ���� ? ���[i] % ͼƬ���� : ���[i];
                    if (i == ���.Length - 1)
                        ����.textBox��ʼλ��.Text = (���[i] + 1).ToString();
                }
            }

        }
        else if (����.radioButton�����.Checked) {
            ��� = new int[N];
            if (���ĸ��� < 10) {
                ͼƬ���� = ��ǩ��[���ĸ���].Count;
                ����������(���, ͼƬ����);
                for (int i = 0; i < ���.Length; i++)
                    ���[i] = ��ǩ��[���ĸ���][���[i]];
            }
            else
                ����������(���, ͼƬ����);
        }


        ��Mnist����ͼƬ(����, ��ǩ, ���);
        �鿴ͼƬ��.Image = �鿴ͼƬ;
        ��ǰѡ����� = 0;
        ��ǰѡ�п� = Rectangle.Empty;
        ��ѡ�п�(0, 0, 0);
        ���Ŵ�ͼ(0, �Ŵ�ͼƬ);
        �Ŵ�ͼƬ��.Image = �Ŵ�ͼƬ;
        ��ǰ������� = -1;
    }

    private void ��֮��ť_Click(object sender, EventArgs e) {
        ((Button)sender).Enabled = false;
        ��֮(������� != null);
        ((Button)sender).Enabled = true;
    }

    private void ��Mnist����ͼƬ(byte[] ����, byte[] ��ǩ, int[] ���) {
        BitmapData ͼ���� = �鿴ͼƬ.LockBits(new Rectangle(0, 0, �鿴ͼƬ.Width, �鿴ͼƬ.Height), ImageLockMode.ReadWrite, �鿴ͼƬ.PixelFormat);
        byte �Ҷ�ֵ = 0;
        int �������� = 0;
        unsafe {
            byte* ͼָ�� = (byte*)(ͼ����.Scan0);
            for (int y = 0; y < ͼ����.Height; y++)  //���������"λͼ����.Height",��С��д��"λͼ.Height"�ͻᵼ�³���Ī�������ܶ�.
                for (int x = 0; x < ͼ����.Width; x++, ͼָ�� += 4) {

                    if (����.��ʾ�߿�.Checked && (x == ͼ����.Width - 1 || y == ͼ����.Height - 1 || x % ͼ�ߴ� == 0 || y % ͼ�ߴ� == 0)) {
                        ͼָ��[��] = ͼָ��[��] = ͼָ��[��] = 128;
                        ͼָ��[͸] = 255;
                    }//���߿�
                    else {
                        int x��� = x / ͼ�ߴ�;                        //ĳһ�еĵڼ���ͼ
                        int y��� = y / ͼ�ߴ�;                        //�ڼ���
                        int ͼ��� = y��� * W + x���;                //��������еĵڼ���
                        int ͼX = (x - x��� * ͼ�ߴ� - �߿���) / �Ŵ���; //����ͼƬ�ϵ�X����
                        int ͼY = (y - y��� * ͼ�ߴ� - �߿���) / �Ŵ���;
                        if ((x - x��� * ͼ�ߴ� - �߿���) % �Ŵ��� == 0 && ͼ��� < ���.Length) {
                            if (ͼX == 0)
                                �������� = 4 * 4 + ���[ͼ���] * MnistSize * MnistSize + ͼY * MnistSize + ͼX;
                            �Ҷ�ֵ = ����[��������];
                            ��������++;
                        }
                        ͼָ��[��] = ͼָ��[��] = ͼָ��[��] = �Ҷ�ֵ;
                        ͼָ��[͸] = 255;
                    }
                }
        }

        �鿴ͼƬ.UnlockBits(ͼ����);
        Graphics g = Graphics.FromImage(�鿴ͼƬ);
        if (����.��ʾ��ǩ.Checked) {
            for (int i = 0; i < ���.Length; i++) {
                g.DrawString(��ǩ[���[i]].ToString(), TimesNR����(7 * �Ŵ���), new SolidBrush(Color.LightGreen), new Point(i % W * ͼ�ߴ� - 1, i / W * ͼ�ߴ� - 1));
                if (�����ǩ != null)
                    g.DrawString(�����ǩ[���[i]].ToString(), TimesNR����(7 * �Ŵ���), new SolidBrush(Color.Red), new Point(i % W * ͼ�ߴ� - 1, i / W * ͼ�ߴ� + 7 * �Ŵ���));
            }//����ǩ
        }
        g.Dispose();
    }

    /// <summary>
    /// ����������ұ�ͼƬ���ƶ�ʱ���������ʾ�Ŵ������ͼ
    /// </summary>
    /// <param name="ͼ��"></param>
    /// <param name="��ͼƬ"></param>
    private void ���Ŵ�ͼ(int ͼ��, Bitmap ��ͼƬ) {
        byte[] ���� = ѵ������;
        byte[] ��ǩ = ѵ����ǩ;
        if (!��ǰͼƬ��ѵ����) {
            ���� = ��������;
            ��ǩ = ���Ա�ǩ;
        }
        int j = 4 * 4 + ���[ͼ��] * MnistSize * MnistSize;
        BitmapData ͼ���� = ԭ����ͼ.LockBits(new Rectangle(0, 0, ԭ����ͼ.Width, ԭ����ͼ.Height), ImageLockMode.ReadWrite, ԭ����ͼ.PixelFormat);
        unsafe {
            byte* ͼָ�� = (byte*)(ͼ����.Scan0);
            for (int y = 0; y < ͼ����.Height; y++) {
                for (int x = 0; x < ͼ����.Width; x++, ͼָ�� += 3) {
                    ͼָ��[��] = ͼָ��[��] = ͼָ��[��] = ����[j];
                    j++;
                }
                ͼָ�� += ͼ����.Stride - ͼ����.Width * 3;
            }
        }
        ԭ����ͼ.UnlockBits(ͼ����);
        ͼ��.Nxλͼ(ԭ����ͼ, 11, false, ��ͼƬ);
        Graphics g = Graphics.FromImage(��ͼƬ);
        g.DrawString(��ǩ[���[ͼ��]].ToString(), TimesNR����(24), new SolidBrush(Color.LightGreen), new Point(0, 0));
        if (�����ǩ != null)
            g.DrawString(�����ǩ[���[ͼ��]].ToString(), TimesNR����(24), new SolidBrush(Color.Red), new Point(0, 24));
        g.Dispose();
    }

    /// <summary>
    /// �ѡ�0����ͼƬ����1����ͼƬ...���±궼�ҳ����ֱ�浽10���б���
    /// </summary>
    static void ��ʼ����ǩ��() {
        if (!(File.Exists(Mnist·�� + ѵ����ǩ��) && File.Exists(Mnist·�� + ���Ա�ǩ��))) {
            MessageBox.Show("û�ҵ�����Mnist�����ļ�(����Ҫ�޸�'Mnist�鿴��ǩҳ.cs'��������ļ���): " + ѵ����ǩ�� + "\n" + ���Ա�ǩ��);
            return;
        }
        if (ѵ�����ֱ�ǩ�� != null) return;
        ѵ�����ֱ�ǩ�� = new List<int>[10];
        �������ֱ�ǩ�� = new List<int>[10];
        ѵ����ǩ = new byte[ѵ��ͼƬ��];
        ���Ա�ǩ = new byte[����ͼƬ��];
        for (int i = 0; i < ѵ�����ֱ�ǩ��.Length; i++) {
            ѵ�����ֱ�ǩ��[i] = new List<int>(7000);
            �������ֱ�ǩ��[i] = new List<int>(1200);
        }
        using FileStream ѵ����ǩ�� = new FileStream(Mnist·�� + ѵ����ǩ��, FileMode.Open);
        using FileStream ���Ա�ǩ�� = new FileStream(Mnist·�� + ���Ա�ǩ��, FileMode.Open);
        ѵ����ǩ��.Seek(8, SeekOrigin.Begin);
        ѵ����ǩ��.Read(ѵ����ǩ, 0, ѵ��ͼƬ��);
        ���Ա�ǩ��.Seek(8, SeekOrigin.Begin);
        ���Ա�ǩ��.Read(���Ա�ǩ, 0, ����ͼƬ��);
        for (int i = 0; i < ѵ��ͼƬ��; i++)
            ѵ�����ֱ�ǩ��[(int)ѵ����ǩ[i]].Add(i);
        for (int i = 0; i < ����ͼƬ��; i++)
            �������ֱ�ǩ��[(int)���Ա�ǩ[i]].Add(i);
    }

    private void ����ÿ������ͼƬ����Ŀ() {
        if (ѵ�����ֱ�ǩ�� == null) return;
        List<int>[] ��ǩ�� = ѵ�����ֱ�ǩ��;
        int ͼƬ�� = ѵ��ͼƬ��;
        if (����.radioButton���Լ�.Checked) {
            ��ǩ�� = �������ֱ�ǩ��;
            ͼƬ�� = ����ͼƬ��;
        }
        for (int i = 0; i < 10; i++)
            ����.comboBox���ĸ���.Items[i] = "ֻ��" + i.ToString() + " " + ��ǩ��[i].Count.ToString();
        ����.comboBox���ĸ���.Items[10] = "ȫ��" + " " + ͼƬ��.ToString();

    }

    static public void ����������(int[] �������, int ������ֵ, int ���� = -1) {
        Random ��� = new Random();
        if (���� > 0)
            ��� = new Random(����);
        �������[0] = ���.Next(������ֵ);
        for (int i = 1; i < �������.Length; i++) {
            bool ��֮ǰ��� = true;
            while (��֮ǰ���) {
                �������[i] = ���.Next(������ֵ);
                for (int j = 0; j < i; j++)
                    if (�������[i] == �������[j]) {
                        ��֮ǰ��� = true;
                        break;
                    }
                    else
                        ��֮ǰ��� = false;
            }
        }
        Array.Sort(�������);
    }

    public void �ر�() {
        Dispose();
    }
    static public Font TimesNR����(int �ֺ�) {
        return new Font(new FontFamily("Times New Roman"), �ֺ�);
    }
}

