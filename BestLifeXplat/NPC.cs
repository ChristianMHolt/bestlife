using System;

namespace BestLifeXplat
{
    public class NPC
    {
        public string Name { get; set; } = string.Empty;
        public string RelationType { get; set; } = string.Empty;
        public int RelationshipStat { get; set; }
        public decimal Money { get; set; }

        public NPC(string name, string relationType, int relationshipStat)
        {
            Name = name;
            RelationType = relationType;
            RelationshipStat = relationshipStat;

            Random random = new Random();
            Money = (decimal)(random.NextDouble() * 99900 + 100);
        }

        public override string ToString()
        {
            return $"{Name} ({RelationType}) - Relationship: {RelationshipStat}%";
        }
    }
}
