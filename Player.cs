using System;

namespace BitLifeClone
{
    public class Player
    {
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public int Health { get; set; }
        public int Happiness { get; set; }
        public int Smarts { get; set; }
        public int Looks { get; set; }
        public decimal Money { get; set; }
        public string CurrentEducation { get; set; } = "None";
        public Job? CurrentJob { get; set; }
        public System.Collections.Generic.List<NPC> Relationships { get; set; } = new System.Collections.Generic.List<NPC>();

        public Player(string name, string gender)
        {
            Name = name;
            Gender = gender;
            Age = 0;

            Random random = new Random();
            Health = random.Next(70, 101);
            Happiness = random.Next(70, 101);
            Smarts = random.Next(20, 101);
            Looks = random.Next(20, 101);
            Money = 0;
        }
    }
}
