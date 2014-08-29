using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WiinUSoft
{
    public partial class Configure : Form
    {
        public Configure()
        {
            InitializeComponent();

            comboA.SelectedIndex = 1;
            comboB.SelectedIndex = 2;
            comboX.SelectedIndex = 3;
            comboY.SelectedIndex = 4;
            comboL.SelectedIndex = 5;
            comboR.SelectedIndex = 6;
            comboZL.SelectedIndex = 7;
            comboZR.SelectedIndex = 8;
            comboLS.SelectedIndex = 9;
            comboRS.SelectedIndex = 10;
            comboUp.SelectedIndex = 11;
            comboDown.SelectedIndex = 12;
            comboLeft.SelectedIndex = 13;
            comboRight.SelectedIndex = 14;
            comboStart.SelectedIndex = 15;
            comboSelect.SelectedIndex = 16;
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }
    }
}
