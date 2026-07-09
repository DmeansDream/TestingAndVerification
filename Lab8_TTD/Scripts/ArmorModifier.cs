namespace ScriptsToTest.TDD
{
    public class ArmorModifier : IDamageModifier
    {
        public int ExecutionOrder { get; set; } = 30;
        public void Process(DamageContext damageContext)
        {
            damageContext.CurrentDamage -= damageContext.Armor;
        }
    }
}