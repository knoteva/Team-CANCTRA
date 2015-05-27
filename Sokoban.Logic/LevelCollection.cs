using System;
using System.Linq;
using System.Xml.Linq;

namespace Sokoban.Logic
{
    public class LevelCollection
    {
        public int NumberOfLevels { get; private set; }
        public string CollectionName { get; private set; }

        private XDocument _levelsFile;
       
        public LevelCollection(string fileName)
        {
            LoadLevels(fileName);
        }

        public LevelCollection() { }

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

            this.CollectionName = _levelsFile.Root.Element("LevelCollection").Attribute("Name").Value;
            NumberOfLevels = _levelsFile.Descendants("Level").Count();
        }

        public Level this[int levelNumber]
        {
            get { return GetLevel(levelNumber); }
        }

        private Level GetLevel(int levelNumber)
        {
            var level = _levelsFile.Descendants("Level").FirstOrDefault(t => t.Attribute("Id").Value == levelNumber.ToString()).Elements();
            if (level == null) throw new ArgumentNullException("Няма повече нива!");

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
