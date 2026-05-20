using System;

namespace BitLifeClone
{
    public class NPC
    {
        public string Name { get; set; } = string.Empty;
        public string RelationType { get; set; } = string.Empty;
        public int RelationshipStat { get; set; }

        public NPC(string name, string relationType, int relationshipStat)
        {
            Name = name;
            RelationType = relationType;
            RelationshipStat = relationshipStat;
        }

        public override string ToString()
        {
            return $"{Name} ({RelationType}) - Relationship: {RelationshipStat}%";
        }
    }
}
