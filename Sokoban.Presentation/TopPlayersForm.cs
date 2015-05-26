using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Resources;
using System.Collections;
using Sokoban.Presentation.Helpers;
using System.IO;

namespace Sokoban.Presentation
{
    public partial class TopPlayersForm : Form
    {

        private List<TopPlayer> players = new List<TopPlayer>();
        private TopPlayer tp;

        public TopPlayersForm()
        {
            InitializeComponent();
            tp = new TopPlayer();

            //Create directory if it does not exst
            if (!Directory.Exists(Path.GetFullPath("TopPlayers")))
            {
                Directory.CreateDirectory(Path.GetFullPath("TopPlayers"));
            }
            string path = Path.GetFullPath("TopPlayers\\topPlayers.txt");
            string[] lines = new string[10] { "0 bob", "0 bob", "0 bob", "0 bob", "0 bob", "0 bob", "0 bob", "0 bob", "0 bob", "0 bob" };
            //Create file if it does not exist
            if (!File.Exists(path))
            {
                File.Create(path).Close();
                File.WriteAllLines(path, lines);
            }
            else
            {
                lines = File.ReadAllLines(Path.GetFullPath("TopPlayers\\topPlayers.txt"));
            }

            //fill array with top players
            foreach (string line in lines)
            {
                string[] p = line.Split(' ');
                tp = new TopPlayer();
                tp.Name = p[1];
                tp.Score = p[0];
                players.Add(tp);
            }

            //bind array to datagridview
            playersGrid.DataSource = new BindingList<TopPlayer>(players);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void playersGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (e.RowIndex < 10)
            {
                playersGrid.Rows[e.RowIndex].Cells[0].Value
            = (e.RowIndex + 1).ToString();
            }
        }

        public static void ShowForm()
        {
            using (TopPlayersForm form = new TopPlayersForm())
            {
                form.ShowDialog();
            }
        }

    }

}
