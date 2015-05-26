using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Activation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Sokoban.Logic
{
    public class Soko : IEnumerable
    {
        public Soko()
        {
            this.Collections = new List<LevelCollection>();
            this.Collections = GetCollections();
            this.SelectedCollection = this.Collections.FirstOrDefault();
            _movesHistory = new Stack<List<Element>>();
        }

        #region Variables

        public int Width { get; private set; }
        public int Height { get; private set; }
        public List<LevelCollection> Collections { get; set; }
        public LevelCollection SelectedCollection { get; set; }
        public int CurrentLevel { get; private set; }
        public bool IsPlaying { get; set; }
        public bool IsLastLevel { get; set; }
        public bool IsLevelCompleted { get; set; }
        public int StartScore { get; set; }
        public int TotalScore { get; set; }
        public int TimeLeft { get; set; }

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

        private Stack<List<Element>> _movesHistory;
        public int MovesHistoryCount { get { return _movesHistory.Count; } }

        #endregion Variables

        /// <summary>
        /// Зарежда дадено ниво като конвертира символите от картата в конкретни обекти които после ще се визуализират във формата('@' == играча)
        /// </summary>
        /// <param name="level">Ниво</param>
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
            _movesHistory.Clear();
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

            if (_level[newPlayerRow][newPlayerCol].Type == ElementType.BonusPoints)
            {
                this.StartScore += 10;
            }

            if (_level[newPlayerRow][newPlayerCol].Type == ElementType.BonusTime)
            {
                this.TimeLeft += 10;
            }

            if (!isWallThere && !(isTryMoveBox && !boxCanMove))
            {
                List<Element> elementsList = new List<Element>()
                {
                    new Element() {Type = _level[_player.Row][_player.Column].Type, Row = _player.Row, Column = _player.Column},
                    new Element() {Type = _level[newPlayerRow][newPlayerCol].Type, Row = newPlayerRow, Column = newPlayerCol}
                };

                _level[_player.Row][_player.Column].Type = _level[_player.Row][_player.Column].Type == ElementType.PlayerOnGoal ? ElementType.Goal : ElementType.Floor;


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
                            this.TotalScore = this.StartScore + _goalsFilled * 10 + (this.TimeLeft * 2);
                            this.StartScore = this.TotalScore;
                            this.IsLevelCompleted = true;
                            if (this.CurrentLevel == this.SelectedCollection.NumberOfLevels)
                            {
                                this.IsLastLevel = true;
                            }

                            LevelCompleted(this, new EventArgs());
                        }
                    }
                    else
                    {
                        _level[newBoxRow][newBoxCol].Type = ElementType.Box;
                    }
                }

                _movesHistory.Push(elementsList);
                _player = _level[newPlayerRow][newPlayerCol];
                hasMoved = true;
                //
            }
            return hasMoved;
        }
        /// <summary>
        /// Зарежда всички нива от колекцията Levels
        /// </summary>
        /// <returns>Списък с колекции</returns>
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

        /// <summary>
        /// Прави нова игра като започва от 1 ниво
        /// </summary>
        /// <param name="game">Тип на играта(Стандартна или Практикувай)</param>
        public void StartStandartGame()
        {
            if (this.SelectedCollection == null)
            {
                throw new ArgumentNullException("Избраната колекция е празна!");
            }
            this.CurrentLevel = 1;
            this.TotalScore = 0;
            this.StartScore = 0;
            this.TimeLeft = 120;
            this.LoadLevel();
            IsPlaying = true;
        }

        /// <summary>
        /// Зарежда следващото ниво
        /// </summary>
        public void NextLevel()
        {
            CurrentLevel++;
            this.TimeLeft = 120;
            this.StartScore = this.TotalScore;
            this.IsPlaying = true;
            this.LoadLevel();
        }

        /// <summary>
        /// Зарежда ниво
        /// </summary>
        /// <param name="levelNumber">Номер на нивото</param>
        public void LoadLevel()
        {
            this.LoadLevel(this.SelectedCollection[CurrentLevel]);
        }

        /// <summary>
        /// Връща макс. дължина на текущото ниво
        /// </summary>
        /// <returns>Дължина на нивото</returns>
        public int GetSelectedCollectionWidth()
        {
            return SelectedCollection[CurrentLevel].Width;
        }
        /// <summary>
        /// Връща макс. височина на текущото ниво
        /// </summary>
        /// <returns>Височина на нивото</returns>
        public int GetSelectedCollectionHeight()
        {
            return SelectedCollection[CurrentLevel].Height;
        }

        public IEnumerator GetEnumerator()
        {
            return _level.SelectMany(row => row).GetEnumerator();
        }

        public void StartPractice()
        {
            this.CurrentLevel = 1;
            this.IsPlaying = true;
            this.TotalScore = 0;
            this.LoadLevel();
        }

        public void RestartLevel()
        {
            this.IsPlaying = true;
            this.StartScore = this.TotalScore;
            this.TimeLeft = 120;
            this.LoadLevel();
        }

        public void SetCurrentLevel(int current)
        {
            this.CurrentLevel = current;
        }

        public void UndoMovement()
        {
            if (_movesHistory.Count > 0)
            {
                List<Element> elementsList = _movesHistory.Pop();

                foreach (var element in elementsList)
                {
                    if (element.Type == ElementType.BoxOnGoal)
                    {
                        _goalsFilled++;
                    }
                    else if (element.Type == ElementType.Goal &&
                             _level[element.Row][element.Column].Type == ElementType.BoxOnGoal)
                    {
                        _goalsFilled--;
                    }
                    else if (element.Type == ElementType.BonusPoints)
                    {
                        this.StartScore -= 10;
                    }
                    else if (element.Type == ElementType.BonusTime)
                    {
                        this.TimeLeft -= 10;
                    }

                    _level[element.Row][element.Column].Type = element.Type;
                }

                _player = elementsList[0];
            }
        }

    }
}
