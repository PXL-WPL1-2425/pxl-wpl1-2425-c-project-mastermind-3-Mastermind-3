using Microsoft.VisualBasic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Mastermind3
{
    public partial class MainWindow : Window
    {
        private int attempts = 0;
        private int maxAttempts = 10;
        private int totalPenaltyPoints = 0;
        private int parsedMaxAttempts = 20;

        private DispatcherTimer timer;
        private int counter = 0;

        private string[] names;
        private int[] scores;
        private string playerName;
        private string[] highScores = new string[15];
        private List<string> playerNames = new List<string>();
        private int currentPlayerIndex = 0;

        private string[] colors = { "rood", "geel", "oranje", "wit", "groen", "blauw" };
        private string[] colorCode = new string[4];
        private Random random = new Random();


        public MainWindow()
        {
            InitializeComponent();
            GenerateSecretCode();
            InitializeTimer();
        }

        private void AddHighScore(string playerName, int attempts, int score)
        {
            string newHighScore = $"{playerName} - {attempts} pogingen - {score}/100";

            for (int i = 0; i < highScores.Length; i++)
            {
                if (highScores[i] == null)
                {
                    highScores[i] = newHighScore;
                    break;
                }

                if (i == highScores.Length - 1)
                {
                    highScores[i] = newHighScore;
                }
            }

            UpdateHighScoresMenu();
        }

        private void UpdateHighScoresMenu()
        {
            highScoresMenu.Items.Clear();
            foreach (string highScore in highScores)
            {
                if (!string.IsNullOrEmpty(highScore))
                {
                    highScoresMenu.Items.Add(highScore);
                }
            }
        }

        private void GenerateSecretCode()
        {
            for (int i = 0; i < 4; i++)
            {
                colorCode[i] = colors[random.Next(colors.Length)];
            }
        }



        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }



        private void Timer_Tick(object sender, EventArgs e)
        {
            counter++;
            if (counter == 10)
            {
                attempts++;
                timer.Stop();
            }
            textBoxTime.Text = $"Timer: {counter} seconden";
        }


        private string GetComboBoxValue(ComboBox comboBox)
        {
            return (comboBox.SelectedItem as ComboBoxItem)?.Content.ToString().Trim().ToLower() ?? string.Empty;
        }


        private void BuyHint()
        {
            if (totalPenaltyPoints >= 0)
            {
                MessageBoxResult result = MessageBox.Show("Wil je een hint kopen? Je hebt de keuze tussen:\n" +
                                                          "1. Juiste kleur (15 strafpunten)\n" +
                                                          "2. Juiste kleur op de juiste plaats (25 strafpunten)\n" +
                                                          "Kies een optie of Annuleer.",
                                                          "Hint Kopen", MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Yes)
                {
                    MessageBoxResult hintChoice1 = MessageBox.Show("Wil je een juiste kleur of een juiste kleur op de juiste plaats?",
                                                                  "Maak je keuze.",
                                                                  MessageBoxButton.YesNoCancel);
                    MessageBoxResult hintChoice2 = MessageBox.Show("Wil je een juiste kleur op de juiste plaats?",
                                                                 "Maak je keuze.",
                                                                 MessageBoxButton.YesNoCancel);

                    if (hintChoice1 == MessageBoxResult.Yes)
                    {

                        totalPenaltyPoints += 15;
                        penaltyPointsLabel.Content = $"Strafpunten: {totalPenaltyPoints}";
                        ProvideColorHint();
                    }
                    else if (hintChoice2 == MessageBoxResult.Yes)
                    {

                        totalPenaltyPoints += 25;
                        penaltyPointsLabel.Content = $"Strafpunten: {totalPenaltyPoints}";
                        ProvideCorrectPositionHint();
                    }
                }
            }
        }

        private void ProvideColorHint()
        {

            List<string> availableColors = new List<string>(colors);
            string correctColor = colorCode[random.Next(colorCode.Length)];


            MessageBox.Show($"Een van de kleuren in de code is: {correctColor}",
                            "Juiste Kleur Hint");
        }

        private void ProvideCorrectPositionHint()
        {

            for (int i = 0; i < colorCode.Length; i++)
            {
                if (colorCode[i] == "rood")
                {
                    MessageBox.Show($"De juiste kleur op positie {i + 1} is: {colorCode[i]}",
                                    "Juiste Kleur op de Juiste Plaats Hint");
                    break;
                }
            }
        }

        private void CheckCode_Click(object sender, RoutedEventArgs e)
        {
            attempts++;
            counter = 0;
            timer.Start();
            attempsTextBox.Text = $"Attempts: {attempts}\t Actieve speler: {playerName}.";

            BuyHint();

            string[] userColors = new string[]
            {
         GetComboBoxValue(comboBox1),
         GetComboBoxValue(comboBox2),
         GetComboBoxValue(comboBox3),
         GetComboBoxValue(comboBox4)
            };

            StackPanel attemptPanel = CreateAttemptPanel(userColors);
            historyList.Items.Add(attemptPanel);

            if (colorCode.SequenceEqual(userColors))
            {
                int score = Math.Max(0, 100 - totalPenaltyPoints);
                AddHighScore(playerName, attempts, score);
                timer.Stop();
                MessageBox.Show($"Proficiat! Je hebt gewonnen in {attempts} pogingen.");
                SwitchToNextPlayer();
                return;
            }

            if (attempts >= maxAttempts)
            {
                int score = Math.Max(0, 100 - totalPenaltyPoints);
                AddHighScore(playerName, attempts, score);
                MessageBox.Show($"Je hebt verloren!\t De correcte kleuren waren: {string.Join(", ", colorCode)}.");
                reset();
                timer.Stop();
                SwitchToNextPlayer();
                return;
            }
        }


        private void SwitchToNextPlayer()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % playerNames.Count;
            playerName = playerNames[currentPlayerIndex];
            MessageBox.Show($"Nu is het de beurt van: {playerName}", "Volgende speler");
        }


        private StackPanel CreateAttemptPanel(string[] userColors)
        {
            StackPanel attemptPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
            Label[] attemptLabels = new Label[4];

            for (int i = 0; i < 4; i++)
            { 
                attemptLabels[i] = CreateAttemptLabel(GetColorBrush(userColors[i]));
                attemptPanel.Children.Add(attemptLabels[i]);
            }

            AddFeedbackAndPenalty(userColors, attemptPanel);
            return attemptPanel;


        }


        private Label CreateAttemptLabel(Brush background)
        {
            return new Label
            {
                Background = background,
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(2),
                Width = 25,
                Height = 25,
                Margin = new Thickness(2)
            };
        }


        private Brush GetColorBrush(string color)
        {
            return color.ToLower() switch
            {
                "wit" => new SolidColorBrush(Colors.White),
                "groen" => new SolidColorBrush(Colors.Green),
                "blauw" => new SolidColorBrush(Colors.Blue),
                "rood" => new SolidColorBrush(Colors.Red),
                "geel" => new SolidColorBrush(Colors.Yellow),
                "oranje" => new SolidColorBrush(Colors.Orange),
                _ => new SolidColorBrush(Colors.Transparent)
            };
        }


        private void AddFeedbackAndPenalty(string[] userColors, StackPanel attemptPanel)
        {
            totalPenaltyPoints = 0;
            for (int i = 0; i < colorCode.Length; i++)
            {
                string userColor = userColors[i];
                Label currentLabel = attemptPanel.Children[i] as Label;


                if (colorCode[i] == userColor)
                {
                    currentLabel.BorderBrush = new SolidColorBrush(Colors.Green);
                    currentLabel.BorderThickness = new Thickness(3);
                }

                else if (colorCode.Contains(userColor))
                {
                    currentLabel.BorderBrush = new SolidColorBrush(Colors.Wheat);
                    currentLabel.BorderThickness = new Thickness(3);
                }
                else
                {
                    currentLabel.BorderBrush = new SolidColorBrush(Colors.Red);
                    currentLabel.BorderThickness = new Thickness(3);
                }


                if (colorCode[i] == userColor)
                {
                    totalPenaltyPoints += 0;
                }
                else if (colorCode.Contains(userColor))
                {
                    totalPenaltyPoints += 1;
                }
                else
                {
                    totalPenaltyPoints += 2;
                }
            }
            penaltyPointsLabel.Content = $"Strafpunten: {totalPenaltyPoints}";
        }



        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.F12)
            {
                colorsTextBox.Visibility = Visibility.Visible;
                colorsTextBox.Text = string.Join(", ", colorCode);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bent u zeker dat u de applicatie wilt afsluiten?",
                                                       "Bevestigen",
                                                       MessageBoxButton.YesNo);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool morePlayers = true;

            while (morePlayers)
            {
                string name = Interaction.InputBox("Voer de naam van de speler in:");
                while (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("Voer een geldige naam in!", "Foutieve invoer");
                    name = Interaction.InputBox("Voer de naam van de speler in:");
                }

                playerNames.Add(name);

                MessageBoxResult result = MessageBox.Show("Wil je nog een speler toevoegen?", "Nog een speler?", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    morePlayers = false;
                }
            }

            if (playerNames.Count > 0)
            {
                playerName = playerNames[0];
            }
            else
            {
                MessageBox.Show("Er moeten minstens één speler worden toegevoegd. Het spel wordt afgesloten.", "Fout");
                Close();
            }
        }

        private void closeApp_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AttemptNumber_Click(object sender, RoutedEventArgs e)
        {
            string userInput = Interaction.InputBox("Voer het maximale aantal pogingen in: ");


            if (!int.TryParse(userInput, out parsedMaxAttempts) || parsedMaxAttempts > 3 || parsedMaxAttempts < 20)
            {
                maxAttempts = parsedMaxAttempts;
            }
            else
            {
                MessageBox.Show("Voer een geldig positief nummer in!", "Foutieve invoer");
            }
        }
        private void reset()
        {
            attempts = 0;
            counter = 0;
            totalPenaltyPoints = 0;
            historyList.Items.Clear();
            penaltyPointsLabel.Content = "Strafpunten: 0";
            attempsTextBox.Text = "Attempts: 0";
            textBoxTime.Text = "Timer: 0 seconden";
            timer.Stop();
            GenerateSecretCode();
        }
        private void newGame_Click(object sender, RoutedEventArgs e)
        {
            reset();
        }

        private void Tooltip_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("witte rand:Juiste kleur foute positie,\rrode rand: Juiste kleur, juiste positie, \rgeen kleur: Foute kleur");
        }
    }
}