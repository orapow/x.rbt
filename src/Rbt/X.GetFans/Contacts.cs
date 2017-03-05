using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using X.GetFans.App;

namespace X.GetFans
{
    public partial class Contacts : Base
    {
        public Wc.Contact SelectedContact { get; set; }
        public Contacts()
        {
            InitializeComponent();
            Text = "选择通讯录";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            foreach (var c in Rbt.user.Contacts) lb_list.Items.Add(c);
        }

        private void lb_list_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();
            if (e.Index < 0) return;
            e.Graphics.DrawString(lb_list.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.Left + 5, e.Bounds.Top + 5);
        }

        private void lb_list_DoubleClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            SelectedContact = lb_list.SelectedItem as Wc.Contact;
            Close();
        }

        private void lb_key_KeyUp(object sender, KeyEventArgs e)
        {
            var cs = Rbt.user.Contacts.Where(o => o.NickName.Contains(lb_key.Text));
            lb_list.Items.Clear();
            foreach (var c in cs) lb_list.Items.Add(c);
        }
    }
}
