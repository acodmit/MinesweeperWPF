using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MinesweeperWPF.Core;
using MinesweeperWPF.Controls;
using System.IO;

namespace MinesweeperWPF
{
    public partial class MainWindow : Window
    {
        private const int DefaultBombs = 10;
        private const int DefaultHeight = 9;
        private const int DefaultWidth = 9;

        private Game _game;
        private MinesweeperButton[,] _btnBoard;
        private RadioButton _selectedRadioButton;
        public MainWindow()
        {
            InitializeComponent();

            SoundMenuItem.IsChecked = true;
            _selectedRadioButton = RadioBtnEasy;

            StartGame();
        }

        private void StartGame()
        {
            InitializeGame(DefaultHeight, DefaultWidth, DefaultBombs);
            RefreshBoard();
        }

        private void InitializeGame(int height, int width, int bombs)
        {
            GenerateMinesweeperGrid(height, width);
            if (_game != null)
                _game.EndGame();
            _game = new Game(width, height, bombs, SoundMenuItem.IsChecked);
            DataContext = _game;
            _game.PropertyChanged += GameStateChanged;
        }

        private void GenerateMinesweeperGrid(int height, int width)
        {
            MinesweeperGrid.Children.Clear();
            MinesweeperGrid.ColumnDefinitions.Clear();
            MinesweeperGrid.RowDefinitions.Clear();

            _btnBoard = new MinesweeperButton[height, width];

            for (int i = 0; i < width; i++)
                MinesweeperGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int j = 0; j < height; j++)
                MinesweeperGrid.RowDefinitions.Add(new RowDefinition());

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    MinesweeperButton btn = new MinesweeperButton( i, j);
                    Grid.SetColumn(btn, j);
                    Grid.SetRow(btn, i);

                    AttachEventsButton(btn);
                    MinesweeperGrid.Children.Add(btn);
                    _btnBoard[i, j]= btn;
                }
            }
        }

        private void AttachEventsButton(MinesweeperButton btn)
        {
            btn.MouseRightButtonDown += Button_MouseRightDown;
            btn.PreviewMouseLeftButtonDown += Button_MouseLeftDown;
        }

        private void Button_NewGame_Click(object sender, RoutedEventArgs e)
        {
            if(_game != null)
                _game.EndGame();
            HandleNewGame();
        }

        private void Button_MouseRightDown(object sender, MouseButtonEventArgs e)
        {
            MinesweeperButton button = (MinesweeperButton)sender;
            _game.FlagField(button.Row, button.Column);
            RefreshBoard();
            e.Handled = true;
        }

        private void Button_MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            MinesweeperButton button = (MinesweeperButton)sender;
            _game.CheckField(button.Row, button.Column);
            RefreshBoard();
            e.Handled = true;
        }

        private void MenuItem_Level_Checked(object sender, RoutedEventArgs e)
        {
            if (_game != null)
            {
                RadioButton radioButton = (RadioButton)sender;
                _selectedRadioButton = radioButton;

                MessageBoxResult result = MessageBoxResult.OK;

                if (_game.CurrentState == Game.GameState.GameInProgress)
                {
                    result = MessageBox.Show("Do you want to cancel the current game?", "", MessageBoxButton.OKCancel);
                }

                if (result == MessageBoxResult.OK)
                {
                    HandleNewGame();
                }
                else
                {
                    RestorePreviousDifficultySetting(Game.Difficulty);
                }
            }
        }

        private void HandleNewGame()
        {
            switch (_selectedRadioButton.Content)
            {
                case "Easy":
                    Game.Difficulty = Game.DifficultyLevel.Easy;
                    InitializeGame(9, 9, 10);
                    break;
                case "Medium":
                    Game.Difficulty = Game.DifficultyLevel.Medium;
                    InitializeGame(9, 15, 20);
                    break;
                case "Hard":
                    Game.Difficulty = Game.DifficultyLevel.Hard;
                    InitializeGame(9, 20, 30);
                    break;
            }
        }

        private void RestorePreviousDifficultySetting(Game.DifficultyLevel level)
        {
            switch (level)
            {
                case Game.DifficultyLevel.Easy:
                    RadioBtnEasy.IsChecked = true;
                    break;
                case Game.DifficultyLevel.Medium:
                    RadioBtnMedium.IsChecked = true;
                    break;
                case Game.DifficultyLevel.Hard:
                    RadioBtnHard.IsChecked = true;
                    break;
            }
        }

        private void MenuItem_Scores_Click(object sender, RoutedEventArgs e)
        {
            ScoresWindow scoresWindow = new ScoresWindow();
            scoresWindow.ShowDialog();
        }

        private void GameStateChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentState")
            {
                RefreshBoard();
                switch (_game.CurrentState)
                {
                    case Game.GameState.Lost:
                        ShowMinesOnBoard();
                        MessageBox.Show("Game Over!!!");
                        break;
                    case Game.GameState.Won:
                        SubmitWindow submitWindow = new SubmitWindow(_game.ElapsedTime, Game.Difficulty.ToString());
                        submitWindow.ShowDialog();
                        break;
                }
            }
        }


        private void RefreshBoard()
        {
            foreach (MinesweeperButton btn in MinesweeperGrid.Children)
            {
                int row = Grid.GetRow(btn);
                int col = Grid.GetColumn(btn);
                btn.RefreshButton(_game.GameBoard[row, col]);
            }
        }

        private void ShowMinesOnBoard()
        {
            foreach (MinesweeperButton btn in MinesweeperGrid.Children)
            {
                int row = Grid.GetRow(btn);
                int col = Grid.GetColumn(btn);
                btn.ShowMine(_game.GameBoard[row, col]);
            }
     
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
