using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sokoban.Logic
{
    public class LevelsCollection
    {
        public int NumberOfLevels { get; private set; }

        private XDocument _levelsFile;
        private Level[] _levels;

        public LevelsCollection(string fileName)
        {
            LoadLevels(fileName);
        }

        public void LoadLevels(string fileName)
        {
            try
            {
                _levelsFile = XDocument.Load(fileName);
            }
            catch 
            {
                throw new Exception("File does not exist");
            }

            NumberOfLevels = _levelsFile.Descendants("Level").Count();
        }

        public Level this[int levelNumber]
        {
            get { return GetLevel(levelNumber); }
        }

        private Level GetLevel(int levelNumber)
        {
            var level = _levelsFile.Descendants("Level").Skip(levelNumber).First().Elements();
            int levelWidth = (from row in level select row.Value.Length).Max();
                              
            int levelHeight = level.Count();

            string[] levelData = new string[levelHeight];
            int rowNumber = 0;
            foreach (var row in level)
            {
                levelData[rowNumber] += row.Value;
                rowNumber++;
            }

            return new Level() { Data = levelData, Width = levelWidth, Height = levelHeight };
        }

    }
}
