using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;
using System.Collections;

namespace Sokoban.Presentation
{
    public partial class TopPlayersForm : Form
    {
        public TopPlayersForm()
        {
            InitializeComponent();
            List<tPlayer> players = new List<tPlayer>();
            try
            {
                string resxFile = @".\topPlayersResources.resx";
                using (ResXResourceReader resxReader = new ResXResourceReader(resxFile))
                {
                    foreach (DictionaryEntry entry in resxReader)
                    {
                        if (((string)entry.Key).StartsWith("Player"))
                            players.Add((tPlayer)entry.Value);

                    }
                }
                if (players.Count < 10)
                {
                    for (int i = players.Count; i < 10; i++)
                    {
                        tPlayer np = new tPlayer();
                        np.name = "bob";
                        np.score = "0";
                        players.Add(np);

                    }

                }
            }
            catch
            {
                for (int i = 0; i < 10; i++)
                {
                    tPlayer np = new tPlayer();
                    np.name = "bob";
                    np.score = "0";
                    players.Add(np);
                }

            }
            List<tPlayer> sortedList = players.OrderByDescending(p => p.score).ToList();

            playersGrid.DataSource = new BindingList<tPlayer>(players);
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

    }
    class tPlayer
    {
        public string name { get; set; }
        public string score { get; set; }
    }
}
