using System.Collections.Generic;

namespace ScriptsToTest.TDD
{
    public class DamageContext
    {
        public float OriginalDamage { get; set; }
        public float CurrentDamage { get; set; }
        public DamageType Type { get; set; }
        public bool IsCritical { get; set; }
        public float CritMultiplier { get; set; } = 2.0f;
        
        public int Armor { get; set; }
        public Dictionary<DamageType, float> Resistances { get; set; } = new();
    }

    public enum DamageType
    {
        Poison,
        Acid,
        Cold,
        Fire
    }
}