using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace MinesweeperWPF.Core
{
    internal class Game : INotifyPropertyChanged
    {
        public Field[,] GameBoard;

        private int _boardWidth;
        private int _boardHeight;

        private int _totalMines;

        private int _elapsedTime;
        public int ElapsedTime
        {
            get { return _elapsedTime; }
            set
            {
                if (_elapsedTime != value)
                {
                    _elapsedTime = value;
                    OnPropertyChanged(nameof(ElapsedTime));
                    OnPropertyChanged(nameof(TimeLabelContent));
                }
            }
        }
        public string TimeLabelContent
        {
            get { return $"Time: {_elapsedTime / 60:D2}:{_elapsedTime % 60:D2}"; }
        }

        private int _remainingCoveredFields;
        private bool _isSoundEnabled;
        public bool Sound
        {
            get { return _isSoundEnabled; }
            set
            {
                _isSoundEnabled = value;
                OnPropertyChanged(nameof(Sound));

                if (CurrentState == GameState.GameInProgress)
                {
                    if (Sound)
                    {
                        if (!_clockSoundPlayer.HasAudio)
                        {
                            string relativePath = Path.Combine("..", "..", "Sounds", "ticking-clock.mp3");
                            string fullPath = Path.Combine(Environment.CurrentDirectory, relativePath);

                            if (File.Exists(fullPath))
                            {
                                _clockSoundPlayer.Open(new Uri(fullPath));
                            }
                        }
                        _clockSoundPlayer.Play();
                    }
                    else
                    {
                        _clockSoundPlayer.Stop();
                    }
                }
            }
        }

        private DispatcherTimer _gameTimer;

        private GameState _currentState;
        public GameState CurrentState
        {
            get { return _currentState; }
            set
            {
                _currentState = value;
                OnPropertyChanged("CurrentState");
            }
        }

        public static DifficultyLevel Difficulty { get; set; }

        private MediaPlayer _clockSoundPlayer = new MediaPlayer();
        private MediaPlayer _mineExplosionPlayer = new MediaPlayer();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Game(int boardWidth, int boardHeight, int totalMines, bool isSoundEnabled)
        {
            _boardWidth = boardWidth;
            _boardHeight = boardHeight;
            _totalMines = totalMines;
            Sound = isSoundEnabled;

            InitializeGame();
        }

        private void InitializeGame()
        {
            _elapsedTime = 0;
            _remainingCoveredFields = _boardWidth * _boardHeight;

            CurrentState = GameState.ReadyToStart;

            _gameTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            _gameTimer.Tick += Timer_Tick;

            _clockSoundPlayer.MediaEnded += ClockSoundPlayer_MediaEnded;
            _mineExplosionPlayer.Volume = 0.1;

            CreateGameBoard();
        }

        private void CreateGameBoard()
        {
            GameBoard = new Field[_boardHeight, _boardWidth];
            Random localRandom = new Random();

            for (int h = 0; h < _boardHeight; h++)
            {
                for (int w = 0; w < _boardWidth; w++)
                {
                    GameBoard[h, w] = new Field();
                }
            }

            int mineCounter = 0;

            do
            {
                int h = localRandom.Next(0, _boardHeight);
                int w = localRandom.Next(0, _boardWidth);

                if (!GameBoard[h, w].HasMine)
                {
                    GameBoard[h, w].HasMine = true;
                    mineCounter++;
                }
            } while (mineCounter != _totalMines);
        }

        public void FlagField(int row, int col)
        {
            if (GameBoard[row, col].IsCovered && (CurrentState == GameState.GameInProgress || CurrentState == GameState.ReadyToStart))
            {
                GameBoard[row, col].ToggleFlag();
            }
        }

        public void CheckField(int row, int col)
        {
            if (CurrentState == GameState.ReadyToStart)
            {
                StartGame();
                if (Sound)
                {
                    PlayClockSound();
                }
            }

            if (CurrentState == GameState.GameInProgress && !GameBoard[row, col].HasFlag)
            {
                DiscoverField(row, col);

                if (_remainingCoveredFields == _totalMines)
                {
                    EndGame(GameState.Won);
                }
            }
        }
        public void DiscoverField(int row, int col)
        {
            if (GameBoard[row, col].HasMine)
            {
                _gameTimer.Stop();
                if (Sound)
                {
                    PlayExplosionSound();
                }

                StopClockSound();
                CurrentState = GameState.Lost;

                return;
            }

            if (GameBoard[row, col].IsCovered)
            {
                GameBoard[row, col].IsCovered = false;
                _remainingCoveredFields--;

                if (GameBoard[row, col].HasFlag)
                {
                    GameBoard[row, col].ToggleFlag();
                }

                int mineCounter = 0;

                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if ((row + i >= 0) && (row + i < _boardHeight) && (col + j >= 0) && (col + j < _boardWidth))
                        {
                            if (GameBoard[row + i, col + j].HasMine)
                            {
                                mineCounter++;
                            }
                        }
                    }
                }

                if (mineCounter == 0)
                {
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if ((row + i >= 0) && (row + i < _boardHeight) && (col + j >= 0) && (col + j < _boardWidth))
                            {
                                if (!GameBoard[row + i, col + j].HasMine) 
                                    DiscoverField(row + i, col + j);
                            }
                        }
                    }
                }
                GameBoard[row, col].MinesAround = mineCounter;
            }
        }

        private void StartGame()
        {
            _gameTimer.Start();
            CurrentState = GameState.GameInProgress;
        }
        public void EndGame()
        {
            _gameTimer.Stop();
            StopClockSound();
        }
        
        public void EndGame(GameState state)
        {
            _gameTimer.Stop();
            StopClockSound();
            CurrentState = state;
        }

        private void PlayClockSound()
        {
            string relativePath = Path.Combine("..", "..", "Sounds", "ticking-clock.mp3");
            string fullPath = Path.Combine(Environment.CurrentDirectory, relativePath);

            if (File.Exists(fullPath))
            {
                _clockSoundPlayer.Open(new Uri(fullPath));
                _clockSoundPlayer.Play();
            }
        }
        private void PlayExplosionSound ()
        {
            string relativePath = Path.Combine("..", "..", "Sounds", "explosion.mp3");
            string fullPath = Path.Combine(Environment.CurrentDirectory, relativePath);

            if (File.Exists(fullPath))
            {
                _mineExplosionPlayer.Open(new Uri(fullPath));
                _mineExplosionPlayer.Play();
            }
        }
        private void StopClockSound()
        {
            _clockSoundPlayer.Stop();
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            ElapsedTime++;
        }

        private void ClockSoundPlayer_MediaEnded(object sender, EventArgs e)
        {
            PlayClockSound();
        }

        public enum GameState
        {
            ReadyToStart,
            GameInProgress,
            Won,
            Lost,
        }
        public enum DifficultyLevel
        {
            Easy,
            Medium,
            Hard
        }
    }
}
