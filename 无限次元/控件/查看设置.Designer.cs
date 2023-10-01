namespace 控件 {
    partial class 查看设置 {
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
            this.数据选择 = new System.Windows.Forms.GroupBox();
            this.radioButton测试集 = new System.Windows.Forms.RadioButton();
            this.radioButton训练集 = new System.Windows.Forms.RadioButton();
            this.comboBox放大倍数 = new System.Windows.Forms.ComboBox();
            this.label放大 = new System.Windows.Forms.Label();
            this.groupBox读方式 = new System.Windows.Forms.GroupBox();
            this.textBox起始位置 = new System.Windows.Forms.TextBox();
            this.radioButton顺序读 = new System.Windows.Forms.RadioButton();
            this.radioButton随机读 = new System.Windows.Forms.RadioButton();
            this.label读哪个数 = new System.Windows.Forms.Label();
            this.comboBox读哪个数 = new System.Windows.Forms.ComboBox();
            this.读之按钮 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label训练序号 = new System.Windows.Forms.Label();
            this.label测试序号 = new System.Windows.Forms.Label();
            this.button关闭 = new System.Windows.Forms.Button();
            this.button保存右图 = new System.Windows.Forms.Button();
            this.button保存上图 = new System.Windows.Forms.Button();
            this.label灰度值 = new System.Windows.Forms.Label();
            this.上一页 = new System.Windows.Forms.Button();
            this.显示标签 = new System.Windows.Forms.CheckBox();
            this.显示边框 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.数据选择.SuspendLayout();
            this.groupBox读方式.SuspendLayout();
            this.SuspendLayout();
            // 
            // 数据选择
            // 
            this.数据选择.Controls.Add(this.radioButton测试集);
            this.数据选择.Controls.Add(this.radioButton训练集);
            this.数据选择.Location = new System.Drawing.Point(6, 48);
            this.数据选择.Margin = new System.Windows.Forms.Padding(4);
            this.数据选择.Name = "数据选择";
            this.数据选择.Padding = new System.Windows.Forms.Padding(4);
            this.数据选择.Size = new System.Drawing.Size(78, 95);
            this.数据选择.TabIndex = 0;
            this.数据选择.TabStop = false;
            this.数据选择.Text = "数据选择";
            // 
            // radioButton测试集
            // 
            this.radioButton测试集.AutoSize = true;
            this.radioButton测试集.Location = new System.Drawing.Point(8, 61);
            this.radioButton测试集.Margin = new System.Windows.Forms.Padding(4);
            this.radioButton测试集.Name = "radioButton测试集";
            this.radioButton测试集.Size = new System.Drawing.Size(62, 21);
            this.radioButton测试集.TabIndex = 1;
            this.radioButton测试集.Text = "测试集";
            this.radioButton测试集.UseVisualStyleBackColor = true;
            // 
            // radioButton训练集
            // 
            this.radioButton训练集.AutoSize = true;
            this.radioButton训练集.Checked = true;
            this.radioButton训练集.Location = new System.Drawing.Point(8, 30);
            this.radioButton训练集.Margin = new System.Windows.Forms.Padding(4);
            this.radioButton训练集.Name = "radioButton训练集";
            this.radioButton训练集.Size = new System.Drawing.Size(62, 21);
            this.radioButton训练集.TabIndex = 0;
            this.radioButton训练集.TabStop = true;
            this.radioButton训练集.Text = "训练集";
            this.radioButton训练集.UseVisualStyleBackColor = true;
            // 
            // comboBox放大倍数
            // 
            this.comboBox放大倍数.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox放大倍数.FormattingEnabled = true;
            this.comboBox放大倍数.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "8",
            "12",
            "16",
            "24",
            "28",
            "32",
            "36"});
            this.comboBox放大倍数.Location = new System.Drawing.Point(39, 156);
            this.comboBox放大倍数.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox放大倍数.Name = "comboBox放大倍数";
            this.comboBox放大倍数.Size = new System.Drawing.Size(47, 25);
            this.comboBox放大倍数.TabIndex = 1;
            // 
            // label放大
            // 
            this.label放大.AutoSize = true;
            this.label放大.Location = new System.Drawing.Point(3, 160);
            this.label放大.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label放大.Name = "label放大";
            this.label放大.Size = new System.Drawing.Size(35, 17);
            this.label放大.TabIndex = 2;
            this.label放大.Text = "放大:";
            // 
            // groupBox读方式
            // 
            this.groupBox读方式.Controls.Add(this.textBox起始位置);
            this.groupBox读方式.Controls.Add(this.radioButton顺序读);
            this.groupBox读方式.Controls.Add(this.radioButton随机读);
            this.groupBox读方式.Location = new System.Drawing.Point(146, 48);
            this.groupBox读方式.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox读方式.Name = "groupBox读方式";
            this.groupBox读方式.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox读方式.Size = new System.Drawing.Size(158, 95);
            this.groupBox读方式.TabIndex = 2;
            this.groupBox读方式.TabStop = false;
            this.groupBox读方式.Text = "读取方式";
            // 
            // textBox起始位置
            // 
            this.textBox起始位置.Location = new System.Drawing.Point(80, 57);
            this.textBox起始位置.Margin = new System.Windows.Forms.Padding(4);
            this.textBox起始位置.MaxLength = 5;
            this.textBox起始位置.Name = "textBox起始位置";
            this.textBox起始位置.Size = new System.Drawing.Size(74, 23);
            this.textBox起始位置.TabIndex = 2;
            this.textBox起始位置.Text = "0";
            this.textBox起始位置.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox起始位置.TextChanged += new System.EventHandler(this.textBox起始位置_TextChanged);
            this.textBox起始位置.Leave += new System.EventHandler(this.textBox起始位置_Leave);
            // 
            // radioButton顺序读
            // 
            this.radioButton顺序读.AutoSize = true;
            this.radioButton顺序读.Checked = true;
            this.radioButton顺序读.Location = new System.Drawing.Point(7, 30);
            this.radioButton顺序读.Margin = new System.Windows.Forms.Padding(4);
            this.radioButton顺序读.Name = "radioButton顺序读";
            this.radioButton顺序读.Size = new System.Drawing.Size(151, 21);
            this.radioButton顺序读.TabIndex = 1;
            this.radioButton顺序读.TabStop = true;
            this.radioButton顺序读.Text = "顺序读——的起始位置:";
            this.radioButton顺序读.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.radioButton顺序读.UseVisualStyleBackColor = true;
            // 
            // radioButton随机读
            // 
            this.radioButton随机读.AutoSize = true;
            this.radioButton随机读.Location = new System.Drawing.Point(7, 61);
            this.radioButton随机读.Margin = new System.Windows.Forms.Padding(4);
            this.radioButton随机读.Name = "radioButton随机读";
            this.radioButton随机读.Size = new System.Drawing.Size(62, 21);
            this.radioButton随机读.TabIndex = 0;
            this.radioButton随机读.Text = "随机读";
            this.radioButton随机读.UseVisualStyleBackColor = true;
            // 
            // label读哪个数
            // 
            this.label读哪个数.AutoSize = true;
            this.label读哪个数.Location = new System.Drawing.Point(87, 160);
            this.label读哪个数.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label读哪个数.Name = "label读哪个数";
            this.label读哪个数.Size = new System.Drawing.Size(35, 17);
            this.label读哪个数.TabIndex = 5;
            this.label读哪个数.Text = "选数:";
            // 
            // comboBox读哪个数
            // 
            this.comboBox读哪个数.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox读哪个数.FormattingEnabled = true;
            this.comboBox读哪个数.Items.AddRange(new object[] {
            "只读0",
            "只读1",
            "只读2",
            "只读3",
            "只读4",
            "只读5",
            "只读6",
            "只读7",
            "只读8",
            "只读9",
            "全部"});
            this.comboBox读哪个数.Location = new System.Drawing.Point(122, 156);
            this.comboBox读哪个数.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox读哪个数.Name = "comboBox读哪个数";
            this.comboBox读哪个数.Size = new System.Drawing.Size(101, 25);
            this.comboBox读哪个数.TabIndex = 4;
            // 
            // 读之按钮
            // 
            this.读之按钮.ForeColor = System.Drawing.Color.Red;
            this.读之按钮.Location = new System.Drawing.Point(260, 156);
            this.读之按钮.Margin = new System.Windows.Forms.Padding(4);
            this.读之按钮.Name = "读之按钮";
            this.读之按钮.Size = new System.Drawing.Size(40, 25);
            this.读之按钮.TabIndex = 6;
            this.读之按钮.Text = "读之";
            this.读之按钮.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(88, 48);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "当前序号:";
            // 
            // label训练序号
            // 
            this.label训练序号.AutoSize = true;
            this.label训练序号.Location = new System.Drawing.Point(92, 81);
            this.label训练序号.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label训练序号.Name = "label训练序号";
            this.label训练序号.Size = new System.Drawing.Size(15, 17);
            this.label训练序号.TabIndex = 8;
            this.label训练序号.Text = "0";
            this.label训练序号.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label测试序号
            // 
            this.label测试序号.AutoSize = true;
            this.label测试序号.Location = new System.Drawing.Point(92, 112);
            this.label测试序号.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label测试序号.Name = "label测试序号";
            this.label测试序号.Size = new System.Drawing.Size(15, 17);
            this.label测试序号.TabIndex = 9;
            this.label测试序号.Text = "0";
            this.label测试序号.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button关闭
            // 
            this.button关闭.Location = new System.Drawing.Point(6, 7);
            this.button关闭.Margin = new System.Windows.Forms.Padding(4);
            this.button关闭.Name = "button关闭";
            this.button关闭.Size = new System.Drawing.Size(70, 24);
            this.button关闭.TabIndex = 10;
            this.button关闭.Text = "关闭本页";
            this.button关闭.UseVisualStyleBackColor = true;
            this.button关闭.Click += new System.EventHandler(this.button关闭_Click);
            // 
            // button保存右图
            // 
            this.button保存右图.Location = new System.Drawing.Point(229, 7);
            this.button保存右图.Margin = new System.Windows.Forms.Padding(4);
            this.button保存右图.Name = "button保存右图";
            this.button保存右图.Size = new System.Drawing.Size(74, 24);
            this.button保存右图.TabIndex = 11;
            this.button保存右图.Text = "保存右图";
            this.button保存右图.UseVisualStyleBackColor = true;
            this.button保存右图.Click += new System.EventHandler(this.button保存右图_Click);
            // 
            // button保存上图
            // 
            this.button保存上图.Location = new System.Drawing.Point(147, 7);
            this.button保存上图.Margin = new System.Windows.Forms.Padding(4);
            this.button保存上图.Name = "button保存上图";
            this.button保存上图.Size = new System.Drawing.Size(74, 24);
            this.button保存上图.TabIndex = 12;
            this.button保存上图.Text = "保存上图";
            this.button保存上图.UseVisualStyleBackColor = true;
            this.button保存上图.Click += new System.EventHandler(this.button保存上图_Click);
            // 
            // label灰度值
            // 
            this.label灰度值.AutoSize = true;
            this.label灰度值.Location = new System.Drawing.Point(84, 11);
            this.label灰度值.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label灰度值.Name = "label灰度值";
            this.label灰度值.Size = new System.Drawing.Size(44, 17);
            this.label灰度值.TabIndex = 13;
            this.label灰度值.Text = "灰度值";
            // 
            // 上一页
            // 
            this.上一页.Location = new System.Drawing.Point(228, 156);
            this.上一页.Margin = new System.Windows.Forms.Padding(4);
            this.上一页.Name = "上一页";
            this.上一页.Size = new System.Drawing.Size(26, 25);
            this.上一页.TabIndex = 14;
            this.上一页.Text = "<";
            this.上一页.UseVisualStyleBackColor = true;
            // 
            // 显示标签
            // 
            this.显示标签.AutoSize = true;
            this.显示标签.Checked = true;
            this.显示标签.CheckState = System.Windows.Forms.CheckState.Checked;
            this.显示标签.Location = new System.Drawing.Point(123, 187);
            this.显示标签.Name = "显示标签";
            this.显示标签.Size = new System.Drawing.Size(51, 21);
            this.显示标签.TabIndex = 15;
            this.显示标签.Text = "标签";
            this.显示标签.UseVisualStyleBackColor = true;
            // 
            // 显示边框
            // 
            this.显示边框.AutoSize = true;
            this.显示边框.Checked = true;
            this.显示边框.CheckState = System.Windows.Forms.CheckState.Checked;
            this.显示边框.Location = new System.Drawing.Point(180, 187);
            this.显示边框.Name = "显示边框";
            this.显示边框.Size = new System.Drawing.Size(51, 21);
            this.显示边框.TabIndex = 16;
            this.显示边框.Text = "边框";
            this.显示边框.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(87, 187);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 17);
            this.label2.TabIndex = 17;
            this.label2.Text = "显示:";
            // 
            // 查看设置
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.显示边框);
            this.Controls.Add(this.显示标签);
            this.Controls.Add(this.上一页);
            this.Controls.Add(this.label灰度值);
            this.Controls.Add(this.button保存上图);
            this.Controls.Add(this.button保存右图);
            this.Controls.Add(this.button关闭);
            this.Controls.Add(this.label测试序号);
            this.Controls.Add(this.label训练序号);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.读之按钮);
            this.Controls.Add(this.label读哪个数);
            this.Controls.Add(this.comboBox读哪个数);
            this.Controls.Add(this.groupBox读方式);
            this.Controls.Add(this.label放大);
            this.Controls.Add(this.comboBox放大倍数);
            this.Controls.Add(this.数据选择);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "查看设置";
            this.Size = new System.Drawing.Size(308, 212);
            this.数据选择.ResumeLayout(false);
            this.数据选择.PerformLayout();
            this.groupBox读方式.ResumeLayout(false);
            this.groupBox读方式.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.GroupBox 数据选择;
        public System.Windows.Forms.RadioButton radioButton测试集;
        public System.Windows.Forms.RadioButton radioButton训练集;
        public System.Windows.Forms.ComboBox comboBox放大倍数;
        private System.Windows.Forms.Label label放大;
        public System.Windows.Forms.GroupBox groupBox读方式;
        public System.Windows.Forms.RadioButton radioButton顺序读;
        public System.Windows.Forms.RadioButton radioButton随机读;
        public System.Windows.Forms.TextBox textBox起始位置;
        private System.Windows.Forms.Label label读哪个数;
        public System.Windows.Forms.ComboBox comboBox读哪个数;
        public System.Windows.Forms.Button 读之按钮;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label训练序号;
        public System.Windows.Forms.Label label测试序号;
        public System.Windows.Forms.Button button关闭;
        public System.Windows.Forms.Button button保存右图;
        public System.Windows.Forms.Button button保存上图;
        public System.Windows.Forms.Label label灰度值;
        public System.Windows.Forms.Button 上一页;
        public CheckBox 显示标签;
        public CheckBox 显示边框;
        private Label label2;
    }
}
