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
		
		public bool IsActivityDone(string activityName)
		{
			return _activitiesDoneThisYear.Contains(activityName);
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

            if (!CanDoActivity($"Spend Time With {npc.Name}")) return;

            Log($"You spent time with {npc.Name}.");
            npc.RelationshipStat += _random.Next(2, 8);
            npc.RelationshipStat = Math.Clamp(npc.RelationshipStat, 0, 100);

            Player.Happiness += _random.Next(1, 4);
            ClampStats();
        }

		public void AskForMoney(NPC npc)
		{
			if (Player == null) return;

			// Add this check to limit the activity to once per year per NPC
			if (!CanDoActivity($"Ask {npc.Name} for Money")) return;

			if (npc.Money <= 0)
			{
				Log($"{npc.Name} doesn't have any money to give you.");
				return;
			}

			int baseChance = npc.RelationshipStat;
			if (npc.RelationType == "Parent")
			{
				baseChance += 20;
			}
			else if (npc.RelationType == "Sibling")
			{
				baseChance += 10;
			}

			if (_random.Next(0, 100) < baseChance)
			{
				decimal amountToGive = Math.Min(npc.Money, (decimal)(_random.NextDouble() * 10000));

				if (Player.Age < 18)
				{
					amountToGive = Math.Min(amountToGive, (decimal)(_random.NextDouble() * 100 + 10)); // Children get way less
				}

				amountToGive = Math.Round(amountToGive, 2);

				if (amountToGive > 0)
				{
					npc.Money -= amountToGive;
					Player.Money += amountToGive;
					Log($"{npc.Name} gave you ${amountToGive}.");
				}
				else
				{
					Log($"{npc.Name} agreed to give you money, but didn't have enough to spare.");
				}
			}
			else
			{
				Log($"{npc.Name} refused to give you money.");
				npc.RelationshipStat -= _random.Next(1, 6);
				npc.RelationshipStat = Math.Clamp(npc.RelationshipStat, 0, 100);
			}
			ClampStats();
		}

        public ComplimentResult Compliment(NPC npc)
        {
            if (Player == null) return new ComplimentResult("You tried to compliment them, but you don't exist.", npc.RelationshipStat, 0);
			
			if (!CanDoActivity($"Compliment {npc.Name}")) 
			{
				return new ComplimentResult($"You already complimented {npc.Name} this year.", npc.RelationshipStat, 0);
			}

            int oldStat = npc.RelationshipStat;
            bool backfire = false;

            if (npc.RelationshipStat < 50)
            {
                backfire = _random.Next(0, 100) < 40; // 40% chance to backfire if relationship < 50%
            }
            else
            {
                backfire = _random.Next(0, 100) < 10; // 10% chance to backfire otherwise
            }

            string resultText = "";

            if (backfire)
            {
                int decrease = _random.Next(5, 31);
                npc.RelationshipStat -= decrease;
                resultText = $"You complimented {npc.Name}, but it backfired. They got offended and your relationship decreased.";
            }
            else
            {
                int increase = _random.Next(5, 16);
                npc.RelationshipStat += increase;
                resultText = $"You complimented {npc.Name}. They appreciated it and your relationship improved.";
            }

            npc.RelationshipStat = Math.Clamp(npc.RelationshipStat, 0, 100);
            int actualChange = npc.RelationshipStat - oldStat;
            Log(resultText);

            return new ComplimentResult(resultText, npc.RelationshipStat, actualChange);
        }

        public void ArgueWith(NPC npc)
		{
			if (Player == null) return;
			
			// Add this check
			if (!CanDoActivity($"Argue With {npc.Name}")) return;

			Log($"You argued with {npc.Name}.");
			npc.RelationshipStat -= _random.Next(10, 20);
			npc.RelationshipStat = Math.Clamp(npc.RelationshipStat, 0, 100);

			Player.Happiness -= _random.Next(5, 10);
			ClampStats();
		}
    }

    public class ComplimentResult
    {
        public string OutcomeText { get; set; }
        public int NewRelationshipStat { get; set; }
        public int RelationshipChange { get; set; }

        public ComplimentResult(string outcomeText, int newRelationshipStat, int relationshipChange)
        {
            OutcomeText = outcomeText;
            NewRelationshipStat = newRelationshipStat;
            RelationshipChange = relationshipChange;
        }
    }
}
