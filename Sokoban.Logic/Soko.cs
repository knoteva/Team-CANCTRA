using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Activation;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban.Logic
{
    public class Soko : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            return _level.SelectMany(row => row).GetEnumerator();
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        //

        public enum MoveDirection
        {
            Up,
            Down,
            Right,
            Left
        }

        public event EventHandler LevelCompleted;
        private Elements[][] _level;
        private Elements _player;
        private int _goalsCount;
        private int _goalsFilled;
        private int _bonusCoins;
        private int _bonusTime;

        public void LoadLevevel(Level level)
        {
            Width = level.Width;
            Height = level.Height;
            _goalsCount = 0;
            _goalsFilled = 0;
            _bonusTime = 0;
            _bonusCoins = 0;         
            _level = new Elements[level.Data.Length][];

            for (int row = 0; row < level.Data.Length; row++)
            {
                _level[row] = new Elements[level.Data[row].Length];
                for (int col = 0; col < level.Data[row].Length; col++)
                {
                    _level[row][col] = new Elements() { Row = row, Col = col };

                    switch (level.Data[row][col])
                    {
                        case '@':
                            _level[row][col].Type = ElementsType.Player;
                            _player = _level[row][col];
                            break;
                        case '+':
                            _level[row][col].Type = ElementsType.PlayerOnGoal;
                            _player = _level[row][col];
                            break;
                        case '#':
                            _level[row][col].Type = ElementsType.Wall;
                            break;
                        case '$':
                            _level[row][col].Type = ElementsType.Box;
                            break;
                        case '*':
                            _level[row][col].Type = ElementsType.BoxOnGoal;
                            _goalsCount++;
                            _goalsFilled++;
                            break;
                        case '.':
                            _level[row][col].Type = ElementsType.Goal;
                            _goalsCount++;
                            break;
                        case ' ':
                            _level[row][col].Type = ElementsType.Floor;
                            break;
                        case '~':
                            _level[row][col].Type = ElementsType.BonusTime;
                            _bonusTime += 5;
                            break;
                        case '%':
                            _level[row][col].Type = ElementsType.BonusPoints;
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
            int newPlayerCol = _player.Col;
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

            bool isWallThere = _level[newPlayerRow][newPlayerCol].Type == ElementsType.Wall;
            bool isTryMoveBox = _level[newPlayerRow][newPlayerCol].Type == ElementsType.Box ||
                                _level[newPlayerRow][newPlayerCol].Type == ElementsType.BoxOnGoal;
            bool boxCanMove = isTryMoveBox && (_level[newBoxRow][newBoxCol].Type == ElementsType.Floor || _level[newBoxRow][newBoxCol].Type == ElementsType.Goal);

            if (!isWallThere && !(isTryMoveBox && !boxCanMove))
            {
                List<Elements> elementsList = new List<Elements>()
                {
                    new Elements() {Type = _level[_player.Row][_player.Col].Type, Row = _player.Row, Col = _player.Col},
                    new Elements() {Type = _level[newPlayerRow][newPlayerCol].Type, Row = newPlayerRow, Col = newPlayerCol}
                };

                _level[_player.Row][_player.Col].Type = _level[_player.Row][_player.Col].Type == ElementsType.PlayerOnGoal ? ElementsType.Goal: ElementsType.Floor;
                                                                                                                                
                // TODO if for coins and time
                if (_level[newPlayerRow][newPlayerCol].Type == ElementsType.Goal || 
                    _level[newPlayerRow][newPlayerCol].Type == ElementsType.BoxOnGoal)
                    
                {
                    if (_level[newPlayerRow][newPlayerCol].Type == ElementsType.BoxOnGoal)
                    {
                        _goalsFilled--;
                    }

                    _level[newPlayerRow][newPlayerCol].Type = ElementsType.PlayerOnGoal;
                }
                else
                {
                    _level[newPlayerRow][newPlayerCol].Type = ElementsType.Player;
                }

                if (isTryMoveBox)
                {
                    elementsList.Add(new Elements() { Type = _level[newBoxRow][newBoxCol].Type, Row = newBoxRow, Col = newBoxCol });

                    if (_level[newBoxRow][newBoxCol].Type == ElementsType.Goal)
                    {
                        _level[newBoxRow][newBoxCol].Type = ElementsType.BoxOnGoal;
                        _goalsFilled++;

                        if (_goalsFilled == _goalsCount && LevelCompleted != null)
                        {
                            LevelCompleted(this, new EventArgs());
                        }
                    }
                    else
                    {
                        _level[newBoxRow][newBoxCol].Type = ElementsType.Box;
                    }
                }

                _player = _level[newPlayerRow][newPlayerCol];
                hasMoved = true;
                //
            }
            return hasMoved;
        }
    }
}
