namespace 控件 {
    partial class 局部放大图 {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.图片框 = new System.Windows.Forms.PictureBox();
            this.放大倍数 = new System.Windows.Forms.TrackBar();
            this.红 = new System.Windows.Forms.Label();
            this.绿 = new System.Windows.Forms.Label();
            this.蓝 = new System.Windows.Forms.Label();
            this.色 = new System.Windows.Forms.Label();
            this.饱 = new System.Windows.Forms.Label();
            this.亮 = new System.Windows.Forms.Label();
            this.Y坐标 = new System.Windows.Forms.Label();
            this.X坐标 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.倍数 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.图片框)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.放大倍数)).BeginInit();
            this.SuspendLayout();
            // 
            // 图片框
            // 
            this.图片框.Location = new System.Drawing.Point(0, 0);
            this.图片框.Name = "图片框";
            this.图片框.Size = new System.Drawing.Size(128, 128);
            this.图片框.TabIndex = 0;
            this.图片框.TabStop = false;
            this.图片框.MouseEnter += new System.EventHandler(this.局部放大图_MouseEnter);
            // 
            // 放大倍数
            // 
            this.放大倍数.LargeChange = 2;
            this.放大倍数.Location = new System.Drawing.Point(134, 58);
            this.放大倍数.Maximum = 11;
            this.放大倍数.Minimum = 3;
            this.放大倍数.Name = "放大倍数";
            this.放大倍数.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.放大倍数.Size = new System.Drawing.Size(45, 74);
            this.放大倍数.TabIndex = 1;
            this.toolTip1.SetToolTip(this.放大倍数, "选择不插值局部放大的倍数");
            this.放大倍数.Value = 11;
            this.放大倍数.Scroll += new System.EventHandler(this.放大倍数_Scroll);
            // 
            // 红
            // 
            this.红.AutoSize = true;
            this.红.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.红.ForeColor = System.Drawing.Color.Red;
            this.红.Location = new System.Drawing.Point(171, 3);
            this.红.Name = "红";
            this.红.Size = new System.Drawing.Size(26, 21);
            this.红.TabIndex = 2;
            this.红.Tag = "";
            this.红.Text = "红";
            this.红.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.红, "鼠标所在像素点的R值");
            // 
            // 绿
            // 
            this.绿.AutoSize = true;
            this.绿.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.绿.ForeColor = System.Drawing.Color.Green;
            this.绿.Location = new System.Drawing.Point(171, 23);
            this.绿.Name = "绿";
            this.绿.Size = new System.Drawing.Size(26, 21);
            this.绿.TabIndex = 3;
            this.绿.Tag = "";
            this.绿.Text = "绿";
            this.绿.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.绿, "鼠标指向像素点的G值");
            // 
            // 蓝
            // 
            this.蓝.AutoSize = true;
            this.蓝.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.蓝.ForeColor = System.Drawing.Color.Blue;
            this.蓝.Location = new System.Drawing.Point(171, 43);
            this.蓝.Name = "蓝";
            this.蓝.Size = new System.Drawing.Size(26, 21);
            this.蓝.TabIndex = 4;
            this.蓝.Tag = "";
            this.蓝.Text = "蓝";
            this.蓝.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.蓝, "鼠标指向像素点的B值");
            // 
            // 色
            // 
            this.色.AutoSize = true;
            this.色.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.色.Location = new System.Drawing.Point(171, 63);
            this.色.Name = "色";
            this.色.Size = new System.Drawing.Size(42, 21);
            this.色.TabIndex = 5;
            this.色.Tag = "";
            this.色.Text = "色度";
            this.色.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.色, "鼠标指向像素点的色调值");
            // 
            // 饱
            // 
            this.饱.AutoSize = true;
            this.饱.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.饱.Location = new System.Drawing.Point(171, 83);
            this.饱.Name = "饱";
            this.饱.Size = new System.Drawing.Size(42, 21);
            this.饱.TabIndex = 6;
            this.饱.Tag = "";
            this.饱.Text = "饱和";
            this.饱.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.饱, "饱和度值");
            // 
            // 亮
            // 
            this.亮.AutoSize = true;
            this.亮.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.亮.Location = new System.Drawing.Point(171, 103);
            this.亮.Name = "亮";
            this.亮.Size = new System.Drawing.Size(42, 21);
            this.亮.TabIndex = 7;
            this.亮.Tag = "";
            this.亮.Text = "亮度";
            this.亮.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.亮, "亮度值");
            // 
            // Y坐标
            // 
            this.Y坐标.AutoSize = true;
            this.Y坐标.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Y坐标.Location = new System.Drawing.Point(132, 23);
            this.Y坐标.Name = "Y坐标";
            this.Y坐标.Size = new System.Drawing.Size(17, 20);
            this.Y坐标.TabIndex = 9;
            this.Y坐标.Tag = "";
            this.Y坐标.Text = "Y";
            this.Y坐标.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.Y坐标, "鼠标指向像素点的Y坐标");
            // 
            // X坐标
            // 
            this.X坐标.AutoSize = true;
            this.X坐标.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.X坐标.Location = new System.Drawing.Point(132, 3);
            this.X坐标.Name = "X坐标";
            this.X坐标.Size = new System.Drawing.Size(18, 20);
            this.X坐标.TabIndex = 8;
            this.X坐标.Tag = "";
            this.X坐标.Text = "X";
            this.X坐标.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.X坐标, "鼠标指向像素点的X坐标");
            // 
            // 倍数
            // 
            this.倍数.AutoSize = true;
            this.倍数.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.倍数.Location = new System.Drawing.Point(130, 43);
            this.倍数.Name = "倍数";
            this.倍数.Size = new System.Drawing.Size(37, 20);
            this.倍数.TabIndex = 10;
            this.倍数.Text = "倍数";
            this.倍数.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.倍数, "放大多少倍");
            // 
            // 局部放大图
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.倍数);
            this.Controls.Add(this.Y坐标);
            this.Controls.Add(this.X坐标);
            this.Controls.Add(this.亮);
            this.Controls.Add(this.饱);
            this.Controls.Add(this.色);
            this.Controls.Add(this.蓝);
            this.Controls.Add(this.绿);
            this.Controls.Add(this.红);
            this.Controls.Add(this.放大倍数);
            this.Controls.Add(this.图片框);
            this.Name = "局部放大图";
            this.Size = new System.Drawing.Size(214, 128);
            this.MouseEnter += new System.EventHandler(this.局部放大图_MouseEnter);
            ((System.ComponentModel.ISupportInitialize)(this.图片框)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.放大倍数)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBox 图片框;
        private TrackBar 放大倍数;
        private Label 红;
        private Label 绿;
        private Label 蓝;
        private Label 色;
        private Label 饱;
        private Label 亮;
        private Label Y坐标;
        private Label X坐标;
        private ToolTip toolTip1;
        private Label 倍数;
    }
}
