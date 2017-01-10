using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace X.Wx.Controls
{
    public partial class XPanel : UserControl
    {
        public List<XContact> list { get; private set; }
        public XContact Current { get; set; }

        public void AddContact(XContact ct)
        {
            list.Add(ct);
            if (list.Count < 15) ct.LoadImage();
            UpdateControls();
        }

        public void AddContactRange(List<XContact> cts)
        {
            list.AddRange(cts);
            UpdateControls();
        }

        public void RemoveContact(XContact ct)
        {
            list.Remove(ct);
            UpdateControls();
        }

        public XPanel()
        {
            list = new List<XContact>();
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            vsb.Value -= e.Delta > 0 ? vsb.SmallChange : -vsb.SmallChange;
            fp.VerticalScroll.Value = vsb.Value;
        }

        private void UpdateControls()
        {
            if (list == null) return;
            list = list.OrderByDescending(o => o.MsgCount).ToList();
            fp.Controls.Clear();
            fp.Controls.AddRange(list.ToArray());
            vsb.Maximum = fp.VerticalScroll.Maximum;
            vsb.Minimum = 0;
            foreach (var c in list.Where(o => o.Location.Y >= 0 && o.Location.Y <= this.Height)) c.LoadImage();
        }

        private void vsb_Scroll(object sender, ScrollEventArgs e)
        {
            fp.VerticalScroll.Value = vsb.Value;
        }
    }
}
