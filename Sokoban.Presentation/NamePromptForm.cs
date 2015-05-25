using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sokoban.Presentation.Helpers;
using System.IO;

namespace Sokoban.Presentation
{
    public partial class NamePromptForm : Form
    {
        public int score;
        public NamePromptForm()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            string nameEntered = nameBox.Text; 
            rewriteTopPlayers(nameEntered, score);
            Close();
        }

        private void rewriteTopPlayers(string name, int s)
        {
            if (!Directory.Exists(Path.GetFullPath("TopPlayers")))
            {
                Directory.CreateDirectory(Path.GetFullPath("TopPlayers"));
            }
            string path = Path.GetFullPath("TopPlayers\\topPlayers.txt");
            string[] lines = new string[] { "0 bob", "0 bob", "0 bob", "0 bob", "0 bob", "0 bob", "0 bob", "0 bob", "0 bob", "0 bob" };
            if (!File.Exists(path))
            {
                File.Create(path).Close(); 
                File.WriteAllLines(path, lines);
            }
            else
            {
                lines = File.ReadAllLines(path);
            }
           
            lines[9] = score.ToString() + " " + name;
            Array.Sort(lines);
            Array.Reverse(lines);
            File.WriteAllLines(path, lines, Encoding.Unicode);

        }
    }
}
