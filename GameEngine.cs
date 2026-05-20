using System;
using System.Collections.Generic;

namespace BitLifeClone
{
    public class GameEngine
    {
        public Player? Player { get; private set; }
        private Random _random;
        public Action<string>? OnLog { get; set; }
        public EventSystem EventSystem { get; private set; }
        public Action<Event>? OnEventTriggered { get; set; }

        private HashSet<string> _activitiesDoneThisYear = new HashSet<string>();

        public GameEngine()
        {
            _random = new Random();
            EventSystem = new EventSystem();
        }

        public void StartGame(string name, string gender)
        {
            Player = new Player(name, gender);
            Log($"You were born a {gender.ToLower()}. Your name is {name}.");

            // Generate parents
            Player.Relationships.Add(new NPC("Mom", "Parent", _random.Next(50, 100)));
            Player.Relationships.Add(new NPC("Dad", "Parent", _random.Next(50, 100)));

            ClampStats();
        }

        public void Log(string message)
        {
            OnLog?.Invoke(message);
        }

        public void ClampStats()
        {
            if (Player == null) return;
            Player.Health = Math.Clamp(Player.Health, 0, 100);
            Player.Happiness = Math.Clamp(Player.Happiness, 0, 100);
            Player.Smarts = Math.Clamp(Player.Smarts, 0, 100);
            Player.Looks = Math.Clamp(Player.Looks, 0, 100);
        }

        public bool CanDoActivity(string activityName)
        {
            if (_activitiesDoneThisYear.Contains(activityName))
            {
                Log($"You have already done {activityName} this year.");
                return false;
            }
            _activitiesDoneThisYear.Add(activityName);
            return true;
        }

        public bool CheckDeath()
        {
            if (Player == null) return false;
            if (Player.Health <= 0)
            {
                Log("You died of natural causes.");
                Log($"Game Over. You died at age {Player.Age}.");
                return true;
            }
            return false;
        }

        public void AgeUp()
        {
            if (Player == null || Player.Health <= 0) return;

            Player.Age++;
            _activitiesDoneThisYear.Clear();
            Log($"You aged up to {Player.Age} years old.");

            Player.Happiness += _random.Next(-5, 6);
            Player.Health += _random.Next(-2, 3);

            ClampStats();

            CheckForEvent();
        }

        private void CheckForEvent()
        {
            var randomEvent = EventSystem.GetRandomEvent();
            if (randomEvent != null)
            {
                OnEventTriggered?.Invoke(randomEvent);
            }
        }

        public void HandleEventChoice(EventChoice choice)
        {
            if (Player == null) return;

            Log($"You chose: {choice.Text}");

            foreach (var statChange in choice.StatChanges)
            {
                switch (statChange.Key)
                {
                    case "Health": Player.Health += statChange.Value; break;
                    case "Happiness": Player.Happiness += statChange.Value; break;
                    case "Smarts": Player.Smarts += statChange.Value; break;
                    case "Looks": Player.Looks += statChange.Value; break;
                    case "Money": Player.Money += statChange.Value; break;
                }
            }

            ClampStats();
        }

        public void GoToGym()
        {
            if (Player == null || Player.Health <= 0) return;

            if (Player.Age < 12)
            {
                Log("You are too young to hit the gym. Wait until you are 12.");
                return;
            }

            if (!CanDoActivity("the Gym")) return;

            Log("You hit the gym.");
            Player.Health += _random.Next(1, 5);
            Player.Looks += _random.Next(1, 3);
            Player.Happiness += _random.Next(1, 4);

            ClampStats();
        }

        public void ReadBook()
        {
            if (Player == null || Player.Health <= 0) return;

            if (!CanDoActivity("Reading")) return;

            Log("You read a book.");
            Player.Smarts += _random.Next(1, 5);
            Player.Happiness += _random.Next(0, 3);

            ClampStats();
        }

        public void Diet()
        {
            if (Player == null || Player.Health <= 0) return;
            if (!CanDoActivity("Dieting")) return;
            Log("You went on a diet.");
            Player.Health += _random.Next(1, 4);
            Player.Looks += _random.Next(0, 3);
            Player.Happiness -= _random.Next(0, 3);
            ClampStats();
        }

        public void Garden()
        {
            if (Player == null || Player.Health <= 0) return;
            if (!CanDoActivity("Gardening")) return;
            Log("You did some gardening.");
            Player.Happiness += _random.Next(1, 4);
            Player.Health += _random.Next(0, 2);
            ClampStats();
        }

