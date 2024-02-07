using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace MinesweeperWPF
{
    public partial class SubmitWindow : Window
    {
        private readonly string _filePath = System.IO.Path.Combine("..", "..", "Scores.txt"); // Relative file path

        private int _elapsedTime;
        private string _difficulty;
        public SubmitWindow(int elapsedTime, string difficulty)
        {
            _elapsedTime=elapsedTime;
            _difficulty=difficulty;

            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string playerName = NameTextBox.Text;

            WriteScoreToFile(playerName, _difficulty, _elapsedTime);
            Close();

            ScoresWindow scoresWindow = new ScoresWindow();
            scoresWindow.ShowDialog();
        }
        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void WriteScoreToFile(string playerName, string difficulty, int elapsedTime)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_filePath, true))
                {
                    writer.WriteLine($"{playerName},{difficulty},{elapsedTime}");
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show("Error writing score to file: " + ex.Message);
            }
        }

        public static Dictionary<string, List<ScoreEntry>> ReadScoresFromFile(string filePath)
        {
            Dictionary<string, List<ScoreEntry>> scores = new Dictionary<string, List<ScoreEntry>>();

            try
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(',');
                        string playerName = parts[0];
                        string difficulty = parts[1];
                        int elapsedTime = int.Parse(parts[2]);

                        if (!scores.ContainsKey(difficulty))
                        {
                            scores.Add(difficulty, new List<ScoreEntry>());
                        }

                        scores[difficulty].Add(new ScoreEntry(playerName, elapsedTime));
                    }

                    // Sort scores by elapsed time for each difficulty level
                    foreach (var kvp in scores)
                    {
                        kvp.Value.Sort((x, y) => x.ElapsedTime.CompareTo(y.ElapsedTime));
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show("Error reading scores from file: " + ex.Message);
            }

            return scores;
        }

        public class ScoreEntry
        {
            public string PlayerName { get; }
            public int ElapsedTime { get; }

            public ScoreEntry(string playerName, int elapsedTime)
            {
                PlayerName = playerName;
                ElapsedTime = elapsedTime;
            }
        }
    }
}

