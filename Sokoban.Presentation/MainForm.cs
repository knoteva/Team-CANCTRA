using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sokoban.Logic;
using Sokoban.Presentation.Helpers;

namespace Sokoban.Presentation
{
    public partial class MainForm : Form
    {
        #region Variables
        public Soko Model { get; set; }

        private int _paddingX;
        private int _paddingY;

        private readonly int _cellSize = Properties.Resources.Wall.Width;
        // private int _currentLevel;

        private readonly int _defaultFormWidth;
        private readonly int _defaultFormHeight;

        private readonly int _defaultBackgroundPanelWidth;
        private readonly int _defaultBackgroundPanelHeight;

        #endregion Variables

        public MainForm()
        {
            InitializeComponent();

            _defaultFormWidth = Width;
            _defaultFormHeight = Height;

            _defaultBackgroundPanelWidth = backgroundPanel.Width;
            _defaultBackgroundPanelHeight = backgroundPanel.Height;

            this.undoButton.Click += undoButton_Click;
            this.restartButton.Click += restartButton_Click;
            this.newGameMenuItem.Click += newGameMenuItem_Click;
            this.exitMenuItem.Click += exitMenuItem_Click;
            this.drawingArea.Paint += drawingArea_Paint;
            this.KeyDown += MainForm_KeyDown;
            this.Load += MainForm_Load;
        }

        #region Events

        void MainForm_Load(object sender, EventArgs e)
        {
            this.Model = new Soko();
            this.Model.LevelCompleted += Model_LevelCompleted;
        }

        void Model_LevelCompleted(object sender, EventArgs e)
        {
            //_isLevelComplete = true;
            // undoMenuItem.Enabled = false;
            undoButton.Enabled = false;
            //_currentLevel++;
            this.Model.NextLevel();
            if (this.Model.IsLastLevel)
            {
                statusLabel.Text = "Level Completed! No more levels in the collection.";
            }
            else
            {
                statusLabel.Text = "Level Completed! Press enter to go to the next level...";
            }

            drawingArea.Invalidate();
        }

        void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.Model.SelectedCollection == null)
            {
                return;
            }

