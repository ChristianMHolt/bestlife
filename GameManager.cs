using System;

namespace BitLifeClone
{
    public class GameManager
    {
        private Player? _player;
        private Random _random;

        public GameManager()
        {
            _random = new Random();
        }

        public void InitializeGame()
        {
            Console.WriteLine("Welcome to BitLife Clone!");
            Console.Write("Enter your first name: ");
            string name = Console.ReadLine() ?? "Unknown";

            Console.WriteLine("Select your gender:");
            Console.WriteLine("1. Male");
            Console.WriteLine("2. Female");
            string genderChoice = Console.ReadLine() ?? "1";
            string gender = genderChoice == "1" ? "Male" : "Female";

            _player = new Player(name, gender);
            Console.WriteLine($"\nYou were born a {gender.ToLower()}. Your name is {name}.");
        }

        public void StartGame()
        {
            InitializeGame();

            if (_player == null) return;

            bool isPlaying = true;
            while (isPlaying && _player.Health > 0)
            {
                DisplayStatus();
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("1. Age Up");
                Console.WriteLine("2. Activities");
                Console.WriteLine("3. Quit");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        AgeUp();
                        break;
                    case "2":
                        ActivitiesMenu();
                        break;
                    case "3":
                        isPlaying = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }

            Console.WriteLine($"\nGame Over. You died at age {_player.Age}.");
        }

        private void DisplayStatus()
        {
            if (_player == null) return;
            Console.WriteLine("\n--------------------------------------------------");
            Console.WriteLine($"Name: {_player.Name} | Gender: {_player.Gender} | Age: {_player.Age}");
            Console.WriteLine($"Health: {_player.Health}% | Happiness: {_player.Happiness}% | Smarts: {_player.Smarts}% | Looks: {_player.Looks}%");
            Console.WriteLine($"Money: ${_player.Money}");
            Console.WriteLine("--------------------------------------------------");
        }

        private void AgeUp()
        {
            if (_player == null) return;
            _player.Age++;
            Console.WriteLine($"\nYou aged up to {_player.Age} years old.");

            // Basic random decay/increase
            _player.Happiness += _random.Next(-5, 6);
            _player.Health += _random.Next(-2, 3);

            ClampStats();

            if (_player.Health <= 0)
            {
                 Console.WriteLine("You died of natural causes.");
            }
        }

        private void ActivitiesMenu()
        {
            Console.WriteLine("\n--- Activities ---");
            Console.WriteLine("1. Go to the Gym");
            Console.WriteLine("2. Read a Book");
            Console.WriteLine("3. Go to Work");
            Console.WriteLine("4. Back");

            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    GoToGym();
                    break;
                case "2":
                    ReadBook();
                    break;
                case "3":
                    GoToWork();
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        private void GoToGym()
        {
            if (_player == null) return;
            Console.WriteLine("\nYou hit the gym.");
            _player.Health += _random.Next(1, 5);
            _player.Looks += _random.Next(1, 3);
            _player.Happiness += _random.Next(1, 4);
            ClampStats();
        }

        private void ReadBook()
        {
            if (_player == null) return;
            Console.WriteLine("\nYou read a book.");
            _player.Smarts += _random.Next(1, 5);
            _player.Happiness += _random.Next(0, 3);
            ClampStats();
        }

        private void GoToWork()
        {
            if (_player == null) return;
            if (_player.Age < 16)
            {
                Console.WriteLine("\nYou are too young to work!");
                return;
            }

            decimal earned = _random.Next(50, 200);
            Console.WriteLine($"\nYou went to work and earned ${earned}.");
            _player.Money += earned;
            _player.Happiness -= _random.Next(1, 5);
            ClampStats();
        }

        private void ClampStats()
        {
            if (_player == null) return;
            _player.Health = Math.Clamp(_player.Health, 0, 100);
            _player.Happiness = Math.Clamp(_player.Happiness, 0, 100);
            _player.Smarts = Math.Clamp(_player.Smarts, 0, 100);
            _player.Looks = Math.Clamp(_player.Looks, 0, 100);
        }
    }
}
