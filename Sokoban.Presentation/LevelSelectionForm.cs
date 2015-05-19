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
using Sokoban.Logic;


namespace Sokoban.Presentation
{
    public partial class LevelSelectionForm : Form
    {
        private Soko soko;
        public int SelectedLevel { get; set; }
        int _cellSize;
        int _paddingX;
        int _paddingY;
        string _file;
        public LevelSelectionForm()
        {
            InitializeComponent();
            soko = new Soko();
        }

        private void levelSelectionGrid_SelectionChanged(object sender, EventArgs e)
        {
            SelectedLevel = levelSelectionGrid.CurrentRow.Index;

            LevelsCollection selectedLevelCollection = new LevelsCollection(_file);
            soko.LoadLevevel(selectedLevelCollection[SelectedLevel]);

            _cellSize = levelPreview.Width / Math.Max(soko.Width, soko.Height);
            _paddingX = (levelPreview.Width - (soko.Width * _cellSize)) / 2;
            _paddingY = (levelPreview.Height - (soko.Height * _cellSize)) / 2;

            levelPreview.Invalidate();


        }

        private void LevelSelectionForm_Load(object sender, EventArgs e)
        {
            string cwd = Directory.GetCurrentDirectory();
            if (cwd.EndsWith("\\bin\\Debug"))
            {
                cwd = cwd.Replace("\\bin\\Debug", "");
            }          
            _file = cwd + @"\\Levels\\Levels.slc";
            var selectedLevelCollection = new LevelsCollection(_file);
            for (int i = 1; i <= selectedLevelCollection.NumberOfLevels; i++)
            {
                levelSelectionGrid.Rows.Add("Level " + i);
            }
        }

        //draw level preview
        private void levelPreview_Paint(object sender, PaintEventArgs e)
        {
            foreach (Elements element in soko)
            {
                Bitmap img = null;

                switch (element.Type)
                {
                    case ElementsType.Wall:
                        img = Properties.Resources.Wall;
                        break;
                    case ElementsType.Box:
                    case ElementsType.BoxOnGoal:
                        img = Properties.Resources.Box;
                        break;
                    case ElementsType.Goal:
                        img = Properties.Resources.Goal;
                        break;
                    case ElementsType.Player:
                    case ElementsType.PlayerOnGoal:
                        img = Properties.Resources.Player;
                        break;
                    case ElementsType.BonusTime:
                        img = Properties.Resources.Time;
                        break;
                    case ElementsType.BonusPoints:
                        img = Properties.Resources.Points;
                        break;
                    
                }

                if (img != null)
                {
                    e.Graphics.DrawImage(img, _paddingX + element.Col * _cellSize,
                                         _paddingY + element.Row * _cellSize,
                                         _cellSize, _cellSize);
                }
            }
        }


    }
}
