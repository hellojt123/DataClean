using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace DataClean.Components.Dialog
{
    public partial class ProcessDialog : MetroFramework.Forms.MetroForm
    {
        public ProcessDialog()
        {
            InitializeComponent();
            initDialog();
        }

        private void initDialog()
        {
            listBox1.MouseClick += onListBoxClick;
            metroLabel1.Text = "";
            metroLabel1.SizeChanged += onTextWindowChanged;       
         
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel =true;
            this.Hide();
        }

        public void ShowDialog(bool isShow)
        {
            if (isShow)
            {
                this.Show();
                listBox1.Items.Clear();
                Dictionary<string, System.Diagnostics.Process> dict = DataClean.Util.PythonRunTime.Instance.getProcess();
                foreach (KeyValuePair<string, System.Diagnostics.Process> kvp in dict)
                {
                    listBox1.Items.Add(kvp.Key);
                }
            }
            else 
            {
                this.Hide();
            }           
        }   

        private void onListBoxClick(object sender, MouseEventArgs e)
        {
            metroLabel1.Text = "";
            Dictionary<string, StringBuilder> dict = DataClean.Util.PythonRunTime.Instance.getProcessStr();
            ListBox box = (ListBox)sender;
            if (box.SelectedItem != null)
            {
                string name = (string)box.SelectedItem;
                StringBuilder sb;
                dict.TryGetValue(name, out sb);
                if (sb != null)
                {
                    metroLabel1.Text = sb.ToString();                   
                }
            }           
        }
        delegate void updatetext();
        public void UpdateText()
        {
            this.Invoke(new updatetext(()=>_UpdateText()));
        }

        private void _UpdateText()
        {
            if (this.Visible)
            {
                if (listBox1.SelectedItem != null)
                {
                    Dictionary<string, StringBuilder> dict = DataClean.Util.PythonRunTime.Instance.getProcessStr();
                    string name = (string)listBox1.SelectedItem;
                    StringBuilder sb;
                    dict.TryGetValue(name, out sb);
                    if (sb != null)
                    {
                        metroLabel1.Text = sb.ToString();
                    }
                }
            }
        }

        private void onTextWindowChanged(object sender, EventArgs e)
        {
            metroPanel1.VerticalScroll.Value = metroPanel1.VerticalScroll.Maximum;
            metroPanel1.Update();
        }
        private void ProcessDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