        public void VisitLibrary()
        {
            if (Player == null || Player.Health <= 0) return;
            if (!CanDoActivity("Visiting the Library")) return;
            Log("You visited the library.");
            Player.Smarts += _random.Next(1, 4);
            Player.Happiness += _random.Next(0, 3);
            ClampStats();
        }

        public void Meditate()
        {
            if (Player == null || Player.Health <= 0) return;
            if (!CanDoActivity("Meditating")) return;
            Log("You meditated.");
            Player.Happiness += _random.Next(2, 5);
            Player.Health += _random.Next(0, 2);
            ClampStats();
        }

        public void GoForWalk()
        {
            if (Player == null || Player.Health <= 0) return;
            if (!CanDoActivity("Going for a Walk")) return;
            Log("You went for a walk.");
            Player.Health += _random.Next(1, 3);
            Player.Happiness += _random.Next(1, 3);
            ClampStats();
        }

        public void EnrollEducation(string level)
        {
            if (Player == null) return;
            if (level == "Elementary" && Player.Age >= 5 && Player.CurrentEducation == "None")
            {
                Player.CurrentEducation = "Elementary";
                Log("You enrolled in Elementary School.");
            }
            else if (level == "High School" && Player.Age >= 14 && Player.CurrentEducation == "Elementary")
            {
                Player.CurrentEducation = "High School";
                Log("You enrolled in High School.");
            }
            else if (level == "University" && Player.Age >= 18 && Player.CurrentEducation == "High School")
            {
                Player.CurrentEducation = "University";
                Log("You enrolled in University.");
            }
            else
            {
                Log($"You cannot enroll in {level} at this time.");
            }
        }

        public void Study()
        {
            if (Player == null || Player.CurrentEducation == "None" || Player.CurrentEducation == "Graduated") return;

            Log("You studied hard for school.");
            Player.Smarts += _random.Next(2, 6);
            Player.Happiness -= _random.Next(1, 4);

            ClampStats();
        }

        public void Graduate()
        {
            if (Player == null || Player.CurrentEducation == "None" || Player.CurrentEducation == "Graduated") return;

            Log($"You graduated from {Player.CurrentEducation}!");
            Player.Happiness += 10;
            Player.Smarts += 5;
            Player.CurrentEducation = "Graduated";

            ClampStats();
        }

        public void ApplyForJob(Job job)
        {
            if (Player == null) return;

            if (Player.Age < 16)
            {
                Log("You are too young to apply for a job!");
                return;
            }

            if (job.RequiredEducation == "University" && Player.CurrentEducation != "Graduated")
            {
                Log($"You don't have the required education ({job.RequiredEducation}) for {job.Title}.");
                return;
            }

            if (Player.Smarts > 40)
            {
                Player.CurrentJob = job;
                Log($"You got the job as a {job.Title}!");
            }
            else
            {
                Log($"You were rejected for the {job.Title} position.");
            }
        }

        public void Work()
        {
            if (Player == null) return;
            if (Player.CurrentJob == null)
            {
                Log("You don't have a job to go to!");
                return;
            }

            Log($"You went to work as a {Player.CurrentJob.Title} and earned ${Player.CurrentJob.Salary}.");
            Player.Money += Player.CurrentJob.Salary;
            Player.Happiness -= _random.Next(1, 5);

            // Chance to be promoted or fired
            if (_random.Next(0, 100) < 5 && Player.Smarts > 70 && Player.Health > 50)
            {
                Player.CurrentJob.Salary += 1000;
                Player.CurrentJob.Title = $"Senior {Player.CurrentJob.Title}";
                Log($"You were promoted to {Player.CurrentJob.Title}!");
                Player.Happiness += 20;
            }
            else if (_random.Next(0, 100) < 2 && (Player.Smarts < 30 || Player.Health < 30))
            {
                Log($"You were fired from your job as a {Player.CurrentJob.Title}!");
                Player.CurrentJob = null;
                Player.Happiness -= 30;
            }

            ClampStats();
        }

        public void SpendTimeWith(NPC npc)
        {
            if (Player == null) return;

            Log($"You spent time with {npc.Name}.");
            npc.RelationshipStat += _random.Next(2, 8);
            npc.RelationshipStat = Math.Clamp(npc.RelationshipStat, 0, 100);

            Player.Happiness += _random.Next(1, 4);
            ClampStats();
        }

        public void ArgueWith(NPC npc)
        {
            if (Player == null) return;

            Log($"You argued with {npc.Name}.");
            npc.RelationshipStat -= _random.Next(10, 20);
            npc.RelationshipStat = Math.Clamp(npc.RelationshipStat, 0, 100);

            Player.Happiness -= _random.Next(5, 10);
            ClampStats();
        }
    }
}
