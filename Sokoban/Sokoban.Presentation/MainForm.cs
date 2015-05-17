using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sokoban.Presentation
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void GoToLevel(int levelNumber)
        {

        }

        private void levelSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LevelSelectionForm levelSelectionForm = new LevelSelectionForm();
            if (levelSelectionForm.ShowDialog() == DialogResult.OK)
            {
               
            }
        }
    }
}
