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
        public int selectedLevel { get; set; }
        int cellSize;
        int paddingX;
        int paddingY;
        string file;
        public LevelSelectionForm()
        {
            InitializeComponent();
            soko = new Soko();
        }

        private void levelSelectionGrid_SelectionChanged(object sender, EventArgs e)
        {
            selectedLevel = levelSelectionGrid.CurrentRow.Index;

            LevelsCollection selectedLevelCollection = new LevelsCollection(file);
            soko.LoadLevevel(selectedLevelCollection[selectedLevel]);

            cellSize = levelPreview.Width / Math.Max(soko.Width, soko.Height);
            paddingX = (levelPreview.Width - (soko.Width * cellSize)) / 2;
            paddingY = (levelPreview.Height - (soko.Height * cellSize)) / 2;

            levelPreview.Invalidate();


        }

        private void LevelSelectionForm_Load(object sender, EventArgs e)
        {
            string cwd = Directory.GetCurrentDirectory();
            if (cwd.EndsWith("\\bin\\Debug"))
            {
                cwd = cwd.Replace("\\bin\\Debug", "");
            }          
            file = cwd + @"\\Levels\\Levels.slc";
            LevelsCollection selectedLevelCollection = new LevelsCollection(file);
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
                    
                }

                if (img != null)
                {
                    e.Graphics.DrawImage(img, paddingX + element.Col * cellSize,
                                         paddingY + element.Row * cellSize,
                                         cellSize, cellSize);
                }
            }
        }


    }
}
