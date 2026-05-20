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
            HappinessBlock.Text = $"{_engine.Player.Happiness}%";
            SmartsBlock.Text = $"{_engine.Player.Smarts}%";
            LooksBlock.Text = $"{_engine.Player.Looks}%";
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
        }
    }
}
