using System;
using System.Collections.Generic;

namespace ScriptsToTest.TDD
{
    public class DamageCalculator
    {
        private readonly List<IDamageModifier> _modifiers = new List<IDamageModifier>();
        private bool _isSorted = false;

        public DamageCalculator AddModifier(IDamageModifier modifier)
        {
            _modifiers.Add(modifier);
            _isSorted = false;
            return this;
        }

        public float CalculateFinalDamage(DamageContext context)
        {
            if (!_isSorted)
            {
                _modifiers.Sort((a, b) => a.ExecutionOrder.CompareTo(b.ExecutionOrder));
                _isSorted = true;
            }

            foreach (var modifier in _modifiers)
            {
                modifier.Process(context);
            }

            return Math.Max(0f, context.CurrentDamage);
        }
    }
}