            if (this.Model.IsLevelCompleted)
            {
                if (!this.Model.IsLastLevel && e.KeyCode == Keys.Enter)
                {
                    GoToLevel(this.Model.CurrentLevel);
                }
            }
            else
            {
                bool hasMoved = false;

                switch (e.KeyCode)
                {
                    case Keys.Up:
                        hasMoved = this.Model.MovePlayer(Soko.MoveDirection.Up);
                        break;
                    case Keys.Down:
                        hasMoved = this.Model.MovePlayer(Soko.MoveDirection.Down);
                        break;
                    case Keys.Right:
                        hasMoved = this.Model.MovePlayer(Soko.MoveDirection.Right);
                        break;
                    case Keys.Left:
                        hasMoved = this.Model.MovePlayer(Soko.MoveDirection.Left);
                        break;
                    case Keys.Space:
                        // for testing...
                        //this.Model.IsLevelCompleted = true;
                        //this.Model._goalsFilled = 10;
                        // GoToLevel(this.Model.CurrentLevel+1);
                        break;
                    case Keys.Back:
                        //UndoMovement();
                        break;
                }

                if (hasMoved)
                {
                    // undoMenuItem.Enabled = true;
                    undoButton.Enabled = true;
                }

                drawingArea.Invalidate();
            }
        }

        void drawingArea_Paint(object sender, PaintEventArgs e)
        {
            if (this.Model.Collections != null)
            {
                foreach (Element element in this.Model)
                {
                    Bitmap imageToDraw = null;

                    switch (element.Type)
                    {
                        case ElementType.Wall:
                            imageToDraw = Properties.Resources.Wall;
                            break;
                        case ElementType.Box:
                        case ElementType.BoxOnGoal:
                            imageToDraw = Properties.Resources.Box;
                            break;
                        case ElementType.Goal:
                            imageToDraw = Properties.Resources.Goal;
                            break;
                        case ElementType.Player:
                        case ElementType.PlayerOnGoal:
                            imageToDraw = Properties.Resources.Player;
                            break;
                        case ElementType.BonusTime:
                            imageToDraw = Properties.Resources.Time;
                            break;
                        case ElementType.BonusPoints:
                            imageToDraw = Properties.Resources.Points;
                            break;
                    }

                    if (imageToDraw != null)
                    {
                        e.Graphics.DrawImage(imageToDraw, element.Column * _cellSize, element.Row * _cellSize, _cellSize,
                                             _cellSize);
                    }
                }
            }
        }

        void topRankingMenuItem_Click(object sender, EventArgs e)
        {
            TopPlayersForm tops = new TopPlayersForm();
            if (tops.ShowDialog() == DialogResult.Cancel)
            {

            }
        }

        void exitMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Model.IsPlaying)
            {
                if (DialogResult.Yes ==
                    MessageBox.Show("Сигурни ли сте, че искате да затворите играта?", "Внимание",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }

        void newGameMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Model.IsPlaying)
            {
                if (DialogResult.Yes ==
                    MessageBox.Show("Сигурни ли сте, че искате да започнете нова игра?", "Внимание",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    StartGame();
                }
            }
            else
            {
                StartGame();
            }

        }

        void restartButton_Click(object sender, EventArgs e)
        {
            RestartLevel();
        }

        void undoButton_Click(object sender, EventArgs e)
        {

        }

        #endregion Events

        #region Methods

        private void StartGame()
        {
            var game = SelectGameTypeForm.ShowForm();
            if (GameType.Standart == game)
            {
                this.Model.StartNewGame(game);
                GoToLevel(this.Model.CurrentLevel);
            }
            else if (GameType.Practice == game)
            {
                this.Model.SetCurrentLevel(1);

                if (DialogResult.OK == LevelSelectionForm.ShowForm(this.Model))
                {
                    this.Model.StartPractice();
                    GoToLevel(this.Model.CurrentLevel);
                }
            }
        }

        private void GoToLevel(int levelNumber)
        {
            this.Model.LoadLevel(levelNumber);

            drawingArea.Width = this.Model.GetSelectedCollectionWidth() * _cellSize;
            drawingArea.Height = this.Model.GetSelectedCollectionHeight() * _cellSize;

            // some code to resize the form to fit the level size, and also to center the level in the form
            int formNewWidth = drawingArea.Width > _defaultBackgroundPanelWidth
                                   ? _defaultFormWidth + drawingArea.Width - _defaultBackgroundPanelWidth + 10
                                   : _defaultFormWidth;
            int formNewHeight = drawingArea.Height > _defaultBackgroundPanelHeight
                                    ? _defaultFormHeight + drawingArea.Height - _defaultBackgroundPanelHeight + 10
                                    : _defaultFormHeight;

            Size = new Size(formNewWidth, formNewHeight);
            //CenterToScreen();

            int x = backgroundPanel.Size.Width / 2 - drawingArea.Size.Width / 2;
            int y = backgroundPanel.Size.Height / 2 - drawingArea.Size.Height / 2;
            drawingArea.Location = new Point(x, y);

            this.Model.IsLevelCompleted = false;

            statusLabel.Text = "Playing";
            //levelCollectionLabel.Text = _levelCollection.Title;
            levelLabel.Text = string.Format("{0} of {1}", this.Model.CurrentLevel, this.Model.SelectedCollection.NumberOfLevels);


            restartButton.Enabled = true;
            undoButton.Enabled = false;

            drawingArea.Invalidate();
            drawingArea.Visible = true;
        }

        private void RestartLevel()
        {
            GoToLevel(this.Model.CurrentLevel);
        }

        #endregion Methods
    }
}
