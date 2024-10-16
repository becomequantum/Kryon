//無限次元:https://space.bilibili.com/2139404925
//本代码来自:https://github.com/becomequantum/Kryon
namespace 无限次元 {
    partial class 无限主窗体 {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(无限主窗体));
            splitContainer1 = new SplitContainer();
            menuStrip1 = new MenuStrip();
            打开图片ToolStripMenuItem = new ToolStripMenuItem();
            保存ToolStripMenuItem = new ToolStripMenuItem();
            另存为ToolStripMenuItem = new ToolStripMenuItem();
            覆盖原图ToolStripMenuItem = new ToolStripMenuItem();
            另存二值PNGToolStripMenuItem = new ToolStripMenuItem();
            关闭所有图片ToolStripMenuItem = new ToolStripMenuItem();
            回到原图ToolStripMenuItem = new ToolStripMenuItem();
            复制粘贴ToolStripMenuItem = new ToolStripMenuItem();
            粘贴图片ToolStripMenuItem = new ToolStripMenuItem();
            复制当前页到新页ToolStripMenuItem = new ToolStripMenuItem();
            复制当前页到粘贴板ToolStripMenuItem = new ToolStripMenuItem();
            局部放大图1 = new 局部放大图();
            缩略图片框 = new PictureBox();
            信息显示 = new Label();
            功能标签页1 = new 功能标签页();
            toolTip1 = new ToolTip(components);
            图片右键菜单 = new ContextMenuStrip(components);
            放大此处ToolStripMenuItem = new ToolStripMenuItem();
            复制到新页ToolStripMenuItem = new ToolStripMenuItem();
            复制到粘贴板ToolStripMenuItem = new ToolStripMenuItem();
            复制当前页到新页ToolStripMenuItem1 = new ToolStripMenuItem();
            关闭ToolStripMenuItem = new ToolStripMenuItem();
            tabPage1 = new TabPage();
            链接1 = new LinkLabel();
            说明 = new Label();
            主标签页 = new TabControl();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)缩略图片框).BeginInit();
            图片右键菜单.SuspendLayout();
            tabPage1.SuspendLayout();
            主标签页.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Top;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(menuStrip1);
            splitContainer1.Panel1.Controls.Add(局部放大图1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(缩略图片框);
            splitContainer1.Panel2.Controls.Add(信息显示);
            splitContainer1.Panel2.Controls.Add(功能标签页1);
            splitContainer1.Size = new Size(1675, 128);
            splitContainer1.SplitterDistance = 298;
            splitContainer1.SplitterWidth = 1;
            splitContainer1.TabIndex = 0;
            // 
            // menuStrip1
            // 
            menuStrip1.Dock = DockStyle.None;
            menuStrip1.Font = new Font("微软雅黑", 10.5F);
            menuStrip1.GripMargin = new Padding(0);
            menuStrip1.Items.AddRange(new ToolStripItem[] { 打开图片ToolStripMenuItem, 保存ToolStripMenuItem, 回到原图ToolStripMenuItem, 复制粘贴ToolStripMenuItem });
            menuStrip1.LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
            menuStrip1.Location = new Point(217, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(0);
            menuStrip1.Size = new Size(81, 98);
            menuStrip1.TabIndex = 2;
            menuStrip1.Text = "menuStrip1";
            // 
            // 打开图片ToolStripMenuItem
            // 
            打开图片ToolStripMenuItem.ForeColor = Color.Purple;
            打开图片ToolStripMenuItem.Name = "打开图片ToolStripMenuItem";
            打开图片ToolStripMenuItem.Size = new Size(80, 24);
            打开图片ToolStripMenuItem.Text = "打开图片";
            打开图片ToolStripMenuItem.ToolTipText = "可多选";
            打开图片ToolStripMenuItem.Click += 打开图片ToolStripMenuItem_Click;
            // 
            // 保存ToolStripMenuItem
            // 
            保存ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 另存为ToolStripMenuItem, 覆盖原图ToolStripMenuItem, 另存二值PNGToolStripMenuItem, 关闭所有图片ToolStripMenuItem });
            保存ToolStripMenuItem.Name = "保存ToolStripMenuItem";
            保存ToolStripMenuItem.Size = new Size(80, 24);
            保存ToolStripMenuItem.Text = "保存 关闭";
            // 
            // 另存为ToolStripMenuItem
            // 
            另存为ToolStripMenuItem.Name = "另存为ToolStripMenuItem";
            另存为ToolStripMenuItem.Size = new Size(164, 24);
            另存为ToolStripMenuItem.Text = "另存为";
            另存为ToolStripMenuItem.Click += 另存为ToolStripMenuItem_Click;
            // 
            // 覆盖原图ToolStripMenuItem
            // 
            覆盖原图ToolStripMenuItem.Name = "覆盖原图ToolStripMenuItem";
            覆盖原图ToolStripMenuItem.Size = new Size(164, 24);
            覆盖原图ToolStripMenuItem.Text = "覆盖原图";
            覆盖原图ToolStripMenuItem.Click += 覆盖原图ToolStripMenuItem_Click;
            // 
            // 另存二值PNGToolStripMenuItem
            // 
            另存二值PNGToolStripMenuItem.Name = "另存二值PNGToolStripMenuItem";
            另存二值PNGToolStripMenuItem.Size = new Size(164, 24);
            另存二值PNGToolStripMenuItem.Text = "另存二值PNG";
            另存二值PNGToolStripMenuItem.Click += 另存为ToolStripMenuItem_Click;
            // 
            // 关闭所有图片ToolStripMenuItem
            // 
            关闭所有图片ToolStripMenuItem.Name = "关闭所有图片ToolStripMenuItem";
            关闭所有图片ToolStripMenuItem.Size = new Size(164, 24);
            关闭所有图片ToolStripMenuItem.Text = "关闭所有图片";
            关闭所有图片ToolStripMenuItem.Click += 关闭所有图片页ToolStripMenuItem_Click;
            // 
            // 回到原图ToolStripMenuItem
            // 
            回到原图ToolStripMenuItem.Name = "回到原图ToolStripMenuItem";
            回到原图ToolStripMenuItem.Size = new Size(80, 24);
            回到原图ToolStripMenuItem.Text = "回到原图";
            回到原图ToolStripMenuItem.Click += 回到原图ToolStripMenuItem_Click;
            // 
            // 复制粘贴ToolStripMenuItem
            // 
            复制粘贴ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 粘贴图片ToolStripMenuItem, 复制当前页到新页ToolStripMenuItem, 复制当前页到粘贴板ToolStripMenuItem });
            复制粘贴ToolStripMenuItem.Name = "复制粘贴ToolStripMenuItem";
            复制粘贴ToolStripMenuItem.Size = new Size(80, 24);
            复制粘贴ToolStripMenuItem.Text = "复制 粘贴";
            // 
            // 粘贴图片ToolStripMenuItem
            // 
            粘贴图片ToolStripMenuItem.Name = "粘贴图片ToolStripMenuItem";
            粘贴图片ToolStripMenuItem.Size = new Size(204, 24);
            粘贴图片ToolStripMenuItem.Text = "粘贴图片";
            粘贴图片ToolStripMenuItem.ToolTipText = "把剪贴板里的图片粘贴到这里";
            粘贴图片ToolStripMenuItem.Click += 粘贴图片ToolStripMenuItem_Click;
            // 
            // 复制当前页到新页ToolStripMenuItem
            // 
            复制当前页到新页ToolStripMenuItem.Name = "复制当前页到新页ToolStripMenuItem";
            复制当前页到新页ToolStripMenuItem.Size = new Size(204, 24);
            复制当前页到新页ToolStripMenuItem.Text = "复制当前页到新页";
            复制当前页到新页ToolStripMenuItem.Click += 复制当前页到新页ToolStripMenuItem_Click;
            // 
            // 复制当前页到粘贴板ToolStripMenuItem
            // 
            复制当前页到粘贴板ToolStripMenuItem.Name = "复制当前页到粘贴板ToolStripMenuItem";
            复制当前页到粘贴板ToolStripMenuItem.Size = new Size(204, 24);
            复制当前页到粘贴板ToolStripMenuItem.Text = "复制当前页到粘贴板";
            复制当前页到粘贴板ToolStripMenuItem.Click += 复制当前页到粘贴板ToolStripMenuItem_Click;
            // 
            // 局部放大图1
            // 
            局部放大图1.Location = new Point(0, 0);
            局部放大图1.Name = "局部放大图1";
            局部放大图1.Size = new Size(214, 128);
            局部放大图1.TabIndex = 0;
            // 
            // 缩略图片框
            // 
            缩略图片框.Dock = DockStyle.Right;
            缩略图片框.Location = new Point(1276, 0);
            缩略图片框.Name = "缩略图片框";
            缩略图片框.Size = new Size(100, 128);
            缩略图片框.SizeMode = PictureBoxSizeMode.Zoom;
            缩略图片框.TabIndex = 2;
            缩略图片框.TabStop = false;
            缩略图片框.Visible = false;
            // 
            // 信息显示
            // 
            信息显示.AutoSize = true;
            信息显示.Location = new Point(3, 108);
            信息显示.Name = "信息显示";
            信息显示.Size = new Size(32, 17);
            信息显示.TabIndex = 1;
            信息显示.Text = "耗时";
            // 
            // 功能标签页1
            // 
            功能标签页1.BackColor = SystemColors.Control;
            功能标签页1.Dock = DockStyle.Fill;
            功能标签页1.Location = new Point(0, 0);
            功能标签页1.Name = "功能标签页1";
            功能标签页1.Size = new Size(1376, 128);
            功能标签页1.TabIndex = 0;
            功能标签页1.图像处理按钮点击 += 功能标签页1_图像处理按钮点击;
            功能标签页1.神经网络按钮点击 += 功能标签页1_神经网络按钮点击;
            功能标签页1.画啥按钮点击 += 功能标签页1_画啥按钮点击;
            功能标签页1.显示RGB点击 += 功能标签页1_显示RGB点击;
            // 
            // 图片右键菜单
            // 
            图片右键菜单.Items.AddRange(new ToolStripItem[] { 放大此处ToolStripMenuItem, 复制到新页ToolStripMenuItem, 复制到粘贴板ToolStripMenuItem, 复制当前页到新页ToolStripMenuItem1, 关闭ToolStripMenuItem });
            图片右键菜单.Name = "图片右键菜单";
            图片右键菜单.Size = new Size(185, 114);
            // 
            // 放大此处ToolStripMenuItem
            // 
            放大此处ToolStripMenuItem.Name = "放大此处ToolStripMenuItem";
            放大此处ToolStripMenuItem.Size = new Size(184, 22);
            放大此处ToolStripMenuItem.Text = "放大此处";
            放大此处ToolStripMenuItem.Click += 放大此处ToolStripMenuItem_Click;
            // 
            // 复制到新页ToolStripMenuItem
            // 
            复制到新页ToolStripMenuItem.Name = "复制到新页ToolStripMenuItem";
            复制到新页ToolStripMenuItem.Size = new Size(184, 22);
            复制到新页ToolStripMenuItem.Text = "复制框选区域到新页";
            复制到新页ToolStripMenuItem.Click += 复制选中区域到新页ToolStripMenuItem_Click;
            // 
            // 复制到粘贴板ToolStripMenuItem
            // 
            复制到粘贴板ToolStripMenuItem.Name = "复制到粘贴板ToolStripMenuItem";
            复制到粘贴板ToolStripMenuItem.Size = new Size(184, 22);
            复制到粘贴板ToolStripMenuItem.Text = "复制框选到粘贴板";
            复制到粘贴板ToolStripMenuItem.Click += 复制选中到粘贴板ToolStripMenuItem_Click;
            // 
            // 复制当前页到新页ToolStripMenuItem1
            // 
            复制当前页到新页ToolStripMenuItem1.Name = "复制当前页到新页ToolStripMenuItem1";
            复制当前页到新页ToolStripMenuItem1.Size = new Size(184, 22);
            复制当前页到新页ToolStripMenuItem1.Text = "复制当前页到新页";
            复制当前页到新页ToolStripMenuItem1.Click += 复制当前页到新页ToolStripMenuItem_Click;
            // 
            // 关闭ToolStripMenuItem
            // 
            关闭ToolStripMenuItem.Name = "关闭ToolStripMenuItem";
            关闭ToolStripMenuItem.Size = new Size(184, 22);
            关闭ToolStripMenuItem.Text = "关闭本页";
            关闭ToolStripMenuItem.Click += 主标签页_DoubleClick;
            // 
            // tabPage1
            // 
            tabPage1.BackColor = SystemColors.Control;
            tabPage1.Controls.Add(链接1);
            tabPage1.Controls.Add(说明);
            tabPage1.Font = new Font("Microsoft YaHei UI", 12F);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1667, 559);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "说明";
            // 
            // 链接1
            // 
            链接1.AutoSize = true;
            链接1.Font = new Font("微软雅黑", 14.25F);
            链接1.Location = new Point(3, 3);
            链接1.Name = "链接1";
            链接1.Size = new Size(157, 25);
            链接1.TabIndex = 1;
            链接1.TabStop = true;
            链接1.Text = "無限次元B站首页";
            链接1.LinkClicked += 链接1_LinkClicked;
            // 
            // 说明
            // 
            说明.AutoSize = true;
            说明.Font = new Font("微软雅黑", 12F);
            说明.Location = new Point(6, 33);
            说明.Name = "说明";
            说明.Size = new Size(1403, 126);
            说明.TabIndex = 0;
            说明.Text = resources.GetString("说明.Text");
            // 
            // 主标签页
            // 
            主标签页.Controls.Add(tabPage1);
            主标签页.Dock = DockStyle.Fill;
            主标签页.Location = new Point(0, 128);
            主标签页.MinimumSize = new Size(300, 200);
            主标签页.Name = "主标签页";
            主标签页.SelectedIndex = 0;
            主标签页.Size = new Size(1675, 589);
            主标签页.TabIndex = 1;
            主标签页.SelectedIndexChanged += 主标签页_SelectedIndexChanged;
            主标签页.DoubleClick += 主标签页_DoubleClick;
            // 
            // 无限主窗体
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1675, 717);
            Controls.Add(主标签页);
            Controls.Add(splitContainer1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MainMenuStrip = menuStrip1;
            Name = "无限主窗体";
            Text = "無限次元开源V1.0";
            WindowState = FormWindowState.Maximized;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)缩略图片框).EndInit();
            图片右键菜单.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            主标签页.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        public 局部放大图 局部放大图1;
        private ToolTip toolTip1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem 打开图片ToolStripMenuItem;
        private 功能标签页 功能标签页1;
        private Label 信息显示;
        private ToolStripMenuItem 保存ToolStripMenuItem;
        private ToolStripMenuItem 另存为ToolStripMenuItem;
        private ToolStripMenuItem 覆盖原图ToolStripMenuItem;
        private ToolStripMenuItem 回到原图ToolStripMenuItem;
        private ToolStripMenuItem 关闭所有图片ToolStripMenuItem;
        private ToolStripMenuItem 复制粘贴ToolStripMenuItem;
        private ToolStripMenuItem 粘贴图片ToolStripMenuItem;
        private ToolStripMenuItem 复制当前页到新页ToolStripMenuItem;
        private ContextMenuStrip 图片右键菜单;
        private ToolStripMenuItem 复制到新页ToolStripMenuItem;
        private ToolStripMenuItem 复制到粘贴板ToolStripMenuItem;
        private ToolStripMenuItem 关闭ToolStripMenuItem;
        private ToolStripMenuItem 放大此处ToolStripMenuItem;
        private ToolStripMenuItem 复制当前页到粘贴板ToolStripMenuItem;
        private PictureBox 缩略图片框;
        private ToolStripMenuItem 复制当前页到新页ToolStripMenuItem1;
        private TabPage tabPage1;
        private LinkLabel 链接1;
        private Label 说明;
        private TabControl 主标签页;
        private ToolStripMenuItem 另存二值PNGToolStripMenuItem;
    }
}