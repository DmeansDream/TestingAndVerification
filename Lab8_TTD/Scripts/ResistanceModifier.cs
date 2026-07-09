using System;

namespace ScriptsToTest.TDD
{
    public class ResistanceModifier : IDamageModifier
    {
        public int ExecutionOrder { get; set; } = 20;

        public void Process(DamageContext damageContext)
        {
            if (damageContext.Resistances.TryGetValue(damageContext.Type, out float resistancePercentage)) 
            {
                float effectiveResistance = Math.Min(1.0f, resistancePercentage);
                damageContext.CurrentDamage -= (damageContext.CurrentDamage * effectiveResistance);
            }
        }
    }
}