using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace BitLifeClone
{
    public partial class MainWindow : Window
    {
        private Player? _player;
        private Random _random;

        public MainWindow()
        {
            InitializeComponent();
            _random = new Random();
        }

        private void StartButton_Click(object? sender, RoutedEventArgs e)
        {
            string name = string.IsNullOrWhiteSpace(NameTextBox.Text) ? "Unknown" : NameTextBox.Text;
            string gender = GenderComboBox.SelectedIndex == 0 ? "Male" : "Female";

            _player = new Player(name, gender);

            SetupPanel.IsVisible = false;
            GamePanel.IsVisible = true;

            ActivityLog.Items.Clear();
            Log($"You were born a {gender.ToLower()}. Your name is {name}.");

            UpdateStatsUI();
            EnableActionButtons(true);
            RestartButton.IsVisible = false;
        }

        private void UpdateStatsUI()
        {
            if (_player == null) return;

            NameGenderAgeBlock.Text = $"Name: {_player.Name} | Gender: {_player.Gender} | Age: {_player.Age}";
            HealthBlock.Text = $"{_player.Health}%";
            HappinessBlock.Text = $"{_player.Happiness}%";
            SmartsBlock.Text = $"{_player.Smarts}%";
            LooksBlock.Text = $"{_player.Looks}%";
            MoneyBlock.Text = $"${_player.Money}";
        }

        private void Log(string message)
        {
            ActivityLog.Items.Add(message);
            ActivityLog.ScrollIntoView(ActivityLog.Items.Count - 1);
        }

        private void ClampStats()
        {
            if (_player == null) return;
            _player.Health = Math.Clamp(_player.Health, 0, 100);
            _player.Happiness = Math.Clamp(_player.Happiness, 0, 100);
            _player.Smarts = Math.Clamp(_player.Smarts, 0, 100);
            _player.Looks = Math.Clamp(_player.Looks, 0, 100);
        }

        private void CheckDeath()
        {
            if (_player == null) return;
            if (_player.Health <= 0)
            {
                Log("You died of natural causes.");
                Log($"Game Over. You died at age {_player.Age}.");
                EnableActionButtons(false);
                RestartButton.IsVisible = true;
            }
        }

        private void EnableActionButtons(bool enable)
        {
            AgeUpButton.IsEnabled = enable;
            GymButton.IsEnabled = enable;
            BookButton.IsEnabled = enable;
            WorkButton.IsEnabled = enable;
        }

        private void AgeUpButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_player == null || _player.Health <= 0) return;

            _player.Age++;
            Log($"You aged up to {_player.Age} years old.");

            _player.Happiness += _random.Next(-5, 6);
            _player.Health += _random.Next(-2, 3);

            ClampStats();
            UpdateStatsUI();
            CheckDeath();
        }

        private void GymButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_player == null || _player.Health <= 0) return;

            Log("You hit the gym.");
            _player.Health += _random.Next(1, 5);
            _player.Looks += _random.Next(1, 3);
            _player.Happiness += _random.Next(1, 4);

            ClampStats();
            UpdateStatsUI();
        }

        private void BookButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_player == null || _player.Health <= 0) return;

            Log("You read a book.");
            _player.Smarts += _random.Next(1, 5);
            _player.Happiness += _random.Next(0, 3);

            ClampStats();
            UpdateStatsUI();
        }

        private void WorkButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_player == null || _player.Health <= 0) return;

            if (_player.Age < 16)
            {
                Log("You are too young to work!");
                return;
            }

            decimal earned = _random.Next(50, 200);
            Log($"You went to work and earned ${earned}.");
            _player.Money += earned;
            _player.Happiness -= _random.Next(1, 5);

            ClampStats();
            UpdateStatsUI();
        }

        private void RestartButton_Click(object? sender, RoutedEventArgs e)
        {
            GamePanel.IsVisible = false;
            SetupPanel.IsVisible = true;
            NameTextBox.Text = string.Empty;
            GenderComboBox.SelectedIndex = 0;
            _player = null;
        }
    }
}
