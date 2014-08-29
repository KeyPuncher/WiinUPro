using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WiinUSoft
{
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
        }

        private void buttonListGames_Click(object sender, EventArgs e)
        {
            foreach (string s in Main.testList)
                logBox.AppendText(s + "\n");
        }

        private void buttonAddProcess_Click(object sender, EventArgs e)
        {
            String game = Microsoft.VisualBasic.Interaction.InputBox("Process Name:", "Add Test Process/Game", "Steam.exe");
            game = game.Replace(".exe", "");
            if (game != null && game != "")
                Main.AddGame(game);
        }
    }
}
