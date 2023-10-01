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
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(无限主窗体));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.打开图片ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.另存为ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.覆盖原图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关闭所有图片ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.回到原图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.复制粘贴ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.粘贴图片ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.复制当前页到新页ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.复制当前页到粘贴板ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.局部放大图1 = new 控件.局部放大图();
            this.缩略图片框 = new System.Windows.Forms.PictureBox();
            this.信息显示 = new System.Windows.Forms.Label();
            this.功能标签页1 = new 控件.功能标签页();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.图片右键菜单 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.放大此处ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.复制到新页ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.复制到粘贴板ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.复制当前页到新页ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.关闭ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.链接1 = new System.Windows.Forms.LinkLabel();
            this.说明 = new System.Windows.Forms.Label();
            this.主标签页 = new System.Windows.Forms.TabControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.缩略图片框)).BeginInit();
            this.图片右键菜单.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.主标签页.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.menuStrip1);
            this.splitContainer1.Panel1.Controls.Add(this.局部放大图1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.缩略图片框);
            this.splitContainer1.Panel2.Controls.Add(this.信息显示);
            this.splitContainer1.Panel2.Controls.Add(this.功能标签页1);
            this.splitContainer1.Size = new System.Drawing.Size(1675, 128);
            this.splitContainer1.SplitterDistance = 298;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开图片ToolStripMenuItem,
            this.保存ToolStripMenuItem,
            this.回到原图ToolStripMenuItem,
            this.复制粘贴ToolStripMenuItem});
            this.menuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.menuStrip1.Location = new System.Drawing.Point(217, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.menuStrip1.Size = new System.Drawing.Size(81, 98);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 打开图片ToolStripMenuItem
            // 
            this.打开图片ToolStripMenuItem.ForeColor = System.Drawing.Color.Purple;
            this.打开图片ToolStripMenuItem.Name = "打开图片ToolStripMenuItem";
            this.打开图片ToolStripMenuItem.Size = new System.Drawing.Size(80, 24);
            this.打开图片ToolStripMenuItem.Text = "打开图片";
            this.打开图片ToolStripMenuItem.ToolTipText = "可多选";
            this.打开图片ToolStripMenuItem.Click += new System.EventHandler(this.打开图片ToolStripMenuItem_Click);
            // 
            // 保存ToolStripMenuItem
            // 
            this.保存ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.另存为ToolStripMenuItem,
            this.覆盖原图ToolStripMenuItem,
            this.关闭所有图片ToolStripMenuItem});
            this.保存ToolStripMenuItem.Name = "保存ToolStripMenuItem";
            this.保存ToolStripMenuItem.Size = new System.Drawing.Size(80, 24);
            this.保存ToolStripMenuItem.Text = "保存 关闭";
            // 
            // 另存为ToolStripMenuItem
            // 
            this.另存为ToolStripMenuItem.Name = "另存为ToolStripMenuItem";
            this.另存为ToolStripMenuItem.Size = new System.Drawing.Size(162, 24);
            this.另存为ToolStripMenuItem.Text = "另存为";
            this.另存为ToolStripMenuItem.Click += new System.EventHandler(this.另存为ToolStripMenuItem_Click);
            // 
            // 覆盖原图ToolStripMenuItem
            // 
            this.覆盖原图ToolStripMenuItem.Name = "覆盖原图ToolStripMenuItem";
            this.覆盖原图ToolStripMenuItem.Size = new System.Drawing.Size(162, 24);
            this.覆盖原图ToolStripMenuItem.Text = "覆盖原图";
            this.覆盖原图ToolStripMenuItem.Click += new System.EventHandler(this.覆盖原图ToolStripMenuItem_Click);
            // 
            // 关闭所有图片ToolStripMenuItem
            // 
            this.关闭所有图片ToolStripMenuItem.Name = "关闭所有图片ToolStripMenuItem";
            this.关闭所有图片ToolStripMenuItem.Size = new System.Drawing.Size(162, 24);
            this.关闭所有图片ToolStripMenuItem.Text = "关闭所有图片";
            this.关闭所有图片ToolStripMenuItem.Click += new System.EventHandler(this.关闭所有图片页ToolStripMenuItem_Click);
            // 
            // 回到原图ToolStripMenuItem
            // 
            this.回到原图ToolStripMenuItem.Name = "回到原图ToolStripMenuItem";
            this.回到原图ToolStripMenuItem.Size = new System.Drawing.Size(80, 24);
            this.回到原图ToolStripMenuItem.Text = "回到原图";
            this.回到原图ToolStripMenuItem.Click += new System.EventHandler(this.回到原图ToolStripMenuItem_Click);
            // 
            // 复制粘贴ToolStripMenuItem
            // 
            this.复制粘贴ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.粘贴图片ToolStripMenuItem,
            this.复制当前页到新页ToolStripMenuItem,
            this.复制当前页到粘贴板ToolStripMenuItem});
            this.复制粘贴ToolStripMenuItem.Name = "复制粘贴ToolStripMenuItem";
            this.复制粘贴ToolStripMenuItem.Size = new System.Drawing.Size(80, 24);
            this.复制粘贴ToolStripMenuItem.Text = "复制 粘贴";
            // 
            // 粘贴图片ToolStripMenuItem
            // 
            this.粘贴图片ToolStripMenuItem.Name = "粘贴图片ToolStripMenuItem";
            this.粘贴图片ToolStripMenuItem.Size = new System.Drawing.Size(204, 24);
            this.粘贴图片ToolStripMenuItem.Text = "粘贴图片";
            this.粘贴图片ToolStripMenuItem.ToolTipText = "把剪贴板里的图片粘贴到这里";
            this.粘贴图片ToolStripMenuItem.Click += new System.EventHandler(this.粘贴图片ToolStripMenuItem_Click);
            // 
            // 复制当前页到新页ToolStripMenuItem
            // 
            this.复制当前页到新页ToolStripMenuItem.Name = "复制当前页到新页ToolStripMenuItem";
            this.复制当前页到新页ToolStripMenuItem.Size = new System.Drawing.Size(204, 24);
            this.复制当前页到新页ToolStripMenuItem.Text = "复制当前页到新页";
            this.复制当前页到新页ToolStripMenuItem.Click += new System.EventHandler(this.复制当前页到新页ToolStripMenuItem_Click);
            // 
            // 复制当前页到粘贴板ToolStripMenuItem
            // 
            this.复制当前页到粘贴板ToolStripMenuItem.Name = "复制当前页到粘贴板ToolStripMenuItem";
            this.复制当前页到粘贴板ToolStripMenuItem.Size = new System.Drawing.Size(204, 24);
            this.复制当前页到粘贴板ToolStripMenuItem.Text = "复制当前页到粘贴板";
            this.复制当前页到粘贴板ToolStripMenuItem.Click += new System.EventHandler(this.复制当前页到粘贴板ToolStripMenuItem_Click);
            // 
            // 局部放大图1
            // 
            this.局部放大图1.Location = new System.Drawing.Point(0, 0);
            this.局部放大图1.Name = "局部放大图1";
            this.局部放大图1.Size = new System.Drawing.Size(214, 128);
            this.局部放大图1.TabIndex = 0;
            // 
            // 缩略图片框
            // 
            this.缩略图片框.Dock = System.Windows.Forms.DockStyle.Right;
            this.缩略图片框.Location = new System.Drawing.Point(1276, 0);
            this.缩略图片框.Name = "缩略图片框";
            this.缩略图片框.Size = new System.Drawing.Size(100, 128);
            this.缩略图片框.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.缩略图片框.TabIndex = 2;
            this.缩略图片框.TabStop = false;
            this.缩略图片框.Visible = false;
            // 
            // 信息显示
            // 
            this.信息显示.AutoSize = true;
            this.信息显示.Location = new System.Drawing.Point(3, 108);
            this.信息显示.Name = "信息显示";
            this.信息显示.Size = new System.Drawing.Size(32, 17);
            this.信息显示.TabIndex = 1;
            this.信息显示.Text = "耗时";
            // 
            // 功能标签页1
            // 
            this.功能标签页1.BackColor = System.Drawing.SystemColors.Control;
            this.功能标签页1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.功能标签页1.Location = new System.Drawing.Point(0, 0);
            this.功能标签页1.Name = "功能标签页1";
            this.功能标签页1.Size = new System.Drawing.Size(1376, 128);
            this.功能标签页1.TabIndex = 0;
            this.功能标签页1.图像处理按钮点击 += new System.EventHandler(this.功能标签页1_图像处理按钮点击);
            this.功能标签页1.神经网络按钮点击 += new System.EventHandler(this.功能标签页1_神经网络按钮点击);
            this.功能标签页1.画啥按钮点击 += new System.EventHandler(this.功能标签页1_画啥按钮点击);
            this.功能标签页1.显示RGB点击 += new System.EventHandler(this.功能标签页1_显示RGB点击);
            // 
            // 图片右键菜单
            // 
            this.图片右键菜单.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.放大此处ToolStripMenuItem,
            this.复制到新页ToolStripMenuItem,
            this.复制到粘贴板ToolStripMenuItem,
            this.复制当前页到新页ToolStripMenuItem1,
            this.关闭ToolStripMenuItem});
            this.图片右键菜单.Name = "图片右键菜单";
            this.图片右键菜单.Size = new System.Drawing.Size(185, 114);
            // 
            // 放大此处ToolStripMenuItem
            // 
            this.放大此处ToolStripMenuItem.Name = "放大此处ToolStripMenuItem";
            this.放大此处ToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.放大此处ToolStripMenuItem.Text = "放大此处";
            this.放大此处ToolStripMenuItem.Click += new System.EventHandler(this.放大此处ToolStripMenuItem_Click);
            // 
            // 复制到新页ToolStripMenuItem
            // 
            this.复制到新页ToolStripMenuItem.Name = "复制到新页ToolStripMenuItem";
            this.复制到新页ToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.复制到新页ToolStripMenuItem.Text = "复制框选区域到新页";
            this.复制到新页ToolStripMenuItem.Click += new System.EventHandler(this.复制选中区域到新页ToolStripMenuItem_Click);
            // 
            // 复制到粘贴板ToolStripMenuItem
            // 
            this.复制到粘贴板ToolStripMenuItem.Name = "复制到粘贴板ToolStripMenuItem";
            this.复制到粘贴板ToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.复制到粘贴板ToolStripMenuItem.Text = "复制框选到粘贴板";
            this.复制到粘贴板ToolStripMenuItem.Click += new System.EventHandler(this.复制选中到粘贴板ToolStripMenuItem_Click);
            // 
            // 复制当前页到新页ToolStripMenuItem1
            // 
            this.复制当前页到新页ToolStripMenuItem1.Name = "复制当前页到新页ToolStripMenuItem1";
            this.复制当前页到新页ToolStripMenuItem1.Size = new System.Drawing.Size(184, 22);
            this.复制当前页到新页ToolStripMenuItem1.Text = "复制当前页到新页";
            this.复制当前页到新页ToolStripMenuItem1.Click += new System.EventHandler(this.复制当前页到新页ToolStripMenuItem_Click);
            // 
            // 关闭ToolStripMenuItem
            // 
            this.关闭ToolStripMenuItem.Name = "关闭ToolStripMenuItem";
            this.关闭ToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.关闭ToolStripMenuItem.Text = "关闭本页";
            this.关闭ToolStripMenuItem.Click += new System.EventHandler(this.主标签页_DoubleClick);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.链接1);
            this.tabPage1.Controls.Add(this.说明);
            this.tabPage1.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1667, 559);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "说明";
            // 
            // 链接1
            // 
            this.链接1.AutoSize = true;
            this.链接1.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.链接1.Location = new System.Drawing.Point(3, 3);
            this.链接1.Name = "链接1";
            this.链接1.Size = new System.Drawing.Size(157, 25);
            this.链接1.TabIndex = 1;
            this.链接1.TabStop = true;
            this.链接1.Text = "無限次元B站首页";
            this.链接1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.链接1_LinkClicked);
            // 
            // 说明
            // 
            this.说明.AutoSize = true;
            this.说明.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.说明.Location = new System.Drawing.Point(6, 33);
            this.说明.Name = "说明";
            this.说明.Size = new System.Drawing.Size(1403, 126);
            this.说明.TabIndex = 0;
            this.说明.Text = resources.GetString("说明.Text");
            // 
            // 主标签页
            // 
            this.主标签页.Controls.Add(this.tabPage1);
            this.主标签页.Dock = System.Windows.Forms.DockStyle.Fill;
            this.主标签页.Location = new System.Drawing.Point(0, 128);
            this.主标签页.MinimumSize = new System.Drawing.Size(300, 200);
            this.主标签页.Name = "主标签页";
            this.主标签页.SelectedIndex = 0;
            this.主标签页.Size = new System.Drawing.Size(1675, 589);
            this.主标签页.TabIndex = 1;
            this.主标签页.SelectedIndexChanged += new System.EventHandler(this.主标签页_SelectedIndexChanged);
            this.主标签页.DoubleClick += new System.EventHandler(this.主标签页_DoubleClick);
            // 
            // 无限主窗体
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1675, 717);
            this.Controls.Add(this.主标签页);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "无限主窗体";
            this.Text = "無限次元V1.0";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.缩略图片框)).EndInit();
            this.图片右键菜单.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.主标签页.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer splitContainer1;
        public 控件.局部放大图 局部放大图1;
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
    }
}