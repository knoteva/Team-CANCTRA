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
    public partial class topPlayers : Form
    {
        public topPlayers()
        {
            InitializeComponent();
            string file = Properties.Resources.topPlayers;
            List<string> players = file.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<tPlayer> playerList = new List<tPlayer>();
            foreach (string s in players)
            {
                tPlayer p = new tPlayer();
                string[] pLine = s.Split(' ');
                p.name = pLine[1];
                p.score = pLine[0];
                playerList.Add(p);
            }

            //DataGridView playersGrid = new DataGridView();
            //playersGrid.DataSource = null;
            playersGrid.DataSource = new BindingList<tPlayer>(playerList);
        }

    }
    class tPlayer
    {
        public string name { get; set; }
        public string score { get; set; }
    }
}
