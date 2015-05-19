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

            foreach (Elements[] row in _level)
            {
                foreach (Elements element in row)
                {
                    yield return element;
                }
            }
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
        private Elements player;
        private int goalsCount;
        private int goalsFilled;
        private int bonusCount;

        public void LoadLevevel(Level level)
        {
            Width = level.Width;
            Height = level.Height;
            goalsCount = 0;
            goalsFilled = 0;
            bonusCount = 0;
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
                            player = _level[row][col];
                            break;
                        case '+':
                            _level[row][col].Type = ElementsType.PlayerOnGoal;
                            player = _level[row][col];
                            break;
                        case '#':
                            _level[row][col].Type = ElementsType.Wall;
                            break;
                        case '$':
                            _level[row][col].Type = ElementsType.Box;
                            break;
                        case '*':
                            _level[row][col].Type = ElementsType.BoxOnGoal;
                            goalsCount++;
                            goalsFilled++;
                            break;
                        case '.':
                            _level[row][col].Type = ElementsType.Goal;
                            goalsCount++;
                            break;
                        case ' ':
                            _level[row][col].Type = ElementsType.Floor;
                            break;
                        case '%':
                            _level[row][col].Type = ElementsType.Bonus;

                            break;

                    }
                }
            }
            //
        }

        public bool MovePlayer(MoveDirection moveDirection)
        {
            int newPlayerRow = player.Row;
            int newPlayerCol = player.Col;
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
                    new Elements() {Type = _level[player.Row][player.Col].Type, Row = player.Row, Col = player.Col},
                    new Elements() {Type = _level[newPlayerRow][newPlayerCol].Type, Row = newPlayerRow, Col = newPlayerCol}
                };

                _level[player.Row][player.Col].Type = _level[player.Row][player.Col].Type == ElementsType.PlayerOnGoal ? ElementsType.Goal
                                                                                                                                : ElementsType.Floor;

                if (_level[newPlayerRow][newPlayerCol].Type == ElementsType.Goal ||
                    _level[newPlayerRow][newPlayerCol].Type == ElementsType.BoxOnGoal)
                {
                    if (_level[newPlayerRow][newPlayerCol].Type == ElementsType.BoxOnGoal)
                    {
                        goalsFilled--;
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
                        goalsFilled++;

                        if (goalsFilled == goalsCount && LevelCompleted != null)
                        {
                            LevelCompleted(this, new EventArgs());
                        }
                    }
                    else
                    {
                        _level[newBoxRow][newBoxCol].Type = ElementsType.Box;
                    }
                }

                player = _level[newPlayerRow][newPlayerCol];
                hasMoved = true;
                //
            }
            return hasMoved;
        }
    }
}
