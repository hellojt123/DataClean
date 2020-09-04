using System;

namespace DataClean.Components.Dialog
{
    public partial class InputDialog : MetroFramework.Forms.MetroForm
    {
        public InputDialog()
        {
            InitializeComponent();
            initDialog();
        }

        private void initDialog()
        {
            RemoveCloseButton();
            metroButton1.Click += onClickOK;
            metroButton2.Click += onClickCancel;
        }



        private void InputDialog_Load(object sender, EventArgs e)
        {

        }

        private Action<string> action;

        public void showDialog(Action<string> callback) 
        {
            Show();
            action = callback;
        }

        private void onClickOK(object o,EventArgs e)
        {
            if (action != null) 
            {
                action(metroTextBox1.Text);
                action = null;
                metroTextBox1.Text = "";
                Hide();
            }
        }
        private void onClickCancel(object o, EventArgs e)
        {
            metroTextBox1.Text = "";
            Hide();
        }
    }
}
