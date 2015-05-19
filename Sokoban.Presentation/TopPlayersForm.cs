using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Resources;
using System.Collections;
using Sokoban.Presentation.Helpers;

namespace Sokoban.Presentation
{
    public partial class TopPlayersForm : Form
    {
        public TopPlayersForm()
        {
            InitializeComponent();
            List<TopPlayer> players = new List<TopPlayer>();
            try
            {
                string resxFile = @".\topPlayersResources.resx";
                using (ResXResourceReader resxReader = new ResXResourceReader(resxFile))
                {
                    foreach (DictionaryEntry entry in resxReader)
                    {
                        if (((string)entry.Key).StartsWith("Player"))
                            players.Add((TopPlayer)entry.Value);

                    }
                }
                if (players.Count < 10)
                {
                    for (int i = players.Count; i < 10; i++)
                    {
                        TopPlayer np = new TopPlayer();
                        np.Name = "bob";
                        np.Score = "0";
                        players.Add(np);
                    }

                }
            }
            catch
            {
                for (int i = 0; i < 10; i++)
                {
                    TopPlayer np = new TopPlayer();
                    np.Name = "bob";
                    np.Score = "0";
                    players.Add(np);
                }

            }

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

    }

}
