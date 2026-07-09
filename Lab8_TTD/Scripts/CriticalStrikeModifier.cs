namespace ScriptsToTest.TDD
{
    public class CriticalStrikeModifier : IDamageModifier
    {
        public int ExecutionOrder { get; set; } = 10;
        public void Process(DamageContext damageContext)
        {
            if (damageContext.IsCritical)
            {
                damageContext.CurrentDamage *= damageContext.CritMultiplier;
            }
        }
    }
}