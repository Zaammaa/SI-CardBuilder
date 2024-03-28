using Spirit_Island_Card_Generator.Classes.Effects.ConditionalEffects;
using Spirit_Island_Card_Generator.Classes.Effects.GlobalEffects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects
{
    //An effect with a simple amount
    public abstract class AmountEffect : Effect
    {
        public int amount {
            get {
                Type type = this.GetType();
                FieldInfo[] properties = type.GetFields().Where(p => Attribute.IsDefined(p, typeof(AmountValue))).ToArray();

                object value = properties[0].GetValue(this);
                return (int)value;
            }
            set {
                Type type = this.GetType();
                FieldInfo[] properties = type.GetFields().Where(p => Attribute.IsDefined(p, typeof(AmountValue))).ToArray();

                properties[0].SetValue(this, value);
            }
        }

        public abstract double effectStrength { get; }

        protected abstract Dictionary<int, double> ExtraAmountMultiplier { get; }
        public virtual int max { get { return ExtraAmountMultiplier.Count; } }
        public virtual int min { get { return 1; } }

        public override double CalculatePowerLevel()
        {
            double powerLevel = 0;
            for (int i = 1; i <= amount; i++)
            {
                if (ExtraAmountMultiplier.ContainsKey(i))
                    powerLevel += effectStrength * ExtraAmountMultiplier[i];
                else
                    powerLevel += effectStrength * ExtraAmountMultiplier.Last().Value;
            }

            return powerLevel;
        }

        public virtual Effect? IncreaseAmount()
        {
            if (effectStrength >= 0 && amount < max)
            {
                AmountEffect newEffect = (AmountEffect)Duplicate();
                newEffect.amount += 1;
                return newEffect;

            }
            else if (effectStrength < 0 && amount > min)
            {
                AmountEffect newEffect = (AmountEffect)Duplicate();
                newEffect.amount -= 1;
                return newEffect;
            }
            return null;
        }

        public virtual Effect? DecreaseAmount()
        {
            if (effectStrength >= 0 && amount > min)
            {
                AmountEffect newEffect = (AmountEffect)Duplicate();
                newEffect.amount -= 1;
                return newEffect;

            } else if (effectStrength < 0 && amount < max)
            {
                AmountEffect newEffect = (AmountEffect)Duplicate();
                newEffect.amount += 1;
                return newEffect;
            }
            return null;
        }
    }

    public class AmountValue : Attribute
    {

    }
}
