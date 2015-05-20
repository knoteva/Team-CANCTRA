using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Activation;
using System.Text;
using System.Threading.Tasks;


namespace Sokoban.Logic
{
    public class Soko : IEnumerable
    {
        public Soko()
        {
            this.Collections = new List<LevelCollection>();
            this.Collections = GetCollections();
            this.SelectedCollection = new LevelCollection();
        }
        public IEnumerator GetEnumerator()
        {
            return _level.SelectMany(row => row).GetEnumerator();
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public List<LevelCollection> Collections { get; set; }
        public LevelCollection SelectedCollection { get; set; }
        public int CurrentLevel { get; private set; }

        public enum MoveDirection
        {
            Up,
            Down,
            Right,
            Left
        }

        public event EventHandler LevelCompleted;
        private Element[][] _level;
        private Element _player;
        private int _goalsCount;
        private int _goalsFilled;
        private int _bonusCoins;
        private int _bonusTime;

        public void LoadLevel(Level level)
        {
            Width = level.Width;
            Height = level.Height;
            _goalsCount = 0;
            _goalsFilled = 0;
            _bonusTime = 0;
            _bonusCoins = 0;
            _level = new Element[level.Data.Length][];

            for (int row = 0; row < level.Data.Length; row++)
            {
                _level[row] = new Element[level.Data[row].Length];
                for (int col = 0; col < level.Data[row].Length; col++)
                {
                    _level[row][col] = new Element() { Row = row, Column = col };

                    switch (level.Data[row][col])
                    {
                        case '@':
                            _level[row][col].Type = ElementType.Player;
                            _player = _level[row][col];
                            break;
                        case '+':
                            _level[row][col].Type = ElementType.PlayerOnGoal;
                            _player = _level[row][col];
                            break;
                        case '#':
                            _level[row][col].Type = ElementType.Wall;
                            break;
                        case '$':
                            _level[row][col].Type = ElementType.Box;
                            break;
                        case '*':
                            _level[row][col].Type = ElementType.BoxOnGoal;
                            _goalsCount++;
                            _goalsFilled++;
                            break;
                        case '.':
                            _level[row][col].Type = ElementType.Goal;
                            _goalsCount++;
                            break;
                        case ' ':
                            _level[row][col].Type = ElementType.Floor;
                            break;
                        case '~':
                            _level[row][col].Type = ElementType.BonusTime;
                            _bonusTime += 5;
                            break;
                        case '%':
                            _level[row][col].Type = ElementType.BonusPoints;
                            _bonusCoins += 10;
                            break;
                    }
                }
            }
            //
        }

        public bool MovePlayer(MoveDirection moveDirection)
        {
            int newPlayerRow = _player.Row;
            int newPlayerCol = _player.Column;
            int newBoxRow = newPlayerRow;
            int newBoxCol = newPlayerCol;
            bool hasMoved = false;

            switch (moveDirection)
            {
                case MoveDirection.Right:
                    newPlayerCol += 1;
                    newBoxCol += 2;
                    break;
                case MoveDirection.Left:
                    newPlayerCol -= 1;
                    newBoxCol -= 2;
                    break;
                case MoveDirection.Up:
                    newPlayerRow -= 1;
                    newBoxRow -= 2;
                    break;
                case MoveDirection.Down:
                    newPlayerRow += 1;
                    newBoxRow += 2;
                    break;
            }

            bool isWallThere = _level[newPlayerRow][newPlayerCol].Type == ElementType.Wall;
            bool isTryMoveBox = _level[newPlayerRow][newPlayerCol].Type == ElementType.Box ||
                                _level[newPlayerRow][newPlayerCol].Type == ElementType.BoxOnGoal;
            bool boxCanMove = isTryMoveBox && (_level[newBoxRow][newBoxCol].Type == ElementType.Floor || _level[newBoxRow][newBoxCol].Type == ElementType.Goal);

            if (!isWallThere && !(isTryMoveBox && !boxCanMove))
            {
                List<Element> elementsList = new List<Element>()
                {
                    new Element() {Type = _level[_player.Row][_player.Column].Type, Row = _player.Row, Column = _player.Column},
                    new Element() {Type = _level[newPlayerRow][newPlayerCol].Type, Row = newPlayerRow, Column = newPlayerCol}
                };

                _level[_player.Row][_player.Column].Type = _level[_player.Row][_player.Column].Type == ElementType.PlayerOnGoal ? ElementType.Goal : ElementType.Floor;

                // TODO if for coins and time
                if (_level[newPlayerRow][newPlayerCol].Type == ElementType.Goal ||
                    _level[newPlayerRow][newPlayerCol].Type == ElementType.BoxOnGoal)
                {
                    if (_level[newPlayerRow][newPlayerCol].Type == ElementType.BoxOnGoal)
                    {
                        _goalsFilled--;
                    }

                    _level[newPlayerRow][newPlayerCol].Type = ElementType.PlayerOnGoal;
                }
                else
                {
                    _level[newPlayerRow][newPlayerCol].Type = ElementType.Player;
                }

                if (isTryMoveBox)
                {
                    elementsList.Add(new Element() { Type = _level[newBoxRow][newBoxCol].Type, Row = newBoxRow, Column = newBoxCol });

                    if (_level[newBoxRow][newBoxCol].Type == ElementType.Goal)
                    {
                        _level[newBoxRow][newBoxCol].Type = ElementType.BoxOnGoal;
                        _goalsFilled++;

                        if (_goalsFilled == _goalsCount && LevelCompleted != null)
                        {
                            LevelCompleted(this, new EventArgs());
                        }
                    }
                    else
                    {
                        _level[newBoxRow][newBoxCol].Type = ElementType.Box;
                    }
                }

                _player = _level[newPlayerRow][newPlayerCol];
                hasMoved = true;
                //
            }
            return hasMoved;
        }



        public bool IsPlaying { get; set; }


        private List<LevelCollection> GetCollections()
        {
            string[] files = Directory.GetFiles("Levels");
            List<LevelCollection> levels = new List<LevelCollection>();

            foreach (string file in files)
            {
                levels.Add(new LevelCollection(file));
            }

            return levels;
        }

        public void StartNewGame(GameType game)
        {
            if (game == GameType.Standart)
            {
                this.SelectedCollection = this.Collections.FirstOrDefault(x => x.CollectionName == "Standart");
                if (this.SelectedCollection == null)
                {
                    throw new ArgumentNullException("Избраната колекция е празна!");
                }
                CurrentLevel = 1;
                this.LoadLevel(this.SelectedCollection[CurrentLevel]);
            }
            else if (game == GameType.Practice)
            {

            }

            IsPlaying = true;
        }


        public void NextLevel()
        {
            this.LoadLevel(this.SelectedCollection[++CurrentLevel]);
        }

        public void LoadLevel(int levelNumber)
        {
            this.CurrentLevel = levelNumber;
        }

        public int GetSelectedCollectionWidth()
        {
            return SelectedCollection[CurrentLevel].Width;
        }

        public int GetSelectedCollectionHeight()
        {
            return SelectedCollection[CurrentLevel].Height;
        }
    }
}
