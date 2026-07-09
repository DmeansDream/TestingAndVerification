namespace ScriptsToTest.TDD
{
    public interface IDamageModifier
    {
        public int ExecutionOrder { get; set; }
        public void Process(DamageContext damageContext);
    }
}