namespace ScriptsToTest
{
    public class PlayerCharacter
    {
        public PlayerData data;

        public PlayerCharacter(string Name, int MaxHealth, int Health, int Damage)
        {
            data = new PlayerData(Name, MaxHealth, Health, Damage);
        }

        public void TakeDamage(int amount)
        {
            data.Health -= amount;
        }

        public void LevelUp(int hpIncrease, int dmgIncrease)
        {
            data.MaxHealth += hpIncrease;
            data.Damage += dmgIncrease;
        }
    }
}