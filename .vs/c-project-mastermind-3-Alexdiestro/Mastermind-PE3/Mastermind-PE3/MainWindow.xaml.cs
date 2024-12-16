using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Mastermind_PE3
{
    public partial class MainWindow : Window
    {
        private List<string> playerList = new List<string>();
        private string[] colors = { "Rood", "Geel", "Oranje", "Wit", "Groen", "Blauw" };
        private string[] secretCode;
        private int pogingen = 0;
        private int score = 100;
        private int currentPlayerIndex = 0;
        private int playerScore = 100;
        

        public MainWindow()
        {
            InitializeComponent();
            GenerateCode();
            FillComboBoxes();
            StartGame();
        }
        private void AddPlayer(string playerName)
        {
            playerList.Add(playerName);
        }
        private void StartGame()
        {
          
            string playerName = Microsoft.VisualBasic.Interaction.InputBox("Enter player name", "Player 1");
            AddPlayer(playerName);

            string addMorePlayers = Microsoft.VisualBasic.Interaction.InputBox("Do you want to add another player? (Yes/No)", "Add Player");
            while (addMorePlayers.ToLower() == "yes")
            {
                playerName = Microsoft.VisualBasic.Interaction.InputBox("Enter player name", "Next Player");
                AddPlayer(playerName);
                addMorePlayers = Microsoft.VisualBasic.Interaction.InputBox("Do you want to add another player? (Yes/No)", "Add Player");
            }
            DisplayCurrentPlayer();
        }
        private void DisplayCurrentPlayer()
        {
            string currentPlayer = playerList[currentPlayerIndex];
            FeedbackMessage.Text = $"It's {currentPlayer}'s turn!";
            ScoreLabel.Content = $"Score: {playerScore}";
        }

        private void GenerateCode()
        {
            Random random = new Random();
            secretCode = new string[4];
            for (int i = 0; i < 4; i++)
            {
                secretCode[i] = colors[random.Next(colors.Length)];
            }
            this.Title = $"MasterMind ({string.Join(",", secretCode)})";
        }

        private void FillComboBoxes()
        {
            foreach (var comboBox in new[] { Color1, Color2, Color3, Color4 })
            {
                comboBox.ItemsSource = colors;
            }
        }

        private void ValidateButton_Click(object sender, RoutedEventArgs e)
        {
            if (pogingen >= 10)
            {
                MessageText.Text = "You have reached the maximum attempts!";
                MessageBox.Show($"You've reached the maximum attempts. The correct code was: {string.Join(", ", secretCode)}", "Game Over");

                return;
            }
            var guess = new[] { Color1.Text, Color2.Text, Color3.Text, Color4.Text };
            if (guess.Any(string.IsNullOrEmpty))
            {
                MessageBox.Show("selecteer de color voordat validatie.");
                return;
            }

            pogingen++;
            AttemptsLabel.Content = $"Pogingen: {pogingen}";
            ValidateGuess(guess);

            if (guess.SequenceEqual(secretCode))
            {
                MessageText.Text = $"Winner! Code is gekraakt in {pogingen} pogingen!";
                MessageBox.Show($"Code cracked in {pogingen} attempts! Your final score: {score}", "Winner");

                return;
            }

            if (pogingen >= 10)
            {

                MessageBox.Show($"You failed! The correct code was: {string.Join(", ", secretCode)}", "Game Over");

            }
        }

        private void ValidateGuess(string[] guess)
        {
            var feedbackColors = new Border[] { FeedbackColor1, FeedbackColor2, FeedbackColor3, FeedbackColor4 };
            for (int i = 0; i < 4; i++)
            {
                if (guess[i] == secretCode[i])
                {
                    feedbackColors[i].Background = Brushes.Green;
                    UpdateBorderValidation(Result1, i, Brushes.Green);
                }
                else if (secretCode.Contains(guess[i]))
                {
                    feedbackColors[i].Background = Brushes.Orange;
                    UpdateBorderValidation(Result1, i, Brushes.Orange);
                    score -= 1;
                }
                else
                {
                    feedbackColors[i].Background = Brushes.Red;
                    UpdateBorderValidation(Result1, i, Brushes.Red);
                    score -= 2;
                }
            }
            ScoreLabel.Content = $"Score: {score}";
        }

        private void UpdateBorderValidation(Border border, int index, Brush color)
        {
            var borders = new[] { Result1, Result2, Result3, Result4 };
            borders[index].BorderBrush = color;
        }


        private void afsluit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("applicatie afsluiten?", "afsluiten", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void newspel_Click(object sender, RoutedEventArgs e)
        {
            
            pogingen = 0;
            score = 100;
            AttemptsLabel.Content = $"Pogingen: {pogingen}";
            ScoreLabel.Content = $"Score: {score}";
            MessageText.Text = string.Empty;
            GenerateCode();

            foreach (var border in new[] { Result1, Result2, Result3, Result4 })
            {
                border.BorderBrush = Brushes.Black;
                border.Background = Brushes.White;
            }
            foreach (var comboBox in new[] { Color1, Color2, Color3, Color4 })
            {
                comboBox.SelectedIndex = -1;
            }

            foreach (var feedback in new[] { FeedbackColor1, FeedbackColor2, FeedbackColor3, FeedbackColor4 })
            {
                feedback.Background = Brushes.White;
            }
        }
        private void scores_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"1. - {pogingen} - {score}/100 ", "Highscores");
            
        }
        private void Hint_Click(object sender, RoutedEventArgs e)
        {
            if (playerScore >= 25)
            {
                // Comprar pista
                MessageBox.Show("You bought a hint!");
                playerScore -= 25; // Penalización por pista
                ScoreLabel.Content = $"Score: {playerScore}";
            }
            else
            {
                MessageBox.Show("You don't have enough points to buy a hint.");
            }
        }
    }
}