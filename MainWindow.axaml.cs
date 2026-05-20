using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Layout;

namespace BitLifeClone
{
    public partial class MainWindow : Window
    {
        private GameEngine _engine;
        private List<Job> _availableJobs;
        private Job? _selectedJob;
        private NPC? _selectedNPC;

        public MainWindow()
        {
            InitializeComponent();

            _engine = new GameEngine();
            _engine.OnLog = Log;
            _engine.OnEventTriggered = ShowEventPopup;

            _availableJobs = new List<Job>
            {
                new Job("Fast Food Worker", 15000, "None"),
                new Job("Plumber", 45000, "High School"),
                new Job("Software Engineer", 80000, "University"),
                new Job("Doctor", 150000, "University")
            };

            JobsList.ItemsSource = _availableJobs;
            JobsList.DisplayMemberBinding = new Avalonia.Data.Binding("Title");
        }

        private void StartButton_Click(object? sender, RoutedEventArgs e)
        {
            string name = string.IsNullOrWhiteSpace(NameTextBox.Text) ? "Unknown" : NameTextBox.Text;
            string gender = GenderComboBox.SelectedIndex == 0 ? "Male" : "Female";

            ActivityLog.Items.Clear();
            _engine.StartGame(name, gender);

            SetupPanel.IsVisible = false;
            GamePanel.IsVisible = true;

            UpdateUI();
            EnableActionButtons(true);
            RestartButton.IsVisible = false;
        }

        private void UpdateUI()
        {
            if (_engine.Player == null) return;

            NameGenderAgeBlock.Text = $"Name: {_engine.Player.Name} | Gender: {_engine.Player.Gender} | Age: {_engine.Player.Age}";
            HealthBlock.Text = $"{_engine.Player.Health}%";
            HealthBar.Value = _engine.Player.Health;
            HappinessBlock.Text = $"{_engine.Player.Happiness}%";
            HappinessBar.Value = _engine.Player.Happiness;
            SmartsBlock.Text = $"{_engine.Player.Smarts:0.##}%";
            SmartsBar.Value = _engine.Player.Smarts;
            LooksBlock.Text = $"{_engine.Player.Looks}%";
            LooksBar.Value = _engine.Player.Looks;
            MoneyBlock.Text = $"${_engine.Player.Money}";
            EducationBlock.Text = _engine.Player.CurrentEducation;
            JobBlock.Text = _engine.Player.CurrentJob?.Title ?? "Unemployed";

            RelationshipsList.ItemsSource = null;
            RelationshipsList.ItemsSource = _engine.Player.Relationships;
        }

        private void Log(string message)
        {
            ActivityLog.Items.Add(message);
            ActivityLog.ScrollIntoView(ActivityLog.Items.Count - 1);
        }

        private void EnableActionButtons(bool enable)
        {
            AgeUpButton.IsEnabled = enable;
            GymButton.IsEnabled = enable;
            BookButton.IsEnabled = enable;
            WorkButton.IsEnabled = enable;

            EnrollElementaryButton.IsEnabled = enable;
            EnrollHighSchoolButton.IsEnabled = enable;
            EnrollUniversityButton.IsEnabled = enable;
            StudyButton.IsEnabled = enable;
            GraduateButton.IsEnabled = enable;

            ApplyJobButton.IsEnabled = enable;
            SpendTimeButton.IsEnabled = enable;
            ArgueButton.IsEnabled = enable;

            ShowActivitiesButton.IsEnabled = enable;
            ShowEducationButton.IsEnabled = enable;
            ShowRelationshipsButton.IsEnabled = enable;
        }

        private void CheckDeath()
        {
            if (_engine.CheckDeath())
            {
                EnableActionButtons(false);
                RestartButton.IsVisible = true;
            }
        }

        private void AgeUpButton_Click(object? sender, RoutedEventArgs e)
        {
            _engine.AgeUp();
            UpdateUI();
            CheckDeath();
        }

        private void GymButton_Click(object? sender, RoutedEventArgs e)
        {
            _engine.GoToGym();
            UpdateUI();
        }

        private void BookButton_Click(object? sender, RoutedEventArgs e)
        {
            _engine.ReadBook();
            UpdateUI();
        }

