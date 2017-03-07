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
            rsp.Data.Add(new City() { name = "广州", id = 440100 });
            foreach (var c in rsp.Data) cb_c.Items.Add(c);
            foreach (var c in Rbt.user.Collect.Ids) lb_looks.Items.Add(c);
            foreach (var r in Rbt.user.Send) lb_rules.Items.Add(r);
            tb_keys.Text = Rbt.user.Collect.Keys;
            cb_c.SelectedText = Rbt.cfg.CityName;
            tb_id_fail.Text = Rbt.user.Reply.Identify_Fail;
            tb_id_succ.Text = Rbt.user.Reply.Identify_Succ;
            tb_send_succ.Text = Rbt.user.Reply.Send_Succ;
            tb_online.Text = Rbt.user.Reply.Rbt_Online;
            cb_send_on_fail.Checked = Rbt.user.Reply.SendTpl_OnFail;
            tb_tpl.Text = Rbt.user.Reply.Msg_Tpl;
            cb_debug.Checked = Rbt.user.IsDebug;
            tb_api.Text = Rbt.cfg.GateWay;
            tb_sms.Text = Rbt.user.Tels;
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

            var c = tb_gname.Tag as Wc.Contact;
            r.NickName = c.NickName;
            r.UserName = c.UserName;

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
            Rbt.user.Reply.Identify_Fail = tb_id_fail.Text;
            Rbt.user.Reply.Identify_Succ = tb_id_succ.Text;
            Rbt.user.Reply.Send_Succ = tb_send_succ.Text;
            Rbt.user.Reply.Msg_Tpl = tb_tpl.Text;
            Rbt.user.Reply.Rbt_Online = tb_online.Text;
            Rbt.user.Reply.SendTpl_OnFail = cb_send_on_fail.Checked;
            Rbt.user.IsDebug = cb_debug.Checked;
            Rbt.user.Tels = tb_sms.Text;

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
        }

        private void bt_select_contact_Click(object sender, EventArgs e)
        {
            var cot = new Contacts();
            if (cot.ShowDialog() == DialogResult.OK)
            {
                lb_looks.Items.Add(cot.SelectedContact.NickName);
                Rbt.user.Collect.Ids.Add(cot.SelectedContact.NickName);
            }
        }

        private void tb_gname_Enter(object sender, EventArgs e)
        {
            var cot = new Contacts();
            if (cot.ShowDialog() == DialogResult.OK)
            {
                tb_gname.Text = cot.SelectedContact.NickName;
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
    }
}



