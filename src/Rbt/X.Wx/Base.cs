using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace X.Wx
{
    public class Base : Form
    {
        #region 拖动引用
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        private PictureBox bt_close;
        protected Label lb_title;
        public const int HTCAPTION = 0x0002;
        #endregion

        #region 边框
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
        int nLeftRect, // x-coordinate of upper-left corner
        int nTopRect, // y-coordinate of upper-left corner
        int nRightRect, // x-coordinate of lower-right corner
        int nBottomRect, // y-coordinate of lower-right corner
        int nWidthEllipse, // height of ellipse
        int nHeightEllipse // width of ellipse
        );
        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

        private bool m_aeroEnabled;                     // variables for box shadow
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ACTIVATEAPP = 0x001C;
        public struct MARGINS                           // struct for box shadow
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }
        private const int WM_NCHITTEST = 0x84;          // variables for dragging the form
        private const int HTCLIENT = 0x1;
        protected override CreateParams CreateParams
        {
            get
            {
                m_aeroEnabled = CheckAeroEnabled();

                CreateParams cp = base.CreateParams;
                if (!m_aeroEnabled)
                    cp.ClassStyle |= CS_DROPSHADOW;

                return cp;
            }
        }
        private bool CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                int enabled = 0;
                DwmIsCompositionEnabled(ref enabled);
                return (enabled == 1) ? true : false;
            }
            return false;
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0112 && m.WParam.ToInt32() == 61490) return;
            switch (m.Msg)
            {
                case WM_NCPAINT:
                    if (m_aeroEnabled)
                    {
                        var v = 2;
                        DwmSetWindowAttribute(this.Handle, 2, ref v, 4);
                        MARGINS margins = new MARGINS()
                        {
                            bottomHeight = 1,
                            leftWidth = 1,
                            rightWidth = 1,
                            topHeight = 1
                        };
                        DwmExtendFrameIntoClientArea(this.Handle, ref margins);

                    }
                    break;
                default:
                    break;
            }
            base.WndProc(ref m);

            if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT)     // drag the form
                m.Result = (IntPtr)HTCAPTION;

        }
        #endregion

        public App.Wc.Contact User { get; protected set; }

        protected App.Wc wx = null;
        protected bool isquit = true;
        protected bool ShowTitle { get { return lb_title.Visible; } set { lb_title.Visible = value; } }

        public Base() : this(null) { }
        public Base(App.Wc w)
        {
            InitializeComponent();
            wx = w;
            if (wx != null) wx.LogonOut += Wx_Logout;
        }

        protected void showLoading()
        {
            closeLoading();
            lb_title.Image = Properties.Resources.loading;
            lb_title.Text = "　　" + lb_title.Text;
        }
        protected void closeLoading()
        {
            lb_title.Image = null;
            lb_title.Text = lb_title.Text.Replace("　　", "");
        }

        private void Wx_Logout()
        {
            Application.Exit();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            bt_close.Location = new Point(Width - 8 - bt_close.Width, 8);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (isquit)
            {
                showLoading();
                if (wx != null) wx.Quit();
            }
            base.OnClosing(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            lb_title.Text = Text;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Base));
            this.lb_title = new System.Windows.Forms.Label();
            this.bt_close = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.bt_close)).BeginInit();
            this.SuspendLayout();
            // 
            // lb_title
            // 
            this.lb_title.AutoSize = true;
            this.lb_title.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_title.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lb_title.Location = new System.Drawing.Point(12, 12);
            this.lb_title.Name = "lb_title";
            this.lb_title.Size = new System.Drawing.Size(42, 21);
            this.lb_title.TabIndex = 1;
            this.lb_title.Text = "标题";
            // 
            // bt_close
            // 
            this.bt_close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_close.Image = ((System.Drawing.Image)(resources.GetObject("bt_close.Image")));
            this.bt_close.Location = new System.Drawing.Point(152, 8);
            this.bt_close.Name = "bt_close";
            this.bt_close.Size = new System.Drawing.Size(30, 30);
            this.bt_close.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.bt_close.TabIndex = 2;
            this.bt_close.TabStop = false;
            this.bt_close.Click += new System.EventHandler(this.bt_close_Click);
            // 
            // Base
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(194, 271);
            this.Controls.Add(this.bt_close);
            this.Controls.Add(this.lb_title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Base";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.bt_close)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void bt_close_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="basecode"></param>
        /// <returns></returns>
        public Image base64ToImage(string basecode)
        {
            if (string.IsNullOrEmpty(basecode)) return null;
            var data = Convert.FromBase64String(basecode);
            var ms = new MemoryStream(data);
            var img = Image.FromStream(ms);
            ms.Close();
            ms.Dispose();
            return img;
        }
    }
}