        private void DietButton_Click(object? sender, RoutedEventArgs e)
        {
            _engine.Diet();
            UpdateUI();
        }

        private void GardenButton_Click(object? sender, RoutedEventArgs e)
        {
            _engine.Garden();
            UpdateUI();
        }

        private void LibraryButton_Click(object? sender, RoutedEventArgs e)
        {
            _engine.VisitLibrary();
            UpdateUI();
        }

        private void MeditateButton_Click(object? sender, RoutedEventArgs e)
        {
            _engine.Meditate();
            UpdateUI();
        }

        private void WalkButton_Click(object? sender, RoutedEventArgs e)
        {
            _engine.GoForWalk();
            UpdateUI();
        }

        private int _iqTestQuestionIndex = 0;
        private double _iqTestSmartsGained = 0;

        private void IQTestButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_engine.Player == null || _engine.Player.Health <= 0) return;
            if (!_engine.CanDoActivity("IQ Test")) return;

            _iqTestQuestionIndex = 0;
            _iqTestSmartsGained = 0;
            IQTestOverlay.IsVisible = true;
            ShowNextIQTestQuestion();
        }

        private void ShowNextIQTestQuestion()
        {
            if (_iqTestQuestionIndex >= 20)
            {
                IQTestOverlay.IsVisible = false;
                _engine.Log($"You completed the IQ Test! You gained {_iqTestSmartsGained:0.##}% Smarts.");
                UpdateUI();
                return;
            }

            Random rand = new Random();
            int diffLevel = _iqTestQuestionIndex / 4; // 0: very easy, 1: easy, 2: medium, 3: hard, 4: very hard

            int a = 0, b = 0;
            string op = "+";
            int correctAnswer = 0;

            if (diffLevel == 0)
            {
                a = rand.Next(1, 10);
                b = rand.Next(1, 10);
                op = "+";
                correctAnswer = a + b;
            }
            else if (diffLevel == 1)
            {
                a = rand.Next(10, 50);
                b = rand.Next(1, 20);
                op = "-";
                correctAnswer = a - b;
            }
            else if (diffLevel == 2)
            {
                a = rand.Next(2, 12);
                b = rand.Next(2, 12);
                op = "*";
                correctAnswer = a * b;
            }
            else if (diffLevel == 3)
            {
                b = rand.Next(2, 10);
                correctAnswer = rand.Next(2, 20);
                a = b * correctAnswer;
                op = "/";
            }
            else
            {
                int rOp = rand.Next(0, 4);
                if (rOp == 0) { a = rand.Next(50, 200); b = rand.Next(50, 200); op = "+"; correctAnswer = a + b; }
                else if (rOp == 1) { a = rand.Next(50, 200); b = rand.Next(10, 100); op = "-"; correctAnswer = a - b; }
                else if (rOp == 2) { a = rand.Next(10, 30); b = rand.Next(5, 20); op = "*"; correctAnswer = a * b; }
                else { b = rand.Next(5, 20); correctAnswer = rand.Next(5, 30); a = b * correctAnswer; op = "/"; }
            }

            IQTestQuestionBlock.Text = $"Question {_iqTestQuestionIndex + 1}: What is {a} {op} {b}?";

            List<int> choices = new List<int> { correctAnswer };
            while (choices.Count < 4)
            {
                int wrong = correctAnswer + rand.Next(-10, 11);
                if (wrong != correctAnswer && !choices.Contains(wrong))
                {
                    choices.Add(wrong);
                }
            }

            // Shuffle choices
            for (int i = 0; i < choices.Count; i++)
            {
                int temp = choices[i];
                int randomIndex = rand.Next(i, choices.Count);
                choices[i] = choices[randomIndex];
                choices[randomIndex] = temp;
            }

            IQTestChoicesPanel.Children.Clear();
            foreach (var choice in choices)
            {
                var btn = new Button
                {
                    Content = choice.ToString(),
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center
                };
                int selectedAnswer = choice;
                int correctAns = correctAnswer;
                btn.Click += (s, e) =>
                {
                    if (selectedAnswer == correctAns)
                    {
                        if (_iqTestSmartsGained < 5.0)
                        {
                            _iqTestSmartsGained += 0.25;
                            if (_engine.Player != null)
                            {
                                _engine.Player.Smarts += 0.25;
                                _engine.ClampStats();
                            }
                        }
                        _iqTestQuestionIndex++;
                        ShowNextIQTestQuestion();
                    }
                    else
                    {
                        IQTestOverlay.IsVisible = false;
                        _engine.Log($"You answered incorrectly on question {_iqTestQuestionIndex + 1}. You gained {_iqTestSmartsGained:0.##}% Smarts from the test.");
                        UpdateUI();
                    }
                };
                IQTestChoicesPanel.Children.Add(btn);
            }
        }

        // --- Education & Career ---

        private void EnrollElementary_Click(object? sender, RoutedEventArgs e)
        {
            _engine.EnrollEducation("Elementary");
            UpdateUI();
        }

        private void EnrollHighSchool_Click(object? sender, RoutedEventArgs e)
        {
            _engine.EnrollEducation("High School");
            UpdateUI();
        }

        private void EnrollUniversity_Click(object? sender, RoutedEventArgs e)
        {
            _engine.EnrollEducation("University");
            UpdateUI();
        }

        private void Study_Click(object? sender, RoutedEventArgs e)
        {
            _engine.Study();
            UpdateUI();
        }

        private void Graduate_Click(object? sender, RoutedEventArgs e)
        {
            _engine.Graduate();
            UpdateUI();
        }

        private void JobsList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (JobsList.SelectedItem is Job job)
            {
                _selectedJob = job;
            }
        }

        private void ApplyJob_Click(object? sender, RoutedEventArgs e)
        {
            if (_selectedJob != null)
            {
                _engine.ApplyForJob(_selectedJob);
                UpdateUI();
            }
        }

        private void WorkButton_Click(object? sender, RoutedEventArgs e)
        {
            _engine.Work();
            UpdateUI();
        }

        // --- Relationships ---

        private void RelationshipsList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (RelationshipsList.SelectedItem is NPC npc)
            {
                _selectedNPC = npc;
            }
        }

        private void SpendTime_Click(object? sender, RoutedEventArgs e)
        {
            if (_selectedNPC != null)
            {
                _engine.SpendTimeWith(_selectedNPC);
                UpdateUI();
            }
        }

        private void Argue_Click(object? sender, RoutedEventArgs e)
        {
            if (_selectedNPC != null)
            {
                _engine.ArgueWith(_selectedNPC);
                UpdateUI();
            }
        }

        // --- Navigation ---

        private void ShowActivities_Click(object? sender, RoutedEventArgs e)
        {
            MainMenuPanel.IsVisible = false;
            ActivitiesPanel.IsVisible = true;
        }

        private void ShowEducation_Click(object? sender, RoutedEventArgs e)
        {
            MainMenuPanel.IsVisible = false;
            EducationPanel.IsVisible = true;
        }

        private void ShowRelationships_Click(object? sender, RoutedEventArgs e)
        {
            MainMenuPanel.IsVisible = false;
            RelationshipsPanel.IsVisible = true;
        }

        private void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            ActivitiesPanel.IsVisible = false;
            EducationPanel.IsVisible = false;
            RelationshipsPanel.IsVisible = false;
            MainMenuPanel.IsVisible = true;
        }

        // --- Events ---

        private void ShowEventPopup(Event evt)
        {
            EventPromptBlock.Text = evt.PromptText;
            EventChoicesPanel.Children.Clear();

            foreach (var choice in evt.Choices)
            {
                var btn = new Button
                {
                    Content = choice.Text,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center
                };
                btn.Click += (s, e) =>
                {
                    _engine.HandleEventChoice(choice);
                    EventOverlay.IsVisible = false;
                    UpdateUI();
                    CheckDeath();
                };
                EventChoicesPanel.Children.Add(btn);
            }

            EventOverlay.IsVisible = true;
        }

        private void RestartButton_Click(object? sender, RoutedEventArgs e)
        {
            GamePanel.IsVisible = false;
            SetupPanel.IsVisible = true;
            NameTextBox.Text = string.Empty;
            GenderComboBox.SelectedIndex = 0;
            _engine = new GameEngine();
            _engine.OnLog = Log;
            _engine.OnEventTriggered = ShowEventPopup;
            BackButton_Click(null, null);
        }
    }
}
