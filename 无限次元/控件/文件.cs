using System.Drawing.Imaging;

namespace 控件;
public class 文件 {
    static public string[] 打开文件(string 说明, string 后缀名过滤, bool 多选 = true) {
        OpenFileDialog 打开文件对话框 = new() {
            Filter = 后缀名过滤,
            Multiselect = 多选,  //默认可以多选
            Title = 说明,
            RestoreDirectory = true
        };
        if (打开文件对话框.ShowDialog() == DialogResult.OK)
            return 打开文件对话框.FileNames;
        else return null;
    }

    static public string 获取保存文件名(string 说明, string 后缀名过滤) {
        SaveFileDialog 保存文件对话框 = new SaveFileDialog();
        保存文件对话框.Filter = 后缀名过滤;
        保存文件对话框.Title = 说明;
        保存文件对话框.RestoreDirectory = true;
        保存文件对话框.OverwritePrompt = true;
        if (保存文件对话框.ShowDialog() == DialogResult.OK)
            return 保存文件对话框.FileName;
        else return null;
    }

    static public Bitmap 加载图片文件(string 图片文件名, out ImageFormat 图片格式) { //这样加载图片后，原来的图片文件不会被占用锁定
        try {
            using FileStream 文件流 = new(图片文件名, FileMode.Open, FileAccess.Read);
            Image 图片 = Image.FromStream(文件流);
            图片格式 = 图片.RawFormat;
            Bitmap 位图 = new(图片); //这样再拷贝一下是因为如果不这样的话,用Graphics截取部分图片时图像会变化.
            if (位图.PixelFormat == PixelFormat.Format32bppRgb) {//这个32位格式有8位没用,所以就转成24.
                Bitmap 位图24 = new(位图.Width, 位图.Height, PixelFormat.Format24bppRgb);
                using Graphics g = Graphics.FromImage(位图24);
                g.DrawImage(位图, 0, 0);
                位图.Dispose();
                return 位图24;
            }
            return 位图;
        }
        catch (Exception 意外) {
            MessageBox.Show(意外.Message);
            图片格式 = null;
            return null;
        }
    }

    static public void 保存图片(Bitmap 位图, string 文件名, ImageFormat 图片格式) {
        try {
            位图.Save(文件名, 图片格式);
        }
        catch {
            using var 位图拷贝 = new Bitmap(位图);
            位图拷贝.Save(文件名, 图片格式);
        }
    }

    static public ImageFormat 根据文件格式保存图片(Bitmap 位图, string 文件名) {
        ImageFormat 图片格式;
        switch (Path.GetExtension(文件名).ToLower()) {
            case ".bmp":
                图片格式 = ImageFormat.Bmp;
                break;
            case ".jpg":
                图片格式 = ImageFormat.Jpeg;
                break;
            case ".png":
                图片格式 = ImageFormat.Png;
                break;
            case ".ico":
                图片格式 = ImageFormat.Icon;
                break;
            default:
                MessageBox.Show("只能存bmp,jpg,png,ico四种文件类型!");
                return null;
        }
        保存图片(位图,文件名,图片格式);
        return 图片格式;
    }

    public static string 打开文件夹(string 描述) {
        FolderBrowserDialog 对话框 = new FolderBrowserDialog();
        对话框.Description = 描述;
        if (对话框.ShowDialog() == DialogResult.OK && 对话框.SelectedPath != null)
            return 对话框.SelectedPath;
        return null;
    }
}
