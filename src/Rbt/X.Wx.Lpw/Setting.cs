using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using X.Wx.App;

namespace X.Wx
{
    public partial class Setting : Base
    {
        public Setting()
        {
            InitializeComponent();
        }

        private void Rules_Load(object sender, EventArgs e)
        {
            Text = "软件配置";
            Rbt.LoadConfig();
            var rsp = Sdk.LoadCity();
            rsp.Data.Add(new City() { name = "广州", id = 440100 });
            foreach (var c in rsp.Data) cb_c.Items.Add(c);
            foreach (var c in Rbt.cfg.Contacts.OrderBy(o => o.UserName[1] == '@' ? 0 : 1))
            {
                clb_looks.Items.Add(c);
                if (Rbt.cfg.Collect.Ids.Contains(c.NickName)) clb_looks.SetItemChecked(clb_looks.Items.Count - 1, true);
                cb_cots.Items.Add(c);
            }
            foreach (var r in Rbt.cfg.Send) lb_rules.Items.Add(r);
            tb_keys.Text = Rbt.cfg.Collect.Keys;
            cb_c.SelectedText = Rbt.cfg.CityName;
            tb_id_fail.Text = Rbt.cfg.Reply.Identify_Fail;
            tb_id_succ.Text = Rbt.cfg.Reply.Identify_Succ;
            tb_send_succ.Text = Rbt.cfg.Reply.Send_Succ;
            tb_online.Text = Rbt.cfg.Reply.Rbt_Online;
            cb_send_on_fail.Checked = Rbt.cfg.Reply.SendTpl_OnFail;
            tb_tpl.Text = Rbt.cfg.Reply.Msg_Tpl;
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
            var r = Rbt.cfg.Send.FirstOrDefault(o => o.BuildName == tb_bname.Text);
            if (r == null) { r = new Rbt.SendRule() { }; }

            r.BuildName = tb_bname.Text;

            var c = cb_cots.SelectedItem as Wc.Contact;
            r.NickName = c.NickName;
            r.UserName = c.UserName;

            var idx = lb_rules.Items.IndexOf(r);

            if (idx == -1) { lb_rules.Items.Add(r); Rbt.cfg.Send.Add(r); }
            else lb_rules.SelectedItem = r;

            cb_cots.SelectedItem = null;
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

            Rbt.cfg.Collect.Keys = tb_keys.Text;

            Rbt.cfg.Reply.Identify_Fail = tb_id_fail.Text;
            Rbt.cfg.Reply.Identify_Succ = tb_id_succ.Text;
            Rbt.cfg.Reply.Send_Succ = tb_send_succ.Text;
            Rbt.cfg.Reply.Msg_Tpl = tb_tpl.Text;
            Rbt.cfg.Reply.Rbt_Online = tb_online.Text;
            Rbt.cfg.Reply.SendTpl_OnFail = cb_send_on_fail.Checked;

            Rbt.cfg.GateWay = tb_api.Text;

            Sdk.Init();

            Rbt.SaveConfig();

            Close();
        }

        private void bt_remove_Click(object sender, EventArgs e)
        {
            var s = lb_rules.SelectedItem;
            if (s == null) return;
            Rbt.cfg.Send.Remove(s as Rbt.SendRule);
            lb_rules.Items.Remove(s);
            tb_bname.Text = "";
            cb_cots.Text = "";
            cb_cots.SelectedItem = null;
        }

        private void clb_looks_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var c = clb_looks.Items[e.Index] as Wc.Contact;
            if (Rbt.cfg.Collect.Ids == null) Rbt.cfg.Collect.Ids = new List<string>();
            if (e.NewValue == CheckState.Checked)
            {
                if (!Rbt.cfg.Collect.Ids.Contains(c.NickName)) Rbt.cfg.Collect.Ids.Add(c.NickName);
            }
            else Rbt.cfg.Collect.Ids.Remove(c.NickName);
        }

        private void lb_rules_Click(object sender, EventArgs e)
        {
            var r = lb_rules.SelectedItem as Rbt.SendRule;
            if (r == null) return;
            tb_bname.Text = r.BuildName;
            var c = Rbt.cfg.Contacts.FirstOrDefault(o => o.NickName == r.NickName);
            cb_cots.SelectedItem = c;
            cb_cots.Text = r.NickName;
        }
    }
}



