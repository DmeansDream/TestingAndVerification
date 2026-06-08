using UnityEngine;

public class PlayerData
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Damage { get; set; }

    public PlayerData()
    {
    }
    
    public PlayerData(string Name, int MaxHealth, int Health, int Damage)
    {
        ID = 0;
        this.Name = Name;
        this.MaxHealth = MaxHealth;
        this.Health = Health;
        this.Damage = Damage;
    }

}