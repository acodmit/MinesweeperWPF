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
    public partial class ScoresWindow : Window
    {
        private readonly Dictionary<string, List<SubmitWindow.ScoreEntry>> _scores;
        private readonly String filePath = System.IO.Path.Combine("..", "..", "Scores.txt"); // Relative file path

        public ScoresWindow()
        {
            InitializeComponent();
            _scores = LoadScores(filePath);
            DisplayScores();
        }

        private Dictionary<string, List<SubmitWindow.ScoreEntry>> LoadScores(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new Dictionary<string, List<SubmitWindow.ScoreEntry>>();
            }

            string[] lines = File.ReadAllLines(filePath);
            var loadedScores = new Dictionary<string, List<SubmitWindow.ScoreEntry>>();

            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                string playerName = parts[0];
                string difficulty = parts[1];
                int elapsedTime = int.Parse(parts[2]);

                if (!loadedScores.ContainsKey(difficulty))
                {
                    loadedScores[difficulty] = new List<SubmitWindow.ScoreEntry>();
                }

                loadedScores[difficulty].Add(new SubmitWindow.ScoreEntry(playerName, elapsedTime));
            }

            return loadedScores;
        }


        private void DisplayScores()
        {
            Dictionary<string, List<SubmitWindow.ScoreEntry>> scoresByDifficulty = new Dictionary<string, List<SubmitWindow.ScoreEntry>>();

            // Group scores by difficulty level
            foreach (var kvp in _scores)
            {
                string difficultyLevel = kvp.Key;
                List<SubmitWindow.ScoreEntry> scores = kvp.Value;

                if (!scoresByDifficulty.ContainsKey(difficultyLevel))
                {
                    scoresByDifficulty[difficultyLevel] = new List<SubmitWindow.ScoreEntry>();
                }

                scoresByDifficulty[difficultyLevel].AddRange(scores);
            }

            // Display scores for each difficulty level
            foreach (var kvp in scoresByDifficulty)
            {
                string difficultyLevel = kvp.Key;
                List<SubmitWindow.ScoreEntry> scores = kvp.Value;

                ScoreListBox.Items.Add($"Difficulty level: {difficultyLevel}");

                // Sort scores by elapsed time in ascending order
                scores.Sort((s1, s2) => s1.ElapsedTime.CompareTo(s2.ElapsedTime));

                int position = 1;
                foreach (var score in scores)
                {
                    ScoreListBox.Items.Add($"{position}. {score.PlayerName} - {score.ElapsedTime} seconds");
                    position++;
                }

                ScoreListBox.Items.Add(""); // Add empty line for separation
            }
        }


    }
}


