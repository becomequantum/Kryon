using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 加水印 {
    class 文印设置Tabpage : TabPage {
        public 文印参数 本页参数;

        public 文印设置Tabpage(文印参数 参数) {
            本页参数 = 参数;
            BackColor = Color.White;
            Text = 参数.文本.Length > 4 ? 参数.文本.Substring(0,4) : 参数.文本;
        }
    }

    class 图印设置Tabpage : TabPage {
        public 图印参数 本页参数;

        public 图印设置Tabpage(图印参数 参数) {
            本页参数 = 参数;
            BackColor = Color.White;
        }
    }
}
