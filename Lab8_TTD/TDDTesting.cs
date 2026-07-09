using NUnit.Framework;
using ScriptsToTest.TDD;

namespace TDD
{
    public class TDDTesting
    {
        [Test]
        public void CalculateFinalDamage_NoModifiers_ReturnsBaseDamage()
        {
            int dmg = 50;
            DamageContext damageContext = new DamageContext();
            damageContext.OriginalDamage = dmg;
            damageContext.CurrentDamage = dmg;
            
            DamageCalculator calculatorUnderTest = new DamageCalculator();
            
            var res = calculatorUnderTest.CalculateFinalDamage(damageContext);
            
            Assert.That(res, Is.EqualTo(dmg));
        }

        [Test]
        public void CalculateFinalDamage_WithCritical_MultipliesDamage()
        {
            int dmg = 50;
            DamageContext damageContext = new DamageContext();
            damageContext.OriginalDamage = dmg;
            damageContext.CurrentDamage = dmg;
            damageContext.IsCritical = true;
            
            DamageCalculator calculatorUnderTest = new DamageCalculator();
            calculatorUnderTest = calculatorUnderTest.AddModifier(new CriticalStrikeModifier());
            
            var res = calculatorUnderTest.CalculateFinalDamage(damageContext);
            
            Assert.That(res, Is.EqualTo(dmg * damageContext.CritMultiplier));
        }
        
        [Test]
        public void CalculateFinalDamage_NoCritical_IgnoresMultiplication()
        {
            int dmg = 50;
            DamageContext damageContext = new DamageContext();
            damageContext.OriginalDamage = dmg;
            damageContext.CurrentDamage = dmg;
            damageContext.IsCritical = false;
            
            DamageCalculator calculatorUnderTest = new DamageCalculator();
            calculatorUnderTest = calculatorUnderTest.AddModifier(new CriticalStrikeModifier());
            
            var res = calculatorUnderTest.CalculateFinalDamage(damageContext);
            
            Assert.That(res, Is.EqualTo(dmg));
        }

        [Test]
        public void CalculateFinalDamage_WithArmor_ReturnsReducesDamage()
        {
            int dmg = 50;
            int armor = 30;
            DamageContext damageContext = new DamageContext();
            damageContext.OriginalDamage = dmg;
            damageContext.CurrentDamage = dmg;
            damageContext.IsCritical = false;
            damageContext.Armor = armor;
            
            DamageCalculator calculatorUnderTest = new DamageCalculator();
            calculatorUnderTest = calculatorUnderTest.AddModifier(new ArmorModifier());
            
            var res = calculatorUnderTest.CalculateFinalDamage(damageContext);
            
            Assert.That(res, Is.EqualTo(dmg - armor));
        }

        [Test]
        public void CalculateFinalDamage_ArmorExceedsDamage_ReturnsZero()
        {
            int dmg = 50;
            int armor = 80;
            DamageContext damageContext = new DamageContext();
            damageContext.OriginalDamage = dmg;
            damageContext.CurrentDamage = dmg;
            damageContext.IsCritical = false;
            damageContext.Armor = armor;
            
            DamageCalculator calculatorUnderTest = new DamageCalculator();
            calculatorUnderTest = calculatorUnderTest.AddModifier(new ArmorModifier());
            
            var res = calculatorUnderTest.CalculateFinalDamage(damageContext);
            
            Assert.That(res, Is.EqualTo(0));
        }

        [Test]
        public void CalculateFinalDamage_MatchingResistance_ReducesByPercentage()
        {
            int dmg = 50;
            DamageContext damageContext = new DamageContext();
            damageContext.OriginalDamage = dmg;
            damageContext.CurrentDamage = dmg;
            damageContext.IsCritical = false;
            damageContext.Type = DamageType.Cold;
            damageContext.Resistances.Add(DamageType.Cold, 0.5f);
            
            DamageCalculator calculatorUnderTest = new DamageCalculator();
            calculatorUnderTest = calculatorUnderTest.AddModifier(new ResistanceModifier());
            
            var res = calculatorUnderTest.CalculateFinalDamage(damageContext);
            
            Assert.That(res, Is.EqualTo(25));
        }

        [Test]
        public void CalculateFinalDamage_WrongResistance_DamageUnchanged()
        {
            int dmg = 50;
            DamageContext damageContext = new DamageContext();
            damageContext.OriginalDamage = dmg;
            damageContext.CurrentDamage = dmg;
            damageContext.IsCritical = false;
            damageContext.Type = DamageType.Cold;
            damageContext.Resistances.Add(DamageType.Fire, 0.5f);
            
            DamageCalculator calculatorUnderTest = new DamageCalculator();
            calculatorUnderTest = calculatorUnderTest.AddModifier(new ResistanceModifier());
            
            var res = calculatorUnderTest.CalculateFinalDamage(damageContext);
            
            Assert.That(res, Is.EqualTo(50));
        }

        [Test]
        public void CalculateFinalDamage_AllModifiers_PerfectOrder()
        {
            int dmg = 50;
            DamageContext damageContext = new DamageContext();
            damageContext.OriginalDamage = dmg;
            damageContext.CurrentDamage = dmg;
            damageContext.IsCritical = true;
            damageContext.Type = DamageType.Cold;
            damageContext.Resistances.Add(DamageType.Cold, 0.5f);
            damageContext.Armor = 20;
            
            DamageCalculator calculatorUnderTest = new DamageCalculator();
            calculatorUnderTest = calculatorUnderTest.AddModifier(new ResistanceModifier());
            calculatorUnderTest = calculatorUnderTest.AddModifier(new CriticalStrikeModifier());
            calculatorUnderTest = calculatorUnderTest.AddModifier(new ArmorModifier());
            
            var res = calculatorUnderTest.CalculateFinalDamage(damageContext);
            
            Assert.That(res, Is.EqualTo(30));
        }
    }
}