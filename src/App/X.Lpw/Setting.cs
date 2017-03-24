using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using X.Lpw.App;

namespace X.Lpw
{
    public partial class Setting : Base
    {
        public Setting()
        {
            InitializeComponent();
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            Text = "软件配置";
            Rbt.LoadConfig();
            var rsp = Sdk.LoadCity();
            if (rsp != null && rsp.Data != null)
            {
                rsp.Data.Add(new City() { name = "广州", id = 440100 });
                foreach (var c in rsp.Data) cb_c.Items.Add(c);
            }
            foreach (var c in Rbt.user.Collect.Ids) lb_looks.Items.Add(c);
            foreach (var r in Rbt.user.Send) lb_rules.Items.Add(r);
            tb_keys.Text = Rbt.user.Collect.Keys;
            cb_c.SelectedText = Rbt.cfg.CityName;
            cb_debug.Checked = Rbt.user.IsDebug;
            tb_api.Text = Rbt.cfg.GateWay;
            tb_succ.Text = Rbt.user.Reply.Succ;
            tb_warn.Text = Rbt.user.Reply.Warn_User;
            tb_fail.Text = Rbt.user.Reply.Fail;
            tb_fail1.Text = Rbt.user.Reply.Fail1;
        }

        private void lb_rules_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();
            if (e.Index < 0) return;
            e.Graphics.DrawString(lb_rules.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.Left + 5, e.Bounds.Top + 5);
        }

        private void bt_save_Click(object sender, EventArgs e)
        {
            var r = Rbt.user.Send.FirstOrDefault(o => o.BuildName == tb_bname.Text);
            if (r == null) { r = new Rbt.SendRule() { }; }

            r.BuildName = tb_bname.Text;
            r.NickName = tb_gname.Text;

            var idx = lb_rules.Items.IndexOf(r);

            if (idx == -1) { lb_rules.Items.Add(r); Rbt.user.Send.Add(r); }
            else lb_rules.SelectedItem = r;

            tb_gname.Text = "";
            tb_bname.Text = "";
        }

        private void bt_ok_Click(object sender, EventArgs e)
        {
            var ct = cb_c.SelectedItem as City;

            if (ct != null)
            {
                Rbt.cfg.CityName = ct.name;
                Rbt.cfg.City_Id = ct.id;
            }

            Rbt.user.Collect.Keys = tb_keys.Text;
            Rbt.user.IsDebug = cb_debug.Checked;
            Rbt.user.Reply.Fail = tb_fail.Text;
            Rbt.user.Reply.Fail1 = tb_fail1.Text;
            Rbt.user.Reply.Warn_User = tb_warn.Text;
            Rbt.user.Reply.Succ = tb_succ.Text;

            Rbt.cfg.GateWay = tb_api.Text;

            Sdk.Init();

            Rbt.SaveConfig();
            Rbt.SaveUser();

            Close();
        }

        private void bt_remove_Click(object sender, EventArgs e)
        {
            var s = lb_rules.SelectedItem;
            if (s == null) return;
            Rbt.user.Send.Remove(s as Rbt.SendRule);
            lb_rules.Items.Remove(s);
            tb_bname.Text = "";
            tb_gname.Text = "";
        }

        private void lb_rules_Click(object sender, EventArgs e)
        {
            var r = lb_rules.SelectedItem as Rbt.SendRule;
            if (r == null) return;
            tb_bname.Text = r.BuildName;
            tb_gname.Text = r.NickName;
            tb_gname.Tag = Rbt.user.Contacts.FirstOrDefault(o => o.NickName == r.NickName);
            if (tb_gname.Tag == null) tb_gname.Text = "";
        }

        private void bt_select_contact_Click(object sender, EventArgs e)
        {
            var cot = new Contacts();
            cot.SelectedContact = Rbt.user.Collect.Ids;
            if (cot.ShowDialog() == DialogResult.OK)
            {
                foreach (var c in cot.SelectedContact)
                {
                    lb_looks.Items.Add(c);
                    Rbt.user.Collect.Ids.Add(c);
                }
            }
        }

        private void tb_gname_Enter(object sender, EventArgs e)
        {
            var cot = new Contacts();
            cot.SelectedContact = Rbt.user.Send.Select(o => o.NickName).ToList();
            if (cot.ShowDialog() == DialogResult.OK)
            {
                tb_gname.Text = cot.SelectedContact[0];
                tb_gname.Tag = cot.SelectedContact;
            }
        }

        private void lb_looks_DoubleClick(object sender, EventArgs e)
        {
            var c = lb_looks.SelectedItem;
            if (c != null)
            {
                lb_looks.Items.Remove(c);
                Rbt.user.Collect.Ids.Remove(c.ToString());
            }
        }

        private void lb_looks_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();
            if (e.Index < 0) return;
            e.Graphics.DrawString(lb_looks.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.Left + 5, e.Bounds.Top + 5);
        }

        private void tb_warn_Enter(object sender, EventArgs e)
        {
            var cot = new Contacts();
            if (cot.ShowDialog() == DialogResult.OK)
            {
                tb_warn.Text = cot.SelectedContact[0];
            }
        }

        private void bt_loadcity_Click(object sender, EventArgs e)
        {
            var rsp = Sdk.LoadCity();
            if (rsp != null && rsp.Data != null)
            {
                rsp.Data.Add(new City() { name = "广州", id = 440100 });
                foreach (var c in rsp.Data) cb_c.Items.Add(c);
            }
        }
    }
}



